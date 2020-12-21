using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
//using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace SharpSharp.Benchmarks {
	public sealed class DefaultConfig : ManualConfig {
		public DefaultConfig() {
			AddDiagnoser(MemoryDiagnoser.Default);

//#if Windows
//			AddDiagnoser(new NativeMemoryProfiler());
//			AddDiagnoser(new EtwProfiler());
//#endif

			AddColumn(StatisticColumn.AllStatistics);

			AddJob(
				Job.Default
					.WithRuntime(CoreRuntime.Core50)
			);
		}
	}
}
