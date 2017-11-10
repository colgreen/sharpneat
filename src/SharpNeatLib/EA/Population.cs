using System.Collections.Generic;
using SharpNeat.Utils;

namespace SharpNeat.EA
{
    public class Population<TGenome>    
    {
        #region Instance Fields

        readonly List<TGenome> _genomeList;

        // TODO: Consider extracting these into their own class
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        uint _currentGenerationAge;

        #endregion

        #region Constructor

        public Population(
            List<TGenome> genomeList)
        {
            _genomeList = genomeList;
            _genomeIdSeq = new Int32Sequence();
            _innovationIdSeq = new Int32Sequence();
        }

        public Population(
            List<TGenome> genomeList,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq)
        {
            _genomeList = genomeList;
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
        }

        #endregion

        #region Properties

        public List<TGenome> GenomeList => _genomeList;
        public Int32Sequence GenomeIdSeq => _genomeIdSeq;
        public Int32Sequence InnovationIdSeq => _innovationIdSeq;
        public uint CurrentGenerationAge => _currentGenerationAge; 
        
        #endregion

        #region Public Methods

        public void IncrementCurrentGenerationAge() {
            _currentGenerationAge++;
        }

        #endregion
    }
}
