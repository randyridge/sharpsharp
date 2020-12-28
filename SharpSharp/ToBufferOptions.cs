using System;

namespace SharpSharp {
	/// <summary>
	///   Options for writing image to a buffer.
	/// </summary>
	public sealed class ToBufferOptions {
		/// <summary>
		///   Initializes an instance of the options for writing to a buffer.
		/// </summary>
		/// <param name="buffer">
		///   The buffer to write to.
		/// </param>
		/// <param name="callback">
		///   The call back, if any, to call with the <see cref="OutputInfo" /> for the image written.
		/// </param>
		public ToBufferOptions(byte[] buffer, Action<OutputInfo>? callback) {
			Buffer = buffer;
			Callback = callback;
		}

		/// <summary>
		///   The buffer to write to.
		/// </summary>
		public byte[] Buffer { get; internal set; }

		/// <summary>
		///   Optional callback to get <see cref="OutputInfo" />.
		/// </summary>
		public Action<OutputInfo>? Callback { get; }
	}
}
