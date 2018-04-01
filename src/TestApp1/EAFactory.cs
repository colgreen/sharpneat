using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.EA;
using SharpNeat.Evaluation;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Phenomes;
using SharpNeatTasks.BinaryElevenMultiplexer;

namespace TestApp1
{
    public class EAFactory
    {
        EAParameters _eaParams;
        MetaNeatGenome<double> _metaNeatGenome;
        NeatPopulation<double> _neatPop;

        #region Public Methods

        public DefaultEvolutionAlgorithm<NeatGenome<double>> CreateDefaultEvolutionAlgorithm()
        {
            _eaParams = new EAParameters();
            _eaParams.PopulationSize = 100;


            var metaNeatGenome = CreateMetaNeatGenome();
            _neatPop = CreatePopulation(metaNeatGenome, _eaParams.PopulationSize);


            //IGenomeListEvaluator<NeatGenome<double>> genomeListEvaluator = CreateGenomeListEvaluator();


            var ea = new DefaultEvolutionAlgorithm<NeatGenome<double>>(
                _eaParams,
                evaluator: null,
                selectionReproStrategy: null,
                population: _neatPop);

            return ea;
        }

        #endregion

        #region Private Static Methods

        //private IGenomeCollectionEvaluator<NeatGenome<double>> CreateGenomeListEvaluator()
        //{
        //    var genomeDecoder = new NeatGenomeAcyclicDecoder(false);
        //    var phenomeEvaluator = new BinaryElevenMultiplexerEvaluator();
        //    var genomeCollectionEvaluator = new SerialGenomeListEvaluator<NeatGenome<double>, IPhenome<double>>(genomeDecoder, phenomeEvaluator);
        //    return genomeListEvaluator;
        //}

        private static NeatPopulation<double> CreatePopulation(
            MetaNeatGenome<double> metaNeatGenome,
            int popSize)
        {
            NeatPopulation<double> pop = NeatPopulationFactory<double>.CreatePopulation(
                metaNeatGenome,
                connectionsProportion: 1.0,
                popSize: popSize);

            return pop;
        }

        private static MetaNeatGenome<double> CreateMetaNeatGenome()
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 3, 
                outputNodeCount: 1,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNet.Double.ActivationFunctions.ReLU());

            return metaNeatGenome;
        }

        #endregion
    }
}
