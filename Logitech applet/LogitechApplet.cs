using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;

namespace MusicBeePlugin
{

    public partial class Plugin
    {
        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();
        private Logitech logitech = null;
        private Settings settings = null;

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "MusicBee V2";
            about.Description = "Add support for Logitech G13, G15, G510, G19 LCD to MusicBee";
            about.Author = "JimmyD";
            about.TargetApplication = "MusicBee";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type = PluginType.General;
            about.VersionMajor = 2;  // your plugin version
            about.VersionMinor = 0;
            about.Revision = 0;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;   // not implemented yet: height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function
            return about;
        }

        public bool Configure(IntPtr panelHandle)
        {
            settings.Show();
            return true;
        }

        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings()
        {
            // save any persistent settings in a sub-folder of this path
              string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason)
        {
            if (logitech != null)
            {
                logitech.disconnect();
                logitech = null;
            }

            if (settings != null)
            {
                settings.Close();
                settings = null;
            }
        }

        // uninstall this plugin - clean up any persisted files
        public void Uninstall()
        {
        }

        // receive event notifications from MusicBee
        // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            // perform some action depending on the notification type
            switch (type)
            {
                case NotificationType.PluginStartup:

                    if (logitech == null)
                    {
                        logitech = new Logitech(this);

                        settings = new Settings(mbApiInterface.Setting_GetPersistentStoragePath(), logitech);

                        settings.openSettings();

                        logitech.connect();

                    }
                    break;

                case NotificationType.PlayStateChanged:
                    
                    if (logitech != null && logitech.connected)
                    {
                        switch (mbApiInterface.Player_GetPlayState())
                        {
                            case PlayState.Playing:

                                logitech.changeState(PlayState.Playing);

                                if (!logitech.getFirstTime())
                                {
                                    updateTrackText();
                                    logitech.setPosition(mbApiInterface.Player_GetPosition());
                                    logitech.setDuration(mbApiInterface.NowPlaying_GetDuration());
                                }
                                break;
                            case PlayState.Paused:
                                logitech.changeState(PlayState.Paused);
                                if (!logitech.getFirstTime())
                                {
                                    logitech.setPosition(mbApiInterface.Player_GetPosition());
                                    logitech.setDuration(mbApiInterface.NowPlaying_GetDuration());
                                }
                                break;
                            case PlayState.Stopped:
                                logitech.changeState(PlayState.Stopped);
                                if (!logitech.getFirstTime())
                                {
                                    logitech.setPosition(mbApiInterface.Player_GetPosition());
                                    logitech.setDuration(mbApiInterface.NowPlaying_GetDuration());
                                }
                                break;
                            case PlayState.Loading:
                                logitech.changeState(PlayState.Loading);
                                if (!logitech.getFirstTime())
                                {
                                    logitech.setPosition(mbApiInterface.Player_GetPosition());
                                    logitech.setDuration(mbApiInterface.NowPlaying_GetDuration());
                                }
                                break;
                            case PlayState.Undefined:
                                logitech.changeState(PlayState.Undefined);
                                break;

                        }
                    }
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
                    updateTrackText();
                    break;
            }
        }

        //public ArrayList getPlayList(int scroll)
        //{
        //    ArrayList playlist = new ArrayList();

        //    playlist.Add(mbApiInterface.NowPlayingList_GetListFileUrl(mbApiInterface.NowPlayingList_GetCurrentIndex() + scroll));
        //    playlist.Add(mbApiInterface.NowPlayingList_GetListFileUrl(mbApiInterface.NowPlayingList_GetCurrentIndex() + 1 + scroll));
        //    playlist.Add(mbApiInterface.NowPlayingList_GetListFileUrl(mbApiInterface.NowPlayingList_GetCurrentIndex() +2+ scroll));
        //    playlist.Add(mbApiInterface.NowPlayingList_GetListFileUrl(mbApiInterface.NowPlayingList_GetCurrentIndex() + 3 + scroll));
        //    playlist.Add(mbApiInterface.NowPlayingList_GetListFileUrl(mbApiInterface.NowPlayingList_GetCurrentIndex() + 4 + scroll));
            
        //    return playlist;
        //}

        public void changeRating(float number)
        {
            String url = mbApiInterface.NowPlaying_GetFileUrl();
            mbApiInterface.Library_SetFileTag(url, MetaDataType.Rating, number.ToString());
        }

        public void updateTrackText()
        {
            string artist = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist);
            string album = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Album);
            string title = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle);
            string artwork = mbApiInterface.NowPlaying_GetArtwork();
            string rating = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Rating);
          //  ArrayList playlist = getPlayList(0);

            bool autoDJ = mbApiInterface.Player_GetAutoDjEnabled();
            bool equaliser = mbApiInterface.Player_GetEqualiserEnabled();
            RepeatMode repeat = mbApiInterface.Player_GetRepeat();
            bool shuffle = mbApiInterface.Player_GetShuffle();

            logitech.changeArtistTitle(artist, album, title, rating, artwork, mbApiInterface.NowPlaying_GetDuration() / 1000, mbApiInterface.Player_GetPosition() / 1000, autoDJ, equaliser, shuffle, repeat);
        }


        public void changeSettings(bool autoDJ, bool equaliser, bool shuffle, RepeatMode repeat)
        {
            bool autoDJMusicbee = mbApiInterface.Player_GetAutoDjEnabled();
            bool equaliserMusicbee = mbApiInterface.Player_GetEqualiserEnabled();
            RepeatMode repeatMusicbee = mbApiInterface.Player_GetRepeat();
            bool shuffleMusicbee = mbApiInterface.Player_GetShuffle();

            if (autoDJ && autoDJMusicbee != autoDJ)
            {
                mbApiInterface.Player_StartAutoDj();
            }
            else if (autoDJMusicbee != autoDJ)
            {
                mbApiInterface.Player_SetShuffle(true);
                mbApiInterface.Player_SetShuffle(false);
            }

            if (equaliserMusicbee != equaliser)
            {
                mbApiInterface.Player_SetEqualiserEnabled(equaliser);
            }

            if(repeat != repeatMusicbee)
            {

                mbApiInterface.Player_SetRepeat(repeat);
            }

            if (shuffle != shuffleMusicbee)
            {
                mbApiInterface.Player_SetShuffle(shuffle);
            }

        }

        public string getTrackName(String url)
        {
            if(url.Length > 0)
            {
                return mbApiInterface.Library_GetFileTag(url, MetaDataType.TrackTitle);
                }
                else
                {
                    return "";
                }
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

        #region " Storage Plugin "
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