using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            AllHtmlTags += "sup,table,tbody,td,textarea,tfoot,th,thead,time,title,tr,track,tt,u,ul,var,video,wbr";
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

            // Words counter tab
            WordsCounterRegex.Text = m_appSettings.WordsCounter.RegEx;

            // Rtf tab
            RtfBaseFilename.Text = m_appSettings.Rtf.RtfReportBaseFilename;
            RtfNumberOfPagesInReport.Value = m_appSettings.Rtf.RtfNumberOfPagesInReport;
            UpdateRtfFilenameExample();
            SpacesAfterParagraph.Value = m_appSettings.Rtf.SpaceAfterParagraph;

            TextFontSize.Value = m_appSettings.Rtf.TextFontSize;
            TextFontColor.BackColor = ColorTranslator.FromHtml(m_appSettings.Rtf.TextFontColor);
            UpdateTextFontExample();

            AttributeFontSize.Value = m_appSettings.Rtf.AttributeFontSize;
            AttributeFontColor.BackColor = ColorTranslator.FromHtml(m_appSettings.Rtf.AttributeFontColor);

            LinksFontSize.Value = m_appSettings.Rtf.HyperlinkFontSize;
            LinksFontColor.BackColor = ColorTranslator.FromHtml(m_appSettings.Rtf.HyperlinkFontColor);

        }

        private void UpdateRtfFilenameExample()
        {
            RtfPagesExample.Text = String.Format("Example: {0}{1}, {0}{2}", RtfBaseFilename.Text, 0, RtfNumberOfPagesInReport.Value);
        }

        private void UpdateTextFontExample()
        {
            // TODO: clear the textbox first
            TextFontExample.Text = String.Empty;

            //float fontSize = (float)TextFontSize.Value;
            float fontSize = 8f;
            TextFontExample.Font = new Font("Microsoft Sans Serif", fontSize, FontStyle.Regular);
            TextFontExample.SelectionColor = TextFontColor.BackColor;
            TextFontExample.AppendText("Regular text\n");
            /*
            TextFontExample.Font = new Font("Calibri", fontSize, FontStyle.Bold);
            TextFontExample.SelectionColor = TextFontColor.BackColor;
            TextFontExample.AppendText("Bold text");
             */
        }

        private void UpdateSettings()
        {
        }

        private void buttonSaveAndExit_Click(object sender, EventArgs e)
        {
            UpdateSettings();
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

        private void PopulateListBox(ListBox listBox, string commaSeperatedValues)
        {
            listBox.Items.Clear();
            List<string> values = new List<string>(commaSeperatedValues.Split(','));
            foreach (string item in values)
            {
                listBox.Items.Add(item);
            }
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


    }
}
