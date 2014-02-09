using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteWordsExtractor
{
    public class AppSettings
    {
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
        /*
        // Application version
        private string version = "1.0";
        public string Version
        {
            get { return version; }
        }

        // default site URL
        private string siteURL = "";
        public string SiteURL
        {
            get { return siteURL; }
            set { siteURL = value; }
        }

#region Crawler Settings

        // maximum number of pages to browse
        private int maxPagesToCrawl = 9999;
        public int MaxPagesToCrawl
        {
            get { return maxPagesToCrawl; }
            set { maxPagesToCrawl = value; }
        }

        // HTTP timeout in seconds
        private int httpTimeoutSec = 15;
        public int HTTPTimeoutSec
        {
            get { return httpTimeoutSec; }
            set { httpTimeoutSec = value; }
        }

        // Maximum number of allowed redirects
        private int maxRedirects = 7;
        public int MaxRedirects
        {
            get { return maxRedirects; }
            set { maxRedirects = value; }
        }
#endregion
         */
    }
}
