using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Flags listing all the soft buttons, used to know which buttons are pressed.
	/// </summary>
	[Flags]
	public enum LcdSoftButtons {

		/// <summary>
		/// No buttons are pressed.
		/// </summary>
		None = 0,

		/// <summary>
		/// The first soft button on a G13/G15/Z10 is pressed.
		/// </summary>
		Button0 = 0x00000001,

		/// <summary>
		/// The second soft button on a G13/G15/Z10 is pressed.
		/// </summary>
		Button1 = 0x00000002,

		/// <summary>
		/// The third soft button on a G13/G15/Z10 is pressed.
		/// </summary>
		Button2 = 0x00000004,

		/// <summary>
		/// The fourth soft button on a G13/G15/Z10 is pressed.
		/// </summary>
		Button3 = 0x00000008,

		/// <summary>
		/// The fifth soft button is pressed. Not present on current devices.
		/// </summary>
		Button4 = 0x00000010,

		/// <summary>
		/// The sixth soft button is pressed. Not present on current devices.
		/// </summary>
		Button5 = 0x00000020,

		/// <summary>
		/// The seventh soft button is pressed. Not present on current devices.
		/// </summary>
		Button6 = 0x00000040,

		/// <summary>
		/// The eight soft button is pressed. Not present on current devices.
		/// </summary>
		Button7 = 0x00000080,

		/// <summary>
		/// The left arrow button on a G19 is pressed.
		/// </summary>
		Left = 0x00000100,

		/// <summary>
		/// The right arrow button on a G19 is pressed.
		/// </summary>
		Right = 0x00000200,

		/// <summary>
		/// The Ok button on a G19 is pressed.
		/// </summary>
		Ok = 0x00000400,

		/// <summary>
		/// The Cancel button on a G19 is pressed.
		/// </summary>
		Cancel = 0x00000800,

		/// <summary>
		/// The up arrow button on a G19 is pressed.
		/// </summary>
		Up = 0x00001000,

		/// <summary>
		/// The down arrow button on a G19 is pressed.
		/// </summary>
		Down = 0x00002000,

		/// <summary>
		/// The Menu button on a G19 is pressed.
		/// </summary>
		Menu = 0x00004000,

		/// <summary>
		/// The Light button is pressed.
		/// </summary>
		Light = 0x40000000

	}

}