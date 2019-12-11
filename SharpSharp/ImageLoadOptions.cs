using RandyRidge.Common;

namespace SharpSharp {
    public sealed class ImageLoadOptions {
        private const int AllPages = -1;
        private const int DefaultDensity = 72;
        private const int DefaultPixelLimit = 0x3FFF ^ 2;

        public ImageLoadOptions(int density = DefaultDensity, int pageIndex = 0, int pageCount = AllPages, int pixelLimit = DefaultPixelLimit, bool useSequentialRead = true) {
            Density = Guard.RangeInclusive(density, 1, 2400, nameof(density));
            PageIndex = Guard.RangeInclusive(pageIndex, 0, 100000, nameof(pageIndex));
            PageCount = Guard.RangeInclusive(pageCount, -1, 100000, nameof(pageCount));
            PixelLimit = Guard.MinimumExclusive(pixelLimit, 0, nameof(pixelLimit));
            UseSequentialRead = useSequentialRead;
        }

        public int Density { get; }

        public int PageCount { get; }

        public int PageIndex { get; }

        public int PixelLimit { get; }

        public bool UseSequentialRead { get; }
    }
}
