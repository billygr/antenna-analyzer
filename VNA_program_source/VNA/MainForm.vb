'   By Norbert Redeker DG7EAO

'	Basiert auf Code von Beric Dunn K6BEZ - Thrifty Antenna Sweeper

'   PC Software to go with the $20 Antenna Analyser presented at Pacificon 2013 and hosted at
'   http://hamstack.com/project_home.html
'
'
' 	Erstellt mit SharpDevelop.
' 	Benutzer: Norbert
' 	Datum: 23.02.2014
' 	Zeit: 15:13
' 
' 	Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
'





Imports ZedGraph
Imports ZedGraph.PointPair
Imports System.Threading
Imports System.Globalization



Public Partial Class MainForm
	
	Dim myPane As ZedGraph.GraphPane
	
	Dim fStart As Double    'Start Frequency for sweep
	Dim fStop As Double     'Stop Frequency for sweep
	
	Dim BandMarkers(3) As Double  'Frequencies for band markers - used to hold top, middle and bottom of each band
	Dim MarkerFreq As Double      'Freq of user defined marker
	Dim SweepFreqs(1001) As Double 'List of VSWR values from sweep
	Dim SweepSWR(1001) As Double   'List of frequencies from sweep
	Dim bSerialOpen As Boolean    'True if serial comms are open
	Dim current_index As Long
	Dim numSteps As Integer
	Dim minPoint As Double
	Dim minPointFreq As Double
	Dim TimeOutTimer As ULong
	Dim bSweepParamsChanged As Boolean
	
	Dim CommPorts As Array
	Dim bSweeping As Boolean
	Dim bNeedsUpdate As Boolean
	Dim Zoom As Integer = 3
	Dim MaxSwr As Double = 10
	Dim FreqSwr As String = "MHz   SWR"
	
	
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'
		' TODO : Add constructor code after InitializeComponents
		'
	End Sub
	
	
	
	Sub MainFormLoad(sender As Object, e As EventArgs)
		
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("de")		
		
		Dim i As Integer
		'Set up default form values
		txtStartFreq.Text = "1"
		fStart = Val(txtStartFreq.Text)
		txtStopFreq.Text = "30"
		fStop = Val(txtStopFreq.Text)
		txtNumSteps.Text = "101"
		chkContSweep.Checked = False
		bSerialOpen = False
		trackBar1.Value = 10
		lblMaxSwr.Text = MaxSwr
		ComboBandSelect.SelectedIndex = 10
		
		'Get list of available serial ports
		CommPorts = IO.Ports.SerialPort.GetPortNames()
		
		'Populate a comboBox with these names
		For i = 0 To UBound(CommPorts)
			cmbPort.Items.Add(CommPorts(i))
		Next
		'If there are any com ports in the list then select the first one.
		If cmbPort.Items.Count > 0 Then
			cmbPort.Text = cmbPort.Items.Item(0)
		End If
		'Declare that the sweep params have changed and will need to be sent before the sweep
		bSweepParamsChanged = True
		
		CreateGraph(zg1)
		'SetSize()
		
	End Sub
	
	
	
	
	Private Sub CreateGraph(ByVal zgc As ZedGraphControl)
		
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("de")
		
		' Dim myPane As GraphPane = zgc.GraphPane
		myPane = zg1.GraphPane
		myPane.CurveList.Clear()
		
		Thread.Sleep(200)
		
			
		' Set the titles and axis labels
		myPane.Title.Text = "SWR Verlauf"
		myPane.Title.Text = FreqSwr
		myPane.Title.FontSpec.Size= 24
		myPane.XAxis.Title.Text = "Frequenz"
		myPane.YAxis.Title.Text = "SWR"  
		
		zg1.GraphPane.XAxis.Scale.Min = txtStartFreq.text
		zg1.GraphPane.XAxis.Scale.Max = txtStopFreq.text
		zg1.GraphPane.YAxis.Scale.Min = 1
		zg1.GraphPane.YAxis.Scale.Max = MaxSwr
		zg1.GraphPane.YAxis.MajorGrid.IsVisible = True
		
		' Make up some data points from the Sine function
		Dim list1 = New PointPairList()
		Dim list2 = New PointPairList()
		
		' Zeichne Graph	SWR
		For i = 0 To numSteps-1		
		   If SweepSWR(i) > 0 Then				
				list1.Add(SweepFreqs(i) , SweepSWR(i))
		   End If
		Next
		
		
		' Zeichne Linie beim niedrigsten SWR
		Dim j As Integer
		Dim min As Double = 1000
		
		For i = 0 To numSteps-1
			
			If SweepSWR(i) < min And SweepSWR(i) > 0 Then
				min = SweepSWR(i)
				j = i
			End If
			
		Next      
		
		list2.Add(SweepFreqs(j) , 1)
		list2.Add(SweepFreqs(j) , 100)
		
		
		
		Dim myCurve1 As LineItem = myPane.AddCurve _
			("SWR", list1, Color.Red, SymbolType.None)
		myCurve1.Line.Width = 2
		
		Dim myCurve2 As LineItem = myPane.AddCurve _
			("Min SWR", list2, Color.Blue, SymbolType.None)
		myCurve2.Line.Width = 1
		
		' Fill the axis background with a color gradient
		myPane.Chart.Fill = New Fill(Color.White, Color.LightGoldenrodYellow, 45.0F)
		
		' Fill the pane background with a color gradient
		myPane.Fill = New Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0F)
		
		' Calculate the Axis Scale Ranges
		zgc.AxisChange()
		
		
	End Sub
	
	
	
	Private Sub doSweep()	
		
		'If the serial port is not open then don't do the sweep
		If SerialPort1.IsOpen = False Then Return
		
		bSweeping = True
		numSteps = Val(txtNumSteps.Text) + 1
		
		'More than 1000 steps takes a while
		If numSteps > 1001 Then
			numSteps = 1001
			txtNumSteps.Text = numSteps
		End If
		
		
		'Get sweep params from form
				
		'Formatiere "," in "."
		
		Dim fStartstr As String = txtStartFreq.Text
		fStartstr = Replace(fStartstr, ",", ".")
		Dim fStopstr As String = txtStopFreq.Text
		fStopstr = Replace(fStopstr, ",", ".")
		
		fStart = Val(fStartstr)
		fStop = Val(fStopstr)
		
			
		
		'Serial Comms can be risky, so trap the function in a try
		Try
			If bSweepParamsChanged = True Then
				
				'If the parameters have changed since the last sweep then send the new ones.
				
				SerialPort1.WriteLine(Int(fStart * 1000000) & "A" & vbLf) 'Set start frequency
				System.Threading.Thread.Sleep(100)
				SerialPort1.WriteLine(Int(fStop * 1000000) & "B" & vbLf) 'Set stop frequency
				System.Threading.Thread.Sleep(100)
				SerialPort1.WriteLine(numSteps & "N" & vbLf)  'Set number of steps
				System.Threading.Thread.Sleep(100)
				'Now sweep data has been resent, clear the flag
				bSweepParamsChanged = False
			End If
			'Start the sweep
			SerialPort1.WriteLine("S" & vbLf)
			
			
		Catch ex As TimeoutException
			MessageBox.Show("Timeout")
			SerialPort1.Close()
			bSerialOpen = False
		Catch ex As Exception
			MessageBox.Show("Failed to send config data to Sweeper Hardware")
			SerialPort1.Close()
			bSerialOpen = False
		End Try
		TimeOutTimer = TimeOfDay.Millisecond
	End Sub
	
	
	
	Private Sub SerialPort1_DataReceived(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
		'The return data from the PIC/Arduino is handled by a DataReceived listener so the GUI can get on with its stuff		
		
		Dim sString As String
		Dim params() As String
		Dim VSWR As Double
		Dim currentFreq As Double
		
		
		'Read line of data
		sString = SerialPort1.ReadLine
		
		
		If TimeOfDay.Millisecond - TimeOutTimer > 2000 Then
			sString = "End"
		End If
		
		If sString.Contains("End") Then
			'If the line received contains 'End' then the sweep has finished
			' Plot the data received so far
			
			
			' Make Chart               
			CreateGraph(zg1)
			
			
			'Reset sweeping parameters ready for the next sweep
			bSweeping = False
			numSteps = current_index
			
			current_index = 0
		Else
			'Data line - extract Frequency and VSWR values and store them in the sweep arrays
			params = sString.Split(",")
			If params.Length > 2 Then
				VSWR = Val(params(2)) / 1000
				
				' *** eingefügt zum Ausblenden von negativen Zahlen
				If VSWR < 1 Then VSWR = 999
				
				currentFreq = Val(params(0)) / 1000000
				
				'Store the received data in the sweep arrays
				SweepFreqs(current_index) = currentFreq
				SweepSWR(current_index) = VSWR
				'Set the index ready for the next sweep
				current_index += 1
				
			End If
		End If
	End Sub
	
	
	
	
	
	Sub BtnSweepClick(sender As Object, e As EventArgs)
		'Start sweeping
		btnSweep.Text = "Sweeping"
		
		Me.Refresh
		doSweep()
		'If Continuous sweeps are requested then keep sweeping
		While chkContSweep.Checked = True
			If bSweeping = False Then
				doSweep()
				
			End If
		End While
		
		btnSweep.Text = "Sweep"
		
		Me.Refresh
	End Sub
	
	
	
	
	Sub btnConnectClick(sender As Object, e As EventArgs)
		
		'Connect to the Serial Port
		
		If cmbPort.Text = "" Then
			' No port selected, so exit the routine
			Return
		End If
		'Set parameters for serial port
		With SerialPort1
			.BaudRate = 57600
			.PortName = cmbPort.Text
			.DataBits = 8
			.Parity = IO.Ports.Parity.None
			.StopBits = IO.Ports.StopBits.One
			.RtsEnable = True 'Needed for Arduino Micro. PIC doesn't mind
			.DtrEnable = True 'Needed for Arduino Micro. PIC doesn't mind
		End With
		
		'Hide the risky serial port behaviour in a Try statement
		Try
			SerialPort1.Open()
			bSerialOpen = True
		Catch ex As Exception
			SerialPort1.Close()
			MessageBox.Show("Serial port failed to open")
			bSerialOpen = False
		End Try
		'If the port opened then send a single character to trigger the auto-sweeping version of the hardware into computer controlled mode
		'  before bombarding it with sweep data
		If SerialPort1.IsOpen Then
			'Send '.' char, which has no meaning to the sweep controller MCU, but will catch the attention.
			SerialPort1.WriteLine(".")
			'Sleep for 1 second to allow a sweep to finish
			System.Threading.Thread.Sleep(1000)
			'Now trigger a sweep
			doSweep()
			
		End If	
		
	End Sub
	
	
	
	
	Sub Timer1Tick(sender As Object, e As EventArgs)
		
		If chkContSweep.Checked = True And bSweeping = False Then
			'btnSweep.Text = "Sweeping"
			doSweep()
			'btnSweep.Text = "Sweep"
			
		End If		
	End Sub
	
	
	
	Sub Timer2Tick(sender As Object, e As EventArgs)
		'A 100 ms timer to do some housekeeping.
		' Mainly to get around the threading behaviour of the serial istener
		
		zg1.refresh 
		
		try
			ProgressBar1.Minimum = 1
			ProgressBar1.Maximum = txtNumSteps.Text
			ProgressBar1.Value =  current_index+1
		Catch
		End Try
		
		
		'Ausgabe von Freq und Min SWR
		Dim min As Double = 1000
		Dim j As integer      
		
		For i = 0 To numSteps-1
			
			If SweepSWR(i) < min And SweepSWR(i) > 0 Then
				min = SweepSWR(i)
				j = i
			End If
			
		Next      
		
		If SweepFreqs(j) > 0 then   
		FreqSwr = Math.Round(SweepFreqs(j),3).ToString + " MHz   1 : " + Math.Round(SweepSWR(j),2).ToString
		End If
		
		MinPointFreq = SweepFreqs(j)
		
		
		If bNeedsUpdate = True Then
			'Update the VSWR and min frequency labels in the GUI
			
			bNeedsUpdate = False
		End If
		
		If SerialPort1.IsOpen = True Then
			'If the serial port is open then diable the connect button and enable the sweep buttons
			btnConnect.Enabled = False
			btnSweep.Enabled = True
			
			btnZoom.Enabled = True
			btnZoom1.Enabled = True
		Else
			'If the serial port is not open the enable the connect button and disable the sweep buttons
			btnConnect.Enabled = True
			btnSweep.Enabled = False
			btnZoom.Enabled = False
			BtnZoom1.Enabled = False
			chkContSweep.Checked = False
		End If
	End Sub
	
	
	Sub BtnDisconnectClick(sender As Object, e As EventArgs)
		'Disconnect
		SerialPort1.close
		Application.Restart
	End Sub
	
	
	
	Sub BtnRescanClick(sender As Object, e As EventArgs)
		'Scan for the serial port list and populate the combo box
		
		CommPorts = IO.Ports.SerialPort.GetPortNames()
		
		cmbPort.Items.Clear()
		For i = 0 To UBound(CommPorts)
			cmbPort.Items.Add(CommPorts(i))
		Next
		If UBound(CommPorts) > 0 Then
			cmbPort.Text = cmbPort.Items.Item(0)
		Else
			cmbPort.Text = ""
		End If
	End Sub
	
	
	
	
	
	Sub TxtStartFreqTextChanged(sender As Object, e As EventArgs)
		'The frequency in the text box has been changed, so set the flag to say that the sweep parameters need changing
		bSweepParamsChanged = True
	End Sub
	
	Sub TxtStopFreqTextChanged(sender As Object, e As EventArgs)
		'The frequency in the text box has been changed, so set the flag to say that the sweep parameters need changing
		bSweepParamsChanged = True
	End Sub	
	
	
	Sub TxtNumStepsTextChanged(sender As Object, e As EventArgs)
		'The frequency in the text box has been changed, so set the flag to say that the sweep parameters need changing
		bSweepParamsChanged = True
	End Sub
	
	
	
	Sub TrackBar1Scroll(sender As Object, e As EventArgs)
		MaxSwr = TrackBar1.Value
		lblMaxSwr.Text = TrackBar1.Value
	End Sub
	
	
	Sub BtnZoomClick(sender As Object, e As EventArgs)
		'Zoom Out
		
		Zoom = Zoom - 1
		If Zoom = 0 Then Zoom = 1
		
		Dim z As Double = Math.Round(Val(MinPointFreq),0)
		Dim y1, y2 As double
		
		Y1 = (z - Zoom * 1)
		y2 = (z + Zoom * 1)
		
		If y1 < 1 Then y1 = 1
		If y2 < 1 Then y2 = 1
		
		
		txtStartFreq.Text = y1
		txtStopFreq.Text =  y2
		
		If chkContSweep.Checked = False Then
			doSweep()
		End If
	End Sub
	
	
	Sub BtnZoom1Click(sender As Object, e As EventArgs)
		'Zoom Out
		
		Zoom = Zoom + 1        
		
		Dim z As Double = Math.Round(Val(MinPointFreq),0)
		Dim y1, y2 As double
		
		Y1 = (z - Zoom * 1)
		y2 = (z + Zoom * 1)
		
		If y1 < 1 Then y1 = 1
		If y2 < 1 Then y2 = 1
		
		txtStartFreq.Text = y1
		txtStopFreq.Text =  y2       
		
		If chkContSweep.Checked = False Then
			doSweep()
		End If
	End Sub
	
	Sub ComboBandSelectSelectedIndexChanged(sender As Object, e As EventArgs)
		'A different band has been selected so change the sweep parameters to reflect it
        '  Start Freq and stop freq are set to extend beyond the end of the band
        '  BandMarkers is a list of fixed markers showing the top, bottom and CW/Voice switch-over frequency

        Select Case ComboBandSelect.SelectedIndex
            Case 0 & vbLf '160m
                txtStartFreq.Text = "1,7"
                txtStopFreq.Text = "2,1"
                
            Case 1 '80m
                txtStartFreq.Text = "3,4"
                txtStopFreq.Text = "4,1"
                
            Case 2 '60m
                txtStartFreq.Text = "5,2"
                txtStopFreq.Text = "5,5"
                
            Case 3 '40m
                txtStartFreq.Text = "6,9"
                txtStopFreq.Text = "7,4"
                
            Case 4 '30m
                txtStartFreq.Text = "10"
                txtStopFreq.Text = "10,25"
                
            Case 5 '20m
                txtStartFreq.Text = "13,9"
                txtStopFreq.Text = "14,45"
                
            Case 6 '17m
                txtStartFreq.Text = "18"
                txtStopFreq.Text = "18,2"
                
            Case 7 '15m
                txtStartFreq.Text = "20,9"
                txtStopFreq.Text = "21,55"
                
            Case 8 '12m
                txtStartFreq.Text = "24,8"
                txtStopFreq.Text = "25"
                
            Case 9 '10m
                txtStartFreq.Text = "27,9"
                txtStopFreq.Text = "29,8"                
                
            Case 10 'Full sweep 1-30 MHz
                txtStartFreq.Text = "1"
                txtStopFreq.Text = "30"
                
        End Select
        'Parameters have been changed, so set the flag so they are rewritten
        bSweepParamsChanged = True
        
                   
	End Sub
End Class
