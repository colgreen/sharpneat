using System;
using System.Text.Json;
using Xunit;

namespace SharpNeat.IO.Tests
{
    public class JsonReadOptionalUtilsTests
    {
        #region Test Methods

        [Fact]
        public void ReadBoolOptional()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":true}");

            // Test success case.
            bool val = false;
            JsonReadOptionalUtils.ReadBoolOptional(jdoc.RootElement, "foo", x => val = x);
            Assert.True(val);

            // Test missing optional.
            val = false;
            JsonReadOptionalUtils.ReadBoolOptional(jdoc.RootElement, "bar", x => val = x);
            Assert.False(val);

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.Throws<InvalidOperationException>(() => JsonReadOptionalUtils.ReadBoolOptional(jdoc.RootElement, "foo", x => val = x));
        }

        [Fact]
        public void ReadIntOptional()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":1234}");

            // Test success case.
            int val = -1;
            JsonReadOptionalUtils.ReadIntOptional(jdoc.RootElement, "foo", x => val = x);
            Assert.Equal(1234, val);

            // Test missing optional.
            val = -1;
            JsonReadOptionalUtils.ReadIntOptional(jdoc.RootElement, "bar", x => val = x);
            Assert.Equal(-1, val);

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.Throws<InvalidOperationException>(() => JsonReadOptionalUtils.ReadIntOptional(jdoc.RootElement, "foo", x => val = x));
        }

        [Fact]
        public void ReadDoubleOptional()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":1234.5}");

            // Test success case.
            double val = -1.0;
            JsonReadOptionalUtils.ReadDoubleOptional(jdoc.RootElement, "foo", x => val = x);
            Assert.Equal(1234.5, val);

            // Test missing optional.
            val = -1.0;
            JsonReadOptionalUtils.ReadDoubleOptional(jdoc.RootElement, "bar", x => val = x);
            Assert.Equal(-1.0, val);

            // Test invalid value.
            jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");
            Assert.Throws<InvalidOperationException>(() => JsonReadOptionalUtils.ReadDoubleOptional(jdoc.RootElement, "foo", x => val = x));
        }

        [Fact]
        public void ReadStringOptional()
        {
            JsonDocument jdoc = JsonDocument.Parse("{\"foo\":\"abc\"}");

            // Test success case.
            string val = string.Empty;
            JsonReadOptionalUtils.ReadStringOptional(jdoc.RootElement, "foo", x => val = x);
            Assert.Equal("abc", val);

            // Test missing optional.
            val = string.Empty;
            JsonReadOptionalUtils.ReadStringOptional(jdoc.RootElement, "bar", x => val = x);
            Assert.Equal(string.Empty, val);
        }

        #endregion
    }
}
