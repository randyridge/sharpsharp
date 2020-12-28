using NetVips;

namespace SharpSharp {
	public sealed record HeifOptions {
		public string Compression { get; set; } = Enums.ForeignHeifCompression.Av1;

		public int Quality { get; set; } = 50;

		public int Speed { get; set; } = 5;

		public bool UseLossless { get; set; } = false;
	}
}
