using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MusicBeePlugin
{
  class Screen : LcdGdiPage
  {
    protected LcdDeviceType type_;

    protected string screenName_ = "";
    protected int index_ = -1;

    protected LcdGdiImage backgroundGdi_ = null;
    protected Image backgroundImage_ = null;

    protected Font font_ = new Font("Microsoft Sans Serif", 8);
    protected Font font2_ = new Font("Microsoft Sans Serif", 7);
    protected Font font3_ = new Font("Arial", 15);
    protected Font font4_ = new Font("Arial", 10);

    protected Plugin plugin_ = null;

    protected LcdDevice device_ = null;

    public Screen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin musicBeePlugin, int index)
      : base(device)
    {
      plugin_ = musicBeePlugin;
      type_ = type;
      device_ = device;
      index_ = index;

      if (device.DeviceType == LcdDeviceType.Qvga && String.IsNullOrEmpty(backgroundGdi))
      {
        backgroundImage_ = (Image)Resource.G19Background;
        backgroundGdi_ = new LcdGdiImage(backgroundImage_);
        this.Children.Add(backgroundGdi_);
      }
      else if(device.DeviceType == LcdDeviceType.Qvga && backgroundGdi != "")
      {
        backgroundImage_ = Image.FromFile(backgroundGdi, true);
        backgroundGdi_ = new LcdGdiImage(backgroundImage_);
        this.Children.Add(backgroundGdi_);
      }
    }


    public virtual void close()
    {
      if (backgroundImage_ != null)
      {
        backgroundImage_.Dispose();
      }
    }
    
    public virtual void songChanged(string artist, string album, string title, float rating, string artwork, int duration, int position, string lyrics)
    {

    }

    public virtual void volumeChanged(float volume)
    {

    }

    public virtual void playerSettingsChanged(bool autoDJ, bool equaliser, bool shuffle, MusicBeePlugin.Plugin.RepeatMode repeat)
    {

    }

    public string getName()
    {
      return screenName_;
    }

    //public virtual void createMono()
    //{

    //}

    //public virtual void createColor()
    //{

    //}

    //public virtual void buttonPressed(object sender, LcdSoftButtonsEventArgs e)
    //{

    //}

    public virtual void buttonPressedMonochrome(object sender, LcdSoftButtonsEventArgs e)
    {

    }

    public virtual void buttonPressedColor(object sender, LcdSoftButtonsEventArgs e)
    {

    }

    public virtual void update()
    {

    }

    public virtual void settingsChanged()
    {

    }
    public virtual void songChanged()
    {

    }

    public virtual void playerStatusChanged(MusicBeePlugin.Plugin.PlayState state)
    {

    }

    public virtual void positionChanged(int position)
    {
      
    }
  }
}
