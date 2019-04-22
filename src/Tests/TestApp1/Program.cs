using System;

namespace TestApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create and initialise the evolutionary algorithm.
            var factory = new EvolutionAlgorithmFactoryBinary11();
            var ea = factory.CreateNeatEvolutionAlgorithm();
            ea.Initialise();

            var neatPop = ea.Population;

            for(int i=0; i < 10_000; i++)
            {
                ea.PerformOneGeneration();
                Console.WriteLine($"{ea.Stats.Generation} {neatPop.Stats.BestFitness.PrimaryFitness} {ea.Stats.TotalEvaluationCount} {ea.ComplexityRegulationMode}");
            }

            Console.ReadKey();
        }
    }
}
