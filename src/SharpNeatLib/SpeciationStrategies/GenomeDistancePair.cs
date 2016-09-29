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
using System;

namespace SharpNeat.SpeciationStrategies
{
    internal struct GenomeDistancePair<TGenome> : IComparable<GenomeDistancePair<TGenome>>
    {
        internal double _distance;
        internal TGenome _genome;

        internal GenomeDistancePair(double distance, TGenome genome)
        {
            _distance = distance;
            _genome = genome;
        }

        public int CompareTo(GenomeDistancePair<TGenome> other)
        {
            // Sorts in descending order.
            // Just remember, -1 means we don't change the order of x and y.
            if(_distance > other._distance) {
                return -1;
            }
            if(_distance < other._distance) {
                return 1;
            }
            return 0;
        }
    }
}
