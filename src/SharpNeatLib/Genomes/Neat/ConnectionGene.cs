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
using SharpNeat.Network;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// A gene that represents a single connection between neurons in NEAT.
    /// </summary>
    public class ConnectionGene : INetworkConnection
    {
        uint _innovationId;
        uint _sourceNodeId;
        uint _targetNodeId;
        double _weight;

        /// <summary>
        /// Used by the connection mutation routine to flag mutated connections so that they aren't
        /// mutated more than once.
        /// </summary>
        bool _isMutated = false;

        #region Constructor

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public ConnectionGene(ConnectionGene copyFrom)
        {
            _innovationId = copyFrom._innovationId;
            _sourceNodeId = copyFrom._sourceNodeId;
            _targetNodeId = copyFrom._targetNodeId;
            _weight = copyFrom._weight;
        }

        /// <summary>
        /// Construct a new ConnectionGene with the specified source and target neurons and connection weight.
        /// </summary>
        public ConnectionGene(uint innovationId, uint sourceNodeId, uint targetNodeId, double weight)
        {
            _innovationId = innovationId;
            _sourceNodeId = sourceNodeId;
            _targetNodeId = targetNodeId;
            _weight = weight;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the gene's innovation ID.
        /// </summary>
        public uint InnovationId
        {
            get { return _innovationId; }
            set { _innovationId = value; }
        }

        /// <summary>
        /// Gets or sets the gene's source neuron/node ID.
        /// </summary>
        public uint SourceNodeId
        {
            get { return _sourceNodeId; }
            set { _sourceNodeId = value; }
        }

        /// <summary>
        /// Gets or sets the gene's target neuron/node ID.
        /// </summary>
        public uint TargetNodeId
        {
            get { return _targetNodeId; }
            set { _targetNodeId = value; }
        }

        /// <summary>
        /// Gets or sets the gene's connection weight.
        /// </summary>
        public double Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this gene has been mutated. This allows the mutation routine to avoid mutating
        /// genes it has already operated on. These flags are reset for all Connection genes within a NeatGenome on exiting
        /// the mutation routine.
        /// </summary>
        public bool IsMutated
        {
            get { return _isMutated; }
            set { _isMutated = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a copy of the current gene. Virtual method that can be overriden by sub-types.
        /// </summary>
        public virtual ConnectionGene CreateCopy()
        {
            return new ConnectionGene(this);
        }

        #endregion
    }
}
