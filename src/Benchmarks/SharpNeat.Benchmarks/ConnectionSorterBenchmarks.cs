﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Redzen.Random;
using SharpNeat.Graphs;

namespace SharpNeat;

[SimpleJob(RunStrategy.Monitoring)]
[MemoryDiagnoser]
public class ConnectionSorterBenchmarks
{
    readonly Xoshiro256StarStarRandom _rng = new(123);
    ConnectionData[] _dataArr;

    public ConnectionSorterBenchmarks()
    {
        InitCold(1000, 4000);
    }

    #region Public Methods

    [IterationSetup(Target = nameof(ConnectionSorterV1Benchmark))]
    public void ConnectionSorterV1_Init()
    {
        InitWarm();
    }

    [IterationSetup(Target = nameof(ConnectionSorterBenchmark))]
    public void ConnectionSorter_Init()
    {
        InitWarm();
    }

    [Benchmark]
    public void ConnectionSorterV1Benchmark()
    {
        for(int i = 0; i < _dataArr.Length; i++)
        {
            ConnectionData connData = _dataArr[i];
            ConnectionSorterV1.Sort(connData._connIds, connData._weightArr);
        }
    }

    [Benchmark]
    public void ConnectionSorterBenchmark()
    {
        for(int i = 0; i < _dataArr.Length; i++)
        {
            ConnectionData connData = _dataArr[i];
            ConnectionSorter<float>.Sort(connData._connIds, connData._weightArr);
        }
    }

    #endregion

    #region Private Methods

    private void InitCold(int count, int length)
    {
        _dataArr = new ConnectionData[count];
        for(int i=0; i < count; i++)
        {
            ConnectionData connData = new()
            {
                _connIds = new ConnectionIds(length),
                _weightArr = CreateRandomFloatArray(length)
            };

            InitRandomInt32Array(connData._connIds.GetSourceIdSpan());
            InitRandomInt32Array(connData._connIds.GetTargetIdSpan());
            _dataArr[i] = connData;
        }
    }

    private void InitWarm()
    {
        for(int i=0; i < _dataArr.Length; i++)
        {
            ConnectionData connData = _dataArr[i];
            InitRandomInt32Array(connData._connIds.GetSourceIdSpan());
            InitRandomInt32Array(connData._connIds.GetTargetIdSpan());
            InitRandomFloatArray(connData._weightArr);
        }
    }

    private float[] CreateRandomFloatArray(int length)
    {
        float[] arr = new float[length];
        for(int i=0; i < length; i++)
            arr[i] = _rng.NextFloat();

        return arr;
    }

    private void InitRandomInt32Array(Span<int> span)
    {
        for(int i = 0; i < span.Length; i++)
            span[i] = _rng.Next();
    }

    private void InitRandomFloatArray(float[] arr)
    {
        for(int i = 0; i < arr.Length; i++)
            arr[i] = _rng.NextFloat();
    }

    #endregion

    private sealed class ConnectionData
    {
        public ConnectionIds _connIds;
        public float[] _weightArr;
    }
}
