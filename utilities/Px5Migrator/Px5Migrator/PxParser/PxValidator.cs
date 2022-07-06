using FluentValidation.Results;
using System;


namespace Px5Migrator
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

                ParseValidatorResult.Errors.Add(new ValidationFailure("MtrInput", "Parse Error"));
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
