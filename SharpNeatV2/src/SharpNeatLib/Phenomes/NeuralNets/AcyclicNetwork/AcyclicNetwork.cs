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
    // TODO: Work In Progress. Do Not Use.
    public class AcyclicNetwork : IBlackBox
    {
        // Network structure/definition.
        readonly FastConnection[] _connectionArray;
        readonly IActivationFunction[] _neuronActivationFnArray;
        readonly double[][] _neuronAuxArgsArray;

        // Working neuron activation signal array.
        readonly double[] _neuronActivationArr;

        // Wrappers over _postActivationArray that map between black box inputs/outputs to the
        // corresponding underlying network state variables.
        readonly SignalArray _inputSignalArrayWrapper;
        readonly SignalArray _outputSignalArrayWrapper;

        // Convenient counts.
        readonly int _inputNeuronCount;
        readonly int _outputNeuronCount;
        protected readonly int _inputAndBiasNeuronCount;




        #region Constructor

        public AcyclicNetwork(FastConnection[] connectionArray,
                              IActivationFunction[] neuronActivationFnArray,
                              double[][] neuronAuxArgsArray,
                              int neuronCount,
                              int inputNeuronCount,
                              int outputNeuronCount)
        {
            _connectionArray = connectionArray;
            _neuronActivationFnArray = neuronActivationFnArray;
            _neuronAuxArgsArray = neuronAuxArgsArray;

            // Create working neuron activation signal array.
            _neuronActivationArr = new double[neuronCount];

            // Wrap sub-ranges of the neuron signal array as input and output arrays for IBlackBox.
            // Offset is 1 to skip bias neuron (The value at index 1 is the first black box input).
            _inputSignalArrayWrapper = new SignalArray(_neuronActivationArr, 1, inputNeuronCount);

            // Offset to skip bias and input neurons. Output neurons follow input neurons in the arrays.
            _outputSignalArrayWrapper = new SignalArray(_neuronActivationArr, inputNeuronCount+1, outputNeuronCount);
            
            // Store counts for use during activation.
            _inputNeuronCount = inputNeuronCount;
            _inputAndBiasNeuronCount = inputNeuronCount+1;
            _outputNeuronCount = outputNeuronCount;

            // Initialise the bias neuron's fixed output value.
            _neuronActivationArr[0] = 1.0;
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
        /// Activate the network. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public void ResetState()
        {
            // Unnecessary for this implementation. The neuron activation signal state is completely overwritten on each activation.
        }

        #endregion
    }
}
