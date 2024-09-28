using SharpNeat.Graphs;
using Xunit;

namespace SharpNeat.Neat.Genome;

public static class ConnectionCompareUtils
{
    #region Public Static Methods [Compare - No Weights]

    public static void CompareConnectionLists(
        DirectedConnection[] xConnArr,
        in ConnectionIds yIds)
    {
        int xlen = xConnArr.Length;
        Assert.Equal(xlen, yIds.GetSourceIdSpan().Length);
        Assert.Equal(xlen, yIds.GetTargetIdSpan().Length);

        for(int i=0; i < xlen; i++)
        {
            Assert.True(Equal(xConnArr, yIds, i, i));
        }
    }

    public static void CompareConnectionLists<TWeight>(
        ConnectionGenes<TWeight> x,
        in ConnectionIds yIds)
        where TWeight : struct
    {
        Assert.Equal(x.Length, yIds.GetSourceIdSpan().Length);
        Assert.Equal(x.Length, yIds.GetTargetIdSpan().Length);

        for(int i=0; i < x.Length; i++)
        {
            Assert.True(Equal(x, yIds, i, i));
        }
    }

    public static void CompareConnectionLists<TWeight>(
        ConnectionGenes<TWeight> x,
        in ConnectionIds yIds,
        int[] connectionIndexMap)
        where TWeight : struct
    {
        Assert.Equal(x.Length, yIds.GetSourceIdSpan().Length);
        Assert.Equal(x.Length, yIds.GetTargetIdSpan().Length);

        for(int i=0; i < x.Length; i++)
        {
            Assert.True(Equal(x, yIds, i, connectionIndexMap[i]));
        }
    }

    #endregion

    #region Public Static Methods [Compare - With Weights]

    public static void CompareConnectionLists<TWeight>(
        DirectedConnection[] xConnArr, TWeight[] xWeightArr,
        in ConnectionIds yIds, TWeight[] yWeightArr)
        where TWeight : struct
    {
        int xlen = xConnArr.Length;
        Assert.Equal(xlen, xWeightArr.Length);
        Assert.Equal(xlen, yIds.GetSourceIdSpan().Length);
        Assert.Equal(xlen, yIds.GetTargetIdSpan().Length);
        Assert.Equal(xlen, yWeightArr.Length);

        for(int i=0; i < xlen; i++)
        {
            Assert.True(Equal(xConnArr, xWeightArr, yIds, yWeightArr, i, i));
        }
    }

    public static void CompareConnectionLists<TWeight>(
        ConnectionGenes<TWeight> x,
        in ConnectionIds yIds,
        TWeight[] yWeightArr)
        where TWeight : struct
    {
        Assert.Equal(x.Length, yIds.GetSourceIdSpan().Length);
        Assert.Equal(x.Length, yIds.GetTargetIdSpan().Length);
        Assert.Equal(x.Length, yWeightArr.Length);

        for(int i=0; i < x.Length; i++)
        {
            Assert.True(Equal(x, yIds, yWeightArr, i, i));
        }
    }

    #endregion

    #region Private Static Methods

    private static bool Equal<TWeight>(
        ConnectionGenes<TWeight> x,
        in ConnectionIds yIds,
        int xIdx, int yIdx)
        where TWeight : struct
    {
        return Equal(
            x._connArr,
            yIds,
            xIdx, yIdx);
    }

    private static bool Equal(
        DirectedConnection[] xConnArr,
        in ConnectionIds yIds,
        int xIdx, int yIdx)
    {
        return xConnArr[xIdx].SourceId == yIds.GetSourceId(yIdx)
           &&  xConnArr[xIdx].TargetId == yIds.GetTargetId(yIdx);
    }

    private static bool Equal<TWeight>(
        ConnectionGenes<TWeight> x,
        in ConnectionIds yIds, TWeight[] yWeightArr,
        int xIdx, int yIdx)
        where TWeight : struct
    {
        return Equal(
            x._connArr, x._weightArr,
            yIds, yWeightArr,
            xIdx, yIdx);
    }

    private static bool Equal<TWeight>(
        DirectedConnection[] xConnArr, TWeight[] xWeightArr,
        in ConnectionIds yIds, TWeight[] yWeightArr,
        int xIdx, int yIdx)
        where TWeight : struct
    {
        return xConnArr[xIdx].SourceId == yIds.GetSourceId(yIdx)
           &&  xConnArr[xIdx].TargetId == yIds.GetTargetId(yIdx)
           &&  xWeightArr[xIdx].Equals(yWeightArr[yIdx]);
    }

    #endregion
}
