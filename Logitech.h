//-----------------------------------------------------------------
// Logitech File
// C++ Header - Logitech.h - version 2012 v1.0
//-----------------------------------------------------------------

#pragma once

//-----------------------------------------------------------------
// Include Files
//-----------------------------------------------------------------

#include "stdafx.h"
#include "Src\EZ_LCD.h"
#include "resource.h"
#include "windows.h"
#include <thread>
#include <iostream>
#include <fstream>
#include "playState.h"

using namespace std;

//-----------------------------------------------------------------
// Logitech Class
//-----------------------------------------------------------------

class Logitech
{
	// Construction
public:
	//---------------------------
	// Constructor(s)
	//---------------------------
	Logitech();

	//---------------------------
	// Destructor
	//---------------------------
	~Logitech();

	//---------------------------
	// General Methods
	//---------------------------
	BOOL OnInitDialog();

	void changeArtistTitle(wstring artistStr, wstring albumStr, wstring titleStr, wstring duration, int position);
	void changeState(int state);
	void setPosition(int);
	static Logitech * object;
	static void startThread();

	bool getFirstTime();
private:

	int getDuration(wstring);
	wstring getPositionString();
	VOID createMonochrome();
	VOID createColor();

	// -------------------------
	// Datamembers
	// -------------------------
	thread timerThread;
	HICON m_hIcon;

	CEzLcd m_lcd;
	HANDLE logo;
	HANDLE artist;
	HANDLE album;
	HANDLE title;
	HANDLE progressbar;
	HANDLE time;
	HANDLE time1;
	HANDLE playIconHandle;
	HICON playIcon;

	bool firstTime;
	bool stopthread;
	
	wstring artistString;
	wstring albumString;
	wstring titleString;
	wstring durationString;
	int position;
	int duration;
	int state;

	// -------------------------
	// Disabling default copy constructor and default assignment operator.
	// If you get a linker error from one of these functions, your class is internally trying to use them. This is
	// an error in your class, these declarations are deliberately made without implementation because they should never be used.
	// -------------------------
	Logitech(const Logitech& t);
	Logitech& operator=(const Logitech& t);
};