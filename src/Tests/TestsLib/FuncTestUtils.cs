namespace SharpNeat.Tests;

public static class FuncTestUtils
{
    /// <summary>
    /// Function monotonicity test.
    /// </summary>
    public static bool IsMonotonicIncreasing(Func<double,double> fn, double min, double max, double incr, bool strict)
    {
        double y_prev = fn(min);

        if(strict)
        {
            // Strictly monotonic test, i.e. must be increasing and not unchanged.
            for(double x = min + incr; x <= max; x += incr)
            {
                double y = fn(x);
                if(y <= y_prev)
                    return false;

                y_prev = y;
            }
        }
        else
        {
            for(double x = min + incr; x <= max; x += incr)
            {
                double y = fn(x);
                if(y < y_prev)
                    return false;

                y_prev = y;
            }
        }

        return true;
    }
}
