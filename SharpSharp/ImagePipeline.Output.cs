//using System;
//using SharpSharp.Pipeline.Operations;

using System;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp {
    public sealed partial class ImagePipeline {
        public ImagePipeline Heif() => Heif(new HeifOptions());

        public ImagePipeline Heif(HeifOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.HeifOptions = options;
            return this;
        }

        public ImagePipeline Jpeg() => Jpeg(new JpegOptions());

        public ImagePipeline Jpeg(JpegOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.JpegOptions = options;
            return this;
        }

        public ImagePipeline Png() => Png(new PngOptions());

        public ImagePipeline Png(PngOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.PngOptions = options;
            return this;
        }

        public ImagePipeline Raw() => Raw(new RawOptions());

        public ImagePipeline Raw(RawOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.RawOptions = options;
            return this;
        }

//        // TODO: this
//        public ImagePipeline Raw() => throw new NotImplementedException();

//        // TODO: this
//        public ImagePipeline Tiff() => throw new NotImplementedException();

//        // TODO: this
//        public ImagePipeline Tile() => throw new NotImplementedException();

        public byte[]? ToBuffer() {
            byte[]? buffer = null;
            result.ToBufferOptions = new ToBufferOptions(bytes => buffer = bytes);
            Execute();
            return buffer;
        }

        public void ToBuffer(ToBufferOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.ToBufferOptions = options;
            Execute();
        }

        public void ToFile(string filePath) => ToFile(new ToFileOptions(filePath, null));

        public void ToFile(string filePath, Action<ImageInfo> callback) => ToFile(new ToFileOptions(filePath, callback));

        public void ToFile(ToFileOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.ToFileOptions = options;
            Execute();
        }

        public ImagePipeline Webp() => Webp(new WebpOptions());

        public ImagePipeline Webp(WebpOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.WebpOptions = options;
            return this;
        }

        public ImagePipeline VipsImage(Action<Image> callback) {
            Guard.ArgumentNotNull(callback, nameof(callback));
            var (image, _) = imageSource.Load();
            callback(image);
            image?.Dispose();
            return this;
        }

        public ImagePipeline WithMetadata() => WithMetadata(new MetadataOptions());

        public ImagePipeline WithMetadata(MetadataOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.MetadataOptions = options;
            return this;
        }
    }
}
