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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Core;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// A genome class for Neuro Evolution of Augemting Topologies (NEAT).
    /// 
    /// Note that neuron genes must be arranged according to the following layout plan.
    ///      Bias - single neuron. Innovation ID = 0
    ///      Input neurons.
    ///      Output neurons.
    ///      Hidden neurons.
    /// 
    /// This allows us to add and remove hidden neurons without affecting the position of the bias,
    /// input and output neurons; This is convenient because bias and input and output neurons are
    /// fixed, they cannot be added to or removed and so remain constant throughout a given run. In fact they
    /// are only stored in the same list as hidden nodes as an efficiency measure when producing offspring 
    /// and decoding genomes, otherwise it would probably make sense to store them in readonly lists.
    /// </summary>
    public class NeatGenome : IGenome<NeatGenome>, INetworkDefinition
    {
        #region Instance Variables

        NeatGenomeFactory _genomeFactory;
        readonly uint _id;
        int _specieIdx;
        readonly uint _birthGeneration;
        EvaluationInfo _evalInfo;
        CoordinateVector _position;
        object _cachedPhenome;

        // We ensure that the connectionGenes are sorted by innovation ID at all times. This allows significant optimisations
        // to be made in crossover and decoding routines.
        // Neuron genes must also be arranged according to the following layout plan.
        //      Bias - single neuron. Innovation ID = 0
        //      Input neurons.
        //      Output neurons.
        //      Hidden neurons.
        readonly NeuronGeneList _neuronGeneList;
        readonly ConnectionGeneList _connectionGeneList;

        // For efficiency we store the number of input and output neurons. These two quantities do not change
        // throughout the life of a genome. Note that inputNeuronCount does NOT include the bias neuron; Use
        // inputAndBiasNeuronCount.
        readonly int _inputNeuronCount;
        readonly int _inputAndBiasNeuronCount;
        readonly int _outputNeuronCount;
        readonly int _inputBiasOutputNeuronCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene lists.
        /// </summary>
        public NeatGenome(NeatGenomeFactory genomeFactory,
                          uint id, 
                          uint birthGeneration,
                          NeuronGeneList neuronGeneList, 
                          ConnectionGeneList connectionGeneList, 
                          int inputNeuronCount,
                          int outputNeuronCount)
        {
            _genomeFactory = genomeFactory;
            _id = id;
            _birthGeneration = birthGeneration;
            _neuronGeneList = neuronGeneList;
            _connectionGeneList = connectionGeneList;
            _inputNeuronCount = inputNeuronCount;

            // Precalculate some often used values.
            _inputAndBiasNeuronCount = inputNeuronCount+1;
            _outputNeuronCount = outputNeuronCount;
            _inputBiasOutputNeuronCount = _inputAndBiasNeuronCount + outputNeuronCount;

            // If we have a factory then create the evaluation info object now. Otherwise wait until the factory
            // is provided through the property setter.
            if(null != _genomeFactory) {
                _evalInfo = new EvaluationInfo(genomeFactory.NeatGenomeParameters.FitnessHistoryLength);
            }
            
            Debug.Assert(PerformIntegrityCheck());
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public NeatGenome(NeatGenome copyFrom, uint id, uint birthGeneration)
        {
            _genomeFactory = copyFrom._genomeFactory;
            _id = id;
            _birthGeneration = birthGeneration;

            // These copy constructors make clones of the genes rather than copies of the object references.
            _neuronGeneList = new NeuronGeneList(copyFrom._neuronGeneList);
            _connectionGeneList = new ConnectionGeneList(copyFrom._connectionGeneList);
            
            // Copy precalculated values.
            _inputNeuronCount = copyFrom._inputNeuronCount;
            _inputAndBiasNeuronCount = copyFrom._inputAndBiasNeuronCount;
            _outputNeuronCount = copyFrom._outputNeuronCount;
            _inputBiasOutputNeuronCount = copyFrom._inputBiasOutputNeuronCount;

            _evalInfo = new EvaluationInfo(copyFrom.EvaluationInfo.FitnessHistoryLength);

            Debug.Assert(PerformIntegrityCheck());
        }

        #endregion

        #region IGenome<NeatGenome> Members

        /// <summary>
        /// Gets the genome's unique ID. IDs are unique across all genomes created from a single 
        /// IGenomeFactory and all ancestor genomes spawned from those genomes.
        /// </summary>
        public uint Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets or sets a specie index. This is the index of the species that the genome is in.
        /// Implementing this is required only when using evolution algorithms that speciate genomes.
        /// </summary>
        public int SpecieIdx 
        { 
            get { return _specieIdx; }
            set { _specieIdx = value; }
        }

        /// <summary>
        /// Gets the generation that this genome was born/created in. Used to track genome age.
        /// </summary>
        public uint BirthGeneration
        {
            get { return _birthGeneration; }
        }

        /// <summary>
        /// Gets evaluation information for the genome, including its fitness.
        /// </summary>
        public EvaluationInfo EvaluationInfo
        {
            get { return _evalInfo; }
        }

        /// <summary>
        /// Gets a value that indicates the magnitude of a genome's complexity. 
        /// For a NeatGenome we return the number of connection genes since a neural network's
        /// complexity is approximately proportional to the number of connections - the number of
        /// neurons is less important and can be viewed as being a limit on the possible number of
        /// connections.
        /// </summary>
        public double Complexity 
        { 
            get { return _connectionGeneList.Count; }
        }

        /// <summary>
        /// Gets a coordinate that represents the genome's position in the search space (also known
        /// as the genetic encoding space). This allows speciation/clustering algorithms to operate on
        /// an abstract cordinate data type rather than being coded against specific IGenome types.
        /// </summary>
        public CoordinateVector Position 
        { 
            get
            {
                if(null == _position)
                {
                    // Consider each connection gene as a dimension where the innovation ID is the
                    // dimension's ID and the weight is the position within that dimension.
                    // The coordinate elements in the resulting array must be sorted by innovation/dimension ID,
                    // this requirement is met by the connection gene list also requiring to be sorted at all times.
                    ConnectionGeneList list = _connectionGeneList;

                    int count = list.Count;
                    KeyValuePair<ulong,double>[] coordElemArray = new KeyValuePair<ulong,double>[count];

                    for(int i=0; i<count; i++) {
                        coordElemArray[i] = new KeyValuePair<ulong,double>(list[i].InnovationId, list[i].Weight);
                    }
                    _position = new CoordinateVector(coordElemArray);
                }
                return _position;
            }
        }

        /// <summary>
        /// Gets or sets a cached phenome obtained from decodign the genome.
        /// Genomes are typically decoded to Phenomes for evaluation. This property allows decoders to 
        /// cache the phenome in order to avoid decoding on each re-evaluation; However, this is optional.
        /// The phenome in un-typed to prevent the class framework from becoming overly complex.
        /// </summary>
        public object CachedPhenome 
        { 
            get { return _cachedPhenome; }
            set { _cachedPhenome = value; }
        }

        /// <summary>
        /// Asexual reproduction.
        /// </summary>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genome at its birth generation.</param>
        public NeatGenome CreateOffspring(uint birthGeneration)
        {
            // Make a new genome that is a copy of this one but with a new genome ID.
            NeatGenome offspring = _genomeFactory.CreateGenomeCopy(this, _genomeFactory.NextGenomeId(), birthGeneration);

            // Mutate the new genome.
            offspring.Mutate();
            return offspring;
        }

        /// <summary>
        /// Sexual reproduction.
        /// </summary>
        /// <param name="parent">The other parent genome (mates with the current genome).</param>
        /// <param name="birthGeneration">The current evolution algorithm generation. 
        /// Assigned to the new genome at its birth generation.</param>
        public NeatGenome CreateOffspring(NeatGenome parent, uint birthGeneration)
        {
            CorrelationResults correlationResults = CorrelateConnectionGeneLists(_connectionGeneList, parent._connectionGeneList);
            Debug.Assert(correlationResults.PerformIntegrityCheck(), "CorrelationResults failed integrity check.");

            // Construct a ConnectionGeneListBuilder with the capacity set the the maximum number of connections that
            // could be added to it (all connection genes from both parents). This eliminates the possiblity of having to
            // re-allocate list memory, improving performance at the cost of a little additional allocated memory on average.
            ConnectionGeneListBuilder connectionListBuilder = new ConnectionGeneListBuilder(_connectionGeneList.Count +
                                                                                            parent._connectionGeneList.Count);

            // Pre-register all of the fixed neurons (bias, inputs and outputs) with the ConnectionGeneListBuilder's
            // neuron ID dictionary. We do this so that we can use the dictionary later on as a complete list of
            // all neuron IDs required by the offspring genome - if we didn't do this we might miss some of the fixed neurons
            // that happen to not be connected to or from.
            SortedDictionary<uint,uint> neuronIdDictionary = connectionListBuilder.NeuronIdDictionary;
            for(int i=0; i<_inputBiasOutputNeuronCount; i++) {
                neuronIdDictionary.Add(_neuronGeneList[i].InnovationId, _neuronGeneList[i].InnovationId);
            }

            // A variable that stores which parent is fittest, 1 or 2. 0 if both are equal. We pre-calculate this value because this
            // fitness test needs to be done in in subsequent sub-routine calls for each connection gene.
            int fitSwitch;
            if(_evalInfo.Fitness > parent._evalInfo.Fitness) {
                fitSwitch = 1;
            }
            else if(_evalInfo.Fitness < parent._evalInfo.Fitness) {
                fitSwitch = 2;
            }
            else
            {   // Select one of the parents at random to be the 'master' genome during crossover.
                fitSwitch = (_genomeFactory.Rng.NextDouble() < 0.5) ? 1 : 2;
            }

            // TODO: Reconsider this approach.
            // Pre-calcualte a flag that inidates if excess and disjoint genes should be copied into the offspring genome.
            // Excess and disjoint genes are either copied alltogether or none at all.
            bool combineDisjointExcessFlag = _genomeFactory.Rng.NextDouble() < _genomeFactory.NeatGenomeParameters.DisjointExcessGenesRecombinedProbability;

            // Loop through the items within the CorrelationResults, processing each one in turn.
            // Where we have a match between parents we select which parent's copy to use probabilistically with even chance.
            // Otherwise we refer to fitSwitch to select if the gene is used or not.
            // All accepted genes are accumulated in the ConnectionListBuilder.
            int itemCount = correlationResults.CorrelationItemList.Count;
            for(int i=0; i<itemCount; i++) {
                CreateOffspring_Sexual_ProcessCorrelationItem(connectionListBuilder,
                                                              correlationResults.CorrelationItemList[i],
                                                              fitSwitch, combineDisjointExcessFlag);
            }

            // Build a list of neuron genes for the new genome. Conveniently connectionListBuilder.NeuronIdDictionary has
            // been tracking all neuron IDs referred to by connection endpoints, and was also pre-loaded with all of the 
            // fixed (bias, input and output) neurons. Thus we already know how many and which neurons our new genome will
            // have - *and* they are already sorted by innovation ID as required by NeuronGeneList.
            NeuronGeneList neuronGeneList = new NeuronGeneList(connectionListBuilder.NeuronIdDictionary.Count);
            IEnumerator<KeyValuePair<uint,uint>> neuronDictEnumerator = neuronIdDictionary.GetEnumerator();

            // Add single bias neuron.
            neuronDictEnumerator.MoveNext();
            neuronGeneList.Add(_genomeFactory.CreateNeuronGene(neuronDictEnumerator.Current.Key, NodeType.Bias));

            // Add input neuron genes.
            // MoveNext() doesn't need to be in the while tests. We know the bias, input, output neuron genes are available.
            int inputIdx=0;
            while(inputIdx++ < _inputNeuronCount) {   
                neuronDictEnumerator.MoveNext();
                neuronGeneList.Add(_genomeFactory.CreateNeuronGene(neuronDictEnumerator.Current.Key, NodeType.Input));
            }
            
            // Add output neuron genes.
            int outputIdx=0;
            while(outputIdx++ < _outputNeuronCount) {
                neuronDictEnumerator.MoveNext();
                neuronGeneList.Add(_genomeFactory.CreateNeuronGene(neuronDictEnumerator.Current.Key, NodeType.Output));
            }

            // All remaining IDs in neuronDictEnumerator (if any) are hidden nodes.
            while(neuronDictEnumerator.MoveNext()) {
                neuronGeneList.Add(_genomeFactory.CreateNeuronGene(neuronDictEnumerator.Current.Key, NodeType.Hidden));
            }

            // Note that connectionListBuilder.ConnectionGeneList is already sorted by connection gene innovation ID 
            // because it was generated by passing over the correlation items generated by CorrelateConnectionGeneLists()
            // - which returns correlation items in order.
            return _genomeFactory.CreateGenome(_genomeFactory.NextGenomeId(),
                                               birthGeneration,
                                               neuronGeneList,
                                               connectionListBuilder.ConnectionGeneList,
                                               _inputNeuronCount,
                                               _outputNeuronCount);
        }

        #endregion

        #region Properties [NEAT Genome Specific]

        /// <summary>
        /// Gets or sets the NeatGenomeFactory associated with the genome. A reference to the factory is 
        /// passed to spawned genomes, this allows all genomes within a population to have access to common 
        /// data such as NeatGenomeParameters and an ID generator.
        /// Setting the genome factory after construction is allowed in order to resolve chicken-and-egg
        /// scenarios when loading genomes from storage.
        /// </summary>
        public NeatGenomeFactory GenomeFactory
        {
            get { return _genomeFactory; }
            set 
            {
                if(null != _genomeFactory) {
                    throw new SharpNeatException("NeatGenome already has an assigned GenomeFactory.");
                }
                _genomeFactory = value;
                _evalInfo = new EvaluationInfo(_genomeFactory.NeatGenomeParameters.FitnessHistoryLength);
            }
        }
        
        /// <summary>
        /// Gets the genome's list of neuron genes.
        /// </summary>
        public NeuronGeneList NeuronGeneList
        {
            get { return _neuronGeneList; } 
        }

        /// <summary>
        /// Gets the genome's list of connection genes.
        /// </summary>
        public ConnectionGeneList ConnectionGeneList
        {
            get { return _connectionGeneList; }
        }

        /// <summary>
        /// Gets the number of input neurons represented by the genome.
        /// </summary>
        public int InputNeuronCount
        {
            get { return _inputNeuronCount; }
        }

        /// <summary>
        /// Gets the number of input and bias neurons represented by the genome.
        /// </summary>
        public int InputAndBiasNeuronCount
        {
            get { return _inputAndBiasNeuronCount; }
        }

        /// <summary>
        /// Gets the number of output neurons represented by the genome.
        /// </summary>
        public int OutputNeuronCount
        {
            get { return _outputNeuronCount; }
        }

        /// <summary>
        /// Gets the number total number of neurons represented by the genome.
        /// </summary>
        public int InputBiasOutputNeuronCount
        {
            get { return _inputAndBiasNeuronCount; }
        }

        #endregion

        #region Private Methods [Asexual Reproduction / Mutation]

        private void Mutate()
        {
            // If we have fewer than two conenctions then use an alternative RouletteWheelLayout that avoids 
            // destructive mutations. This prevents the creation of genomes with no connections.
            RouletteWheelLayout rwl = (_connectionGeneList.Count < 2) ?
                  _genomeFactory.NeatGenomeParameters.RouletteWheelLayoutNonDestructive 
                : _genomeFactory.NeatGenomeParameters.RouletteWheelLayout;

            // Roulette wheel selection of mutation type.
            int outcome = RouletteWheel.SingleThrow(rwl, _genomeFactory.Rng);
            switch(outcome)
            {
                case 0:
                    Mutate_ConnectionWeights();
                    break;
                case 1:
                    Mutate_AddNode();
                    break;
                case 2:
                    Mutate_AddConnection();
                    break;
                case 3:
                    Mutate_DeleteConnection();
                    break;
                case 4:
                    Mutate_DeleteSimpleNeuronStructure();
                    break;
            }

            Debug.Assert(PerformIntegrityCheck());
        }

        /// <summary>
        /// Add a new node to the Genome. We do this by removing a connection at random and inserting a new
        /// node and two new connections that make the same circuit as the original connection, that is, we split an 
        /// existing connection. This way the new node is integrated into the network from the outset.
        /// </summary>
        private void Mutate_AddNode()
        {
            if(0 == _connectionGeneList.Count) 
            {   // Nodes are added by splitting an existing connection into two and placing a new node
                // between the two new connections. Since we don't have an connections to split we
                // call on to Mutate_AddConnection().
                Mutate_AddConnection();
                return;
            }

            // Select a connection at random, keep a ref to it and delete it from the genome.
            int connectionToReplaceIdx = _genomeFactory.Rng.Next(_connectionGeneList.Count);
            ConnectionGene connectionToReplace = _connectionGeneList[connectionToReplaceIdx];
            _connectionGeneList.RemoveAt(connectionToReplaceIdx);

            // Get IDs for the two new connections and a single neuron. This call will check the history 
            // buffer (AddedNeuronBuffer) for matching structures from previously added neurons (for the search as
            // a whole, not just on this genome).
            AddedNeuronGeneStruct idStruct;
            bool reusedIds = Mutate_AddNode_GetIDs(connectionToReplace.InnovationId, out idStruct);

            // Replace connection with two new connections and a new neuron. The first connection uses the weight
            // from the replaced connection (so it's functionally the same connection, but the ID is new). Ideally
            // we want the functionality of the new structure to match as closely as possible the replaced connection,
            // but that depends on the neuron activation function. As a cheap/quick approximation we make the second 
            // connection's weight full strength (_genomeFactory.NeatGenomeParameters.ConnectionWeightRange). This
            // maps the range 0..1 being output from the new neuron to something close to 0.5..1.0 when using a unipolar
            // sigmoid (depending on exact sigmoid function in use). Weaker weights reduce that range, ultimately a zero
            // weight always gives an output of 0.5 for a unipolar sigmoid.
            NeuronGene newNeuronGene = _genomeFactory.CreateNeuronGene(idStruct.AddedNeuronId, NodeType.Hidden);
            ConnectionGene newConnectionGene1 = new ConnectionGene(idStruct.AddedInputConnectionId,
                                                                   connectionToReplace.SourceNodeId,
                                                                   idStruct.AddedNeuronId,
                                                                   connectionToReplace.Weight);

            ConnectionGene newConnectionGene2 = new ConnectionGene(idStruct.AddedOutputConnectionId,
                                                                   idStruct.AddedNeuronId,
                                                                   connectionToReplace.TargetNodeId,
                                                                   _genomeFactory.NeatGenomeParameters.ConnectionWeightRange);

            // If we are re-using innovation numbers from elsewhere in the population they are likely to have
            // lower values than other genes in the current genome. Therefore we need to be careful to ensure the 
            // genes lists remain sorted by innovation ID. The most efficient means of doing this is to insert the new 
            // genes into the correct location (as opposed to adding them to the list ends and re-sorting the lists).
            if(reusedIds) 
            {
                _neuronGeneList.InsertIntoPosition(newNeuronGene);
                _connectionGeneList.InsertIntoPosition(newConnectionGene1);
                _connectionGeneList.InsertIntoPosition(newConnectionGene2);
            }
            else
            {   // The genes have new innovation IDs - so just add them to the ends of the gene lists.
                _neuronGeneList.Add(newNeuronGene);
                _connectionGeneList.Add(newConnectionGene1);
                _connectionGeneList.Add(newConnectionGene2);
            }
            _genomeFactory.Stats._mutationCountAddNode++;
        }

        /// <summary>
        /// Gets innovation IDs for a new neuron and two connections. We add neurons by splitting an existing connection, here we
        /// check if the connection to be split has previously been split and if so attemopt to re-use the IDs assigned during that
        /// split.
        /// </summary>
        /// <param name="connectionToReplaceId">ID of the connection that is being replaced.</param>
        /// <param name="idStruct">Conveys the required IDs back to the caller.</param>
        /// <returns>Returns true if the IDs are existing IDs from a matching structure in the history buffer (AddedNeuronBuffer).</returns>
        private bool Mutate_AddNode_GetIDs(uint connectionToReplaceId, out AddedNeuronGeneStruct idStruct)
        {
            bool registerNewStruct = false;
            if(_genomeFactory.AddedNeuronBuffer.TryGetValue(connectionToReplaceId, out idStruct))
            {   
                // Found existing matching structure.
                // However we can only re-use the IDs from that structrue if they aren't already present in the current genome;
                // this is possible because genes can be acquired from other genomes via sexual reproduction.
                // Therefore we only re-use IDs if we can re-use all three together, otherwise we aren't assigning the IDs to matching
                // structures throughout the population, which is the reason for ID re-use.
                if(_neuronGeneList.BinarySearch(idStruct.AddedNeuronId) == -1
                && _connectionGeneList.BinarySearch(idStruct.AddedInputConnectionId) == -1
                && _connectionGeneList.BinarySearch(idStruct.AddedOutputConnectionId) == -1)             
                {
                    // Return true to indicate re-use of existing IDs.
                    return true;
                }
            }
            else
            {   // ConnectionID not found. This connectionID has not been split to add a neuron in the past, or at least as far
                // back as the history buffer goes. Therefore we register the structure with the history buffer.
                registerNewStruct = true;
            }

            // No pre-existing matching structure or if there is we already have some of its genes (from sexual reproduction).
            // Generate new IDs for this structure.
            idStruct = new AddedNeuronGeneStruct(_genomeFactory.InnovationIdGenerator);

            // If the connectionToReplaceId was not found (above) then we register it along with the new structure 
            // it is being replaced with.
            if(registerNewStruct) {   
                _genomeFactory.AddedNeuronBuffer.Enqueue(connectionToReplaceId, idStruct);
            }
            return false;
        }

        private void Mutate_AddConnection()
        {
            // We attempt to find a pair of neurons with no connection between them in one or both directions. We disallow multiple
            // connections between the same two neurons going in the same direction, but we *do* allow connections going 
            // in opposite directions (one connection each way). We also allow a neuron to have a single recurrent connection, 
            // that is, a connection that has the same neuron as its source and target neuron.

            // ENHANCEMENT: Test connection 'density' and use alternative connection selection method if above some threshold.
            // Because input/output neurons are fixed (cannot be added to or deleted) and always present (any domain that 
            // doesn't require input/outputs is a bit nonsensical) we always have candidate pairs of neurons to consider
            // adding connections to, but if all neurons are already fully interconnected then we should handle this case
            // where there are no possible neuron pairs to add a connection to. To handle this we use a simple strategy
            // of testing the suitability of randomly selected pairs and after some number of failed tests we bail out
            // of the routine and perform weight mutation as a last resort - so that we did at least some form of mutation on 
            // the genome.
            if(_neuronGeneList.Count < 2) 
            {   // No possibility of adding a connection, perform weight mutation as a last resort.
                Mutate_ConnectionWeights();
                return;
            }

            // TODO: Try to improve chance of finding a candidate connection to make.
            // We have at least 2 neurons, so we have a chance at creating a connection.
            int neuronCount = _neuronGeneList.Count;
            int hiddenOutputNeuronCount = neuronCount - _inputAndBiasNeuronCount;

            for(int attempts=0; attempts<5; attempts++)
            {
                // Select candidate source and target neurons. Any neuron can be used as the source. Input neurons 
                // should not be used as a target           
                // Source neuron can by any neuron. Target neuron is any neuron except input neurons.
                int srcNeuronIdx = _genomeFactory.Rng.Next(neuronCount);
                int tgtNeuronIdx = _inputAndBiasNeuronCount + _genomeFactory.Rng.Next(hiddenOutputNeuronCount);

                NeuronGene sourceNeuron = _neuronGeneList[srcNeuronIdx];            
                NeuronGene targetNeuron = _neuronGeneList[tgtNeuronIdx];

                // Check if a connection already exists between these two neurons.
                // Note. we allow a neuron to connect back on itself, thus the source and target neuron can be the same.
                uint sourceId = sourceNeuron.InnovationId;
                uint targetId = targetNeuron.InnovationId;

                if(!TestForExistingConnection(sourceId, targetId))
                {
                    // Check if a matching mutation has already occured on another genome. 
                    // If so then re-use the connection ID.
                    ConnectionEndpointsStruct connectionKey = new ConnectionEndpointsStruct(sourceId, targetId);
                    uint? existingConnectionId;
                    if(_genomeFactory.AddedConnectionBuffer.TryGetValue(connectionKey, out existingConnectionId))
                    {   
                        // Create a new connection, re-using the ID from existingConnectionId, and add it to the Genome.
                        ConnectionGene newConnectionGene = new ConnectionGene(existingConnectionId.Value,
                                                                              sourceId, targetId,
                                                                              _genomeFactory.GenerateRandomConnectionWeight());

                        // Add the new gene to this genome. We are re-using an ID so we must ensure the connection gene is
                        // inserted into the correct position (sorted by innovation ID). The ID is most likely an older one
                        // with a lower value than recent IDs, and thus it probably doesn't belong on the end of the list.
                        _connectionGeneList.InsertIntoPosition(newConnectionGene);
                    }
                    else
                    {   
                        // Create a new connection with a new ID and add it to the Genome.
                        ConnectionGene newConnectionGene = new ConnectionGene(_genomeFactory.NextInnovationId(),
                                                                              sourceId, targetId,
                                                                              _genomeFactory.GenerateRandomConnectionWeight());

                        // Add the new gene to this genome. We have a new ID so we can safely append the gene to the end 
                        // of the list without risk of breaking the innovation ID sort order.
                        _connectionGeneList.Add(newConnectionGene);

                        // Register the new connection with the added connection history buffer.
                        _genomeFactory.AddedConnectionBuffer.Enqueue(new ConnectionEndpointsStruct(sourceId, targetId),
                                                                     newConnectionGene.InnovationId);
                    }
                    _genomeFactory.Stats._mutationCountAddConnection++;
                    return;
                }
            }

            // We couldn't find a valid connection to create. 
            // Rather than do nothing we invoke connection weight mutation.
            Mutate_ConnectionWeights();
        }

        private void Mutate_DeleteConnection()
        {
            if(_connectionGeneList.Count < 2) 
            {   // Either no connections to delete or only one. Mutate weights instead.
                Mutate_ConnectionWeights();
                return;
            }

            // Select a connection at random.
            int connectionToDeleteIdx = _genomeFactory.Rng.Next(_connectionGeneList.Count);
            ConnectionGene connectionToDelete = _connectionGeneList[connectionToDeleteIdx];

            // Delete the connection.
            _connectionGeneList.RemoveAt(connectionToDeleteIdx);

            // Remove any neurons that may have been left floating.
            // Check source neuron connectedness.
            int idx = _neuronGeneList.BinarySearch(connectionToDelete.SourceNodeId);
            if(IsNeuronRedundant(idx)) {
                _neuronGeneList.RemoveAt(idx);
            }

            // Check target neuron connectedness. Note that if the connection is recurrent then both
            // of its end points are connected to the same neuron. If this is the case then we avoid
            // checking the same neuron again.
            if(connectionToDelete.SourceNodeId != connectionToDelete.TargetNodeId) 
            {
                idx = _neuronGeneList.BinarySearch(connectionToDelete.TargetNodeId);
                if(IsNeuronRedundant(idx)) {
                    _neuronGeneList.RemoveAt(idx);
                }
            }
            _genomeFactory.Stats._mutationCountDeleteConnection++;
        }

        // TODO: Test this once phased search is implemented. It doesn't really get used without some structure pruning.
        /// <summary>
        /// We define a simple neuron structure as a neuron that has a single incoming connection and one or more 
        /// outgoing connections, or one or more incoming connections and one outgoing connection.
        /// We can easily eliminate such a neuron and maintauin connectivity between remainign neurons by changing 
        /// the common endpoint of the multiple connections to that of the single connection's.
        /// If the neuron's non-linearity was not being used then such a mutation is a simplification of the network
        /// structure that may not adversly affect its functionality despite having one less neuron.
        /// </summary>
        private void Mutate_DeleteSimpleNeuronStructure()
        {
            if(_neuronGeneList.Count == 0 || _connectionGeneList.Count < 2) 
            {   // Mutate weights instead.
                Mutate_ConnectionWeights();
                return;
            }

            // Build a dictionary of hidden neuron IDs and their associated input and output connections.
            // This helps us identify the simple structures.
            Dictionary<uint,NeuronConnectionsLookup> tmpDict = BuildHiddenNeuronConnectionsLookupDictionary();

            // Build a list of candidate simple neurons to choose from.
            List<NeuronConnectionsLookup> simpleNeuronList = new List<NeuronConnectionsLookup>(tmpDict.Count);
            foreach(NeuronConnectionsLookup lookup in tmpDict.Values)
            {   
                // We can also handle neurons with 0 connections on one or both sides.
                if((lookup._inputConnectionList.Count < 2) || (lookup._outputConnectionList.Count < 2)) {
                    simpleNeuronList.Add(lookup);
                }
            }

            if(0 == simpleNeuronList.Count)
            {   // No candidate neurons. As a fallback we attempt to delete a connection.
                Mutate_DeleteConnection();
                return;
            }

            // Pick a simple neuron at random and remove it.
            NeuronConnectionsLookup neuronLookup = simpleNeuronList[_genomeFactory.Rng.Next(simpleNeuronList.Count)];
            RemoveSimpleNeuron(neuronLookup);
            _genomeFactory.Stats._mutationCountDeleteSimpleNeuron++;
        }

        private void RemoveSimpleNeuron(NeuronConnectionsLookup lookup)
        {
            // Delete all existing connections that attach to the neuron that is to be removed. We do this
            // before adding new connections that are substitutes for the deleted connections; This means we 
            // don't have both the old and new conenctions in connectionList at the same time, which would temporarily
            // increase the length of the list beyond its current capacity, therefore requiring an expensive
            // increase in capacity.
            
            // ENHANCEMENT: Connection removals can be performed more efficiently with List<>.RemoveAll(Predicate<T>)...
            // ...This prevents us having to shuffle the list items to fill the gaps on each individual remove.
            foreach(ConnectionGene incomingConnection in lookup._inputConnectionList) {
                _connectionGeneList.Remove(incomingConnection.InnovationId);
            }

            foreach(ConnectionGene outgoingConnection in lookup._outputConnectionList)
            {   // Filter out recurrent connections - they will have already been 
                // deleted in the loop through incoming connections (above).
                if(outgoingConnection.TargetNodeId != lookup._neuronId) {
                    _connectionGeneList.Remove(outgoingConnection.InnovationId);
                }
            }

            // Create new connections that connect all of the incoming and outgoing neurons
            // that currently exist for the simple neuron (if any).
            foreach(ConnectionGene inputConnection in lookup._inputConnectionList)
            {
                foreach(ConnectionGene outputConnection in lookup._outputConnectionList)
                {
                    uint sourceId = inputConnection.InnovationId;
                    uint targetId = outputConnection.InnovationId;

                    if((sourceId==targetId) || TestForExistingConnection(sourceId, targetId))
                    {   // Connection is recurrent or already already exists.
                        // Note that recurrent connections are handled by TestForExistingConnection() but we include the
                        // explcit test because it is fast and also to be clear of our intentions - recurrent connections
                        // are discarded with the neuron and no substituting connections are made.
                        continue;
                    }

                    // Check if a matching mutation has already occured on another genome. 
                    // If so then re-use the connection ID.
                    ConnectionEndpointsStruct connectionKey = new ConnectionEndpointsStruct(sourceId, targetId);
                    uint? existingConnectionId;
                    if(_genomeFactory.AddedConnectionBuffer.TryGetValue(connectionKey, out existingConnectionId))
                    {
                        // Create a new connection re-using existingConnectionId, and add it to the Genome.
                        // Also re-use a weight from one of the connections we are replacing.
                        // ENHANCEMENT: Is there a better weight we could use here?
                        ConnectionGene newConnectionGene = new ConnectionGene(existingConnectionId.Value,
                                                                              sourceId, targetId,
                                                                              outputConnection.Weight);

                        // Add the new gene to this genome. We are re-using an ID so we must ensure the connection gene is
                        // inserted into the correct position (sorted by innovation ID).
                        _connectionGeneList.InsertIntoPosition(newConnectionGene);
                    }
                    else
                    {
                        // Create a new connection with a new ID and add it to the Genome.
                        // Also re-use a weight from one of the connections we are replacing.
                        // ENHANCEMENT: Is there a better weight we could use here?
                        ConnectionGene newConnectionGene = new ConnectionGene(_genomeFactory.NextInnovationId(),
                                                                              sourceId, targetId,
                                                                              outputConnection.Weight);

                        // Add the new gene to this genome. We have a new ID so we can safely append the gene to the end 
                        // of the list without risk of breaking the innovation ID sort order.
                        _connectionGeneList.Add(newConnectionGene);

                        // Register the new connection with the added connection history buffer.
                        _genomeFactory.AddedConnectionBuffer.Enqueue(new ConnectionEndpointsStruct(sourceId, targetId),
                                                                     newConnectionGene.InnovationId);
                    }
                }
            }

            // Delete the simple neuron - it no longer has any connections to or from it.
            _neuronGeneList.Remove(lookup._neuronId);
        }

        private void Mutate_ConnectionWeights()
        {
            // Determine the type of weight mutation to perform.
            ConnectionMutationInfo mutationInfo = _genomeFactory.NeatGenomeParameters.ConnectionMutationInfoList.GetRandomItem(_genomeFactory.Rng);
    
            // Get a delegate that performs the mutation specified by mutationInfo. The alternative is to use a switch statement
            // test purturbance type on each connection weight mutation - which creates a lot of unnecessary branch instructions.
            MutateWeightMethod mutateWeigthMethod = Mutate_ConnectionWeights_GetMutateWeightMethod(mutationInfo);

            // Perform mutations of the required type.
            if(mutationInfo.SelectionType == ConnectionSelectionType.Proportional)
            {
                bool mutationOccured=false;
                int connectionCount = _connectionGeneList.Count;

                // ENHANCEMENT: The fastest approach here depends on SelectionProportion and the number of connections...
                // .. implement a simple heuristic.
                for(int i=0; i<connectionCount; i++)
                {
                    if(_genomeFactory.Rng.NextDouble() < mutationInfo.SelectionProportion)
                    {
                        _connectionGeneList[i].Weight = mutateWeigthMethod(_connectionGeneList[i].Weight, mutationInfo);
                        mutationOccured = true;
                    }
                }
                if(!mutationOccured && 0!=connectionCount)
                {   // Perform at least one mutation. Pick a gene at random.
                    ConnectionGene connectionGene = _connectionGeneList[_genomeFactory.Rng.Next(connectionCount)];
                    connectionGene.Weight = mutateWeigthMethod(connectionGene.Weight, mutationInfo);
                }
            }
            else // if(mutationInfo.SelectionType==ConnectionSelectionType.FixedQuantity)
            {
                // Determine how many mutations to perform. At least one - if there are any genes.
                int connectionCount = _connectionGeneList.Count;
                int mutations = Math.Min(connectionCount, mutationInfo.SelectionQuantity);
                if(0==mutations) {
                    return;
                }

                // TODO: Review this approach.
                // The mutation loop. Here we pick an index at random and scan forward from that point
                // for the first non-mutated gene. This prevents any gene from being mutated more than once without
                // too much overhead.
                // Ensure all IsMutated flags are reset prior to entering the loop. Not doing so introduces the
                // possibility of getting stuck in the inner while loop forever, as well as preventing previously 
                // mutated connections from being mutated again.
                _connectionGeneList.ResetIsMutatedFlags();
                for(int i=0; i<mutations; i++)
                {
                    // Pick an index at random.
                    int index = _genomeFactory.Rng.Next(connectionCount);

                    // Scan forward and find the first non-mutated gene.
                    while(_connectionGeneList[index].IsMutated)
                    {   // Increment index. Wrap around back to the start if we go off the end.
                        if(++index==connectionCount) {
                            index=0; 
                        }
                    }
                    
                    // Mutate the gene at 'index'.
                    _connectionGeneList[index].Weight = mutateWeigthMethod(_connectionGeneList[index].Weight, mutationInfo);
                    _connectionGeneList[index].IsMutated = true;
                }
            }
            _genomeFactory.Stats._mutationCountConnectionWeights++;
        }

        delegate double MutateWeightMethod(double weight, ConnectionMutationInfo info);

        /// <summary>
        /// Method that returns a delegate to perform connection weight mutation based on the provided ConnectionMutationInfo
        /// object. Re-using such a delegate obviates the need to test the type of mutation on each weight mutation operation, thus
        /// eliminating many branch execution operations.
        /// </summary>
        private MutateWeightMethod Mutate_ConnectionWeights_GetMutateWeightMethod(ConnectionMutationInfo mutationInfo)
        {
            // FIXME: We are experimenting with unclamped connection weights. Revert before releasing code.
            // ENHANCEMENT: Can we use something akin to a closure here to package up mutation params with the delegate code?
            switch(mutationInfo.PerturbanceType)
            {
                case ConnectionPerturbanceType.JiggleUniform:
                {
                    return delegate(double weight, ConnectionMutationInfo info)
                    {
                        //return CapWeight(weight + (((_genomeFactory.Rng.NextDouble()*2.0) - 1.0) * info.PerturbanceMagnitude));
                        return weight + (((_genomeFactory.Rng.NextDouble()*2.0) - 1.0) * info.PerturbanceMagnitude);
                    };
                }
                case ConnectionPerturbanceType.JiggleGaussian:
                {
                    return delegate(double weight, ConnectionMutationInfo info)
                    {
                        //return CapWeight(weight + _genomeFactory.SampleGaussianDistribution(0, info.Sigma));
                        return weight + _genomeFactory.SampleGaussianDistribution(0, info.Sigma);
                    };
                }
                case ConnectionPerturbanceType.Reset:
                {
                    return delegate {
                        return _genomeFactory.GenerateRandomConnectionWeight();
                    };
                }
            }
            throw new SharpNeatException("Unexpected ConnectionPerturbanceType");
        }

        private double CapWeight(double weight)
        {
            double weightRange = _genomeFactory.NeatGenomeParameters.ConnectionWeightRange;
            if(weight > weightRange) 
            {
                weight = weightRange;
            }
            else if(weight < -weightRange) 
            {
                weight = -weightRange;
            }
            return weight;
        }

        /// <summary>
        /// Tests for an existing connection between a source and target neuron specified by innovation ID.
        /// </summary>
        private bool TestForExistingConnection(uint sourceId, uint targetId)
        {
            // ENHANCEMENT: Consider using a hashmap to speed-up connection lookups.
            int count = _connectionGeneList.Count;
            for(int i=0; i<count; i++)
            {
                if(_connectionGeneList[i].SourceNodeId == sourceId 
                && _connectionGeneList[i].TargetNodeId == targetId) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Redundant neurons are hidden neurons with no connections attached to them.
        /// </summary>
        private bool IsNeuronRedundant(int idx)
        {
            NeuronGene neuronGene = _neuronGeneList[idx];
            if(neuronGene.NodeType != NodeType.Hidden) {
                return false;
            }
            return !IsNeuronConnected(neuronGene.InnovationId);
        }

        private bool IsNeuronConnected(uint neuronId)
        {
            int count = _connectionGeneList.Count;
            for(int i=0; i<count; i++)
            {
                ConnectionGene connectionGene = _connectionGeneList[i];
                if(neuronId == connectionGene.SourceNodeId) {
                    return true;
                }   
                if(neuronId == connectionGene.TargetNodeId) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Builds a dictionary for temporary use required by Mutate_DeleteSimpleNeuronStructure() and 
        /// RemoveSimpleNeuron().
        /// 
        /// The dictionary keys all hidden neurons by their ID. The keyed value is a NeuronConnectionsLookup
        /// object which lists all of the neuron's incoming and outgoing connections in two separate lists.
        /// </summary>
        private Dictionary<uint,NeuronConnectionsLookup> BuildHiddenNeuronConnectionsLookupDictionary()
        {
            Dictionary<uint,NeuronConnectionsLookup> tmpDict 
                = new Dictionary<uint,NeuronConnectionsLookup>(_neuronGeneList.Count - _inputBiasOutputNeuronCount);

            // Create a dictionary entry for all hidden neurons.
            // Loop over all hidden neurons. A fast way to do this is to skip over the bias, input and output neurons
            // that are always at the head of the neuron list.
            int neuronCount = _neuronGeneList.Count;
            for(int neuronIdx = _inputBiasOutputNeuronCount; neuronIdx<neuronCount; neuronIdx++)
            {
                NeuronConnectionsLookup lookup = new NeuronConnectionsLookup();
                lookup._neuronId = _neuronGeneList[neuronIdx].InnovationId;
            }

            // Loop over all connections for this genome. Registering them with NeuronConnectionsLookup against 
            // the neurons they connect to.
            foreach(ConnectionGene connectionGene in _connectionGeneList)
            {
                // Lookup the neuron IDs at the connection endpoints. Remember that bias, input and output neurons
                // aren't in the dictionary, hence we use TryGetValue instead of a straight lookup; We're only tracking
                // connections to hidden neurons.
                NeuronConnectionsLookup lookup;
                if(tmpDict.TryGetValue(connectionGene.SourceNodeId, out lookup)) {   
                    lookup._outputConnectionList.Add(connectionGene);
                }
                if(tmpDict.TryGetValue(connectionGene.TargetNodeId, out lookup)) {   
                    lookup._inputConnectionList.Add(connectionGene);
                }
            }
            return tmpDict;
        }

        #endregion

        #region Private Methods [Sexual Reproduction]

        private void CreateOffspring_Sexual_ProcessCorrelationItem(ConnectionGeneListBuilder connectionListBuilder,
                                                                   CorrelationItem correlationItem,
                                                                   int fitSwitch,
                                                                   bool combineDisjointExcessFlag)                                             
        {
            switch(correlationItem.CorrelationItemType)
            {   
                // Disjoint and excess genes.
                case CorrelationItemType.Disjoint:
                case CorrelationItemType.Excess:
                {
                    // If the gene is in the fittest parent then override any existing entry in the connectionGeneTable.
                    if(1==fitSwitch && null != correlationItem.ConnectionGene1)
                    {
                        CreateOffspring_Sexual_AddGene(connectionListBuilder, correlationItem.ConnectionGene1, true);
                        return;
                    }
                    if(fitSwitch==2 && correlationItem.ConnectionGene2 != null)
                    {
                        CreateOffspring_Sexual_AddGene(connectionListBuilder, correlationItem.ConnectionGene2, true);
                        return;
                    }

                    // The disjoint/excess gene must be on the least fit parent.
                    if(combineDisjointExcessFlag)
                    {
                        // Only add the gene to the genome if a equivalent gene (same endpoints) hasn't already been added.
                        CreateOffspring_Sexual_AddGene(
                            connectionListBuilder,
                            correlationItem.ConnectionGene1 ?? correlationItem.ConnectionGene2,
                            false);
                    }
                    break;
                }
                case CorrelationItemType.Match:
                {
                    // Pick one of the two genes at random. Override any existing gene in case it was a disjoint/excess gene - 
                    // we prefer matched genes to disjoint/excess ones as they are more likely to contribute to fitness.
                    CreateOffspring_Sexual_AddGene(
                            connectionListBuilder,
                            RouletteWheel.SingleThrow(0.5, _genomeFactory.Rng) ? correlationItem.ConnectionGene1 : correlationItem.ConnectionGene2,
                            true);
                    break;
                }
            } 
        }

        /// <summary>
        /// Register/add a connection gene to the provided ConnectionGeneListBuilder. This is a list that will be used to 
        /// instantiate a new genome. New additions are registered with a dictionary of ConnectionGenes keyed
        /// on ConnectionEndpointsStruct so that we can avoid adding connection genes with the same neuron endpoints,
        /// this is possible because the innovation history buffers throw away old innovations in a FIFO manner in 
        /// order to prevent them from growing in size indefinitely.
        /// 
        /// A dictionary of neuron IDs keeps track of all neuron IDs encountered on connection endpoints. We use this
        /// as the source of unique neuron IDs when creating neuron genes for the new genome. We use a SortedDictionary
        /// because it has O(log n) insertion time for unsorted data versus the SortedList's O(n).
        /// </summary>
        private void CreateOffspring_Sexual_AddGene(ConnectionGeneListBuilder connectionListBuilder,
                                                    ConnectionGene connectionGene, bool overwriteExisting)
        {
            ConnectionEndpointsStruct connectionKey = new ConnectionEndpointsStruct(
                                                                connectionGene.SourceNodeId, 
                                                                connectionGene.TargetNodeId);

            // Check if a matching gene has already been added.
            ConnectionGene existingGene;
            connectionListBuilder.ConnectionGeneDictionary.TryGetValue(connectionKey, out existingGene);
            if(null == existingGene)
            {   // We have not yet added a connection with the same neuron endpoints as the one we are trying to add.
                // Append the connection gene to the ConnectionListBuilder.
                connectionListBuilder.Append(connectionGene, connectionKey);
            }
            else if(overwriteExisting)
            {   // The genome we are building already has a connection with the same neuron endpoints as the one we are
                // trying to add. It didn't match up during correlation because it has a different innovation number, this
                // is possible because the innovation history buffers throw away old innovations in a FIFO manner in order
                // to prevent them from bloating.

                // Here the 'overwriteExisting' flag is set so the gene we are currently trying to add is probably from the
                // fitter parent, and therefore we want to use its connection weight in place of the existing gene's weight.
                existingGene.Weight = connectionGene.Weight;
            }
        }

        #endregion

        #region Private Methods [Genome Comparison]

        /// <summary>
        /// Correlates the ConnectionGenes from two distinct genomes based upon gene innovation numbers.
        /// </summary>
        private static CorrelationResults CorrelateConnectionGeneLists(ConnectionGeneList list1, ConnectionGeneList list2)
        {
            // If none of the connections match up then the number of correlation items will be the sum of the two
            // connections list counts..
            CorrelationResults correlationResults = new CorrelationResults(list1.Count + list2.Count);

        //----- Test for special cases.
            int list1Count = list1.Count;
            int list2Count = list2.Count;
            if(0 == list1Count && 0 == list2Count)
            {   // Both lists are empty!
                return correlationResults;
            }

            if(0 == list1Count)
            {   // All list2 genes are excess.
                correlationResults.CorrelationStatistics.ExcessConnectionGeneCount = list2Count;
                foreach(ConnectionGene connectionGene in list2) {
                    correlationResults.CorrelationItemList.Add(new CorrelationItem(CorrelationItemType.Excess, null, connectionGene));
                }
                return correlationResults;
            }

            if(0 == list2Count)
            {   // All list1 genes are excess.
                correlationResults.CorrelationStatistics.ExcessConnectionGeneCount = list1Count;
                foreach(ConnectionGene connectionGene in list1) {
                    correlationResults.CorrelationItemList.Add(new CorrelationItem(CorrelationItemType.Excess, connectionGene, null));
                }
                return correlationResults;
            }

        //----- Both connection genes lists contain genes - compare their contents.
            int list1Idx=0;
            int list2Idx=0;
            ConnectionGene connectionGene1 = list1[list1Idx];
            ConnectionGene connectionGene2 = list2[list2Idx];
            for(;;)
            {
                if(connectionGene2.InnovationId < connectionGene1.InnovationId)
                {   
                    // connectionGene2 is disjoint.
                    correlationResults.CorrelationItemList.Add(new CorrelationItem(CorrelationItemType.Disjoint, null, connectionGene2));
                    correlationResults.CorrelationStatistics.DisjointConnectionGeneCount++;

                    // Move to the next gene in list2.
                    list2Idx++;
                }
                else if(connectionGene1.InnovationId == connectionGene2.InnovationId)
                {
                    correlationResults.CorrelationItemList.Add(new CorrelationItem(CorrelationItemType.Match, connectionGene1, connectionGene2));
                    correlationResults.CorrelationStatistics.ConnectionWeightDelta += Math.Abs(connectionGene1.Weight - connectionGene2.Weight);
                    correlationResults.CorrelationStatistics.MatchingGeneCount++;

                    // Move to the next gene in both lists.
                    list1Idx++;
                    list2Idx++;
                }
                else // (connectionGene2.InnovationId > connectionGene1.InnovationId)
                {   
                    // connectionGene1 is disjoint.
                    correlationResults.CorrelationItemList.Add(new CorrelationItem(CorrelationItemType.Disjoint, connectionGene1, null));
                    correlationResults.CorrelationStatistics.DisjointConnectionGeneCount++;

                    // Move to the next gene in list1.
                    list1Idx++;
                }
                
                // Check if we have reached the end of one (or both) of the lists. If we have reached the end of both then 
                // although we enter the first 'if' block it doesn't matter because the contained loop is not entered if both 
                // lists have been exhausted.
                if(list1Count == list1Idx)
                {   
                    // All remaining list2 genes are excess.
                    for(; list2Idx < list2Count; list2Idx++)
                    {
                        correlationResults.CorrelationItemList.Add(new CorrelationItem(CorrelationItemType.Excess, null, list2[list2Idx]));
                        correlationResults.CorrelationStatistics.ExcessConnectionGeneCount++;
                    }
                    return correlationResults;
                }

                if(list2Count == list2Idx)
                {
                    // All remaining list1 genes are excess.
                    for(; list1Idx < list1Count; list1Idx++)
                    {
                        correlationResults.CorrelationItemList.Add(new CorrelationItem(CorrelationItemType.Excess, list1[list1Idx], null));
                        correlationResults.CorrelationStatistics.ExcessConnectionGeneCount++;
                    }
                    return correlationResults;
                }

                connectionGene1 = list1[list1Idx];
                connectionGene2 = list2[list2Idx];
            }
        }

        #endregion

        #region Private Methods [Debug Code / Integrity Checking]

        /// <summary>
        /// Performs an integrity check on the genome's internal data.
        /// Returns true if OK.
        /// </summary>
        public bool PerformIntegrityCheck()
        {
            // Check neuron genes.
            int count = _neuronGeneList.Count;
            
            // We will always have at least a bias and an output.
            if(count < 2) {
                Debug.WriteLine(string.Format("NeuronGeneList has less than the minimum number of neuron genes [{0}]", count));
                return false;
            }

            // Check bias neuron.
            if(NodeType.Bias != _neuronGeneList[0].NodeType) {
                Debug.WriteLine("Missing bias gene");
                return false;
            }

            if(0u != _neuronGeneList[0].InnovationId) {
                Debug.WriteLine(string.Format("Bias neuron ID != 0. [{0}]", _neuronGeneList[0].InnovationId));
                return false;
            }

            // Check input neurons.
            uint prevId = 0u;
            int idx = 1;
            for(int i=0; i<_inputNeuronCount; i++, idx++)
            {
                if(NodeType.Input != _neuronGeneList[idx].NodeType) {
                    Debug.WriteLine(string.Format("Invalid neuron gene type. Expected Input, got [{0}]", _neuronGeneList[idx].NodeType));
                    return false;
                }

                if(_neuronGeneList[idx].InnovationId <= prevId) {
                    Debug.WriteLine("Input neuron gene is out of order and/or a duplicate.");
                    return false;
                }

                prevId = _neuronGeneList[idx].InnovationId;
            }

            // Check output neurons.
            for(int i=0; i<_outputNeuronCount; i++, idx++)
            {
                if(NodeType.Output != _neuronGeneList[idx].NodeType) {
                    Debug.WriteLine(string.Format("Invalid neuron gene type. Expected Output, got [{0}]", _neuronGeneList[idx].NodeType));
                    return false;
                }

                if(_neuronGeneList[idx].InnovationId <= prevId) {
                    Debug.WriteLine("Output neuron gene is out of order and/or a duplicate.");
                    return false;
                }

                prevId = _neuronGeneList[idx].InnovationId;
            }

            // Check hidden neurons.
            // All remaining neurons should be hidden neurons.
            for(; idx<count; idx++)
            {
                if(NodeType.Hidden != _neuronGeneList[idx].NodeType) {
                    Debug.WriteLine(string.Format("Invalid neuron gene type. Expected Hidden, got [{0}]", _neuronGeneList[idx].NodeType));
                    return false;
                }

                if(_neuronGeneList[idx].InnovationId <= prevId) {
                    Debug.WriteLine("Hidden neuron gene is out of order and/or a duplicate.");
                    return false;
                }

                prevId = _neuronGeneList[idx].InnovationId;
            }

            // Check connection genes.
            count = _connectionGeneList.Count;
            if(0 == count) 
            {   // At leaast one conenction is required. 
                // (A) Connectionless genomes are pointless and 
                // (B) Connections form the basis for defining a genome's position in the encoding space.
                // Without a position speciation will be sub-optimal and may fail (depending on the speciation strategy).
                Debug.WriteLine("Zero connection genes.");
                return false;
            }

            Dictionary<ConnectionEndpointsStruct, object> endpointDict = new Dictionary<ConnectionEndpointsStruct,object>(count);
            
            // Initialise with the first connection's details.
            ConnectionGene connectionGene = _connectionGeneList[0];
            prevId = connectionGene.InnovationId;
            endpointDict.Add(new ConnectionEndpointsStruct(connectionGene.SourceNodeId, connectionGene.TargetNodeId), null);

            // Loop over remaining connections.
            for(int i=1; i<count; i++)
            {
                connectionGene = _connectionGeneList[i];
                if(connectionGene.InnovationId <= prevId) {
                    Debug.WriteLine("Connection gene is out of order and/or a duplicate.");
                    return false;
                }

                ConnectionEndpointsStruct key = new ConnectionEndpointsStruct(connectionGene.SourceNodeId, connectionGene.TargetNodeId);
                if(endpointDict.ContainsKey(key))
                {
                    Debug.WriteLine("Connection gene error. A connection between the specified endpoints already exists.");
                    return false;
                }

                endpointDict.Add(key, null);
                prevId = connectionGene.InnovationId;
            }
            return true;
        }

        #endregion

        #region Inner Classes

        class NeuronConnectionsLookup
        {
            public uint _neuronId;
            public readonly List<ConnectionGene> _inputConnectionList = new List<ConnectionGene>();
            public readonly List<ConnectionGene> _outputConnectionList = new List<ConnectionGene>();
        }

        #endregion

        #region INetworkDefinition Members

        /// <summary>
        /// Gets the number of input nodes. This does not include the bias node which is always present.
        /// </summary>
        public int InputNodeCount
        {
            get { return _inputNeuronCount; }
        }

        /// <summary>
        /// Gets the number of output nodes.
        /// </summary>
        public int OutputNodeCount
        {
            get { return _outputNeuronCount; }
        }

        /// <summary>
        /// Gets the network's activation function library. The activation function at each node is 
        /// represented by an integer ID, which refers to a function in this activation function library.
        /// </summary>
        public IActivationFunctionLibrary ActivationFnLibrary 
        {
            get { return _genomeFactory.ActivationFnLibrary; }
        }

        /// <summary>
        /// Gets the list of network nodes.
        /// </summary>
        public INodeList NodeList
        {
            get { return _neuronGeneList; }
        }

        /// <summary>
        /// Gets the list of network connections.
        /// </summary>
        public IConnectionList ConnectionList
        {
            get { return _connectionGeneList; }
        }

        #endregion
    }
}
