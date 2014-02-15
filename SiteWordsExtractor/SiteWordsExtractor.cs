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
using System.Windows;
using Abot.Crawler;
using Abot.Poco;
using Abot.Core;
using System.Net;

namespace SiteWordsExtractor
{
 
    public partial class SiteWordsExtractor : Form
    {
        private class RtfFileWriter
        {
            RtfPageFile rtfFile;
            EventHandler<string> startDelegate;
            EventHandler<string> endDelegate;
            EventHandler<KeyValuePair<string, string>> textFoundDelegate;
            EventHandler<KeyValuePair<string, string>> attFoundDelegate;
            HtmlPageProcessor m_processor;
            WordsCounter m_wordsCounter;

            private int wordsCount;
            public int WordsCount
            {
                get { return wordsCount; }
            }

            public RtfFileWriter(string filepath, string wordsRegex)
            {
                wordsCount = 0;
                m_wordsCounter = new WordsCounter(wordsRegex);

                rtfFile = new RtfPageFile(filepath);
                startDelegate = delegate(object o, string url)
                {
                    rtfFile.AddHyperlink(url, "--- START PAGE ---");
                };
                endDelegate = delegate(object o, string url)
                {
                    rtfFile.AppendText("--- END PAGE (" + wordsCount.ToString() + " words) ---");
                };
                textFoundDelegate = delegate(object obj, KeyValuePair<string, string> kvp)
                {
                    string tagName = kvp.Key;
                    string text = kvp.Value;
                    if (!String.IsNullOrWhiteSpace(text))
                    {
                        rtfFile.AppendHtmlElement(text);
                        wordsCount += m_wordsCounter.CountWords(text);
                    }
                };
                attFoundDelegate = delegate(object obj, KeyValuePair<string, string> kvp)
                {
                    string attName = kvp.Key;
                    string attValue = kvp.Value;
                    if (!String.IsNullOrWhiteSpace(attValue))
                    {
                        rtfFile.AppendHtmlAttribute(attValue);
                        wordsCount += m_wordsCounter.CountWords(attValue);
                    }
                };
            }

            public void Start(HtmlPageProcessor processor)
            {
                m_processor = processor;
                m_processor.OnStartProcessPage += startDelegate;
                m_processor.OnEndProcessPage += endDelegate;
                m_processor.OnTextFound += textFoundDelegate;
                m_processor.OnAttributeFound += attFoundDelegate;
            }

            public void Done()
            {
                m_processor.OnStartProcessPage -= startDelegate;
                m_processor.OnEndProcessPage -= endDelegate;
                m_processor.OnTextFound -= textFoundDelegate;
                m_processor.OnAttributeFound -= attFoundDelegate;
                rtfFile.saveRtf();
            }
        }

        #region Delegates

        // delegate for passing a string parameter
        delegate void StringParameterDelegate(string value);

        // delegate for passing a number parameter
        delegate void IntParameterDelegate(int value);

        // delegate for passing the counters as parameters
        delegate void CoutnersParametersDelegate(int linksFound, int linksVisited, int linksSkipped);

        // delegate for updating the list view
        delegate void UpdateListViewDelegate(string url, int wordsCount, string filename);

        #endregion // Delegates

        #region Constants

        private const string REPROT_FOLDER_NAME = "Report";
        private const string LOG_FILE_NAME = "log.txt";
        private const string GLOBAL_RFT_FILENAME = "report.rtf";

        #endregion // Constants

        #region Data Members
        
        AppSettings m_appSettings;
        string m_statsRootFolder;
        string m_reportFolder;
        BackgroundWorker m_backgroundWorker;
        List<string> m_notAllowedFileExtList;
        SimpleLogger m_simpleLogger;
        HtmlPageProcessor m_pageProcessor;

        #endregion
        
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
            m_simpleLogger = null;
            m_pageProcessor = new HtmlPageProcessor();

            validateSettings();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            Settings settingsDlg = new Settings();
            settingsDlg.SetAppSettings(m_appSettings);

            DialogResult result = settingsDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_appSettings = settingsDlg.GetAppSettings();
                if (validateSettings())
                {
                    AppSettingsStorage.Save(m_appSettings);
                    siteURL.Text = m_appSettings.defaultUrl;
                    updateStatusLine("Configuration Saved");
                }
                else
                {
                    updateStatusLine("Failed to validate settings");
                }
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.UserClosing:
                    if (MessageBox.Show("Are you sure you want to exit the application?", "Site Words Extractor", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                    break;
                case CloseReason.WindowsShutDown:
                    e.Cancel = false; //this is propably dumb
                    break;
                default:
                    break;
            }

            base.OnFormClosing(e);
        }

        private bool validateSettings()
        {
            // TODO: validate settings

            m_notAllowedFileExtList = new List<string>(m_appSettings.fileExt.Split(','));

            m_pageProcessor.SetTagsToIgnore(m_appSettings.scrappedHTMLTags);
            m_pageProcessor.SetAttributesToRip(m_appSettings.attributes);

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
            m_simpleLogger.closeLogFile();
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
            crawler.PageCrawlStartingAsync += crawler_PageCrawlStartingAsync;
            crawler.PageCrawlCompletedAsync += crawler_PageCrawlCompletedAsync;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowedAsync;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowedAsync;
            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                CrawlDecision decision = new CrawlDecision();
                FileInfo fi = new FileInfo(pageToCrawl.Uri.AbsolutePath);
                string ext = fi.Extension;
                foreach (string notAllowedExt in m_notAllowedFileExtList)
                {
                    if (ext == notAllowedExt)
                    {
                        return new CrawlDecision { Allow = false, Reason = "File extension is not allowed: " + ext };
                    }
                }

                decision = new CrawlDecision { Allow = true };

                return decision;
            });

