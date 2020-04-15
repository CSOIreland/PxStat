using API;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources.PxParser;
using PxStat.Template;
using System.Dynamic;

namespace PxStat.Build
{
    internal class Build_BSO_ReadDatasetByAllPeriods : BaseTemplate_Read<BuildUpdate_DTO, Build_VLD_BuildReadNewPeriods>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Build_BSO_ReadDatasetByAllPeriods(JSONRPC_API request) : base(request, new Build_VLD_BuildReadNewPeriods())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //do the physical structure validation

            //This is required for validation in the Matrix code, but is not used for px build
            Request.parameters.GrpCode = Utility.GetCustomConfig("APP_DEFAULT_GROUP");
            Request.parameters.CprCode = Utility.GetCustomConfig("APP_DEFAULT_SOURCE");
            //validate the px file

            //We get the PxDocument from the validator
            PxValidator pxValidator = new PxValidator();
            PxDocument PxDoc = pxValidator.ParsePxFile(DTO.MtrInput);
            if (!pxValidator.ParseValidatorResult.IsValid)
            {
                Response.error = Error.GetValidationFailure(pxValidator.ParseValidatorResult.Errors);
                return false;
            }

            //Get this matrix from the px file 
            Matrix theMatrixData = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");

            Build_BSO bBso = new Build_BSO();


            //Get this matrix from the px file 
            theMatrixData = bBso.UpdateMatrixFromDto(theMatrixData, DTO, Ado, false);

            //We need to check the matrix in case it incurred any validation problems at the time of creation
            //If there are, then we need to return the details of these errors to the caller and terminate this process
            if (theMatrixData.ValidationResult != null)
            {
                if (!theMatrixData.ValidationResult.IsValid)
                {
                    Response.error = Error.GetValidationFailure(theMatrixData.ValidationResult.Errors);
                    return false;
                }
            }

            dynamic result = new ExpandoObject();

            result.csv = theMatrixData.GetCsvObject(DTO.LngIsoCode, true);
            result.MtrCode = theMatrixData.Code;
            Response.data = result;


            return true;
        }
    }
}
