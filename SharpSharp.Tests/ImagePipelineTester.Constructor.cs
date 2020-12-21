#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using NetVips;
using Shouldly;
using Xunit;

namespace SharpSharp {
	public partial class ImagePipelineTester {
		public static class FromBuffer {
			[Fact]
			public static void returns_pipeline() => ImagePipeline.FromBuffer(TestValues.InputBuffer).ShouldNotBeNull();

			[Fact]
			public static void throws_on_empty_buffer() => Should.Throw<ArgumentException>(() => ImagePipeline.FromBuffer(Array.Empty<byte>()));

			[Fact]
			public static void throws_on_null_buffer() => Should.Throw<ArgumentNullException>(() => ImagePipeline.FromBuffer(null!));
		}

		public static class FromFile {
			[Fact]
			public static void returns_pipeline() => ImagePipeline.FromFile(TestValues.InputPath).ShouldNotBeNull();

			[Fact]
			public static void throws_on_empty_buffer() => Should.Throw<ArgumentException>(() => ImagePipeline.FromFile(string.Empty));

			[Fact]
			public static void throws_on_null_buffer() => Should.Throw<ArgumentNullException>(() => ImagePipeline.FromFile(null!));
		}

		public sealed class FromImage : IDisposable {
			private static readonly Image Image = Image.NewFromFile(TestValues.InputPath);

			public void Dispose() {
				Image.Dispose();
			}

			[Fact]
			public static void returns_pipeline() => ImagePipeline.FromImage(Image).ShouldNotBeNull();

			[Fact]
			public static void throws_on_null_image() => Should.Throw<ArgumentNullException>(() => ImagePipeline.FromImage(null!));
		}

		public static class FromStream {
			[Fact]
			public static void returns_pipeline() {
				using var stream = File.OpenRead(TestValues.InputPath);
				ImagePipeline.FromStream(stream).ShouldNotBeNull();
			}

			[Fact]
			public static void throws_on_null_stream() => Should.Throw<ArgumentNullException>(() => ImagePipeline.FromStream(null!));
		}

		public static class FromUriAsync {
			[Fact]
			public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(TestValues.InputUri)).ShouldNotBeNull();

			[Fact]
			public static async Task throws_on_null_uri() => await Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync(((Uri?) null)!));
		}

		public static class FromUriStringAsync {
			[Fact]
			public static async Task returns_pipeline() => (await ImagePipeline.FromUriAsync(TestValues.InputUrl)).ShouldNotBeNull();

			[Fact]
			public static async Task throws_on_null_uri() => await Should.ThrowAsync<ArgumentNullException>(() => ImagePipeline.FromUriAsync((string?) null!));
		}
	}
}
