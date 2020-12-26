using System;

namespace SharpSharp {
	public sealed record AnimationOptions {
		public int PageHeight { get; set; } = 0;

		public int[] Delay { get; set; } = Array.Empty<int>();

		public int Loop { get; set; } = -1;
	}
}
