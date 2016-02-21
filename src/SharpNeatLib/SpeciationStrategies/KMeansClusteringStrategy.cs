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

namespace SharpNeat.SpeciationStrategies
{
    // ENHANCEMENT: k-means++ seeks to choose better starting clusters. (http://en.wikipedia.org/wiki/K-means_clustering)
    // ENHANCEMENT: The filtering algorithm uses kd-trees to speed up each k-means step[9]. (http://en.wikipedia.org/wiki/K-means_clustering)
    // ENHANCEMENT: Euclidean squared distance metric is equivalent for k-means and faster than euclidean (http://www.improvedoutcomes.com/docs/WebSiteDocs/Clustering/Clustering_Parameters/Euclidean_and_Euclidean_Squared_Distance_Metrics.htm)

    /// <summary>
    /// An ISpeciationStrategy that speciates genomes using the k-means clustering method.
    /// k-means requires a distance metric and as such this class requires am IDistanceMetric to be provided at 
    /// construction time. Different distance metrics can be used including NeatDistanceMetric which is 
    /// equivalent to the metric used in the standard NEAT method albeit with a different clustering/speciation
    /// algorithm (Standard NEAT does not use k-means).
    /// </summary>
    /// <typeparam name="TGenome">The genome type to apply clustering to.</typeparam>
    public class KMeansClusteringStrategy<TGenome> : ISpeciationStrategy<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        const int __MAX_KMEANS_LOOPS = 5;
        readonly IDistanceMetric _distanceMetric;

        #region Constructor

        /// <summary>
        /// Constructor that accepts an IDistanceMetric to be used for the k-means method.
        /// </summary>
        public KMeansClusteringStrategy(IDistanceMetric distanceMetric)
        {
            _distanceMetric = distanceMetric;
        }

        #endregion

        #region ISpeciationStrategy<TGenome> Members

        /// <summary>
        /// Speciates the genomes in genomeList into the number of species specified by specieCount
        /// and returns a newly constructed list of Specie objects containing the speciated genomes.
        /// </summary>
        public IList<Specie<TGenome>> InitializeSpeciation(IList<TGenome> genomeList, int specieCount)
        {
            // Create empty specieList.
            // Use an initial specieList capacity that will limit the need for memory reallocation but that isn't
            // too wasteful of memory.
            int initSpeciesCapacity = (genomeList.Count * 2) / specieCount;
            List<Specie<TGenome>> specieList = new List<Specie<TGenome>>(specieCount);
            for(int i=0; i<specieCount; i++) {
                specieList.Add(new Specie<TGenome>((uint)i, i, initSpeciesCapacity));
            }

            // Speciate genomes into the empty species.
            SpeciateGenomes(genomeList, specieList);
            return specieList;
        }

        /// <summary>
        /// Speciates the genomes in genomeList into the provided specieList. It is assumed that
        /// the genomeList represents all of the required genomes and that the species are currently empty.
        /// 
        /// This method can be used for initialization or completely respeciating an existing genome population.
        /// </summary>
        public void SpeciateGenomes(IList<TGenome> genomeList, IList<Specie<TGenome>> specieList)
        {
            Debug.Assert(SpeciationUtils.TestEmptySpecies(specieList), "SpeciateGenomes(IList<TGenome>,IList<Species<TGenome>>) called with non-empty species");
            Debug.Assert(genomeList.Count >= specieList.Count, string.Format("SpeciateGenomes(IList<TGenome>,IList<Species<TGenome>>). Species count [{0}] is greater than genome count [{1}].", specieList.Count, genomeList.Count));

            // Randomly allocate the first k genomes to their own specie. Because there is only one genome in these
            // species each genome effectively represents a specie centroid. This is necessary to ensure we get k specieList.
            // If we randomly assign all genomes to species from the outset and then calculate centroids then typically some
            // of the species become empty.
            // This approach ensures that each species will have at least one genome - because that genome is the specie 
            // centroid and therefore has distance of zero from the centroid (itself).
            int specieCount = specieList.Count;
            for(int i=0; i<specieCount; i++)
            {
                Specie<TGenome> specie = specieList[i];
                genomeList[i].SpecieIdx = specie.Idx;
                specie.GenomeList.Add(genomeList[i]);

                // Just set the specie centroid directly.
                specie.Centroid = genomeList[i].Position;
            }

            // Now allocate the remaining genomes based on their distance from the centroids.
            int genomeCount = genomeList.Count;
            for(int i=specieCount; i<genomeCount; i++) {
                TGenome genome = genomeList[i];
                Specie<TGenome> closestSpecie = FindClosestSpecie(genome, specieList);
                genome.SpecieIdx = closestSpecie.Idx;
                closestSpecie.GenomeList.Add(genome);
            }

            // Recalculate each specie's centroid.
            foreach(Specie<TGenome> specie in specieList) {
                specie.Centroid = CalculateSpecieCentroid(specie);
            }

            // Perform the main k-means loop until convergence.
            SpeciateUntilConvergence(genomeList, specieList);

            Debug.Assert(SpeciationUtils.PerformIntegrityCheck(specieList));
        }

