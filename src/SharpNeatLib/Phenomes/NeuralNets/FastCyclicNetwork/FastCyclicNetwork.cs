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

// Disable missing comment warnings for non-private variables.
#pragma warning disable 1591

namespace SharpNeat.Phenomes.NeuralNets
{
    // TODO: Create a version of this class that uses single precision signals for extra performance. 
    // TODO: Reconsider algorithm. Even better/faster way?

    /// <summary>
    /// A neural network class that represents a network with recurrent (cyclic) connections. 
    /// 
    /// This is a much faster implementation of CyclicNetwork. The speedup is approximately 5x depending on 
    /// hardware and CLR platform, see http://sharpneat.sourceforge.net/network_optimization.html for detailed info.
    /// 
    /// The speedup is achieved by compactly storing all required data in arrays and in a way that maximizes
    /// in-order memory accesses; This allows us to maximize use of CPU caches. In contrast the CyclicNetwork
    /// class represents the network directly, that is, as a network of neuron/node objects; This has additional
    /// overhead such as the standard data associated with each object in dotNet which results in less efficient
    /// packing of the true neural net data in memory, which in turns results in less efficient use of CPU memory 
    /// caches. Finally, representing the network directly as a graph of connected nodes is not conducive to 
    /// writing code with in-order memory accesses.
    /// 
    /// Algorithm Overview.
    /// 1) Loop connections. Each connection gets its input signal from its source neuron, applies its weight and
    /// stores its output value./ Connections are ordered by source neuron index, thus all memory accesses here are
    /// sequential/in-order.
    /// 
    /// 2) Loop connections (again). Each connection adds its output value to its target neuron, thus each neuron  
    /// accumulates or 'collects' its input signal in its pre-activation variable. Because connections are sorted by
    /// source neuron index and not target index, this loop generates out-of order memory accesses, but is the only 
    /// loop to do so.
    /// 
    /// 3) Loop neurons. Pass each neuron's pre-activation signal through the activation function and set its 
    /// post-activation signal value. 
    /// 
    /// The activation loop is now complete and we can go back to (1) or stop.
    /// </summary>
    public class FastCyclicNetwork : IBlackBox
    {
        protected readonly FastConnection[] _connectionArray;
        protected readonly IActivationFunction[] _neuronActivationFnArray;
        protected readonly double[][] _neuronAuxArgsArray;

        // Neuron pre- and post-activation signal arrays.
        protected readonly double[] _preActivationArray;
        protected readonly double[] _postActivationArray;

        // Wrappers over _postActivationArray that map between black box inputs/outputs to the
        // corresponding underlying network state variables.
        readonly SignalArray _inputSignalArrayWrapper;
        readonly SignalArray _outputSignalArrayWrapper;

        // Convenient counts.
        readonly int _inputNeuronCount;
        readonly int _outputNeuronCount;
        protected readonly int _inputAndBiasNeuronCount;
        protected readonly int _timestepsPerActivation;

        #region Constructor

        /// <summary>
        /// Constructs a FastCyclicNetwork with the provided pre-built FastConnection array and 
        /// associated data.
        /// </summary>
        public FastCyclicNetwork(FastConnection[] connectionArray,
                                 IActivationFunction[] neuronActivationFnArray,
                                 double[][] neuronAuxArgsArray,
                                 int neuronCount,
                                 int inputNeuronCount,
                                 int outputNeuronCount,
                                 int timestepsPerActivation)
        {
            _connectionArray = connectionArray;
            _neuronActivationFnArray = neuronActivationFnArray;
            _neuronAuxArgsArray = neuronAuxArgsArray;

            // Create neuron pre- and post-activation signal arrays.
            _preActivationArray = new double[neuronCount];
            _postActivationArray = new double[neuronCount];

            // Wrap sub-ranges of the neuron signal arrays as input and output arrays for IBlackBox.
            // Offset is 1 to skip bias neuron (The value at index 1 is the first black box input).
            _inputSignalArrayWrapper = new SignalArray(_postActivationArray, 1, inputNeuronCount);

            // Offset to skip bias and input neurons. Output neurons follow input neurons in the arrays.
            _outputSignalArrayWrapper = new SignalArray(_postActivationArray, inputNeuronCount+1, outputNeuronCount);

            // Store counts for use during activation.
            _inputNeuronCount = inputNeuronCount;
            _inputAndBiasNeuronCount = inputNeuronCount+1;
            _outputNeuronCount = outputNeuronCount;
            _timestepsPerActivation = timestepsPerActivation;

            // Initialise the bias neuron's fixed output value.
            _postActivationArray[0] = 1.0;
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
        /// Activate the network for a fixed number of iterations defined by the 'maxIterations' parameter
        /// at construction time. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public virtual void Activate()
        {
            // Activate the network for a fixed number of timesteps.
            for(int i=0; i<_timestepsPerActivation; i++)
            {
                // Loop connections. Get each connection's input signal, apply the weight and add the result to 
                // the preactivation signal of the target neuron.
                for(int j=0; j<_connectionArray.Length; j++) {
                    _preActivationArray[_connectionArray[j]._tgtNeuronIdx] += _postActivationArray[_connectionArray[j]._srcNeuronIdx] * _connectionArray[j]._weight;
                }

                // Loop the neurons. Pass each neuron's pre-activation signals through its activation function
                // and store the resulting post-activation signal.
                // Skip over bias and input neurons as these have no incoming connections and therefore have fixed
                // post-activation values and are never activated. 
                for(int j=_inputAndBiasNeuronCount; j<_preActivationArray.Length; j++)
                {
                    _postActivationArray[j] = _neuronActivationFnArray[j].Calculate(_preActivationArray[j], _neuronAuxArgsArray[j]);
                    
                    // Take the opportunity to reset the pre-activation signal array in preperation for the next 
                    // activation loop.
                    _preActivationArray[j] = 0.0F;
                }
            }
        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public void ResetState()
        {
            // TODO: Avoid resetting if network state hasn't changed since construction or previous reset.

            // Reset the output signal for all output and hidden neurons.
            // Ignore connection signal state as this gets overwritten on each iteration.
            for(int i=_inputAndBiasNeuronCount; i<_postActivationArray.Length; i++) {
                _preActivationArray[i] = 0.0;
                _postActivationArray[i] = 0.0;
            }
        }

        #endregion
    }
}
