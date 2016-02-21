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
using SharpNeat.Network;

namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// A version of FastCyclicNetwork that activates a network until it becomes 'relaxed' rather
    /// than for some fixed number of iterations. This class is exactly the same as FastCyclicNetwork
    /// in all other respects; See that class for more detailed info.
    /// 
    /// A network is defined as being relaxed when the change in output signal value between two successive
    /// update iterations is less than some threshold value (defined by maxAllowedSignalDelta on the constructor)
    /// for all hidden and output neurons (inpus and bias neurons have a fixed output value).
    /// </summary>
    public class FastRelaxingCyclicNetwork : FastCyclicNetwork
    {
        bool _isStateValid = false;
        readonly double _signalDeltaThreshold;

        #region Constructor

        /// <summary>
        /// Constructs a FastRelaxingCyclicNetwork with the provided pre-built FastConnection array and 
        /// associated data.
        /// </summary>
        public FastRelaxingCyclicNetwork(FastConnection[] connectionArray,
                                         IActivationFunction[] neuronActivationFnArray,
                                         double[][] neuronAuxArgsArray,
                                         int neuronCount,
                                         int inputNeuronCount,
                                         int outputNeuronCount,
                                         int maxTimesteps,
                                         double signalDeltaThreshold)
            : base(connectionArray, neuronActivationFnArray, neuronAuxArgsArray,
                   neuronCount, inputNeuronCount, outputNeuronCount,
                   maxTimesteps)
        {
            _signalDeltaThreshold = signalDeltaThreshold;
        }

        #endregion

        #region IBlackBox Members

        /// <summary>
        /// Gets a value indicating whether the internal state is valid. Returns false if the network did not relax within the
        /// maximum number of timesteps.
        /// </summary>
        public override bool IsStateValid
        {
            get { return _isStateValid; }
        }

        /// <summary>
        /// Activate the network until it becomes 'relaxed' or until maxIterations is reached. If maxIterations 
        /// is reached without the network relaxing then the IsValidState property will return false, although 
        /// the network outputs are still provided and can be read as normal.
        /// </summary>
        public override void Activate()
        {
            // Activate the network for a fixed number of timesteps.
            bool isNotRelaxed = true;
            for(int i=0; i<_timestepsPerActivation && isNotRelaxed; i++)
            {
                isNotRelaxed = false;

                // Loop connections. Get each connections input signal, apply the weight and add the result to 
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
                    double tmp = _neuronActivationFnArray[j].Calculate(_preActivationArray[j], _neuronAuxArgsArray[j]);
                    
                    // Compare the neuron's new output value with its old value. If the difference is greater
                    // than _signalDeltaThreshold then the network is not yet relaxed.
                    if(Math.Abs(tmp - _postActivationArray[j]) > _signalDeltaThreshold) {
                        isNotRelaxed = true;
                    }
                    _postActivationArray[j] = tmp;

                    // Take the opportunity to reset the pre-activation signal array in preperation for the next 
                    // activation loop.
                    _preActivationArray[j] = 0.0F;
                }
            }

            // If we performed the maximum number of update iterations without the network relaxing then
            // we define the network's state as being invalid.
            _isStateValid = !isNotRelaxed;
        }

        #endregion
    }
}
