#include <EEPROM.h>
#include "cw.h"

/***
 * update 12 02 2016 Version "dds_sweeper2_2"
 * - Korrektur Tunefunktion 't' und Funktion 'c' ( On/off DDS)
 * 
   update 16 01 2016 Version "dds_sweeper2_1"
   - bakengeschwindigkeit von 18 auf 12 wpm erniedrigt.
   - dds_off() bei ende des sweep
   - dds_off() bei beliebiger taste
   - befehl 't' = "tune": sendet traeger mit der startfrequenz ( 'a' oder 'c' )
*/
/***************************************************************************\
    Name    : DDS_Sweeper2
    Author  : Beric Dunn (K6BEZ)
    Notice  : Copyright (c) 2013  CC-BY-SA
            : Creative Commons Attribution-ShareAlike 3.0 Unported License
    Date    : 9/26/2013  10/2015 (hs)
    Version : n
    Notes   : Written using for the Arduino
            :   Pins:
            :    A0 - Reverse Detector Analog in
            :    A1 - Forward Detector Analog in
            : Modified by Norbert Redeker (DG7EAO) 07/2014
            : Modified by Heribert Schulte dk2jk 09/2014 //(hs)
            : mit verstaerker hinter dds, (Hardware m4) okt 2015
            : 17.10.15 bake
  \***************************************************************************/

// Define Pins used to control AD9850 DDS

// board ..
const int SCLK  = 12;
const int FQ_UD = 11;
const int SDAT  = 10;
const int RESET = 9;

double Fstart_MHz = 1.0;  // Start Frequency for sweep
double Fstop_MHz = 30.000;  // Stop Frequency for sweep
double current_freq_MHz; // Temp variable used during sweep
long serial_input_number; // Used to build number from serial stream
int num_steps = 100; // Number of steps to use in the sweep
//char incoming_char; // Character read from serial stream
double offset_forward;
double offset_reverse;
bool interpreter_aktiv = false;
bool tune_aktiv = false;

struct s_eeprom {
  char text[40]; // das ist der text fuer die bake
};
s_eeprom eeprom;

const char  SoftwareVersion[] = "Software Version: " __FILE__; //"Vers.:dds_sweeper1_13dez2014.ino";
const char     SoftwareDate[] = "Build Date      : " __DATE__; //"Vers.:dds_sweeper1_13dez2014.ino";
Cw bake(12);  // 2_1 speed von 18 auf 12 erniedrigt

// the setup routine runs once when you press reset:
void setup() {
  // Configiure DDS control pins for digital output
  pinMode(FQ_UD, OUTPUT);
  pinMode(SCLK, OUTPUT);
  pinMode(SDAT, OUTPUT);
  pinMode(RESET, OUTPUT);

  // Configure LED pin for digital output
  pinMode(13, OUTPUT);


  // Set up analog inputs on A0 and A1, internal reference voltage
  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  analogReference(INTERNAL); // ref voltage = 1,1 volt


  // initialize serial communication at 57600 baud
  Serial.begin(57600);

  // Reset the DDS
  digitalWrite(RESET, HIGH);
  digitalWrite(RESET, LOW);

  //Initialise the incoming serial number to zero
  serial_input_number = 0;

  EEPROM.get(0, eeprom);
  calc_offset();
  version();
  Serial.flush();

}

void interpreter(char rxd)
{
  switch (rxd) {
    case '0':
    case '1':
    case '2':
    case '3':
    case '4':
    case '5':
    case '6':
    case '7':
    case '8':
    case '9':  // eingabezahl ansammeln
      serial_input_number = serial_input_number * 10 + (rxd - '0');
      break;
    case 'A':  // 7000000A
    case 'a':
      //Turn frequency into FStart
      Fstart_MHz = ((double)serial_input_number) / 1000000;
      serial_input_number = 0;
      break;
    case 'B': // 10000000B
    case 'b':
      //Turn frequency into FStop
      Fstop_MHz = ((double)serial_input_number) / 1000000;
      serial_input_number = 0;
      break;
    case 'C':  // 7032000C
    case 'c':
      //Turn frequency into FStart and set DDS output to single frequency
      Fstart_MHz = ((double)serial_input_number) / 1000000;
      //SetDDSFreq(Fstart_MHz);
      SetDDSFreq(Fstart_MHz * 1000000);
      delay(100);
      SetDDSFreq(Fstart_MHz * 1000000);
      serial_input_number = 0;
      break;
    case 'N': // 100N
    case 'n':
      // Set number of steps in the sweep
      num_steps = serial_input_number;
      serial_input_number = 0;
      break;
    case 'S':
    case 's':
      Perform_sweep();
      break;
    case '!':
      inputText();
      break;
    case '#':
      runBake();
      break;
    case 't':
      if ( tune_aktiv)
      { dds_off();
        Serial.println("tune off");
        tune_aktiv = false;
      }
      else
      { tune();
        Serial.println("tune on");
        tune_aktiv = true;
      }
      break;
    case '?':
      sweep_info();
      Serial.println();
      break;
    case 'h':
    case 'H':
      help();
      break;
    case 'v':
    case 'V':
      version();
      break;
  }
}
void sweep_info() {
  Serial.println("--- Sweep info ---");
  Serial.print("Start Freq:\t");
  Serial.println(Fstart_MHz * 1000000);
  Serial.print("Stop Freq:\t");
  Serial.println(Fstop_MHz * 1000000);
  Serial.print("Num Steps:\t");
  Serial.println(num_steps);
  Serial.println();
}
void version()
{ Serial.println(SoftwareVersion);
  Serial.println(SoftwareDate);
  Serial.println();
}
void help()
{ Serial.println("---- commands---");
  Serial.println("letter\tdescription\t\texample\t\tresult");
  Serial.println("'a'\tset start frequency\t6000000a\t6.000 Mhz");
  Serial.println("'b'\tset end frequency\t8000000b\t8.000 Mhz");
  Serial.println("'c'\tset constant frequency\t7032000c\t7.032 Mhz");
  Serial.println("'n'\tset numer of steps\t100n\t\t100 steps");
  Serial.println("'s'\tperform sweep from frequency a to b with n steps");
  Serial.println("'?'\tsweep info");
  Serial.println("'h'\thelp");
  Serial.println("'v'\tsoftware version");
  Serial.println("---- commands may be lower or upper case ----");
  Serial.println("Gimmick:");
  Serial.println("'t'\t\"tune\" on/off ; send carrier with start frequency ( command 'a' or 'c' )");
  Serial.println("'!'\tstore following text in EEPROM for beacon tx, end with <enter>");
  Serial.println("'#'\tsend beacon tx text again and again ( break with '#' )");
  Serial.println(" \tBeacon frequency is start frequency ( command 'a' or 'c' )");
  Serial.print("actual beacon text: \"");
  EEPROM.get(0, eeprom);
  Serial.print(eeprom.text);
  Serial.println("\"");
  Serial.print("Info offset forward: ");
  Serial.print(offset_forward, 0);
  Serial.print("; reverse: ");
  Serial.println(offset_reverse, 0);
  Serial.println();
}


