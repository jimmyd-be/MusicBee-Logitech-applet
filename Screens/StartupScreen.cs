using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Screens
{
  class StartupScreen: Screen
  {
    private Font font = new Font("Microsoft Sans Serif", 25);

    public StartupScreen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin plugin)
      : base(device, type, backgroundGdi, plugin)
    {
      screenName_ = "StartupScreen";

      if (type == LcdDeviceType.Monochrome)
      {
        createMono();
      }
      else if (type == LcdDeviceType.Qvga)
      {
        createColor();
      }
    }

    public override void songChanged(string artist, string album, string title, float rating, string artwork, int duration, int position)
    {

    }

    public override void volumeChanged(float volume)
    {

    }

    public override void playerSettingsChanged(bool autoDJ, bool equaliser, bool shuffle, MusicBeePlugin.Plugin.RepeatMode repeat)
    {

    }

   private void createMono()
    {
      this.Children.Add(new LcdGdiText("MusicBee", font)
      {
        Margin = new MarginF(2, 0, 0, 0),
        VerticalAlignment = LcdGdiVerticalAlignment.Middle
      });
    }

   private void createColor()
    {
      backgroundImage_ = (Image)Resource.G19logo;
      backgroundGdi_ = new LcdGdiImage(backgroundImage_);
      this.Children.Add(backgroundGdi_);
    }


    public override void buttonPressedMonochrome(object sender, LcdSoftButtonsEventArgs e)
    {

    }

    public override void buttonPressedColor(object sender, LcdSoftButtonsEventArgs e)
    {

    }

    public override void update()
    {

    }

    public override void settingsChanged()
    {

    }
    public override void songChanged()
    {

    }

    public override void playerStatusChanged(MusicBeePlugin.Plugin.PlayState state)
    {

    }
  }
}
