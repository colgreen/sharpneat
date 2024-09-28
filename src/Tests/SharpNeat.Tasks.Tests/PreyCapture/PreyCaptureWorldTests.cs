using System.Reflection;
using Xunit;

namespace SharpNeat.Tasks.PreyCapture;

public class PreyCaptureWorldTests
{
    [Fact]
    public void MoveAgent()
    {
        var world = new PreyCaptureWorld(4, 1f, 4f, 100);
        using var agent = new MockPreyCaptureAgent();

        // Agent moving north test.
        {
            world.InitPositions();
            Int32Point posBefore = world.AgentPosition;
            var outputs = agent.Outputs.Span;
            outputs.Clear();
            outputs[0] = 1.0;
            world.MoveAgent(agent);
            Int32Point posDelta = world.AgentPosition - posBefore;
            Assert.Equal(new Int32Point(0, 1), posDelta);
        }

        // Agent moving east test.
        {
            world.InitPositions();
            Int32Point posBefore = world.AgentPosition;
            var outputs = agent.Outputs.Span;
            outputs.Clear();
            outputs[1] = 1.0;
            world.MoveAgent(agent);
            Int32Point posDelta = world.AgentPosition - posBefore;
            Assert.Equal(new Int32Point(1, 0), posDelta);
        }

        // Agent moving south test.
        {
            world.InitPositions();
            Int32Point posBefore = world.AgentPosition;
            var outputs = agent.Outputs.Span;
            outputs.Clear();
            outputs[2] = 1.0;
            world.MoveAgent(agent);
            Int32Point posDelta = world.AgentPosition - posBefore;
            Assert.Equal(new Int32Point(0, -1), posDelta);
        }

        // Agent moving west test.
        {
            world.InitPositions();
            Int32Point posBefore = world.AgentPosition;
            var outputs = agent.Outputs.Span;
            outputs.Clear();
            outputs[3] = 1.0;
            world.MoveAgent(agent);
            Int32Point posDelta = world.AgentPosition - posBefore;
            Assert.Equal(new Int32Point(-1, 0), posDelta);
        }
    }

    [Fact]
    public void Atan2LookupTable()
    {
        // Use reflection to extract private static fields from the PreyCaptureWorld class; this is not ideal, but preferable
        // to making fields public that have no reason to be other than for unit testing.
        FieldInfo gridSizeFieldInfo = typeof(PreyCaptureWorld).GetField("__gridSize", BindingFlags.Static | BindingFlags.NonPublic);
        int gridSize = (int)gridSizeFieldInfo.GetValue(null);

        FieldInfo atan2LookupOffsetFieldInfo = typeof(PreyCaptureWorld).GetField("__atan2LookupOffset", BindingFlags.Static | BindingFlags.NonPublic);
        int atan2LookupOffset = (int)atan2LookupOffsetFieldInfo.GetValue(null);

        FieldInfo atan2LookupFieldInfo = typeof(PreyCaptureWorld).GetField("__atan2Lookup", BindingFlags.Static | BindingFlags.NonPublic);
        float[,] atan2Lookup = (float[,])atan2LookupFieldInfo.GetValue(null);


        // Iterate over all possible relative grid coordinates, and compare the cached atan2 result with the value returned by MathF.Atan2().
        for(int x = -(gridSize-1); x < gridSize; x++)
        {
            for(int y = -(gridSize-1); y < gridSize; y++)
            {
                Assert.Equal(MathF.Atan2(y, x), atan2Lookup[y + atan2LookupOffset, x + atan2LookupOffset]);
            }
        }
    }

    [Fact]
    public void AngleDelta()
    {
        // Use reflection to call the CartesianToPolar() method from the PreyCaptureWorld class; this is not ideal, but preferable
        // to making methods internal or public that have no reason to be other than for unit testing.
        MethodInfo methodInfo = typeof(PreyCaptureWorld).GetMethod("AngleDelta", BindingFlags.Static | BindingFlags.NonPublic);

        // Define a local function that calls on the PreyCaptureWorld.AngleDelta() via reflection.
        float angleDelta(float a, float b)
        {
            return (float)methodInfo.Invoke(null, new object[] { a, b });
        }

        // Loop over angle A test values.
        for(float a = 0f; a < 2*MathF.PI; a += 0.1f)
        {
            // Angle delta between same angle should always be zero.
            Assert.Equal(0f, angleDelta(a, a));

            // Angle delta between a and a + PI should always be PI (but with some error caused by limited floating point precision).
            float err = Math.Abs(angleDelta(a, a + MathF.PI) - MathF.PI);
            Assert.True(err < 0.0001);

            // a + PI/4
            err = Math.Abs(angleDelta(a, a + MathF.PI/4f) - MathF.PI/4f);
            Assert.True(err < 0.0001);

            // a - PI/4
            err = Math.Abs(angleDelta(a, a - MathF.PI/4f) - MathF.PI/4f);
            Assert.True(err < 0.0001);

            // a + PI/2
            err = Math.Abs(angleDelta(a, a + MathF.PI/2f) - MathF.PI/2f);
            Assert.True(err < 0.0001);

            // a - PI/2
            err = Math.Abs(angleDelta(a, a - MathF.PI/2f) - MathF.PI/2f);
            Assert.True(err < 0.0001);

            // a + PI
            err = Math.Abs(angleDelta(a, a + MathF.PI) - MathF.PI);
            Assert.True(err < 0.0001);

            // a - PI
            err = Math.Abs(angleDelta(a, a - MathF.PI) - MathF.PI);
            Assert.True(err < 0.0001);

            // a + (5/4)*PI (expected delta is 90 degrees)
            err = Math.Abs(angleDelta(a, a + MathF.PI*(5f/4f)) - MathF.PI*(3f/4f));
            Assert.True(err < 0.0001);

            // a + (3/2)*PI (expected delta is 90 degrees)
            err = Math.Abs(angleDelta(a, a + MathF.PI*(3f/2f)) - MathF.PI*(1f/2f));
            Assert.True(err < 0.0001);

            // a + 2*PI (expected delta is 0)
            err = Math.Abs(angleDelta(a, a + MathF.PI*2f));
            Assert.True(err < 0.0001);
        }
    }

