using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp.Model
{
    /// <summary>
    /// Result containing JWT
    /// </summary>
    [VersionCompatibility(1, 0)]
    public class LoginResult
    {
        /// <summary>
        /// Login token for further usage
        /// </summary>
        public string Token
        {
            get;
            set;
        }
    }
}