// the loop routine runs over and over again forever:
void loop() {
  //Check for character
  if (Serial.available() > 0)
  { interpreter( Serial.read() );
    Serial.flush();
  }
}

void calc_offset()
{ int i;
  int n = 16;
  SetDDSFreq(0.0); //frequenz auf null einstellen
  delay(100);
  SetDDSFreq(0.0); //frequenz auf null einstellen
  delay(100);
  offset_reverse = 0;
  offset_forward = 0;
  for (i = 0; i < n; i++)
  { offset_reverse += (double)analogRead(A0);
    offset_forward += (double)analogRead(A1);
    delay(100);
  }
  offset_reverse =   offset_reverse / n;
  offset_forward  =  offset_forward / n;
}
void Perform_sweep() {
  double FWD = 0;
  double REV = 0;
  double VSWR;
  double Fstep_MHz = (Fstop_MHz - Fstart_MHz) / num_steps;

  // Start loop
  for (int i = 0; i <= num_steps; i++) {
    // Calculate current frequency
    current_freq_MHz = Fstart_MHz + i * Fstep_MHz;
    // Set DDS to current frequency
    SetDDSFreq(current_freq_MHz * 1000000);
    // Wait a little for settling
    delay(100);
    // Read the forawrd and reverse voltages
    REV = (double)analogRead(A0) - offset_reverse;
    FWD = (double)analogRead(A1) - offset_forward;
    if (REV >= FWD)
    { REV = FWD - 1;
    }
    if (REV < 1)
    { REV = 1;
    }
    VSWR = (FWD + REV) / (FWD - REV);

    if (VSWR >= 29.0)
    { VSWR = 28.999;
    }
    //Skalieren fuer Ausgabe
    VSWR = VSWR * 1000.0;

    // Send current line back to PC over serial bus
    Serial.print(current_freq_MHz * 1000000);
    Serial.print(", 0, ");
    Serial.print(VSWR, 5);
    Serial.print(", ");
    Serial.print(FWD, 0);
    Serial.print(", ");
    Serial.println(REV, 0);
  }
  // Send "End" to PC to indicate end of sweep
  dds_off();
  Serial.println("End");
  Serial.flush();
}

void SetDDSFreq(double Freq_Hz)
{ // Calculate the DDS word - from AD9850 Datasheet
  int32_t f = Freq_Hz * 4294967295 / 125000000;
  // Send one byte at a time
  for (int b = 0; b < 4; b++, f >>= 8)
  { send_byte(f & 0xFF);
  }
  // 5th byte needs to be zeros
  send_byte(0);
  // Strobe the Update pin to tell DDS to use values
  digitalWrite(FQ_UD, HIGH);
  digitalWrite(FQ_UD, LOW);
}

void send_byte(byte data_to_send) {
  // Bit bang the byte over the SPI bus
  for (int i = 0; i < 8; i++, data_to_send >>= 1)
  { // Set Data bit on output pin
    digitalWrite(SDAT, data_to_send & 0x01);
    // Strobe the clock pin
    digitalWrite(SCLK, HIGH);
    digitalWrite(SCLK, LOW);
  }
}

void inputText()
{ int i;
  eeprom.text[0] = ' ';
  for (i = 1; i < sizeof(eeprom.text); i++)
  { do
    {} while (Serial.available() == 0);
    char c = Serial.read();
    if ( (c == 0x0d ) || (c == 0x0a))
    { break;
    }
    else
    { eeprom.text[i] = c;
    }
  }
  eeprom.text[i] = 0;
  EEPROM.put(0, eeprom);
}

void runBake() {
  bool ende = false;
  Serial.print("bake laeuft! stop '#' ");
  EEPROM.get(0, eeprom);
  bake.setText(eeprom.text);
  do
  { bake.start();
    while ( bake.busy() )
    { if (Serial.available() > 0)
      { char c;
        c = Serial.read();
        if (c == '#')
        { bake.stop();
          ende = true;
          break;
        }
      }
      bake.run();
      if (bake.key)
      { SetDDSFreq(Fstart_MHz * 1000000);
      }
      else
      { SetDDSFreq(0.0);
      }
    }
  } while ( !ende);
  dds_off();
  Serial.println(".. ende");
}

void tune()
{ SetDDSFreq(Fstart_MHz * 1000000);
}

void dds_off()
{ SetDDSFreq(0L); // Null hertz = AUS
}

