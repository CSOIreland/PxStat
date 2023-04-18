using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxStat.Security
{
    public interface IPathProvider
    {
        string MapPath(string path);
    }
}
