using API;
using PxStat.Security;
using System.Collections.Generic;
using System.Data;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Creates a Mandatory Keyword Subject
    /// </summary>
    internal class Keyword_Subject_BSO_Mandatory
    {
        internal void Create(ADO Ado, Subject_DTO subjectDto, int subjectID)
        {
            //There is no direct means of finding out which langauge the product name uses,
            // so we take a default language from the settings
            string languageCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

            //Create the table that will be bulk inserted
            DataTable dt = new DataTable();
            dt.Columns.Add("KSB_VALUE", typeof(string));
            dt.Columns.Add("KSB_SBJ_ID", typeof(int));
            dt.Columns.Add("KSB_MANDATORY_FLAG", typeof(bool));

            Keyword_Subject_ADO keywordSubjectAdo = new Keyword_Subject_ADO(Ado);


            //Get a Keyword Extractor - the particular version returned will depend on the language
            Keyword_BSO_Extract kbe = new Navigation.Keyword_BSO_Extract(languageCode);
            //IKeywordExtractor ext = kbe.GetExtractor();
            AddToTable(ref dt, kbe.ExtractSplitSingular(subjectDto.SbjValue), subjectID);

            keywordSubjectAdo.Create(dt);

        }

        /// <summary>
        /// Deletes a Keyword subject
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="subjectDto"></param>
        /// <param name="mandatoryOnly"></param>
        /// <returns></returns>
        internal int Delete(ADO Ado, Subject_DTO subjectDto, bool? mandatoryOnly = null)
        {
            Keyword_Subject_ADO ksAdo = new Keyword_Subject_ADO(Ado);
            Keyword_Subject_DTO ksDto = new Keyword_Subject_DTO();
            ksDto.SbjCode = subjectDto.SbjCode;
            if (mandatoryOnly != null)
                return ksAdo.Delete(ksDto, mandatoryOnly);
            else
                return ksAdo.Delete(ksDto);
        }

        /// <summary>
        /// Adds a list of words to a datatable for upload to the Keyword Subject table
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="words"></param>
        /// <param name="sbjID"></param>
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
