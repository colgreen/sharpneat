using System.Collections.Generic;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Speciation
{
    /// <summary>
    /// Static utility methods for GeneticKMeansSpeciationStrategy.
    /// </summary>
    public static class GeneticKMeansSpeciationStrategyUtils
    {


        #region Public Static Methods [IDistanceMetric Utility Methods]

        /// <summary>
        /// Get the index of the species with a centroid that is nearest to the provided genome.
        /// </summary>
        public static int GetNearestSpecies<T>(
            IDistanceMetric<T> distanceMetric,
            NeatGenome<T> genome,
            Species<T>[] speciesArr)
        where T : struct
        {
            int nearestSpeciesIdx = 0;
            double nearestDistance = distanceMetric.GetDistance(genome.ConnectionGenes, speciesArr[0].Centroid);

            for(int i=1; i < speciesArr.Length; i++)
            {
                double distance = distanceMetric.GetDistance(genome.ConnectionGenes, speciesArr[i].Centroid);
                if(distance < nearestDistance)
                {
                    nearestSpeciesIdx = i;
                    nearestDistance = distance;
                }
            }
            return nearestSpeciesIdx;
        }

        #endregion
    }
}
