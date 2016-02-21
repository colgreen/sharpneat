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
namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// Represents a single connection between two neurons in a CyclicNetwork.
    /// </summary>
    public class Connection
    {
        readonly Neuron _srcNeuron;
        readonly Neuron _tgtNeuron;
        readonly double _weight;
        double _outputValue;

        #region Constructor

        /// <summary>
        /// Constructs a Connection with the provided source and target neurons, and connection weight. 
        /// </summary>
        public Connection(Neuron srcNeuron, Neuron tgtNeuron, double weight)
        {
            _tgtNeuron = tgtNeuron;
            _srcNeuron = srcNeuron;
            _weight = weight;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection's source neuron.
        /// </summary>
        public Neuron SourceNeuron
        {
            get { return _srcNeuron; }
        }

        /// <summary>
        /// Gets the connection's target neuron.
        /// </summary>
        public Neuron TargetNeuron
        {
            get { return _tgtNeuron; }
        }

        /// <summary>
        /// Gets the connection's weight.
        /// </summary>
        public double Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// Gets or sets the connection's output value.
        /// </summary>
        public double OutputValue
        {
            get { return _outputValue; }
            set { _outputValue = value; }
        }

        #endregion
    }
}
