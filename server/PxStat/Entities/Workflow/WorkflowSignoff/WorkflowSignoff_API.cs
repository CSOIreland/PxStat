using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// These APIs control access to the Workflow-Signoff process (i.e. the final stage of a Workflow)
    /// </summary>
    public class WorkflowSignoff_API
    {

        /// <summary>
        /// Entry point to the API method - Creates a Workflow Signoff event
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        /// 
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + "/MtrCode,"
            + Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + "/MtrCode,"
            + Resources.Constants.C_CAS_NAVIGATION_SEARCH + ","
            + Resources.Constants.C_CAS_NAVIGATION_READ + ","
            + Resources.Constants.C_CAS_DATA_CUBE_READ_COLLECTION)]
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new WorflowSignoff_BSO_Create(requestApi).Create().Response;
        }

    }
}