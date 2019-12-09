using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IO;
using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
    public sealed partial class ImagePipeline {
        private static readonly RecyclableMemoryStreamManager RecyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

        public static ImagePipeline FromBuffer(byte[] buffer) => FromBuffer(buffer, new ImageLoadOptions());

        public static ImagePipeline FromBuffer(byte[] buffer, ImageLoadOptions options) {
            Guard.ArgumentNotNull(buffer, nameof(buffer));
            Guard.ArgumentNotNull(options, nameof(options));
            return new ImagePipeline(new BufferImageSource(buffer, options));
        }

        public static ImagePipeline FromFile(string path) => FromFile(path, new ImageLoadOptions());

        public static ImagePipeline FromFile(string path, ImageLoadOptions options) {
            Guard.ArgumentNotNullOrWhiteSpace(path, nameof(path));
            Guard.ArgumentNotNull(options, nameof(options));
            return FromBuffer(File.ReadAllBytes(path), options);
        }

        public static Task<ImagePipeline> FromFileAsync(string path) => FromFileAsync(path, new ImageLoadOptions());

        public static async Task<ImagePipeline> FromFileAsync(string path, ImageLoadOptions options) {
            Guard.ArgumentNotNullOrWhiteSpace(path, nameof(path));
            Guard.ArgumentNotNull(options, nameof(options));
            return FromBuffer(await File.ReadAllBytesAsync(path).ForAwait(), options);
        }

        public static ImagePipeline FromImage(Image image) => FromImage(image, new ImageLoadOptions());

        public static ImagePipeline FromImage(Image image, ImageLoadOptions options) {
            Guard.ArgumentNotNull(image, nameof(image));
            Guard.ArgumentNotNull(options, nameof(options));
            return new ImagePipeline(new VipsImageSource(image, options));
        }

        public static ImagePipeline FromStream(Stream stream) => FromStream(stream, new ImageLoadOptions());

        public static ImagePipeline FromStream(Stream stream, ImageLoadOptions options) {
            Guard.ArgumentNotNull(stream, nameof(stream));
            Guard.ArgumentNotNull(options, nameof(options));
            using var ms = RecyclableMemoryStreamManager.GetStream();
            stream.CopyTo(ms);
            return FromBuffer(ms.ToArray(), options);
        }

        public static Task<ImagePipeline> FromStreamAsync(Stream stream) => FromStreamAsync(stream, new ImageLoadOptions());

        public static async Task<ImagePipeline> FromStreamAsync(Stream stream, ImageLoadOptions options) {
            Guard.ArgumentNotNull(stream, nameof(stream));
            Guard.ArgumentNotNull(options, nameof(options));
            await using var ms = RecyclableMemoryStreamManager.GetStream();
            await stream.CopyToAsync(ms);
            return FromBuffer(ms.ToArray(), options);
        }

        public static Task<ImagePipeline> FromUriAsync(string uri) => FromUriAsync(uri, new ImageLoadOptions());

        public static Task<ImagePipeline> FromUriAsync(string uri, ImageLoadOptions options) {
            Guard.ArgumentNotNullOrWhiteSpace(uri, nameof(uri));
            Guard.ArgumentNotNull(options, nameof(options));
            return FromUriAsync(new Uri(uri), new ImageLoadOptions());
        }

        public static Task<ImagePipeline> FromUriAsync(Uri uri) => FromUriAsync(uri, new ImageLoadOptions());

        public static async Task<ImagePipeline> FromUriAsync(Uri uri, ImageLoadOptions options) {
            Guard.ArgumentNotNull(uri, nameof(uri));
            Guard.ArgumentNotNull(options, nameof(options));
            using var httpClient = new HttpClient(); // being super lazy
            return new ImagePipeline(new BufferImageSource(await httpClient.GetByteArrayAsync(uri).ForAwait(), options));
        }
    }
}
