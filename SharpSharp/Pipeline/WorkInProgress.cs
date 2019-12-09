using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
    internal sealed class WorkInProgress {
        public WorkInProgress(Image image, ImageType imageType) {
            Image = Guard.ArgumentNotNull(image, nameof(image));
            ImageType = Guard.ArgumentNotNull(imageType, nameof(imageType));
            InputHeight = image.Height;
            InputWidth = image.Width;
        }

        public Image Image { get; }

        public ImageType ImageType { get; }

        public int InputWidth { get; set; }

        public int InputHeight { get; set; }
    }
}
