using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Event arguments used when updating a page.
	/// </summary>
	public class UpdateEventArgs : EventArgs {
		private readonly TimeSpan _elapsedTotalTime;
		private readonly TimeSpan _elapsedTimeSinceLastFrame;

		/// <summary>
		/// Gets the total time elapsed since the creation of the device.
		/// </summary>
		public TimeSpan ElapsedTotalTime {
			get { return _elapsedTotalTime; }
		}

		/// <summary>
		/// Gets the time elapsed since the last frame update on this page.
		/// </summary>
		public TimeSpan ElapsedTimeSinceLastFrame {
			get { return _elapsedTimeSinceLastFrame; }
		}

		/// <summary>
		/// Creates a new instance of <see cref="UpdateEventArgs"/> with the given elapsed total time
		/// and elapsed time since last frame.
		/// </summary>
		/// <param name="elapsedTotalTime">Total time elapsed since the creation of the device.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since the last frame update on this page.</param>
		public UpdateEventArgs(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame) {
			_elapsedTotalTime = elapsedTotalTime;
			_elapsedTimeSinceLastFrame = elapsedTimeSinceLastFrame;
		}
	}

}
