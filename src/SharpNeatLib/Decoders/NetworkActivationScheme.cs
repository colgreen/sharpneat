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
namespace SharpNeat.Decoders
{
    /// <summary>
    /// Represents network activation schemes. E.g. fixed number of activation timesteps
    /// or activation until the network becomes 'relaxed'. Relaxed here means that no node's
    /// output value changed by more than some threshold value.
    /// </summary>
    public class NetworkActivationScheme
    {
        bool _acyclicNetwork;

     //=== Cyclic network specific activation.
        bool _relaxingActivation;

        // Non-relaxing network parameter.
        int _timestepsPerActivation;

        // Relaxing network parameters.
        double _signalDeltaThreshold;
        int _maxTimesteps;

        #region Constructors

        /// <summary>
        /// Private constructor to restrict construction to static factory methods.
        /// </summary>
        private NetworkActivationScheme()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the network is acyclic or not (cyclic).
        /// </summary>
        public bool AcyclicNetwork
        {
            get { return _acyclicNetwork; }
        }

        /// <summary>
        /// Gets a value indicating whether the scheme is a relaxing activation scheme.
        /// </summary>
        public bool RelaxingActivation
        {
            get { return _relaxingActivation; }
        }

        /// <summary>
        /// Gets a fixed number of activation timesteps.
        /// Non-relaxing activation scheme.
        /// </summary>
        public int TimestepsPerActivation
        {
            get { return _timestepsPerActivation; }
        }

        /// <summary>
        /// Gets a maximum signal delta threshold used to determine if a network is relaxed.
        /// Relaxing activation scheme.
        /// </summary>
        public double SignalDeltaThreshold
        {
            get { return _signalDeltaThreshold; }
        }

        /// <summary>
        /// Gets the maximum number of activation timesteps before stopping.
        /// Relaxing activation scheme. 
        /// </summary>
        public int MaxTimesteps
        {
            get { return _maxTimesteps; }
        }

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Create an activation scheme for acyclic networks.
        /// </summary>
        public static NetworkActivationScheme CreateAcyclicScheme()
        {
            NetworkActivationScheme scheme = new NetworkActivationScheme();
            scheme._acyclicNetwork = true;
            return scheme;
        }

        /// <summary>
        /// Create an activation scheme with a fixed number of activation timesteps (suitable for cyclic networks only).
        /// </summary>
        public static NetworkActivationScheme CreateCyclicFixedTimestepsScheme(int timestepsPerActivation)
        {
            NetworkActivationScheme scheme = new NetworkActivationScheme();
            scheme._acyclicNetwork = false;
            scheme._timestepsPerActivation = timestepsPerActivation;
            scheme._relaxingActivation = false;
            return scheme;
        }

        /// <summary>
        /// Create a relaxing activation scheme (suitable for cyclic networks only).
        /// </summary>
        public static NetworkActivationScheme CreateCyclicRelaxingActivationScheme(double signalDeltaThreshold, int maxTimesteps)
        {
            NetworkActivationScheme scheme = new NetworkActivationScheme();
            scheme._acyclicNetwork = false;
            scheme._signalDeltaThreshold = signalDeltaThreshold;
            scheme._maxTimesteps = maxTimesteps;
            scheme._relaxingActivation = true;
            return scheme;
        }

        #endregion
    }
}
