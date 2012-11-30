#pragma once

#include "stdafx.h"
#include "Src\EZ_LCD.h"
 #include <sstream> 
using namespace std;

class Logitech
{
    // Construction
public:
    Logitech();
	~Logitech();
	BOOL OnInitDialog();

	void changeArtistTitle(wstring artist, wstring title, wstring time);
	void changeState(int state);

private:
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

    // Monochrome
    HANDLE screen;

	int test;
	// Bitmaps
//	cBitmap m_background;


    VOID InitLCDObjectsMonochrome();
    VOID InitLCDObjectsColor();

    VOID CheckButtonPresses();
    bool CheckbuttonPressesMonochrome();
    bool CheckbuttonPressesColor();

};