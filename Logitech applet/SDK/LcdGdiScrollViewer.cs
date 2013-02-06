using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Object that can be placed on a <see cref="LcdGdiPage"/> and that provides scrolling support for its child.
	/// </summary>
	public class LcdGdiScrollViewer : LcdGdiObject {


		#region Properties

		private LcdGdiObject _child;

		private SizeF _extent;
		private float _maxScrollX;
		private float _maxScrollY;
		private float _currentScrollX;
		private float _currentScrollY;

		private bool _autoScrollX;
		private bool _autoScrollXWasSet;
		private TimeSpan _autoScrollXStartTime;
		private TimeSpan _autoScrollXEndedTime;
		private float _autoScrollSpeedX = 12.0f;
		private TimeSpan _autoScrollXFixedStartTime = TimeSpan.FromSeconds(1.0);
		private TimeSpan _autoScrollXFixedEndTime = TimeSpan.FromSeconds(1.0);

		private bool _autoScrollY;
		private bool _autoScrollYWasSet;
		private TimeSpan _autoScrollYStartTime;
		private TimeSpan _autoScrollYEndedTime;
		private float _autoScrollSpeedY = 12.0f;
		private TimeSpan _autoScrollYFixedStartTime = TimeSpan.FromSeconds(1.0);
		private TimeSpan _autoScrollYFixedEndTime = TimeSpan.FromSeconds(1.0);

		/// <summary>
		/// Gets the extent of this scroll viewer, that is the complete size taken by the child.
		/// This value only has a meaning after <see cref="Update"/> was called.
		/// </summary>
		public SizeF Extent {
			get { return _child == null ? SizeF.Empty : new SizeF(_child.AbsolutePosition + _child.FinalSize); }
		}

		/// <summary>
		/// Gets the view port of this scroll viewer, that is the visible size.
		/// This value only has a meaning after <see cref="Update"/> was called.
		/// </summary>
		public SizeF Viewport {
			get { return FinalSize; }
		}

		/// <summary>
		/// Gets the maximum scroll value in the horizontal axis.
		/// </summary>
		public float MaxScrollX {
			get { return _maxScrollX; }
		}

		/// <summary>
		/// Gets the maximum scroll value in the vertical axis.
		/// </summary>
		public float MaxScrollY {
			get { return _maxScrollY; }
		}

		/// <summary>
		/// Gets or sets the current scroll value in the horizontal axis.
		/// If less than 0.0f is passed, 0.0f will be used.
		/// If more than <see cref="MaxScrollX"/> is passed, <see cref="MaxScrollX"/> will be used.
		/// </summary>
		public float CurrentScrollX {
			get { return _currentScrollX; }
			set {
				float newValue = Math.Min(Math.Max(0.0f, value), _maxScrollX);
				if (_currentScrollX != newValue) {
					_currentScrollX = newValue;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the current scroll value in the horizontal axis.
		/// If less than 0.0f is passed, 0.0f will be used.
		/// If more than <see cref="MaxScrollY"/> is passed, <see cref="MaxScrollY"/> will be used.
		/// </summary>
		public float CurrentScrollY {
			get { return _currentScrollY; }
			set {
				float newValue = Math.Min(Math.Max(0.0f, value), _maxScrollY);
				if (_currentScrollY != newValue) {
					_currentScrollY = newValue;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the control automatically scrolls in the horizontal axis.
		/// </summary>
		public bool AutoScrollX {
			get { return _autoScrollX; }
			set {
				if (_autoScrollX != value) {
					_autoScrollX = value;
					_autoScrollXWasSet = false;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the speed of the auto scrolling in the horizontal axis, in pixels per second.
		/// The default value is 25.0f;
		/// </summary>
		public float AutoScrollSpeedX {
			get { return _autoScrollSpeedX; }
			set {
				if (_autoScrollSpeedX != value) {
					_autoScrollSpeedX = value;
					_autoScrollXWasSet = false;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets the time to wait before the horizontal scrolling occurs.
		/// The default value is one second.
		/// </summary>
		public TimeSpan AutoScrollXFixedStartTime {
			get { return _autoScrollXFixedStartTime; }
			set {
				if (_autoScrollXFixedStartTime != value) {
					_autoScrollXFixedStartTime = value;
					_autoScrollXWasSet = false;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets the time to wait after the horizontal scrolling occurred, before restarting it.
		/// The default value is one second.
		/// </summary>
		public TimeSpan AutoScrollXFixedEndTime {
			get { return _autoScrollXFixedEndTime; }
			set {
				if (_autoScrollXFixedEndTime != value) {
					_autoScrollXFixedEndTime = value;
					_autoScrollXWasSet = false;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the control automatically scrolls in the vertical axis.
		/// </summary>
		public bool AutoScrollY {
			get { return _autoScrollY; }
			set {
				if (_autoScrollY != value) {
					_autoScrollY = value;
					_autoScrollYWasSet = false;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the speed of the auto scrolling in the vertical axis, in pixels per second.
		/// The default value is 25.0f;
		/// </summary>
		public float AutoScrollSpeedY {
			get { return _autoScrollSpeedY; }
			set {
				if (_autoScrollSpeedY != value) {
					_autoScrollSpeedY = value;
					_autoScrollYWasSet = false;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets the time to wait before the vertical scrolling occurs.
		/// The default value is one second.
		/// </summary>
		public TimeSpan AutoScrollYFixedStartTime {
			get { return _autoScrollYFixedStartTime; }
			set {
				if (_autoScrollYFixedStartTime != value) {
					_autoScrollYFixedStartTime = value;
					_autoScrollYWasSet = false;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets the time to wait after the vertical scrolling occurred, before restarting it.
		/// The default value is one second.
		/// </summary>
		public TimeSpan AutoScrollYFixedEndTime {
			get { return _autoScrollYFixedEndTime; }
			set {
				if (_autoScrollYFixedEndTime != value) {
					_autoScrollYFixedEndTime = value;
					_autoScrollYWasSet = false;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the child object that will be scrolled.
		/// </summary>
		public LcdGdiObject Child {
			get { return _child; }
			set {
				if (_child != value) {
					if (value != null)
						value.Changed -= Child_Changed;
					_child = value;
					if (value != null)
						value.Changed += Child_Changed;
					HasChanged = true;
				}
			}
		}

		#endregion


		private void Child_Changed(object sender, EventArgs e) {
			HasChanged = true;
		}


		/// <summary>
		/// Sets a clipping region that is the intersection of this object's clip, the child clip and the viewport.
		/// </summary>
		/// <param name="graphics">Graphics on which to set the clip.</param>
		private void SetClipForChild(Graphics graphics) {
			Region thisClip = graphics.Clip.Clone();
			LcdGdiPage.PrepareGraphicsForChild(graphics, Child);
			Region childClip = graphics.Clip;
			thisClip.Intersect(childClip);
			thisClip.Intersect(new RectangleF(AbsolutePosition, Viewport));
			graphics.Clip = thisClip;
		}

		/// <summary>
		/// Updates the position of the object.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Update(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame, LcdGdiPage page, Graphics graphics) {
			base.Update(elapsedTotalTime, elapsedTimeSinceLastFrame, page, graphics);
			if (_child != null) {
				SetClipForChild(graphics);
				_child.Update(elapsedTotalTime, elapsedTimeSinceLastFrame, page, graphics);
				_extent = new SizeF(_child.AbsolutePosition + _child.FinalSize);
				_maxScrollX = Math.Max(0.0f, _extent.Width - FinalSize.Width + 1.0f);
				_maxScrollY = Math.Max(0.0f, _extent.Height - FinalSize.Height + 1.0f);

				// Auto-scroll X
				if (_autoScrollXWasSet) {
					TimeSpan elapsedAutoScrollXTime = elapsedTotalTime - _autoScrollXStartTime;
					if (elapsedAutoScrollXTime <= _autoScrollXFixedStartTime)
						_currentScrollX = 0.0f;
					else if (_currentScrollX >= _maxScrollX) {
						if (_autoScrollXEndedTime == TimeSpan.Zero)
							_autoScrollXEndedTime = elapsedTotalTime;
						else if (elapsedTotalTime - _autoScrollXEndedTime >= _autoScrollXFixedEndTime) {
							_autoScrollXWasSet = false;
							_currentScrollX = 0.0f;
						}
					}
					else
						_currentScrollX = (float) ((elapsedAutoScrollXTime - _autoScrollXFixedStartTime).TotalSeconds * _autoScrollSpeedX);
				}
				else {
					_autoScrollXWasSet = true;
					_autoScrollXStartTime = elapsedTotalTime;
					_autoScrollXEndedTime = TimeSpan.Zero;
				}

				// Auto-scroll Y
				if (_autoScrollYWasSet) {
					TimeSpan elapsedAutoScrollYTime = elapsedTotalTime - _autoScrollYStartTime;
					if (elapsedAutoScrollYTime <= _autoScrollYFixedStartTime)
						_currentScrollY = 0.0f;
					else if (_currentScrollY >= _maxScrollY) {
						if (_autoScrollYEndedTime == TimeSpan.Zero)
							_autoScrollYEndedTime = elapsedTotalTime;
						else if (elapsedTotalTime - _autoScrollYEndedTime >= _autoScrollYFixedEndTime) {
							_autoScrollYWasSet = false;
							_currentScrollY = 0.0f;
						}
					}
					else
						_currentScrollY = (float) ((elapsedAutoScrollYTime - _autoScrollYFixedStartTime).TotalSeconds * _autoScrollSpeedY);
				}
				else {
					_autoScrollYWasSet = true;
					_autoScrollYStartTime = elapsedTotalTime;
					_autoScrollYEndedTime = TimeSpan.Zero;
				}

				_currentScrollX = Math.Min(_currentScrollX, _maxScrollX);
				_currentScrollY = Math.Min(_currentScrollY, _maxScrollY);
				_child.AbsolutePosition = new PointF(
					_child.AbsolutePosition.X + AbsolutePosition.X - _currentScrollX,
					_child.AbsolutePosition.Y + AbsolutePosition.Y - _currentScrollY);
			}
			else {
				_extent = SizeF.Empty;
				_maxScrollX = 0.0f;
				_maxScrollY = 0.0f;
				_currentScrollX = 0.0f;
				_currentScrollY = 0.0f;
			}
		}

		/// <summary>
		/// Draws this object.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (_child != null) {
				SetClipForChild(graphics);
				_child.Draw(page, graphics);
				if (_autoScrollX || _autoScrollY)
					HasChanged = true;
			}
		}


		/// <summary>
		/// Creates a new <see cref="LcdGdiScrollViewer"/>.
		/// </summary>
		public LcdGdiScrollViewer() {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiScrollViewer"/> with the specified child.
		/// </summary>
		/// <param name="child">Child to add into the scroll viewer.</param>
		public LcdGdiScrollViewer(LcdGdiObject child) {
			_child = child;
		}
	}

}