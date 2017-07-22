using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.EA;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Phenomes;
using SharpNeatTasks.BinaryElevenMultiplexerTask;

namespace TestApp1
{
    public class EAFactory
    {
        EAParameters _eaParams;
        MetaNeatGenome _metaNeatGenome;
        NeatPopulation _neatPop;

        #region Public Mthods

        public DefaultEvolutionAlgorithm<NeatGenome> CreateDefaultEvolutionAlgorithm()
        {
            _eaParams = new EAParameters();
            _eaParams.PopulationSize = 100;


            _neatPop = CreatePopulation(_eaParams.PopulationSize);


            IGenomeListEvaluator<NeatGenome> genomeListEvaluator = CreateGenomeListEvaluator();


            var ea = new DefaultEvolutionAlgorithm<NeatGenome>(_eaParams, null, null, _neatPop);
            return ea;
        }

        #endregion

        #region Private Methods

        private NeatPopulation CreatePopulation(int size)
        {
            MetaNeatGenome metaNeatGenome = new MetaNeatGenome();
            metaNeatGenome.InputNodeCount = 3;
            metaNeatGenome.OutputNodeCount = 1;
            metaNeatGenome.IsAcyclic = true;

            NeatPopulation neatPop = NeatPopulationFactory.CreatePopulation(metaNeatGenome, 1.0, size);
            return neatPop;
        }


        private IGenomeListEvaluator<NeatGenome> CreateGenomeListEvaluator()
        {

            // TODO: Create genome decoder!





            IPhenomeEvaluator<IBlackBox<double>> phenomeEvaluator = new BinaryElevenMultiplexerEvaluator();
            IGenomeListEvaluator<NeatGenome> genomeListEvaluator = new SerialGenomeListEvaluator<NeatGenome,IBlackBox<double>>(null, phenomeEvaluator);
            return genomeListEvaluator;
        }


        #endregion

    }
}
