using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Characteristics;

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

            // Legacy JITter tests.
            config.Add(new Job(EnvMode.LegacyJitX64, EnvMode.Clr, RunMode.Short)
            {
                Env = { Runtime = Runtime.Clr, Platform = Platform.X64 },
                Run = { LaunchCount = 1, WarmupCount = 1, TargetCount = 1, RunStrategy = BenchmarkDotNet.Engines.RunStrategy.Throughput },
                Accuracy = { RemoveOutliers = true }
            }.WithGcAllowVeryLargeObjects(true));


            // RyuJIT tests.
            config.Add(new Job(EnvMode.RyuJitX64, EnvMode.Clr, RunMode.Short)
            {
                Env = { Runtime = Runtime.Clr, Platform = Platform.X64 },
                Run = { LaunchCount = 1, WarmupCount = 1, TargetCount = 1, RunStrategy = BenchmarkDotNet.Engines.RunStrategy.Throughput },
                Accuracy = { RemoveOutliers = true }
            }.WithGcAllowVeryLargeObjects(true));

            // Uncomment to allow benchmarking of non-optimized assemblies.
            //config.Add(JitOptimizationsValidator.DontFailOnError);

            // Run benchmarks.
            var summary = BenchmarkRunner.Run<FunctionBenchmarks>(config);
        }
    }
}
