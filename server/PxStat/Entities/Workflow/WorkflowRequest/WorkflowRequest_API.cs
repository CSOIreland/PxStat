using API;

namespace PxStat.Workflow
{
	[AllowAPICall]
    public class WorkflowRequest_API
    {
        /// <summary>
        /// Entry point to the API method - Creates a Workflow Request
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new WorkflowRequest_BSO_Create(requestApi).Create().Response;
        }

        /// <summary>
        /// Deletes a Workflow Request
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new WorkflowRequest_BSO_Delete(requestApi).Delete().Response;
        }
    }
}