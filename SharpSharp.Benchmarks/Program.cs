using SharpSharp;
using SharpSharp.Benchmarks;

ImagePipeline
	.FromFile(TestFiles.InputJpg)
	.Resize(800)
	.ToFile("output.avif");

ImagePipeline
	.FromFile(TestFiles.InputJpg)
	.Resize(800)
	.ToFile("output.heif");

ImagePipeline
	.FromFile(TestFiles.InputJpg)
	.Resize(800)
	.ToFile("output.gif");

ImagePipeline
	.FromFile(TestFiles.InputJpg)
	.Resize(800)
	.ToFile("output.jpeg");

ImagePipeline
	.FromFile(TestFiles.InputJpg)
	.Resize(800)
	.ToFile("output.png");

ImagePipeline
	.FromFile(TestFiles.InputJpg)
	.Resize(800)
	.ToFile("output.tiff");

ImagePipeline
	.FromFile(TestFiles.InputJpg)
	.Resize(800)
	.ToFile("output.webp");


//using System;
//using BenchmarkDotNet.Running;
//using NetVips;

//namespace SharpSharp.Benchmarks {
//	internal static class Program {
//		private static void Main(string[] args) {
//			NetVips.NetVips.Concurrency = Environment.ProcessorCount;
//			Cache.Max = 0;
//			Cache.MaxFiles = 0;

//			BenchmarkSwitcher
//				.FromAssembly(typeof(Program).Assembly)
//				.Run(args);
//		}
//	}
//}
