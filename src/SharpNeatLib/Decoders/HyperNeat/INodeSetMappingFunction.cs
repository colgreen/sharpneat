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
    /// Implementations of INodeSetMappingFunction define a mapping between source and target node sets. 
    /// Each mapping is interpreted/used as a connection between the source and target node.
    /// E.g. the simplest such function is to map every source node to every target node (N*M connections).
    /// </summary>
    public interface INodeSetMappingFunction
    {
        /// <summary>
        /// Returns an IEnumerable that yields the mappings/connections defined by the mapping function (from the source nodes to
        /// the target nodes) as a sequence. The alternative of returning a list would require a very long list in extreme scenarios; 
        /// this approach minimizes down memory usage.
        /// </summary>
        IEnumerable<SubstrateConnection> GenerateConnections(SubstrateNodeSet srcNodeSet, SubstrateNodeSet tgtNodeSet);

        /// <summary>
        /// Returns an estimate/hint for the number of connections that would be created between the provided source and target node sets.
        /// </summary>
        int GetConnectionCountHint(SubstrateNodeSet srcNodeSet, SubstrateNodeSet tgtNodeSet);
    }
}
