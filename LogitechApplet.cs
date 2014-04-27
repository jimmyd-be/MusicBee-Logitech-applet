using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using GammaJul.LgLcd;
using MusicBeePlugin.Screens;
using System.IO;
using System.Threading;

namespace MusicBeePlugin
{
  public partial class Plugin
  {
    public static Plugin pluginObject_;
    private bool eventHappened_ = true;
    private int alwaysOnTopCounter_ = 0;
    private long timerTime_ = 0;

    private MusicBeeApiInterface mbApiInterface_;
    private PluginInfo about_ = new PluginInfo();
    private List<Screen> lcdScreenList_ = new List<Screen>();
    private Settings settings_ = null;
    private LcdApplet applet_ = null;
    private LcdDevice device_ = null;

    private AutoResetEvent autoEvent = null;
    private System.Threading.Timer timer = null;

    private int currentPage_ = 0;

    public enum screenEnum { MainScreen, SettingsScreen, VolumeScreen, ControlScreen };
    public enum ScreenUseAllButtons { SettingsScreen, ControlScreen };

    public PluginInfo Initialise(IntPtr apiInterfacePtr)
    {
      mbApiInterface_ = new MusicBeeApiInterface();
      mbApiInterface_.Initialise(apiInterfacePtr);
      about_.PluginInfoVersion = PluginInfoVersion;
      about_.Name = "MusicBee V2";
      about_.Description = "Add support for Logitech G13, G15, G510, G19 LCD to MusicBee";
      about_.Author = "JimmyD";
      about_.TargetApplication = "MusicBee";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
      about_.Type = PluginType.General;
      about_.VersionMajor = 2;  // your plugin version
      about_.VersionMinor = 0;
      about_.Revision = 0;
      about_.MinInterfaceVersion = MinInterfaceVersion;
      about_.MinApiRevision = MinApiRevision;
      about_.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
      about_.ConfigurationPanelHeight = 0;   // not implemented yet: height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function
      return about_;
    }

    public bool Configure(IntPtr panelHandle)
    {
      settings_.Show();
      return true;
    }

    // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
    public void Close(PluginCloseReason reason)
    {
      if (settings_ != null)
      {
        settings_.Close();
        settings_ = null;
      }

      if (device_ != null)
      {
        device_.Dispose();
        device_ = null;
      }
    }

    // uninstall this plugin - clean up any persisted files
    public void Uninstall()
    {
      string path = mbApiInterface_.Setting_GetPersistentStoragePath() + "LogitechLCD\\";

      DirectoryInfo downloadedMessageInfo = new DirectoryInfo(path);

      foreach (FileInfo file in downloadedMessageInfo.GetFiles())
      {
        file.Delete();
      }
      foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
      {
        dir.Delete(true);
      }

      downloadedMessageInfo.Delete();
    }

    // receive event notifications from MusicBee
    // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
    public void ReceiveNotification(string sourceFileUrl, NotificationType type)
    {
      // perform some action depending on the notification type
      switch (type)
      {
        case NotificationType.PluginStartup:

          if (device_ == null)
          {
            settings_ = new Settings(mbApiInterface_.Setting_GetPersistentStoragePath(), this);

            connectDevice();

            settings_.openSettings();
            pluginObject_ = this;

            //Create an event to signal the timeout count threshold in the 
            // timer callback.
            autoEvent = new AutoResetEvent(false);

            // Create an inferred delegate that invokes methods for the timer.
            TimerCallback tcb = refreshLoop;

            // Create a timer that signals the delegate to invoke  
            // CheckStatus after one second, and every 1/4 second  
            // thereafter.
            timer = new System.Threading.Timer(tcb, autoEvent, 1000, 500);
          }
          eventHappened_ = true;
          break;

        case NotificationType.PlayStateChanged:

          if (lcdScreenList_.Count > 0 && lcdScreenList_[0].getName() == "StartupScreen")
          {
            openScreens();
          }

          stateChanged(mbApiInterface_.Player_GetPlayState());
          getSongData();
          getVolume();
          eventHappened_ = true;
          timerTime_ = mbApiInterface_.Player_GetPosition();

          break;

        case NotificationType.NowPlayingListChanged:
        case NotificationType.PlayerEqualiserOnOffChanged:
        case NotificationType.PlayerRepeatChanged:
        case NotificationType.PlayerShuffleChanged:
        case NotificationType.AutoDjStarted:
        case NotificationType.AutoDjStopped:
        case NotificationType.NowPlayingArtworkReady:
        case NotificationType.RatingChanged:
        case NotificationType.TrackChanged:
        case NotificationType.VolumeLevelChanged:
        case NotificationType.VolumeMuteChanged:
          if (lcdScreenList_.Count > 0 && lcdScreenList_[0].getName() == "StartupScreen")
          {
            openScreens();
          }

          getVolume();
          getSongData();
          eventHappened_ = true;
          timerTime_ = mbApiInterface_.Player_GetPosition();
          break;
      }
    }

