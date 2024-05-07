using API;
using PxStat.Security;
using System.Collections.Generic;
using System.Data;

namespace PxStat.System.Navigation
{
    internal class Keyword_Product_BSO_CreateMandatory
    {


        private void AddToTable(ref DataTable dt, List<string> words, int rlsID)
        {
            foreach (string word in words)
            {

                //First check if the keyword is already in the table. It doesn't need to go in twice!
                string searchTerm = "KPR_VALUE = '" + word + "'";
                DataRow[] dr = dt.Select(searchTerm);
                if (dr.Length == 0)
                {
                    var row = dt.NewRow();

                    row["KPR_VALUE"] = word;
                    row["KPR_PRC_ID"] = rlsID;
                    row["KPR_MANDATORY_FLAG"] = true;

                    dt.Rows.Add(row);
                }

            }
        }
    }
}