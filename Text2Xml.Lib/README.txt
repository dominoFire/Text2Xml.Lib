# Text2Xml.Lib

A simple parser that transforms text lines into XML documents.

## Usage


    using Text2Xml;

    // ...

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
    File.WriteAllText(tempPath, "A Text\n" +
        "with multiple lines\n" +
        "and readable content");

    var xmlText = converter.Parse(tempPath);

    Assert.Equal(expected, xmlText);