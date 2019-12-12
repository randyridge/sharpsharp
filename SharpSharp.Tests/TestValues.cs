using System;

namespace SharpSharp {
    public static class TestValues {
        public const string InputUrl = "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png";
        public static readonly Uri InputUri = new Uri(InputUrl);
        public static readonly ImageLoadOptions DefaultImageLoadOptions = new ImageLoadOptions();
    }
}
