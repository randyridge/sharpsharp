using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace SharpSharp.Benchmarks {
	public sealed class DefaultConfig : ManualConfig {
		public DefaultConfig() {
			Add(MemoryDiagnoser.Default);

#if Windows
			Add(new NativeMemoryProfiler());
#endif

			Add(StatisticColumn.AllStatistics);

			Add(Job.Default
				.With(CoreRuntime.Core31)
				.With(Jit.RyuJit)
			);
		}
	}
}