    public static void refreshLoop(Object state)
    {
      AutoResetEvent autoEvent = (AutoResetEvent)state;

      if (!pluginObject_.settings_.alwaysOnTop_ && pluginObject_.eventHappened_)
      {
        pluginObject_.device_.SetAsForegroundApplet = true;
        pluginObject_.alwaysOnTopCounter_ += 500;

        if (pluginObject_.alwaysOnTopCounter_ >= 5000)
        {
          pluginObject_.eventHappened_ = false;
          pluginObject_.alwaysOnTopCounter_ = 0;
          pluginObject_.device_.SetAsForegroundApplet = false;
        }
      }

      if (pluginObject_.mbApiInterface_.Player_GetPlayState() == MusicBeePlugin.Plugin.PlayState.Playing)
      {
        pluginObject_.timerTime_ += 500;

        pluginObject_.lcdScreenList_[pluginObject_.currentPage_].positionChanged(Convert.ToInt32(pluginObject_.timerTime_) / 1000);
      }

      //Update progressbar and position time on the screen after 1 second of music.
      //if (pluginObject_.mbApiInterface_.Player_GetPlayState() == MusicBeePlugin.Plugin.PlayState.Playing && pluginObject_.timerTime_%1000 == 0)
      //{
      //  pluginObject_.lcdScreenList_[pluginObject_.currentPage_].positionChanged((int)(pluginObject_.timerTime_ / 1000));
      //}

      try
      {
        pluginObject_.device_.DoUpdateAndDraw();
      }
      catch (System.InvalidOperationException)
      {
        //Noting to do
      }
    }

    private void openScreens()
    {
      lcdScreenList_.Clear();

      int index = -1;

      foreach (string screenString in settings_.screenList_)
      {
        index++;

        Screen createdScreen = null;

        if (screenString == screenEnum.MainScreen.ToString())
        {
          createdScreen = new MainScreen(device_, device_.DeviceType, settings_.backgroundImage_, this, index);
        }

        else if (screenString == screenEnum.SettingsScreen.ToString())
        {
          createdScreen = new PlayerSettingsScreen(device_, device_.DeviceType, settings_.backgroundImage_, this, index);
        }
        else if (screenString == screenEnum.VolumeScreen.ToString() && device_.DeviceType == LcdDeviceType.Monochrome)
        {
          createdScreen = new VolumeScreen(device_, device_.DeviceType, settings_.backgroundImage_, this, index);
        }

        else if (screenString == screenEnum.ControlScreen.ToString())
        {
          createdScreen = new PlayerControlScreen(device_, device_.DeviceType, settings_.backgroundImage_, this, index);
        }

        if (createdScreen != null)
        {
          lcdScreenList_.Add(createdScreen);

          if (screenString == settings_.startupScreen_.ToString())
          {
            device_.CurrentPage = createdScreen;
            currentPage_ = index;
          }
        }
      }
    }

    public void connectDevice()
    {
      LcdAppletCapabilities appletCapabilities = LcdAppletCapabilities.Both;
      applet_ = new LcdApplet("MusicBee V2", appletCapabilities, false);

      try
      {
        applet_.DeviceArrival += new EventHandler<LcdDeviceTypeEventArgs>(applet_DeviceArrival);
        applet_.Connect();
      }
      catch (Exception)
      {

      }
    }

