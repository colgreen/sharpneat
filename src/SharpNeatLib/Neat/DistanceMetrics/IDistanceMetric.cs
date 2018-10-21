using System.Collections.Generic;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.DistanceMetrics
{
    /// <summary>
    /// Represents a metric for measuring the distance between two genome positions in an encoding space, and thus,
    /// in principle, the compatibility of the two genomes with respect to the probability of creating fit offspring.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public interface IDistanceMetric<T>
        where T : struct
    {
        /// <summary>
        /// Calculates the distance between two positions.
        /// </summary>
        double CalcDistance(ConnectionGenes<T> p1, ConnectionGenes<T> p2);

        /// <summary>
        /// Tests if the distance between the two positions is greater than some threshold.
        /// 
        /// A simple way of implementing this method would be to calculate the distance between the
        /// two genomes and test if it is over the threshold. That approach requires that the internal
        /// data of both positions be fully compared. However, it is faster to compare the contents of the
        /// two positions maintaining an accumulated distance value as we progress through the comparison, 
        /// and to exit the method when and if the threshold is passed.
        /// </summary>
        bool TestDistance(ConnectionGenes<T> p1, ConnectionGenes<T> p2, double threshold);

        /// <summary>
        /// Calculates the centroid for the given set of points.
        /// The centroid is a central position within a set of points that minimizes the sum of the squared distance
        /// between each of those points and the centroid. As such it can also be thought of as being an exemplar 
        /// for a set of points.
        /// 
        /// The centroid calculation is dependent on the distance metric, hence this method is defined on IDistanceMetric.
        /// For some distance metrics the centroid may not be a unique point, in those cases one of the possible centroids
        /// is returned.
        /// 
        /// A centroid is used in k-means clustering to define the centre of a cluster.
        /// </summary>
        ConnectionGenes<T> CalculateCentroid(IEnumerable<ConnectionGenes<T>> coordList);
    }
}
