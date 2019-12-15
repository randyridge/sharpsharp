using Shouldly;
using Xunit;

namespace SharpSharp.SharpTests {
    public static class JpegTester {
        [Fact]
        public static void jpeg_quality() {
            var pipeline = TestPipeline();

            pipeline
                .Jpeg(new JpegOptions(70))
                .ToBuffer(out var buffer70);

            pipeline
                .Jpeg()
                .ToBuffer(out var buffer80);

            pipeline
                .Jpeg(new JpegOptions(90))
                .ToBuffer(out var buffer90);

            buffer70.Length.ShouldBeLessThan(buffer80.Length);
            buffer80.Length.ShouldBeLessThan(buffer90.Length);
        }

        [Fact]
        public static void progressive_jpeg() {
            var pipeline = TestPipeline();

            pipeline
                .Jpeg()
                .ToBuffer(out var progressive);

            pipeline
                .Jpeg(new JpegOptions(makeProgressive:false))
                .ToBuffer(out var notProgressive);

            progressive.Length.ShouldBeLessThan(notProgressive.Length);
        }

        [Fact]
        public static void without_chroma_subsampling_is_larger() {
            var pipeline = TestPipeline();

            pipeline
                .Jpeg()
                .ToBuffer(out var subsampling);

            pipeline
                .Jpeg(new JpegOptions(useChromaSubsampling:false))
                .ToBuffer(out var noSubsampling);

            subsampling.Length.ShouldBeLessThan(noSubsampling.Length);
        }

        [Fact]
        public static void trellis_quantization() {
            var pipeline = TestPipeline();

            pipeline
                .Jpeg()
                .ToBuffer(out var without);

            pipeline
                .Jpeg(new JpegOptions(applyTrellisQuantization: true))
                .ToBuffer(out var with);

            with.Length.ShouldBeLessThanOrEqualTo(without.Length);
        }

        [Fact(Skip = "broken")]
        public static void optimize_scans() {
            var pipeline = TestPipeline();

            pipeline
                .Jpeg()
                .ToBuffer(out var without);

            pipeline
                .Jpeg(new JpegOptions(optimizeScans: true))
                .ToBuffer(out var with);

            with.Length.ShouldNotBe(without.Length);
        }

        private static ImagePipeline TestPipeline() =>
            ImagePipeline
                .FromFile(TestFiles.InputJpg)
                .Resize(320, 240);
    }
}
