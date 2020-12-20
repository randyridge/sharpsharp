using System;

namespace SharpSharp {
	public sealed class SharpSharpException : Exception {
		public SharpSharpException(string message) : base(message) {
		}

		public SharpSharpException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}
