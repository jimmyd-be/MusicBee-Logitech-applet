//-----------------------------------------------------------------
// Logitech File
// C++ Source - Logitech.cpp - version 2012 v1.0
//-----------------------------------------------------------------

//-----------------------------------------------------------------
// Include Files
//-----------------------------------------------------------------
#include "stdafx.h"
#include "Logitech.h"

//-----------------------------------------------------------------
// Logitech methods
//-----------------------------------------------------------------

//This LogitechObject is a instance of the Logitech class for using in the thread
Logitech * Logitech::LogitechObject;

Logitech::Logitech():	stopthread(false), firstTime(true), position(0), duration(0)
{
	LogitechObject = this;
}

Logitech::~Logitech()
{
	stopthread = true;
	this->state = StatePlay::Undefined;
	timerThread.detach();
}

bool Logitech::getFirstTime()
{
	return firstTime;
}

//Initialise Logitech LCD
BOOL Logitech::OnInitDialog()
{
	HRESULT hRes = m_lcd.Initialize(_T("MusicBee"), LG_DUAL_MODE, FALSE, TRUE);

	if (hRes != S_OK)
	{
		return FALSE;
	}

	//Create home screen Logitech Color LCD
	if(m_lcd.IsDeviceAvailable(LG_COLOR))
	{
		m_lcd.ModifyDisplay(LG_COLOR);
		logo = m_lcd.AddText(LG_STATIC_TEXT, LG_BIG, DT_CENTER, LGLCD_BW_BMP_WIDTH);
		m_lcd.SetOrigin(logo, 0, 50);
		m_lcd.SetText(logo, _T("MusicBee"));
		m_lcd.Update();
	}

	//Create home screen Logitech Monochrome LCD
	else if(m_lcd.IsDeviceAvailable(LG_MONOCHROME))
	{
		m_lcd.ModifyDisplay(LG_MONOCHROME);
		logo = m_lcd.AddText(LG_STATIC_TEXT, LG_BIG, DT_CENTER, LGLCD_BW_BMP_WIDTH);
		m_lcd.SetOrigin(logo, 0, 5);
		m_lcd.SetText(logo, _T("MusicBee"));
		m_lcd.Update();
	}

	//Start thread
	timerThread = thread(&Logitech::startThread);

	return TRUE;  // return TRUE  unless you set the focus to a control
}

//Create playing screen for Logitech Monochrome LCD
VOID Logitech::createMonochrome()
{
	m_lcd.RemovePage(0);
	m_lcd.AddNewPage();
	m_lcd.ShowPage(0);

	if (logo != 0)
	{
		delete logo;
		logo = 0;
	}

	artist = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_BW_BMP_WIDTH);
	m_lcd.SetOrigin(artist, 0, 0);

	title = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_BW_BMP_WIDTH);
	m_lcd.SetOrigin(title, 0, 13);

	progressbar = m_lcd.AddProgressBar(LG_FILLED);
	m_lcd.SetProgressBarSize(progressbar, 136, 5);
	m_lcd.SetOrigin(progressbar, 12, 38);

	time = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
	m_lcd.SetOrigin(time, 12, 29);

	time1 = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
	m_lcd.SetOrigin(time1, 125, 29);

	/*	playIcon = static_cast<HICON>(LoadImage(GetModuleHandle(NULL), MAKEINTRESOURCE(IDB_PNG2), IMAGE_BITMAP, 16, 16, LR_MONOCHROME));
	playIconHandle = m_lcd.AddIcon(playIcon, 16, 16);
	m_lcd.SetOrigin(playIconHandle, 2, 29);*/

	firstTime = false;
	changeArtistTitle(this->artistString, this->albumString, this->titleString, this->durationString, this->position);
}

