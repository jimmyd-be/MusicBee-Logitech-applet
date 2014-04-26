using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a cardinal spline on a <see cref="LcdGdiPage"/>.
	/// </summary>
	public class LcdGdiCurve : LcdGdiAbsObject {
		private bool _isClosed;
		private FillMode _fillMode;
		private float _tension = 0.5f;

		/// <summary>
		/// Gets or sets whether the curve is closed.
		/// </summary>
		public bool IsClosed {
			get { return _isClosed; }
			set {
				if (_isClosed != value) {
					_isClosed = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets a member of the <see cref="FillMode"/> enumeration that determines how the curve is filled.
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
		/// Gets or sets a value greater than or equal to 0.0f that specifies the tension of the curve.
		/// The default is 0.5f.
		/// </summary>
		public float Tension {
			get { return _tension; }
			set {
				if (value < 0.0f || Single.IsNaN(value) || Single.IsInfinity(value))
					throw new ArgumentOutOfRangeException("value", "The tension must be greater than or equal to 0.0f.");
				if (_tension != value) {
					_tension = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Changes the points of the curve.
		/// </summary>
		/// <param name="points">Points delimiting the curve.</param>
		/// <param name="isClosed">Whether the curve is closed.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public void SetPoints(PointF[] points, bool isClosed, bool keepAbsolute) {
			if (points == null)
				throw new ArgumentNullException("points");
			if (points.Length < 3)
				throw new ArgumentOutOfRangeException("points", "There must be at least 3 points to make a curve.");
			_isClosed = isClosed;
			CalcAndSetPoints(points, keepAbsolute);
		}


		/// <summary>
		/// Draws the cardinal spline.
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
			if (_isClosed && Brush != null)
				graphics.FillClosedCurve(Brush, points, _fillMode, _tension);
			if (Pen != null) {
				if (_isClosed)
					graphics.DrawClosedCurve(Pen, points, _tension, _fillMode);
				else
					graphics.DrawCurve(Pen, points, _tension);
			}
		}


		/// <summary>
		/// Creates a new <see cref="LcdGdiCurve"/>.
		/// </summary>
		public LcdGdiCurve()
			: base(new PointF[3], false) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiCurve"/> with the specified pen for the edge,
		/// points, and whether the curve is closed.
		/// </summary>
		/// <param name="pen">Pen used to draw the edge of the curve.</param>
		/// <param name="points">Points delimiting the curve.</param>
		/// <param name="isClosed">Whether the curve is closed.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiCurve(Pen pen, PointF[] points, bool isClosed, bool keepAbsolute)
			: this(pen, points, isClosed, 0.5f, keepAbsolute) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiCurve"/> with the specified pen for the edge,
		/// points, tension, and whether the curve is closed.
		/// </summary>
		/// <param name="pen">Pen used to draw the edge of the curve.</param>
		/// <param name="points">Points delimiting the curve.</param>
		/// <param name="isClosed">Whether the curve is closed.</param>
		/// <param name="tension">A value greater than or equal to 0.0f that specifies the tension of the curve.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiCurve(Pen pen, PointF[] points, bool isClosed, float tension, bool keepAbsolute)
			: this(pen, null, points, tension, keepAbsolute) {
			_isClosed = isClosed;
		}

		/// <summary>
		/// Creates a new closed <see cref="LcdGdiCurve"/> with the specified pen for the edge,
		/// brush for the fill, and points.
		/// </summary>
		/// <param name="pen">Pen used to draw the edge of the curve.</param>
		/// <param name="brush">Brush used to fill the closed curve.</param>
		/// <param name="points">Points delimiting the curve.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiCurve(Pen pen, Brush brush, PointF[] points, bool keepAbsolute)
			: this(pen, brush, points, 0.5f, keepAbsolute) {
		}

		/// <summary>
		/// Creates a new closed <see cref="LcdGdiCurve"/> with the specified brush for the fill
		/// and points.
		/// </summary>
		/// <param name="brush">Brush used to fill the closed curve.</param>
		/// <param name="points">Points delimiting the curve.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiCurve(Brush brush, PointF[] points, bool keepAbsolute)
			: this(brush, points, 0.5f, keepAbsolute) {
		}

		/// <summary>
		/// Creates a new closed <see cref="LcdGdiCurve"/> with the specified brush for the fill,
		/// points and tension.
		/// </summary>
		/// <param name="brush">Brush used to fill the closed curve.</param>
		/// <param name="points">Points delimiting the curve.</param>
		/// <param name="tension">A value greater than or equal to 0.0f that specifies the tension of the curve.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiCurve(Brush brush, PointF[] points, float tension, bool keepAbsolute)
			: this(null, brush, points, tension, keepAbsolute) {
		}

		/// <summary>
		/// Creates a new closed <see cref="LcdGdiCurve"/> with the specified pen for the edge,
		/// brush for the fill, points and tension.
		/// </summary>
		/// <param name="pen">Pen used to draw the edge of the curve.</param>
		/// <param name="brush">Brush used to fill the closed curve.</param>
		/// <param name="points">Points delimiting the curve.</param>
		/// <param name="tension">A value greater than or equal to 0.0f that specifies the tension of the curve.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		public LcdGdiCurve(Pen pen, Brush brush, PointF[] points, float tension, bool keepAbsolute)
			: base(points, keepAbsolute) {
			if (points.Length < 3)
				throw new ArgumentOutOfRangeException("points", "There must be at least 3 points to make a curve.");
			if (tension < 0.0f || Single.IsNaN(tension) || Single.IsInfinity(tension))
				throw new ArgumentOutOfRangeException("tension", "The tension must be greater than or equal to 0.0f.");
			Pen = pen;
			Brush = brush;
			_tension = tension;
			_isClosed = true;
		}
	}

}