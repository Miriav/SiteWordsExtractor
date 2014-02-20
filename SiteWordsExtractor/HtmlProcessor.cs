using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using log4net;
using log4net.Config;
using System.Text.RegularExpressions;

namespace SiteWordsExtractor
{
    class HtmlProcessor
    {
        public enum NodeType
        {
            Text,
            BoldText,
            Hyperlink,
            Paragraph,
            Element,
            Input,
            Ignored,
        }

        public static readonly ILog log = LogManager.GetLogger(typeof(HtmlProcessor));

        #region Data members

        Uri m_baseUri;
        List<string> m_attributes;
        Dictionary<string, NodeType> m_tag2Type;

        object m_lock; // sync between threads, allow to process one page at a time

        #endregion // Data members

        #region Synchronous Events

        /// <summary>
        /// called when starting to process a new page (url of the page)
        /// </summary>
        public event EventHandler<string> OnStartProcessPage;

        /// <summary>
        /// called when finished to process a page (url of the page)
        /// </summary>
        public event EventHandler<string> OnEndProcessPage;

        /// <summary>
        /// called when encountered a new paragraph (tag name)
        /// </summary>
        public event EventHandler<string> OnNewParagraph;

        /// <summary>
        /// called when found new text in the page (text)
        /// </summary>
        public event EventHandler<string> OnText;

        /// <summary>
        /// called when found new bold text in the page (text)
        /// </summary>
        public event EventHandler<string> OnBoldText;

        /// <summary>
        /// called when found interesting attribute (attribute value)
        /// </summary>
        public event EventHandler<string> OnAttribute;

        /// <summary>
        /// called when found a hyperlink (link + text)
        /// </summary>
        public event EventHandler<HyperlinkEventArgs> OnHyperlink;

        private void FireOnStartProcessPageEvent(string url)
        {
            try
            {
                log.Debug("FireOnStartProcessPageEvent: " + url);
                EventHandler<string> callback = OnStartProcessPage;
                if (callback != null)
                {
                    callback(this, url);
                }
            }
            catch (Exception e)
            {
                log.Error("OnStartProcessPage: unhandled exception for url [" + url + "]");
                log.Error("OnStartProcessPage: " + e.ToString());
            }
        }

        private void FireOnEndProcessPageEvent(string url)
        {
            try
            {
                log.Debug("FireOnEndProcessPageEvent: " + url);
                EventHandler<string> callback = OnEndProcessPage;
                if (callback != null)
                {
                    callback(this, url);
                }
            }
            catch (Exception e)
            {
                log.Error("OnEndProcessPage: unhandled exception for url [" + url + "]");
                log.Error("OnEndProcessPage: " + e.ToString());
            }
        }

        private void FireOnNewParagraphEvent(string tagName)
        {
            try
            {
                log.Debug("FireOnNewParagraphEvent: " + tagName);
                EventHandler<string> callback = OnNewParagraph;
                if (callback != null)
                {
                    callback(this, tagName);
                }
            }
            catch (Exception e)
            {
                log.Error("OnNewParagraph: unhandled exception for tag [" + tagName + "]");
                log.Error("OnNewParagraph: " + e.ToString());
            }
        }

        private void FireOnTextEvent(string text)
        {
            text = RemoveDoubleSpaces(text);
            try
            {
                log.Debug("FireOnTextEvent: " + text);
                EventHandler<string> callback = OnText;
                if (callback != null)
                {
                    callback(this, text);
                }
            }
            catch (Exception e)
            {
                log.Error("OnText: unhandled exception for text [" + text + "]");
                log.Error("OnText: " + e.ToString());
            }
        }

        private void FireOnBoldTextEvent(string text)
        {
            text = RemoveDoubleSpaces(text);
            try
            {
                log.Debug("FireOnBoldTextEvent: " + text);
                EventHandler<string> callback = OnBoldText;
                if (callback != null)
                {
                    callback(this, text);
                }
            }
            catch (Exception e)
            {
                log.Error("OnBoldText: unhandled exception for text [" + text + "]");
                log.Error("OnBoldText: " + e.ToString());
            }
        }

        private void FireOnAttributeEvent(string value)
        {
            try
            {
                log.Debug("FireOnAttributeEvent: " + value);
                EventHandler<string> callback = OnAttribute;
                if (callback != null)
                {
                    callback(this, value);
                }
            }
            catch (Exception e)
            {
                log.Error("OnAttribute: unhandled exception for value [" + value + "]");
                log.Error("OnAttribute: " + e.ToString());
            }
        }

        private void FireOnHyperlinkEvent(string url, string text)
        {
            text = RemoveDoubleSpaces(text);
            try
            {
                log.Debug("FireOnHyperlinkEvent: url=[" + url + "], text=[" + text + "]");
                EventHandler<HyperlinkEventArgs> callback = OnHyperlink;
                if (callback != null)
                {
                    callback(this, new HyperlinkEventArgs(url, text));
                }
            }
            catch (Exception e)
            {
                log.Error("OnHyperlink: unhandled exception for url [" + url + "], text [" + text + "]");
                log.Error("OnHyperlink: " + e.ToString());
            }
        }

