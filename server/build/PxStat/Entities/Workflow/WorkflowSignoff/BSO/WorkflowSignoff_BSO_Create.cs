using API;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Rules for Workflow Signoff:
    /// 
    /// The actions will depend on (a) the Request type and (b) the current status of the Release
    /// 
    /// Request type PUBLISH
    /// - Release must be Work In Progress (WIP)
    /// - Set the DatetimeFrom of the release to the WorkflowRequest Date time
    /// - Update the Exceptional flag, Reservation flag, Experimental flag, Archive flag, Alert flag values of the release to the corresponding values of the Workflow Request 
    /// - If there is a previous release for that Matrix Code, set the DatetimeTo to the WorkflowRequest Date time
    /// 
    /// Request type FLAG
    /// - Release must either be LiveNow or LiveNext
    /// - Update the Exceptional flag, Reservation flag, Experimental flag, Archive flag, Alert flag values of the release to the corresponding values of the Workflow Request
    /// 
    /// Request type DELETE (not to be confused with Rollback - see below)
    /// - Release must either be LiveNow or LiveNext or WIP
    /// - If the requested release is LiveNext, set the corresponding LiveNow release DatetimeTo to NOW. Then soft delete the LiveNext release.
    /// - Alternatively, if the requested release is LiveNow, set the corresponding LiveNow release DatetimeTo to NOW.
    /// - Alternatively, if the requested release is WIP, soft delete the WIP release.
    /// - If the request release is any other status, output an error.
    /// 
    /// Request type ROLLBACK
    /// - Release must either be LiveNow or LiveNext
    /// - If the requested release is LiveNext, soft delete the LiveNext release. Then set the corresponding LiveNow release DatetimeTo to null
    /// - If the requested release is LiveNow, find the LivePrevious and soft delete the LiveNow release. Then set the DatetimeTo of LivePrevious to null. 
    /// - If no LivePrevious is found then return an error.
    /// - If the request release is any other status, output an error.
    /// </summary>
    class WorflowSignoff_BSO_Create : BaseTemplate_Create<WorkflowSignoff_DTO, WorkflowSignoff_VLD_Create>
    {


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// 

        internal WorflowSignoff_BSO_Create(JSONRPC_API request) : base(request, new WorkflowSignoff_VLD_Create())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Workflow_BSO bso = new Workflow_BSO();

            Response = bso.WorkflowSignoffCreate(Ado, DTO, SamAccountName);
            if (Response.error != null)
            {
                return false;
            }

            return true;
        }


    }
}
