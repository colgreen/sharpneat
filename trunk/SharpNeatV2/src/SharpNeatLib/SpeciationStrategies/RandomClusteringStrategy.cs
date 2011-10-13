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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Core;
using SharpNeat.Utility;

namespace SharpNeat.SpeciationStrategies
{
    /// <summary>
    /// A speciation strategy that allocates genomes to species randomly.
    /// Although allocation is random the strategy does maintain evenly sized species.
    /// Primarily used for testing/debugging and demonstrating comparative effectiveness of random
    /// allocation compared to other strategies.
    /// </summary>
    /// <typeparam name="TGenome">The genome type to apply clustering to.</typeparam>
    public class RandomClusteringStrategy<TGenome> : ISpeciationStrategy<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        readonly FastRandom _rng = new FastRandom();
       
        /// <summary>
        /// Speciates the genomes in genomeList into the number of species specified by specieCount
        /// and returns a newly constructed list of Specie objects containing the speciated genomes.
        /// </summary>
        public IList<Specie<TGenome>> InitializeSpeciation(IList<TGenome> genomeList, int specieCount)
        {
            // Craete the empty specie objects.
            List<Specie<TGenome>> specieList = new List<Specie<TGenome>>(specieCount);
            int capacity = (int)Math.Ceiling((double)genomeList.Count/(double)specieCount);
            for(int i=0; i<specieCount; i++) {
                specieList.Add(new Specie<TGenome>((uint)i, i, capacity));
            }

            // Speciate the genomes.
            SpeciateGenomes(genomeList, specieList);
            return specieList;
        }

        /// <summary>
        /// Speciates the genomes in genomeList into the provided species. It is assumed that
        /// the genomeList represents all of the required genomes and that the species are currently empty.
        /// 
        /// This method can be used for initialization or completely respeciating an existing genome population.
        /// </summary>
        public void SpeciateGenomes(IList<TGenome> genomeList, IList<Specie<TGenome>> specieList)
        {
            Debug.Assert(SpeciationUtils.TestEmptySpecies(specieList), "SpeciateGenomes(IList<TGenome>,IList<Species<TGenome>>) called with non-empty species");
            Debug.Assert(genomeList.Count >= specieList.Count, string.Format("SpeciateGenomes(IList<TGenome>,IList<Species<TGenome>>). Species count [{0}] is greater than genome count [{1}].", specieList.Count, genomeList.Count));

            // Make a copy of genomeList and shuffle the items.
            List<TGenome> gList = new List<TGenome>(genomeList);
            Utilities.Shuffle(gList, _rng);

            // We evenly distribute genomes between species. 
            // Calc how many genomes per specie. Baseline number given by integer division rounding down (by truncating fractional part).
            // This is guaranteed to be at least 1 because genomeCount >= specieCount.
            int genomeCount = genomeList.Count;
            int specieCount = specieList.Count;
            int genomesPerSpecie = genomeCount / specieCount;

            // Allocate genomes to species.
            int genomeIdx = 0;
            for(int i=0; i<specieCount; i++)
            {
                Specie<TGenome> specie = specieList[i];
                for(int j=0; j<genomesPerSpecie; j++, genomeIdx++) 
                {
                    gList[genomeIdx].SpecieIdx = specie.Idx;
                    specie.GenomeList.Add(gList[genomeIdx]);
                }
            }

            // Evenly allocate any remaining genomes.
            int[] specieIdxArr = new int[specieCount];
            for(int i=0; i<specieCount; i++) {
                specieIdxArr[i] = i;
            }
            Utilities.Shuffle(specieIdxArr, _rng);

            for(int i=0; i<specieCount && genomeIdx < genomeCount; i++, genomeIdx++) 
            {
                int specieIdx = specieIdxArr[i];
                gList[genomeIdx].SpecieIdx = specieIdx;
                specieList[specieIdx].GenomeList.Add(gList[genomeIdx]);
            }

            Debug.Assert(SpeciationUtils.PerformIntegrityCheck(specieList));
        }

