using API;
using PxStat.System.Navigation;

namespace PxStat.Data
{
    /// <summary>
    /// Delete a Release
    /// </summary>
    internal class Release_BSO_Delete
    {
        /// <summary>
        /// Delete method
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Delete(ADO Ado, int rlsCode, string userName, bool mandatoryKeywordsOnly)
        {

            Matrix_ADO adoMatrix = new Matrix_ADO(Ado);

            int deleted = adoMatrix.Delete(rlsCode, userName);

            Keyword_Release_ADO adoKeywordRelease = new Keyword_Release_ADO();

            deleted = adoKeywordRelease.Delete(Ado, rlsCode, null, mandatoryKeywordsOnly);

            Release_ADO adoRelease = new Release_ADO(Ado);


            deleted = adoRelease.Delete(rlsCode, userName);

            return deleted;
        }
    }
}