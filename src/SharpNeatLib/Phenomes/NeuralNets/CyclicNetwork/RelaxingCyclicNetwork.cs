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
using System;
using System.Collections.Generic;

namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// A version of CyclicNetwork that activates a network until it becomes 'relaxed' rather than for some
    /// fixed number of iterations. This class is exactly the same as CyclicNetwork in all other respects;
    /// See that class for more detailed info.
    ///
    /// A network is defined as being relaxed when the change in output signal value between two successive
    /// update iterations is less than some threshold value (defined by maxAllowedSignalDelta on the constructor)
    /// for all hidden and output neurons (inpus and bias neurons have a fixed output value).
    /// </summary>
    public class RelaxingCyclicNetwork : CyclicNetwork
    {
        bool _isStateValid = false;
        readonly double _signalDeltaThreshold;

        #region Constructor

        /// <summary>
        /// Constructs a CyclicNetwork with the provided pre-built neurons and connections.
        /// </summary>
        public RelaxingCyclicNetwork(List<Neuron> neuronList,
                                     List<Connection> connectionList,
                                     int inputNeuronCount,
                                     int outputNeuronCount,
                                     int maxTimesteps,
                                     double signalDeltaThreshold)
            : base(neuronList, connectionList, inputNeuronCount, outputNeuronCount, maxTimesteps)
        {
            _signalDeltaThreshold = signalDeltaThreshold;
        }

        #endregion

        #region IBlackBox Members

        /// <summary>
        /// Indicates if the internal state is valid. Returns false if the network did not relax within the
        /// maximum number of timesteps.
        /// </summary>
        public override bool IsStateValid
        {
            get { return _isStateValid; }
        }

        /// <summary>
        /// Activate the network until it becomes 'relaxed' or until maxTimesteps is reached. If maxIterations 
        /// is reached without the network relaxing then the IsValidState property will return false, although 
        /// the network outputs are still provided and can be read as normal.
        /// </summary>
        public override void Activate()
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

            // Activate the network until it becomes relaxed, up to a maximum number of timesteps.
            int connectionCount = _connectionList.Count;
            int neuronCount = _neuronList.Count;
            bool isNotRelaxed = true;
            for(int i=0; i<_timestepsPerActivation && isNotRelaxed; i++)
            {
                isNotRelaxed = false;

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
                    double tmp = neuron.ActivationFunction.Calculate(neuron.InputValue, neuron.AuxiliaryArguments);

                    // Compare the neuron's new output value with its old value. If the difference is greater
                    // than _signalDeltaThreshold then the network is not yet relaxed.
                    if(Math.Abs(neuron.OutputValue - tmp) > _signalDeltaThreshold) {
                        isNotRelaxed = true;
                    }

                    // Assign the neuron its new value.
                    neuron.OutputValue = tmp;

                    // Reset input value, in preparation for the next timestep/iteration.
                    neuron.InputValue = 0.0;
                }
            }

            // Copy the output neuron output values into the output signal array.
            for(int i=_inputAndBiasNeuronCount, outputIdx=0; outputIdx<_outputNeuronCount; i++, outputIdx++) 
            {
                _outputSignalArray[outputIdx] = _neuronList[i].OutputValue;
            }

            // If we performed the maximum number of update timesteps without the network relaxing then
            // we define the network's state as being invalid.
            _isStateValid = !isNotRelaxed;
        }

        /// <summary>
        /// Reset the network's internal state and isValidState flag.
        /// </summary>
        public override void ResetState()
        {
            base.ResetState();
            _isStateValid = true;
        }

        #endregion
    }
}
