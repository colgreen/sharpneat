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
using SharpNeat.Utility;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Represents parameters specific to NEAT genomes. E.g. parameters that describe probabilities
    /// for the different types of mutation and the proportion of possible connections to instantiate 
    /// between input and output neurons within the initial population.
    /// </summary>
    public class NeatGenomeParameters
    {
        #region Constants

        const double DefaultConnectionWeightRange = 5.0;
        const double DefaultInitialInterconnectionsProportion = 0.05;
        const double DefaultDisjointExcessGenesRecombineProbability = 0.1;

        // High level mutation probabilities
        const double DefaultConnectionWeightMutationProbability = 0.988;
        const double DefaultAddNodeMutationProbability = 0.001;
        const double DefaultAddConnectionMutationProbability = 0.01;
        const double DefaultNodeAuxStateMutationProbability = 0.00;
        const double DefaultDeleteConnectionMutationProbability = 0.001;

        #endregion

        #region Instance Fields

        bool _feedforwardOnly;
        IActivationFunction _activationFn;
        double _connectionWeightRange;
        double _initialInterconnectionsProportion;
        double _disjointExcessGenesRecombineProbability;

        // High level mutation probabilities.
        double _connectionWeightMutationProbability;
        double _addNodeMutationProbability;
        double _addConnectionMutationProbability;
        double _nodeAuxStateMutationProbability;
        double _deleteConnectionMutationProbability;

        // RouletteWheelLayout representing the above five mutation probabilities.
        RouletteWheelLayout _rouletteWheelLayout;

        // Alternative RouletteWheelLayout used when we wish to avoid deletion mutations, e.g. when 
        // mutating a genome with just one connection.
        RouletteWheelLayout _rouletteWheelLayoutNonDestructive;

        /// <summary>
        /// A list of ConnectionMutationInfo objects that drives the types of connection mutation
        /// that occur.
        /// </summary>
        ConnectionMutationInfoList _connectionMutationInfoList;
        
        // The fitness history length to be used by genomes when recording their fitness
        int _fitnessHistoryLength;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with default set of parameters.
        /// </summary>
        public NeatGenomeParameters()
        {
            _activationFn                               = SteepenedSigmoid.__DefaultInstance;
            _connectionWeightRange                      = DefaultConnectionWeightRange;
            _initialInterconnectionsProportion          = DefaultInitialInterconnectionsProportion;
            _disjointExcessGenesRecombineProbability    = DefaultDisjointExcessGenesRecombineProbability;
            _connectionWeightMutationProbability        = DefaultConnectionWeightMutationProbability;
            _addNodeMutationProbability                 = DefaultAddNodeMutationProbability;
            _addConnectionMutationProbability           = DefaultAddConnectionMutationProbability;
            _nodeAuxStateMutationProbability            = DefaultNodeAuxStateMutationProbability;
            _deleteConnectionMutationProbability        = DefaultDeleteConnectionMutationProbability;

            _rouletteWheelLayout = CreateRouletteWheelLayout();
            _rouletteWheelLayoutNonDestructive = CreateRouletteWheelLayout_NonDestructive();

            // Create a connection weight mutation scheme.
            _connectionMutationInfoList = CreateConnectionWeightMutationScheme_Default();

            // No fitness history.
            _fitnessHistoryLength = 0;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public NeatGenomeParameters(NeatGenomeParameters copyFrom)
        {
            _feedforwardOnly                            = copyFrom._feedforwardOnly;
            _activationFn                               = copyFrom._activationFn;
            _connectionWeightRange                      = copyFrom._connectionWeightRange;
            _initialInterconnectionsProportion          = copyFrom._initialInterconnectionsProportion;
            _disjointExcessGenesRecombineProbability    = copyFrom._disjointExcessGenesRecombineProbability;
            _connectionWeightMutationProbability        = copyFrom._connectionWeightMutationProbability;
            _addNodeMutationProbability                 = copyFrom._addNodeMutationProbability;
            _addConnectionMutationProbability           = copyFrom._addConnectionMutationProbability;
            _nodeAuxStateMutationProbability            = copyFrom._nodeAuxStateMutationProbability;
            _deleteConnectionMutationProbability        = copyFrom._deleteConnectionMutationProbability;

            _rouletteWheelLayout = new RouletteWheelLayout(copyFrom._rouletteWheelLayout);
            _rouletteWheelLayoutNonDestructive = new RouletteWheelLayout(copyFrom._rouletteWheelLayoutNonDestructive);
            
            _connectionMutationInfoList = new ConnectionMutationInfoList(copyFrom._connectionMutationInfoList);
            _connectionMutationInfoList.Initialize();
            _fitnessHistoryLength = copyFrom._fitnessHistoryLength;
        }

        #endregion

        #region Properties

        // TODO: rename to AcyclicOnly?
        /// <summary>
        /// Gets or sets a boolean that indicates if NEAT should produce only feedforward networks (no recurrent/cyclic connection paths).
        /// </summary>
        public bool FeedforwardOnly
        {
            get { return _feedforwardOnly; }
            set { _feedforwardOnly = value; }
        }

        /// <summary>
        /// Gets or sets the neuron activation function to use in evolved networks. NEAT uses the same activation
        /// function at each node.
        /// </summary>
        public IActivationFunction ActivationFn
        {
            get { return _activationFn; }
            set { _activationFn = value; }
        }

        /// <summary>
        /// Gets or sets the connection weight range to use in NEAT genomes. E.g. a value of 5 defines a weight range
        /// of -5 to 5. The weight range is strictly enforced - e.g. when creating new connections and mutating
        /// existing ones.
        /// </summary>
        public double ConnectionWeightRange
        {
            get { return _connectionWeightRange; }
            set { _connectionWeightRange = value; }
        }

        /// <summary>
        /// Gets or sets a proportion that specifies the number of interconnections to make between input and 
        /// output neurons in an initial random population. This is a proportion of the total number of
        /// possible interconnections.
        /// </summary>
        public double InitialInterconnectionsProportion
        {
            get { return _initialInterconnectionsProportion; }
            set { _initialInterconnectionsProportion = value; }
        }

        /// <summary>
        /// Gets or sets the probability that all excess and disjoint genes are copied into an offspring genome
        /// during sexual reproduction. Currently the execss/disjoint genes are copied in an all or nothing 
        /// strategy.
        /// </summary>
        public double DisjointExcessGenesRecombinedProbability
        {
            get { return _disjointExcessGenesRecombineProbability; }
            set { _disjointExcessGenesRecombineProbability = value; }
        }

        /// <summary>
        /// Gets or sets the probability that a genome mutation operates on genome connection weights.
        /// </summary>
        public double ConnectionWeightMutationProbability
        {
            get 
            {
                return _connectionWeightMutationProbability; 
            }
            set 
            {
                _connectionWeightMutationProbability = value; 
                _rouletteWheelLayout = CreateRouletteWheelLayout();
                _rouletteWheelLayoutNonDestructive = CreateRouletteWheelLayout_NonDestructive();
            }
        }

        /// <summary>
        /// Gets or sets the probability that a genome mutation is an 'add node' mutation.
        /// </summary>
        public double AddNodeMutationProbability
        {
            get 
            {
                return _addNodeMutationProbability; 
            }
            set 
            {
                _addNodeMutationProbability = value; 
                _rouletteWheelLayout = CreateRouletteWheelLayout();
                _rouletteWheelLayoutNonDestructive = CreateRouletteWheelLayout_NonDestructive();
            }
        }

        /// <summary>
        /// Gets or sets the probability that a genome mutation is an 'add connection' mutation.
        /// </summary>
        public double AddConnectionMutationProbability
        {
            get 
            {
                return _addConnectionMutationProbability; 
            }
            set 
            {
                _addConnectionMutationProbability = value; 
                _rouletteWheelLayout = CreateRouletteWheelLayout();
                _rouletteWheelLayoutNonDestructive = CreateRouletteWheelLayout_NonDestructive();
            }
        }

        /// <summary>
        /// Gets or sets the probability that a genome mutation is a 'node auxiliary state' mutation.
        /// </summary>
        public double NodeAuxStateMutationProbability
        {
            get 
            {
                return _nodeAuxStateMutationProbability; 
            }
            set 
            {
                _nodeAuxStateMutationProbability = value; 
                _rouletteWheelLayout = CreateRouletteWheelLayout();
                _rouletteWheelLayoutNonDestructive = CreateRouletteWheelLayout_NonDestructive();
            }
        }

        /// <summary>
        /// Gets or sets the probability that a genome mutation is a 'delete connection' mutation.
        /// </summary>
        public double DeleteConnectionMutationProbability
        {
            get 
            {
                return _deleteConnectionMutationProbability; 
            }
            set 
            {
                _deleteConnectionMutationProbability = value; 
                _rouletteWheelLayout = CreateRouletteWheelLayout();
                _rouletteWheelLayoutNonDestructive = CreateRouletteWheelLayout_NonDestructive();
            }
        }

        /// <summary>
        /// Gets a RouletteWheelLayout that represents the probabilities of each type of genome mutation.
        /// </summary>
        public RouletteWheelLayout RouletteWheelLayout
        {
            get { return _rouletteWheelLayout; }
        }

        /// <summary>
        /// Gets an alternative RouletteWheelLayout for use when we wish to avoid deletion mutations, 
        /// e.g. when  mutating a genome with just one connection.
        /// </summary>
        public RouletteWheelLayout RouletteWheelLayoutNonDestructive
        {
            get { return _rouletteWheelLayoutNonDestructive; }
        }

        /// <summary>
        /// Gets a list of ConnectionMutationInfo objects that drives the types of connection mutation
        /// that occur.
        /// </summary>
        public ConnectionMutationInfoList ConnectionMutationInfoList
        {
            get { return _connectionMutationInfoList; }
        }

        /// <summary>
        /// Gets or sets the fitness history length to be used by genomes when recording their fitness.
        /// </summary>
        public int FitnessHistoryLength
        {
            get { return _fitnessHistoryLength; }
            set { _fitnessHistoryLength = value; }
        }

        #endregion

        #region Private Methods

        private RouletteWheelLayout CreateRouletteWheelLayout()
        {
            double[] probabilities = new double[] 
                {
                    _connectionWeightMutationProbability, 
                    _addNodeMutationProbability,
                    _addConnectionMutationProbability,
                    _nodeAuxStateMutationProbability,
                    _deleteConnectionMutationProbability
                };
            return new RouletteWheelLayout(probabilities);
        }

        private RouletteWheelLayout CreateRouletteWheelLayout_NonDestructive()
        {
            double[] probabilities = new double[] 
                {
                    _connectionWeightMutationProbability, 
                    _addNodeMutationProbability,
                    _addConnectionMutationProbability,
                    _nodeAuxStateMutationProbability
                };
            return new RouletteWheelLayout(probabilities);
        }

        /// <summary>
        /// Returns the default connection weight mutation scheme.
        /// </summary>
        private ConnectionMutationInfoList CreateConnectionWeightMutationScheme_Default()
        {
            ConnectionMutationInfoList list = new ConnectionMutationInfoList(12);

            // Gaussian jiggle with sigma=0.02 (most values between +-0.04)
            // Jiggle 1,2 and 3 connections respectively.
            list.Add(new ConnectionMutationInfo(0.11375, ConnectionPerturbanceType.JiggleGaussian,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 1, 0.0, 0.02));

            list.Add(new ConnectionMutationInfo(0.11375, ConnectionPerturbanceType.JiggleGaussian,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 2, 0.0, 0.02));

            list.Add(new ConnectionMutationInfo(0.11375, ConnectionPerturbanceType.JiggleGaussian,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 3, 0.0, 0.02));

            // Jiggle 2% of connections.
            list.Add(new ConnectionMutationInfo(0.11375, ConnectionPerturbanceType.JiggleGaussian,
                                                ConnectionSelectionType.Proportional, 0.02, 0, 0.0, 0.02));

            // Gaussian jiggle with sigma=1 (most values between +-2)
            // Jiggle 1,2 and 3 connections respectively.
            list.Add(new ConnectionMutationInfo(0.11375, ConnectionPerturbanceType.JiggleGaussian,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 1, 0.0, 1));

            list.Add(new ConnectionMutationInfo(0.11375, ConnectionPerturbanceType.JiggleGaussian,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 2, 0.0, 1));

            list.Add(new ConnectionMutationInfo(0.11375, ConnectionPerturbanceType.JiggleGaussian,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 3, 0.0, 1));

            // Jiggle 2% of connections.
            list.Add(new ConnectionMutationInfo(0.11275, ConnectionPerturbanceType.JiggleGaussian,
                                                ConnectionSelectionType.Proportional, 0.02, 0, 0.0, 1));

            // Reset mutations. 1, 2 and 3 connections respectively.
            list.Add(new ConnectionMutationInfo(0.03, ConnectionPerturbanceType.Reset,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 1, 0.0, 0));

            list.Add(new ConnectionMutationInfo(0.03, ConnectionPerturbanceType.Reset,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 2, 0.0, 0));

            list.Add(new ConnectionMutationInfo(0.03, ConnectionPerturbanceType.Reset,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 3, 0.0, 0));

            // Reset 2% of connections.
            list.Add(new ConnectionMutationInfo(0.001, ConnectionPerturbanceType.Reset,
                                                ConnectionSelectionType.Proportional, 0.02, 0, 0.0, 0));
            list.Initialize();
            return list;
        }

        /// <summary>
        /// Returns the connection weight mutation scheme from SharpNEAT version 1.x
        /// </summary>
        private ConnectionMutationInfoList CreateConnectionWeightMutationScheme_SharpNEAT1()
        {
            ConnectionMutationInfoList list = new ConnectionMutationInfoList(5);
            list.Add(new ConnectionMutationInfo(0.125, ConnectionPerturbanceType.JiggleUniform,
                                                ConnectionSelectionType.Proportional, 0.5, 0, 0.05, 0.0));

            list.Add(new ConnectionMutationInfo(0.125, ConnectionPerturbanceType.JiggleUniform,
                                                ConnectionSelectionType.Proportional, 0.1, 0, 0.05, 0.0));

            list.Add(new ConnectionMutationInfo(0.125, ConnectionPerturbanceType.JiggleUniform,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 1, 0.05, 0.0)); 

            list.Add(new ConnectionMutationInfo(0.5, ConnectionPerturbanceType.Reset,
                                                ConnectionSelectionType.Proportional, 0.1, 0, 0.0, 0.0));   

            list.Add(new ConnectionMutationInfo(0.125, ConnectionPerturbanceType.Reset,
                                                ConnectionSelectionType.FixedQuantity, 0.0, 1, 0.0, 0.0));
            list.Initialize();
            return list;
        }

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Creates parameters suitable for use during the simplifying mode of a NEAT search. Addition 
        /// mutations are disabled, deletion and weight mutation rates are increased.
        /// </summary>
        public static NeatGenomeParameters CreateSimplifyingParameters(NeatGenomeParameters copyFrom)
        {
            NeatGenomeParameters newParams = new NeatGenomeParameters(copyFrom);
            newParams._connectionWeightMutationProbability = 0.6;
            newParams._addNodeMutationProbability = 0.0;
            newParams._addConnectionMutationProbability = 0.0;
            // TODO: better method for automatically generating simplifying parameters?
            newParams._nodeAuxStateMutationProbability = copyFrom._nodeAuxStateMutationProbability;
            newParams._deleteConnectionMutationProbability = 0.4;
            newParams._rouletteWheelLayout = newParams.CreateRouletteWheelLayout();
            newParams._rouletteWheelLayoutNonDestructive = newParams.CreateRouletteWheelLayout_NonDestructive();

            newParams._connectionMutationInfoList = new ConnectionMutationInfoList(copyFrom._connectionMutationInfoList);

            // SharpNEAT version 1.x used this scheme.
            // newParams._connectionMutationInfoList.Add(new ConnectionMutationInfo(0.333, ConnectionPerturbanceType.JiggleUniform, ConnectionSelectionType.Proportional, 0.3, 0, 0.05, 0.0));
            // newParams._connectionMutationInfoList.Add(new ConnectionMutationInfo(0.333, ConnectionPerturbanceType.JiggleUniform, ConnectionSelectionType.Proportional, 0.1, 0, 0.05, 0.0));
            // newParams._connectionMutationInfoList.Add(new ConnectionMutationInfo(0.333, ConnectionPerturbanceType.JiggleUniform, ConnectionSelectionType.Proportional, 0.01, 0, 0.05, 0.0));
            newParams._connectionMutationInfoList.Initialize();
            return newParams;
        }

        #endregion
    }
}
