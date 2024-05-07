using API;
using PxStat.Data;
using PxStat.JsonStatSchema;

namespace PxStat.DataStore
{
    public interface IDataReader
    {
        IDmatrix GetDataset(IADO ado,  string lngIsoCode, Release_DTO rDto);
        IDmatrix QueryDataset(IADO ado,  CubeQuery_DTO query, Release_DTO rDto);
        IDmatrix QueryDataset(IADO ado,  IDmatrix queryMatrix, string queryLngIsoCode);
    }
}