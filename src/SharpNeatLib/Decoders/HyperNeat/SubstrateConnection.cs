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
namespace SharpNeat.Decoders.HyperNeat
{
    /// <summary>
    /// Represents a connection between two nodes in a HyperNEAT substrate.
    /// The node positions are represented as arrays of numbers so as not to limit the number of
    /// dimensions that positions (and therefore substrates) can be defined within.
    /// </summary>
    public class SubstrateConnection
    {
        /// <summary>The source node.</summary>
        public SubstrateNode _srcNode;
        /// <summary>The target node.</summary>
        public SubstrateNode _tgtNode;

        /// <summary>
        /// Constructs with the specified source and target substrate nodes.
        /// </summary>
        public SubstrateConnection(SubstrateNode srcNode, SubstrateNode tgtNode)
        {
            _srcNode = srcNode;
            _tgtNode = tgtNode;
        }
    }
}