        /// <summary>
        /// Speciates the offspring genomes in offspringList into the provided specieList. In contrast to
        /// SpeciateGenomes() offspringList is taken to be a list of new genomes (offspring) that should be 
        /// added to existing species. That is, the species contain genomes that are not in offspringList
        /// that we wish to keep; typically these would be elite genomes that are the parents of the
        /// offspring.
        /// </summary>
        public void SpeciateOffspring(IList<TGenome> offspringList, IList<Specie<TGenome>> specieList)
        {
            // Each specie should contain at least one genome. We need at least one existing genome per specie to act
            // as a specie centroid in order to define where the specie is within the encoding space.
            Debug.Assert(SpeciationUtils.TestPopulatedSpecies(specieList), "SpeciateOffspring(IList<TGenome>,IList<Species<TGenome>>) called with an empty specie.");

            // Update the centroid of each specie. If we're adding offspring this means that old genomes 
            // have been removed from the population and therefore the centroids are out-of-date.
            foreach(Specie<TGenome> specie in specieList) {
                specie.Centroid = CalculateSpecieCentroid(specie);
            }
            
            // Allocate each offspring genome to the specie it is closest to. 
            foreach(TGenome genome in offspringList)
            {
                Specie<TGenome> closestSpecie = FindClosestSpecie(genome, specieList);
                closestSpecie.GenomeList.Add(genome);
                genome.SpecieIdx = closestSpecie.Idx;
            }

            // Recalculate each specie's centroid now that we have additional genomes in the specieList.
            foreach(Specie<TGenome> specie in specieList) {
                specie.Centroid = CalculateSpecieCentroid(specie);
            }

            // Accumulate *all* genomes into a flat genome list.
            int genomeCount = 0;
            foreach(Specie<TGenome> specie in specieList) {
                genomeCount += specie.GenomeList.Count;
            }

            List<TGenome> genomeList = new List<TGenome>(genomeCount);
            foreach(Specie<TGenome> specie in specieList) {
                genomeList.AddRange(specie.GenomeList);
            }

            // Perform the main k-means loop until convergence.
            SpeciateUntilConvergence(genomeList, specieList);

            Debug.Assert(SpeciationUtils.PerformIntegrityCheck(specieList));
        }

        #endregion

        #region Private Methods [k-means]

