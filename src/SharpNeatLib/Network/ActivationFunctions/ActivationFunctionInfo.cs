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
