
using AngleSharp.Text;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using FluentValidation;
using PxStat.JsonQuery;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{
    public class PxApiV1_VLD : AbstractValidator<PxApiV1_DTO>
    {
        public PxApiV1_VLD() 
        {
            RuleFor(x => x.jsonStatQueryPxApiV1).SetValidator(new JsonStatQueryPxApiV1_VLD());
        }
    }

    public class JsonStatQueryPxApiV1_VLD : AbstractValidator<JsonStatQueryPxApiV1>
    {
        public JsonStatQueryPxApiV1_VLD()
        {
            RuleForEach(x => x.Query).SetValidator(new JsonStatPxApiV1Query_VLD());
            RuleFor(x => x.Response).NotNull().NotEmpty();
            RuleFor(x => x.Response).SetValidator(new JsonStatPxApiV1Response_VLD());
        }
    }

    public class JsonStatPxApiV1Query_VLD : AbstractValidator<JsonStatPxApiV1Query>
    {
        public JsonStatPxApiV1Query_VLD()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x=>x.Selection ).NotEmpty();
            RuleFor(x => x.Selection).SetValidator(new JsonStatPxApiV1Selection_VLD());
        }
    }

    public class JsonStatPxApiV1Selection_VLD :AbstractValidator<JsonStatPxApiV1Selection>
    {
        public JsonStatPxApiV1Selection_VLD()
        {
            //If values must be an integer array unless filter is "item"
            RuleFor(x => x.Values.GetType().Equals(typeof(int[]))|| x.Filter.Equals("item")).Equals(true);
        }
    }

    public class JsonStatPxApiV1Response_VLD : AbstractValidator<JsonStatPxApiV1Response>
    {
        public JsonStatPxApiV1Response_VLD()
        {
            
            RuleFor(x=>x.Format).NotNull().NotEmpty();
            RuleFor(f => f.Format).Must(PxApiCustomValidations.ValidateFormats);


        }
    }

    public static class PxApiCustomValidations
    {
        public static bool ValidateFormats(string format)
        {

            string[] formatList = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "pxapiv1.formats");    
           return formatList.Contains(format);
        }
    }
}