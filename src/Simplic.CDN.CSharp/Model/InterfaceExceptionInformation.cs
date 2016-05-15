using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp.Model
{
    /// <summary>
    /// Exception information as model
    /// </summary>
    public class InterfaceExceptionInformation
    {
        /// <summary>
        /// Error code
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Additional message
        /// </summary>
        public string ExceptionMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Exception message
        /// </summary>
        public string Message
        {
            get;
            set;
        }
    }
}
