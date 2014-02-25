using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Abot.Core;
using Abot.Poco;
using log4net;
using System.IO;
using System.Text.RegularExpressions;

namespace SiteWordsExtractor
{
    class SitePageRequester : IPageRequester
    {

        static ILog log = LogManager.GetLogger(typeof(SitePageRequester));

        protected CrawlConfiguration _config;
        protected string _userAgentString;

        public SitePageRequester(CrawlConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            _userAgentString = config.UserAgentString;
            _config = config;

            if (_config.HttpServicePointConnectionLimit > 0)
                ServicePointManager.DefaultConnectionLimit = _config.HttpServicePointConnectionLimit;
        }

        /// <summary>
        /// Make an http web request to the url and download its content
        /// </summary>
        public virtual CrawledPage MakeRequest(Uri uri)
        {
            return MakeRequest(uri, (x) => new CrawlDecision { Allow = true });
        }

        /// <summary>
        /// Make an http web request to the url and download its content based on the param func decision
        /// </summary>
        public virtual CrawledPage MakeRequest(Uri uri, Func<CrawledPage, CrawlDecision> shouldDownloadContent)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            CrawledPage crawledPage = new CrawledPage(uri);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = BuildRequestObject(uri);
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                crawledPage.WebException = e;

                if (e.Response != null)
                    response = (HttpWebResponse)e.Response;

                log.DebugFormat("Error occurred requesting url [{0}]", uri.AbsoluteUri);
                log.Debug(e);
            }
            catch (Exception e)
            {
                log.DebugFormat("Error occurred requesting url [{0}]", uri.AbsoluteUri);
                log.Debug(e);
            }
            finally
            {
                crawledPage.HttpWebRequest = request;

                if (response != null)
                {
                    crawledPage.HttpWebResponse = response;
                    CrawlDecision shouldDownloadContentDecision = shouldDownloadContent(crawledPage);
                    if (shouldDownloadContentDecision.Allow)
                    {
                        crawledPage.RawContent = ProcessResponse(response);
                        crawledPage.PageSizeInBytes = Encoding.UTF8.GetBytes(crawledPage.RawContent).Length;
                    }
                    else
                    {
                        log.DebugFormat("Links on page [{0}] not crawled, [{1}]", crawledPage.Uri.AbsoluteUri, shouldDownloadContentDecision.Reason);
                    }
                    response.Close();
                }
            }
            
            return crawledPage;
        }

        protected virtual HttpWebRequest BuildRequestObject(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AllowAutoRedirect = _config.IsHttpRequestAutoRedirectsEnabled;
            request.UserAgent = _userAgentString;
            request.Accept = "*/*";

            if(_config.HttpRequestMaxAutoRedirects > 0)
                request.MaximumAutomaticRedirections = _config.HttpRequestMaxAutoRedirects;

            if (_config.IsHttpRequestAutomaticDecompressionEnabled)
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if(_config.HttpRequestTimeoutInSeconds > 0)
                request.Timeout = _config.HttpRequestTimeoutInSeconds * 1000;

            return request;
        }

        private string ProcessResponse(HttpWebResponse response)
        {
            Encoding headerEncoding = GetEncodingFromHeader(response);

            Stream s = response.GetResponseStream();
            MemoryStream memStream = new MemoryStream();
            int bytesRead;
            byte[] buffer = new byte[0x1000];
            for (bytesRead = s.Read(buffer, 0, buffer.Length); bytesRead > 0; bytesRead = s.Read(buffer, 0, buffer.Length))
            {
                memStream.Write(buffer, 0, bytesRead);
            }
            s.Close();

            string html;
            memStream.Position = 0;
            using (StreamReader r = new StreamReader(memStream, headerEncoding))
            {
                html = r.ReadToEnd().Trim();
                html = CheckMetaCharSetAndReEncode(headerEncoding, memStream, html);
            }

            return html;
        }

        private Encoding GetEncodingFromHeader(HttpWebResponse response)
        {
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            string charset = null;
            if (string.IsNullOrEmpty(response.CharacterSet))
            {
                Match m = Regex.Match(response.ContentType, @";\s*charset\s*=\s*(?<charset>.*)", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    charset = m.Groups["charset"].Value.Trim(new[] { '\'', '"' });
                }
            }
            else
            {
                charset = response.CharacterSet;
            }
            if (!string.IsNullOrEmpty(charset))
            {
                try
                {
                    encoding = Encoding.GetEncoding(charset);
                }
                catch (ArgumentException)
                {
                }
            }

            return encoding;
        }

        private string CheckMetaCharSetAndReEncode(Encoding originalEncoding, MemoryStream memStream, string html)
        {
            Match m = new Regex(@"<meta\s+.*?charset\s*=\s*(?<charset>[A-Za-z0-9_-]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(html);
            if (m.Success)
            {
                string charset = m.Groups["charset"].Value.ToLower() ?? "iso-8859-1";
                if ((charset == "unicode") || (charset == "utf-16"))
                {
                    charset = "utf-8";
                }

                try
                {
                    Encoding metaEncoding = Encoding.GetEncoding(charset);
                    if (originalEncoding != metaEncoding)
                    {
                        memStream.Position = 0L;
                        StreamReader recodeReader = new StreamReader(memStream, metaEncoding);
                        html = recodeReader.ReadToEnd().Trim();
                        recodeReader.Close();
                    }
                }
                catch (ArgumentException)
                {
                }
            }

            return html;
        }

    }
}
