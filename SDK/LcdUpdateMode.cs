namespace GammaJul.LgLcd {

	/// <summary>
	/// Specify how the screen will be updated.
	/// </summary>
	public enum LcdUpdateMode {

		/// <summary>
		/// The screen will be updated asynchronously. This is the default.
		/// </summary>
		Async = 0,

		/// <summary>
		/// The screen will be updated synchronously. Note that this take about 30ms.
		/// </summary>
		Sync = -2147483648, // 0x80000000

		/// <summary>
		/// The screen will be updated synchronously. If the screen hasn't been updated after 30ms,
		/// <see cref="LcdDevice.UpdateBitmap"/> will return <c>false</c>.
		/// </summary>
		SyncCompleteWithinFrame = -1073741824 // 0xC0000000

	}

}