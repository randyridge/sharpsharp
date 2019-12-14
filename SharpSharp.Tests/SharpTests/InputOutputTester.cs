using System.IO;
using Shouldly;
using Xunit;

namespace SharpSharp.SharpTests {
    public static class InputOutputTester {
        private const int TestWidth = 320;
        private const int TestHeight = 240;

        [Fact]
        public static void read_file_write_stream() {
            using var outputFile = TestFiles.OutputJpg;
            using var fileStream = new FileStream(outputFile.Path, FileMode.Open, FileAccess.Write);
            ImagePipeline
                .FromFile(TestFiles.InputJpg)
                .Resize(TestWidth, TestHeight)
                .Jpeg()
                .ToStream(fileStream, VerifyImageInfo);
        }

        private static void VerifyImageInfo(OutputImageInfo outputImageInfo) {
            outputImageInfo.ShouldNotBeNull();
            outputImageInfo.Format.ShouldBe("jpeg");
            outputImageInfo.Size.ShouldBeGreaterThan(0);
            outputImageInfo.Width.ShouldBe(TestWidth);
            outputImageInfo.Height.ShouldBe(TestHeight);
        }
    }
}
