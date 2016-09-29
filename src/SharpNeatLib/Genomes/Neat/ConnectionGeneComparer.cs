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

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Compares the innovation ID of ConnectionGenes.
    /// </summary>
    public class ConnectionGeneComparer : IComparer<ConnectionGene>
    {
        #region IComparer<ConnectionGene> Members

        /// <summary>
        /// Compare the two genes. Returns -1 if x is before y.
        /// </summary>
        public int Compare(ConnectionGene x, ConnectionGene y)
        {
            // Test the most likely cases first.
            if (x.InnovationId < y.InnovationId) {
                return -1;
            } 
            if (x.InnovationId > y.InnovationId) {
                return 1;
            } 
            return 0;
        }

        #endregion
    }
}
