using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a <see cref="LcdPage"/> that use GDI+ to do its drawing.
	/// </summary>
	public class LcdGdiPage : LcdPage {
		private readonly List<LcdGdiObject> _children = new List<LcdGdiObject>();
		private readonly Bitmap _bitmap;
		private readonly Rectangle _rectangle;
		private readonly byte[] _32BppPixels;
		private readonly byte[] _8BppPixels;
		private Graphics _graphics;

		/// <summary>
		/// Gets the <see cref="Bitmap"/> used to draw this page.
		/// </summary>
		public Bitmap Bitmap {
			get { return _bitmap; }
		}

		/// <summary>
		/// Gets a list of <see cref="LcdGdiObject"/>s that are the children of this page.
		/// </summary>
		public List<LcdGdiObject> Children {
			get { return _children; }
		}

		/// <summary>
		/// Occurs just after the page have been cleared of all contents, but before the children are drawn.
		/// This event provides a <see cref="Graphics"/> object to use for custom drawing.
		/// </summary>
		public event EventHandler<GdiDrawingEventArgs> GdiDrawing;

		/// <summary>
		/// Raises the <see cref="GdiDrawing"/> event.
		/// </summary>
		/// <param name="graphics"><see cref="Graphics"/> to use for custom drawing.</param>
		protected void OnGdiDrawing(Graphics graphics) {
			EventHandler<GdiDrawingEventArgs> handler = GdiDrawing;
			if (handler != null)
				handler(this, new GdiDrawingEventArgs(graphics));
		}


		/// <summary>
		/// Prepares a <see cref="Graphics"/> object for drawing.
		/// </summary>
		/// <param name="graphics"><see cref="Graphics"/> to prepare for drawing.</param>
		protected static void PrepareGraphics(Graphics graphics) {
			if (graphics == null)
				throw new ArgumentNullException("graphics");
			graphics.PageUnit = GraphicsUnit.Pixel;
			graphics.PageScale = 1.0f;
		}

		/// <summary>
		/// Prepares a <see cref="Graphics"/> object for drawing a specified <see cref="LcdGdiObject"/>,
		/// by setting the appropriate properties (clip, interpolation, etc) on the <see cref="Graphics"/>.
		/// </summary>
		/// <param name="graphics"><see cref="Graphics"/> to prepare for drawing <paramref name="child"/>.</param>
		/// <param name="child"><see cref="LcdGdiObject"/> that will be drawn on <paramref name="graphics"/>.</param>
		internal protected static void PrepareGraphicsForChild(Graphics graphics, LcdGdiObject child) {
			if (graphics == null)
				throw new ArgumentNullException("graphics");
			if (child == null)
				throw new ArgumentNullException("child");
			graphics.Transform = child.Transform;
			if (child.Clip != null)
				graphics.Clip = child.Clip;
			graphics.InterpolationMode = child.InterpolationMode;
			graphics.PixelOffsetMode = child.PixelOffsetMode;
			graphics.RenderingOrigin = child.RenderingOrigin;
			graphics.SmoothingMode = child.SmoothingMode;
		}

		/// <summary>
		/// Derived classes override this method in order to update the page content.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <returns><c>true</c> if the update has done something and a redraw is required.</returns>
		protected override bool UpdateCore(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame) {
			Graphics graphics = Graphics.FromImage(_bitmap);
			PrepareGraphics(graphics);
			bool hasChanged = false;
			foreach (LcdGdiObject child in _children) {
				if (child.HasChanged) {
					hasChanged = true;
					PrepareGraphicsForChild(graphics, child);
					child.Update(elapsedTotalTime, elapsedTimeSinceLastFrame, this, graphics);
					child.HasChanged = false;
					graphics.ResetClip();
				}
			}
			if (hasChanged)
				_graphics = graphics;
			else {
				graphics.Dispose();
				_graphics = null;
			}
			return hasChanged;
		}

		/// <summary>
		/// Derived classes override this method in order to draw the page content visually.
		/// </summary>
		/// <returns>Implementors must return a pixel array conforming to
		/// <see cref="LcdPage.Device"/>'s <see cref="LcdDevice.DeviceType"/>.</returns>
		protected override byte[] DrawCore() {
			if (_graphics == null)
				_graphics = Graphics.FromImage(_bitmap);
			using (Graphics graphics = _graphics) {
				PrepareGraphics(graphics);
				graphics.FillRectangle(Brushes.White, _rectangle);
				foreach (LcdGdiObject child in _children) {
					if (child.IsVisible) {
						PrepareGraphicsForChild(graphics, child);
						child.Draw(this, graphics);
						graphics.ResetClip();
					}
				}
				OnGdiDrawing(graphics);
			}
			_graphics = null;
			return PixelsFromBitmap();
		}

		/// <summary>
		/// Copies the pixels from <see cref="Bitmap"/> and returns them.
		/// </summary>
		/// <returns>A copy of the pixels from <see cref="Bitmap"/>.</returns>
		protected byte[] PixelsFromBitmap() {
			BitmapData bitmapData = _bitmap.LockBits(_rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			try {
				Marshal.Copy(bitmapData.Scan0, _32BppPixels, 0, _32BppPixels.Length);

				// 32bpp, simply returns
				if (Device.BitsPerPixel == 32)
					return _32BppPixels;

				// 8bpp, take the mean of each of the 4 8bit color components
				for (int i = 0; i < _8BppPixels.Length; ++i)
					_8BppPixels[i] = (byte) (255 - (_32BppPixels[i * 4] + _32BppPixels[i * 4 + 1] + _32BppPixels[i * 4 + 2] + _32BppPixels[i * 4 + 3]) / 4);
				return _8BppPixels;

			}
			finally {
				_bitmap.UnlockBits(bitmapData);
			}
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiPage"/> on the given device.
		/// </summary>
		/// <param name="device">Device where this page will be shown.</param>
		public LcdGdiPage(LcdDevice device)
			: base(device) {
			if (device.BitsPerPixel != 8 && device.BitsPerPixel != 32)
				throw new NotSupportedException("Only 8bpp and 32bpp devices are supported.");
			_bitmap = new Bitmap(device.PixelWidth, device.PixelHeight, PixelFormat.Format32bppArgb);
			_rectangle = new Rectangle(0, 0, device.PixelWidth, device.PixelHeight);
			_32BppPixels = new byte[device.PixelWidth * device.PixelHeight * 4];
			if (device.BitsPerPixel == 8)
				_8BppPixels = new byte[device.PixelWidth * device.PixelHeight];
		}
	}

}