using System.Collections.Generic;
using SharpNeat.Utils;

namespace SharpNeat.EA
{
    public class Population<TGenome>    
    {
        #region Auto Properties

        public List<TGenome> GenomeList { get; }
        public Int32Sequence GenomeIdSeq { get; }
        public Int32Sequence InnovationIdSeq { get; }
        public Int32Sequence GenerationSeq { get; }
        
        #endregion

        #region Constructor

        public Population(
            List<TGenome> genomeList)
        {
            this.GenomeList = genomeList;
            this.GenomeIdSeq = new Int32Sequence();
            this.InnovationIdSeq = new Int32Sequence();
            this.GenerationSeq = new Int32Sequence();
        }

        public Population(
            List<TGenome> genomeList,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq)
        {
            this.GenomeList = genomeList;
            this.GenomeIdSeq = genomeIdSeq;
            this.InnovationIdSeq = innovationIdSeq;
            this.GenerationSeq = new Int32Sequence();
        }

        #endregion
    }
}
