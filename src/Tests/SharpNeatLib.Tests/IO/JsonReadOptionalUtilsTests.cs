using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SharpNeat.IO;

namespace SharpNeatLib.Tests.IO
{
    [TestClass]
    public class JsonReadOptionalUtilsTests
    {
        #region Test Methods

        [TestMethod]
        public void ReadBoolOptional()
        {
            JObject jobj = JObject.Parse("{'foo':true}");

            // Test success case.
            bool val = false;
            JsonReadOptionalUtils.ReadBoolOptional(jobj, "foo", x => val = x);
            Assert.AreEqual(true, val);

            // Test missing optional.
            val = false;
            JsonReadOptionalUtils.ReadBoolOptional(jobj, "bar", x => val = x);
            Assert.AreEqual(false, val);

            // Test invalid value.
            jobj = JObject.Parse("{'foo':'abc'}");
            Assert.ThrowsException<FormatException>(() => JsonReadOptionalUtils.ReadBoolOptional(jobj, "foo", x => val = x));
        }

        [TestMethod]
        public void ReadIntOptional()
        {
            JObject jobj = JObject.Parse("{'foo':'1234'}");

            // Test success case.
            int val = -1;
            JsonReadOptionalUtils.ReadIntOptional(jobj, "foo", x => val = x);
            Assert.AreEqual(1234, val);

            // Test missing optional.
            val = -1;
            JsonReadOptionalUtils.ReadIntOptional(jobj, "bar", x => val = x);
            Assert.AreEqual(-1, val);

            // Test invalid value.
            jobj = JObject.Parse("{'foo':'abc'}");
            Assert.ThrowsException<FormatException>(() => JsonReadOptionalUtils.ReadIntOptional(jobj, "foo", x => val = x));
        }

        [TestMethod]
        public void ReadDoubleOptional()
        {
            JObject jobj = JObject.Parse("{'foo':'1234.5'}");

            // Test success case.
            double val = -1.0;
            JsonReadOptionalUtils.ReadDoubleOptional(jobj, "foo", x => val = x);
            Assert.AreEqual(1234.5, val);

            // Test missing optional.
            val = -1.0;
            JsonReadOptionalUtils.ReadDoubleOptional(jobj, "bar", x => val = x);
            Assert.AreEqual(-1.0, val);

            // Test invalid value.
            jobj = JObject.Parse("{'foo':'abc'}");
            Assert.ThrowsException<FormatException>(() => JsonReadOptionalUtils.ReadDoubleOptional(jobj, "foo", x => val = x));
        }

        [TestMethod]
        public void ReadStringOptional()
        {
            JObject jobj = JObject.Parse("{'foo':'abc'}");

            // Test success case.
            string val = string.Empty;
            JsonReadOptionalUtils.ReadStringOptional(jobj, "foo", x => val = x);
            Assert.AreEqual("abc", val);

            // Test missing optional.
            val = string.Empty;
            JsonReadOptionalUtils.ReadStringOptional(jobj, "bar", x => val = x);
            Assert.AreEqual(string.Empty, val);
        }

        #endregion
    }
}
