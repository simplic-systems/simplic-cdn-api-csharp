using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp.Model
{
    /// <summary>
    /// Model which pass all information for blob saving
    /// </summary>
    [VersionCompatibility(1, 0)]
    public class SaveBlobModel
    {
        /// <summary>
        /// Blob to save
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Path of the blob
        /// </summary>
        public string Path
        {
            get;
            set;
        }
    }
}
