using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Abot.Crawler;
using Abot.Poco;
using Abot.Core;
using System.Net;

namespace SiteWordsExtractor
{
 
    public partial class SiteWordsExtractor : Form
    {
        #region Delegates

        // delegate for passing a string parameter
        delegate void StringParameterDelegate(string value);

        // delegate for passing the counters as parameters
        delegate void CoutnersParametersDelegate(int linksFound, int linksVisited, int linksSkipped);

        #endregion // Delegates

        #region Constants

        private const string REPROT_FOLDER_NAME = "Report";
        private const string LOG_FILE_NAME = "log.txt";

        #endregion // Constants

        AppSettings m_appSettings;
        string m_statsRootFolder;
        string m_reportFolder;
        StreamWriter m_logger;
        BackgroundWorker m_backgroundWorker;
        List<string> m_scrappedNodesList;
        List<string> m_rippedAttributesList;

        #region Counters
        
        private int m_linksFound;
        private int m_linksVisited;
        private int m_linksSkipped;

        #endregion // Counters

        public SiteWordsExtractor()
        {
            InitializeComponent();
            m_appSettings = AppSettingsStorage.Load();
            updateStatusLine("Configuration Loaded");
            siteURL.Text = m_appSettings.defaultUrl;
            progressLabel.Text = "";
            m_statsRootFolder = null;
            m_reportFolder = null;
            m_logger = null;

            m_scrappedNodesList = new List<string>();
            m_scrappedNodesList.Add("script");
            m_scrappedNodesList.Add("style");
            m_scrappedNodesList.Add("#comment");

            m_rippedAttributesList = new List<string>();
            m_rippedAttributesList.Add("alt");
            m_rippedAttributesList.Add("title");
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            Settings settingsDlg = new Settings();
            settingsDlg.SetAppSettings(m_appSettings);

            DialogResult result = settingsDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_appSettings = settingsDlg.GetAppSettings();
                AppSettingsStorage.Save(m_appSettings);
                siteURL.Text = m_appSettings.defaultUrl;
                updateStatusLine("Configuration Saved");
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            // validate settings
            bool bValidatedSettings = validateSettings();
            if (!bValidatedSettings)
            {
                updateStatusLine("ERROR - Please check applications settings");
                return;
            }

            bool bInitialized = init();
            if (!bInitialized)
            {
                updateStatusLine("ERROR - Failed to initialized");
                return;
            }

            // call the background worker to do the job in the background
            m_backgroundWorker.RunWorkerAsync();
        }

        private bool validateSettings()
        {
            // TODO: validate settings
            return true;
        }

        private bool init()
        {
            // create stats sub folder
            createStatsSubFolder();
            if (String.IsNullOrWhiteSpace(m_statsRootFolder))
            {
                return false;
            }

            // create report sub folder
            createReportFolder();
            if (String.IsNullOrWhiteSpace(m_reportFolder))
            {
                return false;
            }

            m_backgroundWorker = new BackgroundWorker();
            m_backgroundWorker.DoWork += new DoWorkEventHandler(doWork);

            Log("Initialized");

            return true;
        }

        private void done()
        {
            // close log file
            closeLogFile();
        }

        private void doWork(object sender, DoWorkEventArgs e)
        {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();
            //crawlConfig.CrawlTimeoutSeconds = m_appSettings.httpTimeoutSec;
            crawlConfig.MaxConcurrentThreads = 10;
            crawlConfig.MaxPagesToCrawl = m_appSettings.maxPagesPerSite;
            crawlConfig.UserAgentString = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.107 Safari/537.36";
            crawlConfig.HttpRequestMaxAutoRedirects = m_appSettings.maxRedirects;
            crawlConfig.MaxCrawlDepth = m_appSettings.maxSiteDepth;
            //crawlConfig.MinCrawlDelayPerDomainMilliSeconds = m_appSettings.minHTTPdelayMs;

            PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);
            //PoliteWebCrawler crawler = new PoliteWebCrawler();
            crawler.PageCrawlStartingAsync += crawler_PageCrawlStartingAsync;
            crawler.PageCrawlCompletedAsync += crawler_PageCrawlCompletedAsync;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowedAsync;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowedAsync;

            updateCrawlingStarted();

            Uri url = new Uri(siteURL.Text);
            CrawlResult crawlerResult = crawler.Crawl(url);

