using System.Collections.Generic;
using System.Web;
using ToSic.Eav.Repositories;

namespace ToSic.Dnn.DataSources
{
    /// <summary>
    /// This class simply tell the EAV / 2sxc that there is another folder 
    /// which contains content-types to load from...
    /// </summary>
    public class RegisterContentTypeForConfigUi: RepositoryInfoOfFolder
    {
        /// <summary>
        /// Empty constructor, so it can be used from reflection
        /// </summary>
        public RegisterContentTypeForConfigUi() : base(true, true, null) { }

        public override List<string> RootPaths => new List<string>
        {
            HttpContext.Current.Server.MapPath("~/desktopmodules/ToSic.Dnn.DataSources.FnL/.data")
        };

    }
}