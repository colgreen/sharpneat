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
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome
{
    internal static class NeatGenomeBuilderUtils
    {
        #region Public Static Methods

        public static DirectedGraph CreateDirectedGraph<T>(
            MetaNeatGenome<T> metaNeatGenome,
            ConnectionGenes<T> connGenes,
            INodeIdMap nodeIndexByIdMap)
            where T : struct
        {
            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            CopyAndMapIds(
                connGenes._connArr,
                nodeIndexByIdMap,
                out ConnectionIdArrays connIdArrays);

            // Construct a new DirectedGraph.
            var digraph = new DirectedGraph(
                connIdArrays,
                metaNeatGenome.InputNodeCount,
                metaNeatGenome.OutputNodeCount,
                nodeIndexByIdMap.Count);

            return digraph;
        }

        #endregion

        #region Private Static Methods

        private static void CopyAndMapIds(
            DirectedConnection[] connArr,
            INodeIdMap nodeIdMap,
            out ConnectionIdArrays connIdArrays)
        {
            int count = connArr.Length;
            int[] srcIdArr = new int[count];
            int[] tgtIdArr = new int[count];

            for(int i=0; i < count; i++) 
            {
                srcIdArr[i] = nodeIdMap.Map(connArr[i].SourceId);
                tgtIdArr[i] = nodeIdMap.Map(connArr[i].TargetId);
            }

            connIdArrays = new ConnectionIdArrays(srcIdArr, tgtIdArr);
        }

        #endregion
    }
}
