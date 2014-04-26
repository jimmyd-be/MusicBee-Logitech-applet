using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Event arguments for event that notify about a given device type.
	/// </summary>
	public class LcdDeviceTypeEventArgs : EventArgs {
		private readonly LcdDeviceType _deviceType;

		/// <summary>
		/// Gets the <see cref="LcdDeviceType"/> represented by these arguments.
		/// </summary>
		public LcdDeviceType DeviceType {
			get { return _deviceType; }
		}

		/// <summary>
		/// Creates a new instance of <see cref="LcdDeviceTypeEventArgs"/> with the specified device type.
		/// </summary>
		/// <param name="deviceType"><see cref="LcdDeviceType"/> represented by these arguments.</param>
		public LcdDeviceTypeEventArgs(LcdDeviceType deviceType) {
			_deviceType = deviceType;
		}
	}
}