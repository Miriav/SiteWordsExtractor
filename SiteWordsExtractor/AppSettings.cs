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
            public string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
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

            public string RegExDenyURLs = @".jpg|.zip|.pdf|.exe|.doc|.flv|.swf";
        }

        public class WordsCounterSettings
        {
            public string RegEx = @"[^\s\.\$\^\{\[\(\|\)\*\+\?\\!@#%&_=;:'""`~<>\-\/]+";
        }

        public class RtfSettings
        {
            public class FontSettings
            {
                public string Name;
                public FontStyle Style;
                public float Size;
                public string Color;

                public FontSettings()
                {
                    Name = "Calibri";
                    Style = FontStyle.Regular;
                    Size = 12;
                    Color = ColorTranslator.ToHtml(System.Drawing.Color.Black);
                }

                public FontSettings(string name, FontStyle style, int size, string color)
                {
                    Name = name;
                    Style = style;
                    Size = size;
                    Color = color;
                }
            }

            public string RtfReportBaseFilename = "pages_";
            public int RtfNumberOfPagesInReport = 100;
            public int SpaceAfterParagraph = 6;

            public FontSettings TextFont = new FontSettings("Calibri", FontStyle.Regular, 12, ColorTranslator.ToHtml(Color.Black));
            public FontSettings AttributeFont = new FontSettings("Calibri", FontStyle.Regular, 12, ColorTranslator.ToHtml(Color.Gray));
            public FontSettings HyperlinkFont = new FontSettings("Calibri", FontStyle.Regular, 12, ColorTranslator.ToHtml(Color.Blue));
        }

        public ApplicationSettings Application = new ApplicationSettings();
        public HtmlSettings Html = new HtmlSettings();
        public CrawlerSettings Crawler = new CrawlerSettings();
        public WordsCounterSettings WordsCounter = new WordsCounterSettings();
        public RtfSettings Rtf = new RtfSettings();


        // global singleton application settings accessible from all modules
        public static AppSettings Settings = new AppSettings();
    }
}
