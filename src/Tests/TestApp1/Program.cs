using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.EA;
using SharpNeat.EA.Runner;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace TestApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            EAFactory factory = new EAFactory();
            DefaultEvolutionAlgorithm<NeatGenome<double>> ea = factory.CreateDefaultEvolutionAlgorithm();
            //EvolutionAlgorithmController eaController = new EvolutionAlgorithmController(ea);
            ea.PerformOneGeneration();

            Console.ReadKey();
        }

    }
}
