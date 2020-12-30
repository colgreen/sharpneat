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
using System.Collections.Generic;

namespace SharpNeat.Graphs
{
    /// <summary>
    /// Static utility methods for directed graph building.
    /// </summary>
    public static class DirectedGraphBuilderUtils
    {
        /// <summary>
        /// Create an <see cref="INodeIdMap"/> that provides a mapping from old to new node IDs.
        /// </summary>
        /// <param name="inputOutputCount">The total number of input and output nodes (input and output node IDs
        /// remain unchanged, i.e. are mapped with the identity mapping).</param>
        /// <param name="hiddenNodeIdArr">An array of hidden node IDs.</param>
        /// <returns>A new instance of <see cref="DictionaryNodeIdMap"/>.</returns>
        public static DictionaryNodeIdMap CompileNodeIdMap(
            int inputOutputCount,
            int[] hiddenNodeIdArr)
        {
            // Build dictionary of hidden node new ID/index keyed by old ID.
            // Note. the new IDs start immediately after the last input/output node ID (defined by inputOutputCount).
            var hiddenNodeIdxById = new Dictionary<int,int>(hiddenNodeIdArr.Length);

            for(int i=0, nodeIdx = inputOutputCount; i < hiddenNodeIdArr.Length; i++, nodeIdx++) {
                hiddenNodeIdxById.Add(hiddenNodeIdArr[i], nodeIdx);
            }

            return new DictionaryNodeIdMap(inputOutputCount, hiddenNodeIdxById);
        }
    }
}
