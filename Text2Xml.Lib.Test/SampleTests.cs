using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Text2Xml.Lib.Test
{
    public class SampleTests
    {
        private readonly ITestOutputHelper output;

        public SampleTests(ITestOutputHelper testOutput)
        {
            this.output = testOutput;
        }

        [Fact]
        public void TestWithLines()
        {
            var converter = new Text2XmlConverter()
            {
                XmlTagRoot = "F",
                XmlTagRow = "R",
                XmlTagToken = "C",
                IncludeFirstLine = true,
                IncludeLastLine = true,
                EnumerateTokens = false,
                TextRowSeparator = "\n",
            };

            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, @"A Text
with multiple lines
and readable content");

            var xmlText = converter.Parse(tempPath);

            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?><F><R><C>A Text</C></R><R><C>with multiple lines</C></R><R><C>and readable content</C></R></F>";
            Assert.Equal(expected, xmlText);
        }
    }
}
