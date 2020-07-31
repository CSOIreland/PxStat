using API;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Reads Navigation
    /// </summary>
    internal class Navigation_BSO_Read : BaseTemplate_Read<Navigation_DTO_Read, Navigation_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Navigation_BSO_Read(JSONRPC_API request) : base(request, new Navigation_VLD_Read())
        { }

        /// <summary>
        /// Test authentication
        /// </summary>
        /// <returns></returns>
        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Navigation_ADO adoNav = new Navigation_ADO(Ado);
            //If we're cacheing, we only want the cache to live until the next scheduled release goes live
            //Also, if there is a next release before the scheduled cache expiry time, then we won't trust the cache
            var nextRelease = adoNav.ReadNextLiveDate(DateTime.Now);
            DateTime nextReleaseDate = default;
            if (nextRelease.hasData)
            {
                if (!nextRelease.data[0].NextRelease.Equals(DBNull.Value))
                    nextReleaseDate = Convert.ToDateTime(nextRelease.data[0].NextRelease);
            }

            //Read the cached value for this if it's available but only if there isn't a next release before the cache expiry time
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.System.Navigation", "Navigation_API", "Read", DTO);

            if (cache.hasData && nextReleaseDate >= cache.expiresAt)
            {

                Response.data = cache.data;
                return true;
            }


            //No cache available, so read from the database and cache that result

            ADO_readerOutput result = adoNav.Read(DTO);
            if (result.hasData)
            {
                List<dynamic> formattedOutput = formatOutput(result.data);


                if (nextRelease != null)
                {

                    if (nextRelease.hasData)
                    {
                        if (!nextRelease.data[0].NextRelease.Equals(DBNull.Value))
                            nextReleaseDate = Convert.ToDateTime(nextRelease.data[0].NextRelease);
                    }



                }
                Response.data = formattedOutput;

                MemCacheD.Store_BSO<dynamic>("PxStat.System.Navigation", "Navigation_API", "Read", DTO, Response.data, nextReleaseDate, Resources.Constants.C_CAS_NAVIGATION_READ);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Formats the output
        /// </summary>
        /// <param name="rawList"></param>
        /// <returns></returns>
        private List<dynamic> formatOutput(List<dynamic> rawList)
        {
            Subject_ADO sAdo = new Subject_ADO(Ado);

            Subject_DTO sDto = new Subject_DTO();
            sDto.LngIsoCode = DTO.LngIsoCode;

            List<dynamic> subjectsReadList = sAdo.Read(sDto);
            List<dynamic> outList = new List<dynamic>();

            foreach (var subject in subjectsReadList)
            {
                dynamic dSubject = new ExpandoObject();
                dSubject.SbjCode = subject.SbjCode;
                dSubject.SbjValue = subject.SbjValue;
                dSubject.product = new List<dynamic>();
                foreach (var product in rawList)
                {
                    if (product.SbjCode == dSubject.SbjCode)
                    {
                        dynamic dProduct = new ExpandoObject();
                        dProduct.PrcCode = product.PrcCode;
                        dProduct.PrcValue = product.PrcValue;
                        dProduct.PrcReleaseCount = product.PrcReleaseCount;

                        dSubject.product.Add(dProduct);
                    }
                }

                if (dSubject.product.Count > 0)
                    outList.Add(dSubject);

            }

            return outList;
        }
    }
}
