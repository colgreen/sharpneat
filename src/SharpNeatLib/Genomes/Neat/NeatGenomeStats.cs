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

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Various statistics for NeatGenome.
    /// </summary>
    public class NeatGenomeStats
    {
        /// <summary>
        /// Total number of connection weight mutation operations. This is the number of calls to the
        /// mutatation routine, not the total number of weights mutated (there are typically multiple
        /// weights mutated on a genome at a time).
        /// </summary>
        public ulong _mutationCountConnectionWeights;
        /// <summary>
        /// Total number of 'add node' mutations.
        /// </summary>
        public ulong _mutationCountAddNode;
        /// <summary>
        /// Total number of 'add connection' mutations.
        /// </summary>
        public ulong _mutationCountAddConnection;
        /// <summary>
        /// Total number of 'delete connection' mutations.
        /// </summary>
        public ulong _mutationCountDeleteConnection;
        /// <summary>
        /// Total number of 'delete simple neuron' mutations.
        /// </summary>
        public ulong _mutationCountDeleteSimpleNeuron;
    }
}
