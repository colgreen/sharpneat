// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.Xor;

/// <summary>
/// Evaluator for the logical XOR task.
///
/// Two inputs supply the two XOR input values.
///
/// The correct response for the single output is input1 XOR input2.
///
/// Evaluation consists of querying the provided black box for all possible input combinations (2^2 = 4).
/// </summary>
/// <typeparam name="TScalar">Black box input/output data type.</typeparam>
public sealed class XorEvaluator<TScalar> : IPhenomeEvaluator<IBlackBox<TScalar>>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar Half = TScalar.CreateChecked(0.5);
    static readonly TScalar Ten = TScalar.CreateChecked(10.0);

    /// <summary>
    /// Evaluate the provided black box against the logical XOR task,
    /// and return its fitness score.
    /// </summary>
    /// <param name="box">The black box to evaluate.</param>
    /// <returns>A new instance of <see cref="FitnessInfo"/>.</returns>
    public FitnessInfo Evaluate(IBlackBox<TScalar> box)
    {
        TScalar fitness = TScalar.Zero;
        bool success = true;

        // Test case 0, 0.
        TScalar output = Activate(box, TScalar.Zero, TScalar.Zero);
        success &= output <= Half;
        fitness += TScalar.One - (output * output);

        // Test case 1, 1.
        box.Reset();
        output = Activate(box, TScalar.One, TScalar.One);
        success &= output <= Half;
        fitness += TScalar.One - (output * output);

        // Test case 0, 1.
        box.Reset();
        output = Activate(box, TScalar.Zero, TScalar.One);
        success &= output > Half;
        fitness += TScalar.One - ((TScalar.One - output) * (TScalar.One - output));

        // Test case 1, 0.
        box.Reset();
        output = Activate(box, TScalar.One, TScalar.Zero);
        success &= output > Half;
        fitness += TScalar.One - ((TScalar.One - output) * (TScalar.One - output));

        // If all four responses were correct then we add 10 to the fitness.
        if(success)
            fitness += Ten;

        return new FitnessInfo(double.CreateSaturating(fitness));
    }

    private static TScalar Activate(
        IBlackBox<TScalar> box,
        TScalar in1, TScalar in2)
    {
        var inputs = box.Inputs.Span;
        var outputs = box.Outputs.Span;

        // Bias input.
        inputs[0] = TScalar.One;

        // XOR inputs.
        inputs[1] = in1;
        inputs[2] = in2;

        // Activate the black box.
        box.Activate();

        // Read output signal.
        TScalar output = outputs[0];
        Clamp(ref output);
        return output;
    }

    private static void Clamp(ref TScalar x)
    {
        if(x < TScalar.Zero) x = TScalar.Zero;
        else if(x > TScalar.One) x = TScalar.One;
    }
}
