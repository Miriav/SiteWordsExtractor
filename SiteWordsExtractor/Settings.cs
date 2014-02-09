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
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        public void SetAppSettings(AppSettings appSettings)
        {
            defaultURL.Text = appSettings.defaultUrl;
            reportsRoot.Text = appSettings.reportsRootFolder;
            statFilename.Text = appSettings.statFilename;
            scrappedTags.Text = appSettings.scrappedHTMLTags;
            extractedAttributes.Text = appSettings.attributes;

            maxPagesPerSite.Value = appSettings.maxPagesPerSite;
            httpTimeoutSec.Value = appSettings.httpTimeoutSec;
            minHttpDelay.Value = appSettings.minHTTPdelayMs;
            maxRedirects.Value = appSettings.maxRedirects;
            maxSiteDepth.Value = appSettings.maxSiteDepth;
        }

        public AppSettings GetAppSettings()
        {
            AppSettings appSettings = new AppSettings();

            appSettings.defaultUrl = defaultURL.Text;
            appSettings.reportsRootFolder = reportsRoot.Text;
            appSettings.statFilename = statFilename.Text;
            appSettings.scrappedHTMLTags = scrappedTags.Text;
            appSettings.attributes = extractedAttributes.Text;

            appSettings.maxPagesPerSite = Convert.ToInt32(maxPagesPerSite.Value);
            appSettings.httpTimeoutSec = Convert.ToInt32(httpTimeoutSec.Value);
            appSettings.minHTTPdelayMs = Convert.ToInt32(minHttpDelay.Value);
            appSettings.maxRedirects = Convert.ToInt32(maxRedirects.Value);
            appSettings.maxSiteDepth = Convert.ToInt32(maxSiteDepth.Value);

            return appSettings;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            AppSettings defaultSettings = new AppSettings();
            SetAppSettings(defaultSettings);
        }
    }
}
