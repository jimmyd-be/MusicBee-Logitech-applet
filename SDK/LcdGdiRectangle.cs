using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a simple rectangle on a <see cref="LcdGdiPage"/>.
	/// </summary>
	public class LcdGdiRectangle : LcdGdiObject {

		/// <summary>
		/// Draws the rectangle.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (Brush != null)
				graphics.FillRectangle(Brush, AbsolutePosition.X, AbsolutePosition.Y, FinalSize.Width - 1.0f, FinalSize.Height - 1.0f);
			if (Pen != null)
				graphics.DrawRectangle(Pen, AbsolutePosition.X, AbsolutePosition.Y, FinalSize.Width - 1.0f, FinalSize.Height - 1.0f);
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiRectangle"/>.
		/// </summary>
		public LcdGdiRectangle() {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiRectangle"/> with no edge,
		/// and specified brush for the fill and rectangle dimensions.
		/// </summary>
		/// <param name="brush">Brush to use to draw the fill of this object.</param>
		/// <param name="rectangle">Rectangle dimensions.</param>
		public LcdGdiRectangle(Brush brush, RectangleF rectangle)
			: this(null, brush, rectangle) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiRectangle"/> with the specified pen for the edge,
		/// no fill brush and specified rectangle dimensions.
		/// </summary>
		/// <param name="pen">Pen to use to draw the edge of this object.</param>
		/// <param name="rectangle">Rectangle dimensions.</param>
		public LcdGdiRectangle(Pen pen, RectangleF rectangle)
			: this(pen, null, rectangle) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiRectangle"/> with the specified pen for the edge,
		/// brush for the fill and rectangle dimensions.
		/// </summary>
		/// <param name="pen">Pen to use to draw the edge of this object.</param>
		/// <param name="brush">Brush to use to draw the fill of this object.</param>
		/// <param name="rectangle">Rectangle dimensions.</param>
		public LcdGdiRectangle(Pen pen, Brush brush, RectangleF rectangle) {
			Pen = pen;
			Brush = brush;
			Margin = new MarginF(rectangle.Location.X, rectangle.Location.Y, 0.0f, 0.0f);
			Size = rectangle.Size;
		}
	}

}