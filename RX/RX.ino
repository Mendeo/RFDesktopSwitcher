#define RX_PIN 2
#define ERROR_VALUE 2
#define COMMAND_READY 'r'
#define COMMAND_IN_WORK 'w'
#define COMMAND_ANSWER 'o'
#define COMMAND_FIRE 'f'
#define COMMAND_DISCONNECT 'd'
#define SCAN_TIMEOUT 100
#define CHECK_DEVICE_TIMEOUT 1000
#define PAUSE_TIME 8000

const uint8_t RX_DATA[] = {12, 43, 29, 42, 24};
unsigned long _sysTime;
unsigned long _lastInWorkSend = 0;
unsigned long _lastFireSend = 0;
unsigned long _startPauseTime = 0;
bool _isReadyStatus = true;
bool _needWaitPause = false;

volatile unsigned long _RXCurrentTime = 0;
volatile unsigned long _RXPreviousTime = 0;
volatile unsigned long _pulseDuration = 0;
volatile uint8_t _rxCounter = 0;
volatile bool _isFire = false;

void setup()
{
  pinMode(RX_PIN, INPUT);
  Serial.begin(115200);
}

void onPulse()
{
  _RXCurrentTime = millis();
  _pulseDuration = _RXCurrentTime - _RXPreviousTime;
  _RXPreviousTime = _RXCurrentTime;
  if (!(digitalRead(RX_PIN) ^ (_rxCounter % 2)) && _pulseDuration >= RX_DATA[_rxCounter] - ERROR_VALUE && _pulseDuration <= RX_DATA[_rxCounter] + ERROR_VALUE)
  {
    _rxCounter++;
  }
  else
  {
    _rxCounter = 0;
  }
  if (_rxCounter == sizeof(RX_DATA))
  {
    _isFire = true;
    _rxCounter = 0;
  }
}

void loop()
{
  //Режим ожидания подключения.
  if (_isReadyStatus)
  {
    while (true)
    {
      if (Serial.available())
      {
        if (Serial.read() == COMMAND_ANSWER)
        {
          _isReadyStatus = false;
          Serial.flush();
          attachInterrupt(digitalPinToInterrupt(RX_PIN), onPulse, CHANGE);
          break;
        }
      }
      Serial.write(COMMAND_READY);
      delay(SCAN_TIMEOUT);
    }
  }
  _sysTime = millis();
  //Отправка символа работы.
  if (_sysTime - _lastInWorkSend >= CHECK_DEVICE_TIMEOUT)
  {
    _lastInWorkSend = _sysTime;
    Serial.write(COMMAND_IN_WORK);
  }
  //Если пришло сообщение разъединиться.
  if (Serial.available())
  {
    if (Serial.read() == COMMAND_DISCONNECT)
    {
      Serial.flush();
      detachInterrupt(digitalPinToInterrupt(RX_PIN));
      _isReadyStatus = true;
    }
  }
  if (_isFire)
  {
    _isFire = false;
    detachInterrupt(digitalPinToInterrupt(RX_PIN)); //Отключаем перывания на время паузы
    _needWaitPause = true;
    _startPauseTime = _sysTime;
    Serial.write(COMMAND_FIRE);
  }
  if (_needWaitPause && _sysTime - _startPauseTime >= PAUSE_TIME) //Ждём PAUSE_TIME милисекунд после события _isFire.
  {
    _needWaitPause = false;
    attachInterrupt(digitalPinToInterrupt(RX_PIN), onPulse, CHANGE);
  }
}
