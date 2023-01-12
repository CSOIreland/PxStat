using API;
using PxStat.Data;
using PxStat.DataStore;
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
                AddToTable(ref dt, kbe.ExtractSplit(item.CprCode, false), item.RlsID, false);

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
                    AddToTable(ref dt, kbe.ExtractSplit(statItem.SttCode, false), item.RlsID, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(statItem.SttValue), item.RlsID);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(statItem.SttUnit), item.RlsID);
                }



                //Get the keywords from Classification
                IList<dynamic> classificationList = mAdo.ReadClassificationByRelease(latestRelease.RlsCode, item.LngIsoCode);
                foreach (dynamic classItem in classificationList)
                {


                    AddToTable(ref dt, kbe.ExtractSplit(classItem.ClsCode, false), item.RlsID, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(classItem.ClsValue), item.RlsID);

                    //Get the keywords from Variables
                    IList<dynamic> variableList = mAdo.ReadVariableByRelease(latestRelease.RlsCode, item.LngIsoCode, classItem.ClsCode);

                    foreach (dynamic variableItem in variableList)
                    {
                        AddToTable(ref dt, kbe.ExtractSplit(variableItem.VrbCode, false), item.RlsID, false);
                        AddToTable(ref dt, kbe.ExtractSplitSingular(variableItem.VrbValue), item.RlsID);

                    }

                }

                //Get the keywords from Period
                IList<dynamic> periodList = mAdo.ReadPeriodByRelease(latestRelease.RlsCode, item.LngIsoCode, item.FrqCode);
                foreach (dynamic periodItem in periodList)
                {
                    AddToTable(ref dt, kbe.ExtractSplit(periodItem.PrdCode, false), item.RlsID, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(periodItem.PrdValue), item.RlsID);
                }

            }
            var distinctRows = dt.DefaultView.ToTable(true, new string[4] { "KRL_VALUE", "KRL_RLS_ID", "KRL_MANDATORY_FLAG", "KRL_SINGULARISED_FLAG" });
            //Final check to make sure there are no duplicates
            adoKeywordCreate.Create(Ado, distinctRows);
        }


        internal void Create(ADO Ado, int releaseId, string userName, Matrix matrix)
        {
            Release_ADO rAdo = new Data.Release_ADO(Ado);

            var latestRelease = Release_ADO.GetReleaseDTO(rAdo.ReadID(releaseId, userName));

            if (latestRelease == null) return;


            //Create the table that will be bulk inserted
            DataTable dt = new DataTable();
            dt.Columns.Add("KRL_VALUE", typeof(string));
            dt.Columns.Add("KRL_RLS_ID", typeof(int));
            dt.Columns.Add("KRL_MANDATORY_FLAG", typeof(bool));
            dt.Columns.Add("KRL_SINGULARISED_FLAG", typeof(bool));

            Keyword_Release_ADO adoKeywordCreate = new Keyword_Release_ADO();



            List<Matrix.Specification> specList = new List<Matrix.Specification>() { matrix.MainSpec };
            if (matrix.OtherLanguageSpec != null)
            {
                foreach (Matrix.Specification lspec in matrix.OtherLanguageSpec)
                {
                    specList.Add(lspec);
                }
            }

            //Create the table rows
            foreach (var spec in specList)
            {
                //spec = matrix.GetSpecFromLanguage(item.LngIsoCode);
                //Get a Keyword Extractor - the particular version returned will depend on the language
                Keyword_BSO_Extract kbe = new Keyword_BSO_Extract(spec.Language);


                //Add the keywords and other data to the output table
                AddToTable(ref dt, kbe.ExtractSplitSingular(spec.Title), releaseId);

                //Add the MtrCode to the keyword list without any transformations
                AddToTable(ref dt, new List<string>() { spec.MatrixCode }, releaseId, false);

                //Add the Copyright Code
                AddToTable(ref dt, kbe.ExtractSplit(matrix.Copyright.CprCode, false), releaseId, false);

                //Add the Copyright Value
                AddToTable(ref dt, kbe.ExtractSplit(matrix.Copyright.CprValue), releaseId, false);

                //Add the Language
                //AddToTable(ref dt, kbe.ExtractSplit(item.LngIsoName), item.RlsID, false);

                //Add the Frequency Code
                AddToTable(ref dt, kbe.ExtractSplit(spec.Frequency.Code, false), releaseId);

                //Add the Frequency Value
                AddToTable(ref dt, kbe.ExtractSplitSingular(spec.Frequency.Value), releaseId);

                //Get the keywords from Statistic
                foreach (dynamic statItem in spec.Statistic)
                {
                    AddToTable(ref dt, kbe.ExtractSplit(statItem.Code, false), releaseId, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(statItem.Value), releaseId);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(statItem.Unit), releaseId);
                }



                //Get the keywords from Classification
                foreach (dynamic classItem in spec.Classification)
                {


                    AddToTable(ref dt, kbe.ExtractSplit(classItem.Code, false), releaseId, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(classItem.Value), releaseId);

                    //Get the keywords from Variables

                    foreach (dynamic variableItem in classItem.Variable)
                    {
                        AddToTable(ref dt, kbe.ExtractSplit(variableItem.Code, false), releaseId, false);
                        AddToTable(ref dt, kbe.ExtractSplitSingular(variableItem.Value), releaseId);

                    }

                }

                //Get the keywords from Period
                foreach (dynamic periodItem in spec.Frequency.Period)
                {
                    AddToTable(ref dt, kbe.ExtractSplit(periodItem.Code, false), releaseId, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(periodItem.Value), releaseId);
                }

            }
            var distinctRows = dt.DefaultView.ToTable(true, new string[4] { "KRL_VALUE", "KRL_RLS_ID", "KRL_MANDATORY_FLAG", "KRL_SINGULARISED_FLAG" });
            //Final check to make sure there are no duplicates
            adoKeywordCreate.Create(Ado, distinctRows);


        }

        internal void RemoveDupes(IADO ado, int releaseId)
        {
            Keyword_Release_ADO krAdo = new Keyword_Release_ADO();
            krAdo.RemoveDupes(ado, releaseId);
        }

        internal void Create(IADO ado, IDmatrix matrix, int releaseId, string userName)
        {
            Release_ADO rAdo = new Data.Release_ADO(ado);
            DataStore_ADO dAdo = new DataStore_ADO();

            var latestRelease = Release_ADO.GetReleaseDTO(rAdo.ReadID(releaseId, userName));


            if (latestRelease == null) return;


            //Create the table that will be bulk inserted
            DataTable dt = new DataTable();
            dt.Columns.Add("KRL_VALUE", typeof(string));
            dt.Columns.Add("KRL_RLS_ID", typeof(int));
            dt.Columns.Add("KRL_MANDATORY_FLAG", typeof(bool));
            dt.Columns.Add("KRL_SINGULARISED_FLAG", typeof(bool));



            //Create the table rows
            foreach (var item in matrix.Dspecs)
            {

                //Get a Keyword Extractor - the particular version returned will depend on the language
                Keyword_BSO_Extract kbe = new Keyword_BSO_Extract(item.Value.Language);

                //Add the keywords and other data to the output table
                AddToTable(ref dt, kbe.ExtractSplitSingular(item.Value.Title), releaseId);

                //Add the MtrCode to the keyword list without any transformations
                AddToTable(ref dt, new List<string>() { matrix.Code }, releaseId, false);

                //Add the Copyright Code
                AddToTable(ref dt, kbe.ExtractSplit(matrix.Copyright.CprCode, false), releaseId, false);

                //Add the Copyright Value
                AddToTable(ref dt, kbe.ExtractSplit(matrix.Copyright.CprValue), releaseId, false);



                foreach (dynamic dim in matrix.Dspecs[matrix.Language].Dimensions)
                {


                    AddToTable(ref dt, kbe.ExtractSplit(dim.Code, false), releaseId, false);
                    AddToTable(ref dt, kbe.ExtractSplitSingular(dim.Value), releaseId);


                    foreach (dynamic variableItem in dim.Variables)
                    {
                        AddToTable(ref dt, kbe.ExtractSplit(variableItem.Code, false), releaseId, false);
                        AddToTable(ref dt, kbe.ExtractSplitSingular(variableItem.Value), releaseId);

                    }

                }

            }
            DataTable distinctRows = dt.DefaultView.ToTable(true, new string[4] { "KRL_VALUE", "KRL_RLS_ID", "KRL_MANDATORY_FLAG", "KRL_SINGULARISED_FLAG" });
            //Final check to make sure there are no duplicates


            dAdo.CreateKeywordsFromTable(ado, distinctRows);

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

                string searchTerm = "KRL_VALUE = '" + word + "'";

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
