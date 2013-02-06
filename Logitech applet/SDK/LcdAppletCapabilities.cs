using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Flags indicating which capabilities the applet supports.
	/// </summary>
	[Flags]
	public enum LcdAppletCapabilities {

		/// <summary>
		/// This flag indicates that the applet supports 160 by 43 monochrome screen (G13/G15/Z10).
		/// </summary>
		Monochrome = 1,

		/// <summary>
		/// This flag indicates that the applet supports 320 by 240 color screen (G19).
		/// </summary>
		Qvga = 2,

		/// <summary>
		/// This flag indicates that the applet supports both 160 by 43 monochrome screen (G13/G15/Z10)
		/// and 320 by 240 color screen (G19).
		/// </summary>
		Both = Monochrome | Qvga

	}

}