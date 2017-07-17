using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.Neat.Genome;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.NeuralNets;
using SharpNeat.Phenomes.NeuralNets.Acyclic;
using SharpNeat.Phenomes.NeuralNets.Cyclic;

namespace SharpNeat.Neat.Decoding
{
    /// <summary>
    /// Decodes NeatGenome's into concrete network instances.
    /// </summary>
    public class NeatGenomeDecoder : IGenomeDecoder<NeatGenome,IBlackBox<double>>
    {
        readonly NetworkActivationScheme _activationScheme;
        delegate IBlackBox<double> DecodeGenome(NeatGenome genome);
        readonly DecodeGenome _decodeMethod;

        #region Constructors

        /// <summary>
        /// Construct the decoder with the network activation scheme to use in decoded networks.
        /// </summary>
        public NeatGenomeDecoder(NetworkActivationScheme activationScheme)
        {
            _activationScheme = activationScheme;

            // Pre-determine which decode routine to use based on the activation scheme.
            _decodeMethod = GetDecodeMethod(activationScheme);
        }

        #endregion

        #region IGenomeDecoder Members

        /// <summary>
        /// Decodes a NeatGenome to a concrete network instance.
        /// </summary>
        public IBlackBox<double> Decode(NeatGenome genome)
        {
            return _decodeMethod(genome);
        }

        #endregion

        #region Private Methods

        private DecodeGenome GetDecodeMethod(NetworkActivationScheme activationScheme)
        {
            if(activationScheme.AcyclicNetwork) {
                return DecodeToAcyclicNetwork;
            }
            return DecodeToCyclicNetwork;
        }

        private AcyclicNetwork DecodeToAcyclicNetwork(NeatGenome genome)
        {
            return AcyclicNetworkFactory.CreateAcyclicNetwork(genome, true);
        }

        private CyclicNetwork DecodeToCyclicNetwork(NeatGenome genome)
        {
            return CyclicNetworkFactory.CreateCyclicNetwork(genome, _activationScheme, true);
        }

        #endregion
    }
}
