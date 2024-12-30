﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.Neat;

/// <summary>
/// Static utility methods related to <see cref="NeatPopulation{T}"/>.
/// </summary>
internal class NeatPopulationUtils
{
    /// <summary>
    /// Tests if the ID sequence objects represent an ID higher than any existing ID used by the genomes.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="genomeList">Genome list.</param>
    /// <param name="genomeIdSeq">The next genome ID.</param>
    /// <param name="innovationIdSeq">The next innovation ID.</param>
    /// <returns>
    /// True if all genomes in <paramref name="genomeList"/> have an ID that is less than the next genome ID,
    /// and have connection genes with IDs less than the next innovation ID; otherwise false.
    /// </returns>
    public static bool ValidateIdSequences<TScalar>(
        List<NeatGenome<TScalar>> genomeList,
        Int32Sequence genomeIdSeq,
        Int32Sequence innovationIdSeq)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        GetMaxObservedIds(genomeList, out int maxGenomeId, out int maxInnovationId);

        if(maxGenomeId >= genomeIdSeq.Peek)
            return false;

        if(maxInnovationId >= innovationIdSeq.Peek)
            return false;

        return true;
    }

    /// <summary>
    /// Get the maximum genome ID and innovation ID used in the provided genomes.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="genomeList">Genome list.</param>
    /// <param name="maxGenomeId">Returns the maximum genome ID in the genomes of <paramref name="genomeList"/>.</param>
    /// <param name="maxInnovationId">Returns the maximum innovation ID in the genomes of <paramref name="genomeList"/>.</param>
    public static void GetMaxObservedIds<TScalar>(
        List<NeatGenome<TScalar>> genomeList,
        out int maxGenomeId,
        out int maxInnovationId)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        maxGenomeId = 0;
        maxInnovationId = 0;

        foreach(var genome in genomeList)
        {
            maxGenomeId = Math.Max(maxGenomeId, genome.Id);

            DirectedConnection[] connArr = genome.ConnectionGenes._connArr;
            for(int i=0; i < genome.ConnectionGenes.Length; i++)
            {
                maxInnovationId = Math.Max(maxInnovationId, connArr[i].SourceId);
                maxInnovationId = Math.Max(maxInnovationId, connArr[i].TargetId);
            }
        }
    }
}
