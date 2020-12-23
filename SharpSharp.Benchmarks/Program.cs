using BenchmarkDotNet.Running;

namespace SharpSharp.Benchmarks {
	internal static class Program {
		//private static void Main(string[] args) =>
		//	BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
		private static void Main() {
			ImagePipeline.FromFile(TestFiles.InputJpg).Avif().ToFile(@"C:\blah.avif");
			ImagePipeline.FromFile(TestFiles.InputJpg).Webp().ToFile(@"C:\blah.webp");
			ImagePipeline.FromFile(TestFiles.InputJpg).Jpeg().ToFile(@"C:\blah.jpg");
		}
	}
}