        /// <summary>
        /// Perform the main k-means loop until no genome reallocations occur or some maximum number of loops
        /// has been performed. Theoretically a small number of reallocations may occur for a great many loops 
        /// therefore we require the additional max loops threshold exit strategy - the clusters should be pretty
        /// stable and well defined after a few loops even if the the algorithm hasn't converged completely.
        /// </summary>
        private void SpeciateUntilConvergence(IList<TGenome> genomeList, IList<Specie<TGenome>> specieList)
        {
            List<Specie<TGenome>> emptySpecieList = new List<Specie<TGenome>>();
            int specieCount = specieList.Count;

            // Array of flags that indicate if a specie was modified (had genomes allocated to and/or from it).
            bool[] specieModArr = new bool[specieCount];

            // Main k-means loop.
            for(int loops=0; loops<__MAX_KMEANS_LOOPS; loops++)
            {
                // Track number of reallocations made on each loop.
                int reallocations = 0;

                // Loop over genomes. For each one find the specie it is closest to; if it is not the specie
                // it is currently in then reallocate it.
                foreach(TGenome genome in genomeList)
                {
                    Specie<TGenome> closestSpecie = FindClosestSpecie(genome, specieList);
                    if(genome.SpecieIdx != closestSpecie.Idx) 
                    {
                        // Track which species have been modified.
                        specieModArr[genome.SpecieIdx] = true;
                        specieModArr[closestSpecie.Idx] = true;

                        // Add the genome to its new specie and set its speciesIdx accordingly.
                        // For now we leave the genome in its original species; It's more efficient to determine
                        // all reallocations and then remove reallocated genomes from their origin specie all together;
                        // This is because we can shuffle down the remaining genomes in a specie to fill the gaps made by
                        // the removed genomes - and do so in one round of shuffling instead of shuffling to fill a gap on
                        // each remove.
                        closestSpecie.GenomeList.Add(genome);
                        genome.SpecieIdx = closestSpecie.Idx;
                        reallocations++;
                    }
                }

                // Complete the reallocations.
                for(int i=0; i<specieCount; i++)
                {
                    if(!specieModArr[i]) 
                    {   // Specie not changed. Skip.
                        continue;
                    }

                    // Reset flag.
                    specieModArr[i] = false;
                    
                    // Remove the genomes that have been allocated to other other species. We fill the resulting 
                    // gaps by shuffling down the remaining genomes.
                    Specie<TGenome> specie = specieList[i];
                    specie.GenomeList.RemoveAll(delegate(TGenome genome) 
                    {
                        return genome.SpecieIdx != specie.Idx;
                    });

                    // Track empty species. We will allocate genomes to them after this loop.
                    // This is necessary as some distance metrics can result in empty species occuring.
                    if(0 == specie.GenomeList.Count) {
                        emptySpecieList.Add(specie);
                    }
                    else {
                        // Recalc the specie centroid now that it contains a different set of genomes.
                        specie.Centroid = CalculateSpecieCentroid(specie);
                    }
                }

                // Check for empty species. We need to reallocate some genomes into the empty specieList to maintain the 
                // required number of species.
                if(0 != emptySpecieList.Count)
                {
                    // We find the genomes in the population as a whole that are furthest from their containing specie's 
                    // centroid genome - we call these outlier genomes. We then move these genomes into the empty species to
                    // act as the sole member and centroid of those speciea; These act as specie seeds for the next k-means loop.
                    TGenome[] genomeByDistanceArr = GetGenomesByDistanceFromSpecie(genomeList, specieList);

                    // Reallocate each of the outlier genomes from their current specie to an empty specie.
                    int emptySpecieCount = emptySpecieList.Count;
                    int outlierIdx = 0;
                    for(int i=0; i<emptySpecieCount; i++)
                    {
                        // Find the next outlier genome that can be re-allocated. Skip genomes that are the
                        // only member of a specie - that would just create another empty specie.
                        TGenome genome;
                        Specie<TGenome> sourceSpecie;
                        do
                        {
                            genome = genomeByDistanceArr[outlierIdx++];
                            sourceSpecie = specieList[genome.SpecieIdx];
                        } 
                        while(sourceSpecie.GenomeList.Count == 1 && outlierIdx < genomeByDistanceArr.Length);

                        if(outlierIdx == genomeByDistanceArr.Length)
                        {   // Theoretically impossible. We do the test so that we get an easy to trace error message if it does happen.
                            throw new SharpNeatException("Error finding outlier genome. No outliers could be found in any specie with more than 1 genome.");
                        }

                        // Get ref to the empty specie and register both source and target specie with specieModArr.
                        Specie<TGenome> emptySpecie = emptySpecieList[i];
                        specieModArr[emptySpecie.Idx] = true;
                        specieModArr[sourceSpecie.Idx] = true;

                        // Reallocate the genome. Here we do the remove operation right away; We aren't expecting to deal with many empty
                        // species, usually it will be one or two at most; Any more and there's probably something wrong with the distance
                        // metric, e.g. maybe it doesn't satisfy the triangle inequality (see wikipedia).
                        // Another reason to remove right is to eliminate the possibility of removing multiple outlier genomes from the 
                        // same specie and potentially leaving it empty; The test in the do-while loop above only takes genomes from
                        // currently non-empty species.
                        sourceSpecie.GenomeList.Remove(genome);
                        emptySpecie.GenomeList.Add(genome);
                        genome.SpecieIdx = emptySpecie.Idx;
                        reallocations++;
                    }

                    // Recalculate centroid for all affected species.
                    for(int i=0; i<specieCount; i++) {
                        if(specieModArr[i]) 
                        {   // Reset flag while we're here. Do this first to help maintain CPU cache coherency (we just tested it).
                            specieModArr[i] = false;
                            specieList[i].Centroid = CalculateSpecieCentroid(specieList[i]);
                        }
                    }

                    // Clear emptySpecieList after using it. Otherwise we are holding old references and thus creating
                    // work for the garbage collector.
                    emptySpecieList.Clear();
                }

                // Exit the loop if no genome reallocations have occured. The species are stable, speciation is completed.
                if(0==reallocations) {
                    break;
                }
            }
        }

