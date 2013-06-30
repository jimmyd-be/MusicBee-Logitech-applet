using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GammaJul;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Collections;

namespace MusicBeePlugin
{
    public class Logitech
    {
        #region Properties
        public bool connected = false;
        private LcdApplet applet = null;
        private LcdDevice device = null;
        private bool firstTime = true;

        private LcdGdiPage[] page = new LcdGdiPage[2];

        private Plugin musicBeePlugin = null;

        private Font font = new Font("Microsoft Sans Serif", 8);
        private Font font2 = new Font("Microsoft Sans Serif", 7);
        private Font font3 = new Font("Arial", 15);
        private Font font4 = new Font("Arial", 10);

        //private int pageNumber = 0;
        private int timerTime = 0;
        private int position = 0;
        private int duration = 0;

        private string artist = "";
        private string album = "";
        private string title = "";
        private string artwork = "";
        private float rating = 0;
        private MusicBeePlugin.Plugin.PlayState state = Plugin.PlayState.Undefined;

        private Image backgroundImage = null;
        private LcdGdiImage backgroundGdi = null;
        private LcdGdiImage artworkGdi = null;

        private LcdGdiText titleGdi = null;
        private LcdGdiText artistGdi = null;
        private LcdGdiText positionGdi = null;
        private LcdGdiText durationGdi = null;
        private LcdGdiText albumGdi = null;
        private LcdGdiProgressBar progressBarGdi = null;
        private LcdGdiText ratingGdi = null;

        //private LcdGdiText playlistLine1 = null;
        //private LcdGdiText playlistLine2 = null;
        //private LcdGdiText playlistLine3 = null;
        //private LcdGdiText playlistLine4 = null;
        //private LcdGdiText playlistLine5 = null;
        //private ArrayList playlist = new ArrayList();
        //private int playlistScroll = 0;


        //Shuffle 0
        //AutoDJ 1
        //Equaliser 2
        //repeat 3
        private LcdGdiText[] settingsGdi = new LcdGdiText[4];

        private LcdGdiScrollViewer albumScroll = null;
        private LcdGdiScrollViewer titleScroll = null;
        private LcdGdiScrollViewer artistScroll = null;

        private AutoResetEvent autoEvent = null;
        private Timer timer = null;

        private MusicBeePlugin.Plugin.RepeatMode[] repeatArray = new Plugin.RepeatMode[3];
        //private int settingSelected = 0;
        //private int repeatSelected = 0;
        private bool autoDJ = false;
        private bool equaliser = false;
        private bool shuffle = false;
        private MusicBeePlugin.Plugin.RepeatMode repeat;

        static Logitech logitechObject = null;

        private int alwaysOnTopCounter = 0;
        private bool eventHappened = true;
        #region settings
        private bool alwaysOnTop = true;
        #endregion
        #endregion

        #region Consturctor

        public Logitech(Plugin plugin)
        {
            this.musicBeePlugin = plugin;
            Logitech.logitechObject = this;

            repeatArray[0] = Plugin.RepeatMode.None;
            repeatArray[1] = Plugin.RepeatMode.One;
            repeatArray[2] = Plugin.RepeatMode.All;

            //connect();

            // Create an event to signal the timeout count threshold in the 
            // timer callback.
            autoEvent = new AutoResetEvent(false);

            // Create an inferred delegate that invokes methods for the timer.
            TimerCallback tcb = TimerCallback;

            // Create a timer that signals the delegate to invoke  
            // CheckStatus after one second, and every 1/4 second  
            // thereafter.
            timer = new Timer(tcb, autoEvent, 1000, 500);
        }

        public void disconnect()
        {
            timer.Dispose();
            //applet.Disconnect();
            //applet = null;
        }

        #endregion
        #region Getters/Setters

        public bool getFirstTime()
        {
            return firstTime;
        }

        public void setPosition(int position)
        {
            this.position = position / 1000;
        }

        public void setDuration(int duration)
        {
            this.duration = duration / 1000;
        }

        //public int getPlaylistScroll()
        //{
        //    return playlistScroll;
        //}


        #endregion

