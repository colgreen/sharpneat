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
using System.Collections.Generic;

namespace SharpNeat.Core
{
    /// <summary>
    /// An IDistanceMetric represents a metric for measuring the distance between two genome positions in an
    /// encoding space, and thus the compatibility of the two genomes with respect to the probability of creating
    /// fit offspring.
    /// 
    /// What makes a good or ideal compatibility metric is an open question at the time of writing (August-2009).
    /// </summary>
    public interface IDistanceMetric
    {
        /// <summary>
        /// Tests if the distance between the two positions is greater than some threshold.
        /// 
        /// A simple way of implementing this method would be to calculate the distance between the
        /// two genomes and test if it is over the threshold. That approach requires that the internal
        /// data of both positions be fully compared. However, it is faster to compare the contents of the
        /// two positions maintaining an accumulated distance value as we progress through the comparison, 
        /// and to return out of the method when and if the threshold is passed. Writing distance metric 
        /// code in this way is encouraged.
        /// </summary>
        bool MeasureDistance(CoordinateVector p1, CoordinateVector p2, double threshold);

        /// <summary>
        /// Measures the distance between two positions.
        /// </summary>
        double MeasureDistance(CoordinateVector p1, CoordinateVector p2);

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
        /// A centroid is used in k-means clustering to define the center of a cluster.
        /// </summary>
        CoordinateVector CalculateCentroid(IList<CoordinateVector> coordList);

        /// <summary>
        /// Parallelized version of CalculateCentroid().
        /// </summary>
        CoordinateVector CalculateCentroidParallel(IList<CoordinateVector> coordList);
    }
}
