#define RX_PIN 7
#define ERROR_VALUE 2
#define PAUSE_TIME 8000
#define WAIT_SIMBOL 'r'
#define IN_WORK_SIMBOL 'w'
#define COMP_ANSWER 'o'

const uint8_t RX_DATA[] = {12, 43, 29, 42, 24};
unsigned long _sysTime;
uint8_t _rxCounter = 0;


void setup()
{
  pinMode(RX_PIN, INPUT);
  Serial.begin(115200);
  Serial.println("Start");
  uint8_t buf;
  while (true)
  {
    if (Serial.available())
    {
      buf = Serial.read();
      if (buf == COMP_ANSWER)
      {
        Serial.write(IN_WORK_SIMBOL);
        break;
      }
    }
    Serial.write(WAIT_SIMBOL);
    delay(100);
  }
}

void loop()
{
  _sysTime = millis();
  if (_rxCounter % 2 == 0)
  {
    while (digitalRead(RX_PIN)){}
  }
  else
  {
    while (!digitalRead(RX_PIN)){}
  }
  unsigned int len = millis() - _sysTime;
  if (len >= RX_DATA[_rxCounter] - ERROR_VALUE && len <= RX_DATA[_rxCounter] + ERROR_VALUE)
  {
    _rxCounter++;
  }
  else
  {
    _rxCounter = 0;
  }
  if (_rxCounter == sizeof(RX_DATA))
  {
    Serial.println("Ok!");
    delay(PAUSE_TIME);
    _rxCounter = 0;
  }  
}
