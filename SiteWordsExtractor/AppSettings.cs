using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ESCommon.Rtf;

namespace SiteWordsExtractor
{
    public class AppSettings
    {
        public class ApplicationSettings
        {
            public readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;
            public string ReportsRootFolder = "";
            public string StatisticsFilename = "statistics.csv";
            public string ReportsFolderName = "Report";
        }

        public class HtmlSettings
        {
            public string ParagraphTags = "p,br,dl,div,h1,h2,h3,h4,h5,h6,li,ul,ol,table,tr,td,th,tbody,thead";
            public string BoldTextTags = "b,em,strong";
            public string IgnoredTags = "script,style,#comment";
            public string InputTags = "input,textarea";
            public string HyperlinkTags = "a";
            public string Attributes = "value,alt,title";
        }

        public class CrawlerSettings
        {
            public int MaxConcurrentThreads = 10;
            public int MaxPagesToCrawl = 9999;
            public string UserAgentString = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.107 Safari/537.36";
            public int HttpRequestMaxAutoRedirects = 7;
            public int MaxCrawlDepth = 10;

            public string RegExDenyURLs = @"\.jpg|\.zip|\.pdf";
        }

        public class WordsCounterSettings
        {
            public string RegEx = @"[^\s\.\$\^\{\[\(\|\)\*\+\?\\!@#%&_=;:'""`~<>\-\/]+";
        }

        public class RtfSettings
        {
            public string RtfReportFilename = "report.rtf";
            public string RtfReportBaseFilename = "report_";
            public int RtfNumberOfPagesInReport = 100;
            public int SpaceAfterParagraph = 6;

            public string TextFontName = "Calibri";
            public int TextFontSize = 12;
            public string TextFontColor = ColorTranslator.ToHtml(Color.Black);

            public string AttributeFontName = "Calibri";
            public int AttributeFontSize = 12;
            public string AttributeFontColor = ColorTranslator.ToHtml(Color.Gray);

            public string HyperlinkFontName = "Calibri";
            public int HyperlinkFontSize = 12;
            public string HyperlinkFontColor = ColorTranslator.ToHtml(Color.Blue);
        }

        public ApplicationSettings Application = new ApplicationSettings();
        public HtmlSettings Html = new HtmlSettings();
        public CrawlerSettings Crawler = new CrawlerSettings();
        public WordsCounterSettings WordsCounter = new WordsCounterSettings();
        public RtfSettings Rtf = new RtfSettings();
        

        #region Application Settings

        // applicatoin version
        public string version = "1.0";

        // default url
        public string defaultUrl = "";

        // Root folder for site reports
        public string reportsRootFolder = "";

        // Default file name for statistics CSV report
        public string statFilename = "statistics.csv";

        // List of Scrapped HTML tags (default is <script>, <style>, <comment>).
        public string scrappedHTMLTags = "script,style,#comment";

        // List of HTML tags to extract (default is ALT, TITLE).
        public string attributes = "alt,title";

        // List of not allowed file extensions
        public string fileExt = ".jpg,.png,.gif,.zip,.pdf";

        // regular expression to be used when counting words in a page
        public string wordRegex = @"[^\s\.\$\^\{\[\(\|\)\*\+\?\\!@#%&_=;:'""`~<>\-\/]+";

        #endregion

        #region Crawler Settings

        // maximum number of pages to crawl in each site
        public int maxPagesPerSite = 9999;

        // HTTP timeout, default is 15 seconds
        public int httpTimeoutSec = 15;

        // Maximum number of allowed redirects
        public int maxRedirects = 7;

        // Maximum depth to crawl in site (default is 100 folders).
        public int maxSiteDepth = 100;

        // Minimum delay per HTTP request (default is 1000 miliseconds).
        public int minHTTPdelayMs = 1000;



        #endregion

    }
}
