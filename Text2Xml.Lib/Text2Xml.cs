using System;
using System.IO;
using System.Xml;

namespace Text2Xml
{
    /// <summary>
    /// Represents an object that can transform a text file into a XML file
    /// </summary>
    public class Text2XmlConverter
    {
        /// <summary>
        /// Indicates whether to include empty elements, defaults to false
        /// </summary>
        public bool IncludeEmpty { get; set; } = false;

        /// <summary>
        /// Indicates wether to include the last read element
        /// </summary>
        public bool IncludeLastElementRow { get; set; } = false;

        /// <summary>
        /// Indicates wheter to process the first line of the file
        /// </summary>
        public bool IncludeFirstLine { get; set; } = true;

        /// <summary>
        /// Indicates wheter to process the last line of the file
        /// </summary>
        public bool IncludeLastLine { get; set; } = true;

        /// <summary>
        /// Name of the tag for line elemement
        /// </summary>
        public string XmlTagRow { get; set; } = "Row";

        /// <summary>
        /// Name of the tag of the root of the XML document
        /// </summary>
        public string XmlTagRoot { get; set; } = "InvoiceOrder";

        /// <summary>
        /// Name of the tag of the selemtn
        /// </summary>
        public string XmlTagToken { get; set; } = "Token";

        /// <summary>
        /// Option to enumerate the token element
        /// </summary>
        public bool EnumerateTokens { get; set; } = true;

        /// <summary>
        /// String in which to separate the rows of the file. Defaults to newline (system dependent)
        /// </summary>
        public string TextRowSeparator { get; set; } = System.Environment.NewLine;

        /// <summary>
        /// String to separate the tokens inside text lines. Defaults to ','
        /// </summary>
        public string TextColumnSeparator { get; set; } = ",";

        /// <summary>
        /// Public constructor
        /// </summary>
        public Text2XmlConverter() { }

        /// <summary>
        /// Reads text data from stream, produces an XML
        /// </summary>
        /// <param name="stream">Stream to read. Does not rewind</param>
        /// <returns></returns>
        public XmlDocument ParseTextFile(Stream stream)
        {
            string lin;
            StreamReader sr = new StreamReader(stream);
            string[] tokens;
            bool include;
            bool first = true;
            bool lastLine = false;
            int tokenNum = 1;
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = null, rowNode = null, tokenNode = null, lastRowNode = null;

            rootNode = xmlDoc.CreateElement(this.XmlTagRoot);
            xmlDoc.AppendChild(rootNode);

            while(!sr.EndOfStream)
            {
                lin = sr.ReadLine();
                tokenNum = 1;

                if (sr.EndOfStream)
                {
                    lastLine = true;
                }

                if(first && this.IncludeFirstLine)
                {
                    include = false;
                }
                else if (lastLine && this.IncludeLastLine)
                {
                    include = false;
                }
                else if (string.IsNullOrEmpty(lin))
                {
                    include = this.IncludeEmpty;
                }
                else
                {
                    include = true;
                }

                if (include)
                {
                    rowNode = xmlDoc.CreateElement(this.XmlTagRow);
                    tokens = lin.Split(this.TextColumnSeparator.ToCharArray());
                    foreach(var s in tokens)
                    {
                        var tagText = this.EnumerateTokens ? this.XmlTagToken + tokenNum : this.XmlTagToken;
                        tokenNum++;
                        tokenNode = xmlDoc.CreateElement(tagText);
                        tokenNode.InnerText = s;
                        rowNode.AppendChild(tokenNode);
                    }
                    rootNode.AppendChild(rowNode);
                    lastRowNode = rowNode;
                }

                first = false;
            }
            if (this.IncludeLastElementRow && lastRowNode != null)
                rootNode.RemoveChild(lastRowNode);

            return xmlDoc;
        }

        /// <summary>
        /// Reads a file, outputs a XML string
        /// </summary>
        /// <param name="textFilePath">File path to the text file</param>
        /// <returns></returns>
        public string Parse(string textFilePath)
        {
            string result = null;
            XmlDocument outXml = null;
            using (FileStream fs = File.Open(textFilePath, FileMode.Open))
            {
                outXml = this.ParseTextFile(fs);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter xout = XmlWriter.Create(ms))
                {
                    outXml.WriteTo(xout);
                }

                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }
    }
}
