using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a polygon on a <see cref="LcdGdiPage"/>.
	/// </summary>
	public class LcdGdiPolygon : LcdGdiAbsObject {
		private FillMode _fillMode;

		/// <summary>
		/// Gets or sets a member of the <see cref="FillMode"/> enumeration that determines how the polygon is filled.
		/// </summary>
		public FillMode FillMode {
			get { return _fillMode; }
			set {
				if (_fillMode != value) {
					_fillMode = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Changes the points of the polygon.
		/// </summary>
		/// <param name="points">Points delimiting the polygon.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public void SetPoints(PointF[] points, bool keepAbsolute) {
			if (points == null)
				throw new ArgumentNullException("points");
			if (points.Length < 3)
				throw new ArgumentOutOfRangeException("points", "There must be at least 3 points to make a polygon.");
			CalcAndSetPoints(points, keepAbsolute);
		}


		/// <summary>
		/// Draws the polygon.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			PointF[] points = GetPoints();
			for (int i = 0; i < points.Length; ++i) {
				PointF point = points[i];
				point.X += AbsolutePosition.X;
				point.Y += AbsolutePosition.Y;
				points[i] = point;
			}
			if (Brush != null)
				graphics.FillPolygon(Brush, points, _fillMode);
			if (Pen != null)
				graphics.DrawPolygon(Pen, points);
		}


		/// <summary>
		/// Creates a new <see cref="LcdGdiPolygon"/>.
		/// </summary>
		public LcdGdiPolygon()
			: base(new PointF[3], false) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiPolygon"/> with the specified pen for the edge and points.
		/// </summary>
		/// <param name="pen">Pen used to draw the edge of the polygon.</param>
		/// <param name="points">Points delimiting the polygon.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiPolygon(Pen pen, PointF[] points, bool keepAbsolute)
			: this(pen, null, points, keepAbsolute) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiPolygon"/> with the specified brush for the fill and points.
		/// </summary>
		/// <param name="brush">Brush used to fill the polygon.</param>
		/// <param name="points">Points delimiting the polygon.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiPolygon(Brush brush, PointF[] points, bool keepAbsolute)
			: this(null, brush, points, keepAbsolute) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiPolygon"/> with the specified pen for the edge,
		/// brush for the fill and points.
		/// </summary>
		/// <param name="pen">Pen used to draw the edge of the polygon.</param>
		/// <param name="brush">Brush used to fill the polygon.</param>
		/// <param name="points">Points delimiting the polygon.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiPolygon(Pen pen, Brush brush, PointF[] points, bool keepAbsolute)
			: base(points, keepAbsolute) {
			if (points.Length < 3)
				throw new ArgumentOutOfRangeException("points", "There must be at least 3 points to make a curve.");
			Pen = pen;
			Brush = brush;
		}
	}

}