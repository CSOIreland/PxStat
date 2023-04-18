using API;
using PxStat.Data;
using PxStat.Security;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using PxStat.Data.Px;
using System;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_Validate : BaseTemplate_Read<DBuild_DTO_Validate, DBuild_VLD_Validate>
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_Validate(JSONRPC_API request) : base(request, new DBuild_VLD_Validate())
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
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            IDmatrix dmatrix = new Dmatrix();
            dynamic validationResult = new ExpandoObject();

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(DTO.MtrInput);
            var pxDocument = pxManualParser.Parse();

            // Validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Response.error = Error.GetValidationFailure(pxValidation.Errors);
                return false;
            }
            
            string lngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = DTO.FrqValueTimeval, LngIsoCode = lngIsoCode, FrqCodeTimeval = DTO.FrqCodeTimeval };
            var metaData = new MetaData();
            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument, metaData, uDto);

            if(!dmatrix.Dspecs.ContainsKey(lngIsoCode))
            {
                Response.error =String.Format( Label.Get("px.setting.language-no-default", DTO.LngIsoCode), lngIsoCode);
                return false;
            }

            bool requiresResponse = dmatrix.Dspecs[lngIsoCode].Dimensions.Where(x => x.Role.Equals("TIME")).Count() == 0;

            DMatrix_VLD validator = new DMatrix_VLD(metaData, Ado, DTO.LngIsoCode);
            // Also validate in english - just for the logs
            DMatrix_VLD dmvEn = new DMatrix_VLD(metaData, Ado);
            dmvEn.Validate(dmatrix);

            var vresult = validator.Validate(dmatrix);

            List<string> ResponseError = new List<string>();
            if (!vresult.IsValid && !requiresResponse)
            {
                ResponseError = vresult.Errors.Select(x => x.ErrorMessage).ToList();
                Response.error = ResponseError;
                return false;
            }


            //validate maps
            foreach(string lng in dmatrix.Languages)
            {
                dmatrix.Dspecs[lng].ValidateMaps();
                if(dmatrix.Dspecs[lng].ValidationErrors!=null)
                {
                    foreach(var err in dmatrix.Dspecs[lng].ValidationErrors)
                    {
                        ResponseError.Add(err.ErrorMessage);
                    }
                }
            }

            if(ResponseError.Count>0)
            {
                Response.error = ResponseError;
                return false;
            }

            //We couldn't ascertain what the time dimension is - we must return to the user to get the correct value
            if (requiresResponse)
            {
                validationResult.FrqValueCandidate = dmatrix.Dspecs[lngIsoCode].Values.Select(x => x.Key).ToList();
                validationResult.Signature = null;
            }
            else
            {
                validationResult.FrqValues = new List<string>();
                validationResult.Signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()));
            }


            Response.data = validationResult;
            return true;
        }


    }
}
