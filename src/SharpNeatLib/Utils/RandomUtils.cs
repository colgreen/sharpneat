using Redzen.Numerics;

namespace SharpNeat.Utils
{
    public static class RandomUtils
    {
        /// <summary>
        /// Sample a new random connection weight.
        /// </summary>
        public static double SampleConnectionWeight(double connectionWeightRange, IRandomSource rng)
        {
            return ((rng.NextDouble()*2.0) - 1.0) * connectionWeightRange;
        }
    }
}
