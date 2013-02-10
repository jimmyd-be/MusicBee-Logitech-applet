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

namespace MusicBeePlugin
{
    class Logitech
    {

        #region Properties
        public bool connected = false;
        private LcdApplet applet = null;
        private LcdDevice device = null;
        private bool firstTime = true;
        private LcdGdiPage page = null;
        private Font font = new Font("Microsoft Sans Serif", 8);
        private Font font2 = new Font("Microsoft Sans Serif", 7);
        private Font font3 = new Font("Arial", 15);
        private Font font4 = new Font("Arial", 10);

        private int timerTime = 0;
        private int position = 0;
        private int duration = 0;
        private string artist = "";
        private string album = "";
        private string title = "";
        private string artwork = "";
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

        private LcdGdiScrollViewer albumScroll = null;
        private LcdGdiScrollViewer titleScroll = null;
        private LcdGdiScrollViewer artistScroll = null;

        private AutoResetEvent autoEvent = null;
        private Timer timer = null;

        static Logitech logitechObject = null;

        #endregion

        #region Consturctor

        public Logitech()
        {
            Logitech.logitechObject = this;


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

        #endregion

        public void connect()
        {
            LcdAppletCapabilities appletCapabilities = LcdAppletCapabilities.Both;
            applet = new LcdApplet("MusicBee V2", appletCapabilities, false);
            applet.DeviceArrival += new EventHandler<LcdDeviceTypeEventArgs>(applet_DeviceArrival);
            applet.Connect();

        }

        public void changeArtistTitle(string artist, string album, string title, string artwork, int duration, int position)
        {
            this.artist = artist;
            this.album = album;
            this.title = title;
            this.artwork = artwork;
            this.duration = duration;
            this.position = position;

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
                    albumGdi.Text = album;
                    artworkGdi.Image = Base64ToImage(artwork);
                }


                titleGdi.Text = title;
                artistGdi.Text = artist;
                positionGdi.Text = timetoString(position);
                durationGdi.Text = timetoString(duration);
                int progresstime = (int)(((float)position / (float)duration) * 100);
                progressBarGdi.Value = progresstime;
                device.DoUpdateAndDraw();
            }
        }



        public static void TimerCallback(Object state)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)state;

            if (logitechObject.connected)
            {
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

                logitechObject.device.DoUpdateAndDraw();
            }
        }


        private void applet_DeviceArrival(object sender, LcdDeviceTypeEventArgs e)
        {
            device = applet.OpenDeviceByType(e.DeviceType);
            device.SoftButtonsChanged += new EventHandler<LcdSoftButtonsEventArgs>(buttonPressed);

            device.SetAsForegroundApplet = true;

            connected = true;

            if (device.DeviceType == LcdDeviceType.Monochrome)
            {
                page = new LcdGdiPage(device);
                Font font = new Font("Microsoft Sans Serif", 25);

                page.Children.Add(new LcdGdiText("MusicBee", font)
                {
                    Margin = new MarginF(2, 0, 0, 0),
                    VerticalAlignment = LcdGdiVerticalAlignment.Middle
                });
                device.CurrentPage = page;

                device.DoUpdateAndDraw();
                firstTime = false;

            }

            else if (device.DeviceType == LcdDeviceType.Qvga)
            {
                page = new LcdGdiPage(device);
                backgroundImage = (Image)Resource.G19logo;
                backgroundGdi = new LcdGdiImage(backgroundImage);
                page.Children.Add(backgroundGdi);
                device.CurrentPage = page;

                device.DoUpdateAndDraw();
                firstTime = false;
            }

        }

        private void buttonPressed(object sender, LcdSoftButtonsEventArgs e)
        {

        }


        private void createMonochrome()
        {

            page.Children.Clear();
            page.Dispose();
            page = null;

            page = new LcdGdiPage(device);

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


            page.Children.Add(titleGdi);
            page.Children.Add(artistGdi);
            page.Children.Add(titleScroll);
            page.Children.Add(artistScroll);
            page.Children.Add(positionGdi);
            page.Children.Add(durationGdi);
            page.Children.Add(progressBarGdi);

            device.CurrentPage = page;

            device.DoUpdateAndDraw();
        }

        private void createColor()
        {
            page.Children.Clear();
            page.Dispose();
            page = null;

            page = new LcdGdiPage(device);
            backgroundImage = (Image)Resource.G19Background;
            backgroundGdi = new LcdGdiImage(backgroundImage);
            page.Children.Add(backgroundGdi);

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

            page.Children.Add(titleGdi);
            page.Children.Add(titleScroll);
            page.Children.Add(artistGdi);
            page.Children.Add(positionGdi);
            page.Children.Add(durationGdi);
            page.Children.Add(progressBarGdi);
            page.Children.Add(albumGdi);
            page.Children.Add(artworkGdi);



            device.CurrentPage = page;

            device.DoUpdateAndDraw();

        }

        public void changeState(MusicBeePlugin.Plugin.PlayState state)
        {

            this.state = state;

            if (device != null && progressBarGdi == null && state == Plugin.PlayState.Playing)
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
            if (base64String != "")
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



    }
}
