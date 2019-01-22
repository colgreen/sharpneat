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
using System;
using Redzen.Numerics.Distributions.Double;
using Redzen.Random;

namespace SharpNeat.Network
{
    /// <summary>
    /// Gaussian activation function. Output range is 0 to 1, that is, the tails of the Gaussian
    /// distribution curve tend towards 0 as abs(x) -> Infinity and the Gaussian peak is at x = 0.
    /// </summary>
    public sealed class RbfGaussian : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new RbfGaussian(0.1, 0.1);

        double _auxArgsMutationSigmaCenter;
        double _auxArgsMutationSigmaRadius;

        #region Constructor

        /// <summary>
        /// Construct with the specified radial basis function auxiliary arguments.
        /// </summary>
        /// <param name="auxArgsMutationSigmaCenter">Radial basis function center.</param>
        /// <param name="auxArgsMutationSigmaRadius">Radial basis function radius.</param>
        public RbfGaussian(double auxArgsMutationSigmaCenter, double auxArgsMutationSigmaRadius)
        {
            _auxArgsMutationSigmaCenter = auxArgsMutationSigmaCenter;
            _auxArgsMutationSigmaRadius = auxArgsMutationSigmaRadius;
        }

        #endregion

        /// <summary>
        /// Gets the unique ID of the function. Stored in network XML to identify which function a network or neuron 
        /// is using.
        /// </summary>
        public string FunctionId
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// Gets a human readable string representation of the function. E.g 'y=1/x'.
        /// </summary>
        public string FunctionString
        {
            get { return "y = e^(-((x-center)*epsilon)^2)"; }
        }

        /// <summary>
        /// Gets a human readable verbose description of the activation function.
        /// </summary>
        public string FunctionDescription
        {
            get { return "Gaussian.\r\nEffective yrange->[0,1]"; }
        }

        /// <summary>
        /// Gets a flag that indicates if the activation function accepts auxiliary arguments.
        /// </summary>
        public bool AcceptsAuxArgs 
        { 
            get { return true; }
        } 

        /// <summary>
        /// Calculates the output value for the specified input value and activation function auxiliary arguments.
        /// </summary>
        public double Calculate(double x, double[] auxArgs)
        {
            // auxArgs[0] - RBF center.
            // auxArgs[1] - RBF Gaussian epsilon.
            double d = (x-auxArgs[0]) * Math.Sqrt(auxArgs[1]) * 4.0;
            return Math.Exp(-(d*d));
        }

        /// <summary>
        /// Calculates the output value for the specified input value and activation function auxiliary arguments.
        /// This single precision overload of Calculate() will be used in neural network code 
        /// that has been specifically written to use floats instead of doubles.
        /// </summary>
        public float Calculate(float x, float[] auxArgs)
        {
            // auxArgs[0] - RBF centre.
            // auxArgs[1] - RBF Gaussian epsilon.
            float d = (x-auxArgs[0]) * (float)Math.Sqrt(auxArgs[1]) * 4f;
            return (float)Math.Exp(-(d*d));
        }

        /// <summary>
        /// For activation functions that accept auxiliary arguments; generates random initial values for aux arguments for newly
        /// added nodes (from an 'add neuron' mutation).
        /// </summary>
        public double[] GetRandomAuxArgs(IRandomSource rng, double connectionWeightRange)
        {
            double[] auxArgs = new double[2];
            auxArgs[0] = (rng.NextDouble()-0.5) * 2.0;
            auxArgs[1] = rng.NextDouble();
            return auxArgs;
        }

        /// <summary>
        /// Genetic mutation for auxiliary argument data.
        /// </summary>
        public void MutateAuxArgs(double[] auxArgs, IRandomSource rng, double connectionWeightRange)
        {
            // Mutate centre.            
            // Add Gaussian distribution sample and clamp result to +-connectionWeightRange.
            double tmp = auxArgs[0] + ZigguratGaussian.Sample(rng, 0, _auxArgsMutationSigmaCenter);
            if(tmp < -connectionWeightRange) {
                auxArgs[0] = -connectionWeightRange;
            }
            else if(tmp > connectionWeightRange) {
                auxArgs[0] = connectionWeightRange;
            } 
            else {
                auxArgs[0] = tmp;
            }

            // Mutate radius.
            // Add Gaussian distribution sample and clamp result to [0,1]
            tmp = auxArgs[1] + ZigguratGaussian.Sample(rng, 0, _auxArgsMutationSigmaCenter);
            if(tmp < 0.0) {
                auxArgs[1] = 0.0;
            }
            else if(tmp > 1.0) {
                auxArgs[1] = 1.0;
            }
            else {
                auxArgs[1] = tmp;
            }
        }
    }
}
