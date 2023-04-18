using API;
using PxStat.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxStat.DataStore
{
    public interface IDataWriter
    {
        void CreateAndLoadDataField(IADO ado, IDmatrix matrix, string username, int releaseId);
        void CreateAndLoadMetadata(IADO ado, IDmatrix matrix);
    }
}
