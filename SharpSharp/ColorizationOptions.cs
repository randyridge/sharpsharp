namespace SharpSharp {
	public sealed class ColorizationOptions {
		public ColorizationOptions(bool makeGrayscale = false) {
			MakeGrayscale = makeGrayscale;
		}

		public bool MakeGrayscale { get; internal set; }
	}
}
