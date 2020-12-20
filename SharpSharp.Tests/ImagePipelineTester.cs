using System;
using System.Threading.Tasks;
using RandyRidge.Common;
using Shouldly;
using Xunit;

namespace SharpSharp {
	public partial class ImagePipelineTester {
		public static class VipsVersion {
			[Fact]
			public static void returns_version
		}
		public static class FromUriAsync {
			[Fact]
			public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(TestValues.InputUri).ForAwait()).ShouldNotBeNull();

			[Fact]
			public static Task throws_on_null_uri() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((Uri) null));
		}

		public static class FromUriAsyncWithOptions {
			[Fact]
			public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(TestValues.InputUri, new ImageLoadOptions()).ForAwait()).ShouldNotBeNull();

			[Fact]
			public static Task throws_on_null_options() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync(TestValues.InputUri, null));

			[Fact]
			public static Task throws_on_null_uri() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((Uri) null, TestValues.DefaultImageLoadOptions));
		}

		public static class FromUriAsyncWithString {
			[Fact]
			public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(TestValues.InputUrl, new ImageLoadOptions()).ForAwait()).ShouldNotBeNull();

			[Fact]
			public static Task throws_on_null_options() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync(TestValues.InputUrl, null));

			[Fact]
			public static Task throws_on_null_uri() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((string) null, TestValues.DefaultImageLoadOptions));
		}

		public static class FromUriAsyncWithStringAndOptions {
			[Fact]
			public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(TestValues.InputUrl, new ImageLoadOptions()).ForAwait()).ShouldNotBeNull();

			[Fact]
			public static Task throws_on_null_options() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync(TestValues.InputUrl, null));

			[Fact]
			public static Task throws_on_null_uri() => Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((string) null, TestValues.DefaultImageLoadOptions));
		}
	}
}