        /// <summary>
        /// Speciates the offspring genomes in genomeList into the provided species. In contrast to
        /// SpeciateGenomes() genomeList is taken to be a list of new genomes (e.g. offspring) that should be 
        /// added to existing species. That is, the species contain genomes that are not in genomeList
        /// that we wish to keep; typically these would be elite genomes that are the parents of the
        /// offspring.
        /// </summary>
        public void SpeciateOffspring(IList<TGenome> genomeList, IList<Specie<TGenome>> specieList)
        {
            // Each specie should contain at least one genome. We need at least one existing genome per specie to act
            // as a specie centroid in order to define where the specie is within the encoding space.
            Debug.Assert(SpeciationUtils.TestPopulatedSpecies(specieList), "SpeciateOffspring(IList<TGenome>,IList<Species<TGenome>>) called with an empty specie.");

            // Make a copy of genomeList and shuffle the items.
            List<TGenome> gList = new List<TGenome>(genomeList);
            Utilities.Shuffle(gList, _rng);

            // Count how many genomes we have in total.
            int genomeCount = gList.Count;
            int totalGenomeCount = genomeCount;
            foreach(Specie<TGenome> specie in specieList) {
                totalGenomeCount += specie.GenomeList.Count;
            }

            // We attempt to evenly distribute genomes between species. 
            // Calc how many genomes per specie. Baseline number given by integer division rounding down (by truncating fractional part).
            // This is guaranteed to be at least 1 because genomeCount >= specieCount.
            int specieCount = specieList.Count;
            int genomesPerSpecie = totalGenomeCount / specieCount;

            // Sort species, smallest first. We must make a copy of specieList to do this; Species must remain at
            // the correct index in the main specieList. The principle here is that we wish to ensure that genomes are
            // allocated to smaller species in preference to larger species, this is motivated by the desire to create
            // evenly sized species.
            List<Specie<TGenome>> sList = new List<Specie<TGenome>>(specieList);
            sList.Sort(delegate(Specie<TGenome> x, Specie<TGenome> y)
                        {   // We use the difference in size where we aren't expecting that diff value to overflow the range of an int.
                            return x.GenomeList.Count - y.GenomeList.Count;
                        });

            // Add genomes into each specie in turn until they each reach genomesPerSpecie in size.
            int genomeIdx = 0;
            for(int i=0; i<specieCount && genomeIdx<genomeCount; i++)
            {
                Specie<TGenome> specie = sList[i];
                int fillcount = genomesPerSpecie - specie.GenomeList.Count;

                if(fillcount <= 0) 
                {   // We may encounter species with more than genomesPerSpecie genomes. Since we have
                    // ordered the species by size we break out of this loop and allocate the remaining
                    // genomes randomly.
                    break;
                }

                // Don't allocate more genomes than there are remaining in genomeList.
                fillcount = Math.Min(fillcount, genomeCount - genomeIdx);

                // Allocate memory for the genomes we are about to allocate;
                // This eliminates potentially having to dynamically resize the list one or more times.
                if(specie.GenomeList.Capacity < specie.GenomeList.Count + fillcount) {
                    specie.GenomeList.Capacity = specie.GenomeList.Count + fillcount;
                }

                // genomeIdx test not required. Already taken into account by fillCount.
                for(int j=0; j<fillcount; j++) 
                {
                    gList[genomeIdx].SpecieIdx = specie.Idx;
                    specie.GenomeList.Add(gList[genomeIdx++]);
                }
            }

            // Evenly allocate any remaining genomes.
            int[] specieIdxArr = new int[specieCount];
            for(int i=0; i<specieCount; i++) {
                specieIdxArr[i] = i;
            }
            Utilities.Shuffle(specieIdxArr, _rng);

            for(int i=0; i<specieCount && genomeIdx < genomeCount; i++, genomeIdx++) 
            {
                int specieIdx = specieIdxArr[i];
                gList[genomeIdx].SpecieIdx = specieIdx;
                specieList[specieIdx].GenomeList.Add(gList[genomeIdx]);
            }

            Debug.Assert(SpeciationUtils.PerformIntegrityCheck(specieList));
        }
    }
}
