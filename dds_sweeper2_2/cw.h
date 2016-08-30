
#ifndef _cw_h
#define _cw_h

class Cw {

  public:
    bool busy ();
    Cw(int wpm = 18);
    void setText(char *s);
    int run();
    void start();
    void stop();
    void setSpeed(int wpm);
    bool key;

  private:
    enum enumCwElement {
      idle, aktiv, pause, buchstabenPause, wortPause
    }
    ;
    enumCwElement element;
    int t;
    int index;
    int lastIndex;
    char *text;
    void nextElement();
    void startElement();
    void nextCharacter();
    bool nochWas();
    int maske;
    int morsezeichen;
    void key_on();
    void key_off();

};
#endif







