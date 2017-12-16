using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Sexual.Strategy;
using SharpNeat.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat.Reproduction.Sexual
{
    /// <summary>
    /// Creation of offspring given two parents (sexual reproduction).
    /// </summary>
    public class NeatReproductionSexual<T> where T : struct
    {
        readonly NeatReproductionSexualSettings _settings;
        readonly IRandomSource _rng;
        readonly ISexualReproductionStrategy<T> _strategy;

        #region Constructor

        public NeatReproductionSexual(
            MetaNeatGenome<T> metaNeatGenome,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq,
            AddedConnectionBuffer addedConnectionBuffer,
            AddedNodeBuffer addedNodeBuffer,
            NeatReproductionSexualSettings settings)
        {
            _settings = settings;
            _rng = RandomSourceFactory.Create();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sexual reproduction.
        /// </summary>
        /// <param name="parent1">Parent genome 1.</param>
        /// <param name="parent2">Parent genome 2.</param>
        
        public NeatGenome<T> CreateGenome(
            NeatGenome<double> parent1,
            NeatGenome<double> parent2)
        {
            // TODO: Implement.
            throw new NotImplementedException();
        }
        
        #endregion
    }
}
