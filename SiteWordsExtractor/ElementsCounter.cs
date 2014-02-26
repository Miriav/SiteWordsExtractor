using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace SiteWordsExtractor
{
    class ElementsCounter
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(ElementsCounter));

        private HtmlProcessor m_processor;
        private WordsCounter m_wordsCounter;
        private Dictionary<string, int> m_elementsCount;
        private Dictionary<string, int> m_elementsWordCount;

        public ElementsCounter(HtmlProcessor processor)
        {
            m_wordsCounter = new WordsCounter(AppSettings.Settings.WordsCounter.RegEx);

            m_elementsCount = new Dictionary<string, int>();
            m_elementsWordCount = new Dictionary<string, int>();

            m_processor = processor;
            m_processor.OnAttribute += OnText;
            m_processor.OnText += OnText;
            m_processor.OnBoldText += OnText;
            m_processor.OnHyperlink += OnHyperlink;
        }

        private void OnText(object sender, string text)
        {
            if (m_elementsCount.ContainsKey(text))
            {
                m_elementsCount[text]++;
            }
            else
            {
                m_elementsCount.Add(text, 1);
                m_elementsWordCount.Add(text, m_wordsCounter.CountWords(text));
            }
        }

        private void OnHyperlink(object sender, HyperlinkEventArgs args)
        {
            OnText(sender, args.Text);
        }

        public void SaveAsCSV(string csvFilepath)
        {
            StreamWriter csvFile;
            try
            {
                csvFile = new StreamWriter(csvFilepath);
            }
            catch (Exception e)
            {
                log.Error("Failed to open csv file: " + csvFilepath + ", ERROR: " + e.ToString());
                return;
            }

            csvFile.WriteLine("Number of unique elements: " + m_elementsCount.Count);
            csvFile.WriteLine("");
            csvFile.WriteLine("Element,Words,Count");
            int totalWords = 0;
            int totalElements = 0;
            foreach (KeyValuePair<string, int> kvp in m_elementsCount)
            {
                string element = kvp.Key;
                int count = kvp.Value;
                int wordsCount = m_elementsWordCount[kvp.Key];
                csvFile.WriteLine("\"'" + element + "\"," + wordsCount.ToString() + "," + count.ToString());

                totalElements += count;
                totalWords += wordsCount;
            }
            csvFile.WriteLine("Total Words: " + totalWords.ToString());
            csvFile.WriteLine("Total Elements: " + totalElements.ToString());
            csvFile.Close();
        }
    }
}
