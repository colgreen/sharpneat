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
    public class Gaussian : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new Gaussian();

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
            get { return "y = e^(-(x*2.5)^2)"; }
        }

        /// <summary>
        /// Gets a human readable verbose description of the activation function.
        /// </summary>
        public string FunctionDescription
        {
            get { return "Gaussian.\r\nEffective xrange->[-1,1] yrange->[0,1]"; }
        }

        /// <summary>
        /// Gets a flag that indicates if the activation function accepts auxiliary arguments.
        /// </summary>
        public bool AcceptsAuxArgs 
        { 
            get { return false; }
        } 

        /// <summary>
        /// Calculates the output value for the specified input value and optional activation function auxiliary arguments.
        /// </summary>
        public double Calculate(double x, double[] auxArgs)
        {
            return Math.Exp(-Math.Pow(x * 2.5, 2.0));
        }

        /// <summary>
        /// Calculates the output value for the specified input value and optional activation function auxiliary arguments.
        /// This single precision overload of Calculate() will be used in neural network code 
        /// that has been specifically written to use floats instead of doubles.
        /// </summary>
        public float Calculate(float x, float[] auxArgs)
        {
            return (float)Math.Exp(-Math.Pow(x * 2.5f, 2.0));
        }

        /// <summary>
        /// For activation functions that accept auxiliary arguments; generates random initial values for aux arguments for newly
        /// added nodes (from an 'add neuron' mutation).
        /// </summary>
        public double[] GetRandomAuxArgs(FastRandom rng, double connectionWeightRange)
        {
            throw new SharpNeatException("GetRandomAuxArgs() called on activation function that does not use auxiliary arguments.");
        }

        /// <summary>
        /// Genetic mutation for auxiliary argument data.
        /// </summary>
        public void MutateAuxArgs(double[] auxArgs, FastRandom rng, GaussianGenerator gaussianRng, double connectionWeightRange)
        {
            throw new SharpNeatException("MutateAuxArgs() called on activation function that does not use auxiliary arguments.");
        }
    }
}
