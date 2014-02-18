using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ESCommon.Rtf;
using ESCommon;
using System.Drawing;

namespace SiteWordsExtractor
{
    class RtfPageBuilder
    {
        private string m_strFilepath;
        private RtfDocument m_doc;
        private RtfFormattedParagraph m_currParagraph;

        private int m_defaultFontIndex = 1;
        private int m_defaultFontSize = 12;
        private RtfTextAlign m_defaultAlignment = RtfTextAlign.Left;
        private int m_defaultColorIndex = 1;
        private float m_defaultSpaceAfter = 6f;

        public RtfPageBuilder(string filepath)
        {
            m_strFilepath = filepath;
            m_doc = new RtfDocument();
            m_currParagraph = null;
            m_doc.ColorTable.Add(new RtfColor(Color.Black));
            m_doc.ColorTable.Add(new RtfColor(Color.Gray));
            m_doc.ColorTable.Add(new RtfColor(Color.Blue));

            StartNewParagraph();
        }

        public void StartNewParagraph(int fontIndex, int fontSize, RtfTextAlign alignment, int colorIndex, float spaceAfterPoints)
        {
            CloseCurrentParagraph();

            RtfParagraphFormatting formatting = new RtfParagraphFormatting(fontSize, alignment);
            m_currParagraph = new RtfFormattedParagraph(formatting);
            m_currParagraph.Formatting.FontIndex = fontIndex;
            m_currParagraph.Formatting.TextColorIndex = colorIndex;
            m_currParagraph.Formatting.SpaceAfter = TwipConverter.ToTwip(spaceAfterPoints, MetricUnit.Point);
        }

        public void StartNewParagraph()
        {
            StartNewParagraph(m_defaultFontIndex, m_defaultFontSize, m_defaultAlignment, m_defaultColorIndex, m_defaultSpaceAfter);
        }

        public void CloseCurrentParagraph()
        {
            if (m_currParagraph != null)
            {
                if (m_currParagraph.Contents.Count > 0)
                {
                    m_doc.Contents.Add(m_currParagraph);
                }
                m_currParagraph = null;
            }
        }

        public void AppendText(string text)
        {
            AddSpaceIfNeeded();
            RtfFormattedText formattedText = new RtfFormattedText(text, RtfCharacterFormatting.Regular, 1);
            m_currParagraph.AppendText(formattedText);
        }

        public void AppendBoldText(string text)
        {
            AddSpaceIfNeeded();
            RtfFormattedText formattedText = new RtfFormattedText(text, RtfCharacterFormatting.Bold, 1);
            m_currParagraph.AppendText(formattedText);
        }

        public void AppendGrayText(string text)
        {
            AddSpaceIfNeeded();
            RtfFormattedText formattedText = new RtfFormattedText(text, RtfCharacterFormatting.Regular, 2);
            m_currParagraph.AppendText(formattedText);
        }

        public void AppendHyperlink(string url, string text)
        {
            AddSpaceIfNeeded();
            RtfFormattedText formattedText = new RtfFormattedText(text, RtfCharacterFormatting.Underline, 3);
            RtfHyperlink hyperlink = new RtfHyperlink(url, formattedText);
            m_currParagraph.AppendText(hyperlink);
        }

        private void AddSpaceIfNeeded()
        {
            if (m_currParagraph.Contents.Count > 0)
            {
                m_currParagraph.AppendText(" ");
            }
        }
        
        public void CloseFile()
        {
            CloseCurrentParagraph();
            RtfWriter rtfWriter = new RtfWriter();
            TextWriter writer = new StreamWriter(m_strFilepath);
            rtfWriter.Write(writer, m_doc);
            writer.Close();
        }
    }
}
