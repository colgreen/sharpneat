using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Running;

namespace ActivationFnBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ManualConfig.Create(DefaultConfig.Instance);

            // Set up an results exporter.
            // Note. By default results files will be located in .\BenchmarkDotNet.Artifacts\results directory.
            config.Add(new CsvExporter(CsvSeparator.CurrentCulture,
                new BenchmarkDotNet.Reports.SummaryStyle
                {
                    PrintUnitsInHeader = true,
                    PrintUnitsInContent = false,
                    TimeUnit = TimeUnit.Microsecond,
                    SizeUnit = BenchmarkDotNet.Columns.SizeUnit.KB
                }));

            // Run benchmarks.
            var summary = BenchmarkRunner.Run<FunctionBenchmarks>(config);
        }
    }
}
