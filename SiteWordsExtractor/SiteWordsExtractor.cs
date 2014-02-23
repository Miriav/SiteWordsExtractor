using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using log4net;
using log4net.Config;
using log4net.Core;
using Abot.Crawler;
using Abot.Poco;
using Abot.Core;
using System.Net;

namespace SiteWordsExtractor
{
 
    public partial class SiteWordsExtractor : Form
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(SiteWordsExtractor));

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

        private const string REPROT_FOLDER_NAME = "Report\\";
        private const string LOG_FILE_NAME = "log.txt";
        private const string GLOBAL_RFT_FILENAME = "report.rtf";

        #endregion // Constants

        #region Data Members
        
        AppSettings m_appSettings;
        string m_statsRootFolder;
        string m_reportFolder;
        BackgroundWorker m_backgroundWorker;
        List<string> m_notAllowedFileExtList;
        HtmlProcessor m_htmlProcessor;
        ListViewColumnSorter m_columnSorter;
        
        #endregion
        
        #region Counters

        private int m_linksFound;
        private int m_linksVisited;
        private int m_linksProcessed;
        private int m_linksError;
        private int m_linksSkipped;

        #endregion // Counters

        public SiteWordsExtractor()
        {
            // initialize the windows form components
            InitializeComponent();

            listViewResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // load logger configuration
            configLogger();
            log.Info("-------------------");
            log.Info("Application started");


            m_appSettings = AppSettingsStorage.Load();
            updateStatusLine("Configuration Loaded");
            siteURL.Text = m_appSettings.defaultUrl;
            progressLabel.Text = "";
            m_statsRootFolder = null;
            m_reportFolder = null;
            //m_simpleLogger = null;
            m_htmlProcessor = new HtmlProcessor();
            m_columnSorter = new ListViewColumnSorter();
            listViewResults.ListViewItemSorter = m_columnSorter;

            validateSettings();
        }

        private void configLogger()
        {
            FileInfo exePath = new FileInfo(Application.ExecutablePath);
            string log4netConfigFilename = exePath.Directory.FullName + "\\log4net.xml";
            FileInfo log4netConfigFile = new FileInfo(log4netConfigFilename);
            ICollection col = XmlConfigurator.Configure(log4netConfigFile);
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

            // validate root dir
            string rootDir = m_appSettings.reportsRootFolder;
            if (String.IsNullOrWhiteSpace(rootDir))
            {
                // if not selected, select current folder for reports
                rootDir = ".\\";
            }
            if (!(rootDir.EndsWith("\\") || (rootDir.EndsWith("/"))))
            {
                rootDir += "\\";
            }
            m_appSettings.reportsRootFolder = rootDir;

            m_notAllowedFileExtList = new List<string>(m_appSettings.fileExt.Split(','));

            m_htmlProcessor.SetAttributes(m_appSettings.attributes);

            return true;
        }

        private bool init()
        {
            log.Info("Start working on site: " + siteURL.Text);

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

            log.Debug("Initialized");

            return true;
        }

        private void done()
        {
            // close log file
            FileInfo exePath = new FileInfo(Application.ExecutablePath);
            string logFilepath = exePath.Directory.FullName + "\\" + LOG_FILE_NAME;
            log.Info("Done working on site: " + siteURL.Text);
            log.Info(progressLabel.Text);

            // invalidate the reports folder
            m_statsRootFolder = null;
            m_reportFolder = null;
        }

        private void doWork(object sender, DoWorkEventArgs e)
        {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();
            //crawlConfig.CrawlTimeoutSeconds = m_appSettings.httpTimeoutSec;
            crawlConfig.MaxConcurrentThreads = 10;
            //crawlConfig.MaxConcurrentThreads = 1;
            crawlConfig.MaxPagesToCrawl = m_appSettings.maxPagesPerSite;
            crawlConfig.UserAgentString = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.107 Safari/537.36";
            crawlConfig.HttpRequestMaxAutoRedirects = m_appSettings.maxRedirects;
            crawlConfig.MaxCrawlDepth = m_appSettings.maxSiteDepth;
            //crawlConfig.MinCrawlDelayPerDomainMilliSeconds = m_appSettings.minHTTPdelayMs;

            PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig, null, null, null, new SitePageRequester(crawlConfig), null, null, null, null);
            crawler.PageCrawlStartingAsync += crawler_PageCrawlStartingAsync;
            crawler.PageCrawlCompletedAsync += crawler_PageCrawlCompletedAsync;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowedAsync;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowedAsync;
            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                CrawlDecision decision = new CrawlDecision();
                FileInfo fi;
                string ext = "";
                string decodedUrl = "";
                try
                {
                    log.Debug("checking validity of url: " + pageToCrawl.Uri.AbsolutePath);
                    decodedUrl = WebUtility.UrlDecode(pageToCrawl.Uri.AbsolutePath);
                    if (Uri.IsWellFormedUriString(decodedUrl, UriKind.RelativeOrAbsolute))
                    {
                        fi = new FileInfo(decodedUrl);
                        ext = fi.Extension;
                    }
                    else
                    {
                        return new CrawlDecision { Allow = false, Reason = "Url is malformed" };
                    }
                }
                catch (Exception excep)
                {
                    log.Error("failed to get absolute path of url: " + excep.ToString());
                }
                if (!String.IsNullOrEmpty(ext))
                {
                    foreach (string notAllowedExt in m_notAllowedFileExtList)
                    {
                        if (ext == notAllowedExt)
                        {
                            return new CrawlDecision { Allow = false, Reason = "File extension is not allowed: " + ext };
                        }
                    }
                }

                decision = new CrawlDecision { Allow = true };

                return decision;
            });

            string globalRtfFilepath = m_reportFolder + GLOBAL_RFT_FILENAME;
            Html2Rtf globalRtf = new Html2Rtf(globalRtfFilepath, m_appSettings.wordRegex);
            globalRtf.RegisterProcessor(m_htmlProcessor);


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
            int globalWordsCount = globalRtf.WordsCount;
            log.Debug("*** Crawling completed. Elapsed time: " + elapsedTimeStr);
            log.Debug("*** Total words found in this site: " + globalWordsCount.ToString());

            // close global rtf file of site
            globalRtf.UnregisterProcessor();

            // create CSV report
            createCSVFile(globalWordsCount);

            updateCrawlingFinished();
            done();
        }

		private void processCrawledPage(CrawledPage crawledPage, string rtfFilename)
		{
			Uri uri = crawledPage.Uri;
			string url = WebUtility.UrlDecode(uri.AbsoluteUri.ToString());
			string rtfFilepath = m_statsRootFolder + rtfFilename;
			HtmlAgilityPack.HtmlDocument htmlDoc = crawledPage.HtmlDocument;

			int wordsCount = 0;

			Html2Rtf rtfFile = new Html2Rtf(rtfFilepath, m_appSettings.wordRegex);

			rtfFile.RegisterProcessor(m_htmlProcessor);
			try
			{
				m_htmlProcessor.ProcessHtmlPage(url, htmlDoc);
			}
			catch (Exception e)
			{
				log.Error("processCrawledPage: Failed to process HTML page: " + url);
				log.Error("processCrawledPage: " + e.ToString());
			}
			wordsCount = rtfFile.WordsCount;
			rtfFile.UnregisterProcessor();

			updateListView(url, wordsCount, rtfFilename);
		}

        private void createCSVFile(int totalWordsCount)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new IntParameterDelegate(createCSVFile), totalWordsCount);
                return;
            }

            string csvFilepath = m_reportFolder + m_appSettings.statFilename;
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
            log.Debug(msg);
        }

        void crawler_PageCrawlCompletedAsync(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            string msg;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                m_linksError++;
                if (crawledPage.HttpWebResponse != null)
                {
                    msg = "Failed to crawl to page: " + crawledPage.Uri.AbsoluteUri.ToString() + ", Status: " + crawledPage.HttpWebResponse.StatusCode.ToString();
                }
                else
                {
                    msg = "Failed to crawl to page: " + crawledPage.Uri.AbsoluteUri.ToString() + ", no HTTP response";
                }
                log.Error(msg);
            }
            else
            {
                msg = "page visited: " + crawledPage.Uri.AbsoluteUri.ToString();
                log.Debug(msg);
                updateStatusLine(crawledPage.Uri.AbsoluteUri.ToString());
                try
                {
                    lock (m_htmlProcessor)
                    {
                        string rtfFilename = buildFileNameFromUrl(crawledPage.Uri, m_linksProcessed + 1, ".rtf");
                        processCrawledPage(crawledPage, rtfFilename);
                        m_linksProcessed++;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("crawler_PageCrawlCompletedAsync (" + crawledPage.Uri.AbsoluteUri.ToString() + "): " + ex.ToString());
                    m_linksError++;
                }
            }

            m_linksVisited++;
            updateCrawlingProgress(-1, m_linksVisited, -1);
        }

        void crawler_PageCrawlDisallowedAsync(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            string msg;

            m_linksSkipped++;
            updateCrawlingProgress(-1, -1, m_linksSkipped);

            msg = "Did not crawl the links on page " + pageToCrawl.Uri.AbsoluteUri.ToString() + " due to " + e.DisallowedReason.ToString();
            log.Warn(msg);
        }

        void crawler_PageLinksCrawlDisallowedAsync(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            string msg;

            m_linksSkipped++;
            updateCrawlingProgress(-1, -1, m_linksSkipped);

            msg = "Did not crawl the links on page " + crawledPage.Uri.AbsoluteUri.ToString() + " due to " + e.DisallowedReason.ToString();
            log.Warn(msg);
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
                m_statsRootFolder = ".\\";
            }

            m_statsRootFolder += domainName + "\\";
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
                m_reportFolder = m_statsRootFolder + REPROT_FOLDER_NAME;
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

        private string buildFileNameFromUrl(Uri uri, int counter, string ext = ".txt")
        {
            string urlPart = "default";
            
            string str = WebUtility.UrlDecode(uri.AbsoluteUri.ToString());
            int position = str.LastIndexOf('/');
            if (position != -1)
            {
                urlPart = str.Substring(position+1);
                if (String.IsNullOrEmpty(urlPart))
                {
                    urlPart = "_";
                }
            }

            if (urlPart.Length > 20)
            {
                urlPart = urlPart.Substring(0, 20);
            }

            // remove illegal chars from the filename
            urlPart = urlPart.Replace('\\', '-');
            urlPart = urlPart.Replace('/', '-');
            urlPart = urlPart.Replace(':', '.');
            urlPart = urlPart.Replace('*', '.');
            urlPart = urlPart.Replace('?', '-');
            urlPart = urlPart.Replace('"', '\'');
            urlPart = urlPart.Replace('<', '_');
            urlPart = urlPart.Replace('>', '_');
            urlPart = urlPart.Replace('|', '_');


            // filename should be {%04d}_{%-20s}.rtf where d is "links visited" and s is the (up to) 20 first chars of the filename
            string filename = String.Format("{0:0000}{1}{2}", counter, urlPart, ext);

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
            m_linksProcessed = 0;
            m_linksError = 0;

            progressBar.Minimum = 0;
            progressBar.Value = 0;
            progressBar.Enabled = true;
            progressLabel.Enabled = true;
            listViewResults.Items.Clear();
            siteURL.Enabled = false;
            goButton.Enabled = false;
            settingsButton.Enabled = false;

            listViewResults.ColumnClick -= listViewResults_ColumnClick;

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
                // make sure not above maximum
                if (linksVisited <= progressBar.Maximum)
                {
                    progressBar.Value = linksVisited;
                }
            }

            //progressLabel.Text = "" + m_linksVisited + " (" + m_linksProcessed + "+" + m_linksError + ") / " + m_linksFound;
            progressLabel.Text = "" + m_linksFound + " links found. " + m_linksVisited + " visited (" + m_linksProcessed + " processed + " + m_linksError + " error)";
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

            listViewResults.ColumnClick += listViewResults_ColumnClick;

            updateStatusLine("Crawling finished");
        }

        void listViewResults_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //listViewResults.ListViewItemSorter = new ListViewItemComparer(e.Column);
            if (e.Column == m_columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (m_columnSorter.Order == SortOrder.Ascending)
                {
                    m_columnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    m_columnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                m_columnSorter.SortColumn = e.Column;
                m_columnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            listViewResults.Sort();
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

        }

        int minimumColumnWidth = 50;
        private void listViewResults_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (listViewResults.Columns[e.ColumnIndex].Width < minimumColumnWidth)
            {
                listViewResults.Columns[e.ColumnIndex].Width = minimumColumnWidth;
            }
        }

        #endregion // UI Update
    }
}
