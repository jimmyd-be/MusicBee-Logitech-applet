using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();
        private int connection = -1;
        private int deviceNumber = -1;
        Graphics graphics;
        Bitmap LCD;

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = (MusicBeeApiInterface)Marshal.PtrToStructure(apiInterfacePtr, typeof(MusicBeeApiInterface));
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "MusicBee Logitech LCD";
            about.Description = "A support to Logitech G15, G19 LCD to Musicbee";
            about.Author = "JimmyD";
            about.TargetApplication = "MusicBee";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type = PluginType.General;
            about.VersionMajor = 1;  // your plugin version
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
            // save any persistent settings in a sub-folder of this path
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
            // panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
            // keep in mind the panel width is scaled according to the font the user has selected
            // if about.ConfigurationPanelHeight is set to 0, you can display your own popup window
            if (panelHandle != IntPtr.Zero)
            {
                Panel configPanel = (Panel)Panel.FromHandle(panelHandle);
                Label prompt = new Label();
                prompt.AutoSize = true;
                prompt.Location = new Point(0, 0);
                prompt.Text = "prompt:";
                TextBox textBox = new TextBox();
                textBox.Bounds = new Rectangle(60, 0, 100, textBox.Height);
                configPanel.Controls.AddRange(new Control[] { prompt, textBox });
            }
            return false;
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
            if (connection != -1)
            {
                DMcLgLCD.LcdClose(deviceNumber);
                DMcLgLCD.LcdDisconnect(connection);
            }
            DMcLgLCD.LcdDeInit();
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
                    // perform startup initialisation

                    DMcLgLCD.LcdInit();
                    connection = DMcLgLCD.LcdConnect("MusicBee", 0, 0);
                    deviceNumber = DMcLgLCD.LcdOpenByType(connection, DMcLgLCD.LGLCD_DEVICE_QVGA);

                    if (deviceNumber == DMcLgLCD.LGLCD_INVALID_DEVICE)
                    {
                        deviceNumber = DMcLgLCD.LcdOpenByType(connection, DMcLgLCD.LGLCD_DEVICE_BW);
                    }

                     LCD = new Bitmap(160, 43);
                    graphics = Graphics.FromImage(LCD);
                    graphics.Clear(Color.White);
                    graphics.DrawString("MusicBee", new Font("Tahoma", 25), Brushes.Black, new PointF(0, 0));
                    DMcLgLCD.LcdUpdateBitmap(deviceNumber, LCD.GetHbitmap(), DMcLgLCD.LGLCD_DEVICE_BW);
                    graphics.Dispose();
                    LCD.Dispose();

                    break;

                case NotificationType.TrackChanged:
                    break;

                case NotificationType.PlayStateChanged:

                    switch (mbApiInterface.Player_GetPlayState())
                    {
                        case PlayState.Stopped:
                        case PlayState.Playing:
                            string artist = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist);
                            string track = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle);

                            LCD = new Bitmap(160, 43);
                            graphics = Graphics.FromImage(LCD);
                            graphics.Clear(Color.White);
                            graphics.DrawString(artist + "\n" + track, new Font("Tahoma", 10), Brushes.Black, new PointF(0, 0));
                            DMcLgLCD.LcdUpdateBitmap(deviceNumber, LCD.GetHbitmap(), DMcLgLCD.LGLCD_DEVICE_BW);
                            graphics.Dispose();
                            LCD.Dispose();
                            break;

                        case PlayState.Paused:
                            // ...
                            break;
                    }

                    break;
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
            return new string[]{};
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
        public byte[] GetFileArtwork(string url )
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
        public  Exception GetError()
        {
            return null;
        }

        #endregion
    }
}