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
using SharpNeat.Network;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Used for building a list of connection genes. 
    /// 
    /// Connection genes are added one by one to a list and a dictionary of added connection genes is maintained
    /// keyed on ConnectionEndpointsStruct to allow a caller to check if a connection with the same end points
    /// (and potentially a different innovation ID) already exists in the list.
    /// </summary>
    public class ConnectionGeneListBuilder
    {
        readonly ConnectionGeneList _connectionGeneList;
        readonly Dictionary<ConnectionEndpointsStruct,ConnectionGene> _connectionGeneDictionary;
        readonly SortedDictionary<uint,uint> _neuronIdDictionary;

        #region Constructor

        /// <summary>
        /// Constructs the builder with the provided capacity. The capacity should be chosen 
        /// to limit the number of memory re-allocations that occur within the contained
        /// connection list dictionary.
        /// </summary>
        public ConnectionGeneListBuilder(int connectionCapacity)
        {
            _connectionGeneList = new ConnectionGeneList(connectionCapacity);
            _connectionGeneDictionary = new Dictionary<ConnectionEndpointsStruct,ConnectionGene>(connectionCapacity);
            _neuronIdDictionary = new SortedDictionary<uint,uint>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the contained list of connection genes.
        /// </summary>
        public ConnectionGeneList ConnectionGeneList
        {
            get { return _connectionGeneList; }
        }

        /// <summary>
        /// Gets the builder's dictionary of connection genes keyed on ConnectionEndpointsStruct.
        /// </summary>
        public Dictionary<ConnectionEndpointsStruct,ConnectionGene> ConnectionGeneDictionary
        {
            get { return _connectionGeneDictionary; }
        }

        /// <summary>
        /// Gets the builder's dictionary of neuron IDs obtained from contained connection gene endpoints.
        /// </summary>
        public SortedDictionary<uint,uint> NeuronIdDictionary
        {
            get { return _neuronIdDictionary; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a connection gene. Register it with the connection gene dictionary (keyed on end points,
        /// not on innovation ID) and register the neuron IDs (from the connection end points) with the neuron 
        /// ID dictionary.
        /// </summary>
        public void Append(ConnectionGene connectionGene, ConnectionEndpointsStruct connectionKey)
        {
            // Add the gene and register it with the connection gene dictionary (keyed on end 
            // points, not on innovation ID).
            _connectionGeneList.Add(connectionGene);
            _connectionGeneDictionary.Add(connectionKey, connectionGene);

            // Register encountered neuron IDs with neuronIdDictionary.
            _neuronIdDictionary[connectionGene.SourceNodeId] = connectionGene.SourceNodeId;
            _neuronIdDictionary[connectionGene.TargetNodeId] = connectionGene.TargetNodeId;
        }

        #endregion
    }
}
