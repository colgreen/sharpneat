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
