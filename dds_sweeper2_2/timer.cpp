
#include <arduino.h>
#include "timer.h"

Timer:: Timer(long x)
{
  interval( x);
}

void Timer::interval(int x)
{
  _interval = x;
  _last_ms  = millis();
}
bool Timer::event()
{ 
  bool result;
  long x= millis();
  bool overflow1= (x < _last_ms);
  bool overflow2= (x - _last_ms > _interval);
  if( overflow1 || overflow2 )
  {
    _last_ms= x;
    result=1;
  } 
  else
  { 
    result=0;
  }
  return result;
}