            if (m_backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                m_backgroundWorker.ReportProgress(0);
            }

            string elapsedTimeStr = crawlerResult.Elapsed.ToString(@"dd\.hh\:mm\:ss");
            Log("*** Crawling completed. Elapsed time: " + elapsedTimeStr);
            m_logger.Flush();
            
            updateCrawlingFinished();
            done();
        }

        private void processCrawledPage(CrawledPage crawledPage)
        {
            string url = crawledPage.Uri.AbsoluteUri.ToString();
            HtmlAgilityPack.HtmlDocument htmlDoc = crawledPage.HtmlDocument;
            HtmlAgilityPack.HtmlNode root = htmlDoc.DocumentNode;

            Html2Text html2Text = new Html2Text();
            string htmlString = html2Text.ConvertHtml(htmlDoc);
            Log(htmlString);
            /*
            List<string> paragraphs = new List<string>();
            foreach (HtmlAgilityPack.HtmlNode node in root.SelectNodes("//text()[not(parent::script)]"))
            {
                bool bScrapped = false;
                foreach (string tagName in m_scrappedNodesList)
                {
                    if (String.Equals(node.ParentNode.Name, tagName, StringComparison.OrdinalIgnoreCase))
                    //if (node.ParentNode.Name == tagName)
                    {
                        bScrapped = true;
                        break;
                    }
                }
                if (bScrapped)
                {
                    continue;
                }

                string nodeText = node.InnerText.Trim();
                if (!String.IsNullOrWhiteSpace(nodeText))
                {
                    Log("TEXT: " + nodeText);
                }
                else
                {
                    continue;
                }

                foreach (HtmlAgilityPack.HtmlAttribute attribute in node.Attributes)
                {
                    foreach (string attName in m_rippedAttributesList)
                    {
                        if (String.Equals(attName, attribute.Name, StringComparison.OrdinalIgnoreCase))
                        //if (attribute.Name == attName)
                        {
                            Log("[" + attName + ": " + attribute.Value + "]");
                        }
                    }
                }
            }
            */
        }
        
        #region Crawler Callbacks

        void crawler_PageCrawlStartingAsync(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            string msg;

            m_linksFound++;
            updateCrawlingProgress(m_linksFound, -1, -1);

            msg = "checking: " + pageToCrawl.Uri.AbsoluteUri.ToString() + " (parent: " + pageToCrawl.ParentUri.AbsoluteUri.ToString() + ")";
            Log(msg);
        }

