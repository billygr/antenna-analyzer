'
' Erstellt mit SharpDevelop.
' Benutzer: Norbert
' Datum: 23.02.2014
' Zeit: 15:13
' 
' Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
'
Partial Class MainForm
	Inherits System.Windows.Forms.Form
	
	''' <summary>
	''' Designer variable used to keep track of non-visual components.
	''' </summary>
	Private components As System.ComponentModel.IContainer
	
	''' <summary>
	''' Disposes resources used by the form.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing Then
			If components IsNot Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(disposing)
	End Sub
	
	''' <summary>
	''' This method is required for Windows Forms designer support.
	''' Do not change the method contents inside the source code editor. The Forms designer might
	''' not be able to load this method if it was changed manually.
	''' </summary>
	Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Me.btnConnect = New System.Windows.Forms.Button()
		Me.zg1 = New ZedGraph.ZedGraphControl()
		Me.cmbPort = New System.Windows.Forms.ComboBox()
		Me.txtStartFreq = New System.Windows.Forms.TextBox()
		Me.txtStopFreq = New System.Windows.Forms.TextBox()
		Me.txtNumSteps = New System.Windows.Forms.TextBox()
		Me.chkContSweep = New System.Windows.Forms.CheckBox()
		Me.serialPort1 = New System.IO.Ports.SerialPort(Me.components)
		Me.timer2 = New System.Windows.Forms.Timer(Me.components)
		Me.btnSweep = New System.Windows.Forms.Button()
		Me.BtnZoom = New System.Windows.Forms.Button()
		Me.BtnZoom1 = New System.Windows.Forms.Button()
		Me.BtnDisconnect = New System.Windows.Forms.Button()
		Me.BtnRescan = New System.Windows.Forms.Button()
		Me.timer1 = New System.Windows.Forms.Timer(Me.components)
		Me.progressBar1 = New System.Windows.Forms.ProgressBar()
		Me.lblPort = New System.Windows.Forms.Label()
		Me.lblStart = New System.Windows.Forms.Label()
		Me.LblStop = New System.Windows.Forms.Label()
		Me.lblSteps = New System.Windows.Forms.Label()
		Me.trackBar1 = New System.Windows.Forms.TrackBar()
		Me.lblMaxSwr = New System.Windows.Forms.Label()
		Me.lblMaxSwr1 = New System.Windows.Forms.Label()
		Me.lblMaxSwr2 = New System.Windows.Forms.Label()
		Me.lblMaxSwr3 = New System.Windows.Forms.Label()
		Me.ComboBandSelect = New System.Windows.Forms.ComboBox()
		Me.lblbands = New System.Windows.Forms.Label()
		CType(Me.trackBar1,System.ComponentModel.ISupportInitialize).BeginInit
		Me.SuspendLayout
		'
		'btnConnect
		'
		Me.btnConnect.Location = New System.Drawing.Point(580, 432)
		Me.btnConnect.Name = "btnConnect"
		Me.btnConnect.Size = New System.Drawing.Size(75, 23)
		Me.btnConnect.TabIndex = 20
		Me.btnConnect.Text = "Connect"
		Me.btnConnect.UseVisualStyleBackColor = true
		AddHandler Me.btnConnect.Click, AddressOf Me.btnConnectClick
		'
		'zg1
		'
		Me.zg1.Location = New System.Drawing.Point(5, 7)
		Me.zg1.Name = "zg1"
		Me.zg1.ScrollGrace = 0R
		Me.zg1.ScrollMaxX = 0R
		Me.zg1.ScrollMaxY = 0R
		Me.zg1.ScrollMaxY2 = 0R
		Me.zg1.ScrollMinX = 0R
		Me.zg1.ScrollMinY = 0R
		Me.zg1.ScrollMinY2 = 0R
		Me.zg1.Size = New System.Drawing.Size(1006, 392)
		Me.zg1.TabIndex = 1
		Me.zg1.TabStop = false
		'
		'cmbPort
		'
		Me.cmbPort.FormattingEnabled = true
		Me.cmbPort.Location = New System.Drawing.Point(280, 433)
		Me.cmbPort.Name = "cmbPort"
		Me.cmbPort.Size = New System.Drawing.Size(70, 21)
		Me.cmbPort.TabIndex = 15
		'
		'txtStartFreq
		'
		Me.txtStartFreq.Location = New System.Drawing.Point(15, 434)
		Me.txtStartFreq.Name = "txtStartFreq"
		Me.txtStartFreq.Size = New System.Drawing.Size(60, 20)
		Me.txtStartFreq.TabIndex = 1
		AddHandler Me.txtStartFreq.TextChanged, AddressOf Me.TxtStartFreqTextChanged
		'
		'txtStopFreq
		'
		Me.txtStopFreq.Location = New System.Drawing.Point(100, 434)
		Me.txtStopFreq.Name = "txtStopFreq"
		Me.txtStopFreq.Size = New System.Drawing.Size(60, 20)
		Me.txtStopFreq.TabIndex = 5
		AddHandler Me.txtStopFreq.TextChanged, AddressOf Me.TxtStopFreqTextChanged
		'
		'txtNumSteps
		'
		Me.txtNumSteps.Location = New System.Drawing.Point(190, 434)
		Me.txtNumSteps.Name = "txtNumSteps"
		Me.txtNumSteps.Size = New System.Drawing.Size(60, 20)
		Me.txtNumSteps.TabIndex = 10
		AddHandler Me.txtNumSteps.TextChanged, AddressOf Me.TxtNumStepsTextChanged
		'
		'chkContSweep
		'
		Me.chkContSweep.Location = New System.Drawing.Point(427, 472)
		Me.chkContSweep.Name = "chkContSweep"
		Me.chkContSweep.Size = New System.Drawing.Size(100, 18)
		Me.chkContSweep.TabIndex = 18
		Me.chkContSweep.Text = "Dauer Sweep"
		Me.chkContSweep.UseVisualStyleBackColor = true
		'
		'timer2
		'
		Me.timer2.Enabled = true
		AddHandler Me.timer2.Tick, AddressOf Me.Timer2Tick
		'
		'btnSweep
		'
		Me.btnSweep.Font = New System.Drawing.Font("Microsoft Sans Serif", 14!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
		Me.btnSweep.Location = New System.Drawing.Point(427, 426)
		Me.btnSweep.Name = "btnSweep"
		Me.btnSweep.Size = New System.Drawing.Size(84, 34)
		Me.btnSweep.TabIndex = 17
		Me.btnSweep.Text = "Sweep"
		Me.btnSweep.UseVisualStyleBackColor = true
		AddHandler Me.btnSweep.Click, AddressOf Me.BtnSweepClick
		'
		'BtnZoom
		'
		Me.BtnZoom.Location = New System.Drawing.Point(580, 468)
		Me.BtnZoom.Name = "BtnZoom"
		Me.BtnZoom.Size = New System.Drawing.Size(75, 23)
		Me.BtnZoom.TabIndex = 40
		Me.BtnZoom.Text = "Zoom In"
		Me.BtnZoom.UseVisualStyleBackColor = true
		AddHandler Me.BtnZoom.Click, AddressOf Me.BtnZoomClick
		'
		'BtnZoom1
		'
		Me.BtnZoom1.Location = New System.Drawing.Point(677, 467)
		Me.BtnZoom1.Name = "BtnZoom1"
		Me.BtnZoom1.Size = New System.Drawing.Size(75, 23)
		Me.BtnZoom1.TabIndex = 45
		Me.BtnZoom1.Text = "Zoom Out"
		Me.BtnZoom1.UseVisualStyleBackColor = true
		AddHandler Me.BtnZoom1.Click, AddressOf Me.BtnZoom1Click
		'
		'BtnDisconnect
		'
		Me.BtnDisconnect.Location = New System.Drawing.Point(677, 432)
		Me.BtnDisconnect.Name = "BtnDisconnect"
		Me.BtnDisconnect.Size = New System.Drawing.Size(75, 23)
		Me.BtnDisconnect.TabIndex = 25
		Me.BtnDisconnect.Text = "Disconnect"
		Me.BtnDisconnect.UseVisualStyleBackColor = true
		AddHandler Me.BtnDisconnect.Click, AddressOf Me.BtnDisconnectClick
		'
		'BtnRescan
		'
		Me.BtnRescan.Location = New System.Drawing.Point(777, 432)
		Me.BtnRescan.Name = "BtnRescan"
		Me.BtnRescan.Size = New System.Drawing.Size(75, 23)
		Me.BtnRescan.TabIndex = 30
		Me.BtnRescan.Text = "Scan Ports"
		Me.BtnRescan.UseVisualStyleBackColor = true
		AddHandler Me.BtnRescan.Click, AddressOf Me.BtnRescanClick
		'
		'timer1
		'
		Me.timer1.Enabled = true
		AddHandler Me.timer1.Tick, AddressOf Me.Timer1Tick
		'
		'progressBar1
		'
		Me.progressBar1.Location = New System.Drawing.Point(15, 535)
		Me.progressBar1.Name = "progressBar1"
		Me.progressBar1.Size = New System.Drawing.Size(335, 23)
		Me.progressBar1.TabIndex = 15
		'
		'lblPort
		'
		Me.lblPort.Location = New System.Drawing.Point(280, 415)
		Me.lblPort.Name = "lblPort"
		Me.lblPort.Size = New System.Drawing.Size(50, 12)
		Me.lblPort.TabIndex = 16
		Me.lblPort.Text = "Port"
		'
		'lblStart
		'
		Me.lblStart.Location = New System.Drawing.Point(14, 415)
		Me.lblStart.Name = "lblStart"
		Me.lblStart.Size = New System.Drawing.Size(80, 12)
		Me.lblStart.TabIndex = 17
		Me.lblStart.Text = "Start MHz"
		'
		'LblStop
		'
		Me.LblStop.Location = New System.Drawing.Point(100, 415)
		Me.LblStop.Name = "LblStop"
		Me.LblStop.Size = New System.Drawing.Size(80, 12)
		Me.LblStop.TabIndex = 18
		Me.LblStop.Text = "Stop MHz"
		'
		'lblSteps
		'
		Me.lblSteps.Location = New System.Drawing.Point(190, 415)
		Me.lblSteps.Name = "lblSteps"
		Me.lblSteps.Size = New System.Drawing.Size(50, 12)
		Me.lblSteps.TabIndex = 19
		Me.lblSteps.Text = "Steps"
		'
		'trackBar1
		'
		Me.trackBar1.Location = New System.Drawing.Point(572, 535)
		Me.trackBar1.Maximum = 30
		Me.trackBar1.Minimum = 2
		Me.trackBar1.Name = "trackBar1"
		Me.trackBar1.Size = New System.Drawing.Size(272, 42)
		Me.trackBar1.TabIndex = 60
		Me.trackBar1.Value = 2
		AddHandler Me.trackBar1.Scroll, AddressOf Me.TrackBar1Scroll
		'
		'lblMaxSwr
		'
		Me.lblMaxSwr.Location = New System.Drawing.Point(658, 513)
		Me.lblMaxSwr.Name = "lblMaxSwr"
		Me.lblMaxSwr.Size = New System.Drawing.Size(20, 12)
		Me.lblMaxSwr.TabIndex = 21
		Me.lblMaxSwr.Text = "10"
		'
		'lblMaxSwr1
		'
		Me.lblMaxSwr1.Location = New System.Drawing.Point(579, 513)
		Me.lblMaxSwr1.Name = "lblMaxSwr1"
		Me.lblMaxSwr1.Size = New System.Drawing.Size(80, 12)
		Me.lblMaxSwr1.TabIndex = 31
		Me.lblMaxSwr1.Text = "Maximum SWR"
		'
		'lblMaxSwr2
		'
		Me.lblMaxSwr2.Location = New System.Drawing.Point(579, 565)
		Me.lblMaxSwr2.Name = "lblMaxSwr2"
		Me.lblMaxSwr2.Size = New System.Drawing.Size(12, 12)
		Me.lblMaxSwr2.TabIndex = 32
		Me.lblMaxSwr2.Text = "2"
		'
		'lblMaxSwr3
		'
		Me.lblMaxSwr3.Location = New System.Drawing.Point(824, 565)
		Me.lblMaxSwr3.Name = "lblMaxSwr3"
		Me.lblMaxSwr3.Size = New System.Drawing.Size(20, 12)
		Me.lblMaxSwr3.TabIndex = 33
		Me.lblMaxSwr3.Text = "30"
		'
		'ComboBandSelect
		'
		Me.ComboBandSelect.FormattingEnabled = true
		Me.ComboBandSelect.Items.AddRange(New Object() {"160 m", "80 m", "60 m  ", "40 m", "30 m", "20 m", "17 m", "15 m", "12 m", "10 m", "1 .. 30 Mhz"})
		Me.ComboBandSelect.Location = New System.Drawing.Point(15, 487)
		Me.ComboBandSelect.Name = "ComboBandSelect"
		Me.ComboBandSelect.Size = New System.Drawing.Size(79, 21)
		Me.ComboBandSelect.TabIndex = 16
		AddHandler Me.ComboBandSelect.SelectedIndexChanged, AddressOf Me.ComboBandSelectSelectedIndexChanged
		'
		'lblbands
		'
		Me.lblbands.Location = New System.Drawing.Point(15, 467)
		Me.lblbands.Name = "lblbands"
		Me.lblbands.Size = New System.Drawing.Size(49, 12)
		Me.lblbands.TabIndex = 62
		Me.lblbands.Text = "Bands"
		'
		'MainForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(1016, 587)
		Me.Controls.Add(Me.lblbands)
		Me.Controls.Add(Me.ComboBandSelect)
		Me.Controls.Add(Me.lblMaxSwr3)
		Me.Controls.Add(Me.lblMaxSwr2)
		Me.Controls.Add(Me.lblMaxSwr1)
		Me.Controls.Add(Me.lblMaxSwr)
		Me.Controls.Add(Me.trackBar1)
		Me.Controls.Add(Me.lblSteps)
		Me.Controls.Add(Me.LblStop)
		Me.Controls.Add(Me.lblStart)
		Me.Controls.Add(Me.lblPort)
		Me.Controls.Add(Me.progressBar1)
		Me.Controls.Add(Me.BtnRescan)
		Me.Controls.Add(Me.BtnDisconnect)
		Me.Controls.Add(Me.BtnZoom1)
		Me.Controls.Add(Me.BtnZoom)
		Me.Controls.Add(Me.btnSweep)
		Me.Controls.Add(Me.chkContSweep)
		Me.Controls.Add(Me.txtNumSteps)
		Me.Controls.Add(Me.txtStopFreq)
		Me.Controls.Add(Me.txtStartFreq)
		Me.Controls.Add(Me.cmbPort)
		Me.Controls.Add(Me.zg1)
		Me.Controls.Add(Me.btnConnect)
		Me.Name = "MainForm"
		Me.Text = "VNA - Arduino Antennen Analysator"
		AddHandler Load, AddressOf Me.MainFormLoad
		CType(Me.trackBar1,System.ComponentModel.ISupportInitialize).EndInit
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private lblbands As System.Windows.Forms.Label
	Private ComboBandSelect As System.Windows.Forms.ComboBox
	Private lblMaxSwr3 As System.Windows.Forms.Label
	Private lblMaxSwr2 As System.Windows.Forms.Label
	Private lblMaxSwr1 As System.Windows.Forms.Label
	Private lblMaxSwr As System.Windows.Forms.Label
	Private trackBar1 As System.Windows.Forms.TrackBar
	Private lblSteps As System.Windows.Forms.Label
	Private LblStop As System.Windows.Forms.Label
	Private lblStart As System.Windows.Forms.Label
	Private lblPort As System.Windows.Forms.Label
	Private progressBar1 As System.Windows.Forms.ProgressBar
	Private timer1 As System.Windows.Forms.Timer
	Private BtnRescan As System.Windows.Forms.Button
	Private BtnDisconnect As System.Windows.Forms.Button
	Private BtnZoom1 As System.Windows.Forms.Button
	Private BtnZoom As System.Windows.Forms.Button
	Private btnSweep As System.Windows.Forms.Button
	Private timer2 As System.Windows.Forms.Timer
	Private btnConnect As System.Windows.Forms.Button
	Private Withevents serialPort1 As System.IO.Ports.SerialPort
	Private chkContSweep As System.Windows.Forms.CheckBox
	Private txtNumSteps As System.Windows.Forms.TextBox
	Private txtStopFreq As System.Windows.Forms.TextBox
	Private txtStartFreq As System.Windows.Forms.TextBox
	Private cmbPort As System.Windows.Forms.ComboBox
	Private zg1 As ZedGraph.ZedGraphControl
End Class
