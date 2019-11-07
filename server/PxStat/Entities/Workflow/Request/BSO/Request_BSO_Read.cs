using API;
using PxStat.Data;
using PxStat.Template;
using PxStat.Resources;
using System.Collections.Generic;

namespace PxStat.Workflow
{
    /// <summary>
    /// Reads Request static data table. If no parameter is supplied, all Requests are returned
    /// If a RlsCode is supplied, the Execute function returns all Request types that can be validly applied to that Release
    /// </summary>
    internal class Request_BSO_Read : BaseTemplate_Read<Request_DTO, Request_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Request_BSO_Read(JSONRPC_API request) : base(request, new Request_VLD())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Request_ADO adoRequest = new Request_ADO();
            ADO_readerOutput result;

            if (DTO.RlsCode == 0)
            {
                //We're just looking for a full list of Requests
                result = adoRequest.Read(Ado);
                if (!result.hasData)
                    return false;
                else
                {
                    Response.data = result.data;
                    return true;
                }
            }

            else
            {
                //We want the list of Requests available to an individual Release code
                Response.data = ReadAvailableRequests();
                if (Response.data.Count == 0)
                    return false;
                else
                    return true;
            }

        }

        /// <summary>
        /// Returns the list of valid requests for a Release Code
        /// </summary>
        /// <returns></returns>
        private List<Request_DTO> ReadAvailableRequests()
        {

            Request_ADO adoRequest = new Request_ADO();

            Release_ADO adoRelease = new Release_ADO(Ado);

            ADO_readerOutput result = adoRequest.Read(Ado);

            List<Request_DTO> reqList = new List<Request_DTO>();

            List<string> list = new List<string>();

            bool isLiveNow = adoRelease.IsLiveNow(DTO.RlsCode);
            bool isLiveNext = adoRelease.IsLiveNext(DTO.RlsCode);
            bool isWip = adoRelease.IsWip(DTO.RlsCode);
            bool hasLivePrevious = adoRelease.HasPrevious(DTO.RlsCode);



            //We need to choose the correct set of valid Request types depending on the status of the Release
            if (isWip)
                list = Constants.C_WORKFLOW_REQUEST_WIP();

            else if (isLiveNow && hasLivePrevious)
                list = Constants.C_WORKFLOW_REQUEST_LIVE_NOW_WITH_PREVIOUS();

            else if (isLiveNow && !hasLivePrevious)
                list = Constants.C_WORKFLOW_REQUEST_LIVE_NOW_WITHOUT_PREVIOUS();


            else if (isLiveNext && hasLivePrevious)
                list = Constants.C_WORKFLOW_REQUEST_LIVE_NEXT_WITH_PREVIOUS();

            else if (isLiveNext && !hasLivePrevious)
                list = Constants.C_WORKFLOW_REQUEST_LIVE_NEXT_WITHOUT_PREVIOUS();

            else //default - return a blank list
                return reqList;

            //Compose the output based on what's in the constant list
            foreach (var v in result.data)
            {
                if (list.Contains(v.RqsCode))
                {
                    Request_DTO dto = new Request_DTO();
                    dto.RqsCode = v.RqsCode;
                    dto.RqsValue = v.RqsValue;
                    dto.RlsCode = DTO.RlsCode;
                    reqList.Add(dto);
                }
            }

            return reqList;

        }
    }
}