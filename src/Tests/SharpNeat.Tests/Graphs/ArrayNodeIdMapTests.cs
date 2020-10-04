using Xunit;

namespace SharpNeat.Graphs
{
    public class ArrayNodeIdMapTests
    {
        [Fact]
        public void Map()
        {
            var nodeIdByIdx = new int[] { 30, 20, 60, 10, 1, 8 };

            var idMap = new ArrayNodeIdMap(nodeIdByIdx);

            Assert.Equal(6, idMap.Count);
            Assert.Equal(30, idMap.Map(0));
            Assert.Equal(20, idMap.Map(1));
            Assert.Equal(60, idMap.Map(2));
            Assert.Equal(10, idMap.Map(3));
            Assert.Equal(1, idMap.Map(4));
            Assert.Equal(8, idMap.Map(5));
        }
    }
}
