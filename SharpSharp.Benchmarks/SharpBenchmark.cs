using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using FreeImageAPI;
using ImageMagick;
using NetVips;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using SkiaSharp;
using Image = SixLabors.ImageSharp.Image;

namespace SharpSharp.Benchmarks {
	// https://sharp.pixelplumbing.com/performance
	[Config(typeof(DefaultConfig))]
	[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Alphabetical)]
	//[EtwProfiler]
	public class SharpBenchmark {
		private const int Height = 588;
		private const int Quality = 80;
		private const int Width = 720;
		private readonly byte[] buffer;

		public SharpBenchmark() {
			NetVips.NetVips.Concurrency = Environment.ProcessorCount;
			Cache.Max = 0;
			Cache.MaxFiles = 0;
			buffer = File.ReadAllBytes(TestFiles.InputJpg);
		}

		[Benchmark(Description = "FreeImage Buffer to Buffer")]
		public void FreeImageBufferBuffer() {
			using var ims = new MemoryStream(buffer);
			using var image = FreeImageBitmap.FromStream(ims);
			using var resized = new FreeImageBitmap(image, Width, Height);
			using var oms = new MemoryStream();
			resized.Save(oms, FREE_IMAGE_FORMAT.FIF_JPEG, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD);
			oms.ToArray();
		}

		[Benchmark(Description = "FreeImage File to File")]
		public void FreeImageFileFile() {
			using var output = TestFiles.OutputJpg();
			using var image = FreeImageBitmap.FromFile(TestFiles.InputJpg);
			using var resized = new FreeImageBitmap(image, Width, Height);
			resized.Save(output.Path, FREE_IMAGE_FORMAT.FIF_JPEG, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD);
		}

		[Benchmark(Description = "ImageMagick Buffer to Buffer")]
		public void ImageMagickBufferBuffer() {
			using var image = new MagickImage(buffer) {
				FilterType = FilterType.Lanczos,
				Quality = Quality
			};
			image.Resize(Width, Height);
			using var ms = new MemoryStream();
			image.Write(ms, MagickFormat.Jpg);
			ms.ToArray();
		}

		[Benchmark(Description = "ImageMagick File to File")]
		public void ImageMagickFromFileToFile() {
			using var output = TestFiles.OutputJpg();
			using var image = new MagickImage(TestFiles.InputJpg) {
				FilterType = FilterType.Lanczos,
				Quality = Quality
			};
			image.Resize(Width, Height);
			image.Write(output.Path, MagickFormat.Jpg);
		}

		[Benchmark(Description = "ImageSharp Buffer to Buffer")]
		public void ImageSharpBufferBuffer() {
			using var image = Image.Load(buffer);
			image.Mutate(x => x.Resize(Width, Height, LanczosResampler.Lanczos3));
			using var ms = new MemoryStream();
			image.Save(ms, new JpegEncoder {
				Quality = Quality
			});
			ms.ToArray();
		}

		[Benchmark(Description = "ImageSharp File to File")]
		public void ImageSharpFileFile() {
			using var output = TestFiles.OutputJpg();
			using var image = Image.Load(TestFiles.InputJpg);
			image.Mutate(x => x.Resize(Width, Height, LanczosResampler.Lanczos3));
			image.Save(output.Path, new JpegEncoder {
				Quality = Quality
			});
		}

		[Benchmark(Baseline = true, Description = "SharpSharp Buffer to Buffer")]
		public void SharpSharpBufferBuffer() {
			ImagePipeline
				.FromBuffer(buffer)
				.Resize(Width, Height)
				.ToBuffer(out var data);
		}

		[Benchmark(Description = "SharpSharp File to File")]
		public void SharpSharpFileFile() {
			using var output = TestFiles.OutputJpg();
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Resize(Width, Height)
				.ToFile(output.Path);
		}

		[Benchmark(Description = "SkiaSharp Buffer to Buffer")]
		public void SkiaSharpBufferBuffer() {
			using var ims = new MemoryStream(buffer);
			using var inputStream = new SKManagedStream(ims);
			using var original = SKBitmap.Decode(inputStream);
			using var resized = original.Resize(new SKImageInfo(Width, Height), SKBitmapResizeMethod.Lanczos3);
			using var image = SKImage.FromBitmap(resized);
			image.Encode(SKEncodedImageFormat.Jpeg, Quality).ToArray();
		}

		[Benchmark(Description = "SkiaSharp File to File")]
		public void SkiaSharpFileFile() {
			using var fs = File.OpenRead(TestFiles.InputJpg);
			using var inputStream = new SKManagedStream(fs);
			using var original = SKBitmap.Decode(inputStream);
			using var resized = original.Resize(new SKImageInfo(Width, Height), SKBitmapResizeMethod.Lanczos3);
			using var output = TestFiles.OutputJpg();
			using var oms = File.OpenWrite(output.Path);
			using var image = SKImage.FromBitmap(resized);
			image.Encode(SKEncodedImageFormat.Jpeg, Quality).SaveTo(oms);
		}
	}
}
