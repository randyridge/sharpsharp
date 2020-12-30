using System.IO;
using RandyRidge.Common.IO;

namespace SharpSharp.Benchmarks {
	public static class TestFiles {
		public static string InputJpg { get; } = GetInputFilePath("input.jpg"); // http://www.flickr.com/photos/grizdave/2569067123/    }

		public static RandomFile OutputAvif() => new(".avif");

		public static RandomFile OutputGif() => new(".gif");

		public static RandomFile OutputHeif() => new(".heif");

		public static RandomFile OutputJpg() => new(".jpeg");

		public static RandomFile OutputPng() => new(".png");

		public static RandomFile OutputTiff() => new(".tiff");

		public static RandomFile OutputWebp() => new(".webp");

		private static string GetInputFilePath(string fileName) => Path.Join("SharpTestImages", fileName);
	}
}
