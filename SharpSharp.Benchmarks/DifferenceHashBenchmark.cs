using BenchmarkDotNet.Attributes;

namespace SharpSharp.Benchmarks {
	[Config(typeof(DefaultConfig))]
	public class DifferenceHashBenchmark {
		private readonly byte[] buffer;

		public DifferenceHashBenchmark() {
			ImagePipeline
				.FromFile(TestFiles.InputJpg)
				.Png()
				.ToBuffer(out var b);
			buffer = b;
		}

		[Benchmark(Baseline = true)]
		public long ComputeLong() => DifferenceHash.HashLong(buffer);

		[Benchmark]
		public short ComputeShort() => DifferenceHash.HashShort(buffer);
	}
}
