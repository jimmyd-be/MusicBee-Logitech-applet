using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Screens
{
  class PlayerControlScreen : Screen
  {

    private LcdGdiText[] controlsGdi_ = null;

    private int controlSelected_ = 0;

    public PlayerControlScreen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin plugin, int index)
      : base(device, type, backgroundGdi, plugin, index)
    {
      screenName_ = "ControlScreen";

      plugin_.getSongData();

      if (type == LcdDeviceType.Monochrome)
      {
        controlsGdi_ = new LcdGdiText[5];

        createMono();
      }
      else if (type == LcdDeviceType.Qvga)
      {
        controlsGdi_ = new LcdGdiText[5];

        createColor();
      }
    }

    private void createMono()
    {
      controlsGdi_[0] = new LcdGdiText("Start/Pauze", font2_);
      controlsGdi_[0].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      controlsGdi_[0].Margin = new MarginF(-2, -0, 0, 0);

      controlsGdi_[1] = new LcdGdiText("Play next song", font2_);
      controlsGdi_[1].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[1].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      controlsGdi_[1].Margin = new MarginF(-2, 8, 0, 0);

      controlsGdi_[2] = new LcdGdiText("Play previous song", font2_);
      controlsGdi_[2].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[2].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      controlsGdi_[2].Margin = new MarginF(-2, 16, 0, 0);

      controlsGdi_[3] = new LcdGdiText("Stop playing", font2_);
      controlsGdi_[3].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[3].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      controlsGdi_[3].Margin = new MarginF(-2, 24, 0, 0);

      controlsGdi_[4] = new LcdGdiText("Restart song", font2_);
      controlsGdi_[4].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[4].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      controlsGdi_[4].Margin = new MarginF(-2, 32, 0, 0);

      this.Children.Add(controlsGdi_[0]);
      this.Children.Add(controlsGdi_[1]);
      this.Children.Add(controlsGdi_[2]);
      this.Children.Add(controlsGdi_[3]);
      this.Children.Add(controlsGdi_[4]);
    }

    private void createColor()
    {
      controlsGdi_[0] = new LcdGdiText("Start/Pauze", font3_);
      // controlsGdi[0].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[0].Margin = new MarginF(5, 5, 5, 0);

      controlsGdi_[0].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;

      controlsGdi_[1] = new LcdGdiText("Play next song", font3_);
      controlsGdi_[1].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[1].Margin = new MarginF(5, 30, 5, 0);

      controlsGdi_[2] = new LcdGdiText("Play previous song", font3_);
      controlsGdi_[2].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[2].Margin = new MarginF(5, 55, 5, 0);

      controlsGdi_[3] = new LcdGdiText("Stop playing", font3_);
      controlsGdi_[3].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[3].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      controlsGdi_[3].Margin = new MarginF(5, 80, 0, 0);

      controlsGdi_[4] = new LcdGdiText("Restart song", font3_);
      controlsGdi_[4].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      controlsGdi_[4].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      controlsGdi_[4].Margin = new MarginF(5, 105, 0, 0);

      this.Children.Add(backgroundGdi_);
      this.Children.Add(controlsGdi_[0]);
      this.Children.Add(controlsGdi_[1]);
      this.Children.Add(controlsGdi_[2]);
      this.Children.Add(controlsGdi_[3]);
      this.Children.Add(controlsGdi_[4]);
    }


    public override void buttonPressedMonochrome(object sender, LcdSoftButtonsEventArgs e)
    {
      // First button is pressed, switch to page one
      if ((e.SoftButtons & LcdSoftButtons.Button0) == LcdSoftButtons.Button0)
      {
        if (index_ != 0)
        {
          plugin_.goToPreviousPage();
        }
        else
        {
          plugin_.changePlayState(controlSelected_);
        }
      }

      // Second button is pressed
      else if (((e.SoftButtons & LcdSoftButtons.Button1) == LcdSoftButtons.Button1))
      {
        if (controlSelected_ != 0)
        {
          controlSelected_ -= 1;
        }

        for (int i = 0; i < controlsGdi_.Length; i++)
        {
          controlsGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        }

        controlsGdi_[controlSelected_].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      }

            // Third button is pressed
      else if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button2)
      {
        if (controlSelected_ != 4)
        {
          controlSelected_ += 1;
        }

        for (int i = 0; i < controlsGdi_.Length; i++)
        {
          controlsGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        }

        controlsGdi_[controlSelected_].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      }

           // Fourth button is pressed
      else if ((e.SoftButtons & LcdSoftButtons.Button3) == LcdSoftButtons.Button3)
      {
        if (index_ == 0)
        {
          plugin_.goToNextPage();
        }
        else
        {
          plugin_.changePlayState(controlSelected_);
        }
      }
    }

    public override void positionChanged(int position)
    {

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
        if (controlSelected_ != 0)
        {
          controlSelected_ -= 1;
        }

        for (int i = 0; i < controlsGdi_.Length; i++)
        {
          controlsGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        }

        controlsGdi_[controlSelected_].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      }

      //G19 down button pressed
      else if ((e.SoftButtons & LcdSoftButtons.Down) == LcdSoftButtons.Down)
      {
        if (controlSelected_ != controlsGdi_.Length - 1)
        {
          controlSelected_ += 1;
        }

        for (int i = 0; i < controlsGdi_.Length; i++)
        {
          controlsGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        }

        controlsGdi_[controlSelected_].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      }

          //G19 Right button
      else if ((e.SoftButtons & LcdSoftButtons.Right) == LcdSoftButtons.Right)
      {
        plugin_.goToNextPage();
      }

          //G19 Ok button
      else if ((e.SoftButtons & LcdSoftButtons.Ok) == LcdSoftButtons.Ok)
      {
        plugin_.changePlayState(controlSelected_);
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
      throw new NotImplementedException();
    }
  }
}
