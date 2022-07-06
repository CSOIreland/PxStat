using API;
using System.Collections.Generic;
using System.Data;

namespace Px5Migrator
{
    public class Migration_ADO
    {
        internal ADO_readerOutput ReadMigrationStatus(IADO ado)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {

            };

            //Call the stored procedure
            return ado.ExecuteReaderProcedure("Migration_ReadStatus", inputParamList);

        }

        internal ADO_readerOutput ReadUnmigratedList(IADO ado)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();

            //Call the stored procedure
            return ado.ExecuteReaderProcedure("Migration_ReadUnmigrated", inputParamList);
        }


        internal ADO_readerOutput ReadNextUnmigrated(IADO ado, int mtrId = 0)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@Mark", value=false}
            };

            if (mtrId > 0)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@MtrId", value = mtrId });
            }

            //Call the stored procedure
            return ado.ExecuteReaderProcedure("Migration_Read_Mark", inputParamList);
        }

        internal ADO_readerOutput GetFrequencyDetails(IADO ado, int mtrId)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams()
                {
                    name="@MtrId",
                    value=mtrId
                }
            };

            //Call the stored procedure
            return ado.ExecuteReaderProcedure("Migration_GetFrequencyDetails", inputParamList);
        }

        internal int CreateNewMatrix(IADO ado, IDmatrix dmatrix, string username, int releaseId)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@MtrInput",value= dmatrix.MtrInput},
                new ADO_inputParams() {name ="@MtrCode",value= dmatrix.Code},
                new ADO_inputParams() {name ="@MtrTitle",value= dmatrix.Dspecs[dmatrix.Language].Title},
                new ADO_inputParams() {name ="@LngIsoCode",value= dmatrix.Language},
                new ADO_inputParams() {name ="@FrmType",value= dmatrix.FormatType},
                new ADO_inputParams() {name ="@FrmVersion",value= dmatrix.FormatVersion},
                new ADO_inputParams() {name ="@MtrNote",value= dmatrix.Dspecs[dmatrix.Language].NotesAsString},
                new ADO_inputParams() {name ="@CprValue",value= dmatrix.Copyright.CprValue},
                new ADO_inputParams() {name ="@MtrRlsId",value= releaseId},
                new ADO_inputParams() {name ="@userName",value= username}
            };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };
            ado.ExecuteNonQueryProcedure("Data_Matrix_Create", inputParams, ref returnParam);

            return (int)returnParam.value;

        }

        internal int LoadDataField(IADO ado, string data, int matrixId)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrId",value=matrixId},
                new ADO_inputParams(){ name="@MatrixData", value=data}
            };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Matrix_LoadSingleField", inputParams, ref returnParam);

            return (int)returnParam.value;

        }

        internal int DeleteOldStructureData(IADO ado)
        {
            var inputParams = new List<ADO_inputParams>()
            {

            };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Matrix_DeleteMigratedEntities", inputParams, ref returnParam);

            return (int)returnParam.value;

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

        internal int CreateDimensionDb(IADO ado, StatDimension dimension)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MdmSequence",value=dimension.Sequence },
                new ADO_inputParams(){ name ="@MdmCode", value=dimension.Code },
                new ADO_inputParams(){  name="@MdmValue", value=dimension.Value},
                new ADO_inputParams(){ name="@MtrId", value=dimension.MatrixId},
                new ADO_inputParams(){ name="@DmrValue", value=dimension.Role },
                new ADO_inputParams(){ name="@MdmGeoFlag", value=dimension.GeoFlag}


            };
            if (dimension.GeoUrl != null)
                inputParams.Add(new ADO_inputParams() { name = "@MdmGeoUrl", value = dimension.GeoUrl });

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Data_Matrix_Dimension_Create", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        internal int UpdateMatrixMigration(IADO ado, int mtrId, bool migrationFlag)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@MtrId",value= mtrId},
                new ADO_inputParams() {name ="@MigrationFlag",value= migrationFlag}

            };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };
            ado.ExecuteNonQueryProcedure("Migration_Status_Update", inputParams, ref returnParam);

            return (int)returnParam.value;

        }

        //Data Read stored procs:

        internal ADO_readerOutput DataMatrixReadByMatrixId(IADO ado, int mtrId)
        {

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrId",value=mtrId}
            };
            return ado.ExecuteReaderProcedure("Data_Migration_ReadById", inputParams);


        }

        internal ADO_readerOutput GetFieldDataForMatrix(IADO ado, int mtrId)
        {

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrId",value=mtrId}
            };
            return ado.ExecuteReaderProcedure("Data_Matrix_ReadSingleField", inputParams);


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
    }
}
