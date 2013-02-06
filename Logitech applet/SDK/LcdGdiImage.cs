using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents an image on a <see cref="LcdGdiPage" />.
	/// </summary>
	public class LcdGdiImage : LcdGdiObject {
		private Image _image;
		private bool _alignOnPixels = true;

		/// <summary>
		/// Gets or sets the image drawn by this object.
		/// </summary>
		public Image Image {
			get { return _image; }
			set {
				if (_image != value) {
					_image = value;
					if (value != null)
						Size = _image.Size;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the image will be kept aligned on full pixels
		/// to avoid blurry results. The default is <c>true</c>.
		/// </summary>
		public bool AlignOnPixels {
			get { return _alignOnPixels; }
			set {
				if (_alignOnPixels != value) {
					_alignOnPixels = value;
					HasChanged = true;
				}
			}
		}


		/// <summary>
		/// Updates the position of the object.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Update(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame, LcdGdiPage page, Graphics graphics) {
			base.Update(elapsedTotalTime, elapsedTimeSinceLastFrame, page, graphics);
			if (_alignOnPixels)
				AbsolutePosition = new PointF((float) Math.Truncate(AbsolutePosition.X), (float) Math.Truncate(AbsolutePosition.Y));
		}

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (_image != null)
				graphics.DrawImage(_image, AbsolutePosition.X, AbsolutePosition.Y, FinalSize.Width, FinalSize.Height);
		}


		/// <summary>
		/// Releases the resources associated with this <see cref="LcdGdiObject"/>.
		/// </summary>
		/// <param name="disposing">Whether to also release managed resources along with unmanaged ones.</param>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing && _image != null)
				_image.Dispose();
		}


		/// <summary>
		/// Creates a new <see cref="LcdGdiImage"/>.
		/// </summary>
		public LcdGdiImage() {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiImage"/> with the specified image and pixel alignment enabled.
		/// </summary>
		/// <param name="image">Image drawn by this object.</param>
		public LcdGdiImage(Image image)
			: this(image, true) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiImage"/> with the specified image and pixel alignment.
		/// </summary>
		/// <param name="image">Image drawn by this object.</param>
		/// <param name="alignOnPixels">Whether the image will be kept aligned on full pixels to avoid blurry results.</param>
		public LcdGdiImage(Image image, bool alignOnPixels) {
			Image = image;
			_alignOnPixels = alignOnPixels;
		}
	}

}