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
using System;
using SharpNeat.Utility;

namespace SharpNeat.Network
{
    /// <summary>
    /// Gaussian activation function. Output range is 0 to 1, that is, the tails of the gaussian
    /// distribution curve tend towards 0 as abs(x) -> Infinity and the gaussian's peak is at x = 0.
    /// </summary>
    public class RbfGaussian : IActivationFunction
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
            // auxArgs[1] - RBF gaussian epsilon.
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
            // auxArgs[0] - RBF center.
            // auxArgs[1] - RBF gaussian epsilon.
            float d = (x-auxArgs[0]) * (float)Math.Sqrt(auxArgs[1]) * 4f;
            return (float)Math.Exp(-(d*d));
        }

        /// <summary>
        /// For activation functions that accept auxiliary arguments; generates random initial values for aux arguments for newly
        /// added nodes (from an 'add neuron' mutation).
        /// </summary>
        public double[] GetRandomAuxArgs(FastRandom rng, double connectionWeightRange)
        {
            double[] auxArgs = new double[2];
            auxArgs[0] = (rng.NextDouble()-0.5) * 2.0;
            auxArgs[1] = rng.NextDouble();
            return auxArgs;
        }

        /// <summary>
        /// Genetic mutation for auxiliary argument data.
        /// </summary>
        public void MutateAuxArgs(double[] auxArgs, FastRandom rng, GaussianGenerator gaussianRng, double connectionWeightRange)
        {
            // Mutate center.            
            // Add gaussian ditribution sample and clamp result to +-connectionWeightRange.
            double tmp = auxArgs[0] + gaussianRng.NextDouble(0, _auxArgsMutationSigmaCenter);
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
            // Add gaussian ditribution sample and clamp result to [0,1]
            tmp = auxArgs[1] + gaussianRng.NextDouble(0, _auxArgsMutationSigmaRadius);
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
