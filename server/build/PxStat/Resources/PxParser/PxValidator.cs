using System;
using FluentValidation.Results;
using PxParser.Resources.Parser;
using PxStat.Data.Px;

namespace PxStat.Resources.PxParser
{
    /// <summary>
    /// This class is used to validate the physical level of a px file
    /// </summary>
    internal class PxValidator
    {
        internal ValidationResult ParseValidatorResult { get; private set; }
        private PxDocument PxDoc { get; set; }

        internal PxDocument ParsePxFile(string mtrInput)
        {
            ParseValidatorResult = new ValidationResult();
            try
            {
                PxDoc = PxStatEngine.ParsePxInput(mtrInput);
            }

            catch (Exception e)
            {

                ParseValidatorResult.Errors.Add(new ValidationFailure("MtrInput", Label.Get("px.parse")));
                ParseValidatorResult.Errors.Add(new ValidationFailure("MtrInput", e.Message));

                return null;
            }

            PxSchemaValidator psv = new PxSchemaValidator();
            var val = psv.Validate(PxDoc);
            if (!val.IsValid)
            {
                return null;
            }


            return PxDoc;
        }
    }
}
