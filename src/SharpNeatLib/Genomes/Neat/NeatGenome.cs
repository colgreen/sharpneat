using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.Network;

namespace SharpNeat.Genomes.Neat
{
    public class NeatGenome
    {
        #region Instance Fields

        readonly uint _id;

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
        readonly int _outputNeuronCount;
        readonly int _inputAndBiasNeuronCount;
        readonly int _inputBiasOutputNeuronCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene lists.
        /// </summary>
        public NeatGenome(uint id, 
                          uint birthGeneration,
                          NeuronGeneList neuronGeneList, 
                          ConnectionGeneList connectionGeneList, 
                          int inputNeuronCount,
                          int outputNeuronCount,
                          bool rebuildNeuronGeneConnectionInfo)
        {
            _id = id;
            _neuronGeneList = neuronGeneList;
            _connectionGeneList = connectionGeneList;
            _inputNeuronCount = inputNeuronCount;
            _outputNeuronCount = outputNeuronCount;

            // Precalculate some often used values.
            _inputAndBiasNeuronCount = inputNeuronCount+1;
            _inputBiasOutputNeuronCount = _inputAndBiasNeuronCount + outputNeuronCount;

            // Rebuild per neuron connection info if caller has requested it.
            if(rebuildNeuronGeneConnectionInfo) {
                RebuildNeuronGeneConnectionInfo();
            }

            Debug.Assert(NeatGenomeIntegrity.PerformIntegrityCheck(this));
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public NeatGenome(NeatGenome copyFrom, uint id, uint birthGeneration)
        {
            _id = id;

            // These copy constructors make clones of the genes rather than copies of the object references.
            _neuronGeneList = new NeuronGeneList(copyFrom._neuronGeneList);
            _connectionGeneList = new ConnectionGeneList(copyFrom._connectionGeneList);
            
            // Copy pre-calculated values.
            _inputNeuronCount = copyFrom._inputNeuronCount;
            _outputNeuronCount = copyFrom._outputNeuronCount;
            _inputAndBiasNeuronCount = copyFrom._inputAndBiasNeuronCount;
            _inputBiasOutputNeuronCount = copyFrom._inputBiasOutputNeuronCount;

            Debug.Assert(NeatGenomeIntegrity.PerformIntegrityCheck(this));
        }

        #endregion

        #region Properties [NEAT Genome Specific]

        ///// <summary>
        ///// Gets or sets the NeatGenomeFactory associated with the genome. A reference to the factory is 
        ///// passed to spawned genomes, this allows all genomes within a population to have access to common 
        ///// data such as NeatGenomeParameters and an ID generator.
        ///// Setting the genome factory after construction is allowed in order to resolve chicken-and-egg
        ///// scenarios when loading genomes from storage.
        ///// </summary>
        //public NeatGenomeFactory GenomeFactory
        //{
        //    get { return _genomeFactory; }
        //    set 
        //    {
        //        if(null != _genomeFactory) {
        //            throw new SharpNeatException("NeatGenome already has an assigned GenomeFactory.");
        //        }
        //        _genomeFactory = value;
        //        _evalInfo = new EvaluationInfo(_genomeFactory.NeatGenomeParameters.FitnessHistoryLength);
        //        _auxStateNeuronCount = CountAuxStateNodes();
        //    }
        //}
        
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

        #region Private Methods [Initialisation]

        /// <summary>
        /// Rebuild the connection info on each neuron gene. This info is created by genome factories and maintained during evolution,
        /// but requires building after loading genomes from storage.
        /// </summary>
        private void RebuildNeuronGeneConnectionInfo()
        {
            // Ensure data is cleared down.
            int nCount = _neuronGeneList.Count;
            for(int i=0; i<nCount; i++)
            {
                NeuronGene nGene = _neuronGeneList[i];
                nGene.SourceNeurons.Clear();
                nGene.TargetNeurons.Clear();
            }

            // Loop connections and register them with neuron genes.
            int cCount = _connectionGeneList.Count;
            for(int i=0; i<cCount; i++)
            {
                ConnectionGene cGene = _connectionGeneList[i];
                NeuronGene srcNeuronGene = _neuronGeneList.GetNeuronById(cGene.SourceNodeId);
                NeuronGene tgtNeuronGene = _neuronGeneList.GetNeuronById(cGene.TargetNodeId);
                srcNeuronGene.TargetNeurons.Add(cGene.TargetNodeId);
                tgtNeuronGene.SourceNeurons.Add(cGene.SourceNodeId);
            }
        }

        #endregion
    }
}
