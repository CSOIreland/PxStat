using API;
using Newtonsoft.Json.Linq;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources.PxParser;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;

namespace PxStat.Build
{
    /// <summary>
    /// Get a Json stat version of a px file
    /// </summary>
    internal class Build_BSO_Read : BaseTemplate_Read<Build_DTO_Read, Build_Read_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Build_BSO_Read(JSONRPC_API request) : base(request, new Build_Read_VLD())
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
            dynamic result = new ExpandoObject();



            //This is required for validation in the Matrix code, but is not used for px build
            Request.parameters.GrpCode = Utility.GetCustomConfig("APP_DEFAULT_GROUP");
            Request.parameters.source = Utility.GetCustomConfig("APP_DEFAULT_SOURCE");

            //We get the PxDocument from the validator
            PxValidator pxValidator = new PxValidator();
            PxDocument PxDoc = pxValidator.ParsePxFile(DTO.MtrInput);
            if (!pxValidator.ParseValidatorResult.IsValid)
            {
                Response.error = Error.GetValidationFailure(pxValidator.ParseValidatorResult.Errors);
                return false;
            }



            //Get this matrix from the px file , but we also need to pass in the Timeval stuff
            //The "" bit is temporary until we make the parameters optional (Currently this interferes with existing overloads)
            Matrix matrixPxFile = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");


            List<dynamic> cells = new List<dynamic>();

            matrixPxFile.Cells = cells;

            List<JRaw> jsonData = new List<JRaw>();

            var jsonStat = matrixPxFile.GetJsonStatObject(false, false);

            jsonData.Add(new JRaw(Serialize.ToJson(jsonStat)));

            if (matrixPxFile.OtherLanguageSpec != null)
            {
                foreach (Matrix.Specification s in matrixPxFile.OtherLanguageSpec)
                {
                    matrixPxFile.MainSpec = s;
                    jsonStat = matrixPxFile.GetJsonStatObject(false, false);
                    jsonData.Add(new JRaw(Serialize.ToJson(jsonStat)));
                }
            }

            Response.data = jsonData;

            return true;
        }
    }
}
