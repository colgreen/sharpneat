using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.EA;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Phenomes;
using SharpNeatTasks.BinaryElevenMultiplexerTask;

namespace TestApp1
{
    public class EAFactory
    {
        EAParameters _eaParams;
        MetaNeatGenome _metaNeatGenome;
        NeatPopulation<double> _neatPop;

        #region Public Methods

        public DefaultEvolutionAlgorithm<NeatGenome<double>> CreateDefaultEvolutionAlgorithm()
        {
            _eaParams = new EAParameters();
            _eaParams.PopulationSize = 100;


            _neatPop = CreatePopulation(_eaParams.PopulationSize);


            IGenomeListEvaluator<NeatGenome<double>> genomeListEvaluator = CreateGenomeListEvaluator();


            var ea = new DefaultEvolutionAlgorithm<NeatGenome<double>>(_eaParams, null, null, _neatPop);
            return ea;
        }

        #endregion

        #region Private Methods

        private NeatPopulation<double> CreatePopulation(int size)
        {
            MetaNeatGenome metaNeatGenome = new MetaNeatGenome();
            metaNeatGenome.InputNodeCount = 3;
            metaNeatGenome.OutputNodeCount = 1;
            metaNeatGenome.IsAcyclic = true;

            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, size);
            return neatPop;
        }


        private IGenomeListEvaluator<NeatGenome<double>> CreateGenomeListEvaluator()
        {
            var genomeDecoder = new NeatGenomeAcyclicDecoder(false);
            IPhenomeEvaluator<IPhenome<double>> phenomeEvaluator = new BinaryElevenMultiplexerEvaluator();
            IGenomeListEvaluator<NeatGenome<double>> genomeListEvaluator = new SerialGenomeListEvaluator<NeatGenome<double>,IPhenome<double>>(genomeDecoder, phenomeEvaluator);
            return genomeListEvaluator;
        }

        #endregion

    }
}
