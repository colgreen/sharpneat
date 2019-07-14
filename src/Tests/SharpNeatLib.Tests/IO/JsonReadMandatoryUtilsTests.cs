using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SharpNeat.IO;

namespace SharpNeatLib.Tests.IO
{
    [TestClass]
    public class JsonReadMandatoryUtilsTests
    {
        [TestMethod]
        public void ReadBoolMandatory()
        {
            JObject jobj = JObject.Parse("{'foo':true}");

            // Test success case.
            bool val = JsonReadMandatoryUtils.ReadBoolMandatory(jobj, "foo");
            Assert.AreEqual(true, val);

            // Test missing mandatory.
            Assert.ThrowsException<Exception>(() => JsonReadMandatoryUtils.ReadBoolMandatory(jobj, "bar"));

            // Test invalid value.
            jobj = JObject.Parse("{'foo':'abc'}");
            Assert.ThrowsException<FormatException>(() => JsonReadMandatoryUtils.ReadBoolMandatory(jobj, "foo"));
        }

        [TestMethod]
        public void ReadIntMandatory()
        {
            JObject jobj = JObject.Parse("{'foo':'1234'}");

            // Test success case.
            int val = JsonReadMandatoryUtils.ReadIntMandatory(jobj, "foo");
            Assert.AreEqual(1234, val);

            // Test missing mandatory.
            Assert.ThrowsException<Exception>(() => JsonReadMandatoryUtils.ReadIntMandatory(jobj, "bar"));

            // Test invalid value.
            jobj = JObject.Parse("{'foo':'abc'}");
            Assert.ThrowsException<FormatException>(() => JsonReadMandatoryUtils.ReadIntMandatory(jobj, "foo"));
        }

        [TestMethod]
        public void ReadDoubleMandatory()
        {
            JObject jobj = JObject.Parse("{'foo':'1234.5'}");

            // Test success case.
            double val = JsonReadMandatoryUtils.ReadDoubleMandatory(jobj, "foo");
            Assert.AreEqual(1234.5, val);

            // Test missing mandatory.
            Assert.ThrowsException<Exception>(() => JsonReadMandatoryUtils.ReadDoubleMandatory(jobj, "bar"));

            // Test invalid value.
            jobj = JObject.Parse("{'foo':'abc'}");
            Assert.ThrowsException<FormatException>(() => JsonReadMandatoryUtils.ReadDoubleMandatory(jobj, "foo"));
        }

        [TestMethod]
        public void ReadStringMandatory()
        {
            JObject jobj = JObject.Parse("{'foo':'hello world'}");

            // Test success case.
            string val = JsonReadMandatoryUtils.ReadStringMandatory(jobj, "foo");
            Assert.AreEqual("hello world", val);

            // Test missing mandatory.
            Assert.ThrowsException<Exception>(() => JsonReadMandatoryUtils.ReadStringMandatory(jobj, "bar"));
        }
    }
}
