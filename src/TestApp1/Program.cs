using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.EA;
using SharpNeat.EA.Controllers;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace TestApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //EAFactory factory = new EAFactory();


            //DefaultEvolutionAlgorithm<NeatGenome<double>> ea = factory.CreateDefaultEvolutionAlgorithm();
            //EvolutionAlgorithmController eaController = new EvolutionAlgorithmController(ea);



            //ea.PerformOneGeneration();

            //EvolutionAlgorithmController eaController = new EvolutionAlgorithmController()


            MetaNeatGenome metaNeatGenome = new MetaNeatGenome();
            metaNeatGenome.InputNodeCount = 3;
            metaNeatGenome.OutputNodeCount = 1;
            metaNeatGenome.IsAcyclic = true;

            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, 10);

            

        }
        

    }
}
