//using GammaJul.LgLcd;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using GammaJul;
//using System.Drawing;
//using System.IO;
//using System.Reflection;
//using System.Threading;
//using System.Drawing.Drawing2D;
//using System.Collections;

//namespace MusicBeePlugin
//{
//  public class Logitech
//  {
//    #region Properties
//    static Logitech logitechObject = null;
    

//    public bool connected = false;

//    private MusicBeePlugin.Plugin.PlayState state = Plugin.PlayState.Undefined;

//    private Plugin musicBeePlugin = null;

//    private LcdGdiPage[] page = null;

//    private int timerTime = 0;

//    private int alwaysOnTopCounter = 0;
//    private int pageNumber = 0;
//    private int settingSelected = 0;
//    private int controlSelected = 0;
//    private int repeatSelected = 0;

//    private bool firstTime = true;
//    private bool eventHappened = true;
//    private bool alwaysOnTop = true;

//    private bool started = false;
//    private bool volumeActive = false;

//    private AutoResetEvent autoEvent = null;
//    private Timer timer = null;

//    private int currentPage = 0;

//   // private MusicBeePlugin.Plugin.RepeatMode[] repeatArray = new Plugin.RepeatMode[3];


//    #endregion

//    #region Consturctor

//    public Logitech(Plugin plugin)
//    {
//      this.musicBeePlugin = plugin;
//      Logitech.logitechObject = this;

//      //repeatArray[0] = Plugin.RepeatMode.None;
//      //repeatArray[1] = Plugin.RepeatMode.One;
//      //repeatArray[2] = Plugin.RepeatMode.All;

      
//      // Create an event to signal the timeout count threshold in the 
//      // timer callback.
//      autoEvent = new AutoResetEvent(false);

//      // Create an inferred delegate that invokes methods for the timer.
//      TimerCallback tcb = TimerCallback;

//      // Create a timer that signals the delegate to invoke  
//      // CheckStatus after one second, and every 1/4 second  
//      // thereafter.
//      timer = new Timer(tcb, autoEvent, 1000, 500);
//    }

//    public void disconnect()
//    {
//      timer.Dispose();
//      //applet.Disconnect();
//      //applet = null;
//    }

//    #endregion
//    #region Getters/Setters

//    //public bool getFirstTime()
//    //{
//    //  return firstTime;
//    //}

//    //public void setPosition(int position)
//    //{
//    //  this.position = position / 1000;
//    //}

//    //public void setDuration(int duration)
//    //{
//    //  this.duration = duration / 1000;
//    //}
//    #endregion




//    public void getSongData()
//    {
//      musicBeePlugin.songChanged();
//    }

//    public void getPlayerSettings()
//    {
//      throw new NotImplementedException();
//    }

//    public void getVolume()
//    {
      
//    }

   
//    public void songChanged(string artist, string album, string title, float rating, string artwork, int duration, int position)
//    {

//      throw new NotImplementedException();
//      eventHappened = true;

//      if (device != null && progressBarGdi == null)
//      {
//        if (device.DeviceType == LcdDeviceType.Monochrome)
//        {
//          createMonochrome();
//        }

//        else if (device.DeviceType == LcdDeviceType.Qvga)
//        {
//          createColor();
//        }
//      }



//      if (device != null && progressBarGdi != null)
//      {


//        try
//        {
//          device.DoUpdateAndDraw();
//        }
//        catch (System.InvalidOperationException)
//        {

//        }
//      }
//    }

//    public void playerSettingsChanged(bool autoDJ, bool equaliser, bool shuffle, MusicBeePlugin.Plugin.RepeatMode repeat)
//    {
//      throw new NotImplementedException();
//    }

//    public void volumeChanged(float volume)
//    {
//      throw new NotImplementedException();
//    }

//    public static void TimerCallback(Object state)
//    {
//      AutoResetEvent autoEvent = (AutoResetEvent)state;

//      if (logitechObject.connected)
//      {
//        if (!logitechObject.alwaysOnTop && logitechObject.eventHappened)
//        {
//          logitechObject.device.SetAsForegroundApplet = true;
//          logitechObject.alwaysOnTopCounter += 500;

//          if (logitechObject.alwaysOnTopCounter >= 5000)
//          {
//            logitechObject.eventHappened = false;
//            logitechObject.alwaysOnTopCounter = 0;
//            logitechObject.device.SetAsForegroundApplet = false;
//          }
//        }

//        if (logitechObject.state == MusicBeePlugin.Plugin.PlayState.Playing)
//        {
//          logitechObject.timerTime += 500;
//        }

//        //Update progressbar and position time on the screen after 1 second of music.

//        if (logitechObject.state == MusicBeePlugin.Plugin.PlayState.Playing && logitechObject.timerTime == 1000)
//        {
//          logitechObject.timerTime = 0;
//          logitechObject.position++;
//          int progresstime = (int)(((float)logitechObject.position / (float)logitechObject.duration) * 100);
//          logitechObject.progressBarGdi.Value = progresstime;
//          logitechObject.positionGdi.Text = logitechObject.timetoString(logitechObject.position);
//        }

//        //If music stopped then the progressbar and time must stop immediately
//        else if (logitechObject.state == MusicBeePlugin.Plugin.PlayState.Stopped)
//        {
//          logitechObject.timerTime = 0;
//          logitechObject.position = 0;
//          logitechObject.progressBarGdi.Value = 0;
//          logitechObject.positionGdi.Text = "00:00";
//        }

//        try
//        {
//          logitechObject.device.DoUpdateAndDraw();
//        }
//        catch (System.InvalidOperationException)
//        {
//          //Noting to do
//        }
//      }
//    }



    

//    private void createMonochrome()
//    {
//      pageNumber = 2;

//      page[0].Children.Clear();
//      page[0].Dispose();
//      page[0] = null;

//      page[0] = new LcdGdiPage(device);
//      page[1] = new LcdGdiPage(device);
//      page[2] = new LcdGdiPage(device);
//      page[3] = new LcdGdiPage(device);


//      device.CurrentPage = page[2];

//      device.DoUpdateAndDraw();
//      started = true;
//    }

//    private void createColor()
//    {
//      pageNumber = 1;

//      page[0].Children.Clear();
//      page[0].Dispose();
//      page[0] = null;

//      page[0] = new LcdGdiPage(device);
//      page[1] = new LcdGdiPage(device);
//      page[2] = new LcdGdiPage(device);

//      device.CurrentPage = page[1];

//      device.DoUpdateAndDraw();
//      started = true;

//    }

//    public void changePlayState(int controlSelected)
//    {
//      throw new NotImplementedException();
//    }


//    public void changeState(MusicBeePlugin.Plugin.PlayState state)
//    {
//      eventHappened = true;
//      this.state = state;

//      if (device != null && progressBarGdi == null && state != Plugin.PlayState.Playing)
//      {
//        if (device.DeviceType == LcdDeviceType.Monochrome)
//        {
//          createMonochrome();
//        }
//        else
//        {
//          createColor();
//        }
//      }
//    }

//    public void settingsChanged(bool alwaysOnTop_, int volume)
//    {
//      try
//      {
//        this.volumeChanger = volume / 100f;

//        this.alwaysOnTop = alwaysOnTop_;
//        device.SetAsForegroundApplet = alwaysOnTop;
//      }
//      catch (Exception)
//      {

//      }
//    }

//    internal void changeRating(float rating)
//    {
//      throw new NotImplementedException();
//    }

//    internal void changeVolume(float volume_)
//    {
//      throw new NotImplementedException();
//    }
//  }
//}