        public void connect()
        {
            LcdAppletCapabilities appletCapabilities = LcdAppletCapabilities.Both;
            applet = new LcdApplet("MusicBee V2", appletCapabilities, false);
            applet.DeviceArrival += new EventHandler<LcdDeviceTypeEventArgs>(applet_DeviceArrival);
            applet.Connect();

        }

        public void changeArtistTitle(string artist, string album, string title, string rating, string artwork, int duration, int position, bool autoDJ, bool equaliser, bool shuffle, MusicBeePlugin.Plugin.RepeatMode repeat)
        {
            eventHappened = true;
            this.artist = artist;
            this.album = album;
            this.title = title;
            this.artwork = artwork;
            this.duration = duration;
            this.position = position;
            this.autoDJ = autoDJ;
            this.equaliser = equaliser;
            this.shuffle = shuffle;
            this.repeat = repeat;
            // this.playlist = playList;

            if (rating != "")
            {
                this.rating = Convert.ToSingle(rating);
            }
            else
            {
                this.rating = 0;
            }


            timer.Dispose();
            TimerCallback tcb = TimerCallback;


            if (device != null && progressBarGdi == null)
            {
                if (device.DeviceType == LcdDeviceType.Monochrome)
                {
                    createMonochrome();
                }

                else if (device.DeviceType == LcdDeviceType.Qvga)
                {
                    createColor();
                }
            }

            if (device != null && progressBarGdi != null)
            {
                if (LcdDeviceType.Qvga == device.DeviceType)
                {
                    artworkGdi.Image = Base64ToImage(artwork);
                    albumGdi.Text = album;
                }
                else if (device.DeviceType == LcdDeviceType.Monochrome)
                {
                    if (this.rating != 0)
                    {
                        ratingGdi.Text = rating.ToString() + " / 5";
                    }
                    else
                    {
                        ratingGdi.Text = "";
                    }
                }

                titleGdi.Text = title;
                artistGdi.Text = artist;
                positionGdi.Text = timetoString(position);
                durationGdi.Text = timetoString(duration);
                int progresstime = (int)(((float)position / (float)duration) * 100);
                progressBarGdi.Value = progresstime;

                //playlistLine1.Text = musicBeePlugin.getTrackName(playList[0].ToString());
                //playlistLine2.Text = musicBeePlugin.getTrackName(playList[1].ToString());
                //playlistLine3.Text = musicBeePlugin.getTrackName(playList[2].ToString());
                //playlistLine4.Text = musicBeePlugin.getTrackName(playList[3].ToString());
                //playlistLine5.Text = musicBeePlugin.getTrackName(playList[4].ToString());

                //if (this.shuffle)
                //{
                //    settingsGdi[0].Text = "Shuffle: Enabled";
                //}
                //else
                //{
                //    settingsGdi[0].Text = "Shuffle: Disabled";
                //}


                //if (this.autoDJ)
                //{
                //    settingsGdi[1].Text = "Auto DJ = Enabled";
                //}
                //else
                //{
                //    settingsGdi[1].Text = "Auto DJ = Disabled";
                //}

                //if (this.equaliser)
                //{
                //    settingsGdi[2].Text = "Equaliser: Enabled";
                //}
                //else
                //{
                //    settingsGdi[2].Text = "Equaliser: Disabled";
                //}

                //settingsGdi[3].Text = "Repeat: " + this.repeat;

                device.DoUpdateAndDraw();
                timer = new Timer(tcb, autoEvent, 1000, 500);
            }
        }



        public static void TimerCallback(Object state)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)state;

