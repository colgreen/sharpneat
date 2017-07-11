using SharpNeat.Utils;
using System.Collections.Generic;

namespace SharpNeat.EA
{
    public class Population<TGenome>    
    {
        readonly UInt32Sequence _genomeIdSeq;
        readonly UInt32Sequence _innovationIdSeq;
        readonly List<TGenome> _genomeList;
        uint _currentGenerationAge;

        #region Constructor

        public Population(UInt32Sequence genomeIdSeq, UInt32Sequence innovationIdSeq,
                          List<TGenome> genomeList)
        {
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _genomeList = genomeList;
        }

        #endregion

        #region Properties

        public UInt32Sequence GenomeIdSeq => _genomeIdSeq;
        public UInt32Sequence InnovationIdSeq => _innovationIdSeq;
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
