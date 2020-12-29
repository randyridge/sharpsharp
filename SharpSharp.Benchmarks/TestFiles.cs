using System.IO;
using RandyRidge.Common.IO;

namespace SharpSharp.Benchmarks {
	public static class TestFiles {
		public static string InputJpg { get; } = GetInputFilePath("input.jpg"); // http://www.flickr.com/photos/grizdave/2569067123/    }

		public static RandomFile OutputJpg() => new(".jpg");

		private static string GetInputFilePath(string fileName) => Path.Join("SharpTestImages", fileName);
	}
}
