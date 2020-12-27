using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using ImageMagick;
using NetVips;

namespace SharpSharp.Benchmarks {
	[Config(typeof(DefaultConfig))]
	//[EtwProfiler]
	public class JpegOpenResizeSaveBenchmarks {
		private const int Height = 588;
		private const int Width = 720;
		private readonly byte[] buffer;

		public JpegOpenResizeSaveBenchmarks() {
			NetVips.NetVips.Concurrency = Environment.ProcessorCount;
			Cache.Max = 0;
			Cache.MaxFiles = 0;
			buffer = File.ReadAllBytes(TestFiles.InputJpg);
		}

		[Benchmark]
		public void MagickFromBufferToFile() {
			using var output = TestFiles.OutputJpg();
			using var image = new MagickImage(buffer) {
				FilterType = FilterType.Lanczos,
				Quality = 80
			};
			image.Resize(Width, Height);
			image.Write(output.Path, MagickFormat.Jpg);
		}

		[Benchmark]
		public void MagickFromFileToFile() {
			using var output = TestFiles.OutputJpg();
			using var image = new MagickImage(TestFiles.InputJpg) {
				FilterType = FilterType.Lanczos,
				Quality = 80
			};
			image.Resize(Width, Height);
			image.Write(output.Path, MagickFormat.Jpg);
		}

		[Benchmark]
		public void MagickFromStreamToFile() {
			using var output = TestFiles.OutputJpg();
			using var stream = File.OpenRead(TestFiles.InputJpg);
			using var image = new MagickImage(stream) {
				FilterType = FilterType.Lanczos,
				Quality = 80
			};
			image.Resize(Width, Height);
			image.Write(output.Path, MagickFormat.Jpg);
		}

		[Benchmark(Baseline = true)]
		public void SharpSharpFromBufferToFile() {
			using var output = TestFiles.OutputJpg();
			ImagePipeline
				.FromBuffer(buffer)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark]
		public void SharpSharpFromFileToFile() {
			using var output = TestFiles.OutputJpg();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark]
		public void SharpSharpFromStreamToFile() {
			using var output = TestFiles.OutputJpg();
			using var stream = File.OpenRead(TestFiles.InputJpg);
			ImagePipeline
				.FromStream(stream)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}
	}
}
