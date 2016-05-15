using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp
{
    /// <summary>
    /// Interface exception which must be returned in every interface system when an exception occured
    /// </summary>
    public class InterfaceException : SystemException
    {
        /// <summary>
        /// Create exception
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="errorCode">Unique error code</param>
        public InterfaceException(string message, int errorCode, int statusCode)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Error code
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Status code
        /// </summary>
        public int StatusCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Exception message
        /// </summary>
        public override string Message
        {
            get
            {
                return String.Format("{0} - {1} @ HttpStatus: {2}", ErrorCode, base.Message, StatusCode);
            }
        }

        /// <summary>
        /// Stack trace
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public override string StackTrace
        {
            get
            {
                return base.StackTrace;
            }
        }
    }
}
