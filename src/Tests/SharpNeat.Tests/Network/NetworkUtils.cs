using System.Collections.Generic;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;
using Xunit;

namespace SharpNeat.Tests.Network
{
    public static class NetworkUtils
    {
        #region Public Static Methods

        public static void CompareConnectionLists(
            IList<WeightedDirectedConnection<double>> x,
            in ConnectionIdArrays connIdArrays, double[] yWeightArr)
        {
            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            Assert.Equal(x.Count, srcIdArr.Length);
            Assert.Equal(x.Count, tgtIdArr.Length);

            for(int i=0; i < x.Count; i++) {
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

        public static void CompareLayerInfoLists(IList<LayerInfo> x, IList<LayerInfo> y)
        {
            Assert.Equal(x.Count, y.Count);
            for(int i=0; i < x.Count; i++) {
                CompareLayerInfo(x[i], y[i]);
            }
        }

        public static void CompareLayerInfo(in LayerInfo x, in LayerInfo y)
        {
            Assert.Equal(x.EndNodeIdx, y.EndNodeIdx);
            Assert.Equal(x.EndConnectionIdx, y.EndConnectionIdx);
        }

        #endregion
    }
}
