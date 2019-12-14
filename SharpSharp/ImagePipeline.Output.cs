using System;
using System.IO;
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

        /// <summary>
        ///     Use JPEG with default options for output image.
        /// </summary>
        /// <returns>
        ///     The image pipeline.
        /// </returns>
        public ImagePipeline Jpeg() => Jpeg(new JpegOptions());

        /// <summary>
        ///     Use JPEG with the specified options for output image.
        /// </summary>
        /// <param name="options">
        ///     The options for the output image.
        /// </param>
        /// <returns>
        ///     The image pipeline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="options" /> is null.
        /// </exception>
        public ImagePipeline Jpeg(JpegOptions options) {
            Guard.ArgumentNotNull(options, nameof(options));
            result.JpegOptions = options;
            return this;
        }

        /// <summary>
        ///     Use PNG with default options for output image.
        /// </summary>
        /// <returns>
        ///     The image pipeline.
        /// </returns>
        public ImagePipeline Png() => Png(new PngOptions());

        /// <summary>
        ///     Use PNG with the specified options for output image.
        /// </summary>
        /// <param name="options">
        ///     The options for the output image.
        /// </param>
        /// <returns>
        ///     The image pipeline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="options" /> is null.
        /// </exception>
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

        /// <summary>
        ///     Writes the result of the image pipeline to the specified buffer.
        /// </summary>
        /// <param name="buffer">
        ///     The buffer to write to.
        /// </param>
        public void ToBuffer(out byte[] buffer) {
            buffer = Array.Empty<byte>();
            ToBuffer(new ToBufferOptions(buffer, null));
        }

        /// <summary>
        ///     Writes the result of the image pipeline to the specified buffer and invokes a callback with <see cref="OutputImageInfo" />.
        /// </summary>
        /// <param name="callback">
        ///     The callback to receive <see cref="OutputImageInfo" />.
        /// </param>
        /// <param name="buffer">
        ///     The buffer to write to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="callback" /> is null.
        /// </exception>
        public void ToBuffer(Action<OutputImageInfo> callback, out byte[] buffer) {
            buffer = Array.Empty<byte>();
            ToBuffer(new ToBufferOptions(buffer, callback));
        }

        /// <summary>
        ///     Writes the result of the image pipeline using the specified <see cref="ToBufferOptions" />.
        /// </summary>
        /// <param name="bufferOptions">
        ///     The file options to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="bufferOptions" /> is null.
        /// </exception>
        public void ToBuffer(ToBufferOptions bufferOptions) {
            Guard.ArgumentNotNull(bufferOptions, nameof(bufferOptions));
            result.ToBufferOptions = bufferOptions;
            Execute();
        }

        /// <summary>
        ///     Writes the result of the image pipeline to the specified file path.
        /// </summary>
        /// <param name="filePath">
        ///     The file path to write to.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath" /> is empty or contains only whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath" /> is null.
        /// </exception>
        public void ToFile(string filePath) {
            Guard.ArgumentNotNullOrWhiteSpace(filePath, nameof(filePath));
            ToFile(new ToFileOptions(filePath, null));
        }

        /// <summary>
        ///     Writes the result of the image pipeline to the specified file path and invokes a callback with <see cref="OutputImageInfo" />.
        /// </summary>
        /// <param name="filePath">
        ///     The file path to write to.
        /// </param>
        /// <param name="callback">
        ///     The callback to receive <see cref="OutputImageInfo" />.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath" /> is empty or contains only whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath" /> or <paramref name="callback" /> is null.
        /// </exception>
        public void ToFile(string filePath, Action<OutputImageInfo> callback) {
            Guard.ArgumentNotNullOrWhiteSpace(filePath, nameof(filePath));
            Guard.ArgumentNotNull(callback, nameof(callback));
            ToFile(new ToFileOptions(filePath, callback));
        }

        /// <summary>
        ///     Writes the result of the image pipeline using the specified <see cref="ToFileOptions" />.
        /// </summary>
        /// <param name="fileOptions">
        ///     The file options to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileOptions" /> is null.
        /// </exception>
        public void ToFile(ToFileOptions fileOptions) {
            Guard.ArgumentNotNull(fileOptions, nameof(fileOptions));
            result.ToFileOptions = fileOptions;
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

        /// <summary>
        ///     Use WebP with default options for output image.
        /// </summary>
        /// <returns>
        ///     The image pipeline.
        /// </returns>
        public ImagePipeline Webp() => Webp(new WebpOptions());

        /// <summary>
        ///     Use WebP with the specified options for output image.
        /// </summary>
        /// <param name="options">
        ///     The options for the output image.
        /// </param>
        /// <returns>
        ///     The image pipeline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="options" /> is null.
        /// </exception>
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
