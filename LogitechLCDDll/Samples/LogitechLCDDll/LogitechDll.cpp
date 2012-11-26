// LogitechDll

#include "LogitechDll.h"
#include "Logitech.h"
#include "stdafx.h"

using namespace std;

namespace LogitechLCDDll
{

	Logitech * logitech;
	
		 // Initialize
        bool LogitechDll::Initialize()
		{
			logitech = new Logitech();
			
			return logitech->OnInitDialog();
		}

        // DeInitialize
        void LogitechDll::DeInitialize()
		{
			delete logitech;
			logitech = 0;
		}

        // changeArtistTitle
        void LogitechDll::changeArtistTitle(string artist, string title)
		{

		}

		// changeState
        void LogitechDll::changeState(int state)
		{

		}
}