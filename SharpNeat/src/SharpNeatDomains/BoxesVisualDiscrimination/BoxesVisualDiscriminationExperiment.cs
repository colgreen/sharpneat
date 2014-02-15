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
using System.Xml;
using log4net;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.HyperNeat;
using SharpNeat.DistanceMetrics;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;
using System.Threading.Tasks;

namespace SharpNeat.Domains.BoxesVisualDiscrimination
{
    /// <summary>
    /// Boxes Visual Discrimination Task, as described in:
    /// 'A Hypercube-Based Encoding for Evolving Large-Scale Neural Networks', Kenneth O. Stanley, David B. D'Ambrosio,
    /// and Jason Gauci (2009) http://eplex.cs.ucf.edu/publications/2009/stanley.alife09.html
    ///
    /// The task involves a visual field of 11x11 binary pixels (on/off, black/white). The field contains two boxes, one
    /// small and one large box that has an edge length 3x that of the smaller box. The goal is to identify the center
    /// pixel of the larger box.
    ///
    /// A single evaluation consists of 75 test cases with the two boxes randomly positioned. The root mean squared
    /// distance between selected and target pixels is scaled and translated to a range of 0-100, where 0 = no better
    /// than an agent randomly selecting pixels and 100 = perfect. In addition to this the range of output values is
    /// scaled to 0-10 and added to the final score, this encourages solutions with a wide output range between the 
    /// highest activation (the selected pixel) and the lowest activation (this encourages prominent/clear selection).
    ///
    /// The problem domain view allows the performance of the best genome to be observed. The view also allows the 
    /// HyperNEAT genome to be decoded using a range of visual field pixel resolutions beyond the 11x11 resolution that
    /// genomes are trained with. The ability to do this is a feature of HyperNEAT - the evolved genomes are Compositional
    /// Pattern Producing Networks (CPPNs). A CPPN defines the connection strength between nodes positioned in some
    /// euclidean space and therefore we can use a CPPN to produce neural networks with more nodes by increasing the 
    /// number and density of nodes.
    /// </summary>
    public class BoxesVisualDiscriminationExperiment : IGuiNeatExperiment
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
        int _visualFieldResolution;
        int _visualFieldPixelCount;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BoxesVisualDiscriminationExperiment()
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
        /// 6 inputs. 2 * (x,y,z) CPPN substrate node position coordinates, plus one optional connection length input.
        /// </summary>
        public int InputCount
        {
            get { return _lengthCppnInput ? 7 : 6; }
        }

        /// <summary>
        /// Gets the number of outputs required by the network/black-box that the underlying problem domain is based on.
        /// 2 outputs.CPPN weight output and bias weight output.
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

            _visualFieldResolution = XmlUtils.GetValueAsInt(xmlConfig, "Resolution");
            _visualFieldPixelCount = _visualFieldResolution * _visualFieldResolution;
            _lengthCppnInput = XmlUtils.GetValueAsBool(xmlConfig, "LengthCppnInput");

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
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
            NeatGenomeXmlIO.WriteComplete(xw, genomeList, true);
        }

        /// <summary>
        /// Create a genome decoder for the experiment.
        /// </summary>
        public IGenomeDecoder<NeatGenome,IBlackBox> CreateGenomeDecoder()
        {
            return CreateGenomeDecoder(_visualFieldResolution, _lengthCppnInput);
        }

        /// <summary>
        /// Create a genome factory for the experiment.
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
            BoxesVisualDiscriminationEvaluator evaluator = new BoxesVisualDiscriminationEvaluator(_visualFieldResolution);

            // Create genome decoder. Decodes to a neural network packaged with an activation scheme that defines a fixed number of activations per evaluation.
            IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder = CreateGenomeDecoder(_visualFieldResolution, _lengthCppnInput);

            // Create a genome list evaluator. This packages up the genome decoder with the genome evaluator.
            IGenomeListEvaluator<NeatGenome> innerEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator, _parallelOptions);

            // Wrap the list evaluator in a 'selective' evaulator that will only evaluate new genomes. That is, we skip re-evaluating any genomes
            // that were in the population in previous generations (elite genomes). This is determiend by examining each genome's evaluation info object.
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
            return new BoxesVisualDiscriminationView(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the visual resolution for the task, as loaded from the experiment config XML.
        /// </summary>
        public int VisualFieldResolution
        {
            get { return _visualFieldResolution; }
        }

        /// <summary>
        /// Gets the CPPN length input flag, as loaded from the experiment config XML.
        /// </summary>
        public bool LengthCppnInput
        {
            get { return _lengthCppnInput; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a genome decoder. We split this code into a separate  method so that it can be re-used by the problem domain visualization code
        /// (it needs to decode genomes to phenomes in order to create a visualization).
        /// </summary>
        /// <param name="visualFieldResolution">The visual field's pixel resolution, e.g. 11 means 11x11 pixels.</param>
        /// <param name="lengthCppnInput">Indicates if the CPPNs being decoded have an extra input for specifying connection length.</param>
        public IGenomeDecoder<NeatGenome,IBlackBox> CreateGenomeDecoder(int visualFieldResolution, bool lengthCppnInput)
        {
            // Create two layer 'sandwich' substrate.
            int pixelCount = visualFieldResolution * visualFieldResolution;
            double pixelSize = BoxesVisualDiscriminationEvaluator.VisualFieldEdgeLength / visualFieldResolution;
            double originPixelXY = -1 + (pixelSize/2.0);

            SubstrateNodeSet inputLayer = new SubstrateNodeSet(pixelCount);
            SubstrateNodeSet outputLayer = new SubstrateNodeSet(pixelCount);

            // Node IDs start at 1. (bias node is always zero).
            uint inputId = 1;
            uint outputId = (uint)(pixelCount + 1);
            double yReal = originPixelXY;

            for(int y=0; y<visualFieldResolution; y++, yReal += pixelSize)
            {
                double xReal = originPixelXY;
                for(int x=0; x<visualFieldResolution; x++, xReal += pixelSize, inputId++, outputId++)
                {
                    inputLayer.NodeList.Add(new SubstrateNode(inputId, new double[] {xReal, yReal, -1.0}));
                    outputLayer.NodeList.Add(new SubstrateNode(outputId, new double[] {xReal, yReal, 1.0}));
                }
            }

            List<SubstrateNodeSet> nodeSetList = new List<SubstrateNodeSet>(2);
            nodeSetList.Add(inputLayer);
            nodeSetList.Add(outputLayer);

            // Define connection mappings between layers/sets.
            List<NodeSetMapping> nodeSetMappingList = new List<NodeSetMapping>(1);
            nodeSetMappingList.Add(NodeSetMapping.Create(0, 1,(double?)null));

            // Construct substrate.
            Substrate substrate = new Substrate(nodeSetList, DefaultActivationFunctionLibrary.CreateLibraryNeat(SteepenedSigmoid.__DefaultInstance), 0, 0.2, 5, nodeSetMappingList);

            // Create genome decoder. Decodes to a neural network packaged with an activation scheme that defines a fixed number of activations per evaluation.
            IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder = new HyperNeatDecoder(substrate, _activationSchemeCppn, _activationScheme, lengthCppnInput);
            return genomeDecoder;
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
