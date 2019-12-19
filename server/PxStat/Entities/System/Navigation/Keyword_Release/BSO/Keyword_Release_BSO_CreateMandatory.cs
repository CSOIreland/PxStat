using API;
using PxStat.Data;
using System.Collections.Generic;
using System.Data;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Creates the mandatory Release Keywords. These are based on fields in the Matrix and associated entities
    /// </summary>
    internal class Keyword_Release_BSO_CreateMandatory
    {
        /// <summary>
        /// Create a keyword release
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="releaseId"></param>
        /// <param name="userName"></param>
        internal void Create(ADO Ado, int releaseId, string userName)
        {
            Release_ADO rAdo = new Data.Release_ADO(Ado);

            var latestRelease = Release_ADO.GetReleaseDTO(rAdo.ReadID(releaseId, userName));

            if (latestRelease == null) return;

            rAdo.DeleteKeywords(latestRelease.RlsCode, userName);

            //Create the table that will be bulk inserted
            DataTable dt = new DataTable();
            dt.Columns.Add("KRL_VALUE", typeof(string));
            dt.Columns.Add("KRL_RLS_ID", typeof(int));
            dt.Columns.Add("KRL_MANDATORY_FLAG", typeof(bool));
            dt.Columns.Add("KRL_SINGULARISED_FLAG", typeof(bool));

            Keyword_Release_ADO adoKeywordCreate = new Keyword_Release_ADO();

            //Get the matrices for the RlsCode - There may be more than one e.g. if multiple languages are used in the same release
            Matrix_ADO mAdo = new Matrix_ADO(Ado);
            IList<dynamic> matrixList;

            matrixList = mAdo.ReadAllForRelease(latestRelease.RlsCode, userName);

            //Create the table rows
            foreach (dynamic item in matrixList)
            {
                //Get a Keyword Extractor - the particular version returned will depend on the language
                Keyword_BSO_Extract kbe = new Keyword_BSO_Extract(item.LngIsoCode);
                // IKeywordExtractor ext = kbe.GetExtractor();

                //Add the keywords and other data to the output table
                AddToTable(ref dt, kbe.ExtractSplitSingular(item.MtrTitle), item.RlsID);

                //Add the MtrCode to the keyword list without any transformations
                AddToTable(ref dt, new List<string>() { item.MtrCode }, item.RlsID, false);

                //Add the Copyright Code
                AddToTable(ref dt, kbe.ExtractSplit(item.CprCode), item.RlsID, false);

                //Add the Copyright Value
                AddToTable(ref dt, kbe.ExtractSplit(item.CprValue), item.RlsID, false);

                //Add the Language
                AddToTable(ref dt, kbe.ExtractSplit(item.LngIsoName), item.RlsID, false);

                //Add the Frequency Code
                //AddToTable(ref dt, ext.ExtractSplit(item.FrqCode), item.RlsID);

                //Add the Frequency Value
                AddToTable(ref dt, kbe.ExtractSplitSingular(item.FrqValue), item.RlsID);

                //Get the keywords from Statistic
                IList<dynamic> statList = mAdo.ReadStatisticByRelease(latestRelease.RlsCode, item.LngIsoCode);
                foreach (dynamic statItem in statList)
                {
                    AddToTable(ref dt, kbe.ExtractSplit(statItem.SttCode), item.RlsID, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(statItem.SttValue), item.RlsID);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(statItem.SttUnit), item.RlsID);
                }

                //Get the keywords from Classification
                IList<dynamic> classificationList = mAdo.ReadClassificationByRelease(latestRelease.RlsCode, item.LngIsoCode);
                foreach (dynamic classItem in classificationList)
                {
                    AddToTable(ref dt, kbe.ExtractSplit(classItem.ClsCode), item.RlsID, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(classItem.ClsValue), item.RlsID);

                    //Get the keywords from Variables
                    IList<dynamic> variableList = mAdo.ReadVariableByRelease(latestRelease.RlsCode, item.LngIsoCode, classItem.ClsCode);
                    foreach (dynamic variableItem in variableList)
                    {
                        AddToTable(ref dt, kbe.ExtractSplit(variableItem.VrbCode), item.RlsID, false);
                        AddToTable(ref dt, kbe.ExtractSplitSingular(variableItem.VrbValue), item.RlsID);
                    }
                }

                //Get the keywords from Period
                IList<dynamic> periodList = mAdo.ReadPeriodByRelease(latestRelease.RlsCode, item.LngIsoCode, item.FrqCode);
                foreach (dynamic periodItem in periodList)
                {
                    AddToTable(ref dt, kbe.ExtractSplit(periodItem.PrdCode), item.RlsID, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(periodItem.PrdValue), item.RlsID);
                }

            }
            //Final check to make sure there are no duplicates
            adoKeywordCreate.Create(Ado, dt.DefaultView.ToTable(true, new string[4] { "KRL_VALUE", "KRL_RLS_ID", "KRL_MANDATORY_FLAG", "KRL_SINGULARISED_FLAG" }));
        }

        /// <summary>
        /// Adds a list of words to the table
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="words"></param>
        /// <param name="rlsID"></param>
        private void AddToTable(ref DataTable dt, List<string> words, int rlsID, bool singularised = true)
        {
            foreach (string word in words)
            {

                //First check if the keyword is already in the table. It doesn't need to go in twice!
                string searchTerm = "KRL_VALUE = '" + word + "'";
                DataRow[] dr = dt.Select(searchTerm);
                if (dr.Length == 0)
                {
                    var row = dt.NewRow();

                    row["KRL_VALUE"] = word;
                    row["KRL_RLS_ID"] = rlsID;
                    row["KRL_MANDATORY_FLAG"] = true;
                    row["KRL_SINGULARISED_FLAG"] = singularised;

                    dt.Rows.Add(row);
                }

            }
        }
    }
}
