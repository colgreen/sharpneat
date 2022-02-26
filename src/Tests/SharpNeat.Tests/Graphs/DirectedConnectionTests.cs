using Xunit;

namespace SharpNeat.Graphs.Tests;

public class DirectedConnectionTests
{
    [Theory]
    [InlineData(10, 20,  10, 20, true)]
    [InlineData(10, 20,  10, 21, false)]
    [InlineData(10, 20,  11, 20, false)]
    public void EqualsOverride(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB, bool isEqual)
    {
        Assert.Equal(isEqual, new DirectedConnection(srcIdA, tgtIdA).Equals(new DirectedConnection(srcIdB, tgtIdB)));
    }

    [Theory]
    [InlineData(10, 20,  10, 20, true)]
    [InlineData(10, 20,  10, 21, false)]
    [InlineData(10, 20,  11, 20, false)]
    public void EqualsOperator(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB, bool isEqual)
    {
        Assert.Equal(isEqual, new DirectedConnection(srcIdA, tgtIdA) == new DirectedConnection(srcIdB, tgtIdB));
    }

    [Theory]
    [InlineData(10, 20,  10, 20, true)]
    [InlineData(10, 20,  10, 21, false)]
    [InlineData(10, 20,  11, 20, false)]
    public void NotEqualsOperator(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB, bool isEqual)
    {
        Assert.Equal(!isEqual, new DirectedConnection(srcIdA, tgtIdA) != new DirectedConnection(srcIdB, tgtIdB));
    }

    [Theory]
    [InlineData(10, 20,  10, 21, true)]
    [InlineData(10, 20,  11, 20, true)]
    [InlineData(10, 20,  11, 21, true)]
    [InlineData(10, 20,  10, 20, false)]
    [InlineData(10, 20,  9, 20, false)]
    [InlineData(10, 20,  10, 19, false)]
    [InlineData(10, 20,  9, 19, false)]
    public void LessThan(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB, bool isLessThan)
    {
        Assert.Equal(isLessThan, new DirectedConnection(srcIdA, tgtIdA) < new DirectedConnection(srcIdB, tgtIdB));
    }

    [Theory]
    [InlineData(10, 21,  10, 20, true)]
    [InlineData(11, 20,  10, 20, true)]
    [InlineData(11, 21,  10, 20, true)]
    [InlineData(10, 20,  10, 20, false)]
    [InlineData(9, 20,   10, 20, false)]
    [InlineData(10, 19,  10, 20, false)]
    [InlineData(9, 19,   10, 20, false)]
    public void GreaterThan(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB, bool isGtThan)
    {
        Assert.Equal(isGtThan, new DirectedConnection(srcIdA, tgtIdA) > new DirectedConnection(srcIdB, tgtIdB));
    }

    [Theory]
    [InlineData(10, 20,  10, 20,  0)]

    [InlineData(10, 21,  10, 20,  1)]
    [InlineData(11, 20,  10, 20,  1)]
    [InlineData(11, 21,  10, 20,  1)]

    [InlineData(10, 20,  10, 21,  -1)]
    [InlineData(10, 20,  11, 20,  -1)]
    [InlineData(10, 20,  11, 21,  -1)]

    [InlineData(0, 0,    0, int.MaxValue,  -int.MaxValue)]
    [InlineData(0, 0,    int.MaxValue, 0,  -int.MaxValue)]
    [InlineData(0, 0,    int.MaxValue, int.MaxValue,  -int.MaxValue)]

    [InlineData(0, int.MaxValue,             0, 0,  int.MaxValue)]
    [InlineData(int.MaxValue, 0,             0, 0,  int.MaxValue)]
    [InlineData(int.MaxValue, int.MaxValue,  0, 0,  int.MaxValue)]

    [InlineData(0, int.MaxValue,             0, int.MaxValue,  0)]
    [InlineData(int.MaxValue, 0,             int.MaxValue, 0,  0)]
    [InlineData(int.MaxValue, int.MaxValue,  int.MaxValue, int.MaxValue, 0)]
    public void CompareTo(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB, int result)
    {

        Assert.Equal(result, new DirectedConnection(srcIdA, tgtIdA).CompareTo(new DirectedConnection(srcIdB, tgtIdB)));
    }
}
