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

using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Genomes.HyperNeat
{
    /// <summary>
    /// A sub-class of NeatGenomeFactory for creating CPPN genomes.
    /// </summary>
    public class CppnGenomeFactory : NeatGenomeFactory
    {
        #region Constructors

        /// <summary>
        /// Constructs with default NeatGenomeParameters, ID generators initialized to zero and a
        /// default IActivationFunctionLibrary.
        /// </summary>
        public CppnGenomeFactory(int inputNeuronCount, int outputNeuronCount)
            : base(inputNeuronCount, outputNeuronCount, DefaultActivationFunctionLibrary.CreateLibraryCppn())
        {
        }

        /// <summary>
        /// Constructs with default NeatGenomeParameters, ID generators initialized to zero and the
        /// provided IActivationFunctionLibrary.
        /// </summary>
        public CppnGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 IActivationFunctionLibrary activationFnLibrary)
            : base(inputNeuronCount, outputNeuronCount, activationFnLibrary)
        {
        }

        /// <summary>
        /// Constructs with the provided IActivationFunctionLibrary and NeatGenomeParameters.
        /// </summary>
        public CppnGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 IActivationFunctionLibrary activationFnLibrary,
                                 NeatGenomeParameters neatGenomeParams)
            : base(inputNeuronCount,outputNeuronCount, activationFnLibrary, neatGenomeParams)
        {
        }

        /// <summary>
        /// Constructs with the provided IActivationFunctionLibrary, NeatGenomeParameters and ID generators.
        /// </summary>
        public CppnGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 IActivationFunctionLibrary activationFnLibrary,
                                 NeatGenomeParameters neatGenomeParams,
                                 UInt32IdGenerator genomeIdGenerator, UInt32IdGenerator innovationIdGenerator)
            : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, neatGenomeParams, genomeIdGenerator, innovationIdGenerator)
        {
        }

        #endregion

        #region Public Methods [NeatGenome Specific / CPPN Overrides]

        /// <summary>
        /// Override that randomly assigns activation functions to neuron's from an activation function library
        /// based on each library item's selection probability.
        /// </summary>
        public override NeuronGene CreateNeuronGene(uint innovationId, NodeType neuronType)
        {
            int activationFnId;
            switch(neuronType)
            {
                case NodeType.Bias:
                case NodeType.Input:
                case NodeType.Output:
                {   // Use the ID of the first function. By convention this will be the Linear function but in actual 
                    // fact bias and input neurons don't use their activation function.
                    activationFnId = _activationFnLibrary.GetFunctionList()[0].Id;
                    break;
                }
                default:
                {
                    activationFnId = _activationFnLibrary.GetRandomFunction().Id;
                    break;
                }
            }

            return new NeuronGene(innovationId, neuronType, activationFnId);
        }

        #endregion
    }
}
