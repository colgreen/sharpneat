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
        readonly bool _relaxingActivation;

        // Non-relaxing network parameter.
        readonly int _timestepsPerActivation;

        // Relaxing network parameters.
        readonly double _signalDeltaThreshold;
        readonly int _maxTimesteps;

        // Fast flag. Strictly speaking not part of the activation scheme, but this is currently a
        // convenient place for this flag.
        readonly bool _fastFlag;

        #region Constructors

        /// <summary>
        /// Construct a scheme with a fixed number of activation timesteps.
        /// </summary>
        public NetworkActivationScheme(int timestepsPerActivation)
        {
            _timestepsPerActivation = timestepsPerActivation;
            _relaxingActivation = false;
            _fastFlag = true;
        }

        /// <summary>
        /// Construct a scheme with a fixed number of activation timesteps.
        /// 'fastFlag' indicates if a fast network implementation should be used.
        /// </summary>
        public NetworkActivationScheme(int timestepsPerActivation, bool fastFlag)
        {
            _timestepsPerActivation = timestepsPerActivation;
            _relaxingActivation = false;
            _fastFlag = fastFlag;
        }

        /// <summary>
        /// Construct a relaxing network activation scheme.
        /// </summary>
        public NetworkActivationScheme(double signalDeltaThreshold, int maxTimesteps)
        {
            _signalDeltaThreshold = signalDeltaThreshold;
            _maxTimesteps = maxTimesteps;
            _relaxingActivation = true;
            _fastFlag = true;
        }

        /// <summary>
        /// Construct a relaxing network activation scheme.
        /// 'fastFlag' indicates if a fast network implementation should be used.
        /// </summary>
        public NetworkActivationScheme(double signalDeltaThreshold, int maxTimesteps, bool fastFlag)
        {
            _signalDeltaThreshold = signalDeltaThreshold;
            _maxTimesteps = maxTimesteps;
            _relaxingActivation = true;
            _fastFlag = fastFlag;
        }

        #endregion

        #region Properties

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
    }
}
