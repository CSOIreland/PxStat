using API;
using FluentValidation.Results;
using PxStat.Template;
using System.Collections.Generic;

namespace PxStat.Data
{
    internal class GeoMap_BSO_Validate : BaseTemplate_Read<GeoMap_DTO_Validate, GeoMap_VLD_Validate>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GeoMap_BSO_Validate(JSONRPC_API request) : base(request, new GeoMap_VLD_Validate())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            GeoMapValidator mapValidator = new GeoMapValidator();
            ValidationResult vr = mapValidator.Validate(DTO);
            List<string> ValidationResults = new List<string>();

            if (!vr.IsValid)
            {
                foreach (var er in vr.Errors)
                    ValidationResults.Add(er.ErrorMessage);
            }

            if (DTO.GeoMapErrorMessage != null)
            {
                ValidationResults.Add(DTO.GeoMapErrorMessage);

            }

            if (ValidationResults.Count > 0)
            {
                Response.data = ValidationResults;
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}
