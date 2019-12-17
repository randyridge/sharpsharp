using BenchmarkDotNet.Running;

namespace SharpSharp.Benchmarks {
    internal static class Program {
        private static void Main() => BenchmarkRunner.Run<DifferenceHashBenchmark>();
    }
}
