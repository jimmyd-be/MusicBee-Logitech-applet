using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a simple line on a <see cref="LcdGdiPage"/>.
	/// </summary>
	public class LcdGdiLine : LcdGdiObject {
		private bool _isReversedX;
		private bool _isReversedY;

		/// <summary>
		/// Gets the starting point of this line, from absolute position and final size.
		/// </summary>
		protected PointF StartPointForDrawing {
			get {
				return new PointF(
					_isReversedX ? AbsolutePosition.X + FinalSize.Width - 1.0f : AbsolutePosition.X,
					_isReversedY ? AbsolutePosition.Y + FinalSize.Height - 1.0f : AbsolutePosition.Y);
			}
		}

		/// <summary>
		/// Gets the ending point of this line, from absolute position and final size.
		/// </summary>
		protected PointF EndPointForDrawing {
			get {
				return new PointF(
					_isReversedX ? AbsolutePosition.X : AbsolutePosition.X + FinalSize.Width - 1.0f,
					_isReversedY ? AbsolutePosition.Y : AbsolutePosition.Y + FinalSize.Height - 1.0f);
			}
		}

		/// <summary>
		/// Changes both the starting point and the ending point of this line.
		/// </summary>
		/// <param name="startPoint">Starting point of this line.</param>
		/// <param name="endPoint">Ending point of this line.</param>
		public void SetPoints(PointF startPoint, PointF endPoint) {
			Margin = new MarginF(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), 0.0f, 0.0f);
			Size = new SizeF(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
		}

		/// <summary>
		/// Gets or sets the starting point of this line.
		/// </summary>
		public PointF StartPoint {
			get { return new PointF(Margin.Left + (_isReversedX ? -Size.Width : 0.0f), Margin.Top + (_isReversedY ? -Size.Height : 0.0f)); }
			set { SetPoints(value, EndPoint); }
		}

		/// <summary>
		/// Gets or sets the ending point of this line.
		/// </summary>
		public PointF EndPoint {
			get { return new PointF(Margin.Left + (_isReversedX ? 0.0f : Size.Width), Margin.Top + (_isReversedY ? 0.0f : Size.Height)); }
			set { SetPoints(StartPoint, value); }
		}

		/// <summary>
		/// Updates the position of the object.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Update(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame, LcdGdiPage page, Graphics graphics) {
			if (Pen == null) {
				base.Update(elapsedTotalTime, elapsedTimeSinceLastFrame, page, graphics);
				return;
			}
			float finalWidth;
			if (HorizontalAlignment == LcdGdiHorizontalAlignment.Stretch)
				finalWidth = page.Bitmap.Width - Margin.Left - Margin.Right;
			else if (Size.Width == 0)
				finalWidth = Pen.Width;
			else
				finalWidth = Math.Abs(Size.Width);
			float finalHeight;
			if (VerticalAlignment == LcdGdiVerticalAlignment.Stretch)
				finalHeight = page.Bitmap.Height - Margin.Top - Margin.Bottom;
			else if (Size.Height == 0)
				finalHeight = Pen.Width;
			else
				finalHeight = Math.Abs(Size.Height);
			FinalSize = new SizeF(finalWidth, finalHeight);
			CalcAbsolutePosition(page.Bitmap.Size);
		}

		/// <summary>
		/// Draws the rectangle.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (Pen == null)
				return;
			graphics.DrawLine(Pen, StartPointForDrawing, EndPointForDrawing);
		}

		/// <summary>
		/// Raises the <see cref="LcdGdiObject.Changed"/> event.
		/// </summary>
		protected override void OnChanged() {
			_isReversedX = Size.Width < 0.0f;
			_isReversedY = Size.Height < 0.0f;
			base.OnChanged();
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiLine"/>.
		/// </summary>
		public LcdGdiLine() {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiLine"/> with the specified pen, start point and end point.
		/// </summary>
		/// <param name="pen">Pen to use to draw this line.</param>
		/// <param name="startPoint">Starting point of this line.</param>
		/// <param name="endPoint">Ending point of this line.</param>
		public LcdGdiLine(Pen pen, PointF startPoint, PointF endPoint) {
			Pen = pen;
			SetPoints(startPoint, endPoint);
		}
	}

}