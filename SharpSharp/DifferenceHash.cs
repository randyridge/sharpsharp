using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace SharpSharp {
	/// <summary>
	///   Provides a difference gradient hash.
	/// </summary>
	/// <remarks>
	///   See <see href="http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html" />.
	/// </remarks>
	public static class DifferenceHash {
		/// <summary>
		///   Calculates a 64 bit difference hash.
		/// </summary>
		/// <param name="buffer">
		///   The image buffer to hash.
		/// </param>
		/// <returns>
		///   A 64 bit difference hash.
		/// </returns>
		public static long HashLong(byte[] buffer) {
			using var ms = GlobalStatics.RecyclableMemoryStreamManager.GetStream();
			ms.Read(buffer);
			return HashLong(ms);
		}

		/// <summary>
		///   Calculates a 64 bit difference hash.
		/// </summary>
		/// <param name="stream">
		///   The image stream to hash.
		/// </param>
		/// <returns>
		///   A 64 bit difference hash.
		/// </returns>
		public static long HashLong(Stream stream) {
			const int Width = 9;
			const int Height = 8;
			// TODO: allocations

			ImagePipeline
				.FromStream(stream)
				.Resize(new ResizeOptions {
					Canvas = Canvas.IgnoreAspectRatio,
					Height = Height,
					Width = Width,
				})
				.Grayscale()
				.Normalize()
				.Raw()
				.ToBuffer(out var normalized);

			var bitArray = new BitArray(64);
			var bit = 0;
			for(var x = 0; x < Width - 1; x++) {
				for(var y = 0; y < Height; y++) {
					bitArray.Set(bit, normalized[y * Height + x] < normalized[y * Height + x + 1]);
					bit++;
				}
			}

			var array = new byte[8];
			bitArray.CopyTo(array, 0);
			return BitConverter.ToInt64(array, 0);
		}

		/// <summary>
		///   Calculates a 16 bit difference hash.
		/// </summary>
		/// <param name="buffer">
		///   The image buffer to hash.
		/// </param>
		/// <returns>
		///   A 16 bit difference hash.
		/// </returns>
		public static short HashShort(byte[] buffer) {
			using var ms = GlobalStatics.RecyclableMemoryStreamManager.GetStream();
			ms.Read(buffer);
			return HashShort(ms);
		}

		/// <summary>
		///   Calculates a 16 bit difference hash.
		/// </summary>
		/// <param name="stream">
		///   The image stream to hash.
		/// </param>
		/// <returns>
		///   A 16 bit difference hash.
		/// </returns>
		public static short HashShort(Stream stream) {
			const int Width = 6;
			const int Height = 4;
			// TODO: allocations

			ImagePipeline
				.FromStream(stream)
				.Resize(new ResizeOptions {
					Canvas = Canvas.IgnoreAspectRatio,
					Height = Height,
					Width = Width,
				})
				.Grayscale()
				.Normalize()
				.Raw()
				.ToBuffer(out var normalized);

			var bitArray = new BitArray(16);
			var bit = 0;
			for(var x = 0; x < Width - 1; x++) {
				for(var y = 0; y < Height; y++) {
					if(IsCorner(x, y, Width, Height)) {
						// skip corners
						continue;
					}

					bitArray.Set(bit, normalized[y * Height + x] < normalized[y * Height + x + 1]);
					bit++;
				}
			}

			var array = new byte[2];
			bitArray.CopyTo(array, 0);
			return BitConverter.ToInt16(array, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsCorner(int x, int y, int width, int height) => (x == 0 || x == width - 2) && (y == 0 || y == height - 1);
	}
}
