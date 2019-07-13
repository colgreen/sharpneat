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

namespace SharpNeat.Network.Acyclic
{
    /// <summary>
    /// Represents a weighted acyclic directed graph.
    /// </summary>
    public class WeightedAcyclicDirectedGraph<T> : DirectedGraphAcyclic
        where T : struct
    {
        /// <summary>
        /// Connection weight array.
        /// </summary>
        public T[] WeightArray { get; }

        #region Constructor

        internal WeightedAcyclicDirectedGraph(
            in ConnectionIdArrays connIdArrays,
            int inputCount,
            int outputCount,
            int nodeCount,
            LayerInfo[] layerArr,
            int[] outputNodeIdxArr,
            T[] weightArr) 
        : base(in connIdArrays, inputCount, outputCount, nodeCount, layerArr, outputNodeIdxArr)
        {
            this.WeightArray = weightArr;
        }

        #endregion
    }
}
