using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GammaJul;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace MusicBeePlugin
{

    class Logitech
    {
        public bool connected = false;
        private LcdApplet applet = null;
        private LcdDevice device = null;
        private bool firstTime = true;
        private LcdGdiPage page = null;
        private Font font = new Font("Microsoft Sans Serif", 8);
        private Font font2 = new Font("Microsoft Sans Serif", 7);

        private int timerTime = 0;
        private int position = 0;
        private int duration = 0;
        private string artist = "";
        private string album = "";
        private string title = "";
        private string artwork = "";
        private MusicBeePlugin.Plugin.PlayState state = Plugin.PlayState.Undefined;

        private LcdGdiText titleGdi = null;
        private LcdGdiText artistGdi = null;
        private LcdGdiText positionGdi = null;
        private LcdGdiText durationGdi = null;
        private LcdGdiProgressBar progressBarGdi = null;
        private Timer timer = new Timer(TimerCallback, null, 50, Timeout.Infinite);

        static Logitech logitechObject = null;

        public Logitech()
        {
            Logitech.logitechObject = this;
        }

        public void connect()
        {
            LcdAppletCapabilities appletCapabilities = LcdAppletCapabilities.Both;
            applet = new LcdApplet("MusicBee V2", appletCapabilities, false);
            applet.DeviceArrival += new EventHandler<LcdDeviceTypeEventArgs>(applet_DeviceArrival);
            applet.Connect();
        }

        public void disconnect()
        {
            //applet.Disconnect();
            //applet.Dispose();
            //applet = null;
        }

        public void changeArtistTitle(string artist, string album, string title, string artwork, int duration, int position)
        {
            this.artist = artist;
            this.album = album;
            this.title = title;
            this.artwork = artwork;
            this.duration = duration;
            this.position = position;

            if (getFirstTime())
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

            else
            {
                titleGdi.Text = title;
                artistGdi.Text = artist;
                positionGdi.Text = timetoString(position);
                durationGdi.Text = timetoString(duration);
                int progresstime = (int)(((float)position / (float)duration) * 100);
                progressBarGdi.Value = progresstime;
                device.DoUpdateAndDraw();
            }
        }

        public bool getFirstTime()
        {
            return firstTime;
        }

        private static void TimerCallback(Object state)
        {
            if (logitechObject.title != null && (logitechObject.state == Plugin.PlayState.Playing || logitechObject.state == Plugin.PlayState.Stopped))
            {
                //Update progressbar and position time on the screen after 1 second of music.
                if (logitechObject.state == Plugin.PlayState.Playing && logitechObject.timerTime == 1000)
                {
                    logitechObject.timerTime = 0;
                    logitechObject.position++;
                    int progresstime = (int)(((float)logitechObject.position / (float)logitechObject.duration) * 100);
                    logitechObject.progressBarGdi.Value = progresstime;
                    logitechObject.positionGdi.Text = logitechObject.timetoString(logitechObject.position);
                }

                else if (logitechObject.state == Plugin.PlayState.Playing)
                {
                    logitechObject.timerTime += 50;
                }

                //If music stopped then the progressbar and time must stop immediately
                else if (logitechObject.state == Plugin.PlayState.Stopped)
                {
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
                Image logo = (Image)Resource.G19logo;
                page.Children.Add(new LcdGdiImage(logo));
                device.CurrentPage = page;

                device.DoUpdateAndDraw();
                firstTime = false;
            }
        }

        private void createMonochrome()
        {
            page.Children.Clear();
            page.Dispose();
            page = null;

            page = new LcdGdiPage(device);

            LcdGdiText title = new LcdGdiText(this.title, font);
            title.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            title.Margin = new MarginF(-2, -1, 0, 0);

            LcdGdiText artist = new LcdGdiText(this.artist, font);
            artist.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
            artist.Margin = new MarginF(-2, 12, 0, 0);

            LcdGdiText position = new LcdGdiText(timetoString(this.position), font2);
            position.HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
            position.Margin = new MarginF(10, 26, 0, 0);

            LcdGdiText duration = new LcdGdiText(timetoString(this.duration), font2);
            duration.HorizontalAlignment = LcdGdiHorizontalAlignment.Right;
            duration.Margin = new MarginF(0, 26, 13, 0);

            LcdGdiProgressBar progressBar = new LcdGdiProgressBar();
            progressBar.Size = new SizeF(136, 5);
            progressBar.Margin = new MarginF(12, 38, 0, 0);

            page.Children.Add(title);
            page.Children.Add(artist);
            page.Children.Add(position);
            page.Children.Add(duration);
            page.Children.Add(progressBar);

            device.CurrentPage = page;

            device.DoUpdateAndDraw();
        }

        private void createColor()
        {

        }

        public void changeState(MusicBeePlugin.Plugin.PlayState state)
        {
            switch (state)
            {
                case Plugin.PlayState.Playing:

                    state = Plugin.PlayState.Playing;

                    if (getFirstTime())
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
                    break;

                case Plugin.PlayState.Paused:

                    state = Plugin.PlayState.Paused;

                    if (!getFirstTime())
                    {

                    }
                    break;

                case Plugin.PlayState.Stopped:

                    state = Plugin.PlayState.Stopped;

                    if (!getFirstTime())
                    {

                    }
                    break;

                case Plugin.PlayState.Loading:

                    state = Plugin.PlayState.Loading;

                    if (!getFirstTime())
                    {

                    }
                    break;

                case Plugin.PlayState.Undefined:

                    state = Plugin.PlayState.Undefined;

                    if (!getFirstTime())
                    {

                    }
                    break;

            };
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


        public void setPosition(int position)
        {
            this.position = position;
        }

        public void setDuration(int duration)
        {
            this.duration = duration;
        }
    }
}
