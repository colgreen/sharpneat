using System.Collections.Generic;
using SharpNeat.Utils;

namespace SharpNeat.EA
{
    public class Population<TGenome>    
    {
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        readonly List<TGenome> _genomeList;
        uint _currentGenerationAge;

        #region Constructor

        public Population(Int32Sequence genomeIdSeq, Int32Sequence innovationIdSeq,
                          List<TGenome> genomeList)
        {
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _genomeList = genomeList;
        }

        #endregion

        #region Properties

        public Int32Sequence GenomeIdSeq => _genomeIdSeq;
        public Int32Sequence InnovationIdSeq => _innovationIdSeq;
        public List<TGenome> GenomeList => _genomeList;
        public uint CurrentGenerationAge => _currentGenerationAge; 
        
        #endregion

        #region Public Methods

        public void IncrementCurrentGenerationAge() {
            _currentGenerationAge++;
        }

        #endregion
    }
}
