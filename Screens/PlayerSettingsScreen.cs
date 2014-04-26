using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Screens
{
  class PlayerSettingsScreen : Screen
  {
    private LcdGdiText[] settingsGdi_ = null;

    private bool autoDJ_ = false;
    private bool equaliser_ = false;
    private bool shuffle_ = false;
    private MusicBeePlugin.Plugin.RepeatMode repeat_;
    private int settingSelected_ = 0;

    private int repeatSelected_ = 0;
    private bool volumeActive_ = false;
    private float volumeChanger_ = 0.1f;
    private float volume_ = 1;

    private MusicBeePlugin.Plugin.RepeatMode[] repeatArray_ = new Plugin.RepeatMode[3];

    private LcdGdiProgressBar volumeBarGdi_ = null;

    public PlayerSettingsScreen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin plugin)
      : base(device, type, backgroundGdi, plugin)
    {
      screenName_ = "SettingsScreen";

      plugin_.getSongData();

      repeatArray_[0] = Plugin.RepeatMode.None;
      repeatArray_[1] = Plugin.RepeatMode.One;
      repeatArray_[2] = Plugin.RepeatMode.All;

      if (type == LcdDeviceType.Monochrome)
      {
        settingsGdi_ = new LcdGdiText[4];
        createMono();
      }
      else if (type == LcdDeviceType.Qvga)
      {
        settingsGdi_ = new LcdGdiText[5];
        createColor();
      }
    }

    private void createMono()
    {
      settingsGdi_[0] = new LcdGdiText("shuffle_: " + shuffle_, font2_);
      settingsGdi_[0].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      settingsGdi_[0].Margin = new MarginF(-2, 0, 0, 0);

      settingsGdi_[1] = new LcdGdiText("Auto DJ: " + autoDJ_, font2_);
      settingsGdi_[1].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      settingsGdi_[1].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      settingsGdi_[1].Margin = new MarginF(-2, 10, 0, 0);

      settingsGdi_[2] = new LcdGdiText("Equalizer: " + equaliser_, font2_);
      settingsGdi_[2].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      settingsGdi_[2].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      settingsGdi_[2].Margin = new MarginF(-2, 20, 0, 0);

      settingsGdi_[3] = new LcdGdiText("Repeat: " + repeat_, font2_);
      settingsGdi_[3].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      settingsGdi_[3].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      settingsGdi_[3].Margin = new MarginF(-2, 30, 0, 0);

      this.Children.Add(settingsGdi_[0]);
      this.Children.Add(settingsGdi_[1]);
      this.Children.Add(settingsGdi_[2]);
      this.Children.Add(settingsGdi_[3]);
    }

    private void createColor()
    {
      settingsGdi_[0] = new LcdGdiText("shuffle_: " + shuffle_, font3_);
      settingsGdi_[0].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      settingsGdi_[0].Margin = new MarginF(5, 5, 5, 0);

      settingsGdi_[1] = new LcdGdiText("Auto DJ: " + autoDJ_, font3_);
      settingsGdi_[1].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      settingsGdi_[1].Margin = new MarginF(5, 30, 5, 0);

      settingsGdi_[2] = new LcdGdiText("Equalizer: " + equaliser_, font3_);
      settingsGdi_[2].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      settingsGdi_[2].Margin = new MarginF(5, 55, 5, 0);

      settingsGdi_[3] = new LcdGdiText("Repeat: " + repeat_, font3_);
      settingsGdi_[3].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      settingsGdi_[3].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      settingsGdi_[3].Margin = new MarginF(5, 80, 0, 0);

      settingsGdi_[4] = new LcdGdiText("Volume", font3_);
      settingsGdi_[4].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      settingsGdi_[4].VerticalAlignment = LcdGdiVerticalAlignment.Top;
      settingsGdi_[4].Margin = new MarginF(5, 105, 0, 0);

      volumeBarGdi_ = new LcdGdiProgressBar();
      volumeBarGdi_.Minimum = 0;
      volumeBarGdi_.Maximum = 100;
      volumeBarGdi_.Size = new SizeF(310, 20);
      volumeBarGdi_.Margin = new MarginF(5, 150, 5, 0);

      this.Children.Add(backgroundGdi_);
      this.Children.Add(settingsGdi_[0]);
      this.Children.Add(settingsGdi_[1]);
      this.Children.Add(settingsGdi_[2]);
      this.Children.Add(settingsGdi_[3]);
      this.Children.Add(settingsGdi_[4]);
      this.Children.Add(volumeBarGdi_);
    }

    public override void buttonPressedMonochrome(object sender, LcdSoftButtonsEventArgs e)
    {
      if ((e.SoftButtons & LcdSoftButtons.Button0) == LcdSoftButtons.Button0)
      {
        switch (settingSelected_)
        {
          case 0:
            shuffle_ = !shuffle_;
            this.settingsGdi_[0].Text = "shuffle_: " + shuffle_;
            break;

          case 1:
            autoDJ_ = !autoDJ_;
            this.settingsGdi_[1].Text = "Auto DJ: " + autoDJ_;
            break;

          case 2:
            equaliser_ = !equaliser_;
            settingsGdi_[2].Text = "Equalizer: " + equaliser_;
            break;

          case 3:

            if (repeatSelected_ == 2)
            {
              repeatSelected_ = 0;
            }
            else
            {
              repeatSelected_++;
            }
            repeat_ = repeatArray_[repeatSelected_];
            settingsGdi_[3].Text = "Repeat: " + repeat_;
            break;
        };
      }

      else if (((e.SoftButtons & LcdSoftButtons.Button1) == LcdSoftButtons.Button1))
      {
        if (settingSelected_ != 0)
        {
          settingSelected_ -= 1;
        }

        for (int i = 0; i < settingsGdi_.Length; i++)
        {
          settingsGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        }

        settingsGdi_[settingSelected_].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;

      }

            // Third button is pressed
      else if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button2)
      {
        if (settingSelected_ != 3)
        {
          settingSelected_ += 1;
        }

        for (int i = 0; i < settingsGdi_.Length; i++)
        {
          settingsGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        }

        settingsGdi_[settingSelected_].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      }

      else if ((e.SoftButtons & LcdSoftButtons.Button3) == LcdSoftButtons.Button3)
      {
        plugin_.goToNextPage();
      }
    }

    public override void buttonPressedColor(object sender, LcdSoftButtonsEventArgs e)
    {
      //G19 up button pressed
      if ((e.SoftButtons & LcdSoftButtons.Up) == LcdSoftButtons.Up)
      {
        if (volumeActive_)
        {
          volume_ += volumeChanger_;

          if (volume_ > 1)
          {
            volume_ = 1;
          }

          plugin_.changeVolume(volume_);
        }
        else
        {
          if (settingSelected_ != 0)
          {
            settingSelected_ -= 1;
          }
        }
        for (int i = 0; i < settingsGdi_.Length; i++)
        {
          settingsGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        }

        settingsGdi_[settingSelected_].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      }

      //G19 down button pressed
      else if ((e.SoftButtons & LcdSoftButtons.Down) == LcdSoftButtons.Down)
      {
        if (volumeActive_)
        {
          volume_ -= volumeChanger_;

          if (volume_ < 0)
          {
            volume_ = 0;
          }

          plugin_.changeVolume(volume_);
        }
        else
        {
          if (settingSelected_ != this.settingsGdi_.Length - 1)
          {
            settingSelected_ += 1;
          }
        }
        for (int i = 0; i < settingsGdi_.Length; i++)
        {
          settingsGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        }

        settingsGdi_[settingSelected_].HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      }

       //G19 Ok button
      else if ((e.SoftButtons & LcdSoftButtons.Ok) == LcdSoftButtons.Ok)
      {
        switch (settingSelected_)
        {
          case 0:
            shuffle_ = !shuffle_;
            settingsGdi_[0].Text = "Shuffle: " + shuffle_;
            break;

          case 1:
            autoDJ_ = !autoDJ_;
            settingsGdi_[1].Text = "Auto DJ: " + autoDJ_;
            break;

          case 2:
            equaliser_ = !equaliser_;
            settingsGdi_[2].Text = "Equalizer: " + equaliser_;
            break;

          case 3:

            if (repeatSelected_ == 2)
            {
              repeatSelected_ = 0;
            }
            else
            {
              repeatSelected_++;
            }
            repeat_ = repeatArray_[repeatSelected_];
            settingsGdi_[3].Text = "Repeat: " + repeat_;
            break;
          case 4:
            volumeActive_ = !volumeActive_;
            break;
        };
      }

      else if ((e.SoftButtons & LcdSoftButtons.Left) == LcdSoftButtons.Left)
      {
        plugin_.goToPreviousPage();
      }

      else if ((e.SoftButtons & LcdSoftButtons.Right) == LcdSoftButtons.Right)
      {
        plugin_.goToNextPage();
      }
    }

    public override void songChanged(string artist, string album, string title, float rating, string artwork, int duration, int position)
    {
      
    }

    public override void volumeChanged(float volume)
    {
      volumeBarGdi_.Value = (int)(volume * 100);
    }

    public override void playerSettingsChanged(bool autoDJ, bool equaliser, bool shuffle_, MusicBeePlugin.Plugin.RepeatMode repeat)
    {
      if (shuffle_)
      {
        settingsGdi_[0].Text = "shuffle_: Enabled";
      }
      else
      {
        settingsGdi_[0].Text = "shuffle_: Disabled";
      }


      if (autoDJ)
      {
        settingsGdi_[1].Text = "Auto DJ = Enabled";
      }
      else
      {
        settingsGdi_[1].Text = "Auto DJ = Disabled";
      }

      if (equaliser)
      {
        settingsGdi_[2].Text = "Equaliser: Enabled";
      }
      else
      {
        settingsGdi_[2].Text = "Equaliser: Disabled";
      }

      settingsGdi_[3].Text = "Repeat: " + repeat;
    }
  }
}
