using System;
using System.Buffers.Text;
using RandyRidge.Common;

namespace SharpSharp {
	public static class TestValues {
		private const string OnePixelWhite = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+ip1sAAAAASUVORK5CYII="; // Courtesy https://png-pixel.com/

		public const string InputUrl = "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png";

		public const string InputPath = "./SharpTestImages/320x240.jpg";

		public static readonly byte[] InputBuffer = Convert.FromBase64String(OnePixelWhite);
		
		public static readonly ImageLoadOptions DefaultImageLoadOptions = new();

		public static readonly Uri InputUri = new(InputUrl);
	}
}
