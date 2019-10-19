#define RX_PIN 7
#define ERROR_VALUE 2
#define COMMAND_READY 'r'
#define COMMAND_IN_WORK 'w'
#define COMMAND_ANSWER 'o'
#define COMMAND_FIRE 'f'
#define COMMAND_DISCONNECT 'd'
#define SCAN_TIMEOUT 100
#define CHECK_DEVICE_TIMEOUT 1000

const uint8_t RX_DATA[] = {12, 43, 29, 42, 24};
unsigned long _sysTime;
unsigned long _lastInWorkSend = 0;
uint8_t _rxCounter = 0;
bool _isReadyStatus = true;


void setup()
{
  pinMode(RX_PIN, INPUT);
  Serial.begin(115200);
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
          break;
        }
      }
      Serial.write(COMMAND_READY);
      delay(SCAN_TIMEOUT);
    }
  }
  _sysTime = millis();
  //Расшифровка данных от приёмника.
  if (_rxCounter % 2 == 0)
  {
    waiter(digitalRead(RX_PIN));
  }
  else
  {
    waiter(!digitalRead(RX_PIN));
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
    Serial.write(COMMAND_FIRE);
    _rxCounter = 0;
  }
}

 //Ожидаем пока работает заданное условие, при этом посылаем в консоль сообщения, что мы ещё на связи.
void waiter(bool condition)
{
  unsigned long _curTime;
  while (condition)
  {
    //Отправка символа работы.
    _curTime = millis();
    if (_curTime - _lastInWorkSend >= CHECK_DEVICE_TIMEOUT)
    {
      _lastInWorkSend = _curTime;
      Serial.write(COMMAND_IN_WORK);
    }
    if (Serial.available())
    {
      if (Serial.read() == COMMAND_DISCONNECT)
      {
        Serial.flush();
        _isReadyStatus = true;
        break;
      }
    }
  }
}