//Create playing screen for Logitech Color LCD
VOID Logitech::createColor()
{
	m_lcd.RemovePage(0);
	m_lcd.AddNewPage();
	m_lcd.ShowPage(0);

	if (logo != 0)
	{
		delete logo;
		logo = 0;
	}

	artist = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, 320);
	m_lcd.SetOrigin(artist, 5, 5);

	album = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, 320);
	m_lcd.SetOrigin(album, 5, 30);

	title = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, 320);
	m_lcd.SetOrigin(title, 5, 55);

	time = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
	m_lcd.SetOrigin(time, 5, 80);

	time1 = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
	m_lcd.SetOrigin(time1, 275, 80);

	progressbar = m_lcd.AddProgressBar(LG_DOT_CURSOR);//320×240 pixel color screen
	m_lcd.SetProgressBarSize(progressbar, 310, 15);
	m_lcd.SetOrigin(progressbar, 5, 100);

	time1 = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
	m_lcd.SetOrigin(time1, 275, 80);

	/*playIcon = static_cast<HICON>(LoadImage(AfxGetInstanceHandle(), MAKEINTRESOURCE(IDB_PNG1), IMAGE_ICON, 16, 16, LR_COLOR));
	playIconHandle = m_lcd.AddIcon(playIcon, 16, 16);
	m_lcd.SetOrigin(playIconHandle, 5, 29);*/

	firstTime = false;
	changeArtistTitle(this->artistString, this->albumString, this->titleString, this->durationString, this->position);
}

void Logitech::startThread()
{
	while(!LogitechObject->stopthread)
	{
		this_thread::sleep_for( chrono::milliseconds(500) );

		if(!LogitechObject->stopthread && LogitechObject->progressbar != NULL)
		{
			//Update progressbar and position time on the screen after 1 second of music.
			if(LogitechObject->state == StatePlay::Playing)
			{
				this_thread::sleep_for( chrono::milliseconds(500) );
				LogitechObject->position++;

				LogitechObject->m_lcd.SetProgressBarPosition(LogitechObject->progressbar, static_cast<FLOAT>(((float)LogitechObject->position / LogitechObject->duration)*100));
				LogitechObject->m_lcd.SetText(LogitechObject->time, LogitechObject->getPositionString().c_str());
			}

			//If music stopped then the progressbar and time must stop immediately
			else if(LogitechObject->state == StatePlay::Stopped)
			{
				LogitechObject->position = 0;
				LogitechObject->m_lcd.SetProgressBarPosition(LogitechObject->progressbar, 0);
				LogitechObject->m_lcd.SetText(LogitechObject->time, LogitechObject->getPositionString().c_str());
			}

			LogitechObject->m_lcd.Update();
		}
	}
}

void Logitech::changeArtistTitle(wstring artistStr, wstring albumStr, wstring titleStr, wstring duration, int position)
{
	this->artistString = artistStr;
	this->albumString = albumStr;
	this->titleString = titleStr;
	this->durationString = duration;
	this->position = position;
	this->duration = getDuration(duration);

	if(!firstTime)
	{
		if(m_lcd.IsDeviceAvailable(LG_COLOR))
		{
			m_lcd.SetText(album, albumStr.c_str());
		}


		m_lcd.SetText(artist, artistStr.c_str());
		m_lcd.SetText(title, titleStr.c_str());
		m_lcd.SetText(time, getPositionString().c_str());

		string s( duration.begin(), duration.end() );

		if(s.size() < 5)
		{
			s = "0" + s;
		}

		wstring ws( s.begin(), s.end() );

		m_lcd.SetText(time1, ws.c_str());
		ws.clear();

		///*playIcon = static_cast<HICON>(LoadImage(AfxGetInstanceHandle(), MAKEINTRESOURCE(IDB_PNG1), IMAGE_ICON, 16, 16, LR_COLOR));
		//playIconHandle = m_lcd.AddIcon(playIcon, 16, 16);
		//m_lcd.SetOrigin(playIconHandle, 5, 29);*/

		m_lcd.Update();
		artistStr.clear();
		albumStr.clear();
		titleStr.clear();
		duration.clear();
	}
}

//Set current playing position
void Logitech::setPosition(int pos)
{
	this->position = pos/1000;
	m_lcd.SetText(time, getPositionString().c_str());
	m_lcd.Update();
}

//Change play state of the current playing song
void Logitech::changeState(StatePlay state)
{
	this->state = state;

	if(state == StatePlay::Playing && firstTime)
	{
		if(m_lcd.IsDeviceAvailable(LG_COLOR))
		{
			createColor();
		}

		else if(m_lcd.IsDeviceAvailable(LG_MONOCHROME))
		{
			createMonochrome();
		}
	}
}

//Gets the music duration
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

//Change current position in string
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