
// Logitech.cpp : implementation file
//

#include "stdafx.h"
#include "Logitech.h"
#include <vector>


Logitech::Logitech()
{

}

Logitech::~Logitech()
{

}

BOOL Logitech::OnInitDialog()
{

	HRESULT hRes = m_lcd.Initialize(_T("MusicBee"), LG_DUAL_MODE, TRUE, TRUE);

	if (hRes != S_OK)
	{
		// Something went wrong, when connecting to the LCD Manager software. Deal with it...
		PostQuitMessage(0);
		return FALSE;
	}

	if(m_lcd.IsDeviceAvailable(LG_MONOCHROME))
		{
			InitLCDObjectsMonochrome();
		}

		else if(m_lcd.IsDeviceAvailable(LG_COLOR))
		{
			InitLCDObjectsColor();
		}

//	SetTimer(0xabab, 30, NULL); // for scrolling to work smoothly, timer should be pretty fast

	return TRUE;  // return TRUE  unless you set the focus to a control
}

VOID Logitech::InitLCDObjectsMonochrome()
{
	m_lcd.ModifyDisplay(LG_MONOCHROME);

	text = m_lcd.AddText(LG_STATIC_TEXT, LG_BIG, DT_LEFT, LGLCD_BW_BMP_WIDTH);
    m_lcd.SetOrigin(text, 0, 0);
    m_lcd.SetText(text, _T("MusicBee"));
}

VOID Logitech::InitLCDObjectsColor()
{
	m_lcd.ModifyDisplay(LG_COLOR);

}

VOID Logitech::CheckButtonPresses()
{
	if(m_lcd.IsDeviceAvailable(LG_MONOCHROME) && CheckbuttonPressesMonochrome())
	{
		InitLCDObjectsMonochrome();
	}

	else if(m_lcd.IsDeviceAvailable(LG_COLOR) && CheckbuttonPressesColor())
	{
		InitLCDObjectsColor();
	}
}

bool Logitech::CheckbuttonPressesMonochrome()
{
	bool buttonPressed = false;

	m_lcd.ModifyDisplay(LG_MONOCHROME);

	if (m_lcd.ButtonTriggered(LG_BUTTON_4))
	{

	}

	else if(m_lcd.ButtonTriggered(LG_BUTTON_1))
	{

	}

	else if(m_lcd.ButtonTriggered(LG_BUTTON_2))
	{

	}

	else if(m_lcd.ButtonTriggered(LG_BUTTON_3))
	{

	}

	return buttonPressed;
}

bool Logitech::CheckbuttonPressesColor()
{
	m_lcd.ModifyDisplay(LG_COLOR);

	bool buttonPressed = false;

	if (m_lcd.ButtonReleased(LG_BUTTON_RIGHT))
	{

	}

	if (m_lcd.ButtonReleased(LG_BUTTON_LEFT))
	{

	}

	if (m_lcd.ButtonReleased(LG_BUTTON_DOWN))
	{

	}

	if (m_lcd.ButtonReleased(LG_BUTTON_UP))
	{

		buttonPressed = true;
	}

	return buttonPressed;
}