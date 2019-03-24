using System;
using BenchmarkDotNet.Running;
using SharpNeatTasks.Benchmarks.FunctionRegression;

namespace SharpNeatTasks.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<FuncRegressionUtilsBenchmarks>();
        }
    }
}
