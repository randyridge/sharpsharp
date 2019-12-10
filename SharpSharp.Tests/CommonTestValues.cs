using System;

namespace SharpSharp {
    public static class CommonTestValues {
        private const string Murray = @"https://www.fillmurray.com/300/300";
        public const string ImageUrl = Murray;
        public static readonly Uri ImageUri = new Uri(Murray);
        public static readonly ImageLoadOptions DefaultImageLoadOptions = new ImageLoadOptions();
    }
}
