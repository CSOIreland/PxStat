using API;
using PxStat.Data;
using PxStat.DataStore;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Navigation;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Threading;

namespace PxStat.Workflow
{
    class Workflow_BSO_CancelPendingLive : BaseTemplate_Create<Workflow_DTO_CancelPendingLive, Workflow_VLD_CancelPendingLive>
    {
        IDmatrix currentMatrix;
        Release_DTO currentRelease;

        /*
         Check if RlsCode is Pending Live
        Check if user is PowerUser
        Set LiveDatetimeFrom to now()
        Set LiveDatetimeTo to now() - This sets the Pending Live Release to Historical
        Update Signoff Comment to add user id and text to say this was a cancellation
        Get Current live release
        Set LiveDatetimeTo of Current Live to null
        Clone Current live to [Cancelled Release].1 (WIP)
        Email the connections
        -- Check what happens when we upload a new WP - make sure that the version is of the form [Cancelled Release].x
         */

        internal Workflow_BSO_CancelPendingLive(JSONRPC_API request) : base(request, new Workflow_VLD_CancelPendingLive())
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
    
            Release_ADO adoRelease = new Release_ADO(Ado);

            //Get the release corresponding to the RlsCode
            Release_DTO dtoReleasePendingLive = Release_ADO.GetReleaseDTO(adoRelease.Read(DTO.RlsCode, SamAccountName));

            if (dtoReleasePendingLive == null)
            {
                Log.Instance.Debug("Invalid Release");
                Response.error = Label.Get("error.update");
                return false;
            }

            //Check if it's really pending live
            Release_BSO release_BSO = new Release_BSO(Ado);
            if (!release_BSO.IsPendingLive(dtoReleasePendingLive))
            {
                Log.Instance.Debug("Invalid Release");
                Response.error = Label.Get("error.update");
                return false;
            }

            //Get the current live release corresponding to the Pending Live
            Release_DTO dtoReleaseLive = Release_ADO.GetReleaseDTO(adoRelease.ReadLiveNow(dtoReleasePendingLive.RlsCode));
            
            DateTime nowTime=DateTime.Now;
            dtoReleasePendingLive.RlsLiveDatetimeFrom = nowTime;
            dtoReleasePendingLive.RlsLiveDatetimeTo = nowTime;

            if (dtoReleaseLive != null)
            {
                dtoReleaseLive.RlsLiveDatetimeTo = default;
                adoRelease.Update(dtoReleaseLive, SamAccountName);
            }

            //Do comments
            //Update comment
            var adoComment = new Comment_ADO();


            DTO.CmmValue = String.Format(Label.Get("workflow.request.cancel-pending-live-comment", DTO.LngIsoCode),  SamAccountName) + DTO.CmmValue + " " + dtoReleasePendingLive.CmmValue;
            DTO.CmmCode = dtoReleasePendingLive.CmmCode;

            int commentResult = adoComment.Update(Ado, DTO, SamAccountName);
            //

            if (commentResult == 0)
            {
                //No existing comment to update - create a new comment
                int commentCode = adoComment.Create(Ado, DTO, SamAccountName);
                dtoReleasePendingLive.CmmCode = commentCode;
            }


            adoRelease.Update(dtoReleasePendingLive, SamAccountName);
            


            //Clone the Pending Live release
            //Release from and to will be null by default and set to revision 1,  hence wip
            Matrix_BSO mBso = new Matrix_BSO(Ado);
            int clonedRlsId = mBso.CloneReleaseToVersion(dtoReleasePendingLive.RlsCode, dtoReleasePendingLive.RlsVersion, 1, SamAccountName);
            if (clonedRlsId == 0)
            {
                Log.Instance.Debug("Release clone failed for RlsCode " + dtoReleasePendingLive.RlsCode);
                Response.error = Label.Get("error.update");
                return false;
            }

            if (dtoReleaseLive != null)
            {
                //Also clone the associated comment
                mBso.CloneComment(dtoReleaseLive.RlsCode, clonedRlsId, SamAccountName);
            }

            //Get the matrix for the pending live release and prepare to clone it
            IDmatrix pendingLiveMatrix = new Dmatrix().GetMultiLanguageMatrixFromRelease(Ado, dtoReleasePendingLive);
            if (pendingLiveMatrix == null)
            {
                Log.Instance.Debug("Failed to retrieve matrix for RlsCode " + dtoReleasePendingLive.RlsCode);
                Response.error = Label.Get("error.update");
                return false;
            }

            //Save the pending live matrix to the db as a new matrix for the wip release
            DataWriter dw = new();
            dw.CreateAndLoadDataFieldDB(Ado, pendingLiveMatrix, SamAccountName, clonedRlsId);

           
            dw.CreateAndLoadMetadata(Ado, pendingLiveMatrix);


            //Save the keywords for the new release

            Keyword_Release_BSO_CreateMandatory krBSO = new();

            krBSO.Create(Ado, pendingLiveMatrix, clonedRlsId, SamAccountName);

            krBSO.RemoveDupes(Ado, clonedRlsId);

            currentRelease = dtoReleasePendingLive;
            currentMatrix = pendingLiveMatrix;


            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
            
        }

        public override bool PostExecute()
        {
            if (Response.error != null) return false;

            //send email
            Email_BSO_NotifyWorkflow notify = new Email_BSO_NotifyWorkflow();

            Account_BSO aBso = new Account_BSO(Ado);

            var moderators = aBso.getReleaseUsers(currentRelease.RlsCode, null);
            var powerUsers = aBso.getUsersOfPrivilege(Constants.C_SECURITY_PRIVILEGE_POWER_USER);

            var sendMailThread = new Thread(() =>
            {
                //If an email error occurs, just ignore it and continue as before
                try
                {
                    notify.EmailPendingLiveCancelled(currentMatrix.Code, currentRelease, moderators, powerUsers);
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Email error: " + ex.Message);
                }
             });
            sendMailThread.Start();

            //cache flush

            Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_ADDITION + currentRelease.RlsCode);
            Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_DELETION + currentRelease.RlsCode);
            Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_AMENDMENT + currentRelease.RlsCode);

            Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + currentRelease.RlsCode);
            Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + currentRelease.RlsCode);

            return true;
        }

    }
}
