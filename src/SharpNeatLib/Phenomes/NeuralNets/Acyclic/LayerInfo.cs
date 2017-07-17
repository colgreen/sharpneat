/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.Phenomes.NeuralNets.Acyclic
{
    /// <summary>
    /// Stores a node and connection index that represent a layer within the network 
    /// (the nodes and connections at a given depth in a network).
    /// </summary>
    public struct LayerInfo
    {
        /// <summary>
        /// The index (+1) of the last node in the layer.
        /// </summary>
        public int _endNodeIdx;
        /// <summary>
        /// The index (+1) of the last connection in the layer.
        /// </summary>        
        public int _endConnectionIdx;
    }
}
