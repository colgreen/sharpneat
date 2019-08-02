using System;

namespace TestApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create and initialise the evolutionary algorithm.
            var ea = EvolutionAlgorithmFactory.CreateNeatEvolutionAlgorithm_SinglePoleBalancing();
            ea.Initialise();

            var neatPop = ea.Population;

            for(int i = 0; i < 10_000; i++)
            {
                ea.PerformOneGeneration();
                Console.WriteLine($"{ea.Stats.Generation} {neatPop.Stats.BestFitness.PrimaryFitness} {neatPop.Stats.MeanComplexity} {ea.ComplexityRegulationMode}");
            }
        }
    }
}
