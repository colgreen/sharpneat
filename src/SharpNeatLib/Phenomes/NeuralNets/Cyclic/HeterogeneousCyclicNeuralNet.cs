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
using SharpNeat.Network2;

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
    public class HeterogeneousCyclicNeuralNet : IBlackBox<double>
    {
        #region Instance Fields

        // Connection arrays.
        readonly DirectedConnection[] _connArr;
        readonly double[] _weightArr;

        // Activation function array.
        readonly Func<double,double>[] _activationFnArr;

        // Node pre- and post-activation signal arrays.
        readonly double[] _preActivationArr;
        readonly double[] _postActivationArr;

        // Wrappers over _postActivationArray that map between black box inputs/outputs to the
        // corresponding underlying network state variables.
        readonly SignalArray<double> _inputSignalArrWrapper;
        readonly SignalArray<double> _outputSignalArrWrapper;

        // Convenient counts.
        readonly int _inputCount;
        readonly int _outputCount;
        readonly int _activationCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a HeterogeneousCyclicNeuralNet with the provided pre-built ConnectionInfo array and 
        /// associated data.
        /// </summary>
        public HeterogeneousCyclicNeuralNet(
            WeightedDirectedGraph<double> diGraph,
            Func<double,double>[] neuronActivationFnArray,
            int activationCount,
            bool boundedOutput)
        {
            // Store refs to network structure data.
            _connArr = diGraph.ConnectionArray;
            _weightArr = diGraph.WeightArray;

            // Store network activation function and parameters.
            _activationFnArr = neuronActivationFnArray;
            _activationCount = activationCount;

            // Store input/output node counts.
            _inputCount = diGraph.InputNodeCount;
            _outputCount = diGraph.OutputNodeCount;


            // Create node pre- and post-activation signal arrays.
            int nodeCount = diGraph.TotalNodeCount;
            _preActivationArr = new double[nodeCount];
            _postActivationArr = new double[nodeCount];

            // Wrap sub-ranges of the neuron signal arrays as input and output arrays for IBlackBox.
            _inputSignalArrWrapper = new SignalArray<double>(_postActivationArr, 0, _inputCount);

            // Note. Output neurons follow input neurons in the arrays.
            if(boundedOutput) {
                _outputSignalArrWrapper = new BoundedSignalArray(_postActivationArr, _inputCount, _outputCount);
            } else {
                _outputSignalArrWrapper = new SignalArray<double>(_postActivationArr, _inputCount, _outputCount);
            }
        }

        #endregion

        #region IBlackBox Members

        /// <summary>
        /// Gets the number of input nodes.
        /// </summary>
        public int InputCount => _inputCount;

        /// <summary>
        /// Gets the number of output nodes.
        /// </summary>
        public int OutputCount => _outputCount;

        /// <summary>
        /// Gets an array for used for passing input signals to the network, i.e. the network input vector.
        /// </summary>
        public ISignalArray<double> InputSignalArray => _inputSignalArrWrapper;

        /// <summary>
        /// Gets an array of output signals from the network, i.e. the network output vector.
        /// </summary>
        public ISignalArray<double> OutputSignalArray => _outputSignalArrWrapper;

        /// <summary>
        /// Activate the network for a fixed number of iterations defined by the 'maxIterations' parameter
        /// at construction time. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public virtual void Activate()
        {
            // Activate the network for a fixed number of timesteps.
            for(int i=0; i<_activationCount; i++)
            {
                // Loop connections. Get each connection's input signal, apply the weight and add the result to 
                // the pre-activation signal of the target neuron.
                for(int j=0; j<_connArr.Length; j++) {
                    _preActivationArr[_connArr[j].TargetId] += _postActivationArr[_connArr[j].SourceId] * _weightArr[j];
                }

                // Loop the nodes. Pass each node's pre-activation signals through its activation function
                // and store the resulting post-activation signal.
                // Note. We skip over input nodes as these have no incoming connections and therefore have fixed
                // post-activation values and are never activated. 
                for (int j=_inputCount; j<_preActivationArr.Length; j++)
                {
                    _postActivationArr[j] = _activationFnArr[j](_preActivationArr[j]);
                    
                    // Take the opportunity to reset the pre-activation signal array in preparation for the next 
                    // activation loop.
                    _preActivationArr[j] = 0.0;
                }
            }
        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public void ResetState()
        {
            // Reset the output signal for all output and hidden neurons.
            // Ignore connection signal state as this gets overwritten on each iteration.
            for(int i=_inputCount; i<_postActivationArr.Length; i++) {
                _preActivationArr[i] = 0.0;
            }
        }

        #endregion
    }
}
