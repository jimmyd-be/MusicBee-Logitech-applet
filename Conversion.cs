using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicBeePlugin
{
  static class Conversions
  {
    public static string timetoString(int time)
    {
      string minutes = ((int)time / 60).ToString().PadLeft(2, '0');
      string seconds = ((int)time % 60).ToString().PadLeft(2, '0');

      return minutes + ":" + seconds;
    }

    public static Image Base64ToImage(string base64String)
    {
      if (!String.IsNullOrEmpty(base64String))
      {
        // Convert Base64 String to byte[]
        byte[] imageBytes = Convert.FromBase64String(base64String);
        MemoryStream ms = new MemoryStream(imageBytes, 0,
          imageBytes.Length);

        // Convert byte[] to Image
        ms.Write(imageBytes, 0, imageBytes.Length);
        using (Image image = Image.FromStream(ms, true))
        {
            return resizeImage(image, new Size(320, 130));
        }
      }
      else
      {
        return Resource.NoArtwork;
      }
    }

    private static Image resizeImage(Image imgToResize, Size size)
    {
      int sourceWidth = imgToResize.Width;
      int sourceHeight = imgToResize.Height;

      float nPercent = Math.Min(
          (float)size.Width / (float)sourceWidth,
          (float)size.Height / (float)sourceHeight
          );

      int destWidth = (int)(sourceWidth * nPercent);
      int destHeight = (int)(sourceHeight * nPercent);

      Bitmap b = new Bitmap(destWidth, destHeight);
      using (Graphics g = Graphics.FromImage(b))
      {
          g.InterpolationMode = InterpolationMode.HighQualityBicubic;
          g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
      }

      return b;
    }
  }
}
