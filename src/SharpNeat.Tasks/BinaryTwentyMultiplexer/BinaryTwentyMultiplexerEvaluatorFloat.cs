// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.BinaryTwentyMultiplexer;

/// <summary>
/// Evaluator for the Binary 20-Multiplexer task.
///
/// Four 'address' inputs supply a binary number between 0 and 15; this number selects one of the
/// 16 'data' inputs; resulting in 20 inputs in total (21 with the bias input).
///
/// The correct output response is the binary value of the input to the addressed data input.
/// </summary>
public sealed class BinaryTwentyMultiplexerEvaluatorFloat : IPhenomeEvaluator<IBlackBox<float>>
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
        Span<float> dataInputs = inputs.Slice(5, 16);
        ref float output = ref box.Outputs.Span[0];

        // Bias input.
        inputs[0] = 1f;

        // Loop through the 16 addresses.
        for(int addr=0; addr < 16; addr++)
        {
            // Set the four address inputs.
            inputs[1] = addr & 0x1;
            inputs[2] = (addr >> 1) & 0x1;
            inputs[3] = (addr >> 2) & 0x1;
            inputs[4] = (addr >> 3) & 0x1;

            // Evaluate against 128 test input patterns.
            EvaluateTestInputPatterns(
                box, dataInputs,
                ref output,
                addr,
                ref fitness);
        }

        return new FitnessInfo(fitness);
    }

    private static void EvaluateTestInputPatterns(
        IBlackBox<float> box,
        Span<float> dataInputs,
        ref float output,
        int addr,
        ref float fitness)
    {
        // Allocate storage for the input pattern values.
        Span<float> pattern = stackalloc float[6];

        // Enumerate through the 64 input patterns (every possible combination of 6 bits)
        for(int i=0; i < 64; i++)
        {
            // Copy the first 6 bits of 'i' into the elements of 'pattern'.
            pattern[0] = i & 0x1;
            pattern[1] = (i>>1) & 0x1;
            pattern[2] = (i>>2) & 0x1;
            pattern[3] = (i>>3) & 0x1;
            pattern[4] = (i>>4) & 0x1;
            pattern[5] = (i>>5) & 0x1;

            // Repeat the pattern in the slice before the addressed input.
            Fill(dataInputs.Slice(0, addr), pattern);

            // Set the value of the addressed data input to zero.
            dataInputs[addr] = 0f;

            // Repeat the pattern in the slice after the addressed input.
            Fill(dataInputs.Slice(addr + 1), pattern);

            // Activate the black box to get the response/output value.
            box.Activate();

            // Calculate a fitness for this evaluation instance; and add the result to an accumulating fitness score.
            Clamp(ref output);
            fitness += 1f - (output * output);

            // Now evaluate with the addressed data input set to one.
            dataInputs[addr] = 1f;

            // Activate the black box to get the response/output value.
            box.Activate();

            // Calculate a fitness for this evaluation instance; and add the result to an accumulating fitness score.
            Clamp(ref output);
            fitness += 1f - ((1f - output) * (1f - output));
        }
    }

    private static void Clamp(ref float x)
    {
        if(x < 0f) x = 0f;
        else if(x > 1f) x = 1f;
    }

    // TODO: Consider moving to Redzen.SpanUtils.
    /// <summary>
    /// Fill the elements of <paramref name="span"/> with repeating copies of the values from <paramref name="pattern"/>.
    /// </summary>
    /// <param name="span">The span to fill.</param>
    /// <param name="pattern">The pattern span.</param>
    private static void Fill(
        Span<float> span,
        ReadOnlySpan<float> pattern)
    {
        // Loop over 'pattern' sized slices of the target span, copying the pattern into each slice.
        for(; span.Length >= pattern.Length; span = span.Slice(pattern.Length))
        {
            pattern.CopyTo(span);
        }

        // Handle remaining elements (if any).
        if(span.Length != 0)
        {
            pattern.Slice(0, span.Length).CopyTo(span);
        }
    }
}
