using System.Globalization;

namespace SharpNeat.Neat.Genome.IO
{
    public static class NeatGenomeLoaderFactory
    {
        #region Public Static Methods

        public static NeatGenomeLoader<double> CreateLoaderDouble(
            MetaNeatGenome<double> metaNeatGenome)
        {
            return new NeatGenomeLoader<double>(metaNeatGenome, TryParseWeight);
        }

        #endregion

        #region Private Static Methods

        private static (double,bool) TryParseWeight(string str)
        {
            (double, bool) result;
            result.Item2 = double.TryParse(
                str,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture,
                out result.Item1);

            return result;
        }

        #endregion
    }
}
