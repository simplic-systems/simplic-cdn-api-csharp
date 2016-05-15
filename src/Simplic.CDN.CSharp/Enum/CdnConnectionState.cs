using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp
{
    /// <summary>
    /// Represents the state of a CDN instance
    /// </summary>
    public enum CdnConnectionState : byte
    {
        /// <summary>
        /// Not yet connected
        /// </summary>
        NotAuthenticated = 0,

        /// <summary>
        /// Is currently connected
        /// </summary>
        Authenticated = 1
    }
}
