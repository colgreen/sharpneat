/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

using System.Collections.Generic;

namespace SharpNeat.Evaluation
{
    public class DefaultFitnessInfoComparer : IComparer<FitnessInfo>
    {
        public static DefaultFitnessInfoComparer Singleton = new DefaultFitnessInfoComparer();

        public int Compare(FitnessInfo x, FitnessInfo y)
        {
            return x.PrimaryFitness.CompareTo(y.PrimaryFitness);
        }
    }
}
