using API;
using PxStat.Data;
using PxStat.System.Settings;
using System.Collections.Generic;

namespace PxStat.DataStore
{

    public class MatrixFactory
    {
        public IDmatrix Get(IADO ado, Release_DTO rDto)
        {
            DataStore_ADO dAdo = new DataStore_ADO();
            //Get this from the factory
            IDmatrix matrix = new Dmatrix();
            matrix.Release = rDto;
            matrix.Release.Reasons = new List<string>();
            matrix.FormatType = "";
            matrix.FormatVersion = "";
            matrix.Copyright = new Copyright_DTO_Create();
            matrix.Languages = new List<string>();
            matrix.Dspecs = new Dictionary<string, Dspec>();

            if (rDto.MtrId > 0)
            {
                var dbMatrixData = dAdo.GetFieldDataForMatrix(ado, rDto.MtrId);
                matrix.Cells = (List<dynamic>)DeserializeData(dbMatrixData);

            }

            return matrix;
        }

        public IDmatrix Get(Release_DTO rDto)
        {
            DataStore_ADO dAdo = new DataStore_ADO();
            //Get this from the factory
            IDmatrix matrix = new Dmatrix();
            matrix.Release = rDto;
            matrix.Release.Reasons = new List<string>();
            matrix.FormatType = "";
            matrix.FormatVersion = "";
            matrix.Copyright = new Copyright_DTO_Create();
            matrix.Languages = new List<string>();
            matrix.Dspecs = new Dictionary<string, Dspec>();



            return matrix;
        }

        public IEnumerable<dynamic> DeserializeData(ADO_readerOutput result)
        {
            return Utility.JsonDeserialize_IgnoreLoopingReference<List<dynamic>>(result.data[0].MtdData);

        }
    }
}
