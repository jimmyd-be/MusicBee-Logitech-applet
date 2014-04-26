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
      string minutes = ((int)time / 60).ToString();
      string seconds = ((int)time % 60).ToString();

      if (minutes.Length < 2)
      {
        minutes = "0" + minutes;
      }

      if (seconds.Length < 2)
      {
        seconds = "0" + seconds;
      }

      return minutes + ":" + seconds;
    }

    public static Image Base64ToImage(string base64String)
    {
      Image image = null;
      if (base64String != "" && base64String != null)
      {
        // Convert Base64 String to byte[]
        byte[] imageBytes = Convert.FromBase64String(base64String);
        MemoryStream ms = new MemoryStream(imageBytes, 0,
          imageBytes.Length);

        // Convert byte[] to Image
        ms.Write(imageBytes, 0, imageBytes.Length);
        image = Image.FromStream(ms, true);

        image = resizeImage(image, new Size(320, 130));
      }
      else
      {
        image = Resource.NoArtwork;
      }
      return image;
    }

    private static Image resizeImage(Image imgToResize, Size size)
    {
      int sourceWidth = imgToResize.Width;
      int sourceHeight = imgToResize.Height;

      float nPercent = 0;
      float nPercentW = 0;
      float nPercentH = 0;

      nPercentW = ((float)size.Width / (float)sourceWidth);
      nPercentH = ((float)size.Height / (float)sourceHeight);

      if (nPercentH < nPercentW)
        nPercent = nPercentH;
      else
        nPercent = nPercentW;

      int destWidth = (int)(sourceWidth * nPercent);
      int destHeight = (int)(sourceHeight * nPercent);

      Bitmap b = new Bitmap(destWidth, destHeight);
      Graphics g = Graphics.FromImage((Image)b);
      g.InterpolationMode = InterpolationMode.HighQualityBicubic;

      g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
      g.Dispose();

      return (Image)b;
    }
  }
}
