using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Event arguments for event that notify about drawing completed.
	/// </summary>
	public class DrawnEventArgs : EventArgs {
		private readonly byte[] _pixels;

		/// <summary>
		/// Gets the pixels array that will be used to update the device.
		/// Note that you can modify this array here in order to do some sort of post-processing on the pixels.
		/// </summary>
		public byte[] Pixels {
			get { return _pixels; }
		}

		/// <summary>
		/// Creates a new instance of <see cref="DrawnEventArgs"/> with the specified pixels array.
		/// </summary>
		/// <param name="pixels">Pixels array that will be used to update the device.</param>
		public DrawnEventArgs(byte[] pixels) {
			_pixels = pixels;
		}
	}
}