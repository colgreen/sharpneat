using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.NeuralNet;
using static SharpNeat.Tests.ArrayTestUtils;

namespace SharpNeat.Tests.NeuralNets.Double.ActivationFunctions
{
    [TestClass]
    public class ActivationFunctionOverloadTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("ActivationFunctions-Overloads")]
        public void TestOverloads()
        {
            Random rng = new Random();

            // Scan for all implementations of IActivationFunction(double>
            IEnumerable<Type> actFnTypes = FindTypeImplementations(typeof(IActivationFunction<double>));
            foreach(Type type in actFnTypes)
            {
                // Get an instance of the current type.
                var actFn = (IActivationFunction<double>)Activator.CreateInstance(type);

                // Run tests on the instance.
                TestOverloads(actFn, rng);
            }
        }

        #endregion

        #region Private Static Methods

        private static void TestOverloads(IActivationFunction<double> actFn, Random rng)
        {
            // Init an array of random values.
            double[] v = GetRandomArray(1001, rng);

            // Init empty target arrays.
            double[] v_scalar = new double[v.Length];
            double[] v_vector = new double[v.Length];
            double[] v_zero = new double[v.Length];

            // Apply the scalar overload (single input) of the activation function.
            ApplyFunc(actFn.Fn, v, v_scalar);

            // Apply vector based overloads.
            // Overload 1.
            Array.Copy(v, v_vector, v.Length);
            actFn.Fn(v_vector);
            Compare(v_scalar, v_vector);

            // Overload 2.
            Array.Copy(v, v_vector, v.Length);
            actFn.Fn(v_vector, 10, 20);
            Compare(v, v_vector, 0, 10);
            Compare(v_scalar, v_vector, 10, 20);
            Compare(v, v_vector, 20, v.Length);
            
            // Overload 3.
            Array.Clear(v_vector, 0, v_vector.Length);
            actFn.Fn(v, v_vector, 10, 20);
            Compare(v_zero, v_vector, 0, 10);
            Compare(v_scalar, v_vector, 10, 20);
            Compare(v_zero, v_vector, 20, v.Length);
        }

        private static void ApplyFunc(Func<double,double> fn, double[] v, double[] w)
        {
            for(int i=0; i < v.Length; i++) {
                w[i] = fn(v[i]);
            }
        }

        private static double[] GetRandomArray(int length, Random rng)
        {
            double[] v = new double[length];
            for(int i=0; i < v.Length; i++) {
                v[i] = (rng.NextDouble() * 20.0) - 10.0;
            }
            return v;
        }

        private static IEnumerable<Type> FindTypeImplementations(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
        }

        #endregion
    }
}
