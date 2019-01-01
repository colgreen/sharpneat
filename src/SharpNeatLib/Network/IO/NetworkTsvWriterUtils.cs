/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.NeuralNets;

namespace SharpNeat.Network.IO
{
    /// <summary>
    /// Static utility methods for writing network definitions to tab separated file (tsv) format.
    /// </summary>
    public static class NetworkTsvWriterUtils
    {
        #region Public Static Methods

        public static void WriteActivationFunctionsSection(string activationFnName, StreamWriter sw)
        {
            sw.WriteLine("# Activation Functions.");
            sw.WriteLine($"{0}\t{activationFnName}");
            sw.WriteLine();
        }

        public static void WriteNodesSection(DirectedGraph digraph, StreamWriter sw)
        {
            sw.WriteLine("# Nodes.");

            var nodeIdList = ResolveUniqueNodeIdList(digraph.ConnectionArray);

            foreach(int nodeId in nodeIdList) {
                sw.WriteLine(nodeId);
            }

            sw.WriteLine();
        }

        public static void WriteConnectionsSection(DirectedConnection[] connArr, double[] weightArr, StreamWriter sw)
        {
            sw.WriteLine("# Connections.");

            for(int i=0; i < connArr.Length; i++)
            {
                var conn = connArr[i];
                double weight = weightArr[i];
                sw.WriteLine($"{conn.SourceId}\t{conn.TargetId}\t{weight}");
            }

            sw.WriteLine();
        }

        #endregion

        #region Private Static Methods

        private static IList<int> ResolveUniqueNodeIdList(DirectedConnection[] connArr)
        {
            HashSet<int> idSet = new HashSet<int>();

            foreach(DirectedConnection conn in connArr)
            {
                idSet.Add(conn.SourceId);
                idSet.Add(conn.TargetId);
            }

            var idList = idSet.ToList();
            idList.Sort();
            return idList;
        }

        #endregion
    }
}
