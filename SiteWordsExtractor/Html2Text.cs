using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;

namespace SiteWordsExtractor
{
    class Html2Text
    {

        public string Convert(string path)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);

            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        public string ConvertHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        public string ConvertHtml(HtmlDocument doc)
        {
            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            string htmlText = sw.ToString();

            // cleanup the result be removing empty lines
            StringReader sr = new StringReader(htmlText);
            StringWriter result = new StringWriter();
            string line = string.Empty;
            do
            {
                line = sr.ReadLine();
                if (line != null)
                {
                    line = line.Trim();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        result.WriteLine(line);
                    }
                }
            } while (line != null);

            htmlText = result.ToString();

            return htmlText;
        }

        public void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }

        private void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                // check for attributes
                HtmlAttribute att = null;
                att = subnode.Attributes["alt"];
                if (att != null)
                {
                    outText.WriteLine("[ALT: " + att.Value + "]");
                }

                att = subnode.Attributes["title"];
                if (att != null)
                {
                    outText.WriteLine("[TITLE: " + att.Value + "]");
                }

                ConvertTo(subnode, outText);

                // put newline between elements
                outText.WriteLine();
            }
        }
    }
}
