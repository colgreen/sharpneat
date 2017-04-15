using SharpNeat.Utils;
using System.Collections.Generic;

namespace SharpNeat.EA
{
    public class Population<TGenome>    
    {
        readonly Uint32Sequence _genomeIdSeq;
        readonly Uint32Sequence _innovationIdSeq;
        readonly List<TGenome> _genomeList;
        uint _currentGenerationAge;

        #region Constructor

        public Population(Uint32Sequence genomeIdSeq, Uint32Sequence innovationIdSeq,
                          List<TGenome> genomeList)
        {
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _genomeList = genomeList;
        }

        #endregion

        #region Properties

        public Uint32Sequence GenomeIdSeq => _genomeIdSeq;
        public Uint32Sequence InnovationIdSeq => _innovationIdSeq;
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
