namespace SharpSharp {
	public sealed record TrimOptions {
		public double TrimThreshold { get; set; }

		public int TrimOffsetLeft { get; set; }

		public int TrimOffsetTop { get; set; }
	}
}
