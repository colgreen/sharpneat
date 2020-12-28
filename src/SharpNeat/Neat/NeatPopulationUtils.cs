/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using Redzen.Structures;
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat
{
    /// <summary>
    /// NeatPopulation static utility methods.
    /// </summary>
    internal class NeatPopulationUtils
    {
        #region Public Static Methods

        public static bool ValidateIdSequences<T>(
            List<NeatGenome<T>> genomeList,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq)
        where T : struct
        {
            GetMaxObservedIds(genomeList, out int maxGenomeId, out int maxInnovationId);

            if(maxGenomeId >= genomeIdSeq.Peek) {
                return false;
            }

            if(maxInnovationId >= innovationIdSeq.Peek) {
                return false;
            }

            return true;
        }

        public static void GetMaxObservedIds<T>(
            List<NeatGenome<T>> genomeList,
            out int maxGenomeId,
            out int maxInnovationId)
        where T : struct
        {
            maxGenomeId = 0;
            maxInnovationId = 0;

            foreach(var genome in genomeList)
            {
                maxGenomeId = Math.Max(maxGenomeId, genome.Id);

                DirectedConnection[] connArr =  genome.ConnectionGenes._connArr;
                for(int i=0; i < genome.ConnectionGenes.Length; i++)
                {
                    maxInnovationId = Math.Max(maxInnovationId, connArr[i].SourceId);
                    maxInnovationId = Math.Max(maxInnovationId, connArr[i].TargetId);
                }
            }
        }

        #endregion
    }
}
