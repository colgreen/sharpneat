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

using System.Collections.Generic;
using SharpNeat.Utility;

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
        RouletteWheelLayout _rouletteWheelLayout;

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
        /// Initialize the list. Call this after all items have been aded to the list. This
        /// creates a RouletteWheelLayout based upon the activation probability of each item 
        /// in the list.
        /// </summary>
        public void Initialize()
        {
            _rouletteWheelLayout = CreateRouletteWheelLayout();
        }

        /// <summary>
        /// Gets one of the ConnectionMutationInfo items at random based upon the ActivationProbability 
        /// of the contained items.
        /// </summary>
        public ConnectionMutationInfo GetRandomItem(FastRandom rng)
        {
            return this[RouletteWheel.SingleThrow(_rouletteWheelLayout, rng)];
        }

        /// <summary>
        /// Gets the RouletteWheelLayout for the items in the list. This is based upon the activation 
        /// probability of each item in the list at the time Initialise was called.
        /// </summary>
        public RouletteWheelLayout RouletteWheelLayout
        {
            get { return _rouletteWheelLayout; }
        }

        #endregion

        #region Private Methods

        private RouletteWheelLayout CreateRouletteWheelLayout()
        {
            int count = this.Count;
            double[] probabilities = new double[count];
            for(int i=0; i<count; i++) {
                probabilities[i] = this[i].ActivationProbability;
            }
            return new RouletteWheelLayout(probabilities);
        }

        #endregion
    }
}
