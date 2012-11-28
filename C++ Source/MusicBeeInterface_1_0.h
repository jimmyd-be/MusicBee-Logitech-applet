namespace MusicBeePlugin 
{ 
	const short PluginInfoVersion = 1;
	const short MinInterfaceVersion = 5;
	const short MinApiRevision = 8;

	enum PluginType
	{
		General = 1,						// plugin runs in the background
		LyricsRetrieval = 2,
		ArtworkRetrieval = 3,
		PanelView = 4,						// not implemented yet
		DataStream = 5,						// not implemented yet
		InstanstMessenger	= 6,
        Storage = 7
	};

	enum ReceiveNotificationFlags
	{
        StartupOnly = 0x0,
		PlayerEvents = 0x01,
		DataStreamEvents = 0x02,
        TagEvents = 0x04
	};

	enum NotificationType
	{
        PluginStartup = 0,						// notification sent after successful initialisation for an enabled plugin
        TrackChanged = 1,
        PlayStateChanged = 2,
        AutoDjStarted = 3,
        AutoDjStopped = 4,
        VolumeMuteChanged = 5,
        VolumeLevelChanged = 6,
        NowPlayingListChanged = 7,
        NowPlayingArtworkReady = 8,
        NowPlayingLyricsReady = 9,
        TagsChanging = 10,
        TagsChanged = 11,
        RatingChanged = 12,
        PlayCountersChanged = 13
	};

    enum CallbackType
    {
        SettingsUpdated = 1,
        StorageReady = 2,
        StorageFailed = 3,
        FilesRetrievedChanged = 4,
        FilesRetrievedNoChange = 5,
        FilesRetrievedFail = 6
    };

	enum PluginCloseReason
	{
		MusicBeeClosing = 1,
		UserDisabled = 2
	};

	enum FilePropertyType
	{
        Url = 2,
		Kind = 4,
		Format = 5,
		Size = 7,
		Channels = 8,
		SampleRate = 9,
		Bitrate = 10,
		DateModified = 11,
		DateAdded = 12,
		LastPlayed = 13,
		PlayCount = 14,
		SkipCount = 15,
		Duration = 16,
		ReplayGainTrack = 94,
		ReplayGainAlbum = 95
	};

	enum MetaDataType
	{
		TrackTitle = 65,
		Album = 30,
		AlbumArtist = 31,     // displayed album artist
		AlbumArtistRaw = 34,  // stored album artist
        Artist = 32,          // displayed artist
        MultiArtist = 33,     // individual artists, separated by a null char
		Artwork = 40,
		BeatsPerMin = 41,
        Composer = 43,        // displayed composer
        MultiComposer = 89,   // individual composers, separated by a null char
		Comment = 44,
		Conductor = 45,
		Custom1 = 46,
		Custom2 = 47,
		Custom3 = 48,
		Custom4 = 49,
		Custom5 = 50,
		Custom6 = 96,
		Custom7 = 97,
		Custom8 = 98,
		Custom9 = 99,
		DiscNo = 52,
		DiscCount = 54,
		Encoder = 55,
		Genre = 59,
		GenreCategory = 60,
		Grouping = 61,
                HasLyrics = 63,
		Keywords = 84,
		Lyricist = 62,
		Mood = 64,
		Occasion = 66,
		Origin = 67,
		Publisher = 73,
		Quality = 74,
		Rating = 75,
		RatingLove = 76,
		RatingAlbum = 104,
		Tempo = 85,
		TrackNo = 86,
		TrackCount = 87,
		Virtual1 = 109,
		Virtual2 = 110,
		Virtual3 = 111,
		Year = 88
	};

	enum LyricsType
	{
		NotSpecified = 0,
		Synchronised = 1,
		UnSynchronised = 2
	};

	enum PlayState
	{
        Undefined = 0,
        Loading = 1,
        Playing = 3,
        Paused = 6,
        Stopped = 7
	};

	enum RepeatMode
	{
		None = 0,
		All = 1,
		One = 2
	};

    enum PlaylistFormat
	{
		Unknown = 0,
        M3u = 1,
        Xspf = 2,
        Asx = 3,
        Wpl = 4,
        Pls = 5,
        Auto = 7,
        M3uAscii = 8
	};

	enum SkinElement
	{
		SkinInputControl = 7,
		SkinInputPanel = 10,
		SkinInputPanelLabel = 14
	};

	enum ElementState
	{
        ElementStateDefault = 0,
        ElementStateModified = 6
	};

	enum ElementComponent
	{
        ComponentBorder = 0,
        ComponentBackground = 1,
        ComponentForeground = 3
	};

	struct PluginInfo
	{
		short PluginInfoVersion;
        PluginType Type;
        LPCWSTR Name;
        LPCWSTR Description;
        LPCWSTR Author;
        LPCWSTR TargetApplication;
        short VersionMajor;
        short VersionMinor;
        short Revision;
		short MinInterfaceVersion;
		short MinApiRevision;
		ReceiveNotificationFlags ReceiveNotifications;
		int ConfigurationPanelHeight;
		int reserved1;
		int reserved2;
		int reserved3;
		int reserved4;
		int reserved5;
	};

	// api calls available to MusicBee
	// note that MB_ReleaseString needs to be called to release any string value returned by an API call
	struct MusicBeeApiInterface
	{
		short InterfaceVersion;
		short ApiRevsion;
		void (__stdcall *MB_ReleaseString)(LPCWSTR value);		// required to release a string returned by any of the functions below
		void (__stdcall *MB_Trace)(LPCWSTR p1);
		LPCWSTR (__stdcall *Setting_GetPersistentStoragePath)(); 
		LPCWSTR (__stdcall *Setting_GetSkin)(); 
		int (__stdcall *Setting_GetSkinElementColour)(SkinElement element, ElementState state, ElementComponent component); 
		bool (__stdcall *Setting_IsWindowBordersSkinned)(); 
		LPCWSTR (__stdcall *Library_GetFileProperty)(LPCWSTR sourceFileUrl, FilePropertyType type); 
		LPCWSTR (__stdcall *Library_GetFileTag)(LPCWSTR sourceFileUrl, MetaDataType type); 
		bool (__stdcall *Library_SetFileTag)(LPCWSTR sourceFileUrl, MetaDataType type, LPCWSTR value);   // only updates the cache - call Library_CommitTagsToFile to save the changes
		bool (__stdcall *Library_CommitTagsToFile)(LPCWSTR sourceFileUrl);
		LPCWSTR (__stdcall *Library_GetLyrics)(LPCWSTR sourceFileUrl, LyricsType type); 
		LPCWSTR (__stdcall *Library_GetArtwork)(LPCWSTR sourceFileUrl, int index); 
		bool (__stdcall *Library_QueryFiles)(LPCWSTR query);  // use NULL to retrieve all files, query parameter is ignored for now
		LPCWSTR (__stdcall *Library_QueryGetNextFile)();	  // returns file path or NULL when no more files
		int (__stdcall *Player_GetPosition)(); 
		bool (__stdcall *Player_SetPosition)(int position); 
		PlayState (__stdcall *Player_GetPlayState)(); 
		bool (__stdcall *Player_PlayPause)(); 
		bool (__stdcall *Player_Stop)(); 
		bool (__stdcall *Player_StopAfterCurrent)(); 
		bool (__stdcall *Player_PlayPreviousTrack)(); 
		bool (__stdcall *Player_PlayNextTrack)(); 
		bool (__stdcall *Player_StartAutoDj)(); 
		bool (__stdcall *Player_EndAutoDj)(); 
		float (__stdcall *Player_GetVolume)();
		bool (__stdcall *Player_SetVolume)(float volume);
		bool (__stdcall *Player_GetMute)();
		bool (__stdcall *Player_SetMute)(bool volume);
        bool (__stdcall *Player_GetShuffle)();
        bool (__stdcall *Player_SetShuffle)(bool shuffle);
        RepeatMode (__stdcall *Player_GetRepeat)();
        bool (__stdcall *Player_SetRepeat)(RepeatMode shuffle);
        bool (__stdcall *Player_GetEqualiserEnabled)();
        bool (__stdcall *Player_SetEqualiserEnabled)(bool enabled);
        bool (__stdcall *Player_GetDspEnabled)();
        bool (__stdcall *Player_SetDspEnabled)(bool enabled);
        bool (__stdcall *Player_GetScrobbleEnabled)();
        bool (__stdcall *Player_SetScrobbleEnabled)(bool enabled);
		LPCWSTR (__stdcall *NowPlaying_GetFileUrl)(); 
		int (__stdcall *NowPlaying_GetDuration)(); 
		LPCWSTR (__stdcall *NowPlaying_GetFileProperty)(FilePropertyType type); 
		LPCWSTR (__stdcall *NowPlaying_GetFileTag)(MetaDataType type); 
		LPCWSTR (__stdcall *NowPlaying_GetLyrics)(); 
		LPCWSTR (__stdcall *NowPlaying_GetArtwork)(); 
		bool (__stdcall *NowPlayingList_Clear)(); 
		bool (__stdcall *NowPlayingList_QueryFiles)(LPCWSTR query);  // use NULL to retrieve all files, query parameter is ignored for now
		LPCWSTR (__stdcall *NowPlayingList_QueryGetNextFile)();		 // returns file path or NULL when no more files
		bool (__stdcall *NowPlayingList_PlayNow)(LPCWSTR sourceFileUrl); 
		bool (__stdcall *NowPlayingList_QueueNext)(LPCWSTR sourceFileUrl); 
		bool (__stdcall *NowPlayingList_QueueLast)(LPCWSTR sourceFileUrl); 
		bool (__stdcall *NowPlayingList_PlayLibraryShuffled)(); 
		bool (__stdcall *Playlist_QueryPlaylists)();
		LPCWSTR (__stdcall *Playlist_QueryGetNextPlaylist)();		 // returns playlist path or NULL when no more playlists
		PlaylistFormat (__stdcall *Playlist_GetPlaylistType)(LPCWSTR playlistUrl);
		bool (__stdcall *Playlist_QueryFiles)(LPCWSTR playlistUrl);
		LPCWSTR (__stdcall *Playlist_QueryGetNextFile)();			 // returns file path or NULL when no more files
		int * (__stdcall *MB_GetWindowHandle)();
		void (__stdcall *MB_RefreshPanels)();
        void (__stdcall *MB_SendNotification)(CallbackType type);
        void * (__stdcall *MB_AddMenuItem)(LPCWSTR menuPath, LPCWSTR hotkeyDescription, void * handler);  // returns a System.Windows.Forms.ToolStripItem object
        void (__stdcall *Setting_GetFieldNameDelegate)(MetaDataType type);
		LPCWSTR (__stdcall *Library_QueryGetAllFiles)();			// iterate until zero length substring
		LPCWSTR (__stdcall *NowPlayingList_QueryGetAllFiles)();
		LPCWSTR (__stdcall *Playlist_QueryGetAllFiles)();
		void (__stdcall *MB_CreateBackgroundTask)(void * taskCallback, void * owner);
		void (__stdcall *MB_SetBackgroundTaskMessage)(LPCWSTR message);
	};

	__declspec(dllexport) PluginInfo* Initialise(MusicBeeApiInterface* apiInterface); 
	__declspec(dllexport) int Configure(void *); 
	__declspec(dllexport) void SaveSettings(); 
	__declspec(dllexport) void Close(PluginCloseReason reason);
	__declspec(dllexport) void Uninstall(); 
	__declspec(dllexport) void ReceiveNotification(LPCWSTR sourceFileUrl, NotificationType type); 
	__declspec(dllexport) LPCWSTR* GetProviders();
	__declspec(dllexport) LPCWSTR RetrieveLyrics(LPCWSTR sourceFileUrl, LPCWSTR artist, LPCWSTR trackTitle, LPCWSTR album, bool synchronisedPreferred, LPCWSTR provider);
	__declspec(dllexport) LPCWSTR RetrieveArtwork(LPCWSTR sourceFileUrl, LPCWSTR albumArtist, LPCWSTR album, LPCWSTR provider);
	// storage plugin only:
	__declspec(dllexport) void Refresh();
	__declspec(dllexport) bool IsReady();
	__declspec(dllexport) void* GetIcon();
	__declspec(dllexport) bool FolderExists(LPCWSTR path);
	__declspec(dllexport) LPCWSTR * GetFolders(LPCWSTR path);
	__declspec(dllexport) void * GetFiles(LPCWSTR path);
	__declspec(dllexport) bool FileExists(LPCWSTR url);
	__declspec(dllexport) void * GetFile(LPCWSTR url);
	__declspec(dllexport) void * GetFileArtwork(LPCWSTR url);
	__declspec(dllexport) void * GetPlaylists();
	__declspec(dllexport) void * GetPlaylistFiles(LPCWSTR id);
	__declspec(dllexport) void * GetStream(LPCWSTR url);
	__declspec(dllexport) void * GetError();
}
