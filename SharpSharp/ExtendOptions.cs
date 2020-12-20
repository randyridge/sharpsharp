using RandyRidge.Common;

namespace SharpSharp {
	public sealed class ExtendOptions {
		public ExtendOptions(int pixels) : this(pixels, pixels, pixels, pixels) {
		}

		public ExtendOptions(int top, int bottom, int left, int right) {
			Top = Guard.MinimumInclusive(top, 0, nameof(top));
			Bottom = Guard.MinimumInclusive(bottom, 0, nameof(bottom));
			Left = Guard.MinimumInclusive(left, 0, nameof(left));
			Right = Guard.MinimumInclusive(right, 0, nameof(right));
		}

		public int Bottom { get; }

		public int Left { get; }

		public int Right { get; }

		public int Top { get; }
	}
}
