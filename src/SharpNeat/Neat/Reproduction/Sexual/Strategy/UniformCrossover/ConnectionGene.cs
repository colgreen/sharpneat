/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    internal readonly struct ConnectionGene<T> where T : struct
    {
        public readonly DirectedConnection Endpoints;
        public readonly T Weight;

        public ConnectionGene(in DirectedConnection endpoints, T weight)
        {
            this.Endpoints = endpoints;
            this.Weight = weight;
        }
    }   
}
