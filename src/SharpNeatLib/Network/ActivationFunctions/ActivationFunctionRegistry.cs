/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * Written by Scott DeBoer, 2019 (naigonakoii@gmail.com)
 * 
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;

namespace SharpNeat.Network
{
    public static class ActivationFunctionRegistry
    {
        // The default set of activation functions.
        private static Dictionary<string, IActivationFunction> __activationFunctionTable = new Dictionary<string, IActivationFunction>()
        {
            // Bipolar.
            { "BipolarSigmoid", BipolarSigmoid.__DefaultInstance },
            { "BipolarGaussian", BipolarGaussian.__DefaultInstance },
            { "Linear", Linear.__DefaultInstance },
            { "Sine", Sine.__DefaultInstance },

            // Unipolar.
            { "ArcSinH", ArcSinH.__DefaultInstance },
            { "ArcTan", ArcTan.__DefaultInstance },
            { "Gaussian", Gaussian.__DefaultInstance },
            { "LeakyReLU", LeakyReLU.__DefaultInstance },
            { "LeakyReLUShifted", LeakyReLUShifted.__DefaultInstance },
            { "LogisticFunction", LogisticFunction.__DefaultInstance },
            { "LogisticFunctionSteep", LogisticFunctionSteep.__DefaultInstance },
            { "MaxMinusOne", MaxMinusOne.__DefaultInstance },
            { "PolynomialApproximantSteep", PolynomialApproximantSteep.__DefaultInstance },
            { "QuadraticSigmoid", QuadraticSigmoid.__DefaultInstance },
            { "ReLU", ReLU.__DefaultInstance },
            { "ScaledELU", ScaledELU.__DefaultInstance },
            { "SoftSignSteep", SoftSignSteep.__DefaultInstance },
            { "SReLU", SReLU.__DefaultInstance },
            { "SReLUShifted", SReLUShifted.__DefaultInstance },
            { "TanH", TanH.__DefaultInstance },

            // Radial Basis.
            { "RbfGaussian", RbfGaussian.__DefaultInstance },
        };

        #region Public Static Methods

        /// <summary>
        /// Registers a custom activation function in addition to those in the default activation function table.
        /// Alows loading of neural nets from XML that use custom activation functions.
        /// </summary>
        public static void RegisterActivationFunction(IActivationFunction function)
        {
            if(!__activationFunctionTable.ContainsKey(function.FunctionId)) {
                __activationFunctionTable.Add(function.FunctionId, function);
            }
        }

        /// <summary>
        /// Gets an IActivationFunction with the given short name.
        /// </summary>
        public static IActivationFunction GetActivationFunction(string name)
        {
            if(!__activationFunctionTable.ContainsKey(name)) {
                throw new ArgumentException($"Unexpected activation function [{name}]");
            }
            return __activationFunctionTable[name];
        }

        #endregion
    }
}
