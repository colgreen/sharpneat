using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.EvolutionAlgorithm.Runner;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace TestApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create and initialise the evolutionary algorithm.
            EvolutionAlgorithmFactory factory = new EvolutionAlgorithmFactory();
            NeatEvolutionAlgorithm<double> ea = factory.CreateNeatEvolutionAlgorithm();
            ea.Initialise();

            //EvolutionAlgorithmRunner runner = new EvolutionAlgorithmRunner(ea);

            for(int i=0; i < 10_000; i++)
            {
                ea.PerformOneGeneration();
                Console.WriteLine($"{ea.Stats.Generation} {ea.Stats.BestFitness.PrimaryFitness}");
                //Console.WriteLine($"{ea.Stats.Generation} {ea.Stats.TotalEvaluationCount}");
            }

            Console.ReadKey();
        }
    }
}
