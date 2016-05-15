using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp
{
    /// <summary>
    /// Attribute which descripes the version compatibilities
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class VersionCompatibility : Attribute
    {
        private readonly int major;
        private readonly int minor;

        /// <summary>
        /// Create a new version compatibility attribute
        /// </summary>
        /// <param name="major">Major api version</param>
        /// <param name="minor">Minor api version</param>
        public VersionCompatibility(int major, int minor)
        {
            this.major = major;
            this.minor = minor;
        }

        /// <summary>
        /// Major api version
        /// </summary>
        public int Major
        {
            get
            {
                return major;
            }
        }

        /// <summary>
        /// Minor api version
        /// </summary>
        public int Minor
        {
            get
            {
                return minor;
            }
        }
    }
}
