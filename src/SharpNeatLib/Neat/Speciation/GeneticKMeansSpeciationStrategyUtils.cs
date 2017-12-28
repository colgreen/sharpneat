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
        public static Species<T> GetNearestSpecies<T>(
            NeatGenome<T> genome,
            Species<T>[] speciesArr,
            IDistanceMetric<T> distanceMetric,
            out int nearestSpeciesIdx)
        where T : struct
        {
            var nearestSpecies = speciesArr[0];
            nearestSpeciesIdx = 0;
            double nearestDistance = distanceMetric.GetDistance(genome.ConnectionGenes, speciesArr[0].Centroid);

            for(int i=1; i < speciesArr.Length; i++)
            {
                double distance = distanceMetric.GetDistance(genome.ConnectionGenes, speciesArr[i].Centroid);
                if(distance < nearestDistance)
                {
                    nearestSpecies = speciesArr[i];
                    nearestSpeciesIdx = i;
                    nearestDistance = distance;
                }
            }
            return nearestSpecies;
        }

        public static T GetAndRemove<T>(IList<T> list, int idx)
        {
            T tmp = list[idx];
            list.RemoveAt(idx);
            return tmp;
        }
    }
}
