using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Base class for every object that is drawn on a <see cref="LcdGdiPage"/>.
	/// </summary>
	/// <seealso cref="Graphics"/>.
	public abstract class LcdGdiObject : IDisposable {


		#region Properties

		private Pen _pen;
		private Brush _brush;
		private LcdGdiHorizontalAlignment _horizontalAlignment;
		private LcdGdiVerticalAlignment _verticalAlignment;
		private MarginF _margin;
		private SizeF _size;
		private Matrix _transform = new Matrix();
		private Region _clip;
		private InterpolationMode _interpolationMode;
		private PixelOffsetMode _pixelOffsetMode;
		private Point _renderingOrigin;
		private SmoothingMode _smoothingMode;
		private bool _isVisible = true;
		private bool _hasChanged = true;

		/// <summary>
		/// Gets or sets the pen to use to draw the edge of this object.
		/// </summary>
		public Pen Pen {
			get { return _pen; }
			set {
				if (_pen != value) {
					_pen = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush to use to draw the fill of this object.
		/// </summary>
		public Brush Brush {
			get { return _brush; }
			set {
				if (_brush != value) {
					_brush = value;
					HasChanged = true;
				}
			}
		}
        
		/// <summary>
		/// Gets or sets the horizontal alignment of this object on the <see cref="LcdGdiPage"/>.
		/// </summary>
		public LcdGdiHorizontalAlignment HorizontalAlignment {
			get { return _horizontalAlignment; }
			set {
				if (_horizontalAlignment != value) {
					_horizontalAlignment = value;
					HasChanged = true;
				}
			}
		}
        
		/// <summary>
		/// Gets or sets the vertical alignment of this object on the <see cref="LcdGdiPage"/>.
		/// </summary>
		public LcdGdiVerticalAlignment VerticalAlignment {
			get { return _verticalAlignment; }
			set {
				if (_verticalAlignment != value) {
					_verticalAlignment = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the margins of this <see cref="LcdGdiObject"/> on the <see cref="LcdGdiPage"/>.
		/// </summary>
		public MarginF Margin {
			get { return _margin; }
			set {
				if (_margin != value) {
					_margin = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the size of this <see cref="LcdGdiObject"/> on the <see cref="LcdGdiPage"/>.
		/// </summary>
		/// <remarks>
		/// Note that this member is not relevant for every object. For example, <see cref="LcdGdiText"/> has this
		/// member set to (0,0) to not limit the drawn text. In any case, use <see cref="FinalSize"/> to get
		/// the final calculated size of this <see cref="LcdGdiObject"/>.
		/// The <see cref="SizeF.Width"/> member of this property is not used if <see cref="HorizontalAlignment"/>
		/// is <see cref="LcdGdiHorizontalAlignment.Stretch"/>.
		/// The <see cref="SizeF.Height"/> member of this property is not used if <see cref="VerticalAlignment"/>
		/// is <see cref="LcdGdiVerticalAlignment.Stretch"/>.
		/// </remarks>
		public SizeF Size {
			get { return _size; }
			set {
				if (_size != value) {
					_size = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets a copy of the geometric world transformation for this <see cref="LcdGdiObject"/>.
		/// </summary>
		/// <seealso cref="Graphics.Transform"/>.
		public Matrix Transform {
			get { return _transform; }
			set {
				if (_transform != value) {
					_transform = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets a <see cref="Region"/> that delimits the drawing region of this <see cref="LcdGdiObject"/>.
		/// </summary>
		/// <seealso cref="Graphics.Clip"/>.
		public Region Clip {
			get { return _clip; }
			set {
				if (_clip != value) {
					_clip = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the interpolation mode associated with this <see cref="LcdGdiObject"/>.
		/// </summary>
		/// <seealso cref="Graphics.InterpolationMode"/>.
		public InterpolationMode InterpolationMode {
			get { return _interpolationMode; }
			set {
				if (_interpolationMode != value) {
					_interpolationMode = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or set a value specifying how pixels are offset during rendering of this <see cref="LcdGdiObject"/>.
		/// </summary>
		/// <seealso cref="Graphics.PixelOffsetMode"/>.
		public PixelOffsetMode PixelOffsetMode {
			get { return _pixelOffsetMode; }
			set {
				if (_pixelOffsetMode != value) {
					_pixelOffsetMode = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the rendering origin of this <see cref="LcdGdiObject"/> for dithering and for hatch brushes.
		/// </summary>
		/// <seealso cref="Graphics.RenderingOrigin"/>.
		public Point RenderingOrigin {
			get { return _renderingOrigin; }
			set {
				if (_renderingOrigin != value) {
					_renderingOrigin = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the rendering quality for this <see cref="LcdGdiObject"/>.
		/// </summary>
		/// <seealso cref="Graphics.SmoothingMode"/>.
		public SmoothingMode SmoothingMode {
			get { return _smoothingMode; }
			set {
				if (_smoothingMode != value) {
					_smoothingMode = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object is visible and drawn.
		/// </summary>
		/// <remarks>
		/// If this property is <c>false</c>, <see cref="Update"/> will still be called, but not <see cref="Draw"/>.
		/// </remarks>
		public bool IsVisible {
			get { return _isVisible; }
			set {
				if (_isVisible != value) {
					_isVisible = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets any user-defined object associated with this <see cref="LcdGdiObject"/>.
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// Gets the absolute position of this object, calculated from <see cref="HorizontalAlignment"/>,
		/// <see cref="VerticalAlignment"/> and <see cref="Margin"/>. This value is valid only after the object
		/// has been updated on a specified device.
		/// </summary>
		public PointF AbsolutePosition { get; internal set; }

		/// <summary>
		/// Gets the calculated size of this object. This value is valid only after the object has been
		/// updated on a specified device.
		/// </summary>
		public SizeF FinalSize { get; internal set; }

		/// <summary>
		/// Gets or sets whether this object has changed since last frame.
		/// </summary>
		public bool HasChanged {
			get { return _hasChanged; }
			protected internal set {
				_hasChanged = value;
				if (value)
					OnChanged();
			}
		}

		#endregion


		/// <summary>
		/// Occurs when a property change that invalidate the object (and thus requires another update pass) occurs.
		/// </summary>
		public event EventHandler Changed;

		/// <summary>
		/// Raises the <see cref="Changed"/> event.
		/// </summary>
		protected virtual void OnChanged() {
			EventHandler handler = Changed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}


		/// <summary>
		/// Calculates the object absolute position given its alignments and final size.
		/// </summary>
		/// <param name="bitmapSize">Size of the bitmap where this object is drawn.</param>
		/// <param name="offset">Additional offset to fix position problems when right/bottom aligned.</param>
		protected void CalcAbsolutePosition(Size bitmapSize, float offset) {
			float x;
			switch (HorizontalAlignment) {
				case LcdGdiHorizontalAlignment.Right:
					x = bitmapSize.Width - FinalSize.Width - Margin.Right - offset;
					break;
				case LcdGdiHorizontalAlignment.Center:
					x = (float) Math.Round((bitmapSize.Width - FinalSize.Width) / 2.0f + Margin.Left - Margin.Right);
					break;
				default:
					x = Margin.Left;
					break;
			}
			float y;
			switch (VerticalAlignment) {
				case LcdGdiVerticalAlignment.Bottom:
					y = bitmapSize.Height - FinalSize.Height - Margin.Bottom - offset;
					break;
				case LcdGdiVerticalAlignment.Middle:
					y = (float) Math.Round((bitmapSize.Height - FinalSize.Height) / 2.0f + Margin.Top - Margin.Bottom);
					break;
				default:
					y = Margin.Top;
					break;
			}
			AbsolutePosition = new PointF(x, y);
		}

		/// <summary>
		/// Calculates the object absolute position given its alignments and final size.
		/// </summary>
		/// <param name="bitmapSize">Size of the bitmap where this object is drawn.</param>
		protected void CalcAbsolutePosition(Size bitmapSize) {
			CalcAbsolutePosition(bitmapSize, 0f);
		}

		/// <summary>
		/// Derived classes must override this method in order to calculate the
		/// absolute position and final size of the object. This method is called only if
		/// <see cref="HasChanged"/> is <c>true</c>.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal virtual void Update(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame, LcdGdiPage page, Graphics graphics) {
			FinalSize = new SizeF(
				HorizontalAlignment != LcdGdiHorizontalAlignment.Stretch ? Size.Width : page.Bitmap.Width - Margin.Left - Margin.Right,
				VerticalAlignment != LcdGdiVerticalAlignment.Stretch ? Size.Height : page.Bitmap.Height - Margin.Top - Margin.Bottom);
			CalcAbsolutePosition(page.Bitmap.Size);
		}

		/// <summary>
		/// Derived classes must override this method in order to draw the GDI object.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal abstract void Draw(LcdGdiPage page, Graphics graphics);


		#region IDisposable

		/// <summary>
		/// Releases the resources associated with this <see cref="LcdGdiObject"/>.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources associated with this <see cref="LcdGdiObject"/>.
		/// </summary>
		/// <param name="disposing">Whether to also release managed resources along with unmanaged ones.</param>
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (_brush != null)
					_brush.Dispose();
				if (_pen != null)
					_pen.Dispose();
				if (_clip != null)
					_clip.Dispose();
			}
		}

		#endregion


	}

}