using System;
using System.Security.Permissions;
using System.Threading;

namespace GammaJul.LgLcd {

	/// <summary>
	/// This class represent a single LCD applet.
	/// </summary>
	public class LcdApplet : IDisposable {
		private static int _initCount;
		

		#region Properties

		private volatile int _connectionNumber = -1;
		private volatile bool _isDisposed;
		private volatile bool _isEnabled;
		private string _friendlyName = String.Empty;
		private bool _isAutoStartable;
		private LcdAppletCapabilities _capabilities = LcdAppletCapabilities.Monochrome;
		
		/// <summary>
		/// Gets whether the applet is connected.
		/// </summary>
		public bool IsConnected {
			get { return _connectionNumber != -1; }
		}

		/// <summary>
		/// Gets whether the applet has been disposed.
		/// </summary>
		public bool IsDisposed {
			get { return _isDisposed; }
		}

		/// <summary>
		/// Gets whether the applet is enabled.
		/// </summary>
		public bool IsEnabled {
			get { return _isEnabled; }
		}

		/// <summary>
		/// Gets or sets the friendly name of the applet that is displayed in LCD Monitor.
		/// Note that changing this property must be done before calling <see cref="Connect"/>.
		/// </summary>
		public string FriendlyName {
			get { return _friendlyName; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				VerifyNotConnected();
				_friendlyName = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this applet has the option to be auto-startable.
		/// The default is <c>false</c>.
		/// Note that changing this property must be done before calling <see cref="Connect"/>.
		/// </summary>
		public bool IsAutoStartable {
			get { return _isAutoStartable; }
			set {
				VerifyNotConnected();
				_isAutoStartable = value;
			}
		}

		/// <summary>
		/// Gets or sets the capabilities that this applet supports.
		/// The default is <see cref="LcdAppletCapabilities.Monochrome"/>.
		/// Note that changing this property must be done before calling <see cref="Connect"/>.
		/// </summary>
		public LcdAppletCapabilities Capabilities {
			get { return _capabilities; }
			set {
				VerifyNotConnected();
				_capabilities = value;
			}
		}

		/// <summary>
		/// Gets the internal number of this applet.
		/// </summary>
		protected internal int ConnectionNumber {
			get { return _connectionNumber; }
		}

		#endregion


		#region Events

		private EventHandler _configure;
		private static SafeNativeMethods.LgLcdOnNotificationCallback _notificationCallback;
		private static SafeNativeMethods.LgLcdOnConfigureCallback _configCallback;

		/// <summary>
		/// Occurs when the Configure button in the LCD Monitor configuration is clicked.
		/// Note that suscribing or unsuscribing to this event must be done before calling <see cref="Connect"/>.
		/// Note that this event is raised in the context of a thread within the library.
		/// Therefore, take the necessary precautions for thread safety.
		/// </summary>
		public event EventHandler Configure {
			add {
				VerifyNotConnected();
				_configure = (EventHandler) Delegate.Combine(_configure, value);
			}
			remove {
				VerifyNotConnected();
				_configure = (EventHandler) Delegate.Remove(_configure, value);
			}
		}

		/// <summary>
		/// Occurs when at least one device that the applet supports (as indicated by <see cref="Capabilities"/>)
		/// arrives in the system. A device arrival can also be triggered when the user enables a device that
		/// was previously disabled.
		/// Note that this event is raised in the context of a thread within the library.
		/// Therefore, take the necessary precautions for thread safety.
		/// </summary>
		public event EventHandler<LcdDeviceTypeEventArgs> DeviceArrival;

		/// <summary>
		/// Occurs when the user disables all devices of a given type, or when all physical devices of a given
		/// type have been unplugged.
		/// Note that this event is raised in the context of a thread within the library.
		/// Therefore, take the necessary precautions for thread safety.
		/// </summary>
		public event EventHandler<LcdDeviceTypeEventArgs> DeviceRemoval;

		/// <summary>
		/// Occurs when the applet is enabled or disabled using the Progams configuration panel on the LCD Manager
		/// (LCDMon) application program. The global enable and disable functions are initiated by the user on this
		/// configuration screen.
		/// Note that this event is raised in the context of a thread within the library.
		/// Therefore, take the necessary precautions for thread safety.
		/// </summary>
		public event EventHandler IsEnabledChanged;

		/// <summary>
		/// Occurs when the applet’s connection to the LCD Manager application is disrupted for any reason.
		/// Note that this event is raised in the context of a thread within the library.
		/// Therefore, take the necessary precautions for thread safety.
		/// </summary>
		public event EventHandler ConnectionDisrupted;

		/// <summary>
		/// Raises the <see cref="Configure"/> event.
		/// </summary>
		protected virtual void OnConfigure() {
			EventHandler configure = _configure;
			if (configure != null)
				configure(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the <see cref="DeviceArrival"/> event.
		/// </summary>
		/// <param name="deviceType">The device type that has arrived.</param>
		protected virtual void OnDeviceArrival(LcdDeviceType deviceType) {
			EventHandler<LcdDeviceTypeEventArgs> handler = DeviceArrival;
			if (handler != null)
				handler(this, new LcdDeviceTypeEventArgs(deviceType));
		}

		/// <summary>
		/// Raises the <see cref="DeviceRemoval"/> event.
		/// </summary>
		/// <param name="deviceType">The device type that was removed.</param>
		protected virtual void OnDeviceRemoval(LcdDeviceType deviceType) {
			EventHandler<LcdDeviceTypeEventArgs> handler = DeviceRemoval;
			if (handler != null)
				handler(this, new LcdDeviceTypeEventArgs(deviceType));
		}

		/// <summary>
		/// Raises the <see cref="IsEnabledChanged"/> event.
		/// </summary>
		protected virtual void OnIsEnabledChanged() {
			EventHandler handler = IsEnabledChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the <see cref="ConnectionDisrupted"/> event.
		/// </summary>
		protected virtual void OnConnectionDisrupted() {
			EventHandler handler = ConnectionDisrupted;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		#endregion
        

		#region Verifications

		/// <summary>
		/// Ensures that we are connected; throw an exception otherwise.
		/// </summary>
		private void VerifyConnected() {
			VerifyValid();
			if (!IsConnected)
				throw new InvalidOperationException("The applet must be connected to call this method.");
		}

		/// <summary>
		/// Ensures that we are not connected yet; throw an exception otherwise.
		/// </summary>
		private void VerifyNotConnected() {
			VerifyValid();
			if (IsConnected)
				throw new InvalidOperationException("You can only change members of this class before Connect() is called.");
		}

		/// <summary>
		/// Ensures that the object is not disposed.
		/// </summary>
		private void VerifyValid() {
			if (_isDisposed)
				throw new InvalidOperationException("This method cannot be called after the applet has been disposed.");
		}

		#endregion


		/// <summary>
		/// Establishes a connection to the LCD monitor process.
		/// </summary>
		public void Connect() {
			VerifyNotConnected();
			EventHandler configureHandler = _configure;
			SafeNativeMethods.LgLcdConnectContextEx context = new SafeNativeMethods.LgLcdConnectContextEx {
				AppFriendlyName = _friendlyName,
				IsPersistent = false,
				IsAutoStartable = _isAutoStartable,
				ConfigCallback = configureHandler != null ? _configCallback : null,
				ConfigContext = IntPtr.Zero,
				Connection = -1,
				AppletCapabilitiesSupported = _capabilities,
				Reserved = 0,
				NotificationCallback = _notificationCallback,
				NotificationContext = IntPtr.Zero
			};
			_connectionNumber = SafeNativeMethods.LgLcdConnectEx(context);
		}

		/// <summary>
		/// Close an existing connection to the LCD monitor process.
		/// </summary>
		public void Disconnect() {
			VerifyConnected();
			Dispose();
		}

		/// <summary>
		/// Opens a device of a given type.
		/// This function is generally called after a <see cref="DeviceArrival" /> event has been received.
		/// </summary>
		/// <param name="deviceType">Type of the device to open.</param>
		public LcdDevice OpenDeviceByType(LcdDeviceType deviceType) {
			VerifyConnected();
			switch (deviceType) {
				case LcdDeviceType.Monochrome:
					return new LcdDeviceMonochrome(this);
				case LcdDeviceType.Qvga:
					return new LcdDeviceQvga(this);
				default:
					throw new NotSupportedException(deviceType + " is not a supported device type.");
			}
		}

		private int ConfigCallback(int connection, IntPtr context) {
			OnConfigure();
			return 0;
		}

		private int NotificationCallback(int connection, IntPtr context, SafeNativeMethods.NotificationCode notificationCode, int param1, int param2, int param3, int param4) {
			switch (notificationCode) {
				case SafeNativeMethods.NotificationCode.DeviceArrival:
					OnDeviceArrival((LcdDeviceType) param1);
					break;
				case SafeNativeMethods.NotificationCode.DeviceRemoval:
					OnDeviceRemoval((LcdDeviceType) param1);
					break;
				case SafeNativeMethods.NotificationCode.AppletEnabled:
					_isEnabled = true;
					OnIsEnabledChanged();
					break;
				case SafeNativeMethods.NotificationCode.AppletDisabled:
					_isEnabled = false;
					OnIsEnabledChanged();
					break;
                case SafeNativeMethods.NotificationCode.CloseConnection:
					_connectionNumber = -1;
					OnConnectionDisrupted();
					break;
			}
			return 0;
		}


		#region IDisposable

		/// <summary>
		/// Releases the resources associated with this <see cref="LcdApplet"/>.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources associated with this <see cref="LcdApplet"/>.
		/// </summary>
		/// <param name="disposing">Whether to also release managed resources along with unmanaged ones.</param>
		protected virtual void Dispose(bool disposing) {
			if (disposing && !_isDisposed) {
				_configure = null;
				if (_connectionNumber != -1) {
					SafeNativeMethods.LgLcdDisconnect(_connectionNumber);
					_connectionNumber = -1;
				}
				_isDisposed = true;
				if (Interlocked.Decrement(ref _initCount) == 0)
					SafeNativeMethods.LgLcdDeInit();
			}
		}

		#endregion

        
		#region Constructors

		/// <summary>
		/// Creates a new instance of <see cref="LcdApplet"/> that support both monochrome and QVGA devices.
		/// </summary>
		public LcdApplet()
			: this(String.Empty, LcdAppletCapabilities.Both, false) {
		}

		/// <summary>
		/// Creates a new instance of <see cref="LcdApplet"/> that support both monochrome and QVGA devices,
		/// with the specified friendly name.
		/// </summary>
		/// <param name="friendlyName">Friendly name of the applet that is displayed in LCD Monitor.</param>
		public LcdApplet(string friendlyName)
			: this(friendlyName, LcdAppletCapabilities.Both, false) {
		}

		/// <summary>
		/// Creates a new instance of <see cref="LcdApplet"/> with the specified friendly name and capabilities.
		/// </summary>
		/// <param name="friendlyName">Friendly name of the applet that is displayed in LCD Monitor.</param>
		/// <param name="capabilities">Gets or sets the capabilities that this applet supports.</param>
		public LcdApplet(string friendlyName, LcdAppletCapabilities capabilities)
			: this(friendlyName, capabilities, false) {
		}

		/// <summary>
		/// Creates a new instance of <see cref="LcdApplet"/> with the specified friendly name, capabilities and auto-startable flag.
		/// </summary>
		/// <param name="friendlyName">Friendly name of the applet that is displayed in LCD Monitor.</param>
		/// <param name="capabilities">Gets or sets the capabilities that this applet supports.</param>
		/// <param name="isAutoStartable">Whether this applet has the option to be auto-startable.</param>
		public LcdApplet(string friendlyName, LcdAppletCapabilities capabilities, bool isAutoStartable) {
			if (friendlyName == null)
				throw new ArgumentNullException("friendlyName");
			new LgLcdPermission(PermissionState.Unrestricted).Demand();
			if (Interlocked.Increment(ref _initCount) == 1)
				SafeNativeMethods.LgLcdInit();
			_friendlyName = friendlyName;
			_isAutoStartable = isAutoStartable;
			_capabilities = capabilities;
			_configCallback = ConfigCallback;
			_notificationCallback = NotificationCallback;
		}

		#endregion


	}

}