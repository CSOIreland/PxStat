using API;
using System;
using System.Collections.Generic;
using System.Data;

namespace Px5Migrator
{
    public class DataWriter : IDataWriter
    {
        public void CreateAndLoadDataField(IADO ado, IDmatrix dmatrix, string lngIsoCode)
        {
            List<dynamic> DataCells = new List<dynamic>();
            foreach (var c in dmatrix.Cells)
            {
                //Temporary!!!
                DataCells.Add(c.Value);//.Equals("..") ? 0 : c.Value);
            }

            Migration_ADO mAdo = new Migration_ADO();
            Dspec dspec = new Dspec();
            // Adjustment for unit test
            lngIsoCode = lngIsoCode ?? "en";

            //string defaultLanguage = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
            var valuePresent = dmatrix.Dspecs.TryGetValue(lngIsoCode, out dspec);

            // Create the DMatrix for the default language
            //Doesn't happen for a migration
            //var matrixId = mAdo.CreateNewMatrix(dmatrix, username, releaseId);

            mAdo.LoadDataField(ado, Utility.JsonSerialize_IgnoreLoopingReference(DataCells), dmatrix.Dspecs[lngIsoCode].MatrixId);


        }

        public void CreateAndLoadDataField(IADO ado, IDmatrix dMatrix, string username, int releaseId)
        {
            throw new NotImplementedException();
        }

        public void UpdateMatrixStatus(IADO ado, int matrixId, bool status)
        {
            Migration_ADO mAdo = new Migration_ADO();
            mAdo.UpdateMatrixMigration(ado, matrixId, status);
        }

        public void CreateAndLoadMetadata(IADO ado, IDmatrix dmatrix, string lngIsoCode)
        {
            DataTable dtItems = new DataTable();
            dtItems.Columns.Add("DMT_CODE");
            dtItems.Columns.Add("DMT_VALUE");
            dtItems.Columns.Add("DMT_SEQUENCE");
            dtItems.Columns.Add("DMT_MDM_ID");
            dtItems.Columns.Add("DMT_DECIMAL");
            dtItems.Columns.Add("DMT_UNIT");
            dtItems.Columns.Add("DMT_ELIMINATION_FLAG");

            Migration_ADO bAdo = new Migration_ADO();
            var dspec = dmatrix.Dspecs[lngIsoCode];
            foreach (var statDimension in dspec.Dimensions)
            {

                statDimension.MatrixId = dspec.MatrixId;
                statDimension.Id = bAdo.CreateDimensionDb(ado, statDimension);
                foreach (var dimensionVariable in statDimension.Variables)
                {
                    DataRow dr = dtItems.NewRow();
                    dr["DMT_CODE"] = dimensionVariable.Code;
                    dr["DMT_VALUE"] = dimensionVariable.Value;
                    dr["DMT_SEQUENCE"] = dimensionVariable.Sequence;
                    dr["DMT_MDM_ID"] = statDimension.Id;
                    dr["DMT_DECIMAL"] = dimensionVariable.Decimals;
                    dr["DMT_UNIT"] = dimensionVariable.Unit;
                    dr["DMT_ELIMINATION_FLAG"] = dimensionVariable.Elimination;
                    dtItems.Rows.Add(dr);
                }

            }

            //Bulk upload the dimension items:
            bAdo.UploadDimensionItems(ado, dtItems);
        }

        public void CreateAndLoadMetadata(IADO ado, IDmatrix dmatrix)
        {
            throw new NotImplementedException();
        }
    }
}
