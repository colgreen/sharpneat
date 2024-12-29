// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.BinaryElevenMultiplexer;

// TODO: Consider a variant on this evaluator that uses two outputs instead of one, i.e. 'false' and 'true' outputs;
// (if both outputs are low or high then that's just an invalid response).

/// <summary>
/// Evaluator for the Binary 11-Multiplexer task.
///
/// Three inputs supply a binary number between 0 and 7; this number selects one of the
/// further 8 inputs (eleven inputs in total). The correct response is the selected input's
/// input signal (0 or 1).
///
/// Evaluation consists of querying the provided black box for all possible input combinations (2^11 = 2048).
/// </summary>
public sealed class BinaryElevenMultiplexerEvaluatorFloat : IPhenomeEvaluator<IBlackBox<float>>
{
    /// <summary>
    /// Evaluate the provided black box against the Binary 11-Multiplexer task,
    /// and return its fitness score.
    /// </summary>
    /// <param name="box">The black box to evaluate.</param>
    /// <returns>A new instance of <see cref="FitnessInfo"/>.</returns>
    public FitnessInfo Evaluate(IBlackBox<float> box)
    {
        float fitness = 0f;
        Span<float> inputs = box.Inputs.Span;
        ref float output = ref box.Outputs.Span[0];

        // 2048 test cases.
        for(int i=0; i < 2048; i++)
        {
            // Bias input.
            inputs[0] = 1f;

            // Apply bitmask to i and shift left to generate the input signals.
            // Note. We could eliminate all the boolean logic by pre-building a table of test
            // signals and correct responses.
            inputs[1] = i & 0x1;
            inputs[2] = (i>>1) & 0x1;
            inputs[3] = (i>>2) & 0x1;
            inputs[4] = (i>>3) & 0x1;
            inputs[5] = (i>>4) & 0x1;
            inputs[6] = (i>>5) & 0x1;
            inputs[7] = (i>>6) & 0x1;
            inputs[8] = (i>>7) & 0x1;
            inputs[9] = (i>>8) & 0x1;
            inputs[10] = (i>>9) & 0x1;
            inputs[11] = (i>>10) & 0x1;

            // Activate the black box.
            box.Activate();

            // Read output signal.
            Clamp(ref output);

            // Determine the correct answer with somewhat cryptic bit manipulation.
            // The condition is true if the correct answer is true (1.0).
            if(((1 << (3 + (i & 0x7))) &i) != 0)
            {
                // correct answer: true.
                // Assign fitness on sliding scale between 0.0 and 1.0 based on squared error.
                // In tests squared error drove evolution significantly more efficiently in this domain than absolute error.
                // Note. To base fitness on absolute error use: fitness += output;
                fitness += 1f - ((1f - output) * (1f - output));
            }
            else
            {
                // correct answer: false.
                // Assign fitness on sliding scale between 0.0 and 1.0 based on squared error.
                // In tests squared error drove evolution significantly more efficiently in this domain than absolute error.
                // Note. To base fitness on absolute error use: fitness += 1.0-output;
                fitness += 1f - (output * output);
            }

            // Reset black box ready for next test case.
            box.Reset();
        }

        return new FitnessInfo(fitness);
    }

    private static void Clamp(ref float x)
    {
        if(x < 0.0) x = 0f;
        else if(x > 1.0) x = 1f;
    }
}
