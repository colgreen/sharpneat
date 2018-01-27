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

using System.Collections.Generic;
using Redzen.Numerics;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Represents a list of ConnectionMutationInfo objects. 
    /// 
    /// Also hold a RouletteWheelLayout built from the activation probability values of the
    /// contained ConnectionMutationInfo objects. This can be used to select a mutation type
    /// to use within the NeatGenome mutation routines.
    /// </summary>
    public class ConnectionMutationInfoList : List<ConnectionMutationInfo>
    {
        DiscreteDistribution _dist;

        #region Constructors

        /// <summary>
        /// Constructs an empty list.
        /// </summary>
        public ConnectionMutationInfoList()
        {
        }

        /// <summary>
        /// Constructs an empty list.
        /// </summary>
        public ConnectionMutationInfoList(int capacity) 
            : base(capacity)
        {
        }

        /// <summary>
        /// Copy constructor. We make individual copies of the list items (deep copy), rather than
        /// only copying the item object references.
        /// </summary>
        public ConnectionMutationInfoList(ICollection<ConnectionMutationInfo> copyFrom) 
            : base(copyFrom.Count)
        {
            foreach(ConnectionMutationInfo srcInfo in copyFrom) {
                Add(new ConnectionMutationInfo(srcInfo));
            }
        }

        #endregion

        #region Public Methods/Properties

        /// <summary>
        /// Initialize the list. Call this after all items have been add to the list. This
        /// creates a RouletteWheelLayout based upon the activation probability of each item 
        /// in the list.
        /// </summary>
        public void Initialize()
        {
            _dist = CreateRouletteWheelLayout();
        }

        /// <summary>
        /// Gets one of the ConnectionMutationInfo items at random based upon the ActivationProbability 
        /// of the contained items.
        /// </summary>
        public ConnectionMutationInfo GetRandomItem()
        {
            return this[_dist.Sample()];
        }

        /// <summary>
        /// Gets the RouletteWheelLayout for the items in the list. This is based upon the activation 
        /// probability of each item in the list at the time Initialise was called.
        /// </summary>
        public DiscreteDistribution RouletteWheelLayout
        {
            get { return _dist; }
        }

        #endregion

        #region Private Methods

        private DiscreteDistribution CreateRouletteWheelLayout()
        {
            int count = this.Count;
            double[] probabilities = new double[count];
            for(int i=0; i<count; i++) {
                probabilities[i] = this[i].ActivationProbability;
            }
            return new DiscreteDistribution(probabilities);
        }

        #endregion
    }
}
