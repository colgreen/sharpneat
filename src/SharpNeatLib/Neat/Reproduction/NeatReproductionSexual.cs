using SharpNeat.Neat.Genome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat.Reproduction
{
    /// <summary>
    /// Creation of offspring given two parents (sexual reproduction).
    /// </summary>
    public class NeatReproductionSexual
    {
        NeatReproductionSexualSettings _settings;

        #region Constructor

        public NeatReproductionSexual(NeatReproductionSexualSettings settings)
        {
            _settings = settings;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sexual reproduction.
        /// </summary>
        /// <param name="parent1">Parent genome 1.</param>
        /// <param name="parent2">Parent genome 2.</param>
        /// <param name="birthGeneration">The birth generation of the new genome.</param>
        public NeatGenome CreateGenome(NeatGenome parent1, NeatGenome parent2, uint birthGeneration)
        {
            // TODO: Implement.
            throw new NotImplementedException();
        }
        
        #endregion
    }
}
