using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SiteWordsExtractor
{
    class AppSettingsStorage
    {
        private const string DEFAULT_FILENAME = "settings.json";

        static public void Save(AppSettings appSettings, string filename = DEFAULT_FILENAME)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            File.WriteAllText(filename, serializer.Serialize(appSettings));
        }

        static public AppSettings Load(string filename = DEFAULT_FILENAME)
        {
            AppSettings appSettings = new AppSettings();
            if (File.Exists(filename))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                appSettings = serializer.Deserialize<AppSettings>(File.ReadAllText(filename));
            }
            return appSettings;
        }
    }
}
