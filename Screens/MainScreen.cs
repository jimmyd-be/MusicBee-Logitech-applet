using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Screens
{
  class MainScreen : Screen
  {
    private LcdGdiText titleGdi_ = null;
    private LcdGdiText artistGdi_ = null;
    private LcdGdiText positionGdi_ = null;
    private LcdGdiText durationGdi_ = null;
    private LcdGdiText albumGdi_ = null;
    private LcdGdiText ratingGdi_ = null;

    private LcdGdiImage artworkGdi_ = null;

    private LcdGdiProgressBar progressBarGdi_ = null;

    private LcdGdiScrollViewer albumScroll_ = null;
    private LcdGdiScrollViewer titleScroll_ = null;
    private LcdGdiScrollViewer artistScroll_ = null;


    private Image fullStarImage_ = null;
    private Image halfStarImage_ = null;
    private Image emptyStarImage_ = null;
    private Image emptyImage_ = null;

    private LcdGdiImage[] ratingColorGdi_ = null;

    private float rating_ = 0;
    private string artist_ = "";
    private string album_ = "";
    private string title_ = "";
    private string artwork_ = "";

    private int position_ = 0;
    private int duration_ = 0;

    public MainScreen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin plugin, int index)
      : base(device, type, backgroundGdi, plugin, index)
    {
      screenName_ = "MainScreen";
      plugin_.getSongData();

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
      titleGdi_ = new LcdGdiText(this.title_, font_);
      titleGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      titleGdi_.Margin = new MarginF(-2, -1, 0, 0);

      artistGdi_ = new LcdGdiText(this.artist_, font_);
      artistGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      artistGdi_.VerticalAlignment = LcdGdiVerticalAlignment.Top;
      artistGdi_.Margin = new MarginF(-2, 12, 0, 0);

      positionGdi_ = new LcdGdiText(Conversions.timetoString(this.position_), font2_);
      positionGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      positionGdi_.Margin = new MarginF(10, 26, 0, 0);

      ratingGdi_ = new LcdGdiText("", font2_);
      ratingGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      ratingGdi_.Margin = new MarginF(0, 26, 0, 0);

      if (this.rating_ != 0)
      {
        ratingGdi_.Text = this.rating_.ToString() + " / 5";
      }
      else
      {
        ratingGdi_.Text = "";
      }

      durationGdi_ = new LcdGdiText(Conversions.timetoString(this.duration_), font2_);
      durationGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Right;
      durationGdi_.Margin = new MarginF(0, 26, 13, 0);

      progressBarGdi_ = new LcdGdiProgressBar();
      progressBarGdi_.Size = new SizeF(136, 5);
      progressBarGdi_.Margin = new MarginF(12, 38, 0, 0);
      progressBarGdi_.Minimum = 0;
      progressBarGdi_.Maximum = 100;

      titleScroll_ = new LcdGdiScrollViewer(titleGdi_);
      titleScroll_.AutoScrollX = true;
      titleScroll_.AutoScrollY = false;
      titleScroll_.AutoScrollSpeedY = 0;
      titleScroll_.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
      titleScroll_.VerticalAlignment = LcdGdiVerticalAlignment.Top;

      artistScroll_ = new LcdGdiScrollViewer(artistGdi_);
      artistScroll_.AutoScrollY = false;
      artistScroll_.AutoScrollX = true;
      artistScroll_.AutoScrollSpeedY = 0;
      artistScroll_.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
      artistScroll_.VerticalAlignment = LcdGdiVerticalAlignment.Top;

      this.Children.Add(titleGdi_);
      this.Children.Add(artistGdi_);
      this.Children.Add(titleScroll_);
      this.Children.Add(artistScroll_);
      this.Children.Add(positionGdi_);
      this.Children.Add(ratingGdi_);
      this.Children.Add(durationGdi_);
      this.Children.Add(progressBarGdi_);
    }

    private void createColor()
    {
      artistGdi_ = new LcdGdiText(this.artist_, font3_);
      artistGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      artistGdi_.Margin = new MarginF(5, 5, 5, 0);

      titleGdi_ = new LcdGdiText(this.title_, font3_);
      titleGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      titleGdi_.Margin = new MarginF(5, 30, 5, 0);

      albumGdi_ = new LcdGdiText(this.album_, font3_);
      albumGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      albumGdi_.Margin = new MarginF(5, 55, 5, 0);

      positionGdi_ = new LcdGdiText(Conversions.timetoString(this.position_), font4_);
      positionGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
      positionGdi_.Margin = new MarginF(5, 105, 0, 0);

      durationGdi_ = new LcdGdiText(Conversions.timetoString(this.duration_), font4_);
      durationGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Right;
      durationGdi_.Margin = new MarginF(0, 105, 5, 0);

      artworkGdi_ = new LcdGdiImage(Conversions.Base64ToImage(artwork_));
      artworkGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      artworkGdi_.Size = new SizeF(130, 130);
      artworkGdi_.Margin = new MarginF(0, 105, 0, 0);

      progressBarGdi_ = new LcdGdiProgressBar();
      progressBarGdi_.Minimum = 0;
      progressBarGdi_.Maximum = 100;
      progressBarGdi_.Size = new SizeF(310, 20);
      progressBarGdi_.Margin = new MarginF(5, 80, 5, 0);

      titleScroll_ = new LcdGdiScrollViewer(titleGdi_);
      titleScroll_.AutoScrollX = true;
      titleScroll_.AutoScrollY = false;
      titleScroll_.AutoScrollSpeedY = 0;
      titleScroll_.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
      titleScroll_.VerticalAlignment = LcdGdiVerticalAlignment.Top;

      artistScroll_ = new LcdGdiScrollViewer(artistGdi_);
      artistScroll_.AutoScrollY = false;
      artistScroll_.AutoScrollX = true;
      artistScroll_.AutoScrollSpeedY = 0;
      artistScroll_.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
      artistScroll_.VerticalAlignment = LcdGdiVerticalAlignment.Top;

      albumScroll_ = new LcdGdiScrollViewer(albumGdi_);
      albumScroll_.AutoScrollY = false;
      albumScroll_.AutoScrollX = true;
      albumScroll_.AutoScrollSpeedY = 0;
      albumScroll_.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
      albumScroll_.VerticalAlignment = LcdGdiVerticalAlignment.Top;

      fullStarImage_ = (Image)Resource.star_rating_full;
      halfStarImage_ = (Image)Resource.star_rating_half;
      emptyStarImage_ = (Image)Resource.star_rating_empty;
      emptyImage_ = (Image)Resource.empty;

      this.Children.Add(backgroundGdi_);
      this.Children.Add(titleGdi_);
      this.Children.Add(titleScroll_);
      this.Children.Add(artistGdi_);
      this.Children.Add(positionGdi_);
      this.Children.Add(durationGdi_);
      this.Children.Add(progressBarGdi_);
      this.Children.Add(albumGdi_);
      this.Children.Add(artworkGdi_);

      ratingColorGdi_ = new LcdGdiImage[5];

      float tempRating = this.rating_;

      for (int i = 0; i < 5; i++)
      {
        if (this.rating_ != 0)
        {
          if (tempRating - 1 >= 0)
          {
            ratingColorGdi_[i] = new LcdGdiImage(fullStarImage_);
            tempRating--;
          }

          else if (tempRating - 0.5f == 0)
          {
            ratingColorGdi_[i] = new LcdGdiImage(halfStarImage_);
            tempRating -= 0.5f;
          }

          else
          {
            ratingColorGdi_[i] = new LcdGdiImage(emptyStarImage_);
          }
        }
        else
        {
          ratingColorGdi_[i] = new LcdGdiImage(emptyStarImage_);
        }
        ratingColorGdi_[i].HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
        ratingColorGdi_[i].Margin = new MarginF((18 * i) + 5, 215, 0, 0);
        this.Children.Add(ratingColorGdi_[i]);
      }
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
        if (rating_ != 0)
        {
          rating_ -= 0.5f;
          plugin_.changeRating(rating_);
        }

        plugin_.getSongData();
      }

            // Third button is pressed
      else if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button2)
      {
        if (rating_ != 5)
        {
          rating_ += 0.5f;
          plugin_.changeRating(rating_);
        }

        plugin_.getSongData();
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
        if (rating_ != 5)
        {
          rating_ += 0.5f;
          plugin_.changeRating(rating_);
        }

        plugin_.getSongData();
      }

      //G19 down button pressed
      else if ((e.SoftButtons & LcdSoftButtons.Down) == LcdSoftButtons.Down)
      {
        if (rating_ != 0)
        {
          rating_ -= 0.5f;
          plugin_.changeRating(rating_);
        }

        plugin_.getSongData();
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

    public override void positionChanged(int position)
    {
      position_ = position/1000;
      positionGdi_.Text = Conversions.timetoString(position_);
      int progresstime = (int)(((float)position_ / (float)duration_) * 100);
      progressBarGdi_.Value = progresstime;
    }

    public override void songChanged(string artist, string album, string title, float rating, string artwork, int duration, int position, string lyrics)
    {
      artist_ = artist;
      album_ = album;
      title_ = title;
      rating_ = rating;
      artwork_ = artwork;
      duration_ = duration /1000;
      position_ = position /1000;

      titleGdi_.Text = title;
      artistGdi_.Text = artist;
      positionGdi_.Text = Conversions.timetoString(position_);
      durationGdi_.Text = Conversions.timetoString(duration_);
      int progresstime = (int)(((float)position_ / (float)duration_) * 100);
      progressBarGdi_.Value = progresstime;

      if (LcdDeviceType.Qvga == device_.DeviceType)
      {
        artworkGdi_.Image = Conversions.Base64ToImage(artwork);
        albumGdi_.Text = album;

        float tempRating = rating;

        for (int i = 0; i < 5; i++)
        {
          if (rating != 0)
          {
            if (tempRating - 1 >= 0)
            {
              ratingColorGdi_[i].Image = fullStarImage_;
              tempRating--;
            }

            else if (tempRating - 0.5f == 0)
            {
              ratingColorGdi_[i].Image = halfStarImage_;
              tempRating -= 0.5f;
            }

            else
            {
              ratingColorGdi_[i].Image = emptyStarImage_;
            }
          }
          else
          {
            ratingColorGdi_[i].Image = emptyImage_;
          }

        }
      }
      else if (device_.DeviceType == LcdDeviceType.Monochrome)
      {
        if (rating != 0)
        {
          ratingGdi_.Text = rating.ToString() + " / 5";
        }
        else
        {
          ratingGdi_.Text = "";
        }
      }
    }
  }
}
