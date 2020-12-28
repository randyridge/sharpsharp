using Shouldly;
using Xunit;

namespace SharpSharp {
	public partial class ImagePipelineTester {
		public static class Avif {
			[Fact]
			public static void returns_pipeline() => ImagePipeline.FromBuffer(TestValues.InputBuffer).Avif().ShouldNotBeNull();
		}
	}
}
