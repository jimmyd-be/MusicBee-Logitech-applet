using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Permissions;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Encapsulates a LCD device. You can retrieve an derived instance of this class by using
	/// <see cref="LcdApplet.OpenDeviceByType"/>.
	/// </summary>
	public abstract class LcdDevice : IDisposable {


		#region Properties

		private readonly SafeNativeMethods.LgLcdOnSoftButtonsCallback _softButtonsCallback;
		private readonly LcdApplet _applet;
		private readonly LcdDeviceType _deviceType;
		private readonly List<LcdPage> _pages = new List<LcdPage>();
		private LcdPage _currentPage;
		private bool _currentPageJustChanged;
		private readonly Stopwatch _stopwatch;
		private volatile int _deviceNumber;
		private volatile bool _setAsForegroundApplet;

		/// <summary>
		/// Gets the <see cref="LcdApplet"/> that opened this device.
		/// </summary>
		public LcdApplet Applet {
			get { return _applet; }
		}

		/// <summary>
		/// Gets the type of this device.
		/// </summary>
		public LcdDeviceType DeviceType {
			get { return _deviceType; }
		}

		/// <summary>
		/// Gets a list of pages shown on this device.
		/// Note that this list is only provided to easily keep pages associated with a device but
		/// is not mandatory to use. One can set a <see cref="CurrentPage"/> that is not in this collection.
		/// </summary>
		public List<LcdPage> Pages {
			get { return _pages; }
		}

		/// <summary>
		/// Gets or sets the current shown page. The default is <c>null</c>.
		/// </summary>
		public LcdPage CurrentPage {
			get { return _currentPage; }
			set {
				_currentPage = value;
				_currentPageJustChanged = true;
			}
		}

		/// <summary>
		/// Gets the width of this device, in pixels.
		/// </summary>
		public abstract int PixelWidth { get; }

		/// <summary>
		/// Gets the height of this device, in pixels.
		/// </summary>
		public abstract int PixelHeight { get; }

		/// <summary>
		/// Gets the number of bits per pixel (aka color depth) of this device.
		/// </summary>
		public abstract int BitsPerPixel { get; }

		/// <summary>
		/// Gets whether the device has been disposed.
		/// </summary>
		public bool IsDisposed {
			get { return _deviceNumber == -1; }
		}

		/// <summary>
		/// Gets the internal number of this device.
		/// </summary>
		protected internal int DeviceNumber {
			get { return _deviceNumber; }
		}

		/// <summary>
		/// Gets or sets whether the applet will become the one shown on the LCD and prevents the LCD library from
		/// switching to other applications, when this property is <c>true</c>.
		/// When this property is reverted to <c>false</c>, the LCD library resumes its switching algorithm that the user had chosen.
		/// </summary>
		public bool SetAsForegroundApplet {
			get { return _setAsForegroundApplet; }
			set {
				VerifyValid();
				SafeNativeMethods.LgLcdSetAsLcdForegroundApp(_deviceNumber, value);
				_setAsForegroundApplet = value;
			}
		}

		#endregion


		#region Events

		/// <summary>
		/// Occurs when some soft buttons are pressed or released and the applet is currently is foreground on this device.
		/// Note that this event is raised in the context of a thread within the library.
		/// Therefore, take the necessary precautions for thread safety.
		/// </summary>
		public event EventHandler<LcdSoftButtonsEventArgs> SoftButtonsChanged;

		/// <summary>
		/// Raise the <see cref="OnSoftButtonsChanged"/> event.
		/// </summary>
		/// <param name="buttons"></param>
		private void OnSoftButtonsChanged(LcdSoftButtons buttons) {
			EventHandler<LcdSoftButtonsEventArgs> handler = SoftButtonsChanged;
			if (handler != null)
				handler(this, new LcdSoftButtonsEventArgs(buttons));
		}

		#endregion

		
		/// <summary>
		/// Read the current state of the soft buttons for the specified device.
		/// If your application is not being currently displayed, you will receive a resulting
		/// value of <see cref="LcdSoftButtons.None"/>, even if some soft buttons are being pressed.
		/// This is in order to avoid users inadvertently interacting with an application that does
		/// not presently show on the display. 
		/// </summary>
		/// <returns>A combination of <see cref="LcdSoftButtons"/> flags representing which buttons are pressed.</returns>
		public LcdSoftButtons ReadSoftButtons() {
			VerifyValid();
			return SafeNativeMethods.LgLcdReadSoftButtons(_deviceNumber);
		}

		/// <summary>
		/// Updates a bitmap of the device. Use this function if you want to manually manage the pixels sent
		/// to the device, without using the Pages feature.
		/// </summary>
		/// <param name="pixels">An array of pixels constituting the bitmap. See the SDK help for more information.</param>
		/// <param name="priority">Priority of the update.</param>
		/// <param name="updateMode">Update mode (synchronous or asynchronous).</param>
		/// <returns>
		/// If <paramref name="updateMode"/> is <see cref="LcdUpdateMode.SyncCompleteWithinFrame"/>, this function returns
		/// <c>true</c> if the bitmap was corrently updated within the synchronous time frame and <c>false</c> otherwise.
		/// For every other mode, this function always returns <c>true</c>.
		/// </returns>
		public bool UpdateBitmap(byte[] pixels, LcdPriority priority, LcdUpdateMode updateMode) {
			if (pixels == null)
				throw new ArgumentNullException("pixels");
			if (pixels.Length != PixelWidth * PixelHeight * BitsPerPixel / 8)
				throw new ArgumentOutOfRangeException("pixels", "Pixels length is invalid for this device.");
			VerifyValid();
			try {
				return UpdateBitmapCore(pixels, priority, updateMode);
			}
			catch (Win32Exception) {
				// The device was invalidated between VerifyValid() and UpdateBitmapCore(), ignore
				if (_deviceNumber == -1)
					return false;
				throw;
			}
		}

		/// <summary>
		/// Really updates a bitmap of the device.
		/// </summary>
		/// <param name="pixels">An array of pixels constituting the bitmap. See the SDK help for more information.</param>
		/// <param name="priority">Priority of the update.</param>
		/// <param name="updateMode">Update mode (synchronous or asynchronous).</param>
		/// <returns>
		/// If <paramref name="updateMode"/> is <see cref="LcdUpdateMode.SyncCompleteWithinFrame"/>, this function returns
		/// <c>true</c> if the bitmap was corrently updated within the synchronous time frame and <c>false otherwise</c>.
		/// For every other mode, this function always returns <c>true</c>.
		/// </returns>
		protected abstract bool UpdateBitmapCore(byte[] pixels, LcdPriority priority, LcdUpdateMode updateMode);

		/// <summary>
		/// This method must be called by the program in its idle time to keep updating the device's current page.
		/// </summary>
		/// <remarks>
		/// If <see cref="CurrentPage"/> is <c>null</c>, nothing will happen.
		/// Even if this method is called repeatedly, the update and draw only occur
		/// at the <see cref="LcdPage.DesiredFramerate"/>.
		/// </remarks>
		public void DoUpdateAndDraw() {
			LcdPage currentPage = CurrentPage;
			if (currentPage == null)
				return;
			VerifyValid();
			TimeSpan desiredTimeBetweenFrames = new TimeSpan(TimeSpan.TicksPerSecond / currentPage.DesiredFramerate);
			TimeSpan elapsedTotal = _stopwatch.Elapsed;
			TimeSpan elapsedSinceLastFrame = currentPage.LastFrameUpdate == TimeSpan.Zero
				? TimeSpan.Zero
				: elapsedTotal - currentPage.LastFrameUpdate;
			if (elapsedSinceLastFrame >= desiredTimeBetweenFrames || currentPage.LastFrameUpdate == TimeSpan.Zero || _currentPageJustChanged) {
				bool redrawNeeded = currentPage.Update(elapsedTotal, elapsedSinceLastFrame);
				if (redrawNeeded || _currentPageJustChanged)
					UpdateBitmap(currentPage.Draw(), currentPage.Priority, currentPage.UpdateMode);
				currentPage.LastFrameUpdate = elapsedTotal;
				_currentPageJustChanged = false;
			}
		}


		private int SoftButtonsChangedCallback(int device, LcdSoftButtons buttons, IntPtr context) {
			OnSoftButtonsChanged(buttons);
			return 0;
		}

		private void Applet_ConnectionDisrupted(object sender, EventArgs e) {
			Dispose();
		}

		private void Applet_DeviceRemoval(object sender, LcdDeviceTypeEventArgs e) {
			if (e.DeviceType == DeviceType)
				Dispose();
		}

		/// <summary>
		/// Reopens this device after it has been disposed.
		/// </summary>
		public void ReOpen() {
			if (_deviceNumber != -1)
				throw new InvalidOperationException("ReOpen() can only be called on a closed device.");
			SafeNativeMethods.LgLcdOpenByTypeContext context = new SafeNativeMethods.LgLcdOpenByTypeContext {
				Connection = _applet.ConnectionNumber,
				LcdDeviceType = _deviceType,
				SoftButtonsChangedCallback = _softButtonsCallback,
				SoftButtonsChangedContext = IntPtr.Zero,
				Device = -1
			};
			_deviceNumber = SafeNativeMethods.LgLcdOpenByType(context);
			_applet.DeviceRemoval += Applet_DeviceRemoval;
			_applet.ConnectionDisrupted += Applet_ConnectionDisrupted;
			_currentPageJustChanged = true;
			_stopwatch.Start();
		}


		#region IDisposable

		/// <summary>
		/// Ensures that the object is not disposed.
		/// </summary>
		protected void VerifyValid() {
			if (IsDisposed)
				throw new InvalidOperationException("This method cannot be called after the device has been disposed.");
		}
        
		/// <summary>
		/// Releases the resources associated with this <see cref="LcdDevice"/>.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources associated with this <see cref="LcdDevice"/>.
		/// </summary>
		/// <param name="disposing">Whether to also release managed resources along with unmanaged ones.</param>
		protected virtual void Dispose(bool disposing) {
			if (_deviceNumber != -1) {
				_applet.ConnectionDisrupted -= Applet_ConnectionDisrupted;
				_applet.DeviceRemoval -= Applet_DeviceRemoval;
				_setAsForegroundApplet = false;
				SafeNativeMethods.LgLcdClose(_deviceNumber);
				_stopwatch.Reset();
				_deviceNumber = -1;
			}
		}

		#endregion


		/// <summary>
		/// Creates a new instance of <see cref="LcdDevice"/> for the given applet and device type.
		/// </summary>
		/// <param name="applet"><see cref="LcdApplet"/> that opened this device.</param>
		/// <param name="deviceType">Type of this device.</param>
		internal LcdDevice(LcdApplet applet, LcdDeviceType deviceType) {
			new LgLcdPermission(PermissionState.Unrestricted).Demand();
			_applet = applet;
			_deviceType = deviceType;
			_deviceNumber = -1;
			_softButtonsCallback = SoftButtonsChangedCallback;
			_stopwatch = new Stopwatch();
			ReOpen();
		}
	}

}