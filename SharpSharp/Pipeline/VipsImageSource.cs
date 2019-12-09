using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
    internal sealed class VipsImageSource : ImageSource {
        private readonly Image image;
        private readonly ImageType imageType;

        public VipsImageSource(Image image, ImageLoadOptions options) : base(options) {
            this.image = Guard.ArgumentNotNull(image, nameof(image));
            imageType = ImageType.FromImage(image);
        }

        public override (Image Image, ImageType ImageType) Load(VOption? options = null) => (image, imageType);
    }
}
