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
        private Timer timer = null;

        private int timerTime = 0;
        private int position = 0;
        private int duration = 0;
        private string artist = "";
        private string album = "";
        private string title = "";

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

        }

        public bool getFirstTime()
        {
            return firstTime;
        }

        private void applet_DeviceArrival(object sender, LcdDeviceTypeEventArgs e)
        {
            device = applet.OpenDeviceByType(e.DeviceType);

            device.SetAsForegroundApplet = true;

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

            else
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
            page.Children.RemoveAt(0);
        }

        private void createColor()
        {
            page.Children.RemoveAt(0);
        }

        public void changeState(MusicBeePlugin.Plugin.PlayState state)
        {
            switch (state)
            {
                case Plugin.PlayState.Playing:

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

                        timer = new Timer(new TimerCallback(TimerProc));

                        timer.Change(0, 50);

                        firstTime = false;

                    }
                    break;

                case Plugin.PlayState.Paused:
                    if (!getFirstTime())
                    {

                    }
                    break;
                case Plugin.PlayState.Stopped:
                    if (!getFirstTime())
                    {

                    }
                    break;
                case Plugin.PlayState.Loading:
                    if (!getFirstTime())
                    {

                    }
                    break;
                case Plugin.PlayState.Undefined:
                    if (!getFirstTime())
                    {

                    }
                    break;

            };
        }

        private void TimerProc(object state)
        {

            
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
