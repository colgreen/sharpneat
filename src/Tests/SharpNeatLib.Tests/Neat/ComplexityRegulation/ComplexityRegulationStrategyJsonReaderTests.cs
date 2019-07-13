using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SharpNeat.Neat.ComplexityRegulation;

namespace SharpNeatLib.Tests.Neat.ComplexityRegulation
{
    [TestClass]
    public class ComplexityRegulationStrategyJsonReaderTests
    {
        #region Test Methods

        [TestMethod]
        public void Read_NullStrategy()
        {
            JObject jobj = JObject.Parse(
@"{
    'strategyName': 'null'
}");

            IComplexityRegulationStrategy strategy = ComplexityRegulationStrategyJsonReader.Read(jobj);
            Assert.AreEqual("NullComplexityRegulationStrategy", strategy.GetType().Name);
        }

        [TestMethod]
        public void Read_AbsoluteStrategy()
        {
            JObject jobj = JObject.Parse(
@"{
    'strategyName': 'absolute',
    'complexityCeiling': 11,
    'minSimplifcationGenerations': 12
}");

            IComplexityRegulationStrategy strategy = ComplexityRegulationStrategyJsonReader.Read(jobj);
            Assert.AreEqual("AbsoluteComplexityRegulationStrategy", strategy.GetType().Name);

            // Read private variables with reflection.
            var absoluteStrategy = (AbsoluteComplexityRegulationStrategy)strategy;
            object complexityCeiling = GetInstanceField(absoluteStrategy, "_complexityCeiling");
            object minSimplifcationGenerations = GetInstanceField(absoluteStrategy, "_minSimplifcationGenerations");

            Assert.AreEqual(11.0, complexityCeiling);
            Assert.AreEqual(12, minSimplifcationGenerations);
        }

        [TestMethod]
        public void Read_RelativeStrategy()
        {
            JObject jobj = JObject.Parse(
@"{
    'strategyName': 'relative',
    'relativeComplexityCeiling': 13,
    'minSimplifcationGenerations': 14
}");

            IComplexityRegulationStrategy strategy = ComplexityRegulationStrategyJsonReader.Read(jobj);
            Assert.AreEqual("RelativeComplexityRegulationStrategy", strategy.GetType().Name);

            // Read private variables with reflection.
            var absoluteStrategy = (RelativeComplexityRegulationStrategy)strategy;
            object relativeComplexityCeiling = GetInstanceField(absoluteStrategy, "_relativeComplexityCeiling");
            object complexityCeiling = GetInstanceField(absoluteStrategy, "_complexityCeiling");
            object minSimplifcationGenerations = GetInstanceField(absoluteStrategy, "_minSimplifcationGenerations");

            Assert.AreEqual(13.0, relativeComplexityCeiling);
            Assert.AreEqual(13.0, complexityCeiling);
            Assert.AreEqual(14, minSimplifcationGenerations);
        }

        #endregion

        #region Private Static Methods

        private static object GetInstanceField<T>(T instance, string fieldName)
        {                
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo field = typeof(T).GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        #endregion
    }
}
