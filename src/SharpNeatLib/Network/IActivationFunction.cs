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

using Redzen.Numerics;
using SharpNeat.Utility;

namespace SharpNeat.Network
{
    /// <summary>
    /// Interface for neural network activation functions. An activation function simply takes a single input 
    /// value and produces a single output value. IActivationFunction allows for activation functions to be 
    /// plugged in to neural network implementations. Typical activation functions would be a sigmoid or step 
    /// function.
    /// 
    /// The Calculate method has two overloads, one for each of the data types double and float, this allows
    /// an IActivationFunction object to be plugged in to neural network classes that are written to operate
    /// with either of those two data types. Typically the choice of which neural network implementation and
    /// floating point precision to use is part of the setting up and design of a problem domain and experiment.
    /// For some problem domains the extra precision of a double may be unnecessary.
    /// </summary>
    public interface IActivationFunction
    {
        /// <summary>
        /// Gets the unique ID of the function. Stored in network XML to identify which function a network or neuron 
        /// is using.
        /// </summary>
        string FunctionId { get; }

        /// <summary>
        /// Gets a human readable string representation of the function. E.g 'y=1/x'.
        /// </summary>
        string FunctionString { get; }

        /// <summary>
        /// Gets a human readable verbose description of the activation function.
        /// </summary>
        string FunctionDescription { get; }

        /// <summary>
        /// Gets a flag that indicates if the activation function accepts auxiliary arguments.
        /// </summary>
        bool AcceptsAuxArgs { get; } 

        /// <summary>
        /// Calculates the output value for the specified input value and optional activation function auxiliary arguments.
        /// </summary>
        double Calculate(double x, double[] auxArgs);

        /// <summary>
        /// Calculates the output value for the specified input value and optional activation function auxiliary arguments.
        /// This single precision overload of Calculate() will be used in neural network code 
        /// that has been specifically written to use floats instead of doubles.
        /// </summary>
        float Calculate(float x, float[] auxArgs);

        /// <summary>
        /// For activation functions that accept auxiliary arguments; generates random initial values for aux arguments for newly
        /// added nodes (from an 'add neuron' mutation).
        /// </summary>
        double[] GetRandomAuxArgs(XorShiftRandom rng, double connectionWeightRange);

        /// <summary>
        /// Genetic mutation for auxiliary argument data.
        /// </summary>
        void MutateAuxArgs(double[] auxArgs, XorShiftRandom rng, ZigguratGaussianSampler gaussianSampler, double connectionWeightRange);
    }
}
