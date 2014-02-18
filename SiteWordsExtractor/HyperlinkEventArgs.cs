using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteWordsExtractor
{
    class HyperlinkEventArgs : EventArgs
    {
        private readonly string m_url;
        private readonly string m_text;

        public HyperlinkEventArgs(string url, string text)
        {
            m_url = url;
            m_text = text;
        }

        public string Url 
        {
            get { return m_url; } 
        }

        public string Text
        {
            get { return m_text; }
        }

    }
}
