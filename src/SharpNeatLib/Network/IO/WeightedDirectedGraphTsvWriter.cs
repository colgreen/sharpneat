using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.NeuralNets;
using static SharpNeat.Network.IO.NetworkTsvWriterUtils;

namespace SharpNeat.Network.IO
{
    /// <summary>
    /// Writes a WeightedDirectedGraph to file in a tab separated file (tsv) based format.
    /// </summary>
    public static class WeightedDirectedGraphTsvWriter
    {
        #region Public Static Methods

        public static void Save(WeightedDirectedGraph<double> digraph, string activationFnName, string path)
        {
            using(var sw = new StreamWriter(path)) {
                Write(digraph, activationFnName, sw);
            }
        }

        public static void Write(WeightedDirectedGraph<double> digraph, string activationFnName, Stream strm)
        {
            using(StreamWriter sw = new StreamWriter(strm)) {
                Write(digraph, activationFnName, sw);
            }
        }

        public static void Write(WeightedDirectedGraph<double> digraph, string activationFnName, StreamWriter sw)
        {
            WriteActivationFunctionsSection(activationFnName, sw);
            WriteNodesSection(digraph, sw);
            WriteConnectionsSection(digraph.ConnectionArray, digraph.WeightArray, sw);
        }

        #endregion
    }
}
