using SharpNeat.EA;
using SharpNeat.Genomes.Neat;
using SharpNeat.Neat.Genome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat
{
    public class NeatDifferentialReproductionStrategy : IDifferentialReproductionStrategy<NeatGenome<double>>
    {
        public void Invoke(Population<NeatGenome<double>> population)
        {
            var neatPop = population as NeatPopulation<double>;
            

            throw new NotImplementedException();
        }
    }
}
