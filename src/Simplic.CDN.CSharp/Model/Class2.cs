using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp.Model
{
    /// <summary>
    /// Result of the save-process
    /// </summary>
    [VersionCompatibility(1, 0)]
    public class SaveBlobResultModel
    {
        /// <summary>
        /// True if stored/saved successfully
        /// </summary>
        public bool SavedSuccessfully
        {
            get;
            set;
        }
    }
}
