namespace GammaJul.LgLcd {

	/// <summary>
	/// This enumeration lists every possible priority in a screen update.
	/// </summary>
	public enum LcdPriority {

		/// <summary>
		/// Lowest priority, disable displaying. Use this priority when you don’t have anything to show.
		/// </summary>
		IdleNoShow = 0,

		/// <summary>
		/// Priority used for low priority items.
		/// </summary>
		Background = 64,

		/// <summary>
		/// Normal priority, to be used by most applications most of the time.
		/// </summary>
		Normal = 128,

		/// <summary>
		/// Highest priority. To be used only for critical screens, such as “your CPU temperature is too high”.
		/// </summary>
		Alert = 255

	}

}