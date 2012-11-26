#pragma once

#include "../Src/EZ_LCD.h"
#include "Bitmap.h"


class Logitech
{
    // Construction
public:
    Logitech();
	~Logitech();
	BOOL OnInitDialog();



private:
    HICON m_hIcon;

    CEzLcd m_lcd;
	HANDLE text;

    // Monochrome
    HANDLE screen;

	// Bitmaps
	cBitmap m_background;


    VOID InitLCDObjectsMonochrome();
    VOID InitLCDObjectsColor();

    VOID CheckButtonPresses();
    bool CheckbuttonPressesMonochrome();
    bool CheckbuttonPressesColor();

};