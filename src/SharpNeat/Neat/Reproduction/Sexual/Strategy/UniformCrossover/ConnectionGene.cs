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
using SharpNeat.Graphs;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    /// <summary>
    /// Represents a single connection gene.
    /// </summary>
    /// <typeparam name="T">Connection weight numeric data type.</typeparam>
    internal readonly struct ConnectionGene<T> where T : struct
    {
        /// <summary>
        /// The source and target node IDs.
        /// </summary>
        public readonly DirectedConnection Endpoints;
        /// <summary>
        /// Connection weight.
        /// </summary>
        public readonly T Weight;

        /// <summary>
        /// Construct with the given connection node endpoints, and connection weight.
        /// </summary>
        /// <param name="endpoints">The connection endpoints, i.e., the connection's source and target node IDs.</param>
        /// <param name="weight">The connection weight.</param>
        public ConnectionGene(in DirectedConnection endpoints, T weight)
        {
            this.Endpoints = endpoints;
            this.Weight = weight;
        }
    }
}
