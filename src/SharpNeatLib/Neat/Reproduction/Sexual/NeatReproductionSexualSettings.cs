using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat.Reproduction.Sexual
{
    public class NeatReproductionSexualSettings
    {
        // TODO: Review. Not currently being used?
        /// <summary>
        /// Probability that all excess and disjoint genes are copied into an offspring genome during sexual reproduction. 
        /// Currently the excess/disjoint genes are copied in an 'all or nothing' strategy.
        /// </summary>
        public double DisjointExcessGenesRecombinedProbability { get; set; } = 0.1;
    }
}
