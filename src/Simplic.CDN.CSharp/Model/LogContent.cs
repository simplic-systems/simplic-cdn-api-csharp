using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp.Model
{
    /// <summary>
    /// Log message with meta information
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Log message
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Exception level
        /// </summary>
        public int Level
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Log content
    /// </summary>
    public class LogContent
    {
        /// <summary>
        /// Get a list of messages
        /// </summary>
        public IList<LogMessage> Messages
        {
            get;
            set;
        }
    }
}
