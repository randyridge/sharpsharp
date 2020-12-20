using System.IO;

namespace SharpSharp.Tests {
	public static class TestImageData {
		public static readonly string InputJpg = BuildPath("2569067123_aca715a2ee_o.jpg"); // http://www.flickr.com/photos/grizdave/2569067123/
		public static readonly string OutputJpg = BuildPath("output.jpg");

		private static string BuildPath(string fileName) => Path.Join("Images", fileName);
	}
}
