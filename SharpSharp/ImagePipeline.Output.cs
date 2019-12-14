//using System;
//using SharpSharp.Pipeline.Operations;

using System;
using System.IO;
using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

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

        public void ToFile(string filePath, Action<OutputImageInfo> callback) => ToFile(new ToFileOptions(filePath, callback));

        public void ToFile(ToFileOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.ToFileOptions = options;
            Execute();
        }

        /// <summary>
        ///     Writes the result of the image pipeline to the specified stream.
        /// </summary>
        /// <param name="stream">
        ///     The stream to write to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="stream" /> is null.
        /// </exception>
        public void ToStream(Stream stream) {
            Guard.ArgumentNotNull(stream, nameof(stream));
            ToStream(new ToStreamOptions(stream, null));
        }

        /// <summary>
        ///     Writes the result of the image pipeline to the specified stream and invokes a callback with <see cref="OutputImageInfo" />.
        /// </summary>
        /// <param name="stream">
        ///     The stream to write to.
        /// </param>
        /// <param name="callback">
        ///     The callback to receive <see cref="OutputImageInfo" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="stream" /> or <paramref name="callback" /> is null.
        /// </exception>
        public void ToStream(Stream stream, Action<OutputImageInfo> callback) {
            Guard.ArgumentNotNull(stream, nameof(stream));
            Guard.ArgumentNotNull(callback, nameof(callback));
            ToStream(new ToStreamOptions(stream, callback));
        }

        /// <summary>
        ///     Writes the result of the image pipeline to the specified stream with the specified <see cref="ToStreamOptions" />.
        /// </summary>
        /// <param name="streamOptions">
        ///     The stream options to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="streamOptions" /> is null.
        /// </exception>
        public void ToStream(ToStreamOptions streamOptions) {
            Guard.ArgumentNotNull(streamOptions, nameof(streamOptions));
            result.ToStreamOptions = streamOptions;
            Execute();
            if(streamOptions.Callback.HasValue()) {
                streamOptions.Callback(result.OutputImageInfo!);
            }
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
