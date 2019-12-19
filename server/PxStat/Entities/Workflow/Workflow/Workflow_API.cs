using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// These APIs control the overall access to the Workflow processes.
    /// </summary>
    public class Workflow_API
    {
        /// <summary>
        /// Entry point to the API method - Reads a Workflow
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Workflow_BSO_Read(requestApi).Read().Response;
        }


        /// <summary>
        /// Entry point to the API method - Gets a list of all Workflows waiting approval
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadAwaitingResponse(JSONRPC_API requestApi)
        {
            return new Workflow_BSO_ReadAwaitingResponse(requestApi).Read().Response;
        }

        public static dynamic ReadLive(JSONRPC_API requestApi)
        {
            return new Workflow_BSO_ReadLive(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Gets a list of all Workflows waiting signoff
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadAwaitingSignoff(JSONRPC_API requestApi)
        {
            return new Workflow_BSO_ReadAwaitingSignoff(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Reads the approval history of a Workflow
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadHistory(JSONRPC_API requestApi)
        {
            return new Workflow_BSO_ReadHistory(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Gets a list of Workflows in Progress
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadWorkInProgress(JSONRPC_API requestApi)
        {
            return new Workflow_BSO_ReadWorkInProgress(requestApi).Read().Response;
        }
    }
}
