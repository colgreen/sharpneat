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
    /// A gene that represents a single neuron in NEAT.
    /// </summary>
    public class NeuronGene : INetworkNode
    {
        /// <summary>
        /// Although this ID is allocated from the global innovation ID pool, neurons do not participate 
        /// in compatibility measurements and so it is not actually used as an innovation ID. 
        /// </summary>
        readonly uint _innovationId;
        readonly NodeType _neuronType;
        readonly int _activationFnId;
        readonly double[] _auxState;

        #region Constructor

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public NeuronGene(NeuronGene copyFrom)
        {
            _innovationId = copyFrom._innovationId;
            _neuronType = copyFrom._neuronType;
            _activationFnId = copyFrom._activationFnId;
            if(null != copyFrom._auxState) {
                _auxState = (double[])copyFrom._auxState.Clone();
            }
        }

        /// <summary>
        /// Construct new NeuronGene with the specified innovationId, neuron type 
        /// and activation function ID.
        /// </summary>
        public NeuronGene(uint innovationId, NodeType neuronType, int activationFnId)
        {
            _innovationId = innovationId;
            _neuronType = neuronType;
            _activationFnId = activationFnId;
            _auxState = null;
        }

        /// <summary>
        /// Construct new NeuronGene with the specified innovationId, neuron type 
        /// activation function ID and auxiliary state data.
        /// </summary>
        public NeuronGene(uint innovationId, NodeType neuronType, int activationFnId, double[] auxState)
        {
            _innovationId = innovationId;
            _neuronType = neuronType;
            _activationFnId = activationFnId;
            _auxState = auxState;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the neuron's innovation ID.
        /// </summary>
        public uint InnovationId
        {
            get { return _innovationId; }
        }

        /// <summary>
        /// Gets the neuron's type.
        /// </summary>
        public NodeType NodeType
        {
            get { return _neuronType; }
        }

        /// <summary>
        /// Gets the neuron's activation function ID. 
        /// For NEAT this is not used and will always be zero.
        /// For CPPNs/HyperNEAT this ID corresponds to an entry in the IActivationFunctionLibrary
        /// present in the current genome factory.
        /// </summary>
        public int ActivationFnId
        {
            get { return _activationFnId; }
        }

        /// <summary>
        /// Optional auxilliary node state. Null if no aux state is present. 
        /// Note. Radial Basis Function center and epsilon values are stored here.
        /// </summary>
        public double[] AuxState
        {
            get { return _auxState; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a copy of the current gene. Virtual method that can be overriden by sub-types.
        /// </summary>
        public virtual NeuronGene CreateCopy()
        {
            return new NeuronGene(this);
        }

        #endregion

        #region INetworkNode Members

        /// <summary>
        /// Gets the network node's ID.
        /// </summary>
        public uint Id
        {
            get { return _innovationId; }
        }

        #endregion
    }
}
