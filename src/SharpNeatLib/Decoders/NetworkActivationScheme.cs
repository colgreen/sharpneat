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

        // Fast flag. Strictly speaking not part of the activation scheme, but this is currently a
        // convenient place for this flag.
        bool _fastFlag;

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

        /// <summary>
        /// Gets a value indicating whether a fast version of the network should be created when decoding.
        /// </summary>
        public bool FastFlag
        {
            get { return _fastFlag; }
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
            scheme._fastFlag = true;
            return scheme;
        }

        /// <summary>
        /// Create an activation scheme with a fixed number of activation timesteps (suitable for cyclic networks only).
        /// 'fastFlag' indicates if a fast network implementation should be used.
        /// </summary>
        public static NetworkActivationScheme CreateCyclicFixedTimestepsScheme(int timestepsPerActivation, bool fastFlag)
        {
            NetworkActivationScheme scheme = new NetworkActivationScheme();
            scheme._acyclicNetwork = false;
            scheme._timestepsPerActivation = timestepsPerActivation;
            scheme._relaxingActivation = false;
            scheme._fastFlag = fastFlag;
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
            scheme._fastFlag = true;
            return scheme;
        }

        /// <summary>
        /// Create a relaxing activation scheme (suitable for cyclic networks only).
        /// 'fastFlag' indicates if a fast network implementation should be used.
        /// </summary>
        public static NetworkActivationScheme CreateCyclicRelaxingActivationScheme(double signalDeltaThreshold, int maxTimesteps, bool fastFlag)
        {
            NetworkActivationScheme scheme = new NetworkActivationScheme();
            scheme._acyclicNetwork = false;
            scheme._signalDeltaThreshold = signalDeltaThreshold;
            scheme._maxTimesteps = maxTimesteps;
            scheme._relaxingActivation = true;
            scheme._fastFlag = fastFlag;
            return scheme;
        }

        #endregion
    }
}
