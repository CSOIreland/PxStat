using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// These APIs control access to the Workflow-Signoff process (i.e. the final stage of a Workflow)
    /// </summary>
	
	[AllowAPICall]
    public class WorkflowSignoff_API
    {

        /// <summary>
        /// Entry point to the API method - Creates a Workflow Signoff event
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        /// 

        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new WorflowSignoff_BSO_Create(requestApi).Create().Response;
        }

    }
}
