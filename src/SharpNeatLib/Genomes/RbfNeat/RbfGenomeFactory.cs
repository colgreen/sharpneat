/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Genomes.RbfNeat
{
    /// <summary>
    /// A sub-class of NeatGenomeFactory for creating RBF-Neat genomes.
    /// RBF = Radial Basis Functions.
    /// </summary>
    public class RbfGenomeFactory : NeatGenomeFactory
    {
        #region Constructors

        /// <summary>
        /// Constructs with default NeatGenomeParameters, ID generators initialized to zero and the
        /// provided IActivationFunctionLibrary.
        /// </summary>
        public RbfGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 IActivationFunctionLibrary activationFnLibrary)
            : base(inputNeuronCount, outputNeuronCount, activationFnLibrary)
        {
        }

        /// <summary>
        /// Constructs with the provided IActivationFunctionLibrary and NeatGenomeParameters.
        /// </summary>
        public RbfGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 IActivationFunctionLibrary activationFnLibrary,
                                 NeatGenomeParameters neatGenomeParams)
            : base(inputNeuronCount,outputNeuronCount, activationFnLibrary, neatGenomeParams)
        {
        }

        /// <summary>
        /// Constructs with the provided IActivationFunctionLibrary, NeatGenomeParameters and ID generators.
        /// </summary>
        public RbfGenomeFactory(int inputNeuronCount, int outputNeuronCount,
                                 IActivationFunctionLibrary activationFnLibrary,
                                 NeatGenomeParameters neatGenomeParams,
                                 UInt32IdGenerator genomeIdGenerator, UInt32IdGenerator innovationIdGenerator)
            : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, neatGenomeParams, genomeIdGenerator, innovationIdGenerator)
        {
        }

        #endregion

        #region Public Methods [NeatGenome Specific / RbfNeat Overrides]

        /// <summary>
        /// Override that randomly assigns activation functions to neuron's from an activation function library
        /// based on each library item's selection probability.
        /// </summary>
        public override NeuronGene CreateNeuronGene(uint innovationId, NodeType neuronType)
        {
            switch(neuronType)
            {
                case NodeType.Bias:
                case NodeType.Input:
                {   // Use the ID of the first function. By convention this will be the a sigmoid function in nEAT and RBF-NEAT
                    // but in actual fact bias and input neurons don't use their activation function.
                    int activationFnId = _activationFnLibrary.GetFunctionList()[0].Id;
                    return new NeuronGene(innovationId, neuronType, activationFnId);
                }
                default:
                {
                    ActivationFunctionInfo fnInfo = _activationFnLibrary.GetRandomFunction(_rng);
                    IActivationFunction actFn = fnInfo.ActivationFunction;
                    double[] auxArgs = null;
                    if(actFn.AcceptsAuxArgs) {
                        auxArgs = actFn.GetRandomAuxArgs(_rng, _neatGenomeParamsCurrent.ConnectionWeightRange);
                    }

                    return new NeuronGene(innovationId, neuronType, fnInfo.Id, auxArgs);
                }
            }
        }

        #endregion
    }
}
