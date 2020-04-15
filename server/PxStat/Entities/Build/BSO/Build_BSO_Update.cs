using API;
using FluentValidation;
using Newtonsoft.Json.Linq;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources.PxParser;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using static PxStat.Data.Matrix;

namespace PxStat.Build
{
    /// <summary>
    /// Updates an existing px file with new data and period metadata. This may only be used for adding or removing periods.
    /// </summary>
    internal class Build_BSO_Update : BaseTemplate_Read<BuildUpdate_DTO, Build_VLD_Update>
    {
        /// <summary>
        /// 
        /// </summary>
        List<int> divisors = new List<int>();






        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal Build_BSO_Update(JSONRPC_API request) : base(request, new Build.Build_VLD_Update())
        { }

        /// <summary>
        /// 
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
            theMatrixData = bBso.UpdateMatrixFromDto(theMatrixData, DTO, Ado);

            //We need to check the matrix in case it incurred any validation problems at the time of creation
            //If there are, then we need to return the details of these errors to the caller and terminate this process
            if (theMatrixData.ValidationResult != null)
            {
                if (!theMatrixData.ValidationResult.IsValid)
                {
                    Log.Instance.Debug(Error.GetValidationFailure(theMatrixData.ValidationResult.Errors));
                    Response.error = Label.Get("error.validation");
                    return false;
                }
            }


            if (DTO.Format.FrmType == DatasetFormat.Px)
            {
                dynamic result = new ExpandoObject();
                List<dynamic> file = new List<dynamic>();
                file.Add(theMatrixData.GetPxObject(true).ToString());
                result.file = file;
                result.report = DTO.PxData.DataItems;
                Response.data = result;

                return true;
            }

            else if (DTO.Format.FrmType == DatasetFormat.JsonStat)
            {

                //Return the metadata and data, using one json-stat object for each specification
                List<JRaw> jsons = new List<JRaw>();
                List<string> languages = new List<string>();
                if (theMatrixData.Languages != null)
                {
                    foreach (var lang in theMatrixData.Languages)
                    {
                        languages.Add(lang.ToPxValue());
                    }
                }
                else languages.Add(theMatrixData.MainSpec.Language);

                foreach (var lang in languages)
                {

                    JsonStat json = theMatrixData.GetJsonStatObject(false, true, lang);
                    jsons.Add(new JRaw(Serialize.ToJson(json)));
                }
                dynamic result = new ExpandoObject();
                result.file = jsons;
                result.report = DTO.PxData.DataItems;
                Response.data = result;
                return true;
            }

            return false;
        }



        private Specification mergeSpecsMetadata(Specification existingSpec, Specification amendedSpec)
        {
            existingSpec.Title = amendedSpec.Title;
            existingSpec.Contents = amendedSpec.Contents;
            existingSpec.Frequency.Value = amendedSpec.Frequency != null ? amendedSpec.Frequency.Value : existingSpec.Frequency.Value;
            existingSpec.NotesAsString = amendedSpec.NotesAsString != null ? amendedSpec.NotesAsString : existingSpec.NotesAsString;
            existingSpec.ContentVariable = amendedSpec.ContentVariable != null ? amendedSpec.ContentVariable : existingSpec.ContentVariable;

            if (amendedSpec.Classification != null)
            {
                foreach (ClassificationRecordDTO_Create cls in existingSpec.Classification)
                {
                    var newcls = amendedSpec.Classification.Where(x => x.Code == cls.Code).FirstOrDefault();
                    if (newcls != null)
                        cls.GeoUrl = newcls.GeoUrl;
                }
            }

            return existingSpec;
        }



        /// <summary>
        /// Get a list of periods for a specification that include the new periods in the request
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private List<PeriodRecordDTO_Create> getPeriodsForSpec(Specification spec, List<Dimension_DTO> dimensions)
        {
            List<PeriodRecordDTO_Create> periods = new List<PeriodRecordDTO_Create>();

            foreach (var dim in dimensions)
            {
                if (dim.LngIsoCode == spec.Language)
                {
                    periods = spec.Frequency.Period;
                    if (dim.Frequency != null)
                    {
                        foreach (var dimPeriod in dim.Frequency.Period)
                        {
                            if (!periods.Any(x => x.Code == dimPeriod.Code && x.Value == dimPeriod.Value))
                            {
                                periods.Add(new PeriodRecordDTO_Create() { Code = dimPeriod.Code, Value = dimPeriod.Value });
                            }
                        }
                    }
                }

            }

            return periods;
        }




    }

    /// <summary>
    /// A Dimension refers to anything with a value and code that can be used for sorting data. It can be for a Statistic, a Period or a classification
    /// It contains a list of items that are below it in the sort order (Dimensions) and the item that is above it in the sort order (ParentDimension)
    /// </summary>
    internal class Dimension
    {
        /// <summary>
        /// Code
        /// </summary>
        internal string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal List<Dimension> Dimensions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal Dimension ParentDimension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        internal Dimension(string code, string value)
        {
            Code = code;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        internal Dimension()
        {
        }
    }



}
