using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Redzen.Random;
using SharpNeat.Graphs;

namespace SharpNeat.Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring)]
    [MemoryDiagnoser]
    public class ConnectionSorterBenchmarks
    {
        readonly Xoshiro256StarStarRandom _rng = new(123);
        ConnectionData[] _dataArr;

        #region Public Methods

        [IterationSetup(Target = nameof(ConnectionSorterV1Benchmark))]
        public void ConnectionSorterV1_Init()
        {
            InitCold(1000, 4000);
        }

        [IterationSetup(Target = nameof(ConnectionSorterBenchmark))]
        public void ConnectionSorter_Init()
        {
            InitCold(10_000, 4000);
        }

        [Benchmark]
        public void ConnectionSorterV1Benchmark()
        {
            for(int i = 0; i < _dataArr.Length; i++)
            {
                ConnectionData connData = _dataArr[i];
                ConnectionSorterV1.Sort(connData._connIdArrays,connData._weightArr);
            }
        }

        [Benchmark]
        public void ConnectionSorterBenchmark()
        {
            for(int i = 0; i < _dataArr.Length; i++)
            {
                ConnectionData connData = _dataArr[i];
                ConnectionSorter<double>.Sort(connData._connIdArrays,connData._weightArr);
            }
        }

        #endregion

        #region Private Methods

        private void InitCold(int count, int length)
        {
            _dataArr = new ConnectionData[count];
            for(int i=0; i < count; i++)
            {
                int[] srcIdArr = CreateRandomInt32Array(length);
                int[] tgtIdArr = CreateRandomInt32Array(length);

                ConnectionData connData = new()
                {
                    _connIdArrays = new ConnectionIdArrays(srcIdArr, tgtIdArr),
                    _weightArr = CreateRandomDoubleArray(length)
                };
                _dataArr[i] = connData;
            }
        }

        private void InitWarm()
        {
            for(int i=0; i < _dataArr.Length; i++)
            {
                ConnectionData connData = _dataArr[i];
                InitRandomInt32Array(connData._connIdArrays._sourceIdArr);
                InitRandomInt32Array(connData._connIdArrays._targetIdArr);
                InitRandomDoubleArray(connData._weightArr);
            }
        }

        private int[] CreateRandomInt32Array(int length)
        {
            int[] arr = new int[length];
            for(int i=0; i < length; i++) {
                arr[i] = _rng.Next();
            }
            return arr;
        }

        private double[] CreateRandomDoubleArray(int length)
        {
            double[] arr = new double[length];
            for(int i=0; i < length; i++) {
                arr[i] = _rng.NextDouble();
            }
            return arr;
        }

        private void InitRandomInt32Array(int[] arr)
        {
            for(int i=0; i < arr.Length; i++) {
                arr[i] = _rng.Next();
            }
        }

        private void InitRandomDoubleArray(double[] arr)
        {
            for(int i=0; i < arr.Length; i++) {
                arr[i] = _rng.NextDouble();
            }
        }

        #endregion

        private class ConnectionData
        {
            public ConnectionIdArrays _connIdArrays;
            public double[] _weightArr;
        }
    }
}
