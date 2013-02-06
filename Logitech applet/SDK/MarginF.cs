using System;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Describes a margin around an element.
	/// </summary>
	public struct MarginF : IEquatable<MarginF> {

		/// <summary>
		/// Gets or sets the left margin.
		/// </summary>
		public float Left { get; set; }
		
		/// <summary>
		/// Gets or sets the top margin.
		/// </summary>
		public float Top { get; set; }
		
		/// <summary>
		/// Gets or sets the right margin.
		/// </summary>
		public float Right { get; set; }
		
		/// <summary>
		/// Gets or sets the bottom margin.
		/// </summary>
		public float Bottom { get; set; }


		#region Equality Members

		/// <summary>
		/// Indicates whether the current <see cref="MarginF"/> is equal to another <see cref="MarginF"/>.
		/// </summary>
		/// <param name="other">An <see cref="MarginF"/> to compare with this <see cref="MarginF"/>.</param>
		/// <returns><c>true</c> if the current <see cref="MarginF"/> is equal to the <paramref name="other"/>
		/// parameter; otherwise, <c>false</c>.</returns>
		public bool Equals(MarginF other) {
			return other.Left == Left && other.Top == Top && other.Right == Right && other.Bottom == Bottom;
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns><c>true</c> if <paramref name="obj"/> and this instance are the same type
		/// and represent the same value; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj) {
			return (obj is MarginF) && Equals((MarginF) obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode() {
			return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
		}

		/// <summary>
		/// Indicates whether two <see cref="MarginF"/> are equal.
		/// </summary>
		/// <param name="left">First <see cref="MarginF"/>.</param>
		/// <param name="right">Second <see cref="MarginF"/>.</param>
		/// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/>
		/// are equal; <c>false</c> otherwise.</returns>
		public static bool operator ==(MarginF left, MarginF right) {
			return left.Equals(right);
		}

		/// <summary>
		/// Indicates whether two <see cref="MarginF"/> are different.
		/// </summary>
		/// <param name="left">First <see cref="MarginF"/>.</param>
		/// <param name="right">Second <see cref="MarginF"/>.</param>
		/// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/>
		/// are different; <c>false</c> otherwise.</returns>
		public static bool operator !=(MarginF left, MarginF right) {
			return !left.Equals(right);
		}

		#endregion


		/// <summary>
		/// Returns a readable representation of this <see cref="MarginF"/>.
		/// </summary>
		/// <returns>A readable representation of this <see cref="MarginF"/>.</returns>
		public override string ToString() {
			return String.Format("{0};{1};{2};{3}", Left, Top, Right, Bottom);
		}
        

		/// <summary>
		/// Creates a new <see cref="MarginF"/> with an uniform margin for every side.
		/// </summary>
		/// <param name="uniformMargin">Uniform margin for every side.</param>
		public MarginF(float uniformMargin)
			: this() {
			Left = Right = Top = Bottom = uniformMargin;
		}

		/// <summary>
		/// Creates a new <see cref="MarginF"/> with the specified margin for left and right,
		/// and another specified margin for top and bottom.
		/// </summary>
		/// <param name="horizontalMargin">Left and right margin.</param>
		/// <param name="verticalMargin">Top and bottom margin.</param>
		public MarginF(float horizontalMargin, float verticalMargin)
			: this() {
			Left = Right = horizontalMargin;
			Top = Bottom = verticalMargin;
		}

		/// <summary>
		/// Creates a new <see cref="MarginF"/> with the specified left, top, right and bottom margins.
		/// </summary>
		/// <param name="left">Left margin.</param>
		/// <param name="top">Top margin.</param>
		/// <param name="right">Right margin.</param>
		/// <param name="bottom">Bottom margin.</param>
		public MarginF(float left, float top, float right, float bottom)
			: this() {
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}
	}

}