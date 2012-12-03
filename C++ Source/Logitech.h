#pragma once

#include "stdafx.h"
#include "Src\EZ_LCD.h"
#include "resource.h"
#include "windows.h"
#include <thread>

using namespace std;

class Logitech
{
	// Construction
public:
	Logitech();
	~Logitech();
	BOOL OnInitDialog();

	void changeArtistTitle(wstring artist, wstring title, wstring time, int position);
	void changeState(int state);
	void setPosition(int);
	static Logitech * object;
	static void startThread();
private:
	thread timerThread;
	HICON m_hIcon;

	CEzLcd m_lcd;
	HANDLE logo;
	HANDLE artist;
	HANDLE title;
	HANDLE progressbar;
	HANDLE time;
	HANDLE time1;
	HANDLE playIconHandle;
	HICON playIcon;

	wstring artistString;
	wstring titleString;
	wstring durationString;
	int position;
	int duration;
	int state;

	int getDuration(wstring);
	wstring getPositionString();
	VOID InitLCDObjectsMonochrome();
	VOID InitLCDObjectsColor();

	/*  VOID CheckButtonPresses();
	bool CheckbuttonPressesMonochrome();
	bool CheckbuttonPressesColor();*/

};