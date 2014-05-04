using GammaJul.LgLcd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    private LyricsText[] lyrics_;

    private bool synchronized_ = false;

    private LcdGdiText mainTextGdi_ = null;
    private LcdGdiText secondTextGdi_ = null;

    private LcdGdiScrollViewer secondTextScroll_ = null;

    private int lyricsPosition_ = 0;

    public LyricsScreen(LcdDevice device, LcdDeviceType type, string backgroundGdi, Plugin plugin, int index)
      : base(device, type, backgroundGdi, plugin, index)
    {
      screenName_ = "LyricsScreen";

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
      mainTextGdi_ = new LcdGdiText("", font_);
      mainTextGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      mainTextGdi_.Margin = new MarginF(-2, -1, 0, 0);

      secondTextGdi_ = new LcdGdiText("", font2_);
      secondTextGdi_.HorizontalAlignment = LcdGdiHorizontalAlignment.Center;
      secondTextGdi_.Margin = new MarginF(0, 30, 0, 0);

      secondTextScroll_ = new LcdGdiScrollViewer(secondTextGdi_);
      secondTextScroll_.AutoScrollX = true;
      secondTextScroll_.AutoScrollY = false;
      secondTextScroll_.AutoScrollSpeedX = 5;
      secondTextScroll_.AutoScrollSpeedY = 0;
      secondTextScroll_.HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;

      this.Children.Add(mainTextGdi_);
      this.Children.Add(secondTextGdi_);
      this.Children.Add(secondTextScroll_);
    }

    private void createColor()
    {

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

      }

      // Third button is pressed
      else if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button2 && !synchronized_)
      {

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

      }

      //G19 down button pressed
      else if ((e.SoftButtons & LcdSoftButtons.Down) == LcdSoftButtons.Down && !synchronized_)
      {

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
      mainTextGdi_.Text = WordWrap(lyrics_[textLine].text.Replace("\r\n", "\n").Replace("\r", "\n"), 30);
      secondTextGdi_.Text = lyrics_[textLine + 1].text.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    public override void songChanged(string artist, string album, string title, float rating, string artwork, int duration, int position, string lyrics)
    {
      parseLyrics(lyrics);
      lyricsPosition_ = 0;
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
      for (int i = lyricsPosition_; i < lyrics_.Length-1; i++ )
      {
        if (lyrics_[i+1].time >= (float)position)
        {
          lyricsPosition_ = i;
          return i;
        }
      }
      return lyricsPosition_;
    }

    private void parseLyrics(string lyrics)
    {
      if (lyrics != null && lyrics.Length != 0)
      {
        string[] lrc_1 = lyrics.Split(new char[] { '[', ']' });

        format_1(lrc_1);
        format_2(lrc_1);
        format_3();
      }
    }

    private void format_1(string[] lrc_1)
    {
      for (int i = 2, j = 0; i < lrc_1.Length; i += 2, j = i)
      {
        while (lrc_1[j] == string.Empty)
        {
          lrc_1[i] = lrc_1[j += 2];
        }
      }
    }

    private void format_2(string[] lrc_1)
    {
      lyrics_ = new LyricsText[lrc_1.Length / 2];
      for (int i = 1, j = 0; i < lrc_1.Length; i++, j++)
      {
        lyrics_[j].time = timeToMs(lrc_1[i]);
        lyrics_[j].text = lrc_1[++i];
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

    private void format_3()
    {
      LyricsText tlrc_temp;
      bool b = true;
      for (int i = 0; i < lyrics_.Length - 1; i++, b = true)
      {
        for (int j = 0; j < lyrics_.Length - i - 1; j++)
        {
          if (lyrics_[j].time > lyrics_[j + 1].time)
          {
            tlrc_temp = lyrics_[j];
            lyrics_[j] = lyrics_[j + 1];
            lyrics_[j + 1] = tlrc_temp;
            b = false;
          }
        }
        if (b) break;
      }
    }
  }
}
