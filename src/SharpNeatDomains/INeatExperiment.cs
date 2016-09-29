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
using System.Xml;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains
{
    /// <summary>
    /// Interface for classes that aggregate a number of experimental parameters and component creation routines.
    /// INeatExperiment is a layer onto of the core SharpNeatLib that provides a convenient packaging up of various
    /// parts of an experiment into one object that can be plugged in to the SharpNeat GUI.
    /// </summary>
    public interface INeatExperiment
    {
        /// <summary>
        /// Gets the name of the experiment.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets human readable explanatory text for the experiment.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the number of inputs required by the network/black-box that the underlying problem domain is based on.
        /// </summary>
        int InputCount { get; }

        /// <summary>
        /// Gets the number of outputs required by the network/black-box that the underlying problem domain is based on.
        /// </summary>
        int OutputCount { get; }

        /// <summary>
        /// Gets the default population size to use for the experiment.
        /// </summary>
        int DefaultPopulationSize { get; }

        /// <summary>
        /// Gets the NeatEvolutionAlgorithmParameters to be used for the experiment. Parameters on this object can be 
        /// modified. Calls to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in 
        /// at the time of the call.
        /// </summary>
        NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters { get; }

        /// <summary>
        /// Gets the NeatGenomeParameters to be used for the experiment. Parameters on this object can be modified. Calls
        /// to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in at the time of the call.
        /// </summary>
        NeatGenomeParameters NeatGenomeParameters { get; }

        /// <summary>
        /// Initialize the experiment with some optional XML configutation data.
        /// </summary>
        void Initialize(string name, XmlElement xmlConfig);

        /// <summary>
        /// Load a population of genomes from an XmlReader and returns the genomes in a new list.
        /// The genome factory for the genomes can be obtained from any one of the genomes.
        /// </summary>
        List<NeatGenome> LoadPopulation(XmlReader xr);

        /// <summary>
        /// Save a population of genomes to an XmlWriter.
        /// </summary>
        void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList);

        /// <summary>
        /// Create a genome decoder for the experiment.
        /// </summary>
        IGenomeDecoder<NeatGenome,IBlackBox> CreateGenomeDecoder();

        /// <summary>
        /// Create a genome factory for the experiment.
        /// </summary>
        IGenomeFactory<NeatGenome> CreateGenomeFactory();

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// Uses the experiments default population size defined in the experiment's config XML.
        /// </summary>
        NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm();

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a population size parameter that specifies how many genomes to create in an initial randomly
        /// generated population.
        /// </summary>
        NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize);

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a pre-built genome population and their associated/parent genome factory.
        /// </summary>
        NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList);
    }
}
