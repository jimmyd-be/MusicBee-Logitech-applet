using System;
using System.Drawing;
using System.Drawing.Text;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a simple text on a <see cref="LcdGdiPage"/> page.
	/// This class defaults <see cref="LcdGdiObject.Brush"/> to <see cref="Brushes.Black"/>.
	/// <see cref="LcdGdiObject.Size"/> is used only if you want to clip or wrap the text:
	/// the default is (0,0) and means that the text will be drawn without constraints.
	/// </summary>
	public class LcdGdiText : LcdGdiObject {


		#region Properties

		private string _text;
		private Font _font;
		private StringFormat _stringFormat;
		private int _textContrast = 4;
		private TextRenderingHint _textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
		private SizeF _boundSize;
        
		/// <summary>
		/// Gets or sets the text to draw.
		/// </summary>
		public string Text {
			get { return _text; }
			set {
				if (_text != value) {
					_text = value;
					HasChanged = true;
				}
			}
		}
        
		/// <summary>
		/// Gets or sets the <see cref="Font"/> that defines the text format of the string.
		/// </summary>
		public Font Font {
			get { return _font; }
			set {
				if (_font != value) {
					_font = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="StringFormat"/> that specifies formatting attributes,
		/// such as line spacing and alignment, that are applied to the drawn text.
		/// </summary>
		public StringFormat StringFormat {
			get { return _stringFormat; }
			set {
				_stringFormat = value ?? new StringFormat(StringFormat.GenericDefault);
				HasChanged = true;
			}
		}

		/// <summary>
		/// Gets or sets the gamma correction value for rendering text.
		/// The default value is 4.
		/// </summary>
		/// <seealso cref="Graphics.TextContrast"/>.
		public int TextContrast {
			get { return _textContrast; }
			set {
				if (_textContrast != value) {
					_textContrast = value;
					HasChanged = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the rendering mode for text.
		/// The default value is <see cref="System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit"/>.
		/// </summary>
		/// <seealso cref="Graphics.TextRenderingHint"/>.
		public TextRenderingHint TextRenderingHint {
			get { return _textRenderingHint; }
			set {
				if (_textRenderingHint != value) {
					_textRenderingHint = value;
					HasChanged = true;
				}
			}
		}

		#endregion


		/// <summary>
		/// Updates the position of the object.
		/// </summary>
		/// <param name="elapsedTotalTime">Time elapsed since the device creation.</param>
		/// <param name="elapsedTimeSinceLastFrame">Time elapsed since last frame update.</param>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Update(TimeSpan elapsedTotalTime, TimeSpan elapsedTimeSinceLastFrame, LcdGdiPage page, Graphics graphics) {
			if (String.IsNullOrEmpty(Text) || Brush == null || Font == null)
				FinalSize = SizeF.Empty;
			else {
				graphics.TextContrast = _textContrast;
				graphics.TextRenderingHint = _textRenderingHint;
				_stringFormat.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, _text.Length) });
				_boundSize = Size == SizeF.Empty ? new SizeF(65536.0f, 65536.0f) : Size;
				if (HorizontalAlignment == LcdGdiHorizontalAlignment.Stretch)
					_boundSize.Width = page.Bitmap.Width - Margin.Left - Margin.Right;
				if (VerticalAlignment == LcdGdiVerticalAlignment.Stretch)
					_boundSize.Height = page.Bitmap.Height - Margin.Top - Margin.Bottom;
				Region[] regions = graphics.MeasureCharacterRanges(Text, Font, new RectangleF(PointF.Empty, _boundSize), _stringFormat);
				FinalSize = regions[0].GetBounds(graphics).Size;
			}
			CalcAbsolutePosition(page.Bitmap.Size, 1.0f);
		}

		/// <summary>
		/// Draws the text.
		/// </summary>
		/// <param name="page">Page where this object will be drawn.</param>
		/// <param name="graphics"><see cref="Graphics"/> to use for drawing.</param>
		protected internal override void Draw(LcdGdiPage page, Graphics graphics) {
			if (String.IsNullOrEmpty(Text) || Brush == null || Font == null)
				return;
			graphics.TextContrast = _textContrast;
			graphics.TextRenderingHint = _textRenderingHint;
			graphics.DrawString(_text, _font, Brush, new RectangleF(AbsolutePosition, _boundSize), _stringFormat);
		}


		/// <summary>
		/// Releases the resources associated with this <see cref="LcdGdiText"/>.
		/// </summary>
		/// <param name="disposing">Whether to also release managed resources along with unmanaged ones.</param>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (_font != null)
					_font.Dispose();
				if (_stringFormat != null)
					_stringFormat.Dispose();
			}
		}


		/// <summary>
		/// Creates a new <see cref="LcdGdiText"/> without text and a default generic font.
		/// </summary>
		public LcdGdiText()
			: this(null) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiText"/> with the given text and a default generic font.
		/// </summary>
		/// <param name="text">Text to draw.</param>
		public LcdGdiText(string text)
			: this(text, new Font(FontFamily.GenericSansSerif, 7.0f)) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiText"/> with the given text and font.
		/// </summary>
		/// <param name="text">Text to draw.</param>
		/// <param name="font"><see cref="Font"/> that defines the text format of the string.</param>
		public LcdGdiText(string text, Font font)
			: this(text, font, new StringFormat(StringFormat.GenericDefault)) {
		}

		/// <summary>
		/// Creates a new <see cref="LcdGdiText"/> with the given text, font and string format.
		/// </summary>
		/// <param name="text">Text to draw.</param>
		/// <param name="font"><see cref="Font"/> that defines the text format of the string.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> that specifies formatting attributes, such as line
		/// spacing and alignment, that are applied to the drawn text.</param>
		public LcdGdiText(string text, Font font, StringFormat stringFormat) {
			Brush = Brushes.Black;
			_text = text;
			_font = font;
			_stringFormat = stringFormat;
		}
	}

}