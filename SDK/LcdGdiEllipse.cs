using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a simple ellipse on a <see cref="LcdGdiPage"/>.
	/// </summary>
	public class LcdGdiEllipse : LcdGdiObject {

		/// <summary>
		/// Draws the ellipse.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (Brush != null)
				graphics.FillEllipse(Brush, AbsolutePosition.X, AbsolutePosition.Y, FinalSize.Width - 1.0f, FinalSize.Height - 1.0f);
			if (Pen != null)
				graphics.DrawEllipse(Pen, AbsolutePosition.X, AbsolutePosition.Y, FinalSize.Width - 1.0f, FinalSize.Height - 1.0f);
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiEllipse"/>.
		/// </summary>
		public LcdGdiEllipse() {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiEllipse"/> with no edge,
		/// and specified brush for the fill and rectangle dimensions.
		/// </summary>
		/// <param name="brush">Brush to use to draw the fill of this object.</param>
		/// <param name="rectangle">Rectangle dimensions.</param>
		public LcdGdiEllipse(Brush brush, RectangleF rectangle)
			: this(null, brush, rectangle) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiEllipse"/> with the specified pen for the edge,
		/// no fill brush and specified rectangle dimensions.
		/// </summary>
		/// <param name="pen">Pen to use to draw the edge of this object.</param>
		/// <param name="rectangle">Rectangle dimensions.</param>
		public LcdGdiEllipse(Pen pen, RectangleF rectangle)
			: this(pen, null, rectangle) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiEllipse"/> with the specified pen for the edge,
		/// brush for the fill and rectangle dimensions.
		/// </summary>
		/// <param name="pen">Pen to use to draw the edge of this object.</param>
		/// <param name="brush">Brush to use to draw the fill of this object.</param>
		/// <param name="rectangle">Rectangle dimensions.</param>
		public LcdGdiEllipse(Pen pen, Brush brush, RectangleF rectangle) {
			Pen = pen;
			Brush = brush;
			Margin = new MarginF(rectangle.Location.X, rectangle.Location.Y, 0.0f, 0.0f);
			Size = rectangle.Size;
		}
	}

}