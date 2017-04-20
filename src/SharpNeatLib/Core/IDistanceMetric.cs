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
using System.Collections.Generic;

namespace SharpNeat.Core
{
    /// <summary>
    /// Represents a metric for measuring the distance between two genome positions in an encoding space, and thus 
    /// the compatibility of the two genomes with respect to the probability of creating fit offspring.
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
        /// and to exit the method when and if the threshold is passed.
        /// </summary>
        bool TestDistance(CoordinateVector p1, CoordinateVector p2, double threshold);

        /// <summary>
        /// Gets the distance between two positions.
        /// </summary>
        double GetDistance(CoordinateVector p1, CoordinateVector p2);

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
    }
}
