using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeatLib.Tests.Network
{
    public static class NetworkUtils
    {
        #region Public Static Methods

        public static void CompareConnectionLists(IList<IWeightedDirectedConnection<double>> x,
                                                  ConnectionIdArrays connIdArrays, double[] yWeightArr)
        {
            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            Assert.AreEqual(x.Count, srcIdArr.Length);
            Assert.AreEqual(x.Count, tgtIdArr.Length);

            for(int i=0; i<x.Count; i++) {
                CompareConnections(x[i], srcIdArr[i], tgtIdArr[i], yWeightArr[i]);
            }
        }

        public static void CompareConnectionLists(IList<IWeightedDirectedConnection<double>> x,
                                                  IList<DirectedConnection> y, double[] yWeightArr)
        {
            Assert.AreEqual(x.Count, y.Count);
            for(int i=0; i<x.Count; i++) {
                CompareConnections(x[i], y[i], yWeightArr[i]);
            }
        }

        public static void CompareConnections(IWeightedDirectedConnection<double> x,
                                              DirectedConnection y, double yWeight)
        {
            Assert.AreEqual(x.SourceId, y.SourceId);
            Assert.AreEqual(x.TargetId, y.TargetId);
            Assert.AreEqual(x.Weight, yWeight);
        }

        public static void CompareConnections(IWeightedDirectedConnection<double> x,
                                              int ySrcId, int yTgtId, double yWeight)
        {
            Assert.AreEqual(x.SourceId, ySrcId);
            Assert.AreEqual(x.TargetId, yTgtId);
            Assert.AreEqual(x.Weight, yWeight);
        }

        public static void CompareLayerInfoLists(IList<LayerInfo> x, IList<LayerInfo> y)
        {
            Assert.AreEqual(x.Count, y.Count);
            for(int i = 0; i < x.Count; i++) {
                CompareLayerInfo(x[i], y[i]);
            }
        }

        public static void CompareLayerInfo(LayerInfo x, LayerInfo y)
        {
            Assert.AreEqual(x.EndNodeIdx, y.EndNodeIdx);
            Assert.AreEqual(x.EndConnectionIdx, y.EndConnectionIdx);
        }

        #endregion
    }
}
