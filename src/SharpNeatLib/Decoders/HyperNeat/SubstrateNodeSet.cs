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
using System.Collections.Generic;

namespace SharpNeat.Decoders.HyperNeat
{
    /// <summary>
    /// Represenst a set of nodes on a substrate. Nodesets are used to represent sets such as the input and output nodes.
    /// Hidden nodes can be represented as layers with each layer being represented by a set. This allows connection mapping
    /// to be defined between between sets.
    /// </summary>
    public class SubstrateNodeSet
    {
        List<SubstrateNode> _nodePosList;

        #region Constructors

        /// <summary>
        /// Construct an empty nodeset. Node can be added after construction.
        /// </summary>
        public SubstrateNodeSet()
        {
            _nodePosList = new List<SubstrateNode>();
        }

        /// <summary>
        /// Construct an empty nodeset with an initial capacity. Node can be added after construction.
        /// </summary>
        public SubstrateNodeSet(int capacity)
        {
            _nodePosList = new List<SubstrateNode>(capacity);
        }

        /// <summary>
        /// Construct a nodeset with the provided list of nodes.
        /// </summary>
        public SubstrateNodeSet(List<SubstrateNode> nodePosList)
        {
            _nodePosList = nodePosList;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the nodes in the nodeset.
        /// </summary>
        public IList<SubstrateNode> NodeList
        {
            get { return _nodePosList; }
        }

        #endregion
    }
}
