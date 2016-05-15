using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp.Model
{
    /// <summary>
    /// Result of a ping request process
    /// </summary>
    [VersionCompatibility(1, 0)]
    public class PingResult
    {
        /// <summary>
        /// Result as string, called pong
        /// </summary>
        public string Pong
        {
            get;
            set;
        }
    }
}
