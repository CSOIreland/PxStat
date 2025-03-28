﻿using API;
using PxStat.DataStore;
using PxStat.Resources;
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
        internal int Delete(IADO Ado, int rlsCode, string userName, bool mandatoryKeywordsOnly)
        {

            DataStore_ADO  adoMatrix = new DataStore_ADO();

            int deleted = adoMatrix.Delete(Ado,rlsCode, userName);

            Keyword_Release_ADO adoKeywordRelease = new Keyword_Release_ADO();

            deleted = adoKeywordRelease.Delete(Ado, rlsCode, null, mandatoryKeywordsOnly);

            Release_ADO adoRelease = new Release_ADO(Ado);


            deleted = adoRelease.Delete(rlsCode, userName);
            if (deleted > 0)
            {
                //Flush the cache for search - it's now out of date
               Cas.RunCasFlush(Resources.Constants.C_CAS_NAVIGATION_SEARCH);
            }
            return deleted;
        }
    }
}
