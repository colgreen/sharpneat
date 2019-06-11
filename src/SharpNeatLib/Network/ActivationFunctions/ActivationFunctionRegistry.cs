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
namespace SharpNeat.Network
{
    using System.Collections.Generic;

    public static class ActivationFunctionRegistry
    {
        #region Static Properties
        internal static Dictionary<string, IActivationFunction> _registeredActivationFunctions = new Dictionary<string, IActivationFunction>()
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

            // Radial Bias.
            { "RbfGaussian", RbfGaussian.__DefaultInstance },
        };

        #endregion // Static Properties

        #region Static Public Methods
        /// <summary>
        /// Registers a custom activation function not implemented by the framework that can be loaded via XML later.
        /// </summary>
        public static void RegisterActivationFunction(IActivationFunction function)
        {
            if (!_registeredActivationFunctions.ContainsKey(function.FunctionId))
            {
                _registeredActivationFunctions.Add(function.FunctionId, function);
            }
        }
        #endregion // Static Public Methods
    }
}
