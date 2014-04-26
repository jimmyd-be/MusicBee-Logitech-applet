using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Event arguments for event that notify about pressing/releasing soft buttons.
	/// </summary>
	public class LcdSoftButtonsEventArgs : EventArgs {
		private readonly LcdSoftButtons _softButtons;

		/// <summary>
		/// Gets the <see cref="LcdSoftButtons"/> represented by these arguments.
		/// </summary>
		public LcdSoftButtons SoftButtons {
			get { return _softButtons; }
		}

		/// <summary>
		/// Creates a new instance of <see cref="LcdSoftButtonsEventArgs"/> with the specified soft buttons.
		/// </summary>
		/// <param name="softButtons"><see cref="LcdSoftButtons"/> represented by these arguments.</param>
		public LcdSoftButtonsEventArgs(LcdSoftButtons softButtons) {
			_softButtons = softButtons;
		}
	}
}