    private void applet_DeviceArrival(object sender, LcdDeviceTypeEventArgs e)
    {
      device_ = applet_.OpenDeviceByType(e.DeviceType);
      device_.SoftButtonsChanged += new EventHandler<LcdSoftButtonsEventArgs>(buttonPressed);

      device_.SetAsForegroundApplet = settings_.alwaysOnTop_;

      Screen startupScreen = new StartupScreen(device_, device_.DeviceType, null, this, 0);
      lcdScreenList_.Add(startupScreen);

      device_.CurrentPage = startupScreen;

      device_.DoUpdateAndDraw();
    }

    private void buttonPressed(object sender, LcdSoftButtonsEventArgs e)
    {
      LcdDevice device = (LcdDevice)sender;

      if (device.DeviceType == LcdDeviceType.Monochrome)
      {
        lcdScreenList_[currentPage_].buttonPressedMonochrome(sender, e);
      }

      else if (device.DeviceType == LcdDeviceType.Qvga)
      {
        lcdScreenList_[currentPage_].buttonPressedColor(sender, e);
      }
    }

    internal void goToPreviousPage()
    {
      currentPage_ -= 1;

      if (currentPage_ < 0)
      {
        currentPage_ = lcdScreenList_.Count;
      }

      device_.CurrentPage = lcdScreenList_[currentPage_];
    }

    internal void goToNextPage()
    {
      currentPage_ += 1;

      if (currentPage_ >= lcdScreenList_.Count)
      {
        currentPage_ = 0;
      }

      device_.CurrentPage = lcdScreenList_[currentPage_];
    }

    #region Changers

    internal void settingsChanged()
    {
      openScreens();
    }

    public void changeRating(float number)
    {
      String url = mbApiInterface_.NowPlaying_GetFileUrl();
      mbApiInterface_.Library_SetFileTag(url, MetaDataType.Rating, number.ToString());

      mbApiInterface_.Library_CommitTagsToFile(url);
      mbApiInterface_.MB_RefreshPanels();
    }

    public void changeVolume(float volume)
    {
      mbApiInterface_.Player_SetVolume(volume);
    }

    public void changeSettings(bool autoDJ, bool equaliser, bool shuffle, RepeatMode repeat)
    {
      bool autoDJMusicbee = mbApiInterface_.Player_GetAutoDjEnabled();
      bool equaliserMusicbee = mbApiInterface_.Player_GetEqualiserEnabled();
      RepeatMode repeatMusicbee = mbApiInterface_.Player_GetRepeat();
      bool shuffleMusicbee = mbApiInterface_.Player_GetShuffle();

      if (autoDJ && autoDJMusicbee != autoDJ)
      {
        mbApiInterface_.Player_StartAutoDj();
      }
      else if (autoDJMusicbee != autoDJ)
      {
        mbApiInterface_.Player_SetShuffle(true);
        mbApiInterface_.Player_SetShuffle(false);
      }

      if (equaliserMusicbee != equaliser)
      {
        mbApiInterface_.Player_SetEqualiserEnabled(equaliser);
      }

      if (repeat != repeatMusicbee)
      {

        mbApiInterface_.Player_SetRepeat(repeat);
      }

      if (shuffle != shuffleMusicbee)
      {
        mbApiInterface_.Player_SetShuffle(shuffle);
      }
    }

    public void changePlayState(int state)
    {
      switch (state)
      {
        case 0: mbApiInterface_.Player_PlayPause();
          break;

        case 1: mbApiInterface_.Player_PlayNextTrack();
          break;

        case 2: mbApiInterface_.Player_PlayPreviousTrack();
          break;

        case 3: mbApiInterface_.Player_Stop();
          break;

        case 4: mbApiInterface_.Player_Stop();
          mbApiInterface_.Player_PlayPause();
          break;
      }
    }

    #endregion

    #region Getters
    internal void getSongData()
    {
      string artist = mbApiInterface_.NowPlaying_GetFileTag(MetaDataType.Artist);
      string album = mbApiInterface_.NowPlaying_GetFileTag(MetaDataType.Album);
      string title = mbApiInterface_.NowPlaying_GetFileTag(MetaDataType.TrackTitle);
      string artwork = mbApiInterface_.NowPlaying_GetArtwork();
      string ratingString = mbApiInterface_.NowPlaying_GetFileTag(MetaDataType.Rating);

      float rating = 0;

      if (ratingString != "")
      {
        rating = Convert.ToSingle(ratingString);
      }

      foreach (Screen screen in lcdScreenList_)
      {
        screen.songChanged(artist, album, title, rating, artwork, mbApiInterface_.NowPlaying_GetDuration() / 1000, mbApiInterface_.Player_GetPosition() / 1000);
      }
    }

