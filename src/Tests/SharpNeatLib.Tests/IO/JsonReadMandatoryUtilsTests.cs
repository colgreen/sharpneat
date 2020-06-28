using System;
using System.Text.Json;
using SharpNeat.IO;
using Xunit;

namespace SharpNeatLib.Tests.IO
{
    public class JsonReadMandatoryUtilsTests
    {
        [Fact]
        public void ReadBoolMandatory()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":true}");

            // Test success case.
            bool val = JsonReadMandatoryUtils.ReadBoolMandatory(jdoc.RootElement, "foo");
            Assert.True(val);

            // Test missing mandatory.
            Assert.Throws<Exception>(() => JsonReadMandatoryUtils.ReadBoolMandatory(jdoc.RootElement, "bar"));

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"'abc\"}");
            Assert.Throws<InvalidOperationException>(() => JsonReadMandatoryUtils.ReadBoolMandatory(jdoc.RootElement, "foo"));
        }

        [Fact]
        public void ReadIntMandatory()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":1234}");

            // Test success case.
            int val = JsonReadMandatoryUtils.ReadIntMandatory(jdoc.RootElement, "foo");
            Assert.Equal(1234, val);

            // Test missing mandatory.
            Assert.Throws<Exception>(() => JsonReadMandatoryUtils.ReadIntMandatory(jdoc.RootElement, "bar"));

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.Throws<InvalidOperationException>(() => JsonReadMandatoryUtils.ReadIntMandatory(jdoc.RootElement, "foo"));
        }

        [Fact]
        public void ReadDoubleMandatory()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":1234.5}");

            // Test success case.
            double val = JsonReadMandatoryUtils.ReadDoubleMandatory(jdoc.RootElement, "foo");
            Assert.Equal(1234.5, val);

            // Test missing mandatory.
            Assert.Throws<Exception>(() => JsonReadMandatoryUtils.ReadDoubleMandatory(jdoc.RootElement, "bar"));

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.Throws<InvalidOperationException>(() => JsonReadMandatoryUtils.ReadDoubleMandatory(jdoc.RootElement, "foo"));
        }

        [Fact]
        public void ReadStringMandatory()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":\"hello world\"}");

            // Test success case.
            string val = JsonReadMandatoryUtils.ReadStringMandatory(jdoc.RootElement, "foo");
            Assert.Equal("hello world", val);

            // Test missing mandatory.
            Assert.Throws<Exception>(() => JsonReadMandatoryUtils.ReadStringMandatory(jdoc.RootElement, "bar"));
        }
    }
}
