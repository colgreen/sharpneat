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

namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// Represents a single neuron in a CyclicNetwork.
    /// </summary>
    public class Neuron 
    {
        readonly uint _id;
        readonly NodeType _neuronType;
        readonly IActivationFunction _activationFn;
        readonly double[] _auxArgs;
        double _inputValue;
        double _outputValue;
        
        #region Constructor

        /// <summary>
        /// Constructs a Neuron with the provided ID, type and activation function.
        /// </summary>
        public Neuron(uint id, NodeType neuronType, IActivationFunction activationFn, double[] auxArgs)
        {            
            _id = id;
            _neuronType = neuronType;
            _activationFn = activationFn;
            _auxArgs = auxArgs;

            // Bias neurons have a fixed output value of 1.0
            _outputValue = (NodeType.Bias == _neuronType) ? 1.0 : 0.0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the neuron's ID.
        /// </summary>
        public uint Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the neuron's type.
        /// </summary>
        public NodeType NeuronType
        {
            get { return _neuronType; }
        }

        /// <summary>
        /// Gets the neuron's activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get { return _activationFn; }
        }

        /// <summary>
        /// Gets the neuron's acticvation function auxiliary arguments (if any).
        /// </summary>
        public double[] AuxiliaryArguments
        {
            get { return _auxArgs; }
        }

        /// <summary>
        /// Gets or sets the neuron's current input value.
        /// </summary>
        public double InputValue
        {
            get { return _inputValue; }
            set 
            {
                if(NodeType.Bias == _neuronType || NodeType.Input == _neuronType) {
                    throw new SharpNeatException("Attempt to set the InputValue of bias or input neuron. Bias neurons have no input, and Input neuron signals should be passed in via their OutputValue property setter.");
                }
                _inputValue = value; 
            }
        }

        /// <summary>
        /// Gets or sets the neuron's current output value. This is set to a fixed value for bias neurons.
        /// </summary>
        public double OutputValue
        {
            get { return _outputValue; }
            set 
            {
                if(NodeType.Bias == _neuronType) {
                    throw new SharpNeatException("Attempt to set the OutputValue of a bias neuron.");
                }
                _outputValue = value; 
            }
        }

        #endregion
    }
}
