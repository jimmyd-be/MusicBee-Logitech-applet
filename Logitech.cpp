
// Logitech.cpp : implementation file
//

#include "stdafx.h"
#include "Logitech.h"

Logitech * Logitech::object;

Logitech::Logitech()
{
	object = this;
	stopthread = false;
}

Logitech::~Logitech()
{
	stopthread = true;
	this->state = -1;
	timerThread.detach();
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

	timerThread = thread(&Logitech::startThread);

	if(m_lcd.IsDeviceAvailable(LG_COLOR))
	{
		InitLCDObjectsColor();
	}

	else if(m_lcd.IsDeviceAvailable(LG_MONOCHROME))
	{
		InitLCDObjectsMonochrome();
	}

	return TRUE;  // return TRUE  unless you set the focus to a control
}

void Logitech::startThread()
{
	while(!object->stopthread)
	{
		this_thread::sleep_for( chrono::milliseconds(500) );
		if(object->state == 3)
		{
			this_thread::sleep_for( chrono::milliseconds(500) );
			object->position++;

			object->m_lcd.SetProgressBarPosition(object->progressbar, static_cast<FLOAT>(((float)object->position / object->duration)*100));
			object->m_lcd.SetText(object->time, object->getPositionString().c_str());
		}

		else if(object->state == 7)
		{
			object->position = 0;
			object->m_lcd.SetProgressBarPosition(object->progressbar, 0);
			object->m_lcd.SetText(object->time, object->getPositionString().c_str());
		}

		object->m_lcd.Update();
	}
}

VOID Logitech::InitLCDObjectsMonochrome()
{
	m_lcd.ModifyDisplay(LG_MONOCHROME);

	m_lcd.RemovePage(0);
	m_lcd.AddNewPage();
	m_lcd.ShowPage(0);

	logo = m_lcd.AddText(LG_STATIC_TEXT, LG_BIG, DT_CENTER, LGLCD_BW_BMP_WIDTH);
	m_lcd.SetOrigin(logo, 0, 5);
	m_lcd.SetText(logo, _T("MusicBee"));
	m_lcd.Update();
}

VOID Logitech::InitLCDObjectsColor()
{
	m_lcd.ModifyDisplay(LG_COLOR);

	m_lcd.RemovePage(0);
	m_lcd.AddNewPage();
	m_lcd.ShowPage(0);

	logo = m_lcd.AddText(LG_STATIC_TEXT, LG_BIG, DT_CENTER, LGLCD_BW_BMP_WIDTH);
	m_lcd.SetOrigin(logo, 0, 50);
	m_lcd.SetText(logo, _T("MusicBee"));
	m_lcd.Update();
}

void Logitech::changeArtistTitle(wstring artistStr, wstring titleStr, wstring duration, int position)
{
	this->artistString = artistStr;
	this->titleString = titleStr;
	this->durationString = duration;
	this->position = position;
	this->duration = getDuration(duration);

	m_lcd.RemovePage(0);
	m_lcd.AddNewPage();
	m_lcd.ShowPage(0);

	if (logo != 0)
	{
		delete logo;
		logo = 0;
	}

	if(m_lcd.IsDeviceAvailable(LG_COLOR))
	{
		artist = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_LEFT, 320);
		m_lcd.SetOrigin(artist, 5, 5);
		m_lcd.SetText(artist, artistStr.c_str());

		title = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_LEFT, 320);
		m_lcd.SetOrigin(title, 5, 30);
		m_lcd.SetText(title, titleStr.c_str());

		time = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd.SetOrigin(time, 5, 55);
		m_lcd.SetText(time, getPositionString().c_str());

		string s( duration.begin(), duration.end() );

		if(s.size() < 5)
		{
			s = "0" + s;
		}

		wstring ws( s.begin(), s.end() );
		time1 = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd.SetOrigin(time1, 275, 55);
		m_lcd.SetText(time1, ws.c_str());
		ws.clear();
	
		progressbar = m_lcd.AddProgressBar(LG_DOT_CURSOR);//320×240 pixel color screen
		m_lcd.SetProgressBarSize(progressbar, 310, 15);
		m_lcd.SetOrigin(progressbar, 5, 75);
		m_lcd.SetProgressBarPosition(progressbar, static_cast<FLOAT>(((float)position / (float)this->duration)*100));
		
		/*playIcon = static_cast<HICON>(LoadImage(AfxGetInstanceHandle(), MAKEINTRESOURCE(IDB_PNG1), IMAGE_ICON, 16, 16, LR_COLOR));
		playIconHandle = m_lcd.AddIcon(playIcon, 16, 16);
		m_lcd.SetOrigin(playIconHandle, 5, 29);*/

		m_lcd.Update();
	}

	else if(m_lcd.IsDeviceAvailable(LG_MONOCHROME))
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
		m_lcd.SetProgressBarPosition(progressbar, static_cast<FLOAT>(((float)this->position / this->duration)*100));

		time = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd.SetOrigin(time, 12, 29);
		m_lcd.SetText(time, getPositionString().c_str());

		string s( duration.begin(), duration.end() );

		if(s.size() < 5)
		{
			s = "0" + s;
		}

		wstring ws( s.begin(), s.end() );
		time1 = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
		m_lcd.SetOrigin(time1, 125, 29);
		m_lcd.SetText(time1, ws.c_str());
		ws.clear();
		
		/*	playIcon = static_cast<HICON>(LoadImage(AfxGetInstanceHandle(), MAKEINTRESOURCE(IDB_PNG1), IMAGE_ICON, 16, 16, LR_MONOCHROME));
		playIconHandle = m_lcd.AddIcon(playIcon, 16, 16);
		m_lcd.SetOrigin(playIconHandle, 2, 29);*/

		m_lcd.Update();
	}

	artistStr.clear();
	titleStr.clear();
	duration.clear();
}

void Logitech::setPosition(int pos)
{
	if(pos <= this->duration)
	{
		this->position = pos;
		m_lcd.SetText(time1, getPositionString().c_str());
	}
}
/*Undefined = 0,
Loading = 1,
Playing = 3,
Paused = 6,
Stopped = 7*/
void Logitech::changeState(int state)
{
	this->state = state;
}

int Logitech::getDuration(wstring duration)
{
	string s( duration.begin(), duration.end() );

	int position = s.find(":");
	string minutes = s.substr(0, s.size() -position);
	string seconds = s.substr(position);
	int minutesInt = atoi(minutes.c_str());
	int secondsInt = atoi(seconds.c_str());

	return (minutesInt *60) + secondsInt;
}

wstring Logitech::getPositionString()
{
	string minutes = to_string((int)position /60);
	string seconds = to_string((int)position%60);

	if(minutes.size() < 2)
	{
		minutes = "0" + minutes;
	}

	if(seconds.size() < 2)
	{
		seconds = "0" + seconds;
	}

	string time = minutes + ":" + seconds;

	return wstring( time.begin(), time.end() );
}

/*VOID Logitech::CheckButtonPresses()
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
}*/