using BenchmarkDotNet.Running;

namespace SharpSharp.Benchmarks {
    internal static class Program {
        private static void Main() => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly);
    }
}
