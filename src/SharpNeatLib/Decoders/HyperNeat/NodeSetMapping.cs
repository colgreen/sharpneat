/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System.Collections.Generic;

namespace SharpNeat.Decoders.HyperNeat
{
    /// <summary>
    /// Describes a mapping between nodesets.
    /// Packages an INodeSetMappingFunction with indexes into a list of node sets that identify the source
    /// and target nodesets for the mapping. 
    /// </summary>
    public class NodeSetMapping
    {
        int _srcNodeSetIdx;
        int _tgtNodeSetIdx;
        INodeSetMappingFunction _mappingFn;

        #region Constructor

        /// <summary>
        /// Constructs with the provided source and target nodeset indexes and mapping function to apply between those sets.
        /// </summary>
        public NodeSetMapping(int srcNodeSetIdx, int tgtNodeSetIdx, INodeSetMappingFunction mappingFn)
        {
            _srcNodeSetIdx = srcNodeSetIdx;
            _tgtNodeSetIdx = tgtNodeSetIdx;
            _mappingFn = mappingFn;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the index of the source nodeset in some list of nodesets.
        /// </summary>
        public int SourceNodeSetIdx
        {
            get { return _srcNodeSetIdx; }
        }

        /// <summary>
        /// Gets the index of the target nodeset in some list of nodesets.
        /// </summary>
        public int TargetNodeSetIdx
        {
            get { return _tgtNodeSetIdx; }
        }

        /// <summary>
        /// Gets the mapping function to apply between the source and target nodesets.
        /// </summary>
        public INodeSetMappingFunction MappingFunction
        {
            get { return _mappingFn; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates the connections defined by the mapping.
        /// </summary>
        public IEnumerable<SubstrateConnection> GenerateConnections(List<SubstrateNodeSet> nodeSetList)
        {
            return _mappingFn.GenerateConnections(nodeSetList[_srcNodeSetIdx],
                                                  nodeSetList[_tgtNodeSetIdx]);
        }

        /// <summary>
        /// Returns an estimate/hint for the number of connections that would be created by the mapping.
        /// </summary>
        public int GetConnectionCountHint(List<SubstrateNodeSet> nodeSetList)
        {
            return _mappingFn.GetConnectionCountHint(nodeSetList[_srcNodeSetIdx],
                                                     nodeSetList[_tgtNodeSetIdx]);
        }

        #endregion

        #region Publc Static Methods [Convenient Factory Methods]

        /// <summary>
        /// Creates a NodeSet from the provided source and target nodeset indexes and mapping function.
        /// </summary>
        public static NodeSetMapping Create(int srcNodeSetIdx, int tgtNodeSetIdx, INodeSetMappingFunction mappingFn)
        {
            return new NodeSetMapping(srcNodeSetIdx, tgtNodeSetIdx, mappingFn);
        }

        /// <summary>
        /// Creates a NodeSet from the provided source and target nodeset indexes and maximum connection distance for mappings/connections.
        /// </summary>
        public static NodeSetMapping Create(int srcNodeSetIdx, int tgtNodeSetIdx, double? maximumConnectionDistance)
        {
            return new NodeSetMapping(srcNodeSetIdx, tgtNodeSetIdx, new DefaultNodeSetMappingFunction(maximumConnectionDistance, false));
        }

        /// <summary>
        /// Creates a NodeSet from the provided source and target nodeset indexes, maximum connection distance for mappings/connections and 
        /// a flag defining if local recurrent conenctions should be created when mapping between nodes in the same nodeset.
        /// </summary>
        public static NodeSetMapping Create(int srcNodeSetIdx, int tgtNodeSetIdx, double? maximumConnectionDistance,  bool allowLocalRecurrentConnections)
        {
            return new NodeSetMapping(srcNodeSetIdx, tgtNodeSetIdx, new DefaultNodeSetMappingFunction(maximumConnectionDistance, allowLocalRecurrentConnections));
        }

        #endregion
    }
}
