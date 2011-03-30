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

namespace SharpNeat.Network
{
    /// <summary>
    /// An approximation of the SteepenedSigmoid activation function. Faster to calculate but anecdotal evidence
    /// suggests using this function gives poorer results than SteepenedSigmoid.
    /// 
    /// The increase in speed may also be in question with more recent hardware developments. E.g. with faster
    /// implementations of an exp function and underlying CPU execution of the code.
    /// </summary>
    public class SteepenedSigmoidApproximation : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new SteepenedSigmoidApproximation();

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
            get { return "A fast approximation of y = 1.0/(1.0 + exp(-4.9*x))"; }
        }

        /// <summary>
        /// Gets a human readable verbose description of the activation function.
        /// </summary>
        public string FunctionDescription
        {
            get { return "An approximation of the SteepenedSigmoid function. Faster to calculate but anecdotal evidence suggests using this function gives poorer results than SteepenedSigmoid.\r\nxrange->[-1,1] yrange->[0,1]"; }
        }

        /// <summary>
        /// Calculates the output value for the specified input value.
        /// </summary>
        public double Calculate(double x)
        {
            const double One = 1.0;
            const double Half = 0.5; 

            // Note. The condition statements here are actually not conducive to speedups in superscaler CPUs.
            // Probably still faster than exp() though.
            if(x < -1.0) {
                return 0.0;
            }
            if(x < 0.0) {
                return (x + One) * (x + One) * Half;
            }
            if(x < One) {
                return One - ((x - One) * (x - One) * Half);
            }
            return One;
        }

        /// <summary>
        /// Calculates the output value for the specified input value with float/single precision.
        /// This single precision overload of Calculate() will be used in neural network code 
        /// that has been specifically written to use floats instead of doubles.
        /// </summary>
        public float Calculate(float x)
        {
            const float One = 1.0f;
            const float Half = 0.5f; 

            if(x < -1.0f) {
                return 0.0f;
            }
            if(x < 0.0f) {
                return (x + One) * (x + One) * Half;
            }
            if(x < One) {
                return One - ((x - One) * (x - One) * Half);
            }
            return One;
        }
    }
}
