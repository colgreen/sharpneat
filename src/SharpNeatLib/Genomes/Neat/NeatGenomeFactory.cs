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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Redzen.Numerics;
using Redzen.Random;
using Redzen.Random.Double;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Core;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// An IGenomeFactory for NeatGenomes. We use the factory as a means of generating an initial population either
    /// randomly or using a seed genome or genomes.
    /// Subsequently all NeatGenome objects keep a reference to this factory object for convenient access to
    /// NeatGenome parameters and ID generator objects.
    /// </summary>
    public class NeatGenomeFactory : IGenomeFactory<NeatGenome>
    {
        const int __INNOVATION_HISTORY_BUFFER_SIZE = 0x20000;

        /// <summary>NeatGenomeParameters currently in effect.</summary>
        protected NeatGenomeParameters _neatGenomeParamsCurrent;
        readonly NeatGenomeParameters _neatGenomeParamsComplexifying;
        readonly NeatGenomeParameters _neatGenomeParamsSimplifying;
        readonly NeatGenomeStats _stats = new NeatGenomeStats();
        readonly int _inputNeuronCount;
        readonly int _outputNeuronCount;
        readonly UInt32IdGenerator _genomeIdGenerator;
        readonly UInt32IdGenerator _innovationIdGenerator;
        int _searchMode;

        readonly KeyedCircularBuffer<ConnectionEndpointsStruct,uint?> _addedConnectionBuffer 
                = new KeyedCircularBuffer<ConnectionEndpointsStruct,uint?>(__INNOVATION_HISTORY_BUFFER_SIZE);

        readonly KeyedCircularBuffer<uint,AddedNeuronGeneStruct> _addedNeuronBuffer 
                = new KeyedCircularBuffer<uint,AddedNeuronGeneStruct>(__INNOVATION_HISTORY_BUFFER_SIZE);

        /// <summary>Random number generator associated with this factory.</summary>
        protected readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();
        readonly ZigguratGaussianDistribution _gaussianSampler = new ZigguratGaussianDistribution();

        /// <summary>Activation function library associated with this factory.</summary>
        protected readonly IActivationFunctionLibrary _activationFnLibrary;

        #region Constructors [NEAT]

        /// <summary>
        /// Constructs with default NeatGenomeParameters and ID generators initialized to zero.
        /// </summary>
        public NeatGenomeFactory(int inputNeuronCount, int outputNeuronCount)
        {
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;

            _neatGenomeParamsCurrent = new NeatGenomeParameters();
            _neatGenomeParamsComplexifying = _neatGenomeParamsCurrent;
            _neatGenomeParamsSimplifying = NeatGenomeParameters.CreateSimplifyingParameters(_neatGenomeParamsComplexifying);

            _genomeIdGenerator = new UInt32IdGenerator();
            _innovationIdGenerator = new UInt32IdGenerator();

            _activationFnLibrary = DefaultActivationFunctionLibrary.CreateLibraryNeat(_neatGenomeParamsCurrent.ActivationFn);
        }

        /// <summary>
        /// Constructs a NeatGenomeFactory with the provided NeatGenomeParameters and ID generators initialized to zero.
        /// </summary>
        public NeatGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 NeatGenomeParameters neatGenomeParams)
        {
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;
            _activationFnLibrary = DefaultActivationFunctionLibrary.CreateLibraryNeat(neatGenomeParams.ActivationFn);

            _neatGenomeParamsCurrent = neatGenomeParams;
            _neatGenomeParamsComplexifying = _neatGenomeParamsCurrent;
            _neatGenomeParamsSimplifying = NeatGenomeParameters.CreateSimplifyingParameters(_neatGenomeParamsComplexifying);

            _genomeIdGenerator = new UInt32IdGenerator();
            _innovationIdGenerator = new UInt32IdGenerator();
        }

        /// <summary>
        /// Constructs a NeatGenomeFactory with the provided NeatGenomeParameters and ID generators.
        /// </summary>
        public NeatGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 NeatGenomeParameters neatGenomeParams,
                                 UInt32IdGenerator genomeIdGenerator,
                                 UInt32IdGenerator innovationIdGenerator)
        {
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;
            _activationFnLibrary = DefaultActivationFunctionLibrary.CreateLibraryNeat(neatGenomeParams.ActivationFn);

            _neatGenomeParamsCurrent = neatGenomeParams;
            _neatGenomeParamsComplexifying = _neatGenomeParamsCurrent;
            _neatGenomeParamsSimplifying = NeatGenomeParameters.CreateSimplifyingParameters(_neatGenomeParamsComplexifying);

            _genomeIdGenerator = genomeIdGenerator;
            _innovationIdGenerator = innovationIdGenerator;
        }

        #endregion

        #region Constructors [CPPN/HyperNEAT]

        /// <summary>
        /// Constructs with default NeatGenomeParameters, ID generators initialized to zero and the provided
        /// IActivationFunctionLibrary.
        /// This overload required for CPPN support.
        /// </summary>
        public NeatGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 IActivationFunctionLibrary activationFnLibrary)
        {
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;
            _activationFnLibrary = activationFnLibrary;

            _neatGenomeParamsCurrent = new NeatGenomeParameters();
            _neatGenomeParamsComplexifying = _neatGenomeParamsCurrent;
            _neatGenomeParamsSimplifying = NeatGenomeParameters.CreateSimplifyingParameters(_neatGenomeParamsComplexifying);

            _genomeIdGenerator = new UInt32IdGenerator();
            _innovationIdGenerator = new UInt32IdGenerator();
        }

        /// <summary>
        /// Constructs with ID generators initialized to zero and the provided
        /// IActivationFunctionLibrary and NeatGenomeParameters.
        /// This overload required for CPPN support.
        /// </summary>
        public NeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, 
                                 IActivationFunctionLibrary activationFnLibrary,
                                 NeatGenomeParameters neatGenomeParams)
        {
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;
            _activationFnLibrary = activationFnLibrary;

            _neatGenomeParamsCurrent = neatGenomeParams;
            _neatGenomeParamsComplexifying = _neatGenomeParamsCurrent;
            _neatGenomeParamsSimplifying = NeatGenomeParameters.CreateSimplifyingParameters(_neatGenomeParamsComplexifying);

            _genomeIdGenerator = new UInt32IdGenerator();
            _innovationIdGenerator = new UInt32IdGenerator();
        }

        /// <summary>
        /// Constructs with the provided IActivationFunctionLibrary, NeatGenomeParameters and
        /// ID Generators.
        /// This overload required for CPPN support.
        /// </summary>
        public NeatGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 IActivationFunctionLibrary activationFnLibrary,
                                 NeatGenomeParameters neatGenomeParams,
                                 UInt32IdGenerator genomeIdGenerator,
                                 UInt32IdGenerator innovationIdGenerator)
        {
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;
            _activationFnLibrary = activationFnLibrary;

            _neatGenomeParamsCurrent = neatGenomeParams;
            _neatGenomeParamsComplexifying = _neatGenomeParamsCurrent;
            _neatGenomeParamsSimplifying = NeatGenomeParameters.CreateSimplifyingParameters(_neatGenomeParamsComplexifying);

            _genomeIdGenerator = genomeIdGenerator;
            _innovationIdGenerator = innovationIdGenerator;
        }

        #endregion

        #region IGenomeFactory<NeatGenome> Members

        /// <summary>
        /// Gets the factory's genome ID generator.
        /// </summary>
        public UInt32IdGenerator GenomeIdGenerator
        {
            get { return _genomeIdGenerator; }
        }

        /// <summary>
        /// Gets or sets a mode value. This is intended as a means for an evolution algorithm to convey changes
        /// in search mode to genomes, and because the set of modes is specific to each concrete implementation
        /// of an IEvolutionAlgorithm the mode is defined as an integer (rather than an enum[eration]).
        /// E.g. SharpNEAT's implementation of NEAT uses an evolutionary algorithm that alternates between
        /// a complexifying and simplifying mode, in order to do this the algorithm class needs to notify the genomes
        /// of the current mode so that the CreateOffspring() methods are able to generate offspring appropriately - 
        /// e.g. we avoid adding new nodes and connections and increase the rate of deletion mutations when in
        /// simplifying mode.
        /// </summary>
        public int SearchMode 
        { 
            get { return _searchMode; }
            set 
            {
                // Store the mode and switch to a set of NeatGenomeParameters appropriate to the mode.
                // Note. we don't reference the ComplexityRegulationMode enum directly so as not to introduce a
                // compile time dependency between this class and the NeatEvolutionaryAlgorithm - we
                // may wish to use NeatGenome with other algorithm classes in the future.
                _searchMode = value; 
                switch(value)
                {
                    case 0: // ComplexityRegulationMode.Complexifying
                        _neatGenomeParamsCurrent = _neatGenomeParamsComplexifying;
                        break;
                    case 1: // ComplexityRegulationMode.Simplifying
                        _neatGenomeParamsCurrent = _neatGenomeParamsSimplifying;
                        break;
                    default:
                        throw new SharpNeatException("Unexpected SearchMode");
                }
            }
        }

        /// <summary>
        /// Creates a list of randomly initialised genomes.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genomes as their birth generation.</param>
        public List<NeatGenome> CreateGenomeList(int length, uint birthGeneration)
        {   
            List<NeatGenome> genomeList = new List<NeatGenome>(length);
            for(int i=0; i<length; i++)
            {   // We reset the innovation ID to zero so that all created genomes use the same 
                // innovation IDs for matching neurons and connections. This isn't a strict requirement but
                // throughout the SharpNeat code we attempt to use the same innovation ID for like structures
                // to improve the effectiveness of sexual reproduction.
                _innovationIdGenerator.Reset();
                genomeList.Add(CreateGenome(birthGeneration));
            }
            return genomeList;
        }

        /// <summary>
        /// Creates a list of genomes spawned from a seed genome. Spawning uses asexual reproduction.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genomes as their birth generation.</param>
        /// <param name="seedGenome">The seed genome to spawn new genomes from.</param>
        public List<NeatGenome> CreateGenomeList(int length, uint birthGeneration, NeatGenome seedGenome)
        {   
            Debug.Assert(this == seedGenome.GenomeFactory, "seedGenome is from a different genome factory.");

            List<NeatGenome> genomeList = new List<NeatGenome>(length);
            
            // Add an exact copy of the seed to the list.
            NeatGenome newGenome = CreateGenomeCopy(seedGenome, _genomeIdGenerator.NextId, birthGeneration);
            genomeList.Add(newGenome);

            // For the remainder we create mutated offspring from the seed.
            for(int i=1; i<length; i++) {
                genomeList.Add(seedGenome.CreateOffspring(birthGeneration));
            }
            return genomeList;
        }

        /// <summary>
        /// Creates a list of genomes spawned from a list of seed genomes. Spawning uses asexual reproduction and
        /// typically we would simply repeatedly loop over (and spawn from) the seed genomes until we have the 
        /// required number of spawned genomes.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genomes as their birth generation.</param>
        /// <param name="seedGenomeList">A list of seed genomes from which to spawn new genomes from.</param>
        public List<NeatGenome> CreateGenomeList(int length, uint birthGeneration, List<NeatGenome> seedGenomeList)
        {   
            if(seedGenomeList.Count == 0) {
                throw new SharpNeatException("CreateGenomeList() requires at least on seed genome in seedGenomeList.");
            }

            // Create a copy of the list so that we can shuffle the items without modifying the original list.
            seedGenomeList = new List<NeatGenome>(seedGenomeList);
            SortUtils.Shuffle(seedGenomeList, _rng);

            // Make exact copies of seed genomes and insert them into our new genome list.
            List<NeatGenome> genomeList = new List<NeatGenome>(length);
            int idx=0;
            int seedCount = seedGenomeList.Count;
            for(int seedIdx=0; idx<length && seedIdx<seedCount; idx++, seedIdx++)
            {
                // Add an exact copy of the seed to the list.
                NeatGenome newGenome = CreateGenomeCopy(seedGenomeList[seedIdx], _genomeIdGenerator.NextId, birthGeneration);
                genomeList.Add(newGenome);
            }

            // Keep spawning offspring from seed genomes until we have the required number of genomes.
            for(; idx<length;) {
                for(int seedIdx=0; idx<length && seedIdx<seedCount; idx++, seedIdx++) {
                    genomeList.Add(seedGenomeList[seedIdx].CreateOffspring(birthGeneration));
                }
            }
            return genomeList;
        }

        /// <summary>
        /// Creates a single randomly initialised genome. 
        /// A random set of connections are made form the input to the output neurons, the number of 
        /// connections made is based on the NeatGenomeParameters.InitialInterconnectionsProportion
        /// which specifies the proportion of all possible input-output connections to be made in
        /// initial genomes.
        /// 
        /// The connections that are made are allocated innovation IDs in a consistent manner across
        /// the initial population of genomes. To do this we allocate IDs sequentially to all possible 
        /// interconnections and then randomly select some proportion of connections for inclusion in the
        /// genome. In addition, for this scheme to work the innovation ID generator must be reset to zero
        /// prior to each call to CreateGenome(), and a test is made to ensure this is the case.
        /// 
        /// The consistent allocation of innovation IDs ensure that equivalent connections in different 
        /// genomes have the same innovation ID, and although this isn't strictly necessary it is 
        /// required for sexual reproduction to work effectively - like structures are detected by comparing
        /// innovation IDs only.
        /// </summary>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genome as its birth generation.</param>
        public NeatGenome CreateGenome(uint birthGeneration)
        {   
            NeuronGeneList neuronGeneList = new NeuronGeneList(_inputNeuronCount + _outputNeuronCount);
            NeuronGeneList inputNeuronGeneList = new NeuronGeneList(_inputNeuronCount); // includes single bias neuron.
            NeuronGeneList outputNeuronGeneList = new NeuronGeneList(_outputNeuronCount);

            // Create a single bias neuron.
            uint biasNeuronId = _innovationIdGenerator.NextId;
            if(0 != biasNeuronId) 
            {   // The ID generator must be reset before calling this method so that all generated genomes use the
                // same innovation ID for matching neurons and structures.
                throw new SharpNeatException("IdGenerator must be reset before calling CreateGenome(uint)");
            }

            // Note. Genes within nGeneList must always be arranged according to the following layout plan.
            //   Bias - single neuron. Innovation ID = 0
            //   Input neurons.
            //   Output neurons.
            //   Hidden neurons.
            NeuronGene neuronGene = CreateNeuronGene(biasNeuronId, NodeType.Bias);
            inputNeuronGeneList.Add(neuronGene);
            neuronGeneList.Add(neuronGene);

            // Create input neuron genes.
            for(int i=0; i<_inputNeuronCount; i++)
            {
                neuronGene = CreateNeuronGene(_innovationIdGenerator.NextId, NodeType.Input);
                inputNeuronGeneList.Add(neuronGene);
                neuronGeneList.Add(neuronGene);
            }

            // Create output neuron genes. 
            for(int i=0; i<_outputNeuronCount; i++)
            {
                neuronGene = CreateNeuronGene(_innovationIdGenerator.NextId, NodeType.Output);
                outputNeuronGeneList.Add(neuronGene);
                neuronGeneList.Add(neuronGene);
            }

            // Define all possible connections between the input and output neurons (fully interconnected).
            int srcCount = inputNeuronGeneList.Count;
            int tgtCount = outputNeuronGeneList.Count;
            ConnectionDefinition[] connectionDefArr = new ConnectionDefinition[srcCount * tgtCount];

            for(int srcIdx=0, i=0; srcIdx<srcCount; srcIdx++) {
                for(int tgtIdx=0; tgtIdx<tgtCount; tgtIdx++) {
                    connectionDefArr[i++] = new ConnectionDefinition(_innovationIdGenerator.NextId, srcIdx, tgtIdx);
                }
            }

            // Shuffle the array of possible connections.
            SortUtils.Shuffle(connectionDefArr, _rng);

            // Select connection definitions from the head of the list and convert them to real connections.
            // We want some proportion of all possible connections but at least one (Connectionless genomes are not allowed).
            int connectionCount = (int)NumericsUtils.ProbabilisticRound(
                (double)connectionDefArr.Length * _neatGenomeParamsComplexifying.InitialInterconnectionsProportion,
                _rng);
            connectionCount = Math.Max(1, connectionCount);

            // Create the connection gene list and populate it.
            ConnectionGeneList connectionGeneList = new ConnectionGeneList(connectionCount);
            for(int i=0; i<connectionCount; i++)
            {
                ConnectionDefinition def = connectionDefArr[i];
                NeuronGene srcNeuronGene = inputNeuronGeneList[def._sourceNeuronIdx];
                NeuronGene tgtNeuronGene = outputNeuronGeneList[def._targetNeuronIdx];

                ConnectionGene cGene = new ConnectionGene(def._innovationId,
                                                        srcNeuronGene.InnovationId,
                                                        tgtNeuronGene.InnovationId,
                                                        GenerateRandomConnectionWeight());
                connectionGeneList.Add(cGene);

                // Register connection with endpoint neurons.
                srcNeuronGene.TargetNeurons.Add(cGene.TargetNodeId);
                tgtNeuronGene.SourceNeurons.Add(cGene.SourceNodeId);
            }

            // Ensure connections are sorted.
            connectionGeneList.SortByInnovationId();

            // Create and return the completed genome object.
            return CreateGenome(_genomeIdGenerator.NextId, birthGeneration,
                                neuronGeneList, connectionGeneList,
                                _inputNeuronCount, _outputNeuronCount, false);
        }

        /// <summary>
        /// Supports debug/integrity checks. Checks that a given genome object's type is consistent with the genome factory. 
        /// Typically the wrong type of object may occur where factories are sub-typed and not all of the relevant virtual methods are overridden.
        /// Returns true if OK.
        /// </summary>
        public virtual bool CheckGenomeType(NeatGenome genome)
        {
            return genome.GetType() == typeof(NeatGenome);
        }

        #endregion

        #region Properties [NeatGenome Specific]

        /// <summary>
        /// Gets the factory's NeatGenomeParameters currently in effect.
        /// </summary>
        public NeatGenomeParameters NeatGenomeParameters
        {
            get { return _neatGenomeParamsCurrent; }
        }

        /// <summary>
        /// Gets the number of input neurons expressed by the genomes related to this factory.
        /// </summary>
        public int InputNeuronCount
        {
            get { return _inputNeuronCount; }
        }

        /// <summary>
        /// Gets the number of output neurons expressed by the genomes related to this factory.
        /// </summary>
        public int OutputNeuronCount
        {
            get { return _outputNeuronCount; }
        }

        /// <summary>
        /// Gets the factory's activation function library.
        /// </summary>
        public IActivationFunctionLibrary ActivationFnLibrary
        {
            get { return _activationFnLibrary; }
        }

        /// <summary>
        /// Gets the factory's innovation ID generator.
        /// </summary>
        public UInt32IdGenerator InnovationIdGenerator
        {
            get { return _innovationIdGenerator; }
        }

        /// <summary>
        /// Gets the history buffer of added connections. Used when adding new connections to check if an
        /// identical connection has been added to a genome elsewhere in the population. This allows re-use
        /// of the same innovation ID for like connections.
        /// </summary>
        public KeyedCircularBuffer<ConnectionEndpointsStruct,uint?> AddedConnectionBuffer 
        {
            get { return _addedConnectionBuffer; }
        }

        /// <summary>
        /// Gets the history buffer of added neurons. Used when adding new neurons to check if an
        /// identical neuron has been added to a genome elsewhere in the population. This allows re-use
        /// of the same innovation ID for like neurons.
        /// </summary>
        public KeyedCircularBuffer<uint,AddedNeuronGeneStruct> AddedNeuronBuffer
        {
            get { return _addedNeuronBuffer; }
        }

        /// <summary>
        /// Gets a random number generator associated with the factory. 
        /// Note. The provided RNG is not thread safe, if concurrent use is required then sync locks
        /// are necessary or some other RNG mechanism.
        /// </summary>
        public IRandomSource Rng
        {
            get { return _rng; }
        }

        /// <summary>
        /// Gets a Gaussian sampler associated with the factory. 
        /// Note. The provided RNG is not thread safe, if concurrent use is required then sync locks
        /// are necessary or some other RNG mechanism.
        /// </summary>
        public ZigguratGaussianDistribution GaussianSampler
        {
            get { return _gaussianSampler; }
        }

        /// <summary>
        /// Gets some statistics associated with the factory and NEAT genomes that it has spawned.
        /// </summary>
        public NeatGenomeStats Stats
        {
            get { return _stats; }
        }

        #endregion

        #region Public Methods [NeatGenome Specific]

        /// <summary>
        /// Convenient method for obtaining the next genome ID.
        /// </summary>
        public uint NextGenomeId()
        {
            return _genomeIdGenerator.NextId;
        }

        /// <summary>
        /// Convenient method for obtaining the next innovation ID.
        /// </summary>
        public uint NextInnovationId()
        {
            return _innovationIdGenerator.NextId;
        }

        /// <summary>
        /// Convenient method for generating a new random connection weight that conforms to the connection
        /// weight range defined by the NeatGenomeParameters.
        /// </summary>
        public double GenerateRandomConnectionWeight()
        {
            return ((_rng.NextDouble()*2.0) - 1.0) * _neatGenomeParamsCurrent.ConnectionWeightRange;
        }

        /// <summary>
        /// Gets a variable from the Gaussian distribution with the provided mean and standard deviation.
        /// </summary>
        public double SampleGaussianDistribution(double mu, double sigma)
        {
            return _gaussianSampler.Sample(mu, sigma);
        }

        /// <summary>
        /// Create a genome with the provided internal state/definition data/objects.
        /// Overridable method to allow alternative NeatGenome sub-classes to be used.
        /// </summary>
        public virtual NeatGenome CreateGenome(uint id, 
                                               uint birthGeneration,
                                               NeuronGeneList neuronGeneList, 
                                               ConnectionGeneList connectionGeneList, 
                                               int inputNeuronCount,
                                               int outputNeuronCount,
                                               bool rebuildNeuronGeneConnectionInfo)
        {
            return new NeatGenome(this, id, birthGeneration, neuronGeneList, connectionGeneList,
                                  inputNeuronCount, outputNeuronCount, rebuildNeuronGeneConnectionInfo);
        }

        /// <summary>
        /// Create a copy of an existing NeatGenome, substituting in the specified ID and birth generation.
        /// Overridable method to allow alternative NeatGenome sub-classes to be used.
        /// </summary>
        public virtual NeatGenome CreateGenomeCopy(NeatGenome copyFrom, uint id, uint birthGeneration)
        {
            return new NeatGenome(copyFrom, id, birthGeneration);
        }

        /// <summary>
        /// Overridable method to allow alternative NeuronGene sub-classes to be used.
        /// </summary>
        public virtual NeuronGene CreateNeuronGene(uint innovationId, NodeType neuronType)
        {
            // NEAT uses the same activation function at each neuron; Hence we use an activationFnId of 0 here.
            return new NeuronGene(innovationId, neuronType, 0);
        }

        #endregion

        #region Inner Class [ConnectionDefinition]

        struct ConnectionDefinition
        {
            public readonly uint _innovationId;
            public readonly int _sourceNeuronIdx;
            public readonly int _targetNeuronIdx;

            public ConnectionDefinition(uint innovationId, int sourceNeuronIdx, int targetNeuronIdx)
            {
                _innovationId = innovationId;
                _sourceNeuronIdx = sourceNeuronIdx;
                _targetNeuronIdx = targetNeuronIdx;
            }
        }

        #endregion
    }
}
