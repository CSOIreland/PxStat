using API;
using PxStat.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PxStat.Build
{
    internal class Build_ADO
    {
        internal int CreateDimensionDb(IADO ado, StatDimension dim)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MdmSequence",value=dim.Sequence },
                new ADO_inputParams(){ name ="@MdmCode", value=dim.Code },
                new ADO_inputParams(){  name="@MdmValue", value=dim.Value},
                new ADO_inputParams(){ name="@MtrId", value=dim.MatrixId},
                new ADO_inputParams(){ name="@DmrValue", value=dim.Role },
                new ADO_inputParams(){ name="@MdmGeoFlag", value=dim.GeoFlag}


            };
            if (dim.GeoUrl != null)
                inputParams.Add(new ADO_inputParams() { name = "@MdmGeoUrl", value = dim.GeoUrl });

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Data_Matrix_Dimension_Create", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }




        internal void UploadDimensionItems(IADO ado, DataTable dt)
        {
            List<KeyValuePair<string, string>> maps = new List<KeyValuePair<string, string>>();
            maps.Add(new KeyValuePair<string, string>("DMT_CODE", "DMT_CODE"));
            maps.Add(new KeyValuePair<string, string>("DMT_VALUE", "DMT_VALUE"));
            maps.Add(new KeyValuePair<string, string>("DMT_SEQUENCE", "DMT_SEQUENCE"));
            maps.Add(new KeyValuePair<string, string>("DMT_MDM_ID", "DMT_MDM_ID"));
            maps.Add(new KeyValuePair<string, string>("DMT_DECIMAL", "DMT_DECIMAL"));
            maps.Add(new KeyValuePair<string, string>("DMT_UNIT", "DMT_UNIT"));
            maps.Add(new KeyValuePair<string, string>("DMT_ELIMINATION_FLAG", "DMT_ELIMINATION_FLAG"));
            ado.ExecuteBulkCopy("TD_DIMENSION_ITEM", maps, dt, true);
        }

        internal ADO_readerOutput GetMatrixDetails(IADO ado, int rlsCode, string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@RlsCode",value=rlsCode },
                new ADO_inputParams(){ name="@LngIsoCode",value=lngIsoCode }
            };

            return ado.ExecuteReaderProcedure("Data_Matrix_Release_Read", inputParams);
        }

        internal ADO_readerOutput GetDimensionsForMatrix(IADO ado, int mtrId)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrId",value=mtrId }

            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadDimensions", inputParams);
        }

        internal ADO_readerOutput GetItemsForDimension(IADO ado, int mdmId)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MdmId",value=mdmId}
            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadDimensionItems", inputParams);
        }

        internal IEnumerable<double> GetFieldDataForMatrix(IADO ado, int mtrId)
        {
            //{
            //    var inputParams = new List<ADO_inputParams>()
            //    {
            //        new ADO_inputParams(){ name="@MtrId",value=mtrId}
            //    };

            //    var result = ado.ExecuteReaderProcedure("Data_Matrix_ReadDataField", inputParams);

            //    if (result.hasData)
            //    {
            //        byte[] readByte = (byte[])result.data[0].MtdData;

            //        var v = Encoding.UTF8.GetString(readByte, 0, readByte.Length);
            //        return Encoding.Default.GetString(readByte);
            //    }
            //    return "";
            List<double> itemList;
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrId",value=mtrId}
            };
            var result = ado.ExecuteReaderProcedure("Data_Matrix_ReadSingleField", inputParams);
            //byte[] resultData = result.data[0].MtdData;
            //using (MemoryStream ms = new MemoryStream(resultData))
            //{
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    itemList = (List<dynamic>)formatter.Deserialize(ms);
            //}
            itemList = Utility.JsonDeserialize_IgnoreLoopingReference<List<double>>(result.data[0].MtdData);

            return itemList;
        }
    }
}
