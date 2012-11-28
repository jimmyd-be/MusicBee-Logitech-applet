
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

	//HRESULT hRes = m_lcd->Initialize(_T("MusicBee"), LG_DUAL_MODE, TRUE, TRUE);

	m_lcd = new CEzLcd();

	HRESULT hRes = m_lcd->Initialize(_T("MusicBee"), LG_DUAL_MODE, TRUE, TRUE);

	if (hRes != S_OK)
	{
		// Something went wrong, when connecting to the LCD Manager software. Deal with it...
		PostQuitMessage(0);
		return FALSE;
	}

	if(m_lcd->IsDeviceAvailable(LG_MONOCHROME))
	{
		InitLCDObjectsMonochrome();
	}

	else if(m_lcd->IsDeviceAvailable(LG_COLOR))
	{
		InitLCDObjectsColor();
	}

	//	SetTimer(0xabab, 30, NULL); // for scrolling to work smoothly, timer should be pretty fast

	return TRUE;  // return TRUE  unless you set the focus to a control
}

VOID Logitech::InitLCDObjectsMonochrome()
{
	m_lcd->ModifyDisplay(LG_MONOCHROME);

	logo = m_lcd->AddText(LG_STATIC_TEXT, LG_BIG, DT_CENTER, LGLCD_BW_BMP_WIDTH);
	m_lcd->SetOrigin(logo, 0, 5);
	m_lcd->SetText(logo, _T("MusicBee"));
}

VOID Logitech::InitLCDObjectsColor()
{
	m_lcd->ModifyDisplay(LG_COLOR);

}

void Logitech::changeArtistTitle(string artistStr, string titleStr)
{
	if(m_lcd->IsDeviceAvailable(LG_MONOCHROME))
	{

		artist = m_lcd->AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_BW_BMP_WIDTH);
		m_lcd->SetOrigin(artist, 0, 0);
		wstring ws;
		ws.assign(artistStr.begin(), artistStr.end());

		m_lcd->SetText(artist, ws.c_str());
		ws.clear();

		title = m_lcd->AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_BW_BMP_WIDTH);
		m_lcd->SetOrigin(title, 0, 13);
		ws.assign(titleStr.begin(), titleStr.end());

		m_lcd->SetText(title, ws.c_str());
		ws.clear();

		progressbar = m_lcd->AddProgressBar(LG_DOT_CURSOR);
		m_lcd->SetProgressBarSize(progressbar, 136, 5);
		m_lcd->SetOrigin(progressbar, 12, 38);


		time = m_lcd->AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd->SetOrigin(time, 12, 29);
		m_lcd->SetText(time, _T("00:00"));

		time1 = m_lcd->AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd->SetOrigin(time1, 125, 29);
		m_lcd->SetText(time1, _T("00:05"));

		/*   playIcon = static_cast<HICON>(LoadImage(
        AfxGetInstanceHandle(), 
        MAKEINTRESOURCE(IDI_NEXT),
        IMAGE_ICON, 
        16, 
        16, 
        LR_MONOCHROME));
    playIconHandle = m_lcd->AddIcon(playIcon, 16, 16);
    m_lcd->SetOrigin(playIconHandle, 2, 29);*/


	}

	else if(m_lcd->IsDeviceAvailable(LG_COLOR))
	{

	}

}

void Logitech::changeState(int state, string artist, string title)
{

}



VOID Logitech::CheckButtonPresses()
{
	if(m_lcd->IsDeviceAvailable(LG_MONOCHROME) && CheckbuttonPressesMonochrome())
	{
		InitLCDObjectsMonochrome();
	}

	else if(m_lcd->IsDeviceAvailable(LG_COLOR) && CheckbuttonPressesColor())
	{
		InitLCDObjectsColor();
	}
}

bool Logitech::CheckbuttonPressesMonochrome()
{
	bool buttonPressed = false;

	m_lcd->ModifyDisplay(LG_MONOCHROME);

	if (m_lcd->ButtonTriggered(LG_BUTTON_4))
	{

	}

	else if(m_lcd->ButtonTriggered(LG_BUTTON_1))
	{

	}

	else if(m_lcd->ButtonTriggered(LG_BUTTON_2))
	{

	}

	else if(m_lcd->ButtonTriggered(LG_BUTTON_3))
	{

	}

	return buttonPressed;
}

bool Logitech::CheckbuttonPressesColor()
{
	m_lcd->ModifyDisplay(LG_COLOR);

	bool buttonPressed = false;

	if (m_lcd->ButtonReleased(LG_BUTTON_RIGHT))
	{

	}

	if (m_lcd->ButtonReleased(LG_BUTTON_LEFT))
	{

	}

	if (m_lcd->ButtonReleased(LG_BUTTON_DOWN))
	{

	}

	if (m_lcd->ButtonReleased(LG_BUTTON_UP))
	{

		buttonPressed = true;
	}

	return buttonPressed;
}