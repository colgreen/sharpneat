using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":true}");

            // Test success case.
            bool val = false;
            JsonReadOptionalUtils.ReadBoolOptional(jdoc.RootElement, "foo", x => val = x);
            Assert.AreEqual(true, val);

            // Test missing optional.
            val = false;
            JsonReadOptionalUtils.ReadBoolOptional(jdoc.RootElement, "bar", x => val = x);
            Assert.AreEqual(false, val);

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.ThrowsException<InvalidOperationException>(() => JsonReadOptionalUtils.ReadBoolOptional(jdoc.RootElement, "foo", x => val = x));
        }

        [TestMethod]
        public void ReadIntOptional()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":1234}");

            // Test success case.
            int val = -1;
            JsonReadOptionalUtils.ReadIntOptional(jdoc.RootElement, "foo", x => val = x);
            Assert.AreEqual(1234, val);

            // Test missing optional.
            val = -1;
            JsonReadOptionalUtils.ReadIntOptional(jdoc.RootElement, "bar", x => val = x);
            Assert.AreEqual(-1, val);

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.ThrowsException<InvalidOperationException>(() => JsonReadOptionalUtils.ReadIntOptional(jdoc.RootElement, "foo", x => val = x));
        }

        [TestMethod]
        public void ReadDoubleOptional()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":1234.5}");

            // Test success case.
            double val = -1.0;
            JsonReadOptionalUtils.ReadDoubleOptional(jdoc.RootElement, "foo", x => val = x);
            Assert.AreEqual(1234.5, val);

            // Test missing optional.
            val = -1.0;
            JsonReadOptionalUtils.ReadDoubleOptional(jdoc.RootElement, "bar", x => val = x);
            Assert.AreEqual(-1.0, val);

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.ThrowsException<InvalidOperationException>(() => JsonReadOptionalUtils.ReadDoubleOptional(jdoc.RootElement, "foo", x => val = x));
        }

        [TestMethod]
        public void ReadStringOptional()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");

            // Test success case.
            string val = string.Empty;
            JsonReadOptionalUtils.ReadStringOptional(jdoc.RootElement, "foo", x => val = x);
            Assert.AreEqual("abc", val);

            // Test missing optional.
            val = string.Empty;
            JsonReadOptionalUtils.ReadStringOptional(jdoc.RootElement, "bar", x => val = x);
            Assert.AreEqual(string.Empty, val);
        }

        #endregion
    }
}
