using SharpNeat.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.EA
{
    public class Population<TGenome>    
    {
        readonly Uint32Sequence _genomeIdSeq;
        readonly List<TGenome> _genomeList;
        uint _currentGenerationAge;

        #region Constructor

        public Population(Uint32Sequence genomeIdSeq, List<TGenome> genomeList)
        {
            _genomeIdSeq = genomeIdSeq;
            _genomeList = genomeList;
        }

        #endregion

        #region Properties

        public Uint32Sequence GenomeIdSeq => _genomeIdSeq;
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
