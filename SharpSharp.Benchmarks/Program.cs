using System;
using BenchmarkDotNet.Running;
using NetVips;

namespace SharpSharp.Benchmarks {
	internal static class Program {
		//private static void Main(string[] args) =>
		//	BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
		private static void Main() {
			NetVips.NetVips.ConcurrencySet(Environment.ProcessorCount);
			Cache.MaxFiles = 0;
			Image.NewFromFile(TestFiles.InputJpg).Heifsave(@".\lichtenstein.avif", q: 50, compression: Enums.ForeignHeifCompression.Av1, speed: 6, strip: true);
			//ImagePipeline.FromFile(TestFiles.InputJpg).Avif().ToFile(@"C:\blah.avif");
			////ImagePipeline.FromFile(TestFiles.InputJpg).Webp().ToFile(@"C:\blah.webp");
			////ImagePipeline.FromFile(TestFiles.InputJpg).Jpeg().ToFile(@"C:\blah.jpg");
		}
	}
}
