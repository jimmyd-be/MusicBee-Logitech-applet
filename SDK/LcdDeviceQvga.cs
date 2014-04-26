using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Encapsulates a 320x240 color LCD device.
	/// </summary>
	public sealed class LcdDeviceQvga : LcdDevice {

		/// <summary>
		/// Gets the width of this device, in pixels.
		/// </summary>
		public override int PixelWidth {
			get { return SafeNativeMethods.BmpQvgaWidth; }
		}

		/// <summary>
		/// Gets the height of this device, in pixels.
		/// </summary>
		public override int PixelHeight {
			get { return SafeNativeMethods.BmpQvgaHeight; }
		}

		/// <summary>
		/// Gets the number of bits per pixel (aka color depth) of this device.
		/// </summary>
		public override int BitsPerPixel {
			get { return SafeNativeMethods.BmpQvgaBpp; }
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
		protected override bool UpdateBitmapCore(byte[] pixels, LcdPriority priority, LcdUpdateMode updateMode) {
			return SafeNativeMethods.LgLcdUpdateBitmapQvga(DeviceNumber, pixels, priority, updateMode);
		}

		/// <summary>
		/// Creates a new instance of <see cref="LcdDeviceMonochrome"/> for the given applet.
		/// </summary>
		/// <param name="applet"><see cref="LcdApplet"/> that opened this device.</param>
		internal LcdDeviceQvga(LcdApplet applet)
			: base(applet, LcdDeviceType.Qvga) {
		}
	}

}