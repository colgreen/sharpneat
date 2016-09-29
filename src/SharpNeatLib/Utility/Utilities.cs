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

namespace SharpNeat.Utility
{
    /// <summary>
    /// General purpose static utility methods.
    /// </summary>
    public static class Utilities
    {
        public static double MagnifyFitnessRange(double x, double metricThreshold, double metricMax, double fitnessThreshold, double fitnessMax)
        {
            if(x < 0.0) {
                x = 0.0;
            }
            else if (x > metricMax) {
                x = metricMax;
            }

            if(x > metricThreshold)
            {   
                return ((x - metricThreshold) / (metricMax - metricThreshold) * (fitnessMax - fitnessThreshold)) + fitnessThreshold;
            }
            // else
            return (x / metricThreshold) * fitnessThreshold;
        }
    }
}
