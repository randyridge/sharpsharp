using Shouldly;
using Xunit;

namespace SharpSharp {
	public partial class ImagePipelineTester {
		public static class VipsVersion {
			[Fact]
			public static void returns_version() {
				var version = ImagePipeline.VipsVersion;
				version.ShouldNotBeNull();
				version.Major.ShouldBe(8);
				version.Minor.ShouldBeGreaterThanOrEqualTo(10);
			}
		}
	}
}
