using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNeat.Neat.Genome.IO
{
    public class NeatPopulationLoader<T> where T : struct
    {
        readonly NeatGenomeLoader<T> _genomeLoader;

        #region Constructors

        public NeatPopulationLoader(NeatGenomeLoader<T> genomeLoader)
        {
            _genomeLoader = genomeLoader ?? throw new ArgumentNullException(nameof(genomeLoader));
        }

        #endregion

        #region Public Methods

        public void Load(string path)
        {

        }

        #endregion
    }
}
