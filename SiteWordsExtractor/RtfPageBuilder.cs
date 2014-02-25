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
        private const int TEXT_INDEX = 1;
        private const int ATTRIBUTE_INDEX = 2;
        private const int HYPERLINK_INDEX = 3;

        private string m_strFilepath;
        private RtfDocument m_doc;
        private RtfFormattedParagraph m_currParagraph;

        public RtfDocument RtfDoc
        {
            get { return m_doc; }
        }

        public RtfPageBuilder(string filepath)
        {
            m_strFilepath = filepath;
            m_doc = new RtfDocument();
            m_currParagraph = null;

            // resize font table
            m_doc.FontTable.Add(new RtfFont(AppSettings.Settings.Rtf.TextFont.Name));
            m_doc.FontTable.Add(new RtfFont(AppSettings.Settings.Rtf.AttributeFont.Name));
            m_doc.FontTable.Add(new RtfFont(AppSettings.Settings.Rtf.HyperlinkFont.Name));

            // resize color table
            m_doc.ColorTable.Add(new RtfColor(ColorTranslator.FromHtml(AppSettings.Settings.Rtf.TextFont.Color)));
            m_doc.ColorTable.Add(new RtfColor(ColorTranslator.FromHtml(AppSettings.Settings.Rtf.AttributeFont.Color)));
            m_doc.ColorTable.Add(new RtfColor(ColorTranslator.FromHtml(AppSettings.Settings.Rtf.HyperlinkFont.Color)));

            StartNewParagraph();
        }

        public void StartNewParagraph(int fontIndex, float fontSize, RtfTextAlign alignment, int colorIndex, float spaceAfterPoints)
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
            StartNewParagraph(TEXT_INDEX, AppSettings.Settings.Rtf.TextFont.Size, RtfTextAlign.Left, TEXT_INDEX, AppSettings.Settings.Rtf.SpaceAfterParagraph);
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

            RtfFormattedText formattedText = new RtfFormattedText(text, RtfCharacterFormatting.Regular);
            formattedText.FontIndex = TEXT_INDEX;
            formattedText.TextColorIndex = TEXT_INDEX;
            formattedText.FontSize = AppSettings.Settings.Rtf.TextFont.Size;

            m_currParagraph.AppendText(formattedText);
        }

        public void AppendBoldText(string text)
        {
            AddSpaceIfNeeded();

            RtfFormattedText formattedText = new RtfFormattedText(text, RtfCharacterFormatting.Bold);
            formattedText.FontIndex = TEXT_INDEX;
            formattedText.TextColorIndex = TEXT_INDEX;
            formattedText.FontSize = AppSettings.Settings.Rtf.TextFont.Size;

            m_currParagraph.AppendText(formattedText);
        }

        public void AppendAttributeText(string text)
        {
            AddSpaceIfNeeded();

            RtfFormattedText formattedText = new RtfFormattedText(text, RtfCharacterFormatting.Regular);
            formattedText.FontIndex = ATTRIBUTE_INDEX;
            formattedText.TextColorIndex = ATTRIBUTE_INDEX;
            formattedText.FontSize = AppSettings.Settings.Rtf.AttributeFont.Size;

            m_currParagraph.AppendText(formattedText);
        }

        public void AppendHyperlink(string url, string text)
        {
            AddSpaceIfNeeded();

            RtfFormattedText formattedText = new RtfFormattedText(text, RtfCharacterFormatting.Regular);
            formattedText.FontIndex = HYPERLINK_INDEX;
            formattedText.TextColorIndex = HYPERLINK_INDEX;
            formattedText.FontSize = AppSettings.Settings.Rtf.HyperlinkFont.Size;
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
