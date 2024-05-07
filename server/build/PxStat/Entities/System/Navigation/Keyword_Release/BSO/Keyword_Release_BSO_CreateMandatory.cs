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

                var test = kbe.ExtractSplitSingular(item.Value.Title);
                //Add the keywords and other data to the output table
                AddToTable(ref dt, kbe.ExtractSplitSingular(item.Value.Title), releaseId);

                //Add the MtrCode to the keyword list without any transformations
                AddToTable(ref dt, new List<string>() { matrix.Code }, releaseId, false);

                //Add the Copyright Code
                AddToTable(ref dt, kbe.ExtractSplit(matrix.Copyright.CprCode, false), releaseId, false);

                //Add the Copyright Value
                AddToTable(ref dt, kbe.ExtractSplit(matrix.Copyright.CprValue), releaseId, false);



                foreach (dynamic dim in matrix.Dspecs[item.Value.Language].Dimensions)
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
