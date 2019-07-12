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
    }
}
