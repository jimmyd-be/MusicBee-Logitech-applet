using System;
using System.Drawing;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Event arguments for the <see cref="LcdGdiPage.GdiDrawing"/> event.
	/// </summary>
	public class GdiDrawingEventArgs : EventArgs {
		private readonly Graphics _graphics;

		/// <summary>
		/// Gets the <see cref="Graphics"/> to use to draw on the page.
		/// </summary>
		public Graphics Graphics {
			get { return _graphics; }
		}

		/// <summary>
		/// Creates a new instance of <see cref="DrawnEventArgs"/> with the specified graphics.
		/// </summary>
		/// <param name="graphics"><see cref="Graphics"/> to use to draw on the page.</param>
		public GdiDrawingEventArgs(Graphics graphics) {
			_graphics = graphics;
		}
	}
}