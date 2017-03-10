/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Decoders.Neat
{
    /// <summary>
    /// Decodes NeatGenome's into concrete network instances.
    /// </summary>
    public class NeatGenomeDecoder : IGenomeDecoder<NeatGenome,IBlackBox>
    {
        readonly NetworkActivationScheme _activationScheme;
        delegate IBlackBox DecodeGenome(NeatGenome genome);
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
        public IBlackBox Decode(NeatGenome genome)
        {
            return _decodeMethod(genome);
        }

        #endregion

        #region Private Methods

        private DecodeGenome GetDecodeMethod(NetworkActivationScheme activationScheme)
        {
            if(activationScheme.AcyclicNetwork) {
                return DecodeToFastAcyclicNetwork;
            }

            if(activationScheme.FastFlag) {
                return DecodeToFastCyclicNetwork;
            }
            return DecodeToCyclicNetwork;
        }

        private FastAcyclicNetwork DecodeToFastAcyclicNetwork(NeatGenome genome)
        {
            return FastAcyclicNetworkFactory.CreateFastAcyclicNetwork(genome);
        }

        private CyclicNetwork DecodeToCyclicNetwork(NeatGenome genome)
        {
            return CyclicNetworkFactory.CreateCyclicNetwork(genome, _activationScheme);
        }

        private FastCyclicNetwork DecodeToFastCyclicNetwork(NeatGenome genome)
        {
            return FastCyclicNetworkFactory.CreateFastCyclicNetwork(genome, _activationScheme);
        }

        #endregion
    }
}
