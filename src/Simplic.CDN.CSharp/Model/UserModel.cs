using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp.Model
{
    /// <summary>
    /// Represents a user in the user file
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Unique user-name
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

        /// <summary>
        /// Salt
        /// </summary>
        public string Salt
        {
            get;
            set;
        }

        /// <summary>
        /// List of roles
        /// </summary>
        public IList<string> Roles
        {
            get;
            set;
        }
    }
}
