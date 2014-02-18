using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteWordsExtractor
{
    class Html2Rtf
    {
        private RtfPageBuilder m_rtf;
        private HtmlProcessor m_processor;
        private WordsCounter m_wordsCounter;

        private int m_wordsCount;
        public int WordsCount
        {
            get { return m_wordsCount; }
        }

        public Html2Rtf(string filepath, string wordsRegex)
        {
            m_wordsCount = 0;
            m_wordsCounter = new WordsCounter(wordsRegex);

            m_rtf = new RtfPageBuilder(filepath);
        }

        public void RegisterProcessor(HtmlProcessor processor)
        {
            m_processor = processor;
            m_processor.OnStartProcessPage += OnStartProcessPage;
            m_processor.OnEndProcessPage += OnEndProcessPage;
            m_processor.OnNewParagraph += OnNewParagraph;
            m_processor.OnText += OnText;
            m_processor.OnBoldText += OnBoldText;
            m_processor.OnAttribute += OnAttribute;
            m_processor.OnHyperlink += OnHyperlink;
        }

        public void UnregisterProcessor()
        {
            m_processor.OnStartProcessPage -= OnStartProcessPage;
            m_processor.OnEndProcessPage -= OnEndProcessPage;
            m_processor.OnNewParagraph -= OnNewParagraph;
            m_processor.OnText -= OnText;
            m_processor.OnBoldText -= OnBoldText;
            m_processor.OnAttribute -= OnAttribute;
            m_processor.OnHyperlink -= OnHyperlink;
            m_processor = null;
            m_rtf.CloseFile();
        }

        private void OnStartProcessPage(object sender, string url)
        {
            m_rtf.StartNewParagraph();
            m_rtf.AppendText("-- PAGE START: ");
            m_rtf.AppendHyperlink(url, url);
            m_rtf.StartNewParagraph();
        }

        private void OnEndProcessPage(object sender, string url)
        {
            m_rtf.StartNewParagraph();
            m_rtf.AppendText("-- PAGE END. Total words: " + WordsCount.ToString());
            m_rtf.StartNewParagraph();
        }

        private void OnNewParagraph(object sender, string tagName)
        {
            m_rtf.StartNewParagraph();
        }

        private void OnText(object sender, string text)
        {
            m_rtf.AppendText(text);
            m_wordsCount += m_wordsCounter.CountWords(text);
        }

        private void OnBoldText(object sender, string text)
        {
            m_rtf.AppendBoldText(text);
            m_wordsCount += m_wordsCounter.CountWords(text);
        }

        private void OnAttribute(object sender, string value)
        {
            // make sure attributes are on a seperate paragraph
            m_rtf.AppendGrayText(value);
            m_rtf.StartNewParagraph();
            m_wordsCount += m_wordsCounter.CountWords(value);
        }

        private void OnHyperlink(object sender, HyperlinkEventArgs args)
        {
            m_rtf.AppendHyperlink(args.Url, args.Text);
            m_wordsCount += m_wordsCounter.CountWords(args.Text);
        }
    }
}
