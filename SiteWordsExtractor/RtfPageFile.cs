using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESCommon.Rtf;

namespace SiteWordsExtractor
{
    class RtfPageFile
    {
        private string m_strFilepath;
        private RtfDocument m_doc;

        // rtf default settings
        // paragraph settings
        /*
        private int const PARAGRAPH_FONT_SIZE = 12;
        private RtfTextAlign 
        */

        public RtfPageFile(string filepath)
        {
            m_strFilepath = filepath;
            m_doc = new RtfDocument();
        }

        public void AddTitle(string title)
        {
        }

        public void AddParagraph(string paragraph)
        {
            RtfFormattedParagraph p = new RtfFormattedParagraph(new RtfParagraphFormatting(12, RtfTextAlign.Left));
        }

        public void AddHighlightedText(string text)
        {
        }

        public void saveRtf()
        {
            RtfWriter rtfWriter = new RtfWriter();
            TextWriter writer = new StreamWriter(m_strFilepath);
            rtfWriter.Write(writer, m_doc);
        }

    }
}
