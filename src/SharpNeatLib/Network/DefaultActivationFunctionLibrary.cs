/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
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

using System.Collections.Generic;
using Redzen.Numerics;
using SharpNeat.Utility;

namespace SharpNeat.Network
{
    /// <summary>
    /// Default implementation of an IActivationFunctionLibrary. 
    /// Also provides static factory methods to create libraries with commonly used activation functions.
    /// </summary>
    public class DefaultActivationFunctionLibrary : IActivationFunctionLibrary
    {
        readonly IList<ActivationFunctionInfo> _functionList;
        readonly Dictionary<int,IActivationFunction> _functionDict;
        readonly DiscreteDistribution _rwl;

        #region Constructor

        /// <summary>
        /// Constructs an activation function library with the provided list of activation functions.
        /// </summary>
        public DefaultActivationFunctionLibrary(IList<ActivationFunctionInfo> fnList)
        {
            // Build a RouletteWheelLayout based on the selection probability on each item.
            int count = fnList.Count;
            double[] probabilities = new double[count];
            for(int i=0; i<count; i++) {
                probabilities[i] = fnList[i].SelectionProbability;
            }
            _rwl = new DiscreteDistribution(probabilities);
            _functionList = fnList;

            // Build a dictionary of functions keyed on integer ID.
            _functionDict = CreateFunctionDictionary(_functionList);
        }

        #endregion

        #region IActivationFunctionLibrary Members

        /// <summary>
        /// Gets the function with the specified integer ID.
        /// </summary>
        public IActivationFunction GetFunction(int id)
        {
            return _functionDict[id];
        }

        /// <summary>
        /// Randomly select a function based on each function's selection probability.
        /// </summary>
        public ActivationFunctionInfo GetRandomFunction(XorShiftRandom rng)
        {
            return _functionList[DiscreteDistributionUtils.Sample(_rwl, rng)];
        }

        /// <summary>
        /// Gets a list of all functions in the library.
        /// </summary>
        public IList<ActivationFunctionInfo> GetFunctionList()
        {
            return _functionList;
        }

        #endregion

        #region Private Methods

        private static Dictionary<int,IActivationFunction> CreateFunctionDictionary(IList<ActivationFunctionInfo> fnList)
        {
            Dictionary<int,IActivationFunction> dict = new Dictionary<int,IActivationFunction>(fnList.Count);
            foreach(ActivationFunctionInfo fnInfo in fnList) {
                dict.Add(fnInfo.Id, fnInfo.ActivationFunction);
            }
            return dict;
        }

        #endregion

        #region Public Static Factory Methods

        /// <summary>
        /// Create an IActivationFunctionLibrary for use with NEAT.
        /// NEAT uses the same activation function for all neurons/nodes therefore this factory method
        /// creates an IActivationFunction containing only the single provided IActivationFunction.
        /// </summary>
        public static IActivationFunctionLibrary CreateLibraryNeat(IActivationFunction activationFn)
        {
            List<ActivationFunctionInfo> fnList = new List<ActivationFunctionInfo>(1);
            fnList.Add(new ActivationFunctionInfo(0, 1.0, activationFn));
            return new DefaultActivationFunctionLibrary(fnList);
        }

        /// <summary>
        /// Create an IActivationFunctionLibrary for use with CPPNs.
        /// </summary>
        public static IActivationFunctionLibrary CreateLibraryCppn()
        {
            List<ActivationFunctionInfo> fnList = new List<ActivationFunctionInfo>(4);
            fnList.Add(new ActivationFunctionInfo(0, 0.25, Linear.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(1, 0.25, BipolarSigmoid.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(2, 0.25, Gaussian.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(3, 0.25, Sine.__DefaultInstance));
            return new DefaultActivationFunctionLibrary(fnList);
        }

        /// <summary>
        /// Create an IActivationFunctionLibrary for use with Radial Basis Function NEAT.
        /// </summary>
        public static IActivationFunctionLibrary CreateLibraryRbf(IActivationFunction activationFn, double auxArgsMutationSigmaCenter, double auxArgsMutationSigmaRadius)
        {
            List<ActivationFunctionInfo> fnList = new List<ActivationFunctionInfo>(2);
            fnList.Add(new ActivationFunctionInfo(0, 0.8, activationFn));
            fnList.Add(new ActivationFunctionInfo(1, 0.2, new RbfGaussian(auxArgsMutationSigmaCenter, auxArgsMutationSigmaRadius)));
            return new DefaultActivationFunctionLibrary(fnList);
        }

        #endregion
    }
}
