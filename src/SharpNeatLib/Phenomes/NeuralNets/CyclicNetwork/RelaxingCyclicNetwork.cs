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
    /// for all hidden and output neurons (inputs and bias neurons have a fixed output value).
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
                // Note. we set the output value of the input neurons, not the input value. This is because we 
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

                // TODO: Performance tune the activation function method call.
                // The call to Calculate() cannot be inlined because it is via an interface and therefore requires a virtual table lookup.
                // The obvious/simplest performance improvement would be to pass an array of values to Calculate(), but the auxargs parameter
                // currently thwarts the performance boost of that approach.

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
