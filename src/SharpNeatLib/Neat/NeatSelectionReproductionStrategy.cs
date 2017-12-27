using SharpNeat.EA;

using SharpNeat.Neat.Genome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat
{
    public class NeatSelectionReproductionStrategy : ISelectionReproductionStrategy<NeatGenome<double>>
    {
        public void Invoke(Population<NeatGenome<double>> population)
        {
            var neatPop = population as NeatPopulation<double>;
            

            throw new NotImplementedException();
        }
    }
}
