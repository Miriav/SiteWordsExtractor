using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using log4net;

namespace SiteWordsExtractor
{
    class WordsCounter
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(WordsCounter));

        Regex m_regex;

        /*
         * Constructor. initialize the object with the pattern for the regular expression
         * the regular expression should be an application configuration parameter
         */
        public WordsCounter(string pattern)
        {
            m_regex = new Regex(pattern, RegexOptions.None);
            log.Debug("Build new regular expression: " + pattern);
        }

        /*
         * count the words in a string using the pre-compiled regular expression.
         */
        public int CountWords(string s)
        {
            MatchCollection matches = m_regex.Matches(s);
            int count = matches.Count;

            log.Debug(count.ToString() + " words in text: " + s);

            return count;
        }

        /*
         this is a static version of the object's method. it is slower becuase it
         recompile regular expression in each execution.
         */
        public static int CountWords(string s, string regex)
        {
            MatchCollection matches = Regex.Matches(s, regex);
            int count = matches.Count;

            log.Debug(count.ToString() + " words in text: " + s);

            return count;
        }
    }
}
