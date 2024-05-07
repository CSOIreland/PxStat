using API;
using PxStat.Build;
using PxStat.Data;
using PxStat.Security;
using System.Collections.Generic;
using System.Data;

namespace PxStat.DataStore
{

    public class DataWriter : IDataWriter
    {
        public void CreateAndLoadDataField(IADO ado, IDmatrix matrix, string username, int releaseId)
        {
            List<dynamic> DataCells = new List<dynamic>();
            foreach (var c in matrix.Cells)
            {
                //Temporary!!!
                DataCells.Add(c.Value);//.Equals("..") ? 0 : c.Value);
            }

            Matrix_IADO mAdo = new Matrix_IADO(ado);
            Dspec dspec = new Dspec();
            string defaultLanguage = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            var valuePresent = matrix.Dspecs.TryGetValue(defaultLanguage, out dspec);

            // Create the DMatrix for the default language
            var matrixId = mAdo.CreateNewMatrix(matrix, username, releaseId, defaultLanguage);
            matrix.Id = matrixId;
            matrix.Dspecs[defaultLanguage].MatrixId = matrixId;
            mAdo.LoadDataField(Utility.JsonSerialize_IgnoreLoopingReference(DataCells), matrixId);

            // Create the new DMatrix for other languages
            foreach (KeyValuePair<string, Dspec> keyValue in matrix.Dspecs)
            {
                var language = keyValue.Key;
                if (language != defaultLanguage)
                {
                    matrixId = mAdo.CreateNewMatrix(matrix, username, releaseId, language);
                    matrix.Id = matrixId;
                    matrix.Dspecs[language].MatrixId = matrixId;
                    mAdo.LoadDataField(Utility.JsonSerialize_IgnoreLoopingReference(DataCells), matrixId);
                }
            }
        }

        public void CreateAndLoadMetadata(IADO ado, IDmatrix matrix)
        {
            DataTable dtItems = new DataTable();
            dtItems.Columns.Add("DMT_CODE");
            dtItems.Columns.Add("DMT_VALUE");
            dtItems.Columns.Add("DMT_SEQUENCE");
            dtItems.Columns.Add("DMT_MDM_ID");
            dtItems.Columns.Add("DMT_DECIMAL");
            dtItems.Columns.Add("DMT_UNIT");
            dtItems.Columns.Add("DMT_ELIMINATION_FLAG");

            Build_ADO bAdo = new Build_ADO();
            foreach (var dspec in matrix.Dspecs.Values)
            {
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
            }

            //Bulk upload the dimension items:
            bAdo.UploadDimensionItems(ado, dtItems);
        }
    }
}
