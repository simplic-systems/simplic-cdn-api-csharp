using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp.Model
{
    /// <summary>
    /// Login model
    /// </summary>
    [VersionCompatibility(1, 0)]
    public class LoginModel
    {
        /// <summary>
        /// Unique username
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get;
            set;
        }
    }
}
