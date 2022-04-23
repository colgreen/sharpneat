// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.BinarySixMultiplexer;

// TODO: Consider a variant on this evaluator that uses two outputs instead of one, i.e. 'false' and 'true' outputs;
// (if both outputs are low or high then that's just an invalid response).

/// <summary>
/// Evaluator for the Binary 6-Multiplexer task.
///
/// Two inputs supply a binary number between 0 and 3; this number selects one of the
/// further 4 inputs (six inputs in total). The correct response is the selected input's
/// input signal (0 or 1).
///
/// Evaluation consists of querying the provided black box for all possible input combinations (2^6 = 64).
/// </summary>
public sealed class BinarySixMultiplexerEvaluator : IPhenomeEvaluator<IBlackBox<double>>
{
    #region Public Methods

    /// <summary>
    /// Evaluate the provided black box against the Binary 6-Multiplexer task,
    /// and return its fitness score.
    /// </summary>
    /// <param name="box">The black box to evaluate.</param>
    /// <returns>A new instance of <see cref="FitnessInfo"/>.</returns>
    public FitnessInfo Evaluate(IBlackBox<double> box)
    {
        double fitness = 0.0;
        bool success = true;
        Span<double> inputs = box.Inputs.Span;
        Span<double> outputs = box.Outputs.Span;

        // 64 test cases.
        for(int i=0; i < 64; i++)
        {
            // Bias input.
            inputs[0] = 1.0;

            // Apply bitmask to i and shift left to generate the input signals.
            // Note. We could eliminate all the boolean logic by pre-building a table of test
            // signals and correct responses.
            for(int tmp = i, j=1; j < 7; j++)
            {
                inputs[j] = tmp & 0x1;
                tmp >>= 1;
            }

            // Activate the black box.
            box.Activate();

            // Read output signal.
            double output = outputs[0];
            Clamp(ref output);
            Debug.Assert(output >= 0.0, "Unexpected negative output.");
            bool trueResponse = (output > 0.5);

            // Determine the correct answer with somewhat cryptic bit manipulation.
            // The condition is true if the correct answer is true (1.0).
            if(((1 << (2 + (i & 0x3))) &i) != 0)
            {
                // correct answer: true.
                // Assign fitness on sliding scale between 0.0 and 1.0 based on squared error.
                // In tests squared error drove evolution significantly more efficiently in this domain than absolute error.
                // Note. To base fitness on absolute error use: fitness += output;
                fitness += 1.0 - ((1.0 - output) * (1.0 - output));

                // Reset success flag if at least one response is wrong.
                success &= trueResponse;
            }
            else
            {
                // correct answer: false.
                // Assign fitness on sliding scale between 0.0 and 1.0 based on squared error.
                // In tests squared error drove evolution significantly more efficiently in this domain than absolute error.
                // Note. To base fitness on absolute error use: fitness += 1.0-output;
                fitness += 1.0 - (output * output);

                // Reset success flag if at least one response is wrong.
                success &= !trueResponse;
            }

            // Reset black box ready for next test case.
            box.Reset();
        }

        // If the correct answer was given in each case then add a bonus value to the fitness.
        if(success)
            fitness += 1_000.0;

        return new FitnessInfo(fitness);
    }

    #endregion

    #region Private Static Methods

    private static void Clamp(ref double x)
    {
        if(x < 0.0) x = 0.0;
        else if(x > 1.0) x = 1.0;
    }

    #endregion
}
