/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2012 Colin Green (sharpneat@gmail.com)
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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.HyperNeat;
using SharpNeat.DistanceMetrics;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// INeatExperiment for the Walker2D task.
    /// </summary>
    public class WalkerBox2dExperimentHyperNeat : IGuiNeatExperiment
    {
        private static readonly ILog __log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        NeatEvolutionAlgorithmParameters _eaParams;
        NeatGenomeParameters _neatGenomeParams;
        string _name;
        int _populationSize;
        int _specieCount;
        NetworkActivationScheme _activationSchemeCppn;
        NetworkActivationScheme _activationScheme;
        string _complexityRegulationStr;
        int? _complexityThreshold;
        string _description;
        ParallelOptions _parallelOptions;
        bool _lengthCppnInput;

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WalkerBox2dExperimentHyperNeat()
        {
        }

        #endregion

        #region INeatExperiment

        /// <summary>
        /// Gets the name of the experiment.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets human readable explanatory text for the experiment.
        /// </summary>
		public string Description
		{
			get { return _description; }
		}

        /// <summary>
        /// Gets the number of inputs required by the network/black-box that the underlying problem domain is based on.
        /// </summary>
        public int InputCount
        {
           get { return _lengthCppnInput ? 7 : 6; }
        }

        /// <summary>
        /// Gets the number of outputs required by the network/black-box that the underlying problem domain is based on.
        /// </summary>
        public int OutputCount
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the default population size to use for the experiment.
        /// </summary>
        public int DefaultPopulationSize 
        {
            get { return _populationSize; }
        }

        /// <summary>
        /// Gets the NeatEvolutionAlgorithmParameters to be used for the experiment. Parameters on this object can be 
        /// modified. Calls to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in 
        /// at the time of the call.
        /// </summary>
        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
        {
            get { return _eaParams; }
        }

        /// <summary>
        /// Gets the NeatGenomeParameters to be used for the experiment. Parameters on this object can be modified. Calls
        /// to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in at the time of the call.
        /// </summary>
        public NeatGenomeParameters NeatGenomeParameters
        {
            get { return _neatGenomeParams; }
        }

        /// <summary>
        /// Initialize the experiment with some optional XML configutation data.
        /// </summary>
        public void Initialize(string name, XmlElement xmlConfig)
        {
            _name = name;
            _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
            _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
            _activationSchemeCppn = ExperimentUtils.CreateActivationScheme(xmlConfig, "ActivationCppn");
            _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
            _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
            _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
            _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");
            _parallelOptions = ExperimentUtils.ReadParallelOptions(xmlConfig);
            _lengthCppnInput = XmlUtils.GetValueAsBool(xmlConfig, "LengthCppnInput");

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
            _neatGenomeParams.FeedforwardOnly = _activationSchemeCppn.AcyclicNetwork;
            _neatGenomeParams.InitialInterconnectionsProportion = 0.5;
        }

        /// <summary>
        /// Load a population of genomes from an XmlReader and returns the genomes in a new list.
        /// The genome factory for the genomes can be obtained from any one of the genomes.
        /// </summary>
        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            NeatGenomeFactory genomeFactory = (NeatGenomeFactory)CreateGenomeFactory();
            return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
        }

        /// <summary>
        /// Save a population of genomes to an XmlWriter.
        /// </summary>
        public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
        {
            // Writing node IDs is not necessary for NEAT.
            NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
        }

        /// <summary>
        /// Create a genome decoder for the experiment.
        /// </summary>
        public IGenomeDecoder<NeatGenome,IBlackBox> CreateGenomeDecoder()
        {
            // Create HyperNEAT network substrate.

        //-- Create input layer nodes.
            SubstrateNodeSet inputLayer = new SubstrateNodeSet(13);

            //-- Hip joint inputs.
            // Left hip joint.
            AddNode(inputLayer, 1, -1.0, +1.0, -1.0);  // Angular velocity.
            AddNode(inputLayer, 2, -0.5, +1.0, -1.0);  // Angle.
            
            // Right hip joint.
            AddNode(inputLayer, 3, +0.5, +1.0, -1.0);  // Angle.
            AddNode(inputLayer, 4, +1.0, +1.0, -1.0);  // Angular velocity.

            //-- Knee joint inputs.
            // Left knee joint.
            AddNode(inputLayer, 5, -1.0, -1.0, -1.0);  // Angular velocity.
            AddNode(inputLayer, 6, -0.5, -1.0, -1.0);  // Angle.

            // Right knee joint.
            AddNode(inputLayer, 7, +0.5, -1.0, -1.0);  // Angular velocity.
            AddNode(inputLayer, 8, +1.0, -1.0, -1.0);  // Angle.

            //-- Torso inputs.
            AddNode(inputLayer, 9, 0.0, +1.0, -1.0);    // Torso elevation.
            AddNode(inputLayer, 10, 0.0, +0.75, -1.0);  // Torso angle.
            AddNode(inputLayer, 11, 0.0, +0.5, -1.0);   // Torso angular velocity.
            AddNode(inputLayer, 12, 0.0, +0.25, -1.0);  // Velocity X.
            AddNode(inputLayer, 13, 0.0, 0.0, -1.0);    // Velocity Y.

        //-- Output layer nodes.
            SubstrateNodeSet outputLayer = new SubstrateNodeSet(4);
            AddNode(outputLayer, 14, -1.0, +1.0, +1.0); // Left hip torque.
            AddNode(outputLayer, 15, +1.0, +1.0, +1.0); // Right hip torque.
            AddNode(outputLayer, 16, -1.0, -1.0, +1.0); // Left knee torque.
            AddNode(outputLayer, 17, +1.0, -1.0, +1.0); // Right knee torque.

        //-- Hidden layer nodes.
            SubstrateNodeSet h1Layer = new SubstrateNodeSet(4);
            AddNode(h1Layer, 18, -1.0, +1.0, 0.0); 
            AddNode(h1Layer, 19, +1.0, +1.0, 0.0); 
            AddNode(h1Layer, 20, -1.0, -1.0, 0.0); 
            AddNode(h1Layer, 21, +1.0, -1.0, 0.0); 

        // Connect up layers.
            List<SubstrateNodeSet> nodeSetList = new List<SubstrateNodeSet>(2);
            nodeSetList.Add(inputLayer);
            nodeSetList.Add(outputLayer);
            nodeSetList.Add(h1Layer);

            List<NodeSetMapping> nodeSetMappingList = new List<NodeSetMapping>(1);
            nodeSetMappingList.Add(NodeSetMapping.Create(0, 1, (double?)null));  // Input -> Output.
            nodeSetMappingList.Add(NodeSetMapping.Create(0, 2, (double?)null));  // Input -> Hidden.
            nodeSetMappingList.Add(NodeSetMapping.Create(2, 1, (double?)null));  // Hidden -> Output.
            nodeSetMappingList.Add(NodeSetMapping.Create(1, 2, (double?)null));  // Output -> Hidden

            // Construct substrate.
            Substrate substrate = new Substrate(nodeSetList, DefaultActivationFunctionLibrary.CreateLibraryNeat(SteepenedSigmoid.__DefaultInstance), 0, 0.2, 5, nodeSetMappingList);

            // Create genome decoder. Decodes to a neural network packaged with an activation scheme that defines a fixed number of activations per evaluation.
            IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder = new HyperNeatDecoder(substrate, _activationSchemeCppn, _activationScheme, _lengthCppnInput);
            return genomeDecoder;
        }

        private void AddNode(SubstrateNodeSet nodeSet, uint id, double x, double y, double z)
        {
            nodeSet.NodeList.Add(new SubstrateNode(id, new double[] {x, y, y}));
        }

        /// <summary>
        /// Create a genome factory for the experiment.
        /// Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
        /// </summary>
        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new CppnGenomeFactory(InputCount, OutputCount, GetCppnActivationFunctionLibrary(), _neatGenomeParams);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// Uses the experiments default population size defined in the experiment's config XML.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
        {
            return CreateEvolutionAlgorithm(_populationSize);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a population size parameter that specifies how many genomes to create in an initial randomly
        /// generated population.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
        {
            // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

            // Create evolution algorithm.
            return CreateEvolutionAlgorithm(genomeFactory, genomeList);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a pre-built genome population and their associated/parent genome factory.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
        {
            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, _parallelOptions);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

            // Create the evolution algorithm.
            NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy, complexityRegulationStrategy);

            // Create IBlackBox evaluator.
            WalkerBox2dEvaluator evaluator = new WalkerBox2dEvaluator();

            // Create genome decoder.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder();

            // Create a genome list evaluator. This packages up the genome decoder with the genome evaluator.
            IGenomeListEvaluator<NeatGenome> innerEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator, _parallelOptions);
            

            // Wrap the list evaluator in a 'selective' evaulator that will only evaluate new genomes. That is, we skip re-evaluating any genomes
            // that were in the population in previous generations (elite genomes). This is determined by examining each genome's evaluation info object.
            IGenomeListEvaluator<NeatGenome> selectiveEvaluator = new SelectiveGenomeListEvaluator<NeatGenome>(
                                                                                    innerEvaluator,
                                                                                    SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());
            // Initialize the evolution algorithm.
            ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);

            // Finished. Return the evolution algorithm
            return ea;
        }

        /// <summary>
        /// Create a System.Windows.Forms derived object for displaying genomes.
        /// </summary>
        public AbstractGenomeView CreateGenomeView()
        {
            return new CppnGenomeView(GetCppnActivationFunctionLibrary());
        }

        /// <summary>
        /// Create a System.Windows.Forms derived object for displaying output for a domain (e.g. show best genome's output/performance/behaviour in the domain). 
        /// </summary>
        public AbstractDomainView CreateDomainView()
        {
            return new WalkerBox2dView(CreateGenomeDecoder());
        }

        #endregion

        #region Private Methods

        IActivationFunctionLibrary GetCppnActivationFunctionLibrary()
        {
            return DefaultActivationFunctionLibrary.CreateLibraryCppn();
        }

        #endregion
    }
}
