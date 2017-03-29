using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// Working data struct for use in FastCyclicNetwork and sub-classes.
    /// Represents a single connection - its weight and source/target neurons.
    /// </summary>
    public struct FastConnection
    {
        public int _srcNeuronIdx;
        public int _tgtNeuronIdx;
        public double _weight;
    }
}
