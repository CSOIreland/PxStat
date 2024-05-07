using FluentValidation;
using PxStat.Security;
using System;

namespace PxStat.Report
{
    public class TableAudit_VLD
    {
    }
    /// <summary>
    /// Validator for performance read
    /// </summary>
    public class TableAudit_VLD_Read : AbstractValidator<TableAudit_DTO_Read>
    {

        /// <summary>
        /// Check if dateTime is between start and end
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool IsBetweenTwoDates(DateTime dateTime, DateTime start, DateTime end)
        {
            return dateTime >= start && dateTime <= end;
        }

        /// <summary>
        /// This is a blank constructor. It is used for test purposes only.
        /// </summary>
        /// <param name="isTest"></param>
        public TableAudit_VLD_Read(bool isTest)
        {
        }


        public TableAudit_VLD_Read()
        {
            //DateFrom must be past DateTo
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom).WithMessage("End date must be the same or after start date.");

            // Sets the minDate to the absolute start of the day hence i.e. 00:00:00 and subtracts days from report.data-validation.minDate
            DateTime minDate = DateTime.Today.AddDays(-Configuration_BSO.GetApplicationConfigItem (ConfigType.global, "report.date-validation.minDate"));

            // Sets the maxDate to the absolute start of the day hence i.e. 00:00:00 and adds days from report.data-validation.maxDate
            DateTime maxDate = DateTime.Today.AddDays(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "report.date-validation.maxDate"));

            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime)).NotEmpty().WithMessage("*Required");
            RuleFor(x => IsBetweenTwoDates(x.DateFrom, minDate, maxDate)).NotEqual(false)
                .WithMessage(x => string.Format("Date: {0} is less than minDate: {1} or greater than maxDate: {2}", x.DateFrom, minDate, maxDate));

            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime)).NotEmpty().WithMessage("*Required");
            RuleFor(x => IsBetweenTwoDates(x.DateTo, minDate, maxDate)).NotEqual(false)
                .WithMessage(x => string.Format("Date: {0} is less than minDate: {1} or greater than maxDate: {2}", x.DateTo, minDate, maxDate));
        }
    }
}