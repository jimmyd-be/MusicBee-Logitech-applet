// LogitechDll
#include <string>
#include "stdafx.h"
using namespace std;

namespace LogitechLCDDll
{
	
    class LogitechDll
    {
		
    public:
        // Initialize
        static __declspec(dllexport) bool Initialize();

        // DeInitialize
        static __declspec(dllexport) void DeInitialize();

        // changeArtistTitle
        static __declspec(dllexport) void changeArtistTitle(string artist, string title);

		// changeState
        static __declspec(dllexport) void changeState(int state);
		
    };
}