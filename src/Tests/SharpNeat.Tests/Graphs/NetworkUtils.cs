﻿using SharpNeat.Graphs.Acyclic;
using Xunit;

namespace SharpNeat.Graphs;

public static class NetworkUtils
{
    public static void CompareConnectionLists(
        Span<WeightedDirectedConnection<double>> x,
        in ConnectionIds connIds, double[] yWeightArr)
    {
        ReadOnlySpan<int> srcIdArr = connIds.GetSourceIdSpan();
        ReadOnlySpan<int> tgtIdArr = connIds.GetTargetIdSpan();

        Assert.Equal(x.Length, srcIdArr.Length);
        Assert.Equal(x.Length, tgtIdArr.Length);

        for(int i=0; i < x.Length; i++)
        {
            CompareConnections(x[i], srcIdArr[i], tgtIdArr[i], yWeightArr[i]);
        }
    }

    public static void CompareConnections(
        in WeightedDirectedConnection<double> x,
        int ySrcId, int yTgtId, double yWeight)
    {
        Assert.Equal(x.SourceId, ySrcId);
        Assert.Equal(x.TargetId, yTgtId);
        Assert.Equal(x.Weight, yWeight);
    }

    public static void CompareLayerInfoLists(
        Span<LayerInfo> x,
        Span<LayerInfo> y)
    {
        Assert.Equal(x.Length, y.Length);
        for(int i=0; i < x.Length; i++)
        {
            CompareLayerInfo(x[i], y[i]);
        }
    }

    public static void CompareLayerInfo(in LayerInfo x, in LayerInfo y)
    {
        Assert.Equal(x.EndNodeIdx, y.EndNodeIdx);
        Assert.Equal(x.EndConnectionIdx, y.EndConnectionIdx);
    }
}
