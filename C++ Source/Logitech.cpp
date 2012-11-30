
// Logitech.cpp : implementation file
//

#include "stdafx.h"
#include "Logitech.h"



Logitech::Logitech()
{
	test = 0;
}

Logitech::~Logitech()
{

}

BOOL Logitech::OnInitDialog()
{

	HRESULT hRes = m_lcd.Initialize(_T("MusicBee"), LG_DUAL_MODE, FALSE, TRUE);


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

	logo = m_lcd.AddText(LG_STATIC_TEXT, LG_BIG, DT_CENTER, LGLCD_BW_BMP_WIDTH);
	m_lcd.SetOrigin(logo, 0, 5);
	m_lcd.SetText(logo, _T("MusicBee"));
	m_lcd.Update();
}

VOID Logitech::InitLCDObjectsColor()
{
	m_lcd.ModifyDisplay(LG_COLOR);
	logo = m_lcd.AddText(LG_STATIC_TEXT, LG_BIG, DT_CENTER, LGLCD_BW_BMP_WIDTH);
	m_lcd.SetOrigin(logo, 0, 5);
	m_lcd.SetText(logo, _T("MusicBee"));
	m_lcd.Update();

}

void Logitech::changeArtistTitle(wstring artistStr, wstring titleStr, wstring duration)
{
	m_lcd.RemovePage(0);
	m_lcd.AddNewPage();
	m_lcd.ShowPage(0);

	if(m_lcd.IsDeviceAvailable(LG_MONOCHROME))
	{
		artist = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_BW_BMP_WIDTH);
		m_lcd.SetOrigin(artist, 0, 0);
		m_lcd.SetText(artist, artistStr.c_str());

		title = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_BW_BMP_WIDTH);
		m_lcd.SetOrigin(title, 0, 13);
		m_lcd.SetText(title, titleStr.c_str());

		progressbar = m_lcd.AddProgressBar(LG_DOT_CURSOR);
		m_lcd.SetProgressBarSize(progressbar, 136, 5);
		m_lcd.SetOrigin(progressbar, 12, 38);

		time = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd.SetOrigin(time, 12, 29);
		m_lcd.SetText(time, _T("00:05"));

		time1 = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd.SetOrigin(time1, 125, 29);
		m_lcd.SetText(time1, duration.c_str());

		/*   playIcon = static_cast<HICON>(LoadImage(
		AfxGetInstanceHandle(), 
		MAKEINTRESOURCE(IDB_PNG1),
		IMAGE_ICON, 
		16, 
		16, 
		LR_MONOCHROME));
		playIconHandle = m_lcd.AddIcon(playIcon, 16, 16);
		m_lcd.SetOrigin(playIconHandle, 2, 29);*/

		m_lcd.Update();
	}

	else if(m_lcd.IsDeviceAvailable(LG_COLOR))
	{


	}

	artistStr.clear();
	titleStr.clear();
	duration.clear();

}

/*Undefined = 0,
Loading = 1,
Playing = 3,
Paused = 6,
Stopped = 7*/
void Logitech::changeState(int state)
{
	if(m_lcd.IsDeviceAvailable(LG_COLOR))
	{


	}

	else if(m_lcd.IsDeviceAvailable(LG_MONOCHROME))
	{
	}
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