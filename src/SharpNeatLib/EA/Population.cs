using System.Collections.Generic;
using SharpNeat.Utils;

namespace SharpNeat.EA
{
    public class Population<TGenome>    
    {
        #region Auto Properties

        public List<TGenome> GenomeList { get; }
        public Int32Sequence GenerationSeq { get; }
        
        #endregion

        #region Constructor

        public Population(
            List<TGenome> genomeList)
        {
            this.GenomeList = genomeList;
            this.GenerationSeq = new Int32Sequence();
        }

        #endregion
    }
}