        /// <summary>
        /// Recalculate the specie centroid based on the genomes currently in the specie.
        /// </summary>
        private CoordinateVector CalculateSpecieCentroid(Specie<TGenome> specie)
        {
            // Special case - 1 genome in specie (its position *is* the specie centroid).
            if(1 == specie.GenomeList.Count) {
                return new CoordinateVector(specie.GenomeList[0].Position.CoordArray);
            }

            // Create a temp list containing all of the genome positions.
            List<TGenome> genomeList = specie.GenomeList;
            int count = genomeList.Count;
            List<CoordinateVector> coordList = new List<CoordinateVector>(count);
            for(int i=0; i<count; i++) {
                coordList.Add(genomeList[i].Position);
            }

            // The centroid calculation is a function of the distance metric.
            return _distanceMetric.CalculateCentroid(coordList);
        }

        // ENHANCEMENT: Optimization candidate.
        /// <summary>
        /// Gets an array of all genomes ordered by their distance from their current specie.
        /// </summary>
        private TGenome[] GetGenomesByDistanceFromSpecie(IList<TGenome> genomeList, IList<Specie<TGenome>> specieList)
        {
            // Build a list of all genomes paired with their distance from their centriod.
            int genomeCount = genomeList.Count;
            GenomeDistancePair<TGenome>[] genomeDistanceArr = new GenomeDistancePair<TGenome>[genomeCount];
            for(int i=0; i<genomeCount; i++)
            {
                TGenome genome = genomeList[i];
                double distance = _distanceMetric.MeasureDistance(genome.Position, specieList[genome.SpecieIdx].Centroid);
                genomeDistanceArr[i] = new GenomeDistancePair<TGenome>(distance, genome);
            }

            // Sort list. Longest distance first.
            Array.Sort(genomeDistanceArr);

            // Put the sorted genomes in an array and return it.
            TGenome[] genomeArr = new TGenome[genomeCount];
            for(int i=0; i<genomeCount; i++) {
                genomeArr[i] = genomeDistanceArr[i]._genome;
            }

            return genomeArr;
        }

        /// <summary>
        /// Find the specie that a genome is closest to as determined by the distance metric.
        /// </summary>
        private Specie<TGenome> FindClosestSpecie(TGenome genome, IList<Specie<TGenome>> specieList)
        {
            // Measure distance to first specie's centroid.
            Specie<TGenome> closestSpecie = specieList[0];
            double closestDistance = _distanceMetric.MeasureDistance(genome.Position, closestSpecie.Centroid);

            // Measure distance to all remaining species.
            int speciesCount = specieList.Count;
            for(int i=1; i<speciesCount; i++)
            {
                double distance = _distanceMetric.MeasureDistance(genome.Position, specieList[i].Centroid);
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSpecie = specieList[i];
                }
            }

            return closestSpecie;
        }

        #endregion
    }
}
