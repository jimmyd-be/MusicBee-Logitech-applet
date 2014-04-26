using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Base class for objects that support absolute and relative positions.
	/// </summary>
	public abstract class LcdGdiAbsObject : LcdGdiObject {
		private bool _keepAbsolute;
		private PointF[] _points;

		/// <summary>
		/// Gets or sets whether the object points are left untouched.
		/// </summary>
		/// <remarks>
		/// When this property is <c>true</c>, <see cref="LcdGdiObject.Margin"/>,
		/// <see cref="LcdGdiObject.HorizontalAlignment"/> and <see cref="LcdGdiObject.VerticalAlignment"/>
		/// are unused and <see cref="LcdGdiObject.AbsolutePosition"/> is always (0,0).
		/// This means that the object is always placed absolutely on the screen.
		/// When this property is <c>false</c>, margin and alignments are used, and every space in the X or Y
		/// coordinate is transformed into a margin. This means that the points will always be relative to
		/// <see cref="LcdGdiObject.Margin"/>, like every other object does.
		/// </remarks>
		public bool KeepAbsolute {
			get { return _keepAbsolute; }
			set { CalcAndSetPoints(_points, value); }
		}

		/// <summary>
		/// Gets the number of points used to draw this object.
		/// </summary>
		public int TotalPoints {
			get { return _points.Length; }
		}

		/// <summary>
		/// Gets a point at a specified index.
		/// </summary>
		/// <param name="index">Index of the point.</param>
		/// <returns>Point at <paramref name="index"/>.</returns>
		public PointF GetPoint(int index) {
			return _points[index];
		}

		/// <summary>
		/// Gets a copy of the points used to draw this object.
		/// </summary>
		/// <returns>A copy of the points used to draw this object.</returns>
		public PointF[] GetPoints() {
			return (PointF[]) _points.Clone();
		}

		/// <summary>
		/// Calculates the size of the objects from the points, computes margin and makes points relative if needed.
		/// </summary>
		/// <param name="points">Points defining the object.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		protected void CalcAndSetPoints(PointF[] points, bool keepAbsolute) {
			if (points == null)
				throw new ArgumentNullException("points");
			if (points.Length == 0)
				throw new ArgumentOutOfRangeException("points", "There must be at least 1 point in the points array.");
			PointF firstPoint = points[0];
			float minX = firstPoint.X;
			float minY = firstPoint.Y;
			float maxX = firstPoint.X;
			float maxY = firstPoint.Y;
			for (int i = 1; i < points.Length; ++i) {
				PointF point = points[i];
				minX = Math.Min(minX, point.X);
				minY = Math.Min(minY, point.Y);
				maxX = Math.Max(maxX, point.X);
				maxY = Math.Max(maxY, point.Y);
			}
			if (!keepAbsolute) {
				Margin = new MarginF(minX, minY, 0.0f, 0.0f);
				maxX -= minX;
				maxY -= minY;
				for (int i = 0; i < points.Length; ++i) {
					PointF point = points[i];
					point.X -= minX;
					point.Y -= minY;
					points[i] = point;
				}
			}
			_points = points;
			_keepAbsolute = keepAbsolute;
			Size = new SizeF(maxX, maxY);
			HasChanged = true;
		}

		/// <summary>
		/// Updates the position of the object.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Update(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame, LcdGdiPage page, Graphics graphics) {
			FinalSize = Size;
			if (KeepAbsolute)
				AbsolutePosition = new PointF();
			else
				CalcAbsolutePosition(page.Bitmap.Size);
		}

		/// <summary>
		/// Checks if the alignments are set to stretch, throw if they are.
		/// </summary>
		protected override void OnChanged() {
			base.OnChanged();
			if (HorizontalAlignment == LcdGdiHorizontalAlignment.Stretch || VerticalAlignment == LcdGdiVerticalAlignment.Stretch)
				throw new ArgumentException("Stretch alignment cannot be used on a " + GetType().Name);
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiAbsObject"/> with the specified points.
		/// </summary>
		/// <param name="points">Points defining the object.</param>
		/// <param name="keepAbsolute">Whether the given points are left untouched.
		/// <see cref="LcdGdiAbsObject.KeepAbsolute"/> for details.</param>
		protected LcdGdiAbsObject(PointF[] points, bool keepAbsolute) {
			CalcAndSetPoints(points, keepAbsolute);
		}
	}

}