        #endregion // Synchronous Events

        #region Setters

        public void SetTags(NodeType type, string commaSeperatedList)
        {
            log.Debug("SetTags for type: " + type.ToString() + " [" + commaSeperatedList + "]");
            List<string> tags = new List<string>(commaSeperatedList.Split(','));
            foreach (string tag in tags)
            {
                string lowerTag = tag.ToLower();
                m_tag2Type[lowerTag] = type;
            }
        }

        public void SetAttributes(string commaSeperatedList)
        {
            log.Debug("SetAttributes: [" + commaSeperatedList + "]");
            m_attributes = new List<string>(commaSeperatedList.Split(','));
        }

        #endregion // Setters

        public HtmlProcessor()
        {
            m_lock = new object();
            m_baseUri = null;
            m_attributes = new List<string>();
            m_tag2Type = new Dictionary<string, NodeType>();

            // set defaults
            SetAttributes("value,alt,title");
            SetTags(NodeType.Paragraph, "p,br,dl,div,h1,h2,h3,h4,h5,h6,li,ul,ol,table,tr,td,th,tbody,thead");
            SetTags(NodeType.BoldText, "b,em,strong");
            SetTags(NodeType.Hyperlink, "a");
            SetTags(NodeType.Input, "input,textarea");
            SetTags(NodeType.Ignored, "script,style,#comment");
        }

        public void ProcessHtmlPage(string url, HtmlDocument doc)
        {
            lock (m_lock)
            {
                FireOnStartProcessPageEvent(url);

                m_baseUri = new Uri(url, UriKind.Absolute);
                ProcessNode(doc.DocumentNode);

                FireOnEndProcessPageEvent(url);
            }
        }
        
        private void ProcessNode(HtmlNode node)
        {
            string html;
            string text;

            NodeType type = GetNodeType(node);
            switch (type)
            {
                case NodeType.Ignored:
                    break;

                case NodeType.Text:
                    // put the text in the file
                    html = ((HtmlTextNode)node).Text;
                    if (!HtmlNode.IsOverlappedClosingElement(html))
                    {
                        if (!String.IsNullOrWhiteSpace(html))
                        {
                            text = HtmlEntity.DeEntitize(html);
                            FireOnTextEvent(text);
                        }
                    }
                    break;

                case NodeType.BoldText:
                    html = node.InnerText;
                    if (!String.IsNullOrWhiteSpace(html))
                    {
                        text = HtmlEntity.DeEntitize(html);
                        FireOnBoldTextEvent(text);
                    }
                    break;

                case NodeType.Hyperlink:
                    if (!ProcessHyperlinkNode(node))
                    {
                        // in cases the link is not a real link
                        ProcessNodeChilds(node);
                    }
                    break;

                case NodeType.Paragraph:
                    FireOnNewParagraphEvent(node.Name);
                    ProcessNodeChilds(node);
                    break;

                case NodeType.Input:
                    ExtractAttributes(node);
                    break;

                case NodeType.Element:
                    ExtractAttributes(node);
                    ProcessNodeChilds(node);
                    break;
            }
        }

        private void ProcessNodeChilds(HtmlNode node)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ProcessNode(subnode);
            }
        }

        private void ExtractAttributes(HtmlNode node)
        {
            foreach (string attName in m_attributes)
            {
                HtmlAttribute att = node.Attributes[attName];
                if (att != null)
                {
                    FireOnAttributeEvent(att.Value);
                }

            }
        }

        private bool ProcessHyperlinkNode(HtmlNode node)
        {
            HtmlAttribute linkSrc = node.Attributes["href"];
            if (linkSrc == null)
            {
                // node should be treated as noremal html element
                return false;
            }

            Uri link = new Uri(m_baseUri, linkSrc.Value);

            string html = node.InnerText;
            if (!String.IsNullOrWhiteSpace(html))
            {
                string text = HtmlEntity.DeEntitize(html);
                text = WebUtility.HtmlDecode(text);
                FireOnHyperlinkEvent(link.ToString(), text);
            }

            return true;
        }

        private NodeType GetNodeType(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Comment)
            {
                return NodeType.Ignored;
            }

            if (node.NodeType == HtmlNodeType.Text)
            {
                return NodeType.Text;
            }

            NodeType type = NodeType.Element; // default type is a regular HTML element
            string nodeTagName = node.Name.ToLower();
            if (m_tag2Type.ContainsKey(nodeTagName))
            {
                type = m_tag2Type[nodeTagName];
            }

            return type;
        }

        public static string RemoveDoubleSpaces(string original)
        {
            return Regex.Replace(original, @"\s+", " ");
        }

    }
}
