using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using ImageMagick;

namespace SharpSharp.Benchmarks {
	[Config(typeof(DefaultConfig))]
	[EtwProfiler]
	public class JpegBufferToFile {
		private const int Height = 588;
		private const int Width = 720;
		private readonly byte[] buffer;

		public JpegBufferToFile() {
			NetVips.NetVips.ConcurrencySet(Environment.ProcessorCount);
			buffer = File.ReadAllBytes(TestFiles.InputJpg);
		}

		[Benchmark]
		public void Magick() {
			using var output = TestFiles.OutputJpg();
			using var image = new MagickImage(buffer) {
				FilterType = FilterType.Lanczos,
				Quality = 80
			};
			image.Resize(Width, Height);
			image.Write(output.Path, MagickFormat.Jpg);
		}

		[Benchmark(Baseline = true)]
		public void SharpSharpFromBuffer() {
			using var output = TestFiles.OutputJpg();
			ImagePipeline
				.FromBuffer(buffer)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark]
		public void SharpSharpFromFile() {
			using var output = TestFiles.OutputJpg();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark]
		public void SharpSharpFromStream() {
			using var output = TestFiles.OutputJpg();
			using var stream = File.OpenRead(TestFiles.InputJpg);
			ImagePipeline
				.FromStream(stream)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}
	}
}
