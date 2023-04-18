using API;
using PxStat.Security;
using PxStat.Template;
using System;

namespace PxStat.Subscription
{
    internal class Query_BSO_Create : BaseTemplate_Create<Query_DTO_Create, Query_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Query_BSO_Create(JSONRPC_API request) : base(request, new Query_VLD_Create())
        {
        }
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
            if (SamAccountName == null)
            {

                if (!API.Firebase.Authenticate(DTO.Uid, DTO.AccessToken))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }

            int queryThreshold = Configuration_BSO.GetCustomConfig(ConfigType.server, "subscription.query-threshold");

            if (!IsValidBase64String(DTO.Snippet.Query.ToString()))
            {
                Log.Instance.Debug("Snippet Base64 string is not valid");
                Response.error = Label.Get("error.subscription.invalid-snippet-base64");
                return false;
            }
            Query_ADO ado = new Query_ADO(Ado);
            int userQueryId = ado.Create(DTO.TagName, DTO.Matrix, DTO.Snippet, queryThreshold, DTO.Uid, SamAccountName);

            if (userQueryId > 0)
            {
                Response.data = JSONRPC.success;
                return true;
            }
            else if (userQueryId == -1) // Duplicate tag name
            {
                Log.Instance.Debug("Cannot create duplicate tag name for the query");
                Response.error = Label.Get("error.subscription.duplicate-tagname");
                return false;
            }
            else if (userQueryId == -2) // Maximum query threshold reached
            {
                Log.Instance.Debug("Maximum query threshold reached");
                Response.error = Label.Get("error.subscription.query-threshold");
                return false;
            }

            Log.Instance.Debug("Can't create Subscription Query");
            Response.error = Label.Get("error.create");
            return false;
        }

        public bool IsValidBase64String(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            string test = Convert.ToBase64String(data);
            return test.Equals(encodedString);
        }
    }

}
