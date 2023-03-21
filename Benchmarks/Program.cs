using BenchmarkDotNet.Running;

namespace Benchmarks;

internal static class Program
{
	static void Main(string[] _)
	{
		var summary = BenchmarkRunner.Run<LookupBenchmarks>();

		Console.WriteLine(summary);
		Console.WriteLine("Benchmarking finished");
		Console.ReadLine();
	}
}
