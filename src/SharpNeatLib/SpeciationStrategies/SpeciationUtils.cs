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
using SharpNeat.Core;
using System.Diagnostics;

namespace SharpNeat.SpeciationStrategies
{
    /// <summary>
    /// Static helper methods for speciation.
    /// </summary>
    public static class SpeciationUtils
    {
        /// <summary>
        /// Returns true if all of the species are empty.
        /// </summary>
        public static bool TestEmptySpecies<TGenome>(IList<Specie<TGenome>> specieList)
             where TGenome : class, IGenome<TGenome>
        {
            foreach(Specie<TGenome> specie in specieList) 
            {
                if(specie.GenomeList.Count != 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if all species contain at least 1 genome.
        /// </summary>
        public static bool TestPopulatedSpecies<TGenome>(IList<Specie<TGenome>> specieList)
            where TGenome : class, IGenome<TGenome>
        {
            foreach(Specie<TGenome> specie in specieList)
            {
                if(specie.GenomeList.Count == 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Perform an integrity check on the provided species.
        /// Returns true if everything is OK.
        /// </summary>
        public static bool PerformIntegrityCheck<TGenome>(IList<Specie<TGenome>> specieList)
            where TGenome : class, IGenome<TGenome>
        {
            // Check that all species contain at least one genome.
            // Also check that the specieIdx of each genome corresponds to the specie it is within.
            foreach(Specie<TGenome> specie in specieList)
            {
                if(specie.GenomeList.Count == 0) {
                    Debug.WriteLine($"Empty species. SpecieIdx = [{specie.Idx}]. Speciation must allocate at least one genome to each specie.");
                    return false;
                }

                foreach(TGenome genome in specie.GenomeList) 
                {
                    if(genome.SpecieIdx != specie.Idx) 
                    {
                        Debug.WriteLine($"Genome with incorrect specieIdx [{genome.SpecieIdx}]. Parent SpecieIdx = [{specie.Idx}]");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
