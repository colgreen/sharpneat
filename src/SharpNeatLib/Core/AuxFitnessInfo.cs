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
namespace SharpNeat.Core
{
    /// <summary>
    /// Auxiliary fitness info, i.e. for evaluation metrics other than the
    /// primary fitness metric but that nonetheless we are interested in observing.
    /// </summary>
    public struct AuxFitnessInfo
    {
        /// <summary>
        /// Auxiliary metric name.
        /// </summary>
        public string _name;
        /// <summary>
        /// Auxiliary metric value.
        /// </summary>
        public double _value;

        /// <summary>
        /// Construct with the provided name-value pair.
        /// </summary>
        public AuxFitnessInfo(string name, double value)
        {
            _name = name;
            _value = value;
        }
    }
}
