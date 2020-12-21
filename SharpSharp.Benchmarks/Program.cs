using BenchmarkDotNet.Running;

namespace SharpSharp.Benchmarks {
	internal static class Program {
		private static void Main(string[] args) =>
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
	}
}
