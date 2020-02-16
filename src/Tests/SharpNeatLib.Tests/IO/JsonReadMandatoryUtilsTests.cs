using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.IO;

namespace SharpNeatLib.Tests.IO
{
    [TestClass]
    public class JsonReadMandatoryUtilsTests
    {
        [TestMethod]
        public void ReadBoolMandatory()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":true}");

            // Test success case.
            bool val = JsonReadMandatoryUtils.ReadBoolMandatory(jdoc.RootElement, "foo");
            Assert.AreEqual(true, val);

            // Test missing mandatory.
            Assert.ThrowsException<Exception>(() => JsonReadMandatoryUtils.ReadBoolMandatory(jdoc.RootElement, "bar"));

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"'abc\"}");
            Assert.ThrowsException<InvalidOperationException>(() => JsonReadMandatoryUtils.ReadBoolMandatory(jdoc.RootElement, "foo"));
        }

        [TestMethod]
        public void ReadIntMandatory()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":1234}");

            // Test success case.
            int val = JsonReadMandatoryUtils.ReadIntMandatory(jdoc.RootElement, "foo");
            Assert.AreEqual(1234, val);

            // Test missing mandatory.
            Assert.ThrowsException<Exception>(() => JsonReadMandatoryUtils.ReadIntMandatory(jdoc.RootElement, "bar"));

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.ThrowsException<InvalidOperationException>(() => JsonReadMandatoryUtils.ReadIntMandatory(jdoc.RootElement, "foo"));
        }

        [TestMethod]
        public void ReadDoubleMandatory()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":1234.5}");

            // Test success case.
            double val = JsonReadMandatoryUtils.ReadDoubleMandatory(jdoc.RootElement, "foo");
            Assert.AreEqual(1234.5, val);

            // Test missing mandatory.
            Assert.ThrowsException<Exception>(() => JsonReadMandatoryUtils.ReadDoubleMandatory(jdoc.RootElement, "bar"));

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.ThrowsException<InvalidOperationException>(() => JsonReadMandatoryUtils.ReadDoubleMandatory(jdoc.RootElement, "foo"));
        }

        [TestMethod]
        public void ReadStringMandatory()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":\"hello world\"}");

            // Test success case.
            string val = JsonReadMandatoryUtils.ReadStringMandatory(jdoc.RootElement, "foo");
            Assert.AreEqual("hello world", val);

            // Test missing mandatory.
            Assert.ThrowsException<Exception>(() => JsonReadMandatoryUtils.ReadStringMandatory(jdoc.RootElement, "bar"));
        }
    }
}
