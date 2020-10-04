using System.Collections.Generic;
using Xunit;

namespace SharpNeat.Graphs
{
    public class DictionaryNodeIdMapTests
    {
        [Fact]
        public void Map_ZeroFixedNodeCount()
        {
            var nodeIdxById = new Dictionary<int,int>
            {
                { 100, 0 },
                { 50, 1 },
                { 200, 2 },
                { 70, 3 },
                { 300, 4 },
                { 30, 5 }
            };

            var nodeIdMap = new DictionaryNodeIdMap(0, nodeIdxById);

            Assert.Equal(6, nodeIdMap.Count);
            Assert.Equal(0, nodeIdMap.Map(100));
            Assert.Equal(1, nodeIdMap.Map(50));
            Assert.Equal(2, nodeIdMap.Map(200));
            Assert.Equal(3, nodeIdMap.Map(70));
            Assert.Equal(4, nodeIdMap.Map(300));
            Assert.Equal(5, nodeIdMap.Map(30));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(29)]
        public void Map_NonZeroFixedNodeCount(int fixedNodeCount)
        {
            var nodeIdxById = new Dictionary<int,int>
            {
                { 100, fixedNodeCount },
                { 50, fixedNodeCount + 1 },
                { 200, fixedNodeCount + 2 },
                { 70, fixedNodeCount + 3 },
                { 300, fixedNodeCount + 4 },
                { 30, fixedNodeCount + 5 }
            };

            var nodeIdMap = new DictionaryNodeIdMap(fixedNodeCount, nodeIdxById);

            Assert.Equal(fixedNodeCount + 6, nodeIdMap.Count);

            Assert.Equal(0, nodeIdMap.Map(0));
            Assert.Equal(fixedNodeCount - 1, nodeIdMap.Map(fixedNodeCount - 1));

            Assert.Equal(fixedNodeCount, nodeIdMap.Map(100));
            Assert.Equal(fixedNodeCount + 1, nodeIdMap.Map(50));
            Assert.Equal(fixedNodeCount + 2, nodeIdMap.Map(200));
            Assert.Equal(fixedNodeCount + 3, nodeIdMap.Map(70));
            Assert.Equal(fixedNodeCount + 4, nodeIdMap.Map(300));
            Assert.Equal(fixedNodeCount + 5, nodeIdMap.Map(30));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(29)]
        public void CreateInverse(int fixedNodeCount)
        {
            var nodeIdxById = new Dictionary<int,int>
            {
                { 100, fixedNodeCount },
                { 50, fixedNodeCount + 1 },
                { 200, fixedNodeCount + 2 },
                { 70, fixedNodeCount + 3 },
                { 300, fixedNodeCount + 4 },
                { 30, fixedNodeCount + 5 }
            };

            var nodeIdMap = new DictionaryNodeIdMap(fixedNodeCount, nodeIdxById);

            INodeIdMap inverseMap = nodeIdMap.CreateInverseMap();

            Assert.Equal(nodeIdMap.Count, inverseMap.Count);

            Assert.Equal(0, inverseMap.Map(0));
            Assert.Equal(fixedNodeCount - 1, inverseMap.Map(fixedNodeCount - 1));

            Assert.Equal(100, inverseMap.Map(fixedNodeCount));
            Assert.Equal(50, inverseMap.Map(fixedNodeCount + 1));
            Assert.Equal(200, inverseMap.Map(fixedNodeCount + 2));
            Assert.Equal(70, inverseMap.Map(fixedNodeCount + 3));
            Assert.Equal(300, inverseMap.Map(fixedNodeCount + 4));
            Assert.Equal(30, inverseMap.Map(fixedNodeCount + 5));
        }
    }
}
