// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.BinaryThreeMultiplexer;

// TODO: Consider a variant on this evaluator that uses two outputs instead of one, i.e. 'false' and 'true' outputs;
// (if both outputs are low or high then that's just an invalid response).

/// <summary>
/// Evaluator for the Binary 3-Multiplexer task.
///
/// One binary input selects which of two other binary inputs to output.
/// The correct response is the selected input's input signal (0 or 1).
///
/// Evaluation consists of querying the provided black box for all possible input combinations (2^3 = 8).
/// </summary>
/// <typeparam name="TScalar">Black box input/output data type.</typeparam>
public sealed class BinaryThreeMultiplexerEvaluator<TScalar> : IPhenomeEvaluator<IBlackBox<TScalar>>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar Half = TScalar.CreateChecked(0.5);
    static readonly TScalar Hundred = TScalar.CreateChecked(100.0);

    /// <summary>
    /// Evaluate the provided black box against the Binary 3-Multiplexer task,
    /// and return its fitness score.
    /// </summary>
    /// <param name="box">The black box to evaluate.</param>
    /// <returns>A new instance of <see cref="FitnessInfo"/>.</returns>
    public FitnessInfo Evaluate(IBlackBox<TScalar> box)
    {
        TScalar fitness = TScalar.Zero;
        bool success = true;
        Span<TScalar> inputs = box.Inputs.Span;
        Span<TScalar> outputs = box.Outputs.Span;

        // 8 test cases.
        for(int i=0; i < 8; i++)
        {
            // Bias input.
            inputs[0] = TScalar.One;

            // Apply bitmask to i and shift left to generate the input signals.
            // Note. We could eliminate all the boolean logic by pre-building a table of test
            // signals and correct responses.
            for(int tmp = i, j=1; j < 4; j++)
            {
                inputs[j] = TScalar.CreateSaturating(tmp & 0x1);
                tmp >>= 1;
            }

            // Activate the black box.
            box.Activate();

            // Read output signal.
            TScalar output = outputs[0];
            Clamp(ref output);
            bool trueResponse = (output > Half);

            // Determine the correct answer with somewhat cryptic bit manipulation.
            // The condition is true if the correct answer is true (1.0).
            if(((1 << (1 + (i & 0x1))) &i) != 0)
            {
                // correct answer: true.
                // Assign fitness on sliding scale between 0.0 and 1.0 based on squared error.
                // In tests squared error drove evolution significantly more efficiently in this domain than absolute error.
                // Note. To base fitness on absolute error use: fitness += output;
                fitness += TScalar.One - ((TScalar.One - output) * (TScalar.One - output));

                // Reset success flag if at least one response is wrong.
                success &= trueResponse;
            }
            else
            {
                // correct answer: false.
                // Assign fitness on sliding scale between 0.0 and 1.0 based on squared error.
                // In tests squared error drove evolution significantly more efficiently in this domain than absolute error.
                // Note. To base fitness on absolute error use: fitness += 1.0-output;
                fitness += TScalar.One - (output * output);

                // Reset success flag if at least one response is wrong.
                success &= !trueResponse;
            }

            // Reset black box ready for next test case.
            box.Reset();
        }

        // If the correct answer was given in each case then add a bonus value to the fitness.
        if(success)
            fitness += Hundred;

        return new FitnessInfo(double.CreateSaturating(fitness));
    }

    private static void Clamp(ref TScalar x)
    {
        if(x < TScalar.Zero) x = TScalar.Zero;
        else if(x > TScalar.One) x = TScalar.One;
    }
}
