using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESCommon.Rtf;
using ESCommon;
using System.Drawing;

namespace SiteWordsExtractor
{
    public class RtfPageFile
    {
        public class RtfPageFormat
        {
            public int ParagraphFontSize;
            public RtfTextAlign ParagraphAlignment;
            public float ParagraphSpaceAfter;
            public RtfColor ParagraphTextColor;
            public RtfColor ParagraphAltColor;
            public RtfColor HyperlinkColor;
            public float AttributeSpaceAfter;
        }

        RtfPageFormat m_pageFormat;
        private string m_strFilepath;
        private RtfDocument m_doc;
                
        public RtfPageFile(string filepath)
        {
            m_strFilepath = filepath;
            m_doc = new RtfDocument();
            LoadPageFormatting();

            m_doc.ColorTable.AddRange(new RtfColor[] {
                m_pageFormat.ParagraphTextColor,
                m_pageFormat.ParagraphAltColor,
                m_pageFormat.HyperlinkColor
            });

        }

        public void LoadPageFormatting(RtfPageFormat format = null)
        {
            if (format != null)
            {
                m_pageFormat = format;
                return;
            }

            // else, use default settings
            m_pageFormat = new RtfPageFormat();
            m_pageFormat.ParagraphFontSize = 12;
            m_pageFormat.ParagraphAlignment = RtfTextAlign.Left;
            m_pageFormat.ParagraphSpaceAfter = 12f;
            m_pageFormat.AttributeSpaceAfter = 6f;
            m_pageFormat.ParagraphTextColor = new RtfColor(Color.Black);
            m_pageFormat.ParagraphAltColor = new RtfColor(Color.Gray);
            m_pageFormat.HyperlinkColor = new RtfColor(Color.Blue);
        }

        public void AppendHtmlElement(string text)
        {
            AppendText(text);
            return;
        }

        public void AppendHtmlAttribute(string attValue)
        {
            RtfFormattedParagraph p = new RtfFormattedParagraph(new RtfParagraphFormatting(m_pageFormat.ParagraphFontSize, m_pageFormat.ParagraphAlignment));
            p.Formatting.FontIndex = 1; // TODO: should be configured
            p.Formatting.TextColorIndex = 2;
            p.Formatting.SpaceAfter = TwipConverter.ToTwip(m_pageFormat.ParagraphSpaceAfter, MetricUnit.Point);
            p.AppendText(attValue);

            m_doc.Contents.Add(p);
        }

        public void AddHyperlink(string url, string prefixText = "")
        {
            RtfFormattedParagraph p = new RtfFormattedParagraph(new RtfParagraphFormatting(m_pageFormat.ParagraphFontSize, m_pageFormat.ParagraphAlignment));

            p.Formatting.SpaceAfter = TwipConverter.ToTwip(m_pageFormat.ParagraphSpaceAfter, MetricUnit.Point);

            RtfFormattedText linkText = new RtfFormattedText(url, RtfCharacterFormatting.Underline, 3); // color index for links is 3
            linkText.BackgroundColorIndex = 1;
            p.AppendParagraph(prefixText);
            p.AppendParagraph(new RtfHyperlink(url, linkText));

            m_doc.Contents.Add(p);
        }

        public void AppendText(string text)
        {
            RtfFormattedParagraph p = new RtfFormattedParagraph(new RtfParagraphFormatting(m_pageFormat.ParagraphFontSize, m_pageFormat.ParagraphAlignment));
            p.Formatting.FontIndex = 1;
            p.Formatting.TextColorIndex = 1;
            p.Formatting.SpaceAfter = TwipConverter.ToTwip(m_pageFormat.ParagraphSpaceAfter, MetricUnit.Point);
            p.AppendText(text);

            m_doc.Contents.Add(p);
        }

        public void saveRtf()
        {
            RtfWriter rtfWriter = new RtfWriter();
            TextWriter writer = new StreamWriter(m_strFilepath);
            rtfWriter.Write(writer, m_doc);
            writer.Close();
        }

    }
}
