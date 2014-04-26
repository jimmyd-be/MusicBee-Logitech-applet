using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a progress bar on a <see cref="LcdGdiPage"/>.
	/// </summary>
	public class LcdGdiProgressBar : LcdGdiObject {
		private Brush _progressBrush;
		private int _minimum;
		private int _value;
		private int _maximum = 100;
		private bool _isVertical;

		/// <summary>
		/// Gets or sets the brush that is used to fill the progress indicator.
		/// <see cref="Brush"/> is used as a background to the bar,
		/// while <see cref="ProgressBrush"/> represents the progress part.
		/// </summary>
		public Brush ProgressBrush {
			get { return _progressBrush; }
			set {
				if (_progressBrush != value) {
					_progressBrush = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the minimum value of this progress bar.
		/// </summary>
		public int Minimum {
			get { return _minimum; }
			set {
				int newValue = Math.Min(value, _maximum);
				if (_minimum != newValue) {
					_minimum = newValue;
					_value = Math.Max(_value, newValue);
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum value of this progress bar.
		/// </summary>
		public int Maximum {
			get { return _maximum; }
			set {
				int newValue = Math.Max(value, _minimum);
				if (_maximum != newValue) {
					_maximum = newValue;
					_value = Math.Min(_value, newValue);
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the current value of this progress bar.
		/// </summary>
		public int Value {
			get { return _value; }
			set {
				int newValue = Math.Max(Math.Min(value, _maximum), _minimum);
				if (_value != newValue) {
					_value = newValue;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this progress bar is vertical.
		/// The default value is <c>false</c>.
		/// </summary>
		public bool IsVertical {
			get { return _isVertical; }
			set {
				if (_isVertical != value) {
					_isVertical = value;
					HasChanged = true;
				}
			}
		}


		/// <summary>
		/// Draws the progress bar.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (Brush != null)
				graphics.FillRectangle(Brush, AbsolutePosition.X, AbsolutePosition.Y, FinalSize.Width - 1.0f, FinalSize.Height - 1.0f);
			float penWidth;
			if (Pen != null) {
				graphics.DrawRectangle(Pen, AbsolutePosition.X, AbsolutePosition.Y, FinalSize.Width - 1.0f, FinalSize.Height - 1.0f);
				penWidth = Pen.Width;
			}
			else
				penWidth = 0.0f;
			if (_progressBrush != null) {
				int zeroBasedMaximum = _maximum - _minimum;
				int zeroBasedValue = _value - _minimum;
				float percent = zeroBasedMaximum == 0 ? 0.0f : zeroBasedValue / (float) zeroBasedMaximum;
				if (_isVertical) {
					float indicatorHeight = (FinalSize.Height - penWidth * 2) * percent;
					graphics.FillRectangle(_progressBrush, AbsolutePosition.X + penWidth,
					                       AbsolutePosition.Y + FinalSize.Height - penWidth - indicatorHeight,
					                       FinalSize.Width - penWidth * 2, indicatorHeight);
				}
				else {
					graphics.FillRectangle(_progressBrush, AbsolutePosition.X + penWidth, AbsolutePosition.Y + penWidth,
					                       (FinalSize.Width - penWidth * 2) * percent, FinalSize.Height - penWidth * 2);
				}
			}
		}


		/// <summary>
		/// Creates a new <see cref="LcdGdiProgressBar"/> with a black edge, white fill
		/// and black indicator part, which is the most common for monochrome devices.
		/// </summary>
		public LcdGdiProgressBar() {
			Pen = Pens.Black;
			Brush = Brushes.White;
			ProgressBrush = Brushes.Black;
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiProgressBar"/> with the specified pen for the edge,
		/// brush for the fill, and brush for the progression.
		/// </summary>
		/// <param name="pen">Pen to use to draw the edge of this object.</param>
		/// <param name="brush">Brush to use to draw the fill of this object.</param>
		/// <param name="progressBrush">Brush to use to fill the progress indicator.</param>
		public LcdGdiProgressBar(Pen pen, Brush brush, Brush progressBrush) {
			Pen = pen;
			Brush = brush;
			ProgressBrush = progressBrush;
		}
	}

}