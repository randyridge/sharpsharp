using System;
using System.Collections.Generic;

namespace SharpSharp {
	internal sealed class InputDescriptor {
		public byte[]? Buffer { get; set; }

		public int BufferLength { get; set; }

		public List<double> CreateBackground { get; set; } = new() {
			0.0,
			0.0,
			0.0,
			255.0
		};

		public int CreateChannels { get; set; }

		public int CreateHeight { get; set; }

		public int CreateWidth { get; set; }

		public double Density { get; set; } = 72.0;

		public bool FailOnError { get; set; } = true;

		public string? File { get; set; }

		public string? Name { get; set; }

		public int Page { get; set; }

		public int Pages { get; set; } = 1;

		public int RawChannels { get; set; }

		public int RawHeight { get; set; }

		public int RawWidth { get; set; }

		internal static InputDescriptor FromBuffer(byte[] buffer) {
			var result = new InputDescriptor {
				Buffer = buffer,
				FailOnError = true
			};
			return result;
		}

		internal static InputDescriptor FromCreate(int width, int height, int channels, int red, int green, int blue, int alpha) {
			if(width < 0) {
				throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
			}

			if(height < 0) {
				throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
			}

			if(channels != 3 || channels != 4) {
				throw new ArgumentOutOfRangeException(nameof(channels), "Channels must be 3 or 4.");
			}

			if(red < 0 || red > 255) {
				throw new ArgumentOutOfRangeException(nameof(red), "Red must be 0 to 255.");
			}

			if(green < 0 || green > 255) {
				throw new ArgumentOutOfRangeException(nameof(green), "Green must be 0 to 255.");
			}

			if(blue < 0 || blue > 255) {
				throw new ArgumentOutOfRangeException(nameof(blue), "Blue must be 0 to 255.");
			}

			if(alpha < 0 || alpha > 255) {
				throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha must be 0 to 255.");
			}

			var result = new InputDescriptor {
				CreateWidth = width,
				CreateHeight = height,
				CreateChannels = channels,
				FailOnError = true,
				CreateBackground = new List<double> {
					red,
					green,
					blue,
					alpha
				}
			};
			return result;
		}

		internal static InputDescriptor FromFile(string fileName) {
			var result = new InputDescriptor {
				FailOnError = true,
				File = fileName
			};
			return result;
		}

		internal static InputDescriptor FromRaw(int width, int height, int channels) {
			if(width < 0) {
				throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
			}

			if(height < 0) {
				throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
			}

			if(channels < 1 || channels > 4) {
				throw new ArgumentOutOfRangeException(nameof(channels), "Channels must be 1 to 4.");
			}

			var result = new InputDescriptor {
				Buffer = Array.Empty<byte>(),
				FailOnError = true,
				RawWidth = width,
				RawHeight = height,
				RawChannels = channels
			};
			return result;
		}
	}
}
