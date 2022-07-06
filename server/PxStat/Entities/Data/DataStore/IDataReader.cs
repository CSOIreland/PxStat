using API;
using PxStat.Data;
using PxStat.JsonStatSchema;

namespace PxStat.DataStore
{
    public interface IDataReader
    {
        IDmatrix GetDataset(IADO ado, IMetaData metaData, string lngIsoCode, Release_DTO rDto);
        IDmatrix QueryDataset(IADO ado, IMetaData metaData, CubeQuery_DTO query, Release_DTO rDto);
        IDmatrix QueryDataset(IADO ado, IMetaData metaData, IDmatrix queryMatrix, string queryLngIsoCode);
    }
}