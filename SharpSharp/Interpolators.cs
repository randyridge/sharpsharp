using NetVips;

namespace SharpSharp {
	public static class Interpolators {
		public static Interpolate Bicubic { get; } = Interpolate.NewFromName("bicubic");

		public static Interpolate Bilinear { get; } = Interpolate.NewFromName("bilinear");

		public static Interpolate LocallyBoundedBicubic { get; } = Interpolate.NewFromName("lbb");

		public static Interpolate Nearest { get; } = Interpolate.NewFromName("nearest");

		public static Interpolate VertexSplitQuadraticBasisSpline { get; } = Interpolate.NewFromName("vsqbs");
	}
}
