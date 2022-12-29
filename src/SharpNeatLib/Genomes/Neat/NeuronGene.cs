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
        readonly HashSet<uint> _srcNeurons;
        readonly HashSet<uint> _tgtNeurons;

        #region Constructor

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="copyFrom">NeuronGene to copy from.</param>
        /// <param name="copyConnectivityData">Indicates whether or not top copy connectivity data for the neuron.</param>
        public NeuronGene(NeuronGene copyFrom, bool copyConnectivityData)
        {
            _innovationId = copyFrom._innovationId;
            _neuronType = copyFrom._neuronType;
            _activationFnId = copyFrom._activationFnId;
            if(null != copyFrom._auxState) {
                _auxState = (double[])copyFrom._auxState.Clone();
            }

            if(copyConnectivityData) {
                _srcNeurons = new HashSet<uint>(copyFrom._srcNeurons);
                _tgtNeurons = new HashSet<uint>(copyFrom._tgtNeurons);
            } else {
                _srcNeurons = new HashSet<uint>();
                _tgtNeurons = new HashSet<uint>();
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
            _srcNeurons = new HashSet<uint>();
            _tgtNeurons = new HashSet<uint>();
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
            _srcNeurons = new HashSet<uint>();
            _tgtNeurons = new HashSet<uint>();
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
        /// Optional auxiliary node state. Null if no aux state is present. 
        /// Note. Radial Basis Function center and epsilon values are stored here.
        /// </summary>
        public double[] AuxState
        {
            get { return _auxState; }
        }

        /// <summary>
        /// Gets a set of IDs for the source neurons that directly connect into this neuron.
        /// </summary>
        public HashSet<uint> SourceNeurons
        {
            get { return _srcNeurons; }
        }

        /// <summary>
        /// Gets a set of IDs for the target neurons this neuron directly connects out to.
        /// </summary>
        public HashSet<uint> TargetNeurons
        {
            get { return _tgtNeurons; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a copy of the current gene. Virtual method that can be overridden by sub-types.
        /// </summary>
        public virtual NeuronGene CreateCopy()
        {
            return new NeuronGene(this, true);
        }

        /// <summary>
        /// Creates a copy of the current gene. Virtual method that can be overridden by sub-types.
        /// </summary>
        /// <param name="copyConnectivityData">Indicates whether or not top copy connectivity data for the neuron.</param>
        public virtual NeuronGene CreateCopy(bool copyConnectivityData)
        {
            return new NeuronGene(this, copyConnectivityData);
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
