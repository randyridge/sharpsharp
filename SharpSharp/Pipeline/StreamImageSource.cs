using System.IO;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
    internal sealed class StreamImageSource : ImageSource {
        public StreamImageSource(Stream stream, ImageLoadOptions options) : base(options) {
            Guard.NotNull(stream, nameof(stream));
            Stream = stream;
        }

        public Stream Stream { get; }

        public override (Image, ImageType) Load(VOption? options = null) {
            try {
                var imageType = ImageType.FromStream(Stream);

                if(options == null) {
                    options = BuildLoadOptionsFromImageType(imageType);
                }

                var image = Image.NewFromStream(
                    Stream,
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
                throw new SharpSharpException("Error loading image from stream.", ex);
            }
        }
    }
}
