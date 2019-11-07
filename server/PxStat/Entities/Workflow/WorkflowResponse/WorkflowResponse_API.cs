using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// These APIs control access to the approval process for Workflow requests.
    /// </summary>
    internal class WorkflowResponse_API
    {
        /// <summary>
        /// Entry point to the API method - Creates a workflow response
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new WorkflowResponse_BSO_Create(requestApi).Create().Response;
        }

    }
}