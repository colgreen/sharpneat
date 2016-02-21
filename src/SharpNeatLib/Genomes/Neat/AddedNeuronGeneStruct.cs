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

using SharpNeat.Utility;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Represents an added neuron. When a neuron is added to a neural network in NEAT an existing
    /// connection between two neurons is discarded and replaced with the new neuron and two new connections,
    /// one connection between the source neuron and the new neuron and another from the new neuron to the target neuron.
    /// This struct represents those three IDs.
    /// 
    /// This struct exists to represent newly added structure in a history buffer of added structures. This allows us to 
    /// re-use IDs where a mutation recreates a structure that has previously occured through previous mutations on other 
    /// genomes.
    /// </summary>
    public struct AddedNeuronGeneStruct
    {
        readonly uint _addedNeuronId;
        readonly uint _addedInputConnectionId;
        readonly uint _addedOutputConnectionId;

        #region Constructor

        /// <summary>
        /// Construct by assigning new IDs gemnerated by the provided UInt32IdGenerator.
        /// </summary>
        public AddedNeuronGeneStruct(UInt32IdGenerator idGenerator)
        {
            _addedNeuronId = idGenerator.NextId;
            _addedInputConnectionId = idGenerator.NextId;
            _addedOutputConnectionId = idGenerator.NextId;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the added neuron's ID.
        /// </summary>
        public uint AddedNeuronId
        {
            get { return _addedNeuronId; }
        }

        /// <summary>
        /// Gets the added input connection's ID.
        /// </summary>
        public uint AddedInputConnectionId
        {
            get { return _addedInputConnectionId; }
        }

        /// <summary>
        /// Gets the added output connection's ID.
        /// </summary>
        public uint AddedOutputConnectionId
        {
            get { return _addedOutputConnectionId; }
        }

        #endregion
    }                   
}
