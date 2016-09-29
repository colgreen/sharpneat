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

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Each instance of ConnectionMutationInfo describes a type of mutation and associated parameters.
    /// 
    /// A list of ConnectionMutationInfo objects describe a connection weight mutation scheme - a set
    /// of a number of different types of mutation along with the probability of each type occuring
    /// when spawning offspring genomes asexually.
    /// </summary>
    public class ConnectionMutationInfo
    {
        readonly double _activationProbability;
        readonly ConnectionPerturbanceType _perturbanceType;
        readonly ConnectionSelectionType _selectionType;
        readonly double _selectionProportion;
        readonly int _selectionQuantity;
        readonly double _perturbanceMagnitude;
        readonly double _sigma;

        #region Constructors

        /// <summary>
        /// Construct with the provided mutation type and supporting parameters.
        /// </summary>
        /// <param name="activationProbability">The probability that this type of mutation will be chosen</param>
        /// <param name="perturbanceType">The type of weight perturbance the info object represents.</param>
        /// <param name="selectionType">The type of connection subset selection the info object represents.</param>
        /// <param name="selectionProportion">For ConnectionSelectionType.Proportional this gives the proportion of connections to select.</param>
        /// <param name="selectionQuantity">For ConnectionSelectionType.FixedQuantity this gives the number of connections to select.</param>
        /// <param name="perturbanceMagnitude">For ConnectionPerturbanceType.JiggleEven this gives the magnitude of the extents of the 
        /// even distribution used for generating jiggle weight deltas.</param>
        /// <param name="sigma">For For ConnectionPerturbanceType.JiggleGaussian this specifies the sigma to use for
        /// the gaussian distribution used for generating jiggle weight deltas.</param>
        public ConnectionMutationInfo(double activationProbability, 
                                      ConnectionPerturbanceType perturbanceType,
                                      ConnectionSelectionType selectionType,
                                      double selectionProportion,
                                      int selectionQuantity,
                                      double perturbanceMagnitude,
                                      double sigma)
        {
            _activationProbability = activationProbability;
            _perturbanceType = perturbanceType;
            _selectionType = selectionType;
            _selectionProportion = selectionProportion;
            _selectionQuantity = selectionQuantity;
            _perturbanceMagnitude = perturbanceMagnitude;
            _sigma = sigma;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public ConnectionMutationInfo(ConnectionMutationInfo copyFrom)
        {
            _activationProbability = copyFrom._activationProbability;
            _perturbanceType = copyFrom._perturbanceType;
            _selectionType = copyFrom._selectionType;
            _selectionProportion = copyFrom._selectionProportion;
            _selectionQuantity = copyFrom._selectionQuantity;
            _perturbanceMagnitude = copyFrom._perturbanceMagnitude;
            _sigma = copyFrom._sigma;       
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the probability that this type of mutation will be chosen.
        /// </summary>
        public double ActivationProbability
        {
            get { return _activationProbability; }
        }

        /// <summary>
        /// Gets the type of weight perturbance the info object represents.
        /// </summary>
        public ConnectionPerturbanceType PerturbanceType
        {
            get { return _perturbanceType; }
        }

        /// <summary>
        /// Gets the type of connection subset selection the info object represents.
        /// </summary>
        public ConnectionSelectionType SelectionType
        {
            get { return _selectionType; }
        }

        /// <summary>
        /// Gets  the proportion of connections to select; for ConnectionSelectionType.Proportional.
        /// </summary>
        public double SelectionProportion
        {
            get { return _selectionProportion; }
        }
        
        /// <summary>
        /// Gets the number of connections to select; for ConnectionSelectionType.FixedQuantity.
        /// </summary>
        public int SelectionQuantity
        {
            get { return _selectionQuantity; }
        }

        /// <summary>
        /// Gets the magnitude of the extents of the even distribution used for generating jiggle weight deltas; for ConnectionPerturbanceType.JiggleEven.
        /// </summary>
        public double PerturbanceMagnitude
        {
            get { return _perturbanceMagnitude; }
        }

        /// <summary>
        /// Gets the sigma to use for the gaussian distribution used for generating jiggle weight deltas; for ConnectionPerturbanceType.JiggleGaussian.
        /// </summary>
        public double Sigma
        {
            get { return _sigma; }
        }

        #endregion
    }
}
