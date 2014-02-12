using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SiteWordsExtractor
{
    class HtmlPageProcessor
    {
        private List<string> m_tagsToIgnore;
        private List<string> m_attributesToRip;

        public HtmlPageProcessor()
        {
            m_attributesToRip = null;
            m_tagsToIgnore = null;
        }

        public bool ProcessHtmlPage(string url, HtmlDocument doc)
        {
            if (m_attributesToRip == null)
            {
                return false;
            }

            if (m_tagsToIgnore == null)
            {
                return false;
            }

            FireOnStartProcessPageEvent(url);

            ProcessNode(doc.DocumentNode);

            FireOnEndProcessPageEvent(url);

            return true;
        }

        private void ProcessNode(HtmlNode node)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ProcessContent(node);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    bool bScrapped = false;
                    foreach (string tagName in m_tagsToIgnore)
                    {
                        if (parentName == tagName)
                        {
                            bScrapped = true;
                        }
                    }
                    if (bScrapped == true)
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        string text = HtmlEntity.DeEntitize(html);
                        //FireOnTextFoundEvent(text);
                        FireOnTextFoundEvent(parentName, text);
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            //FireOnTextFoundEvent("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ProcessContent(node);
                    }
                    break;
            }
        }

        private void ProcessContent(HtmlNode node)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                // check for attributes
                foreach (string attName in m_attributesToRip)
                {
                    HtmlAttribute att = null;
                    att = subnode.Attributes[attName];
                    if (att != null)
                    {
                        FireOnAttributeFoundEvent(attName, att.Value);
                        //FireOnAttributeFoundEvent(att.Value);
                    }
                }

                ProcessNode(subnode);

                // put newline between elements
                FireOnTextFoundEvent(subnode.ParentNode.Name, "\n");
            }
        }

        #region Setters

        public void SetTagsToIgnore(string commaSeperatedListOfTags)
        {
            SetTagsToIgnore(new List<string>(commaSeperatedListOfTags.Split(',')));

        }

        public void SetTagsToIgnore(List<string> tagsToIgnore)
        {
            m_tagsToIgnore = tagsToIgnore;

            // force script and style tags to be ignored
            if (!m_tagsToIgnore.Contains("script"))
            {
                m_tagsToIgnore.Add("script");
            }

            if (!m_tagsToIgnore.Contains("style"))
            {
                m_tagsToIgnore.Add("style");
            }
        }

        public void SetAttributesToRip(string commaSeperatedListOfAttributes)
        {
            m_attributesToRip = new List<string>(commaSeperatedListOfAttributes.Split(','));
        }

        public void SetAttributesToRip(List<string> attributesToRip)
        {
            m_attributesToRip = attributesToRip;
        }

        #endregion // Setters

        #region Synchronous Events

        /// <summary>
        /// synchronous event that is called for each text element found and report tag+text
        /// </summary>
        public event EventHandler<KeyValuePair<string, string>> OnTextFound;

        /// <summary>
        /// synchronous event that is called for each attribute found and report attribute name and value
        /// </summary>
        public event EventHandler<KeyValuePair<string, string>> OnAttributeFound;

        /// <summary>
        /// synchronous event that is called before starting to process a page
        /// </summary>
        public event EventHandler<string> OnStartProcessPage;

        /// <summary>
        /// synchronous event that is called after finished to process a page
        /// </summary>
        public event EventHandler<string> OnEndProcessPage;

        public void FireOnTextFoundEvent(string tagName, string text)
        {
            try
            {
                KeyValuePair<string, string> kvp = new KeyValuePair<string,string>(tagName, text);
                EventHandler<KeyValuePair<string, string>> callback = OnTextFound;
                if (callback != null)
                {
                    callback(this, kvp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OnTextFound: unhandled exception for tag: " + tagName + ", text: " + text);
                Console.WriteLine("Exception: " + e.ToString());
            }
        }

        public void FireOnAttributeFoundEvent(string attName, string attValue)
        {
            try
            {
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(attName, attValue);
                EventHandler<KeyValuePair<string, string>> callback = OnAttributeFound;
                if (callback != null)
                {
                    callback(this, kvp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OnAttributeFound: unhandled exception for attribute: " + attName + ", value: " + attValue);
                Console.WriteLine("Exception: " + e.ToString());
            }
        }

        public void FireOnStartProcessPageEvent(string url)
        {            
            try
            {
                EventHandler<string> callback = OnStartProcessPage;
                if (callback != null)
                {
                    callback(this, url);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OnStartProcessPage: unhandled exception for url: " + url);
                Console.WriteLine("Exception: " + e.ToString());
            }
        }

        public void FireOnEndProcessPageEvent(string url)
        {
            try
            {
                EventHandler<string> callback = OnEndProcessPage;
                if (callback != null)
                {
                    callback(this, url);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OnEndProcessPage: unhandled exception for url: " + url);
                Console.WriteLine("Exception: " + e.ToString());
            }
        }

        #endregion // Synchronous Events
    }
}
