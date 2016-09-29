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

// Disable missing comment warnings.
#pragma warning disable 1591

namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// Working data struct for use in FastCyclicNetwork and sub-classes.
    /// Represents a single connection - its weight and source/target neurons.
    /// </summary>
    public struct FastConnection
    {
        public int _srcNeuronIdx;
        public int _tgtNeuronIdx;
        public double _weight;
    }
}
