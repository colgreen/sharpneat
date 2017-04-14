using SharpNeat.EA;
using SharpNeat.Neat.Genome;
using SharpNeat.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat
{
    public class NeatPopulation : Population<NeatGenome>
    {
        MetaNeatGenome _metaNeatGenome;

        #region Constructor

        public NeatPopulation(Uint32Sequence genomeIdSeq, List<NeatGenome> genomeList, MetaNeatGenome metaNeatGenome)
            : base(genomeIdSeq, genomeList)
        {
            _metaNeatGenome = metaNeatGenome;
        }

        #endregion

        #region Properties

        public MetaNeatGenome MetaNeatGenome => _metaNeatGenome;

        #endregion
    }
}
