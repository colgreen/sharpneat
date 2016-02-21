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

// Disables missing comment warnings for non-private variables.
#pragma warning disable 1591

namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// This class is provided for debugging and educational purposes. FastCyclicNetwork is functionally
    /// equivalent and is much faster and therefore should be used instead of CyclicNetwork in most 
    /// circumstances.
    /// 
    /// A neural network class that represents a network with recurrent (cyclic) connections. Recurrent
    /// connections are handled by each neuron storing two values, a pre- and post-activation value 
    /// (InputValue and OutputValue). This allows us to calculate the output value for the current 
    /// iteration/timestep without modifying the output values from the previous iteration. That is, we
    /// calculate all of this timestep's state based on state from the previous timestep.
    /// 
    /// When activating networks of this class the network's state is updated for a fixed number of 
    /// timesteps, the number of which is specified by the maxIterations parameter on the constructor.
    /// See RelaxingCyclicNetwork for an alternative activation scheme.
    /// </summary>
    public class CyclicNetwork : IBlackBox
    {
        protected readonly List<Neuron> _neuronList;
        protected readonly List<Connection> _connectionList;
        
        // For efficiency we store the number of input and output neurons.
        protected readonly int _inputNeuronCount;
        protected readonly int _outputNeuronCount;
        protected readonly int _inputAndBiasNeuronCount;
        protected readonly int _timestepsPerActivation;

        // The input and output arrays that the black box uses for IO with the outside world.
        protected readonly double[] _inputSignalArray;
        protected readonly double[] _outputSignalArray;

        readonly SignalArray _inputSignalArrayWrapper;
        readonly SignalArray _outputSignalArrayWrapper;

        #region Constructor

        /// <summary>
        /// Constructs a CyclicNetwork with the provided pre-built neurons and connections.
        /// </summary>
        public CyclicNetwork(List<Neuron> neuronList,
                                List<Connection> connectionList,
                                int inputNeuronCount,
                                int outputNeuronCount,
                                int timestepsPerActivation)
        {
            _neuronList = neuronList;
            _connectionList = connectionList;
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;
            _inputAndBiasNeuronCount = inputNeuronCount + 1;
            _timestepsPerActivation = timestepsPerActivation;

            _inputSignalArray = new double[_inputNeuronCount];
            _outputSignalArray = new double[_outputNeuronCount];

            _inputSignalArrayWrapper = new SignalArray(_inputSignalArray, 0, _inputNeuronCount);
            _outputSignalArrayWrapper = new SignalArray(_outputSignalArray, 0, outputNeuronCount);
        }

        #endregion

        #region IBlackBox Members

        /// <summary>
        /// Gets the number of inputs.
        /// </summary>
        public int InputCount
        {
            get { return _inputNeuronCount; }
        }

        /// <summary>
        /// Gets the number of outputs.
        /// </summary>
        public int OutputCount
        {
            get { return _outputNeuronCount; }
        }

        /// <summary>
        /// Gets an array for feeding input signals to the network.
        /// </summary>
        public ISignalArray InputSignalArray
        {
            get { return _inputSignalArrayWrapper; }
        }

        /// <summary>
        /// Gets an array of output signals from the network.
        /// </summary>
        public ISignalArray OutputSignalArray
        {
            get { return _outputSignalArrayWrapper; }
        }

        /// <summary>
        /// Gets a value indicating whether the internal state is valid. Always returns true for this class.
        /// </summary>
        public virtual bool IsStateValid
        {
            get { return true; }
        }

        /// <summary>
        /// Activate the network for a fixed number of timesteps defined by maxTimesteps is reached.
        /// </summary>
        public virtual void Activate()
        {
            // Copy input signals into input neurons.
            // Note. In fast implementations we can skip this step because the array is 
            // part of the working data of the network.
            for(int i=0; i<_inputNeuronCount; i++)
            {   // The +1 takes into account the bias neuron at index 0.
                // Note. we set the outputvalue of the input neurons, not the input value. This is because we 
                // don't want the signal to pass through the neuron's activation function.
                _neuronList[i+1].OutputValue = _inputSignalArray[i];
            }

            // Activate the network for a fixed number of timesteps.
            int connectionCount = _connectionList.Count;
            int neuronCount = _neuronList.Count;
            for(int i=0; i<_timestepsPerActivation; i++)
            {
                // Loop over all connections. 
                // Calculate each connection's output signal by multiplying its weight by the output value
                // of its source neuron.
                // Add the connection's output value to the target neuron's input value. Neurons therefore
                // accumulate all input value from connections targeting them.
                for(int j=0; j<connectionCount; j++) 
                {
                    Connection connection = _connectionList[j];
                    connection.OutputValue = connection.SourceNeuron.OutputValue * connection.Weight;
                    connection.TargetNeuron.InputValue += connection.OutputValue;
                }

                // Loop over all output and hidden neurons, passing their input signal through their activation
                // function to produce an output value. Note we skip bias and input neurons because they have a 
                // fixed output value.
                for(int j=_inputAndBiasNeuronCount; j<neuronCount; j++) 
                {
                    Neuron neuron = _neuronList[j];
                    neuron.OutputValue = neuron.ActivationFunction.Calculate(neuron.InputValue, neuron.AuxiliaryArguments);

                    // Reset input value, in preparation for the next timestep/iteration.
                    neuron.InputValue = 0.0;
                }
            }

            // Copy the output neuron output values into the output signal array.
            for(int i=_inputAndBiasNeuronCount, outputIdx=0; outputIdx<_outputNeuronCount; i++, outputIdx++) 
            {
                _outputSignalArray[outputIdx] = _neuronList[i].OutputValue;
            }
        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public virtual void ResetState()
        {
            // Reset neuron state for all but the bias neuron.
            // Input neurons - avoid setting InputValue. We only use the OutputValue of input neurons.
            // TODO: Not strictly necessary; input node state is always overwritten at the initial stages of network activation.
            for(int i=1; i<_inputAndBiasNeuronCount; i++) {
                _neuronList[i].OutputValue = 0.0;
            }

            // Reset input and output value of all remaining neurons (output and hidden neurons).
            int count = _neuronList.Count;
            for(int i=_inputAndBiasNeuronCount; i<count; i++) {   
                _neuronList[i].InputValue = 0.0;
                _neuronList[i].OutputValue = 0.0;
            }
            
            // Reset connection states.
            count = _connectionList.Count;
            for(int i=0; i<count; i++) {
                _connectionList[i].OutputValue = 0.0;
            }
        }

        #endregion
    }
}
