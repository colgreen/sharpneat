﻿using Xunit;
using static SharpNeat.Tests.ArrayTestUtils;

namespace SharpNeat.NeuralNets.ActivationFunctions;

public class ActivationFunctionOverloadTests
{
    [Fact]
    public void Fn_Overloads()
    {
        Random rng = new();

        // Scan for all implementations of IActivationFunction(double>
        IEnumerable<Type> actFnTypes = FindTypeImplementations(typeof(IActivationFunction<double>));
        foreach(Type type in actFnTypes)
        {
            // Get an instance of the current type.
            var actFn = (IActivationFunction<double>)Activator.CreateInstance(type);

            // Run tests on the instance.
            Fn_Overloads_Inner(actFn, rng);
        }
    }

    #region Private Static Methods

    private static void Fn_Overloads_Inner(IActivationFunction<double> actFn, Random rng)
    {
        // Init an array of random values.
        double[] v = GetRandomArray(1001, rng);

        // Init empty target arrays.
        double[] v_scalar1 = new double[v.Length];
        double[] v_scalar2 = new double[v.Length];
        double[] v_vector = new double[v.Length];
        double[] v_zero = new double[v.Length];

        // Apply the scalar overload of the activation function (single parameter overload).
        v.CopyTo(v_scalar1, 0);
        ApplyFunc(actFn, v_scalar1);

        // Apply the scalar overload of the activation function (two parameter overload).
        ApplyFunc(actFn, v, v_scalar2);
        Assert.Equal(v_scalar1, v_scalar2);

        // Apply vector based overloads.
        // Overload 1.
        v.CopyTo(v_vector, 0);
        actFn.Fn(v_vector);
        Assert.Equal(v_scalar1, v_vector);

        // Overload 2.
        v.CopyTo(v_vector, 0);
        actFn.Fn(v_vector.AsSpan(10..20));
        ConponentwiseEqual(v, v_vector, 0, 10);
        ConponentwiseEqual(v_scalar1, v_vector, 10, 20);
        ConponentwiseEqual(v, v_vector, 20, v.Length);

        // Overload 3.
        Array.Clear(v_vector, 0, v_vector.Length);
        actFn.Fn(v.AsSpan()[10..20], v_vector.AsSpan(10..20));
        ConponentwiseEqual(v_zero, v_vector, 0, 10);
        ConponentwiseEqual(v_scalar1, v_vector, 10, 20);
        ConponentwiseEqual(v_zero, v_vector, 20, v.Length);
    }

    private static void ApplyFunc(IActivationFunction<double> actFn, double[] x)
    {
        for(int i=0; i < x.Length; i++)
        {
            actFn.Fn(ref x[i]);
        }
    }

    private static void ApplyFunc(IActivationFunction<double> actFn, double[] v, double[] w)
    {
        for(int i=0; i < v.Length; i++)
        {
            actFn.Fn(ref v[i], ref w[i]);
        }
    }

    private static double[] GetRandomArray(int length, Random rng)
    {
        double[] v = new double[length];
        for(int i=0; i < v.Length; i++)
        {
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
