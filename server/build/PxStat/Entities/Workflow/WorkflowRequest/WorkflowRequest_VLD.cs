using API;
using FluentValidation;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Globalization;
using System.Linq;

namespace PxStat.Workflow
{
    /// <summary>
    /// Validator for Workflow Request Create
    /// </summary>
    internal class WorkflowRequest_VLD_Create : AbstractValidator<WorkflowRequest_DTO>
    {

        internal WorkflowRequest_VLD_Create()
        {
            //Mandatory - RqsCode - Mandatory for all RqsCodes
            RuleFor(f => f.RqsCode).Length(1, 32).WithMessage("Invalid Request Code").WithName("RequestCodeValidation");
            //Optional - CmmValue
            RuleFor(f => f.CmmValue).Length(1, 1024).When(f => !string.IsNullOrEmpty(f.CmmValue) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_ROLLBACK) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_DELETE)).WithMessage("Invalid Comment Value").WithName("CommentValueValidation");

            //Mandatory - WrqDatetime - Mandatory for Publish
            RuleFor(f => f.WrqDatetime).Must(f => !(f.Equals(default(DateTime)))).WithMessage("Invalid Request Date").WithName("WrqRequestDateValidation").When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH));

            //Date must be one of the allowed days of the week for publish (except emergencies) for publish
            RuleFor(f => f).Must(CustomValidationsWorkflowRequest.IsEmbargoDate).WithMessage("Embargo date violation").When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH)).When(f => f.WrqExceptionalFlag == false);
            //Time must be the configured allowed time (except emergencies) for publish
            RuleFor(f => f).Must(CustomValidationsWorkflowRequest.IsEmbargoTime).WithMessage("Embargo time violation").When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH)).When(f => f.WrqExceptionalFlag == false);

            RuleFor(f => f.WrqDatetime).Must(f => !(f.Equals(default(DateTime)))).WithMessage("Invalid Request Date").WithName("WrqRequestDateValidation").When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH));
            //Mandatory - RlsCode - Mandatory for All RqsCodes
            RuleFor(f => f.RlsCode).NotEmpty().WithMessage("Invalid Release Code").WithName("WrqReleaseCodeValidation");
            //Mandatory - WrqExceptionalFlag - Mandatory for Publish
            RuleFor(f => f.WrqExceptionalFlag).NotNull().When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH));
            //Mandatory - WrqReservationFlag - Mandatory for Publish and Flag
            RuleFor(f => f.WrqReservationFlag).NotNull().When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PROPERTY));
            //Mandatory - WrqArchiveFlag - Mandatory for Publish and Flag
            RuleFor(f => f.WrqArchiveFlag).NotNull().When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PROPERTY));
            //Mandatory - WrqArchiveFlag - Mandatory for Publish and Flag
            RuleFor(f => f.WrqExperimentalFlag).NotNull().When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PROPERTY));
            RuleFor(f => f).Must(HasReasonToPublish).WithMessage("The workflow release must have a Reason");
        }

        private bool HasReasonToPublish(WorkflowRequest_DTO dto)
        {
            if (!Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "workflow.release.reasonRequired")) return true;
            if (dto.RqsCode != "PUBLISH") return true;

            ReasonRelease_ADO rrAdo = new ReasonRelease_ADO();
            using (IADO ado = AppServicesHelper.StaticADO)
            {
                try
                {
                    if (!rrAdo.Read(ado, new ReasonRelease_DTO_Read() { RlsCode = dto.RlsCode, LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") }).hasData)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            return true;
        }
    }

    /// <summary>
    /// Validator for Workflow Request Update
    /// </summary>
    internal class WorkflowRequest_VLD_Update : AbstractValidator<WorkflowRequest_DTO_Update>
    {
        internal WorkflowRequest_VLD_Update()
        {
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
            //Mandatory - WrqCurrentFlag
            RuleFor(f => f.WrqCurrentFlag).NotEmpty();
        }

    }

    /// <summary>
    /// Validator for Workflow Request Delete
    /// </summary>
    internal class WorkflowRequest_VLD_Delete : AbstractValidator<WorkflowRequest_DTO>
    {
        internal WorkflowRequest_VLD_Delete()
        {
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
        }
    }

    /// <summary>
    /// Custom validations for WrqWorkflow
    /// </summary>
    internal static class CustomValidationsWorkflowRequest
    {
        /// <summary>
        /// For a workflow publish, the date must be one of the allowed days of the week (typically Monday to Friday) except for emergencies
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool IsEmbargoDate(WorkflowRequest_DTO dto)
        {
            bool isExceptional = dto.WrqExceptionalFlag.HasValue && dto.WrqExceptionalFlag.Value;

            if (isExceptional) return true;

            int[] days = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "workflow.embargo.day");

            if (days.Contains((int)dto.WrqDatetime.DayOfWeek)) return true;
            return false;
        }

        /// <summary>
        /// For a workflow publish, the time must correspond to a specific time as represented in the config (except for emergencies)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool IsEmbargoTime(WorkflowRequest_DTO dto)
        {
            bool isExceptional = dto.WrqExceptionalFlag.HasValue && dto.WrqExceptionalFlag.Value;

            DateTime etime = DateTime.ParseExact(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "workflow.embargo.time"), "HH:mm:ss",
                                        CultureInfo.InvariantCulture);

            return (etime.Hour == dto.WrqDatetime.Hour && etime.Minute == dto.WrqDatetime.Minute && etime.Second == dto.WrqDatetime.Second);
        }

        internal static bool IsNotPast(WorkflowRequest_DTO dto)
        {
            return dto.WrqDatetime > DateTime.Now;
        }
    }

}
