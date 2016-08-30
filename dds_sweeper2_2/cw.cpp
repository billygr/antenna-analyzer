#include <arduino.h>
#include "cw.h"
#include "timer.h"

const char morse_tabelle[] = {
  0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, // 0x00
  0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
  0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x28,// 0x10
  0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
  0x00, 0x80, 0x52, 0x45, 0x1f, 0x01, 0x28, 0x5e,// 0x20
  0x36, 0x6d, 0x35, 0x2A, 0x73, 0x61, 0x55, 0x32,
  0x3F, 0x2F, 0x27, 0x23, 0x21, 0x20, 0x30, 0x38,// 0x30
  0x3C, 0x3E, 0x78, 0x6a, 0x01, 0x31, 0x01, 0x4C,
  0x01, 0x05, 0x18, 0x1A, 0x0C, 0x02, 0x12, 0x0E,// 0x40
  0x10, 0x04, 0x17, 0x0D, 0x14, 0x07, 0x06, 0x0F,
  0x16, 0x1D, 0x0A, 0x08, 0x03, 0x09, 0x11, 0x0B,// 0x50
  0x19, 0x1B, 0x1C, 0x01, 0x01, 0x01, 0x01, 0x61,
  0x01, 0x05, 0x18, 0x1A, 0x0C, 0x02, 0x12, 0x0E,// 0x60
  0x10, 0x04, 0x17, 0x0D, 0x14, 0x07, 0x06, 0x0F,
  0x16, 0x1D, 0x0A, 0x08, 0x03, 0x09, 0x11, 0x0B,
  0x19, 0x1B, 0x1C, 0x01, 0x01, 0x01, 0x01, 0x01,
  0x01, 0x13, 0x01, 0x01, 0x15, 0x01, 0x01, 0x01,
  0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x15, 0x01,
  0x01, 0x01, 0x01, 0x01, 0x1e, 0x01, 0x01, 0x01,
  0x01, 0x1e, 0x13, 0x01, 0x01, 0x01, 0x01, 0x1
};

Timer cwTimer;

Cw::Cw(int wpm)
{
  setSpeed(wpm);
}
void Cw::setText(char *s)
{
    text = s;
    lastIndex = strlen(s) - 1;
}
 bool Cw::busy (){
  return element!= idle;
 }
 
void Cw::start()
{
    index = 0;
    nextCharacter();
}
void Cw::stop()
{
  element= idle;
}
int Cw::run()
{
  nextElement();
  return element!= idle;
}

bool Cw::nochWas()
{
  index++;
  return ( index > lastIndex) ? 0 : 1;

}

void  Cw::nextCharacter()
{
  int i;
  char c;
  c = text[index];
  i = tolower(c);
  morsezeichen = morse_tabelle[i];
  //             ; umwandlung "ascii to morse" ueber tabelle
  //             ; z.b.  'a'  =00000101b
  //             ;                  ^._
  //
  //             ;       '2'  =00100111b
  //             ;               ^..___
  //             ; bei "^" hinter dem ersten 1-bit
  //             ; faengt der morse-code an

  //sonderfaelle
  if ( morsezeichen == 0 ) // leerzeichen
  {
    cwTimer.interval(4 * t);
    element = wortPause;
  }
  else if ( morsezeichen == 1) // zeichen ohne morsecode
  {
    //    Serial.print( "unbekannter morsecode fÃ¼r: ");
    //    Serial.println(c);
    element = idle;
  }
  else
  {
    // startmarke suchen= 1. bit von links
    maske = 0x80;
    for (i = 7; i >= 0; i--)
    {
      if ( (maske & morsezeichen) > 0)
      {
        break;//gefunden
        // maske zeigt nun auf lang oder kurz {1|0}
      }
      maske = maske >> 1;
    }
    maske = maske >> 1;
    startElement();
  }
}

void Cw::startElement()
{
  key_on();
  if ( (morsezeichen & maske) > 0)
  {
    cwTimer.interval(t * 3); // lang
    //    Serial.print("_");
  }
  else
  {
    cwTimer.interval(t); //kurz
    //    Serial.print(".");
  }
  element = aktiv;
}

void Cw::nextElement()
{
  if ( cwTimer.event())
  {
    switch (element)
    {
      case idle:
        break;

      case aktiv:
        element = pause;
        key_off();
        cwTimer.interval(t);
        break;

      case pause:
        //neachstes element
        maske = maske >> 1;
        if ( maske == 0)
        {
          element = buchstabenPause;
          cwTimer.interval(3 * t);
          //        Serial.print(" ");
        }
        else
        {
          startElement();
        }
        break;

      case wortPause:
      case buchstabenPause:
        if ( nochWas() )
        {
          nextCharacter();
        }
        else
        {
          cwTimer.interval(t);
          element = idle;
        }
        break;

    }//switch
  }
}

void Cw::setSpeed( int wpm)
{
  t = 1200 / wpm;
}

void Cw::key_on()
{
  key = 1;
}
void Cw::key_off()
{
  key = 0;
}










