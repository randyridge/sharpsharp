using RandyRidge.Common;

namespace SharpSharp.Pipeline {
	public sealed class SharpenOptions {
		public SharpenOptions(double? sigma = null, double? flat = null, double? jagged = null) {
			if(!sigma.HasValue) {
				Sigma = -1;
			}
			else {
				Sigma = Guard.RangeInclusive(sigma.Value, 0.01, 10000, nameof(sigma));
			}

			if(flat.HasValue) {
				Flat = Guard.RangeInclusive(flat.Value, 0, 10000, nameof(flat));
			}
			else {
				Flat = 1;
			}

			if(jagged.HasValue) {
				Jagged = Guard.RangeInclusive(jagged.Value, 0, 10000, nameof(jagged));
			}
			else {
				Jagged = 2;
			}
		}

		public double? Flat { get; }

		public double? Jagged { get; }

		public double Sigma { get; }
	}
}
