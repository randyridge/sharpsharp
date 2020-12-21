//using System.IO;
//using Shouldly;
//using Xunit;

//namespace SharpSharp.SharpTests {
//	public static class InputOutputTester {
//		private const int TestHeight = 240;
//		private const int TestWidth = 320;

//		[Fact]
//		public static void read_buffer_write_stream() {
//			var buffer = File.ReadAllBytes(TestFiles.InputJpg);
//			using var outputFile = TestFiles.OutputJpg();
//			using var outputStream = new FileStream(outputFile.Path, FileMode.Open, FileAccess.Write);
//			ImagePipeline
//				.FromBuffer(buffer)
//				.TestActions()
//				.ToStream(outputStream, VerifyImageInfo);
//		}

//		[Fact]
//		public static void read_file_write_stream() {
//			using var outputFile = TestFiles.OutputJpg();
//			using var outputStream = new FileStream(outputFile.Path, FileMode.Open, FileAccess.Write);
//			ImagePipeline
//				.FromFile(TestFiles.InputJpg)
//				.TestActions()
//				.ToStream(outputStream, VerifyImageInfo);
//		}

//		[Fact]
//		public static void read_stream_write_buffer() {
//			using var inputStream = new FileStream(TestFiles.InputJpg, FileMode.Open, FileAccess.Read);
//			ImagePipeline
//				.FromStream(inputStream)
//				.TestActions()
//				.ToBuffer(VerifyImageInfo, out _);
//		}

//		[Fact]
//		public static void read_stream_write_file() {
//			using var inputStream = new FileStream(TestFiles.InputJpg, FileMode.Open, FileAccess.Read);
//			using var outputFile = TestFiles.OutputJpg();
//			ImagePipeline
//				.FromStream(inputStream)
//				.TestActions()
//				.ToFile(outputFile.Path, VerifyImageInfo);
//		}

//		private static ImagePipeline TestActions(this ImagePipeline imagePipeline) => imagePipeline.Resize(TestWidth, TestHeight).Jpeg();

//		private static void VerifyImageInfo(OutputImageInfo outputImageInfo) {
//			outputImageInfo.ShouldNotBeNull();
//			outputImageInfo.Format.ShouldBe("jpeg");
//			outputImageInfo.Size.ShouldBeGreaterThan(0);
//			outputImageInfo.Width.ShouldBe(TestWidth);
//			outputImageInfo.Height.ShouldBe(TestHeight);
//		}
//	}
//}
