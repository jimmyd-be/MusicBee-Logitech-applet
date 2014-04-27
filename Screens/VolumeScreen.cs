using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Screens
{
  class VolumeScreen : Screen
  {
    private LcdGdiIcon lowVolumeIcon_;
    private LcdGdiIcon highVolumeIcon_;

    private LcdGdiImage lowVolumeImageGdi_;
    private LcdGdiImage highVolumeImageGdi_;

    private Image lowVolumeImage_ = null;
    private Image highVolumeImage_ = null;

    private LcdGdiProgressBar volumeBarGdi_ = null;

    private float volume_ = 1;
    private float volumeChanger_ = 0.1f;

    public VolumeScreen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin plugin, int index, int volume)
      : base(device, type, backgroundGdi, plugin, index)
    {
      screenName_ = "VolumeScreen";
      plugin_.getSongData();

      volumeChanger_ = volume / 100f;

      if (type == LcdDeviceType.Monochrome)
      {
        createMono();
      }
      else if(type == LcdDeviceType.Qvga)
      {
        createColor();
      }
    }

    private void createMono()
    {
      volumeBarGdi_ = new LcdGdiProgressBar();
      volumeBarGdi_.Size = new SizeF(136, 5);
      volumeBarGdi_.Margin = new MarginF(12, 38, 0, 0);
      volumeBarGdi_.Minimum = 0;
      volumeBarGdi_.Maximum = 100;
      volumeBarGdi_.Value = (int)(this.volume_ * 100);

      Icon icon = (Icon)Resource.lowVolume;
      icon = new Icon(icon, 16, 16);
      lowVolumeIcon_ = new LcdGdiIcon(icon);
      lowVolumeIcon_.Margin = new MarginF(10, 20, 0, 0);

      icon = (Icon)Resource.highVolume;

      icon = new Icon(icon, 16, 16);
      highVolumeIcon_ = new LcdGdiIcon(icon);
      highVolumeIcon_.Margin = new MarginF(134, 20, 0, 0);

      this.Children.Add(volumeBarGdi_);
      this.Children.Add(highVolumeIcon_);
      this.Children.Add(lowVolumeIcon_);
    }

    private void createColor()
    {
      volumeBarGdi_ = new LcdGdiProgressBar();
      volumeBarGdi_.Minimum = 0;
      volumeBarGdi_.Maximum = 100;
      volumeBarGdi_.Size = new SizeF(310, 20);
      volumeBarGdi_.Margin = new MarginF(5, 150, 5, 0);

      highVolumeImage_ = (Image)Resource.highVolumeG19;
      highVolumeImageGdi_ = new LcdGdiImage(highVolumeImage_);
      highVolumeImageGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Right;
      highVolumeImageGdi_.Margin = new MarginF(0, 90, 5, 0);

      lowVolumeImage_ = (Image)Resource.lowVolumeG19;
      lowVolumeImageGdi_ = new LcdGdiImage(lowVolumeImage_);
      lowVolumeImageGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      lowVolumeImageGdi_.Margin = new MarginF(5, 90, 5, 0);

      this.Children.Add(backgroundGdi_);
      this.Children.Add(highVolumeImageGdi_);
      this.Children.Add(lowVolumeImageGdi_);
      this.Children.Add(volumeBarGdi_);
    }

    public override void buttonPressedMonochrome(object sender, LcdSoftButtonsEventArgs e)
    {
      // First button is pressed, switch to page one
      if ((e.SoftButtons & LcdSoftButtons.Button0) == LcdSoftButtons.Button0)
      {
        plugin_.goToPreviousPage();
      }

      // Second button is pressed
      else if (((e.SoftButtons & LcdSoftButtons.Button1) == LcdSoftButtons.Button1))
      {
        volume_ -= volumeChanger_;

        if (volume_ < 0)
        {
          volume_ = 0;
        }

        plugin_.changeVolume(volume_);
      }

       // Third button is pressed
      else if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button2)
      {
        volume_ += volumeChanger_;

        if (volume_ > 1)
        {
          volume_ = 1;
        }

        plugin_.changeVolume(volume_);
      }

      // Fourth button is pressed
      else if ((e.SoftButtons & LcdSoftButtons.Button3) == LcdSoftButtons.Button3)
      {
        plugin_.goToNextPage();
      }
    }

    public override void buttonPressedColor(object sender, LcdSoftButtonsEventArgs e)
    {
      if ((e.SoftButtons & LcdSoftButtons.Left) == LcdSoftButtons.Left)
      {
        plugin_.goToPreviousPage();
      }

          //G19 up button pressed
      else if ((e.SoftButtons & LcdSoftButtons.Up) == LcdSoftButtons.Up)
      {
        volume_ += volumeChanger_;

        if (volume_ > 1)
        {
          volume_ = 1;
        }

        plugin_.changeVolume(volume_);
      }

      //G19 down button pressed
      else if ((e.SoftButtons & LcdSoftButtons.Down) == LcdSoftButtons.Down)
      {
        volume_ -= volumeChanger_;

        if (volume_ < 0)
        {
          volume_ = 0;
        }

        plugin_.changeVolume(volume_);
      }

          //G19 Right button
      else if ((e.SoftButtons & LcdSoftButtons.Right) == LcdSoftButtons.Right)
      {
        plugin_.goToNextPage();
      }

          //G19 Ok button
      else if ((e.SoftButtons & LcdSoftButtons.Ok) == LcdSoftButtons.Ok)
      {
      }
    }

    public override void volumeChanged(float volume)
    {
      volume_ = volume;
      volumeBarGdi_.Value = (int)(volume_ * 100f);
    }
  }
}