            string globalRtfFilepath = m_reportFolder + "/" + GLOBAL_RFT_FILENAME;
            RtfFileWriter globalRtfWriter = new RtfFileWriter(globalRtfFilepath, m_appSettings.wordRegex);
            globalRtfWriter.Start(m_pageProcessor);

            // TODO: create global words counter

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
            Log("*** Total words found in this site: " + globalRtfWriter.WordsCount);

            int totalWordsCount = globalRtfWriter.WordsCount;

            // close global rtf file of site
            globalRtfWriter.Done();

            // create CSV report
            createCSVFile(totalWordsCount);

            updateCrawlingFinished();
            done();
        }

        private void processCrawledPage(CrawledPage crawledPage)
        {
            Uri uri = crawledPage.Uri;
            string url = uri.AbsoluteUri.ToString();
            string rtfFilename = buildFileNameFromUrl(uri, ".rtf");
            string rtfFilepath = m_statsRootFolder + "/" + rtfFilename;

            HtmlAgilityPack.HtmlDocument htmlDoc = crawledPage.HtmlDocument;
            RtfFileWriter rtfFileWriter = new RtfFileWriter(rtfFilepath, m_appSettings.wordRegex);
            rtfFileWriter.Start(m_pageProcessor);

            m_pageProcessor.ProcessHtmlPage(url, htmlDoc);

            int wordsCount = rtfFileWriter.WordsCount;

            rtfFileWriter.Done();

            Log("Found " + wordsCount.ToString() + " words in page: " + url);

            updateListView(url, wordsCount, rtfFilename);
        }

        private void createCSVFile(int totalWordsCount)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new IntParameterDelegate(createCSVFile), totalWordsCount);
                return;
            }

            string csvFilepath = m_reportFolder + "/" + m_appSettings.statFilename;
            StreamWriter csvFile = new StreamWriter(csvFilepath);
            csvFile.WriteLine("Site: " + siteURL.Text);
            csvFile.WriteLine("Total number of pages: " + listViewResults.Items.Count.ToString());
            csvFile.WriteLine("Total number of words: " + totalWordsCount.ToString());
            csvFile.WriteLine();
            csvFile.WriteLine("URL,Words,RTF File");
            foreach (ListViewItem item in listViewResults.Items)
            {
                string pageUrl = item.Text;
                string pageWordsCount = item.SubItems[1].Text;
                string rtfFilename = item.SubItems[2].Text;
                csvFile.WriteLine(pageUrl + "," + pageWordsCount + "," + rtfFilename);
            }
            csvFile.Close();
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
                if (crawledPage.HttpWebResponse != null)
                {
                    msg = "Failed to crawl to page: " + crawledPage.Uri.AbsoluteUri.ToString() + ", Status: " + crawledPage.HttpWebResponse.StatusCode.ToString();
                }
                else
                {
                    msg = "Failed to crawl to page: " + crawledPage.Uri.AbsoluteUri.ToString() + ", no HTTP response";
                }
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

        #region Files and Folders
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
                    return true;
                }

                DirectoryInfo dirInfo = Directory.CreateDirectory(folderFullPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create folder " + folderFullPath + ": " + e.ToString());
                return false;
            }

            return true;
        }

        private string buildFileNameFromUrl(Uri url, string ext = ".txt")
        {
            string filename = "";

            filename = url.PathAndQuery;
            // remove prefixed "/"
            if (filename.StartsWith("/"))
            {
                filename = filename.Remove(0, 1);
            }
            filename = String.Join(".", filename.Split('/'));
            filename = filename.Replace('\\', '.');
            filename = filename.Replace('/', '.');
            filename = filename.Replace(':', '=');
            filename = filename.Replace('*', '.');
            filename = filename.Replace('"', '\'');
            filename = filename.Replace('<', 'E');
            filename = filename.Replace('>', '3');
            filename = filename.Replace('|', '!');
            filename = filename.Replace('?', '-');
            if (filename == String.Empty)
            {
                filename = "_";
            }
            filename += ext;

            return filename;
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
            progressLabel.Enabled = true;
            listViewResults.Items.Clear();
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
            progressLabel.Enabled = false;
            siteURL.Enabled = true;
            goButton.Enabled = true;
            settingsButton.Enabled = true;

            updateStatusLine("Crawling finished");
        }

        private void updateListView(string url, int wordsCount, string filename)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new UpdateListViewDelegate(updateListView), url, wordsCount, filename);
                return;
            }

            ListViewItem item = new ListViewItem(url);
            item.SubItems.Add(wordsCount.ToString());
            item.SubItems.Add(filename);
            listViewResults.Items.Add(item);
            listViewResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

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
                m_simpleLogger = new SimpleLogger(logFilePath);
            }
        }

        private void Log(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StringParameterDelegate(Log), msg);
                return;
            }

            if (m_simpleLogger == null)
            {
                createLogFile();
            }
            m_simpleLogger.Log(msg);
        }

        private void LogErr(string msg)
        {
            m_simpleLogger.LogErr(msg);
        }

        private void LogWrn(string msg)
        {
            m_simpleLogger.LogWrn(msg);
        }

        #endregion // Logger

    }
}