    [Fact]
    public void CartesianToPolar()
    {
        // Use reflection to call the CartesianToPolar() method from the PreyCaptureWorld class; this is not ideal, but preferable
        // to making methods internal or public that have no reason to be other than for unit testing.
        MethodInfo methodInfo = typeof(PreyCaptureWorld).GetMethod("CartesianToPolar", BindingFlags.Static | BindingFlags.NonPublic);

        // Define a local function that calls on the PreyCaptureWorld.Exp() via reflection.
        void cartesianToPolar(Int32Point p, out int radiusSqr, out float azimuth)
        {
            var args = new object[] { p, null, null };
            methodInfo.Invoke(null, args);
            radiusSqr = (int)args[1];
            azimuth = (float)args[2];
        }

        // Test conversion of a spread of integer Cartesian coordinates.
        {
            cartesianToPolar(new Int32Point(1, 0), out int radiusSqr, out float azimuth);
            Assert.Equal(1, radiusSqr);
            Assert.Equal(0f, azimuth);
        }
        {
            cartesianToPolar(new Int32Point(2, 2), out int radiusSqr, out float azimuth);
            Assert.Equal(8, radiusSqr);
            Assert.Equal(MathF.PI/4f, azimuth);
        }
        {
            cartesianToPolar(new Int32Point(0, 3), out int radiusSqr, out float azimuth);
            Assert.Equal(9, radiusSqr);
            Assert.Equal(MathF.PI/2f, azimuth);
        }
        {
            cartesianToPolar(new Int32Point(-4, 4), out int radiusSqr, out float azimuth);
            Assert.Equal(32, radiusSqr);
            Assert.Equal(MathF.PI*(3f/4f), azimuth);
        }
        {
            cartesianToPolar(new Int32Point(-5, 0), out int radiusSqr, out float azimuth);
            Assert.Equal(25, radiusSqr);
            Assert.Equal(MathF.PI, azimuth);
        }
        {
            cartesianToPolar(new Int32Point(-6, -6), out int radiusSqr, out float azimuth);
            Assert.Equal(72, radiusSqr);
            Assert.Equal(MathF.PI*(5f/4f), azimuth);
        }
        {
            cartesianToPolar(new Int32Point(0, -7), out int radiusSqr, out float azimuth);
            Assert.Equal(49, radiusSqr);
            Assert.Equal(MathF.PI*(3f/2f), azimuth);
        }
        {
            cartesianToPolar(new Int32Point(8, -8), out int radiusSqr, out float azimuth);
            Assert.Equal(128, radiusSqr);
            Assert.Equal(MathF.PI*(7f/4f), azimuth);
        }
    }

    [Fact]
    public void ExpApproximation()
    {
        // Use reflection to call the Exp() method from the PreyCaptureWorld class; this is not ideal, but preferable
        // to making methods internal or public that have no reason to be other than for unit testing.
        MethodInfo expMethodInfo = typeof(PreyCaptureWorld).GetMethod("Exp", BindingFlags.Static | BindingFlags.NonPublic);

        // Define a local function that calls on the PreyCaptureWorld.Exp() via reflection.
        float expApprox(float x)
        {
            return (float)expMethodInfo.Invoke(null, new object[] { x });
        }

        float maxError = 0f;

        // Loop over a range of test input values in the interval [0, 2*PI], as this is the range that PreyCaptureWorld.Exp()
        // is designed to give reasonable approximations for.
        for(float x = 0f; x < 2f*MathF.PI; x += 0.001f)
        {
            // Calc the approximation error as a percentage.
            // Note. absolute error isn't appropriate here, because the exp result has a wildly different magnitude over the input
            // range being tested, this is after all, the exponential function!
            float err = Math.Abs(expApprox(x) / MathF.Exp(x) - 1f);
            maxError = Math.Max(maxError, err);
        }

        // Confirm no more than a 10.86% error; this max percentage error has been confirmed using gnuplot with:
        //
        //    f(x) = (1+ x/29.5)**32
        //    set xrange [0:6.3]
        //    plot f(x) / exp(x)
        Assert.True(maxError < 0.1086f);
    }
}