        void crawler_PageCrawlCompletedAsync(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            string msg;

            m_linksVisited++;
            updateCrawlingProgress(-1, m_linksVisited, -1);

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                msg = "Failed to crawl to page: " + crawledPage.Uri.AbsoluteUri.ToString() + ", Status: " + crawledPage.HttpWebResponse.StatusCode.ToString();
                LogErr(msg);
            }
            else
            {
                msg = "page visited: " + crawledPage.Uri.AbsoluteUri.ToString();
                Log(msg);
                updateStatusLine(crawledPage.Uri.AbsoluteUri.ToString());
                processCrawledPage(crawledPage);
                //CountWordsOnPage(crawledPage);
            }
        }

        void crawler_PageCrawlDisallowedAsync(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            string msg;

            m_linksSkipped++;
            updateCrawlingProgress(-1, -1, m_linksSkipped);

            msg = "Did not crawl the links on page " + pageToCrawl.Uri.AbsoluteUri.ToString() + " due to " + e.DisallowedReason.ToString();
            LogWrn(msg);
        }

        void crawler_PageLinksCrawlDisallowedAsync(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            string msg;

            m_linksSkipped++;
            updateCrawlingProgress(-1, -1, m_linksSkipped);

            msg = "Did not crawl the links on page " + crawledPage.Uri.AbsoluteUri.ToString() + " due to " + e.DisallowedReason.ToString();
            LogWrn(msg);
        }

        #endregion // Crawler Callbacks

        #region Folders
        private void createStatsSubFolder()
        {
            buildStatsFolderPathName();
            if (String.IsNullOrWhiteSpace(m_statsRootFolder))
            {
                return;
            }

            createFolder(m_statsRootFolder);
        }

        private void createReportFolder()
        {
            if (m_reportFolder == null)
            {
                buildReportFolderPathName();
            }
            createFolder(m_reportFolder);
        }

        private void buildStatsFolderPathName()
        {
            // assuming the application settings are correct

            // scrap the domain from the URL
            string domainName = "";
            try
            {
                Uri domainUri = new Uri(siteURL.Text);
                domainName = domainUri.Host;
            }
            catch (Exception e)
            {
                string msg = "Invalid URL: " + siteURL.Text + ": " + e.ToString();
                updateStatusLine(msg);
                return;
            }

            m_statsRootFolder = m_appSettings.reportsRootFolder;
            if (String.IsNullOrWhiteSpace(m_statsRootFolder))
            {
                m_statsRootFolder = "./";
            }

            m_statsRootFolder += domainName;
        }

        private void buildReportFolderPathName()
        {
            if (String.IsNullOrWhiteSpace(m_statsRootFolder))
            {
                buildStatsFolderPathName();
            }

            if (String.IsNullOrWhiteSpace(m_statsRootFolder))
            {
                m_reportFolder = null;
            }
            else
            {
                m_reportFolder = m_statsRootFolder + "/" + REPROT_FOLDER_NAME;
            }
        }

        private bool createFolder(string folderFullPath)
        {
            try
            {
                // check if folder already exists
                if (Directory.Exists(folderFullPath))
                {
                    LogWrn("Folder " + folderFullPath + " already exists");
                    return true;
                }

                DirectoryInfo dirInfo = Directory.CreateDirectory(folderFullPath);
            }
            catch (Exception e)
            {
                LogErr("Failed to create folder " + folderFullPath + ": " + e.ToString());
                return false;
            }

            return true;
        }
        #endregion // Folders

        #region UI Update
        // update the status line of the main form
        private void updateStatusLine(string statusMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StringParameterDelegate(updateStatusLine), statusMessage);
                return;
            }

            toolStripStatusLabel.Text = "Status: " + statusMessage;
        }

        private void updateCrawlingStarted()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(updateCrawlingStarted));
                return;
            }

            m_linksFound = 0;
            m_linksSkipped = 0;
            m_linksVisited = 0;

            progressBar.Minimum = 0;
            progressBar.Value = 0;
            progressBar.Enabled = true;
            listViewResults.Clear();
            siteURL.Enabled = false;
            goButton.Enabled = false;
            settingsButton.Enabled = false;

            updateStatusLine("Crawling started...");
        }

        private void updateCrawlingProgress(int linksFound, int linksVisited, int linksSkipped)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CoutnersParametersDelegate(updateCrawlingProgress), linksFound, linksVisited, linksSkipped);
                return;
            }

            if (linksFound > -1)
            {
                progressBar.Maximum = linksFound;
            }

            if (linksVisited > -1)
            {
                progressBar.Value = linksVisited;
            }

            progressLabel.Text = "" + m_linksVisited + "/" + m_linksFound;
        }

        private void updateCrawlingFinished()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(updateCrawlingFinished));
                return;
            }

            progressBar.Value = progressBar.Maximum;
            progressBar.Enabled = false;
            siteURL.Enabled = true;
            goButton.Enabled = true;
            settingsButton.Enabled = true;

            updateStatusLine("Crawling finished");
        }

        #endregion // UI Update

        #region Logger

        private void createLogFile()
        {
            if (m_reportFolder == null)
            {
                buildReportFolderPathName();
            }

            if (m_reportFolder != null)
            {
                string logFilePath = m_reportFolder + "/" + LOG_FILE_NAME;
                m_logger = new StreamWriter(logFilePath, false, Encoding.UTF8);
            }
        }

        private void Log(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StringParameterDelegate(Log), msg);
                return;
            }

            if (m_logger == null)
            {
                // make sure the logger is created first
                createLogFile();
            }

            if (m_logger != null)
            {
                string timestamp = DateTime.Now.ToString("[HH:mm:ss] ");
                m_logger.WriteLine(timestamp + msg);
            }
        }

        private void LogErr(string msg)
        {
            Log("ERROR: " + msg);
        }

        private void LogWrn(string msg)
        {
            Log("WARNING: " + msg);
        }

        private void closeLogFile()
        {
            if (m_logger != null)
            {
                m_logger.Close();
                m_logger = null;
            }
        }

        #endregion // Logger

    }
}
