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

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a single item in an IActivationFunctionLibrary.
    /// The item represents an IActivationFunction and its ID and selection probability within
    /// the owning IActivationFunctionLibrary.
    /// </summary>
    public struct ActivationFunctionInfo
    {
        readonly int _id;
        readonly double _selectionProbability;
        readonly IActivationFunction _activationFn;
        
        #region Constructor

        /// <summary>
        /// Construct with the provided id, selection probability and activation function.
        /// </summary>
        public ActivationFunctionInfo(int id, 
                                      double selectionProbability,
                                      IActivationFunction activationFn)
        {
            _id = id;
            _selectionProbability = selectionProbability;
            _activationFn = activationFn;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the integer ID assigned to the function in the owning function library.
        /// </summary>
        public int Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the selection probability of the item.
        /// </summary>
        public double SelectionProbability
        {
            get { return _selectionProbability; }
        }

        /// <summary>
        /// Gets the activation function object.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get { return _activationFn; }
        }

        #endregion
    }
}
