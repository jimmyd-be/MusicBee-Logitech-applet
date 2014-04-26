using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Base class for every device page.
	/// </summary>
	public abstract class LcdPage : IDisposable {


		#region Properties

		private readonly LcdDevice _device;
		private int _desiredFramerate;
		private bool _invalidated;

		/// <summary>
		/// Gets the device where this page will be shown.
		/// </summary>
		public LcdDevice Device {
			get { return _device; }
		}

		/// <summary>
		/// Gets the last time this page was updated, relative to the device's creation time.
		/// </summary>
		internal TimeSpan LastFrameUpdate { get; set; }

		/// <summary>
		/// Gets or sets the priority of this page when drawing.
		/// The default value is <see cref="LcdPriority.Normal"/>.
		/// </summary>
		public LcdPriority Priority { get; set; }

		/// <summary>
		/// Gets or sets the mode (synchronous or asynchronous) used to update this page.
		/// The default value is <see cref="LcdUpdateMode.Async"/>.
		/// </summary>
		public LcdUpdateMode UpdateMode { get; set; }

		/// <summary>
		/// Gets or sets the desired frame rate, in frame per seconds, for this page.
		/// The default value is 30.
		/// </summary>
		public int DesiredFramerate {
			get { return _desiredFramerate; }
			set {
				if (value < 1 || value > 60)
					throw new ArgumentOutOfRangeException("value", "DesiredFrameRate must be between 1 and 60");
				_desiredFramerate = value;
			}
		}

		#endregion


		#region Events

		/// <summary>
		/// Occurs when the page starts updating its content.
		/// </summary>
		public event EventHandler<UpdateEventArgs> Updating;

		/// <summary>
		/// Occurs when the page has updated its content.
		/// </summary>
		public event EventHandler<UpdateEventArgs> Updated;

		/// <summary>
		/// Occurs when the page starts drawing its content.
		/// </summary>
		public event EventHandler Drawing;

		/// <summary>
		/// Occurs when the page has drawn its content, but the pixels have not yet been send to the device.
		/// This is your last chance to do some kind of post-processing on the pixels.
		/// </summary>
		public event EventHandler<DrawnEventArgs> Drawn;

		/// <summary>
		/// Raises the <see cref="Updating"/> event.
		/// </summary>
		/// <param name="e">Update parameters.</param>
		protected virtual void OnUpdating(UpdateEventArgs e) {
			EventHandler<UpdateEventArgs> handler = Updating;
			if (handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Raises the <see cref="Updated"/> event.
		/// </summary>
		/// <param name="e">Update parameters.</param>
		protected virtual void OnUpdated(UpdateEventArgs e) {
			EventHandler<UpdateEventArgs> handler = Updated;
			if (handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Raises the <see cref="Drawing"/> event.
		/// </summary>
		protected virtual void OnDrawing() {
			EventHandler handler = Drawing;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the <see cref="Drawn"/> event.
		/// </summary>
		protected virtual void OnDrawn(byte[] pixels) {
			EventHandler<DrawnEventArgs> handler = Drawn;
			if (handler != null)
				handler(this, new DrawnEventArgs(pixels));
		}

		#endregion


		/// <summary>
		/// Sets this page as <see cref="Device"/>'s <see cref="LcdDevice.CurrentPage"/>.
		/// </summary>
		public void SetAsCurrentDevicePage() {
			_device.CurrentPage = this;
		}

		/// <summary>
		/// Call this method to force an update and draw cycle even if the page has not changed.
		/// </summary>
		public void Invalidate() {
			_invalidated = true;
		}

		/// <summary>
		/// Updates the page content.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <returns><c>true</c> if the update has done something and a redraw is required.</returns>
		internal bool Update(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame) {
			UpdateEventArgs updateEventArgs = new UpdateEventArgs(elapsedTotalTime, elapsedTimeSinceLastFrame);
			OnUpdating(updateEventArgs);
			bool redrawNeeded = UpdateCore(elapsedTotalTime, elapsedTimeSinceLastFrame) || _invalidated;
			OnUpdated(updateEventArgs);
			_invalidated = false;
			return redrawNeeded;
		}

		/// <summary>
		/// Derived classes override this method in order to update the page content.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <returns><c>true</c> if the update has done something and a redraw is required.</returns>
		protected abstract bool UpdateCore(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame);

		/// <summary>
		/// Draws the page content.
		/// </summary>
		/// <returns>A pixel array conforming to <see cref="Device"/>'s <see cref="LcdDevice.DeviceType"/>.</returns>
		internal byte[] Draw() {
			OnDrawing();
			byte[] pixels = DrawCore();
			OnDrawn(pixels);
			return pixels;
		}

		/// <summary>
		/// Derived classes override this method in order to draw the page content visually.
		/// </summary>
		/// <returns>Implementors must return a pixel array conforming to
		/// <see cref="Device"/>'s <see cref="LcdDevice.DeviceType"/>.</returns>
		protected abstract byte[] DrawCore();


		#region IDisposable

		/// <summary>
		/// Releases the resources associated with this <see cref="LcdPage"/>.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources associated with this <see cref="LcdPage"/>.
		/// </summary>
		/// <param name="disposing">Whether to also release managed resources along with unmanaged ones.</param>
		protected virtual void Dispose(bool disposing) {	
		}

		#endregion
        

		/// <summary>
		/// Creates a new <see cref="LcdPage"/> on the given device.
		/// </summary>
		/// <param name="device">Device where this page will be shown.</param>
		protected LcdPage(LcdDevice device) {
			if (device == null)
				throw new ArgumentNullException("device");
			_device = device;
			DesiredFramerate = 30;
			LastFrameUpdate = TimeSpan.Zero;
			Priority = LcdPriority.Normal;
			UpdateMode = LcdUpdateMode.Async;
		}
	}

}