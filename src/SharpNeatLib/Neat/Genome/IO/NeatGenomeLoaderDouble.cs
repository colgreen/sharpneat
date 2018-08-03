using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SharpNeat.Neat.Genome.IO
{
    /// <summary>
    /// For loading/deserializing instances of <see cref="NeatGenome{double}"/> from file, stream, etc.
    /// </summary>
    public class NeatGenomeLoaderDouble : NeatGenomeLoader<double>
    {
        #region Constructors

        /// <summary>
        /// Instantiates a new instance of <see cref="NeatGenomeLoaderDouble"/>.
        /// </summary>
        /// <param name="metaNeatGenome">NeatGenome metadata.</param>
        public NeatGenomeLoaderDouble(
            MetaNeatGenome<double> metaNeatGenome) 
            : base(metaNeatGenome)
        {}

        /// <summary>
        /// Instantiates a new instance of <see cref="NeatGenomeLoaderDouble"/>.
        /// </summary>
        /// <param name="metaNeatGenome">NeatGenome metadata.</param>
        /// <param name="connCountEstimate">An estimate of the cumber of connections in the genome(s) to be loaded.</param>
        public NeatGenomeLoaderDouble(
            MetaNeatGenome<double> metaNeatGenome,
            int connCountEstimate)
            : base(metaNeatGenome, connCountEstimate)
        {}

        #endregion

        #region Protected Overrides

        protected override bool TryParseWeight(string str, out double weight)
        {
            return double.TryParse(
                str,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture,
                out weight);
        }

        #endregion
    }
}
