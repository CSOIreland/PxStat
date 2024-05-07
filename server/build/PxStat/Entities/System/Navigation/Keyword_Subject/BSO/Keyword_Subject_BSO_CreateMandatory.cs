using API;
using PxStat.Security;
using System.Collections.Generic;
using System.Data;

namespace PxStat.System.Navigation
{
    internal class Keyword_Subject_BSO_CreateMandatory
    {


        private void AddToTable(ref DataTable dt, List<string> words, int sbjID)
        {
            foreach (string word in words)
            {

                //First check if the keyword is already in the table. It doesn't need to go in twice!
                string searchTerm = "KSB_VALUE = '" + word + "'";
                DataRow[] dr = dt.Select(searchTerm);
                if (dr.Length == 0)
                {
                    var row = dt.NewRow();

                    row["KSB_VALUE"] = word;
                    row["KSB_SBJ_ID"] = sbjID;
                    row["KSB_MANDATORY_FLAG"] = true;

                    dt.Rows.Add(row);
                }

            }
        }
    }
}