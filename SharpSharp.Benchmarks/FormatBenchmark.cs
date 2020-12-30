using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NetVips;

namespace SharpSharp.Benchmarks {
	[Config(typeof(DefaultConfig))]
	[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Alphabetical)]
	//[EtwProfiler]
	public class FormatBenchmark {
		private const int Height = 588;
		private const int Width = 720;

		public FormatBenchmark() {
			NetVips.NetVips.Concurrency = Environment.ProcessorCount;
			Cache.Max = 0;
			Cache.MaxFiles = 0;
		}

		[Benchmark(Description = "AVIF")]
		public void Avif() {
			using var output = TestFiles.OutputAvif();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark(Description = "GIF")]
		public void Gif() {
			using var output = TestFiles.OutputGif();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark(Description = "HEIF")]
		public void Heif() {
			using var output = TestFiles.OutputHeif();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark(Description = "JPEG")]
		public void Jpeg() {
			using var output = TestFiles.OutputJpg();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark(Description = "PNG")]
		public void Png() {
			using var output = TestFiles.OutputPng();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark(Description = "TIFF")]
		public void Tiff() {
			using var output = TestFiles.OutputTiff();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark(Description = "WEBP")]
		public void Webp() {
			using var output = TestFiles.OutputWebp();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}
	}
}
