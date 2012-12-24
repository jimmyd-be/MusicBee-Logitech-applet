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

	m_lcd.SetAsForeground(true);

	//Create home screen Logitech Color LCD
	if(m_lcd.IsDeviceAvailable(LG_COLOR))
	{
		m_lcd.ModifyDisplay(LG_COLOR);
		m_lcd.SetBackground(RGB(245,245,245));
		logo = m_lcd.AddText(LG_STATIC_TEXT, LG_BIG, DT_CENTER, LGLCD_QVGA_BMP_WIDTH);
		m_lcd.SetOrigin(logo, 0, 50);
		m_lcd.SetText(logo, _T("MusicBee"));
		m_lcd.SetTextFontColor(logo, RGB(0,0,0));
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
	changeArtistTitle(this->artistString, this->albumString, this->titleString, this->duration, this->position);
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

	//background.LoadFromResource(NULL, AfxGetInstanceHandle(), IDB_G19BACKGROUND, _T("PNG"));
    //HBITMAP bmpBkg_ = background.GetHBITMAP();
    //m_lcd.SetBackground(bmpBkg_);
		
	m_lcd.SetBackground(RGB(184,220,240));

	artist = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_QVGA_BMP_WIDTH);
	m_lcd.SetOrigin(artist, 5, 5);
	m_lcd.SetTextFontColor(artist, RGB(0,0,0));

	album = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_QVGA_BMP_WIDTH);
	m_lcd.SetOrigin(album, 5, 30);
	m_lcd.SetTextFontColor(album, RGB(0,0,0));

	title = m_lcd.AddText(LG_SCROLLING_TEXT, LG_MEDIUM, DT_CENTER, LGLCD_QVGA_BMP_WIDTH);
	m_lcd.SetOrigin(title, 5, 55);
	m_lcd.SetTextFontColor(title, RGB(0,0,0));

	time = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 80);
	m_lcd.SetOrigin(time, 5, 80);
	m_lcd.SetTextFontColor(time, RGB(0,0,0));

	time1 = m_lcd.AddText(LG_STATIC_TEXT, LG_SMALL, DT_LEFT, 40);
	m_lcd.SetOrigin(time1, 275, 80);
	m_lcd.SetTextFontColor(time1, RGB(0,0,0));

	progressbar = m_lcd.AddProgressBar(LG_FILLED);//320×240 pixel color screen
	m_lcd.SetProgressBarSize(progressbar, 310, 20);
	m_lcd.SetProgressBarColors(progressbar, RGB(25,71,94),NULL);
	m_lcd.SetOrigin(progressbar, 5, 100);

	/*playIcon = static_cast<HICON>(LoadImage(AfxGetInstanceHandle(), MAKEINTRESOURCE(IDB_PNG1), IMAGE_ICON, 16, 16, LR_COLOR));
	playIconHandle = m_lcd.AddIcon(playIcon, 16, 16);
	m_lcd.SetOrigin(playIconHandle, 5, 29);*/

	firstTime = false;
	changeArtistTitle(this->artistString, this->albumString, this->titleString, this->duration, this->position);
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
				float progresstime = ((float)LogitechObject->position / (float)LogitechObject->duration)*100;
				LogitechObject->m_lcd.SetProgressBarPosition(LogitechObject->progressbar, static_cast<FLOAT>(progresstime));
				LogitechObject->m_lcd.SetText(LogitechObject->time, LogitechObject->getTimeString(LogitechObject->position).c_str());
			}

			//If music stopped then the progressbar and time must stop immediately
			else if(LogitechObject->state == StatePlay::Stopped)
			{
				LogitechObject->position = 0;
				LogitechObject->m_lcd.SetProgressBarPosition(LogitechObject->progressbar, 0);
				LogitechObject->m_lcd.SetText(LogitechObject->time, LogitechObject->getTimeString(LogitechObject->position).c_str());
			}

			LogitechObject->m_lcd.Update();
		}
	}
}

void Logitech::changeArtistTitle(wstring artistStr, wstring albumStr, wstring titleStr, int duration, int position)
{
	this->artistString = artistStr;
	this->albumString = albumStr;
	this->titleString = titleStr;
	this->durationString = getTimeString(duration/1000);
	this->position = position;
	this->duration = duration/1000;

	if(!firstTime)
	{
		if(m_lcd.IsDeviceAvailable(LG_COLOR))
		{
			m_lcd.SetText(album, albumStr.c_str());
		}


		m_lcd.SetText(artist, artistStr.c_str());
		m_lcd.SetText(title, titleStr.c_str());
		m_lcd.SetText(time, getTimeString(position).c_str());

		string s( durationString.begin(), durationString.end() );

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

	}
}

//Set current playing position
void Logitech::setPosition(int pos)
{
	this->position = pos/1000;
	m_lcd.SetText(time, getTimeString(this->position).c_str());
	m_lcd.Update();
}

void Logitech::setDuration(int duration)
{
	this->duration = duration/1000;
	m_lcd.SetText(time1, getTimeString(this->duration).c_str());
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


//Change int of time to string
wstring Logitech::getTimeString(int time)
{
	string minutes = to_string((int)time /60);
	string seconds = to_string((int)time%60);

	if(minutes.size() < 2)
	{
		minutes = "0" + minutes;
	}

	if(seconds.size() < 2)
	{
		seconds = "0" + seconds;
	}

	string timeString = minutes + ":" + seconds;

	return wstring( timeString.begin(), timeString.end() );
}