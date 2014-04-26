using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a Bézier curve on a <see cref="LcdGdiPage"/>.
	/// </summary>
	public class LcdGdiBezier : LcdGdiAbsObject {
		
		/// <summary>
		/// Gets or sets the starting point of the Bézier curve.
		/// </summary>
		public PointF StartPoint {
			get { return GetPoint(0); }
			set { SetPoints(value, ControlPoint1, ControlPoint2, EndPoint, KeepAbsolute); }
		}

		/// <summary>
		/// Gets or sets the first control point of the Bézier curve.
		/// </summary>
		public PointF ControlPoint1 {
			get { return GetPoint(1); }
			set { SetPoints(StartPoint, value, ControlPoint2, EndPoint, KeepAbsolute); }
		}

		/// <summary>
		/// Gets or sets the second control point of the Bézier curve.
		/// </summary>
		public PointF ControlPoint2 {
			get { return GetPoint(2); }
			set { SetPoints(StartPoint, ControlPoint1, value, EndPoint, KeepAbsolute); }
		}

		/// <summary>
		/// Gets or sets the end point of the Bézier curve.
		/// </summary>
		public PointF EndPoint {
			get { return GetPoint(3); }
			set { SetPoints(StartPoint, ControlPoint1, ControlPoint2, value, KeepAbsolute); }
		}

		/// <summary>
		/// Changes the four points of the Bézier curve at the same time.
		/// </summary>
		/// <param name="startPoint">Starting point of the Bézier curve</param>
		/// <param name="controlPoint1">First control point of the Bézier curve.</param>
		/// <param name="controlPoint2">Second control point of the Bézier curve.</param>
		/// <param name="endPoint">Ending point of the Bézier curve.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public void SetPoints(PointF startPoint, PointF controlPoint1, PointF controlPoint2, PointF endPoint, bool keepAbsolute) {
			CalcAndSetPoints(new[] { startPoint, controlPoint1, controlPoint2, endPoint }, keepAbsolute);
		}

		/// <summary>
		/// Draws the curve.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			PointF[] points = GetPoints();
			if (Pen != null) {
				graphics.DrawBezier(Pen,
				                    AbsolutePosition.X + points[0].X, AbsolutePosition.Y + points[0].Y,
				                    AbsolutePosition.X + points[1].X, AbsolutePosition.Y + points[1].Y,
				                    AbsolutePosition.X + points[2].X, AbsolutePosition.Y + points[2].Y,
				                    AbsolutePosition.X + points[3].X, AbsolutePosition.Y + points[3].Y);
			}
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiBezier"/>.
		/// </summary>
		public LcdGdiBezier()
			: base(new PointF[4], false) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiBezier"/> with the specified pen and points.
		/// </summary>
		/// <param name="pen">Pen to use to draw this Bézier curve.</param>
		/// <param name="startPoint">Starting point of the Bézier curve</param>
		/// <param name="controlPoint1">First control point of the Bézier curve.</param>
		/// <param name="controlPoint2">Second control point of the Bézier curve.</param>
		/// <param name="endPoint">Ending point of the Bézier curve.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiBezier(Pen pen, PointF startPoint, PointF controlPoint1, PointF controlPoint2, PointF endPoint, bool keepAbsolute)
			: base(new[] { startPoint, controlPoint1, controlPoint2, endPoint }, keepAbsolute) {
			Pen = pen;
		}
	}

}