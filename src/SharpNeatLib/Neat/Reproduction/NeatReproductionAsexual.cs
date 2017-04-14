using SharpNeat.Neat.Genome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat.Reproduction
{
    /// <summary>
    /// Creation of offspring given a single parent (asexual reproduction).
    /// </summary>
    public class NeatReproductionAsexual
    {
        NeatReproductionAsexualSettings _settings;

        #region Constructor

        public NeatReproductionAsexual(NeatReproductionAsexualSettings settings)
        {
            _settings = settings;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Asexual reproduction.
        /// </summary>
        /// <param name="parent1">Parent genome.</param>
        /// <param name="birthGeneration">The birth generation of the new genome.</param>
        public NeatGenome CreateOffspring(NeatGenome parent, uint id, uint birthGeneration)
        {
            // TODO: Implement.
            throw new NotImplementedException();
        }
        
        #endregion

    }
}
