using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redzen.Numerics;
using SharpNeat.Neat;

namespace SharpNeat.Utils
{

    /// <summary>
    /// A factory for creating a source of random connection weights, primarily for creating weights in new random genomes,
    /// and also for 'add connection' mutations.
    /// </summary>
    public static class RandomWeightSourceFactory
    {
        /// <summary>
        /// Create a double precision random connection weight source.
        /// </summary>
        public static Func<double> Create_Double(double weightRange, IRandomSource rng)
        {
            return () => { 
                return ((rng.NextDouble()*2.0) - 1.0) * weightRange;
            };
        }

        /// <summary>
        /// Create a single precision random connection weight source.
        /// </summary>
        public static Func<float> Create_Single(float weightRange, IRandomSource rng)
        {
            return () => { 
                return ((rng.NextFloat()*2f) - 1f) * weightRange;
            };
        }
    }
}
