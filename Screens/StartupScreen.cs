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

    public StartupScreen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin plugin, int index)
      : base(device, type, backgroundGdi, plugin, index)
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
  }
}
