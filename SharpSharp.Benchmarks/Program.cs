using System;
using BenchmarkDotNet.Running;
using NetVips;

namespace SharpSharp.Benchmarks {
	internal static class Program {
		private static void Main(string[] args) {
			NetVips.NetVips.Concurrency = Environment.ProcessorCount;
			Cache.Max = 0;
			Cache.MaxFiles = 0;

			BenchmarkSwitcher
				.FromAssembly(typeof(Program).Assembly)
				.Run(args);
		}
	}
}
