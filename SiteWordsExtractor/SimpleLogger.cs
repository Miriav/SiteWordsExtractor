using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteWordsExtractor
{
    class SimpleLogger
    {
        delegate void StringParameterDelegate(string value);

        StreamWriter m_logger;
        System.Object m_lock;

        public SimpleLogger(string logFilepath)
        {
            m_logger = null;
            m_lock = new System.Object();
            createLogFile(logFilepath);
        }

        public void createLogFile(string logFilepath)
        {
            lock (m_lock)
            {
                // create the log file
                m_logger = new StreamWriter(logFilepath, false, Encoding.UTF8);
            }

            // first line in the log file
            Log("Log file created");
        }

        public void closeLogFile()
        {
            // last line in the log file
            Log("Log file closed");

            lock (m_lock)
            {
                // close the log file
                if (m_logger != null)
                {
                    m_logger.Close();
                    m_logger = null;
                }
            }
        }

        public void Log(string msg, string prefix = "")
        {
            if (m_logger == null)
            {
                return;
            }

            lock (m_lock)
            {
                if (m_logger != null)
                {
                    string timestamp = DateTime.Now.ToString("[HH:mm:ss] ");
                    m_logger.WriteLineAsync(prefix + timestamp + msg);
                }
            }
        }

        public void LogMsg(string msg)
        {
            Log(msg, "");
        }

        public void LogWrn(string msg)
        {
            Log(msg, "Warning: ");
        }

        public void LogErr(string msg)
        {
            Log(msg, "ERROR: ");
        }
    }
}
