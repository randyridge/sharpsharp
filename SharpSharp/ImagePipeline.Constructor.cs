using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
    public sealed partial class ImagePipeline {
        /// <summary>
        ///     Reads an image from the specified buffer.
        /// </summary>
        /// <param name="buffer">
        ///     The buffer containing the image.
        /// </param>
        /// <returns>
        ///     An image pipeline.
        /// </returns>
        /// <remarks>
        ///     Uses the default image load options.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="buffer" /> is empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="buffer" /> is null.
        /// </exception>
        public static ImagePipeline FromBuffer(byte[] buffer) {
            Guard.NotNullOrEmpty(buffer, nameof(buffer));
            return FromBuffer(buffer, new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified buffer using the specified image load options.
        /// </summary>
        /// <param name="buffer">
        ///     The buffer containing the image.
        /// </param>
        /// <param name="options">
        ///     The image load options.
        /// </param>
        /// <returns>
        ///     An image pipeline.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="buffer" /> is empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="buffer" /> or <paramref name="options" /> is null.
        /// </exception>
        public static ImagePipeline FromBuffer(byte[] buffer, ImageLoadOptions options) {
            Guard.NotNull(buffer, nameof(buffer));
            Guard.NotNull(options, nameof(options));
            return new ImagePipeline(new BufferImageSource(buffer, options));
        }

        /// <summary>
        ///     Reads an image from the specified path.
        /// </summary>
        /// <param name="path">
        ///     The path to the image.
        /// </param>
        /// <returns>
        ///     An image pipeline.
        /// </returns>
        /// <remarks>
        ///     Uses the default image load options.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path" /> is empty or contains only whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path" /> is null.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     Thrown if <paramref name="path" /> is not found.
        /// </exception>
        public static ImagePipeline FromFile(string path) {
            Guard.FileExists(path, nameof(path));
            return FromFile(path, new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified path using the specified image load options.
        /// </summary>
        /// <param name="path">
        ///     The path to the image.
        /// </param>
        /// <param name="options">
        ///     The image load options.
        /// </param>
        /// <returns>
        ///     An image pipeline.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path" /> is empty or contains only whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path" /> or <paramref name="options" /> is null.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     Thrown if <paramref name="path" /> is not found.
        /// </exception>
        public static ImagePipeline FromFile(string path, ImageLoadOptions options) {
            Guard.FileExists(path, nameof(path));
            Guard.NotNull(options, nameof(options));
            return FromBuffer(File.ReadAllBytes(path), options);
        }

        /// <summary>
        ///     Reads an image from the specified path.
        /// </summary>
        /// <param name="path">
        ///     The path to the image.
        /// </param>
        /// <returns>
        ///     A task representing the image pipeline read operation.
        /// </returns>
        /// <remarks>
        ///     Uses the default image load options.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path" /> is empty or contains only whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path" /> is null.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     Thrown if <paramref name="path" /> is not found.
        /// </exception>
        public static Task<ImagePipeline> FromFileAsync(string path) {
            Guard.NotNullOrWhiteSpace(path, nameof(path));
            return FromFileAsync(path, new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified path using the specified image load options.
        /// </summary>
        /// <param name="path">
        ///     The path to the image.
        /// </param>
        /// <param name="options">
        ///     The image load options.
        /// </param>
        /// <returns>
        ///     A task representing the image pipeline read operation.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path" /> is empty or contains only whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path" /> or <paramref name="options" /> is null.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     Thrown if <paramref name="path" /> is not found.
        /// </exception>
        public static async Task<ImagePipeline> FromFileAsync(string path, ImageLoadOptions options) {
            Guard.NotNullOrWhiteSpace(path, nameof(path));
            Guard.NotNull(options, nameof(options));
            return FromBuffer(await File.ReadAllBytesAsync(path).ForAwait(), options);
        }

        /// <summary>
        ///     Reads an image from the specified vips image.
        /// </summary>
        /// <param name="image">
        ///     The vips image.
        /// </param>
        /// <returns>
        ///     An image pipeline.
        /// </returns>
        /// <remarks>
        ///     Uses the default image load options.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="image" /> is null.
        /// </exception>
        public static ImagePipeline FromImage(Image image) {
            Guard.NotNull(image, nameof(image));
            return FromImage(image, new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified vips image with the specified image load options.
        /// </summary>
        /// <param name="image">
        ///     The vips image.
        /// </param>
        /// <param name="options">
        ///     The image load options.
        /// </param>
        /// <returns>
        ///     An image pipeline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="image" /> or <paramref name="options" /> is null.
        /// </exception>
        public static ImagePipeline FromImage(Image image, ImageLoadOptions options) {
            Guard.NotNull(image, nameof(image));
            Guard.NotNull(options, nameof(options));
            return new ImagePipeline(new VipsImageSource(image, options));
        }

        /// <summary>
        ///     Reads an image from the specified stream.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the image.
        /// </param>
        /// <returns>
        ///     An image pipeline.
        /// </returns>
        /// <remarks>
        ///     Uses the default image load options.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="stream" /> is null.
        /// </exception>
        public static ImagePipeline FromStream(Stream stream) {
            Guard.NotNull(stream, nameof(stream));
            return FromStream(stream, new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified stream with the specified image load options.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the image.
        /// </param>
        /// <param name="options">
        ///     The image load options.
        /// </param>
        /// <returns>
        ///     An image pipeline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="stream" /> or <paramref name="options" /> is null.
        /// </exception>
        public static ImagePipeline FromStream(Stream stream, ImageLoadOptions options) {
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(options, nameof(options));
            using var ms = GlobalStatics.RecyclableMemoryStreamManager.GetStream();
            stream.CopyTo(ms);
            return FromBuffer(ms.ToArray(), options);
        }

        /// <summary>
        ///     Reads an image from the specified stream.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the image.
        /// </param>
        /// <returns>
        ///     A task representing the image pipeline read operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="stream" /> is null.
        /// </exception>
        public static Task<ImagePipeline> FromStreamAsync(Stream stream) {
            Guard.NotNull(stream, nameof(stream));
            return FromStreamAsync(stream, new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified stream using the specified image load options.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the image.
        /// </param>
        /// <param name="options">
        ///     The image load options.
        /// </param>
        /// <returns>
        ///     A task representing the image pipeline read operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="stream" /> or <paramref name="options" /> is null.
        /// </exception>
        public static async Task<ImagePipeline> FromStreamAsync(Stream stream, ImageLoadOptions options) {
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(options, nameof(options));
            await using var ms = GlobalStatics.RecyclableMemoryStreamManager.GetStream();
            await stream.CopyToAsync(ms);
            return FromBuffer(ms.ToArray(), options);
        }

        /// <summary>
        ///     Reads an image from the specified URI string.
        /// </summary>
        /// <param name="uri">
        ///     The URI to the image.
        /// </param>
        /// <returns>
        ///     A task representing the image pipeline read operation.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="uri" /> is empty or contains only whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uri" /> is null.
        /// </exception>
        public static Task<ImagePipeline> FromUriAsync(string uri) {
            Guard.NotNullOrWhiteSpace(uri, nameof(uri));
            return FromUriAsync(uri, new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified URI string with the specified image load options.
        /// </summary>
        /// <param name="uri">
        ///     The URI to the image.
        /// </param>
        /// <param name="options">
        ///     The image load options.
        /// </param>
        /// <returns>
        ///     A task representing the image pipeline read operation.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="uri" /> is empty or contains only whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uri" /> or <paramref name="options" /> is null.
        /// </exception>
        public static Task<ImagePipeline> FromUriAsync(string uri, ImageLoadOptions options) {
            Guard.NotNullOrWhiteSpace(uri, nameof(uri));
            Guard.NotNull(options, nameof(options));
            return FromUriAsync(new Uri(uri), new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified URI.
        /// </summary>
        /// <param name="uri">
        ///     The URI to the image.
        /// </param>
        /// <returns>
        ///     A task representing the image pipeline read operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uri" /> is null.
        /// </exception>
        public static Task<ImagePipeline> FromUriAsync(Uri uri) {
            Guard.NotNull(uri, nameof(uri));
            return FromUriAsync(uri, new ImageLoadOptions());
        }

        /// <summary>
        ///     Reads an image from the specified URI with the specified image load options.
        /// </summary>
        /// <param name="uri">
        ///     The URI to the image.
        /// </param>
        /// <param name="options">
        ///     The image load options.
        /// </param>
        /// <returns>
        ///     A task representing the image pipeline read operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uri" /> or <paramref name="options" /> is null.
        /// </exception>
        public static async Task<ImagePipeline> FromUriAsync(Uri uri, ImageLoadOptions options) {
            Guard.NotNull(uri, nameof(uri));
            Guard.NotNull(options, nameof(options));
            using var httpClient = new HttpClient(); // being super lazy
            return new ImagePipeline(new BufferImageSource(await httpClient.GetByteArrayAsync(uri).ForAwait(), options));
        }
    }
}
