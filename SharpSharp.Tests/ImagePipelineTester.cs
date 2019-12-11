using System;
using System.Threading.Tasks;
using RandyRidge.Common;
using Shouldly;
using Xunit;

namespace SharpSharp {
    public static class ImagePipelineTester {
        public static class FromUriAsync {
            [Fact]
            public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(CommonTestValues.ImageUri).ForAwait()).ShouldNotBeNull();

            [Fact]
            public static Task throws_on_null_uri() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((Uri) null));
        }

        public static class FromUriAsyncWithOptions {
            [Fact]
            public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(CommonTestValues.ImageUri, new ImageLoadOptions()).ForAwait()).ShouldNotBeNull();

            [Fact]
            public static Task throws_on_null_options() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync(CommonTestValues.ImageUri, null));

            [Fact]
            public static Task throws_on_null_uri() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((Uri) null, CommonTestValues.DefaultImageLoadOptions));
        }

        public static class FromUriAsyncWithString {
            [Fact]
            public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(CommonTestValues.ImageUrl, new ImageLoadOptions()).ForAwait()).ShouldNotBeNull();

            [Fact]
            public static Task throws_on_null_options() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync(CommonTestValues.ImageUrl, null));

            [Fact]
            public static Task throws_on_null_uri() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((string) null, CommonTestValues.DefaultImageLoadOptions));
        }

        public static class FromUriAsyncWithStringAndOptions {
            [Fact]
            public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(CommonTestValues.ImageUrl, new ImageLoadOptions()).ForAwait()).ShouldNotBeNull();

            [Fact]
            public static Task throws_on_null_options() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync(CommonTestValues.ImageUrl, null));

            [Fact]
            public static Task throws_on_null_uri() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((string) null, CommonTestValues.DefaultImageLoadOptions));
        }
    }
}