    public void getVolume()
    {
      foreach (Screen screen in lcdScreenList_)
      {
        screen.volumeChanged(mbApiInterface_.Player_GetVolume());
      }
    }

    private void getPosition()
    {
      foreach (Screen screen in lcdScreenList_)
      {
        screen.positionChanged(mbApiInterface_.Player_GetPosition() / 1000);
      }
    }

    private void stateChanged(PlayState state)
    {
      foreach (Screen screen in lcdScreenList_)
      {
        screen.playerStatusChanged(state);
      }
    }
    #endregion


    #region MusicBee Funtions

    // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
    // its up to you to figure out whether anything has changed and needs updating
    public void SaveSettings()
    {
      // save any persistent settings in a sub-folder of this path
      string dataPath = mbApiInterface_.Setting_GetPersistentStoragePath();
    }

    // return an array of lyric or artwork provider names this plugin supports
    // the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
    public string[] GetProviders()
    {
      return null;
    }

    // return lyrics for the requested artist/title from the requested provider
    // only required if PluginType = LyricsRetrieval
    // return null if no lyrics are found
    public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
    {
      return null;
    }

    // return Base64 string representation of the artwork binary data from the requested provider
    // only required if PluginType = ArtworkRetrieval
    // return null if no artwork is found
    public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
    {
      //Return Convert.ToBase64String(artworkBinaryData)
      return null;
    }

    // user initiated refresh (eg. pressing F5) - reconnect/ clear cache as appropriate
    public void Refresh()
    {
    }

    // is the server ready
    // you can initially return false and then use MB_SendNotification when the storage is ready (or failed)
    public bool IsReady()
    {
      return false;
    }

    // return a 16x16 bitmap for the storage icon
    public Bitmap GetIcon()
    {
      return new Bitmap(16, 16);
    }

    public bool FolderExists(string path)
    {
      return true;
    }

    // return the full path of folders in a folder
    public string[] GetFolders(string path)
    {
      return new string[] { };
    }

    // this function returns an array of files in the specified folder
    // each file is represented as a array of tags - each tag being a KeyValuePair(Of Byte, String), where Byte is a FilePropertyType or MetaDataType enum value and String is the value
    // a tag for FilePropertyType.Url must be included
    // you can initially return null and then use MB_SendNotification when the file data is ready (on receiving the notification MB will call GetFiles(path) again)
    public KeyValuePair<byte, string>[][] GetFiles(string path)
    {
      return null;
    }

    public bool FileExists(string url)
    {
      return true;
    }

    //  each file is represented as a array of tags - each tag being a KeyValuePair(Of Byte, String), where Byte is a FilePropertyType or MetaDataType enum value and String is the value
    // a tag for FilePropertyType.Url must be included
    public KeyValuePair<byte, string>[] GetFile(string url)
    {
      return null;
    }

    // return an array of bytes for the raw picture data
    public byte[] GetFileArtwork(string url)
    {
      return null;
    }

    // return an array of playlist identifiers
    // where each playlist identifier is a KeyValuePair(id, name)
    public KeyValuePair<string, string>[] GetPlaylists()
    {
      return null;
    }

    // return an array of files in a playlist - a playlist being identified by the id parameter returned by GetPlaylists()
    // each file is represented as a array of tags - each tag being a KeyValuePair(Of Byte, String), where Byte is a FilePropertyType or MetaDataType enum value and String is the value
    // a tag for FilePropertyType.Url must be included
    public KeyValuePair<byte, string>[][] GetPlaylistFiles(string id)
    {
      return null;
    }

    // return a stream that can read through the raw (undecoded) bytes of a music file
    public System.IO.Stream GetStream(string url)
    {
      return null;
    }

    // return the last error that occurred
    public Exception GetError()
    {
      return null;
    }

    #endregion
  }
}