/*
 * RFFlash.cpp
 *
 * Created: 09.10.2019 14:12:09
 * Author : Александр Меняйло
 */ 

#define F_CPU 1200000UL

#include <avr/io.h>
#include <avr/interrupt.h>
#include <avr/sleep.h>
#include <util/delay.h>
#include <avr/wdt.h>

#define TX_PAUSE 50
#define TX_REPEAT 5
#define TX_PIN PORTB0
#define TX_POWER PORTB4
//#define TX_SIZE 5

const uint8_t TX_DATA[] = {12, 43, 29, 42, 24};
volatile bool _hasSignal = false;  
volatile bool _txEnable = false;

ISR(INT0_vect)
{
	_hasSignal = true;
}

ISR (WDT_vect)
{
	_txEnable = true;
	_hasSignal = false;
	//WDTCR |= (1 << WDTIE); //Включаем прерывание от WDT.
}

inline void delay(uint8_t t)
{
	while(t--)
	{
		_delay_ms(1);
	}
}

inline void send()
{
	PORTB |= (1 << TX_POWER);
	_delay_ms(50); //Время на включение передатчика.
	for (unsigned int i = 0; i < TX_REPEAT; i++)
	{
		for (unsigned int j = 0; j < sizeof(TX_DATA); j++)
		{
			PORTB ^= (1 << TX_PIN);
			delay(TX_DATA[j]);
		}
		PORTB ^= (1 << TX_PIN);
		_delay_ms(TX_PAUSE);
	}
	PORTB &= ~(1 << TX_POWER);
}

inline void sleep()
{
	PORTB &= ~(1 << TX_PIN); //Выключаем TX_PIN.
	//PORTB &= ~(1 << TX_POWER); //Выключаем TX_POWER.
	//Выключаем BOD на время сна по схеме из даташита.
	BODCR = (1 << BODS) | (1 << BODSE);
	BODCR = (1 << BODS);
	sleep_cpu(); //Включаем сон.
}

inline void wdt_start()
{
	wdt_reset();
	wdt_enable(WDTO_8S);
	WDTCR |= (1 << WDTIE); //Включаем прерывание от WDT.
}

int main(void)
{
	DDRB = (1 << TX_PIN) | (1 << TX_POWER); //Включаем TX_PIN и TX_POWER на выход, остальные на вход.
	GIMSK = (1 << INT0); //Включаем прерывания на INT0 (PB1).
	MCUCR &= ~(1 << ISC00) & ~(1 << ISC01); //Настраиваем прерывание по низкому уровню.
	//Выключаем ADC.
	ADCSRA &= ~(1 << ADEN);
	PRR |= (1 << PRADC);	
	sei(); //Разрешаем прерывания.
	//Нстраиваем WDT
	wdt_start();
	set_sleep_mode(SLEEP_MODE_PWR_DOWN); //Нстраиваем режим сна.
	sleep_enable(); //Разрешаем сон в приниципе.

	while (1)
	{
		if (_hasSignal && _txEnable) 
		{
			cli();
			_hasSignal = false;
			_txEnable = false;
			send();
			wdt_start(); //Взводим WDT.
			sei();
		}
		if (_txEnable) wdt_disable(); //Выключаем WDT для экономии энергии во время сна.
		sleep();
	}
}
