using System.Collections;
using BenchmarkDotNet.Attributes;

namespace SharpSharp.Benchmarks {
    [Config(typeof(DefaultConfig))]
    public class DifferenceHashBenchmark {
        private readonly DifferenceHash hash;

        public DifferenceHashBenchmark() {
            ImagePipeline
                .FromFile(TestFiles.InputJpg)
                .Png()
                .ToBuffer(out var buffer);
            hash = new DifferenceHash(buffer);
        }

        [Benchmark]
        public BitArray ComputeLong() => hash.ComputeLong();

        [Benchmark]
        public BitArray ComputeShort() => hash.ComputeShort();
    }
}