            if (logitechObject.connected)
            {
                if (!logitechObject.alwaysOnTop && logitechObject.eventHappened)
                {
                    logitechObject.device.SetAsForegroundApplet = true;
                    logitechObject.alwaysOnTopCounter += 500;

                    if (logitechObject.alwaysOnTopCounter >= 5000)
                    {
                        logitechObject.eventHappened = false;
                        logitechObject.alwaysOnTopCounter = 0;
                        logitechObject.device.SetAsForegroundApplet = false;
                    }
                }

                if (logitechObject.state == MusicBeePlugin.Plugin.PlayState.Playing)
                {
                    logitechObject.timerTime += 500;
                }

                //Update progressbar and position time on the screen after 1 second of music.

                if (logitechObject.state == MusicBeePlugin.Plugin.PlayState.Playing && logitechObject.timerTime == 1000)
                {
                    logitechObject.timerTime = 0;
                    logitechObject.position++;
                    int progresstime = (int)(((float)logitechObject.position / (float)logitechObject.duration) * 100);
                    logitechObject.progressBarGdi.Value = progresstime;
                    logitechObject.positionGdi.Text = logitechObject.timetoString(logitechObject.position);
                }

                //If music stopped then the progressbar and time must stop immediately
                else if (logitechObject.state == MusicBeePlugin.Plugin.PlayState.Stopped)
                {
                    logitechObject.timerTime = 0;
                    logitechObject.position = 0;
                    logitechObject.progressBarGdi.Value = 0;
                    logitechObject.positionGdi.Text = "00:00";
                }

                //if (logitechObject.playlist != null)
                //{
                //    if (logitechObject.playlist.Count >= 1 && logitechObject.playlist[0] != null && logitechObject.playlistLine1 != null)
                //    {
                //        logitechObject.playlistLine1.Text = logitechObject.musicBeePlugin.getTrackName(logitechObject.playlist[0].ToString());
                //    }

                //    if (logitechObject.playlist.Count >= 2 && logitechObject.playlist[1] != null && logitechObject.playlistLine2 != null)
                //    {
                //        logitechObject.playlistLine2.Text = logitechObject.musicBeePlugin.getTrackName(logitechObject.playlist[1].ToString());
                //    }

                //    if (logitechObject.playlist.Count >= 3 && logitechObject.playlist[2] != null && logitechObject.playlistLine3 != null)
                //    {
                //        logitechObject.playlistLine3.Text = logitechObject.musicBeePlugin.getTrackName(logitechObject.playlist[2].ToString());
                //    }

                //    if (logitechObject.playlist.Count >= 4 && logitechObject.playlist[3] != null && logitechObject.playlistLine4 != null)
                //    {
                //        logitechObject.playlistLine4.Text = logitechObject.musicBeePlugin.getTrackName(logitechObject.playlist[3].ToString());
                //    }

                //    if (logitechObject.playlist.Count >= 5 && logitechObject.playlist[4] != null && logitechObject.playlistLine5 != null)
                //    {
                //        logitechObject.playlistLine5.Text = logitechObject.musicBeePlugin.getTrackName(logitechObject.playlist[4].ToString());
                //    }

                //}
                logitechObject.device.DoUpdateAndDraw();
            }
        }


        private void applet_DeviceArrival(object sender, LcdDeviceTypeEventArgs e)
        {
            device = applet.OpenDeviceByType(e.DeviceType);
            device.SoftButtonsChanged += new EventHandler<LcdSoftButtonsEventArgs>(buttonPressed);

            device.SetAsForegroundApplet = alwaysOnTop;

            connected = true;

            if (device.DeviceType == LcdDeviceType.Monochrome)
            {
                page[0] = new LcdGdiPage(device);
                Font font = new Font("Microsoft Sans Serif", 25);

                page[0].Children.Add(new LcdGdiText("MusicBee", font)
                {
                    Margin = new MarginF(2, 0, 0, 0),
                    VerticalAlignment = LcdGdiVerticalAlignment.Middle
                });
                device.CurrentPage = page[0];

                device.DoUpdateAndDraw();
                firstTime = false;

            }

            else if (device.DeviceType == LcdDeviceType.Qvga)
            {
                page[0] = new LcdGdiPage(device);

                backgroundImage = (Image)Resource.G19logo;
                backgroundGdi = new LcdGdiImage(backgroundImage);
                page[0].Children.Add(backgroundGdi);
                device.CurrentPage = page[0];

                device.DoUpdateAndDraw();
                firstTime = false;
            }

        }

        private void buttonPressed(object sender, LcdSoftButtonsEventArgs e)
        {
            //    LcdDevice device = (LcdDevice)sender;

            //    // First button is pressed, switch to page one
            //    if ((e.SoftButtons & LcdSoftButtons.Button0) == LcdSoftButtons.Button0)
            //    {
            //        if (pageNumber == 0)
            //        {
            //            pageNumber = page.Length - 1;
            //            device.CurrentPage = page[pageNumber];
            //        }
            //        else
            //        {
            //            pageNumber -= 1;
            //            device.CurrentPage = page[pageNumber];
            //        }
            //    }

            //    // Second button is pressed
            //    else if ((e.SoftButtons & LcdSoftButtons.Button1) == LcdSoftButtons.Button1)
            //    {

            //        if (device.CurrentPage == page[0])
            //        {
            //            if (this.rating != 0)
            //            {
            //                this.rating -= 0.5f;
            //                musicBeePlugin.changeRating(this.rating);
            //            }
            //        }

            //        //else if (device.CurrentPage == page[1])
            //        //{
            //        //    playlistScroll -= 1;
            //        //    this.playlist = musicBeePlugin.getPlayList(playlistScroll);
            //        //}

            //        else if (device.CurrentPage == page[1])
            //        {
            //            if (settingSelected != 0)
            //            {
            //                settingSelected -= 1;
            //            }

            //            for (int i = 0; i < settingsGdi.Length; i++)
            //            {
            //                settingsGdi[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
            //            }

            //            settingsGdi[settingSelected].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            //        }
            //    }

            //    // Third button is pressed
            //    else if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button2)
            //    {
            //        if (device.CurrentPage == page[0])
            //        {
            //            if (this.rating != 5)
            //            {
            //                this.rating += 0.5f;
            //                musicBeePlugin.changeRating(this.rating);
            //            }
            //        }
            //        //else if (device.CurrentPage == page[1])
            //        //{
            //        //    playlistScroll += 1;
            //        //    this.playlist = musicBeePlugin.getPlayList(playlistScroll);
            //        //}

            //        else if (device.CurrentPage == page[1])
            //        {
            //            if (settingSelected != 3)
            //            {
            //                settingSelected += 1;
            //            }

            //            for (int i = 0; i < settingsGdi.Length; i++)
            //            {
            //                settingsGdi[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
            //            }

            //            settingsGdi[settingSelected].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            //        }
            //    }

            //    // Fourth button is pressed
            //    else if ((e.SoftButtons & LcdSoftButtons.Button3) == LcdSoftButtons.Button3)
            //    {
            //        if (pageNumber == page.Length - 1)
            //        {
            //            switch (settingSelected)
            //            {

            //                case 0:
            //                    shuffle = !shuffle;
            //                    break;

            //                case 1:
            //                    autoDJ = !autoDJ;
            //                    break;

            //                case 2:
            //                    equaliser = !equaliser;
            //                    break;

            //                case 3:

            //                    if(repeatSelected == 2)
            //                    {
            //                        repeatSelected = 0;
            //                    }
            //                    else
            //                    {
            //                        repeatSelected++;
            //                    }
            //                    repeat = repeatArray[repeatSelected];
            //                    break;
            //            };

            //        }
            //        else
            //        {
            //            pageNumber += 1;
            //            device.CurrentPage = page[pageNumber];
            //        }
            //    }

            //    musicBeePlugin.changeSettings(autoDJ, equaliser, shuffle, repeat);
            //    musicBeePlugin.updateTrackText();
            //    device.DoUpdateAndDraw();

        }

        private void createMonochrome()
        {
            page[0].Children.Clear();
            page[0].Dispose();
            page[0] = null;

            page[0] = new LcdGdiPage(device);
            page[1] = new LcdGdiPage(device);
            //page[2] = new LcdGdiPage(device);

            /**
             * Create first window (player)
             **/
            titleGdi = new LcdGdiText(this.title, font);
            titleGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            titleGdi.Margin = new MarginF(-2, -1, 0, 0);

            artistGdi = new LcdGdiText(this.artist, font);
            artistGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            artistGdi.VerticalAlignment = LcdGdiVerticalAlignment.Top;
            artistGdi.Margin = new MarginF(-2, 12, 0, 0);

            positionGdi = new LcdGdiText(timetoString(this.position), font2);
            positionGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
            positionGdi.Margin = new MarginF(10, 26, 0, 0);

            ratingGdi = new LcdGdiText("", font2);
            ratingGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            ratingGdi.Margin = new MarginF(0, 26, 0, 0);

            if (this.rating != 0)
            {
                ratingGdi.Text = this.rating.ToString() + " / 5";
            }
            else
            {
                ratingGdi.Text = "";
            }


            durationGdi = new LcdGdiText(timetoString(this.duration), font2);
            durationGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Right;
            durationGdi.Margin = new MarginF(0, 26, 13, 0);

            progressBarGdi = new LcdGdiProgressBar();
            progressBarGdi.Size = new SizeF(136, 5);
            progressBarGdi.Margin = new MarginF(12, 38, 0, 0);
            progressBarGdi.Minimum = 0;
            progressBarGdi.Maximum = 100;

            titleScroll = new LcdGdiScrollViewer(titleGdi);
            titleScroll.AutoScrollX = true;
            titleScroll.AutoScrollY = false;
            titleScroll.AutoScrollSpeedY = 0;
            titleScroll.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
            titleScroll.VerticalAlignment = LcdGdiVerticalAlignment.Top;

            artistScroll = new LcdGdiScrollViewer(artistGdi);
            artistScroll.AutoScrollY = false;
            artistScroll.AutoScrollX = true;
            artistScroll.AutoScrollSpeedY = 0;
            artistScroll.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
            artistScroll.VerticalAlignment = LcdGdiVerticalAlignment.Top;

            page[0].Children.Add(titleGdi);
            page[0].Children.Add(artistGdi);
            page[0].Children.Add(titleScroll);
            page[0].Children.Add(artistScroll);
            page[0].Children.Add(positionGdi);
            page[0].Children.Add(ratingGdi);
            page[0].Children.Add(durationGdi);
            page[0].Children.Add(progressBarGdi);


            /**
             * Create second screen (playlist)
             * */

            //playlistLine1 = new LcdGdiText("", font2);
            //playlistLine1.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            //playlistLine1.Margin = new MarginF(-2, -2, 0, 0);

            //playlistLine2 = new LcdGdiText("", font2);
            //playlistLine2.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            //playlistLine2.Margin = new MarginF(-2, 6, 0, 0);

            //playlistLine3 = new LcdGdiText("", font2);
            //playlistLine3.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            //playlistLine3.Margin = new MarginF(-2, 14, 0, 0);

            //playlistLine4 = new LcdGdiText("", font2);
            //playlistLine4.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            //playlistLine4.Margin = new MarginF(-2, 22, 0, 0);

            //playlistLine5 = new LcdGdiText("", font2);
            //playlistLine5.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            //playlistLine5.Margin = new MarginF(-2, 30, 0, 0);

            //page[1].Children.Add(playlistLine1);
            //page[1].Children.Add(playlistLine2);
            //page[1].Children.Add(playlistLine3);
            //page[1].Children.Add(playlistLine4);
            //page[1].Children.Add(playlistLine5);


            /**
             * Create thirth screen (settings)
             * */
            settingsGdi[0] = new LcdGdiText("Shuffle: ", font2);
            settingsGdi[0].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            settingsGdi[0].Margin = new MarginF(-2, 0, 0, 0);

            settingsGdi[1] = new LcdGdiText("Auto DJ: ", font2);
            settingsGdi[1].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
            settingsGdi[1].VerticalAlignment = LcdGdiVerticalAlignment.Top;
            settingsGdi[1].Margin = new MarginF(-2, 10, 0, 0);

            settingsGdi[2] = new LcdGdiText("Equalizer: ", font2);
            settingsGdi[2].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
            settingsGdi[2].VerticalAlignment = LcdGdiVerticalAlignment.Top;
            settingsGdi[2].Margin = new MarginF(-2, 20, 0, 0);

            settingsGdi[3] = new LcdGdiText("Repeat: ", font2);
            settingsGdi[3].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
            settingsGdi[3].VerticalAlignment = LcdGdiVerticalAlignment.Top;
            settingsGdi[3].Margin = new MarginF(-2, 30, 0, 0);

            page[1].Children.Add(settingsGdi[0]);
            page[1].Children.Add(settingsGdi[1]);
            page[1].Children.Add(settingsGdi[2]);
            page[1].Children.Add(settingsGdi[3]);

            device.CurrentPage = page[0];

            device.DoUpdateAndDraw();
        }

        private void createColor()
        {
            page[0].Children.Clear();
            page[0].Dispose();
            page[0] = null;

            page[0] = new LcdGdiPage(device);
            backgroundImage = (Image)Resource.G19Background;
            backgroundGdi = new LcdGdiImage(backgroundImage);
            page[0].Children.Add(backgroundGdi);

            artistGdi = new LcdGdiText(this.artist, font3);
            artistGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            artistGdi.Margin = new MarginF(5, 5, 5, 0);

            titleGdi = new LcdGdiText(this.title, font3);
            titleGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            titleGdi.Margin = new MarginF(5, 30, 5, 0);

            albumGdi = new LcdGdiText(this.album, font3);
            albumGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            albumGdi.Margin = new MarginF(5, 55, 5, 0);

            positionGdi = new LcdGdiText(timetoString(this.position), font4);
            positionGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
            positionGdi.Margin = new MarginF(5, 105, 0, 0);

            durationGdi = new LcdGdiText(timetoString(this.duration), font4);
            durationGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Right;
            durationGdi.Margin = new MarginF(0, 105, 5, 0);

            artworkGdi = new LcdGdiImage(Base64ToImage(artwork));
            artworkGdi.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            artworkGdi.Size = new SizeF(130, 130);
            artworkGdi.Margin = new MarginF(0, 105, 0, 0);

            progressBarGdi = new LcdGdiProgressBar();
            progressBarGdi.Minimum = 0;
            progressBarGdi.Maximum = 100;
            progressBarGdi.Size = new SizeF(310, 20);
            progressBarGdi.Margin = new MarginF(5, 80, 5, 0);

            titleScroll = new LcdGdiScrollViewer(titleGdi);
            titleScroll.AutoScrollX = true;
            titleScroll.AutoScrollY = false;
            titleScroll.AutoScrollSpeedY = 0;
            titleScroll.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
            titleScroll.VerticalAlignment = LcdGdiVerticalAlignment.Top;

            artistScroll = new LcdGdiScrollViewer(artistGdi);
            artistScroll.AutoScrollY = false;
            artistScroll.AutoScrollX = true;
            artistScroll.AutoScrollSpeedY = 0;
            artistScroll.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
            artistScroll.VerticalAlignment = LcdGdiVerticalAlignment.Top;

            albumScroll = new LcdGdiScrollViewer(albumGdi);
            albumScroll.AutoScrollY = false;
            albumScroll.AutoScrollX = true;
            albumScroll.AutoScrollSpeedY = 0;
            albumScroll.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
            albumScroll.VerticalAlignment = LcdGdiVerticalAlignment.Top;

            page[0].Children.Add(titleGdi);
            page[0].Children.Add(titleScroll);
            page[0].Children.Add(artistGdi);
            page[0].Children.Add(positionGdi);
            page[0].Children.Add(durationGdi);
            page[0].Children.Add(progressBarGdi);
            page[0].Children.Add(albumGdi);
            page[0].Children.Add(artworkGdi);

            device.CurrentPage = page[0];

            device.DoUpdateAndDraw();

        }

        public void changeState(MusicBeePlugin.Plugin.PlayState state)
        {
            eventHappened = true;
            this.state = state;

            if (device != null && progressBarGdi == null && state != Plugin.PlayState.Playing)
            {
                if (device.DeviceType == LcdDeviceType.Monochrome)
                {
                    createMonochrome();
                }
                else
                {
                    createColor();
                }
            }
        }

        private string timetoString(int time)
        {
            string minutes = ((int)time / 60).ToString();
            string seconds = ((int)time % 60).ToString();

            if (minutes.Length < 2)
            {
                minutes = "0" + minutes;
            }

            if (seconds.Length < 2)
            {
                seconds = "0" + seconds;
            }

            return minutes + ":" + seconds;
        }

        public Image Base64ToImage(string base64String)
        {
            Image image = null;
            if (base64String != "" && base64String != null)
            {
                // Convert Base64 String to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0,
                  imageBytes.Length);

                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                image = Image.FromStream(ms, true);

                image = resizeImage(image, new Size(320, 130));
            }
            else
            {
                image = Resource.NoArtwork;
            }
            return image;
        }

        private Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        public void settingsChanged(bool alwaysOnTop_)
        {
            this.alwaysOnTop = alwaysOnTop_;
            device.SetAsForegroundApplet = alwaysOnTop;
        }

    }
}
