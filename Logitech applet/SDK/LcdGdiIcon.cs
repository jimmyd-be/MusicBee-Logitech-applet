using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents an icon on a <see cref="LcdGdiPage" />.
	/// Contrary to an image, an icon is always pixel-aligned.
	/// </summary>
	public class LcdGdiIcon : LcdGdiObject {
		private Icon _icon;

		/// <summary>
		/// Gets or sets the icon drawn by this object.
		/// </summary>
		public Icon Icon {
			get { return _icon; }
			set {
				if (_icon != value) {
					_icon = value;
					Size = value != null ? _icon.Size : SizeF.Empty;
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
			AbsolutePosition = new PointF((float) Math.Truncate(AbsolutePosition.X), (float) Math.Truncate(AbsolutePosition.Y));
		}

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (_icon != null)
				graphics.DrawIcon(_icon, new Rectangle((int) AbsolutePosition.X, (int) AbsolutePosition.Y, (int) FinalSize.Width, (int) FinalSize.Height));
		}


		/// <summary>
		/// Releases the resources associated with this <see cref="LcdGdiObject"/>.
		/// </summary>
		/// <param name="disposing">Whether to also release managed resources along with unmanaged ones.</param>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing && _icon != null)
				_icon.Dispose();
		}


		/// <summary>
		/// Creates a new <see cref="LcdGdiIcon"/>.
		/// </summary>
		public LcdGdiIcon() {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiIcon"/> with the specified icon.
		/// </summary>
		/// <param name="icon">Icon drawn by this object.</param>
		public LcdGdiIcon(Icon icon) {
			Icon = icon;
		}
	}

}