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

namespace SharpNeat.Neat.Genome
{
    public static class NeatGenomeBuilderFactory<T>
        where T : struct
    {
        public static INeatGenomeBuilder<T> Create(
            MetaNeatGenome<T> metaNeatGenome)
        {
            if(metaNeatGenome.IsAcyclic)
            {
                return new NeatGenomeAcyclicBuilder<T>(metaNeatGenome);
            }
            // else
            return new NeatGenomeBuilder<T>(metaNeatGenome);
        }
    }
}
