
// Logitech.cpp : implementation file
//

#include "stdafx.h"
#include "Logitech.h"



Logitech::Logitech()
{

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

void Logitech::changeArtistTitle(wstring artistStr, wstring titleStr, wstring duration, int position)
{
	m_lcd.RemovePage(0);
	m_lcd.AddNewPage();
	m_lcd.ShowPage(0);

	if (logo != 0)
	{
		delete logo;
		logo = 0;
	}

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

		int minutes = position/60;
		int seconds = position%60;
		string minuteStr = to_string(minutes);
		string secondStr = to_string(seconds);

		if(minutes < 10)
		{
			minuteStr += "0";
		}

		if(seconds < 10)
		{
			secondStr += "0";
		}

		string positionString = minuteStr + ":" + secondStr;

		wstring ws( positionString.begin(), positionString.end() );

		time = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd.SetOrigin(time, 12, 29);
		m_lcd.SetText(time, ws.c_str());

		string s( duration.begin(), duration.end() );
		
		if(s.size() < 5)
		{
			s = "0" + s;
		}

		ws.clear();

		ws = wstring( s.begin(), s.end() );
		time1 = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd.SetOrigin(time1, 125, 29);
		m_lcd.SetText(time1, ws.c_str());
		ws.clear();
		
		/*playIcon = static_cast<HICON>(LoadImage(
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