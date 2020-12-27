/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using Redzen.Random;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Graphs;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    /// <summary>
    /// A NEAT genome asexual reproduction strategy based on adding a single node.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Offspring genomes are created by taking a clone of a single parent genome and adding a single node,
    /// if possible. A node is added by selecting a connection at random, and splitting it, i.e. replacing
    /// A → B with A → C → B, where A and B are the existing nodes, and C is the new node.
    /// </remarks>
    public sealed class AddNodeStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly INeatGenomeBuilder<T> _genomeBuilder;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        readonly Int32Sequence _generationSeq;
        readonly AddedNodeBuffer _addedNodeBuffer;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="metaNeatGenome">NEAT genome metadata.</param>
        /// <param name="genomeBuilder">NeatGenome builder.</param>
        /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
        /// <param name="innovationIdSeq">Innovation ID sequence; for obtaining new innovation IDs.</param>
        /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
        /// <param name="addedNodeBuffer">A history buffer of added nodes.</param>
        public AddNodeStrategy(
            MetaNeatGenome<T> metaNeatGenome,
            INeatGenomeBuilder<T> genomeBuilder,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq,
            AddedNodeBuffer addedNodeBuffer)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeBuilder = genomeBuilder;
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _generationSeq = generationSeq;
            _addedNodeBuffer = addedNodeBuffer;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new child genome from a given parent genome.
        /// </summary>
        /// <param name="parent">The parent genome.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new child genome.</returns>
        public NeatGenome<T>? CreateChildGenome(NeatGenome<T> parent, IRandomSource rng)
        {
            if(parent.ConnectionGenes.Length == 0)
            {
                // No connections to split (nodes are added by splitting an existing connection).
                return null;
            }

            // Select a connection at random.
            int splitConnIdx = rng.Next(parent.ConnectionGenes.Length);
            var splitConn = parent.ConnectionGenes._connArr[splitConnIdx];

            // The selected connection will be replaced with a new node and two new connections;
            // get an innovation ID for the new node.
            int addedNodeId = GetInnovationID(splitConn, parent, out bool newInnovationIdsFlag);

            // Create the two new connections.
            var newConnArr = new DirectedConnection[] {
                new DirectedConnection(splitConn.SourceId, addedNodeId),
                new DirectedConnection(addedNodeId, splitConn.TargetId)
            };

            // Get weights for the new connections.
            // Connection 1 gets the weight from the original connection; connection 2 gets a fixed
            // weight of _metaNeatGenome.ConnectionWeightRange.

            // ENHANCEMENT: Consider a better choice of weights for the new connections; this scheme has been
            // copied from sharpneat 2.x as a starting point, but can likely be improved upon.
            var newWeightArr = new T[] {
                parent.ConnectionGenes._weightArr[splitConnIdx],
                (T)Convert.ChangeType(_metaNeatGenome.ConnectionWeightScale, typeof(T))
            };

            // Ensure newConnArr is sorted.
            // Later on we'll determine their insertion indexes into the connection array, therefore this ensures that
            // the insert indexes will be sorted correctly.
            if(newConnArr[0].CompareTo(newConnArr[1]) > 0)
            {
                var tmpConn = newConnArr[0];
                newConnArr[0] = newConnArr[1];
                newConnArr[1] = tmpConn;

                T tmpWeight = newWeightArr[0];
                newWeightArr[0] = newWeightArr[1];
                newWeightArr[1] = tmpWeight;
            }

            // Create a new connection gene array that consists of the parent connection genes,
            // with the connection that was split removed, and the two new connection genes that
            // replace it inserted at the correct (sorted) positions.
            var parentConnArr = parent.ConnectionGenes._connArr;
            var parentWeightArr = parent.ConnectionGenes._weightArr;
            int parentLen = parentConnArr.Length;

            // Create the child genome's ConnectionGenes object.
            int childLen = parentLen + 1;
            var connGenes = new ConnectionGenes<T>(childLen);
            var connArr = connGenes._connArr;
            var weightArr = connGenes._weightArr;

            // Build an array of parent indexes to stop at when copying from the parent to the child connection array.
            // Note. Each index is combined with a second value; an index into newConnArr for insertions,
            // and -1 for the split index (the connection to be removed)
            int insertIdx1 = ~Array.BinarySearch(parent.ConnectionGenes._connArr, newConnArr[0]);
            int insertIdx2 = ~Array.BinarySearch(parent.ConnectionGenes._connArr, newConnArr[1]);
            (int,int)[] stopIdxArr = new []
            {
                (splitConnIdx, -1),
                (insertIdx1, 0),
                (insertIdx2, 1)
            };

            // Sort by the first index value.
            Array.Sort(stopIdxArr, ((int,int)x, (int,int)y) => x.Item1.CompareTo(y.Item1));

            // Loop over stopIdxArr.
            int parentIdx = 0;
            int childIdx = 0;

            for(int i=0; i<stopIdxArr.Length; i++)
            {
                int stopIdx = stopIdxArr[i].Item1;
                int newConIdx = stopIdxArr[i].Item2;

                // Copy all parent genes up to the stop index.
                int copyLen = stopIdx - parentIdx;
                if(copyLen > 0)
                {
                    Array.Copy(parentConnArr, parentIdx, connArr, childIdx, copyLen);
                    Array.Copy(parentWeightArr, parentIdx, weightArr, childIdx, copyLen);
                }

                // Update parentIdx, childIdx.
                parentIdx = stopIdx;
                childIdx += copyLen;

                // Test what to do at the stopIdx.
                if(newConIdx == -1)
                {   // We are at the parent connection to be skipped.
                    parentIdx++;
                    continue;
                }

                // We are at an insertion point in connArr.
                connArr[childIdx] = newConnArr[newConIdx];
                weightArr[childIdx] = newWeightArr[newConIdx];

                childIdx++;
            }

            // Copy any remaining connection genes.
            int len = parentConnArr.Length - parentIdx;
            if (len > 0)
            {
                Array.Copy(parentConnArr, parentIdx, connArr, childIdx, len);
                Array.Copy(parentWeightArr, parentIdx, weightArr, childIdx, len);
            }

            // Note. We can construct a NeatGenome without passing the pre-built arrays connIdxArr and hiddenNodeIdArr;
            // however this way is more efficient. The downside is that the logic to pre-build these arrays is highly complex
            // and therefore difficult to understand, modify, and is thus a possible source of defects if modifications are attempted.

            // Create an array of hidden node IDs.
            var hiddenNodeIdArr = GetHiddenNodeIdArray(parent, addedNodeId, newInnovationIdsFlag);

            // Create and return a new genome.
            return _genomeBuilder.Create(
                _genomeIdSeq.Next(),
                _generationSeq.Peek,
                connGenes,
                hiddenNodeIdArr);
        }

        #endregion

        #region Private Methods

        private int GetInnovationID(
            in DirectedConnection splitConn,
            NeatGenome<T> parent,
            out bool newInnovationIdFlag)
        {
            // Test if the selected connection has a previous split recorded in the innovation ID buffer.
            if(_addedNodeBuffer.TryLookup(in splitConn, out int addedNodeId))
            {
                // Found existing matching structure.
                // However we can only re-use the ID from that structure if it isn't already present in the current genome;
                // this can happen if a connection was split previously, and now another connection between the same source
                // and target nodes exists and is also being split.
                if(!parent.ContainsHiddenNode(addedNodeId))
                {
                    // The node ID from the buffer is not present on the parent genome, therefore we can re-use it.
                    newInnovationIdFlag = false;
                    return addedNodeId;
                }

                // We can't re-use the ID from the buffer, so allocate a new ID.
                // Note. this new ID isn't added to the buffer; instead we leave the existing buffer entry for splitConnId in place.
                newInnovationIdFlag = true;
                return _innovationIdSeq.Next();
            }

            // No buffer entry found, therefore we allocate a new ID.
            newInnovationIdFlag = true;
            addedNodeId = _innovationIdSeq.Next();

            // Register the new ID with the buffer.
            _addedNodeBuffer.Register(in splitConn, addedNodeId);

            return addedNodeId;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Get an array of hidden node IDs in the child genome.
        /// </summary>
        public static int[] GetHiddenNodeIdArray(
            NeatGenome<T> parent,
            int addedNodeId,
            bool newInnovationIdsFlag)
        {
            int[] parentIdArr = parent.HiddenNodeIdArray;
            int childLen = parentIdArr.Length + 1;
            int[] childIdArr = new int[childLen];

            // New innovation IDs are always higher than any existing IDs, therefore adding
            // the new node ID to the end of the list will maintain sorter order.
            if(newInnovationIdsFlag)
            {
                Array.Copy(parentIdArr, childIdArr, parentIdArr.Length);
                childIdArr[^1] = addedNodeId;
                return childIdArr;
            }

            // Determine the insertion index for the new node ID.
            int insertIdx = ~Array.BinarySearch(parentIdArr, addedNodeId);

            // Copy all IDs up to the insertion index.
            Array.Copy(parentIdArr, 0, childIdArr, 0, insertIdx);

            // Insert the added node ID.
            childIdArr[insertIdx] = addedNodeId;

            // Copy all remaining IDs after the index.
            Array.Copy(parentIdArr, insertIdx, childIdArr, insertIdx+1, parentIdArr.Length - insertIdx);

            return childIdArr;
        }

        #endregion
    }
}
