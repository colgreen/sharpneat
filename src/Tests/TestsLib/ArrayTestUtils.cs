using SharpNeat.BlackBox;
using Xunit;

namespace SharpNeat.Tests;

public static class ArrayTestUtils
{
    public static void ConponentwiseEqual<T>(T[] expectedArr, T[] actualArr, int startIdx, int endIdx)
    {
        for(int i = startIdx; i < endIdx; i++)
        {
            Assert.Equal(expectedArr[i], actualArr[i]);
        }
    }

    /// <summary>
    /// Returns true if the two arrays are equal.
    /// </summary>
    /// <param name="x">First array.</param>
    /// <param name="y">Second array.</param>
    public static bool ConponentwiseEqual(double[] x, double[] y, double maxdelta)
    {
        // x and y are equal if they are the same reference, or both are null.
        if(x == y)
            return true;

        // Test if one is null and the other not null.
        // Note. We already tested for both being null (above).
        if(x is null || y is null)
            return false;

        if(x.Length != y.Length)
            return false;

        for(int i = 0; i < x.Length; i++)
        {
            double delta = Math.Abs(x[i] - y[i]);

            if(delta > maxdelta)
                return false;
        }

        return true;
    }
}
