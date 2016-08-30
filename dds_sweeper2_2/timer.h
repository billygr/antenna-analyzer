
#include <arduino.h>

#ifndef _timer_h
#define _timer_h

class Timer 
{
public:
  Timer(long interval = 100);
  void interval(int x);
  bool event();
private:
  long _last_ms;
  long _interval;
};

#endif






