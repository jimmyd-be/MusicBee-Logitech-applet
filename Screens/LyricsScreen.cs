using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Screens
{
  class LyricsScreen : Screen
  {
    public struct LyricsText
    {
      public int time;
      public string text;
    }

    private List<LyricsText> lyrics_ = null;

    private bool synchronized_ = false;

    private LcdGdiText mainTextGdi_ = null;
    private LcdGdiText secondTextGdi_ = null;
    private LcdGdiText thirdTextGdi_ = null;

    protected  Font mainTextFont_ = new Font("Arial", 10);

    private int maximumTextSize_ = 0;
    private int lyricsPosition_ = 0;

    public LyricsScreen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin plugin, int index)
      : base(device, type, backgroundGdi, plugin, index)
    {
      screenName_ = "LyricsScreen";

      plugin_.getSongData();

      if (type == LcdDeviceType.Monochrome)
      {
        maximumTextSize_ = 40;
        createMono();
      }
      else if (type == LcdDeviceType.Qvga)
      {
        mainTextFont_ = new Font(mainTextFont_, FontStyle.Bold);
        maximumTextSize_ = 55;
        createColor();
      }

    }

    private void createMono()
    {
      mainTextGdi_ = new LcdGdiText("", font2_);
      mainTextGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      mainTextGdi_.Margin = new MarginF(-2, -1, 0, 0);

      this.Children.Add(mainTextGdi_);
    }

    private void createColor()
    {
      mainTextGdi_ = new LcdGdiText("", mainTextFont_);
      mainTextGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      mainTextGdi_.Margin = new MarginF(5, 5, 5, 0);

      secondTextGdi_ = new LcdGdiText("", font4_);
      secondTextGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      secondTextGdi_.Margin = new MarginF(5, 60, 5, 0);

      thirdTextGdi_ = new LcdGdiText("", font4_);
      thirdTextGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      thirdTextGdi_.Margin = new MarginF(5, 100, 5, 0);

      this.Children.Add(mainTextGdi_);
      this.Children.Add(secondTextGdi_);
      this.Children.Add(thirdTextGdi_);
    }

    public override void buttonPressedMonochrome(object sender, LcdSoftButtonsEventArgs e)
    {
      // First button is pressed, switch to page one
      if ((e.SoftButtons & LcdSoftButtons.Button0) == LcdSoftButtons.Button0)
      {
        plugin_.goToPreviousPage();
      }

      // Second button is pressed
      else if (((e.SoftButtons & LcdSoftButtons.Button1) == LcdSoftButtons.Button1) && !synchronized_)
      {
        if (lyricsPosition_ > 0)
        {
          lyricsPosition_--;
        }
      }

      // Third button is pressed
      else if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button2 && !synchronized_)
      {
        if (lyricsPosition_ < lyrics_.Count)
        {
          lyricsPosition_++;
        }
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
      else if ((e.SoftButtons & LcdSoftButtons.Up) == LcdSoftButtons.Up && !synchronized_)
      {
        if (lyricsPosition_ > 0)
        {
          lyricsPosition_--;
        }
      }

      //G19 down button pressed
      else if ((e.SoftButtons & LcdSoftButtons.Down) == LcdSoftButtons.Down && !synchronized_)
      {
        if (lyricsPosition_ < lyrics_.Count)
        {
          lyricsPosition_++;
        }
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
      int textLine = searchLine(position);

      if (lyrics_ != null && (textLine < lyrics_.Count) && (lyrics_.Count > -1))
      {
        mainTextGdi_.Text = WordWrap(lyrics_[textLine].text.Replace("\r\n", "\n").Replace("\r", "\n"), maximumTextSize_);

        if (device_.DeviceType == LcdDeviceType.Qvga)
        {
          secondTextGdi_.Text = WordWrap(lyrics_[textLine + 1].text.Replace("\r\n", "\n").Replace("\r", "\n"), maximumTextSize_);
          thirdTextGdi_.Text = WordWrap(lyrics_[textLine + 2].text.Replace("\r\n", "\n").Replace("\r", "\n"), maximumTextSize_);
        }
      }
    }

    public override void songChanged(string artist, string album, string title, float rating, string artwork, int duration, int position, string lyrics)
    {
      lyrics_ = null;
      lyricsPosition_ = 0;

      if (lyrics != null && lyrics.Length != 0)
      {
        parseLyrics(lyrics);
      }
      else
      {
        mainTextGdi_.Text = "No lyrics found";
      }
    }

    public static string WordWrap(string text, int width)
    {
      int pos, next;
      StringBuilder sb = new StringBuilder();

      // Lucidity check
      if (width < 1)
        return text;

      // Parse each line of text
      for (pos = 0; pos < text.Length; pos = next)
      {
        // Find end of line
        int eol = text.IndexOf(Environment.NewLine, pos);
        if (eol == -1)
          next = eol = text.Length;
        else
          next = eol + Environment.NewLine.Length;

        // Copy this line of text, breaking into smaller lines as needed
        if (eol > pos)
        {
          do
          {
            int len = eol - pos;
            if (len > width)
              len = BreakLine(text, pos, width);
            sb.Append(text, pos, len);
            sb.Append(Environment.NewLine);

            // Trim whitespace following break
            pos += len;
            while (pos < eol && Char.IsWhiteSpace(text[pos]))
              pos++;
          } while (eol > pos);
        }
        else sb.Append(Environment.NewLine); // Empty line
      }
      return sb.ToString();
    }

    private static int BreakLine(string text, int pos, int max)
    {
      // Find last whitespace in line
      int i = max;
      while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
        i--;

      // If no whitespace found, break at maximum length
      if (i < 0)
        return max;

      // Find start of whitespace
      while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
        i--;

      // Return length of text before whitespace
      return i + 1;
    }

    private int searchLine(int position)
    {
      if (synchronized_)
      {
        for (int i = lyricsPosition_; i < lyrics_.Count - 1; i++)
        {
          if (lyrics_[i + 1].time >= (float)position)
          {
            lyricsPosition_ = i;
            return i;
          }
        }
      } 

      return lyricsPosition_;
    }

    private void parseLyrics(string lyrics)
    {
      lyrics_ = new List<LyricsText>();

      lyrics = lyrics.Replace("\r\n", "\n").Replace("\r", "\n");

      int position = lyrics.IndexOf("\n");
      while (position != -1) {
        LyricsText textObject = new LyricsText();
        string temp = lyrics.Substring(0, position);
        lyrics = lyrics.Remove(0, position + 1);

        int timepos = temp.IndexOf("[");
        int timepos2 = temp.IndexOf("]");

        if (timepos != -1 && timepos2 != -1) {
          int timeInt = 0;
          string time = temp.Substring(timepos+1, timepos2 - timepos - 1);

          int doublePointPos =  time.IndexOf(":");
          timeInt += Convert.ToInt32(time.Substring(0, doublePointPos))* 60000;
          time = time.Remove(0, doublePointPos + 1);

          doublePointPos = time.IndexOf(".");
          if (doublePointPos != -1) {
            timeInt += Convert.ToInt32(time.Substring(0, doublePointPos)) * 1000;
            time = time.Remove(0, doublePointPos + 1);
          }

          timeInt += Convert.ToInt32(time);

          textObject.text = temp.Remove(0, timepos2 + 1);
          textObject.time = timeInt;

          synchronized_ = true;
        } else {
          synchronized_ = false;

          textObject.text = temp;
          textObject.time = 0;
        }

        if (textObject.text != "" && textObject.text != null) {
          lyrics_.Add(textObject);
        }

        position = lyrics.IndexOf("\n");
      }
    }

    private int timeToMs(string lrc_t)
    {
      float m, s, ms;
      string[] lrc_t_1 = lrc_t.Split(':');

      try
      {
        m = float.Parse(lrc_t_1[0]);
      }
      catch
      {
        return 0;
      }
      float.TryParse(lrc_t_1[1].Replace(".", ","), out s);
      ms = m * 60000 + s * 1000;
      return (int)ms;
    }
  }
}
