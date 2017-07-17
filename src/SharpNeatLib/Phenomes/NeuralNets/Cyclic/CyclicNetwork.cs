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
using SharpNeat.Network;

// Disable missing comment warnings for non-private variables.
#pragma warning disable 1591

namespace SharpNeat.Phenomes.NeuralNets.Cyclic
{
    /// <summary>
    /// A neural network class that represents a network with recurrent (cyclic) connections. 
    /// 
    /// This class contains performance improvements described in the following report/post:
    /// 
    ///     http://sharpneat.sourceforge.net/research/network-optimisations.html
    /// 
    /// A speedup over a previous 'naive' implementation was achieved by compactly storing all required data in arrays
    /// and in a way that maximizes in-order memory accesses; this allows for good utilisation of CPU caches. 
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
    public class CyclicNetwork : IBlackBox<double>
    {
        protected readonly ConnectionInfo[] _connectionArray;
        protected readonly VecFnSegment2<double> _activationFn;

        // Neuron pre- and post-activation signal arrays.
        protected readonly double[] _preActivationArray;
        protected readonly double[] _postActivationArray;

        // Wrappers over _postActivationArray that map between black box inputs/outputs to the
        // corresponding underlying network state variables.
        readonly SignalArray<double> _inputSignalArrayWrapper;
        readonly SignalArray<double> _outputSignalArrayWrapper;

        // Convenient counts.
        readonly int _inputNeuronCount;
        readonly int _outputNeuronCount;
        protected readonly int _timestepsPerActivation;

        #region Constructor

        /// <summary>
        /// Constructs a CyclicNetwork with the provided pre-built ConnectionInfo array and 
        /// associated data.
        /// </summary>
        public CyclicNetwork(ConnectionInfo[] connInfoArr,
                             VecFnSegment2<double> activationFn,
                             int neuronCount,
                             int inputNeuronCount,
                             int outputNeuronCount,
                             int timestepsPerActivation,
                             bool boundedOutput)
        {
            _connectionArray = connInfoArr;
            _activationFn = activationFn;

            // Create neuron pre- and post-activation signal arrays.
            _preActivationArray = new double[neuronCount];
            _postActivationArray = new double[neuronCount];

            // Wrap sub-ranges of the neuron signal arrays as input and output arrays for IBlackBox.
            _inputSignalArrayWrapper = new SignalArray<double>(_postActivationArray, 0, inputNeuronCount);

            // Note. Output neurons follow input neurons in the arrays.
            if(boundedOutput) {
                _outputSignalArrayWrapper = new BoundedSignalArray(_postActivationArray, inputNeuronCount, outputNeuronCount);
            } else {
                _outputSignalArrayWrapper = new SignalArray<double>(_postActivationArray, inputNeuronCount, outputNeuronCount);
            }

            // Store counts for use during activation.
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;
            _timestepsPerActivation = timestepsPerActivation;
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
        public ISignalArray<double> InputSignalArray
        {
            get { return _inputSignalArrayWrapper; }
        }

        /// <summary>
        /// Gets an array of output signals from the network.
        /// </summary>
        public ISignalArray<double> OutputSignalArray
        {
            get { return _outputSignalArrayWrapper; }
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
                // the pre-activation signal of the target neuron.
                for(int j=0; j<_connectionArray.Length; j++) {
                    _preActivationArray[_connectionArray[j]._tgtNeuronIdx] += _postActivationArray[_connectionArray[j]._srcNeuronIdx] * _connectionArray[j]._weight;
                }

                // Pass the pre-activation levels through the activation function.
                // Note. the post-activation levels are stored in _postActivationArray.
                // Note. Skip over input neurons as these have no incoming connections and therefore have fixed
                // post-activation values and are never activated. 
                _activationFn(_preActivationArray, _postActivationArray, _inputNeuronCount, _preActivationArray.Length);

                // Reset the elements of _preActivationArray
                Array.Clear(_preActivationArray, _inputNeuronCount, _preActivationArray.Length-_inputNeuronCount);
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
            for(int i=_inputNeuronCount; i<_postActivationArray.Length; i++) {
                _preActivationArray[i] = 0.0;
                _postActivationArray[i] = 0.0;
            }
        }

        #endregion
    }
}
