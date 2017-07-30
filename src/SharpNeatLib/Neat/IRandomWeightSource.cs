using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat
{
    /// <summary>
    /// A source of random connection weights, primarily for creating weights in new random genomes,
    /// and also for 'add connection' mutations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRandomWeightSource<T>
        where T : struct
    {
        /// <summary>
        /// Gets a new random weight sample.
        /// </summary>
        /// <returns></returns>
        T Sample();
    }
}
