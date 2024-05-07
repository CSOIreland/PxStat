using System.Web;
using PxStat;

namespace PxStat.Security
{
    public class ServerPathProvider : IPathProvider
    {
        public string MapPath(string path)
        {
            return HttpContextHelper.Current.Server.MapPath(path);
        }
    }
}
