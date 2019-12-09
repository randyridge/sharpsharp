using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
    public sealed partial class ImagePipeline {
        public static ImagePipeline FromBuffer(byte[] buffer) => FromBuffer(buffer, new ImageLoadOptions());

        public static ImagePipeline FromBuffer(byte[] buffer, ImageLoadOptions options) {
            Guard.ArgumentNotNull(buffer, nameof(buffer));
            return new ImagePipeline(new BufferImageSource(buffer, options));
        }

        public static ImagePipeline FromImage(Image image) => FromImage(image, new ImageLoadOptions());

        public static ImagePipeline FromImage(Image image, ImageLoadOptions options) {
            Guard.ArgumentNotNull(image, nameof(image));
            Guard.ArgumentNotNull(options, nameof(options));
            return new ImagePipeline(new VipsImageSource(image, options));
        }

        //public static ImagePipeline FromFile(string path) => FromFile(path, new ImageLoadOptions());

        //public static ImagePipeline FromFile(string path, ImageLoadOptions options) {
        //    Guard.ArgumentNotNullOrWhiteSpace(path, nameof(path));
        //    return FromBuffer(File.ReadAllBytes(path));
        //}

        //public static ImagePipeline FromStream(Stream stream) => FromStream(stream, new ImageLoadOptions());

        //public static ImagePipeline FromStream(Stream stream, ImageLoadOptions options) {
        //    Guard.ArgumentNotNull(stream, nameof(stream));
        //    using var ms = RecyclableMemoryStreamManager.GetStream();
        //    stream.CopyTo(ms);
        //    return FromBuffer(ms.ToArray());
        //}
    }
}
