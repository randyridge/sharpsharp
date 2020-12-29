using NetVips;

namespace SharpSharp {
	public sealed record TiffOptions {
		public int? BitDepth { get; set; } = null;

		public string Compression { get; set; } = Enums.ForeignTiffCompression.Jpeg;

		public string Predictor { get; set; } = Enums.ForeignTiffPredictor.Horizontal;

		public bool Pyramid { get; set; } = false;

		public int Quality { get; set; } = 80;

		public bool Tile { get; set; }

		public int TileHeight { get; set; } = 256;

		public int TileWidth { get; set; } = 256;

		public double XRes { get; set; } = 1.0;

		public double YRes { get; set; } = 1.0;
	}
}
