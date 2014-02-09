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
        List<string> m_scrappedTags;
        List<string> m_rippedAttributes;

        public Html2Text(List<string> scrappedTags, List<string> rippedAtts)
        {
            m_scrappedTags = scrappedTags;
            m_rippedAttributes = rippedAtts;
        }

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

            // cleanup the result by removing empty lines
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
                    bool bScrapped = false;
                    foreach (string tagName in m_scrappedTags)
                    {
                        if (parentName == tagName)
                        {
                            bScrapped = true;
                        }
                    }
                    if (bScrapped == true)
                        break;

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
                foreach (string attName in m_rippedAttributes)
                {
                    HtmlAttribute att = null;
                    att = subnode.Attributes[attName];
                    if (att != null)
                    {
                        outText.WriteLine("[" + attName + ": " + att.Value + "]");
                    }
                }

                ConvertTo(subnode, outText);

                // put newline between elements
                outText.WriteLine();
            }
        }
    }
}
