using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a simple rectangle on a <see cref="LcdGdiPage"/>.
	/// </summary>
	public class LcdGdiArc : LcdGdiObject {
		private float _startAngle;
		private float _sweepAngle;

		/// <summary>
		/// Gets or sets the angle in degrees measured clockwise from the x-axis to the starting point of the arc.
		/// </summary>
		public float StartAngle {
			get { return _startAngle; }
			set {
				if (_startAngle != value) {
					_startAngle = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the angle in degrees measured clockwise from <see cref="StartAngle"/> to ending point of the arc. 
		/// </summary>
		public float SweepAngle {
			get { return _sweepAngle; }
			set {
				if (_sweepAngle != value) {
					_sweepAngle = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Draws the rectangle.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (Pen != null)
				graphics.DrawArc(Pen, AbsolutePosition.X, AbsolutePosition.Y, FinalSize.Width - 1.0f, FinalSize.Height - 1.0f, _startAngle, _sweepAngle);
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiArc"/>.
		/// </summary>
		public LcdGdiArc() {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiArc"/> with the specified pen, dimensions, start angle and sweep angle.
		/// </summary>
		/// <param name="pen">Pen to use to draw the edge of this object.</param>
		/// <param name="rectangle">Rectangle dimensions.</param>
		/// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.</param>
		/// <param name="sweepAngle">Angle in degrees measured clockwise from <paramref name="startAngle"/> to ending point of the arc.</param>
		public LcdGdiArc(Pen pen, RectangleF rectangle, float startAngle, float sweepAngle) {
			Pen = pen;
			Margin = new MarginF(rectangle.Location.X, rectangle.Location.Y, 0.0f, 0.0f);
			Size = rectangle.Size;
			_startAngle = startAngle;
			_sweepAngle = sweepAngle;
		}
	}

}