using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
    internal sealed class BufferImageSource : ImageSource {
        public BufferImageSource(byte[] buffer, ImageLoadOptions options) : base(options) {
            Guard.ArgumentNotNullOrEmpty(buffer, nameof(buffer));
            Buffer = buffer;
        }

        public byte[] Buffer { get; }

        public override (Image, ImageType) Load(VOption? options = null) {
            try {
                var imageType = ImageType.FromBuffer(Buffer);

                if(options == null) {
                    options = BuildLoadOptionsFromImageType(imageType);
                }

                var image = Image.NewFromBuffer(
                    Buffer,
                    string.Empty,
                    Options.UseSequentialRead ? Enums.Access.Sequential : Enums.Access.Random,
                    true,
                    options
                );

                // TODO: this
                //if(image.Width * image.Height > Options.PixelLimit) {
                //    throw new SharpSharpException("Input image exceeds pixel limit.");
                //}

                return (image, imageType);
            }
            catch(VipsException ex) {
                throw new SharpSharpException("Error loading image from buffer.", ex);
            }
        }
    }
}
