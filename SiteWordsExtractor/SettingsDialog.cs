using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiteWordsExtractor
{
    public partial class SettingsDialog : Form
    {
        private string AllHtmlTags = "";


        private AppSettings m_appSettings;
        public AppSettings Settings
        {
            get { return m_appSettings; }
            set
            {
                m_appSettings = value;
                UpdateAllHtmlTags();
                UpdateUI();
            }
        }

        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void SetAllHtmlTags()
        {
            AllHtmlTags = "a,abbr,acronym,address,applet,area,article,aside,audio,b,base,";
            AllHtmlTags += "basefont,bdi,bdo,big,blockquote,body,br,button,canvas,caption,";
            AllHtmlTags += "center,cite,code,col,colgroup,command,datalist,dd,del,details,";
            AllHtmlTags += "dfn,dialog,dir,div,dl,dt,em,embed,fieldset,figcaption,figure,font,";
            AllHtmlTags += "footer,form,frame,frameset,h1> to ,h6,head,header,hgroup,hr,html,";
            AllHtmlTags += "i,iframe,img,input,ins,kbd,keygen,label,legend,li,link,main,map,";
            AllHtmlTags += "mark,menu,meta,meter,nav,noframes,noscript,object,ol,optgroup,";
            AllHtmlTags += "option,output,p,param,pre,progress,q,rp,rt,ruby,s,samp,script,";
            AllHtmlTags += "section,select,small,source,span,strike,strong,style,sub,summary,";
            AllHtmlTags += "sup,table,tbody,td,textarea,tfoot,th,thead,time,title,tr,track,tt,u,ul,var,video,wbr,footer";
        }

        private void UpdateAllHtmlTags()
        {
            SetAllHtmlTags();
            List<string> allTags = new List<string>(AllHtmlTags.Split(','));
            List<string> paragraphTags = new List<string>(m_appSettings.Html.ParagraphTags.Split(','));
            List<string> boldTags = new List<string>(m_appSettings.Html.BoldTextTags.Split(','));
            List<string> inputTags = new List<string>(m_appSettings.Html.InputTags.Split(','));
            List<string> linkTags = new List<string>(m_appSettings.Html.HyperlinkTags.Split(','));
            List<string> ignoreTags = new List<string>(m_appSettings.Html.IgnoredTags.Split(','));

            allTags = allTags.Except(paragraphTags).ToList();
            allTags = allTags.Except(boldTags).ToList();
            allTags = allTags.Except(inputTags).ToList();
            allTags = allTags.Except(linkTags).ToList();
            allTags = allTags.Except(ignoreTags).ToList();

            AllHtmlTags = string.Join(",", allTags.ToArray());
        }

        private void UpdateUI()
        {
            // Application tab
            string ver = m_appSettings.Application.Version.ToString();
            ApplicationVersion.Text = String.Format("Application version: {0}", m_appSettings.Application.Version.ToString());
            ReportsRootFolder.Text = m_appSettings.Application.ReportsRootFolder;
            StatisticsFilename.Text = m_appSettings.Application.StatisticsFilename;
            ReportsFolderName.Text = m_appSettings.Application.ReportsFolderName;

            // HTML tab
            PopulateListBox(listBoxHtmlTags, AllHtmlTags);
            PopulateListBox(listBoxParagraphTags, m_appSettings.Html.ParagraphTags);
            PopulateListBox(listBoxBoldTextTags, m_appSettings.Html.BoldTextTags);
            PopulateListBox(listBoxInputTags, m_appSettings.Html.InputTags);
            PopulateListBox(listBoxHyperlinkTags, m_appSettings.Html.HyperlinkTags);
            PopulateListBox(listBoxIgnoreTags, m_appSettings.Html.IgnoredTags);
            PopulateListBox(listBoxAttributes, m_appSettings.Html.Attributes);

            // Crawler tab
            MaxConcurrentDownloads.Value = m_appSettings.Crawler.MaxConcurrentThreads;
            MaxPagesToCrawl.Value = m_appSettings.Crawler.MaxPagesToCrawl;
            MaxRedirections.Value = m_appSettings.Crawler.HttpRequestMaxAutoRedirects;
            MaxSiteDepth.Value = m_appSettings.Crawler.MaxCrawlDepth;
            UserAgentString.Text = m_appSettings.Crawler.UserAgentString;
            PopulateCrawlerPatterns(m_appSettings.Crawler.RegExDenyURLs);

            // Words counter tab
            WordsCounterRegex.Text = m_appSettings.WordsCounter.RegEx;

            // Rtf tab
            RtfBaseFilename.Text = m_appSettings.Rtf.RtfReportBaseFilename;
            RtfNumberOfPagesInReport.Value = m_appSettings.Rtf.RtfNumberOfPagesInReport;
            UpdateRtfFilenameExample();
            SpacesAfterParagraph.Value = m_appSettings.Rtf.SpaceAfterParagraph;

            buttonTextColor.BackColor = ColorTranslator.FromHtml(m_appSettings.Rtf.TextFont.Color);
            buttonAttributeColor.BackColor = ColorTranslator.FromHtml(m_appSettings.Rtf.AttributeFont.Color);
            buttonLinkColor.BackColor = ColorTranslator.FromHtml(m_appSettings.Rtf.HyperlinkFont.Color);

            UpdateTextFontExample();
        }

        private void UpdateAppSettings()
        {
            // Application tab
            m_appSettings.Application.ReportsRootFolder = ReportsRootFolder.Text;
            m_appSettings.Application.StatisticsFilename = StatisticsFilename.Text;
            m_appSettings.Application.ReportsFolderName = ReportsFolderName.Text;

            // HTML tab
            PopulateListBox(listBoxHtmlTags, AllHtmlTags);
            m_appSettings.Html.ParagraphTags = GetCommaSeperatedValues(listBoxParagraphTags);
            m_appSettings.Html.BoldTextTags = GetCommaSeperatedValues(listBoxBoldTextTags);
            m_appSettings.Html.InputTags = GetCommaSeperatedValues(listBoxInputTags);
            m_appSettings.Html.HyperlinkTags = GetCommaSeperatedValues(listBoxHyperlinkTags);
            m_appSettings.Html.IgnoredTags = GetCommaSeperatedValues(listBoxIgnoreTags);
            m_appSettings.Html.Attributes = GetCommaSeperatedValues(listBoxAttributes);

            // Crawler tab
            m_appSettings.Crawler.MaxConcurrentThreads = (int)MaxConcurrentDownloads.Value;
            m_appSettings.Crawler.MaxPagesToCrawl = (int)MaxPagesToCrawl.Value;
            m_appSettings.Crawler.HttpRequestMaxAutoRedirects = (int)MaxRedirections.Value;
            m_appSettings.Crawler.MaxCrawlDepth = (int)MaxSiteDepth.Value;
            m_appSettings.Crawler.UserAgentString = UserAgentString.Text;
            m_appSettings.Crawler.RegExDenyURLs = GetCrawlerPatterns();

            // Words counter tab
            m_appSettings.WordsCounter.RegEx = WordsCounterRegex.Text;

            // Rtf tab
            m_appSettings.Rtf.RtfReportBaseFilename = RtfBaseFilename.Text;
            m_appSettings.Rtf.RtfNumberOfPagesInReport = (int)RtfNumberOfPagesInReport.Value;
            m_appSettings.Rtf.SpaceAfterParagraph = (int)SpacesAfterParagraph.Value;
            // fonts and colors updated directly while editing
        }

        private void UpdateRtfFilenameExample()
        {
            RtfPagesExample.Text = String.Format("Example: {0}{1}-{2}.rtf, {0}{3}-{4}.rtf", RtfBaseFilename.Text, 0, RtfNumberOfPagesInReport.Value - 1, RtfNumberOfPagesInReport.Value, RtfNumberOfPagesInReport.Value*2 - 1);
        }

        private void UpdateTextFontExample()
        {
            // clear the textbox first
            RtfExample.Text = String.Empty;

            Font textFont = new Font(m_appSettings.Rtf.TextFont.Name, m_appSettings.Rtf.TextFont.Size, m_appSettings.Rtf.TextFont.Style);
            Font boldFont = new Font(textFont.Name, textFont.Size, FontStyle.Bold);
            Font attFont = new Font(m_appSettings.Rtf.AttributeFont.Name, m_appSettings.Rtf.AttributeFont.Size, m_appSettings.Rtf.AttributeFont.Style);
            Font linkFont = new Font(m_appSettings.Rtf.HyperlinkFont.Name, m_appSettings.Rtf.HyperlinkFont.Size, m_appSettings.Rtf.HyperlinkFont.Style);

            RtfExample.SelectionFont = textFont;
            RtfExample.SelectionColor = ColorTranslator.FromHtml(m_appSettings.Rtf.TextFont.Color);
            RtfExample.AppendText("Regular text\n");

            RtfExample.SelectionFont = boldFont;
            RtfExample.SelectionColor = ColorTranslator.FromHtml(m_appSettings.Rtf.TextFont.Color);
            RtfExample.AppendText("Bold text\n");

            RtfExample.SelectionFont = attFont;
            RtfExample.SelectionColor = ColorTranslator.FromHtml(m_appSettings.Rtf.AttributeFont.Color);
            RtfExample.AppendText("Attribute\n");

            RtfExample.SelectionFont = linkFont;
            RtfExample.SelectionColor = ColorTranslator.FromHtml(m_appSettings.Rtf.HyperlinkFont.Color);
            RtfExample.AppendText("This is a link\n");
        }

        private void buttonSaveAndExit_Click(object sender, EventArgs e)
        {
            UpdateAppSettings();
            AppSettingsStorage.Save(m_appSettings);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            m_appSettings = new AppSettings();
            UpdateUI();
        }

        private void buttonBrowseRootFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browseDlg = new FolderBrowserDialog();
            if (browseDlg.ShowDialog() == DialogResult.OK)
            {
                ReportsRootFolder.Text = browseDlg.SelectedPath;
            }
        }

        private void PopulateCrawlerPatterns(string verticalBarSeperatedList)
        {
            crawlerPatterns.Items.Clear();
            List<string> patterns = new List<string>(verticalBarSeperatedList.Split('|'));
            foreach (string item in patterns)
            {
                crawlerPatterns.Items.Add(item);
            }
        }

        private string GetCrawlerPatterns()
        {
            string verticalBarSeperatedList = "";
            List<string> patterns = new List<string>();
            foreach (object item in crawlerPatterns.Items)
            {
                patterns.Add((string)item);
            }
            verticalBarSeperatedList = String.Join("|", patterns);

            return verticalBarSeperatedList;
        }

        private void PopulateListBox(ListBox listBox, string commaSeperatedValues)
        {
            listBox.Items.Clear();
            List<string> values = new List<string>(commaSeperatedValues.Split(','));
            foreach (string item in values)
            {
                listBox.Items.Add(item);
            }
        }

        private string GetCommaSeperatedValues(ListBox listBox)
        {
            string commaSeperatedValues = "";
            List<string> values = new List<string>();
            foreach (object item in listBox.Items)
            {
                values.Add((string)item);
            }
            commaSeperatedValues = String.Join(",", values);

            return commaSeperatedValues;
        }

        private void MoveSelectedItem(ListBox fromBox, ListBox toBox)
        {
            if (fromBox.SelectedIndex > -1)
            {
                object selected = fromBox.Items[fromBox.SelectedIndex];
                fromBox.Items.Remove(selected);
                toBox.Items.Add(selected);
            }
        }

        private void buttonParagraphAdd_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxHtmlTags, listBoxParagraphTags);
        }

        private void buttonParagraphRemove_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxParagraphTags, listBoxHtmlTags);
        }

        private void buttonBoldAdd_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxHtmlTags, listBoxBoldTextTags);
        }

        private void buttonBoldRemove_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxBoldTextTags, listBoxHtmlTags);
        }

        private void buttonInputAdd_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxHtmlTags, listBoxInputTags);
        }

        private void buttonInputRemove_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxInputTags, listBoxHtmlTags);
        }

        private void buttonHyperlinkAdd_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxHtmlTags, listBoxHyperlinkTags);
        }

        private void buttonHyperlinkRemove_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxHyperlinkTags, listBoxHtmlTags);
        }

        private void buttonIgnoreAdd_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxHtmlTags, listBoxIgnoreTags);
        }

        private void buttonIgnoreRemove_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(listBoxIgnoreTags, listBoxHtmlTags);
        }

        private void buttonAttributeAdd_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(NewAttribute.Text))
            {
                return;
            }

            string newAttribute = NewAttribute.Text.ToLower();
            if (listBoxAttributes.Items.Contains(newAttribute))
            {
                return;
            }

            listBoxAttributes.Items.Add(newAttribute);
        }

        private void buttonAttributeRemove_Click(object sender, EventArgs e)
        {
            if (listBoxAttributes.SelectedIndex < 0)
            {
                return;
            }

            string selected = (string)listBoxAttributes.Items[listBoxAttributes.SelectedIndex];
            listBoxAttributes.Items.Remove(selected);

            NewAttribute.Text = selected;
        }

        private void RtfBaseFilename_TextChanged(object sender, EventArgs e)
        {
            UpdateRtfFilenameExample();
        }

        private void TextFontSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateTextFontExample();
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowEffects = false;
            if (fontDlg.ShowDialog() == DialogResult.OK)
            {
                string msg = "Name=" + fontDlg.Font.Name;
                msg += ", size=" + fontDlg.Font.SizeInPoints;
                msg += ", color=" + fontDlg.Color.ToString();
                MessageBox.Show(msg);
            }
        }

        private void buttonTextFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowEffects = false;
            fontDlg.Font = new Font(m_appSettings.Rtf.TextFont.Name, m_appSettings.Rtf.TextFont.Size, m_appSettings.Rtf.TextFont.Style);
            if (fontDlg.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.Rtf.TextFont.Name = fontDlg.Font.Name;
                m_appSettings.Rtf.TextFont.Size = fontDlg.Font.Size;
                m_appSettings.Rtf.TextFont.Style = fontDlg.Font.Style;
                UpdateTextFontExample();
            }
        }

        private void buttonAttributeFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowEffects = false;
            fontDlg.Font = new Font(m_appSettings.Rtf.AttributeFont.Name, m_appSettings.Rtf.AttributeFont.Size, m_appSettings.Rtf.AttributeFont.Style);
            if (fontDlg.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.Rtf.AttributeFont.Name = fontDlg.Font.Name;
                m_appSettings.Rtf.AttributeFont.Size = fontDlg.Font.Size;
                m_appSettings.Rtf.AttributeFont.Style = fontDlg.Font.Style;
                UpdateTextFontExample();
            }
        }

        private void buttonLinkFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowEffects = false;
            fontDlg.Font = new Font(m_appSettings.Rtf.HyperlinkFont.Name, m_appSettings.Rtf.HyperlinkFont.Size, m_appSettings.Rtf.HyperlinkFont.Style);
            if (fontDlg.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.Rtf.HyperlinkFont.Name = fontDlg.Font.Name;
                m_appSettings.Rtf.HyperlinkFont.Size = fontDlg.Font.Size;
                m_appSettings.Rtf.HyperlinkFont.Style = fontDlg.Font.Style;
                UpdateTextFontExample();
            }
        }

        private void buttonTextColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = ColorTranslator.FromHtml(m_appSettings.Rtf.TextFont.Color);
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.Rtf.TextFont.Color = ColorTranslator.ToHtml(colorDlg.Color);
                buttonTextColor.BackColor = colorDlg.Color;
                UpdateTextFontExample();
            }
        }

        private void buttonAttributeColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = ColorTranslator.FromHtml(m_appSettings.Rtf.AttributeFont.Color);
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.Rtf.AttributeFont.Color = ColorTranslator.ToHtml(colorDlg.Color);
                buttonAttributeColor.BackColor = colorDlg.Color;
                UpdateTextFontExample();
            }
        }

        private void buttonLinkColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = ColorTranslator.FromHtml(m_appSettings.Rtf.HyperlinkFont.Color);
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.Rtf.HyperlinkFont.Color = ColorTranslator.ToHtml(colorDlg.Color);
                buttonLinkColor.BackColor = colorDlg.Color;
                UpdateTextFontExample();
            }
        }

        private void buttonCrawlerPatternAdd_Click(object sender, EventArgs e)
        {
            string pattern = crawlerNewPatternValue.Text;
            pattern = pattern.Trim();
            if (String.IsNullOrWhiteSpace(pattern))
            {
                return;
            }

            pattern = pattern.ToLower();

            if (crawlerPatterns.Items.Contains(pattern))
            {
                return;
            }

            crawlerPatterns.Items.Add(pattern);
        }

        private void buttonCrawlerPatternRemove_Click(object sender, EventArgs e)
        {
            if (crawlerPatterns.SelectedIndex > -1)
            {
                object selected = crawlerPatterns.Items[crawlerPatterns.SelectedIndex];
                crawlerPatterns.Items.Remove(selected);
                crawlerNewPatternValue.Text = (string)selected;
            }
        }


    }
}
