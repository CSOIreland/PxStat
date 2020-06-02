using API;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Globalization;

namespace PxStat.Resources
{
    /// <summary>
    /// Validator extension (IsOptional) 
    /// </summary>
    public static partial class ValidatorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> IsOptional<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IsOptionalValidator());

        }
    }

    /// <summary>
    /// Propery validator
    /// </summary>
    internal class IsOptionalValidator : PropertyValidator
    {
        /// <summary>
        /// 
        /// </summary>
        public IsOptionalValidator() : base("")
        {

        }

        /// <summary>
        /// Is Valid extension
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            return true;
        }
    }

    /// <summary>
    /// Class to ensure that valid values are returned from the database to application classes
    /// </summary>
    internal class DataAdaptor
    {
        /// <summary>
        /// Format boolean reads
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public bool ReadBool(dynamic obj, bool defaultValue = false)
        {
            return obj != null && obj.GetType() != typeof(DBNull) ? obj : defaultValue;
        }

        /// <summary>
        /// Format string reads
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public string ReadString(dynamic obj, string defaultValue = "")
        {
            return obj != null && obj.GetType() != typeof(DBNull) ? obj : defaultValue;
        }

        /// <summary>
        /// format Int reads
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public int ReadInt(dynamic obj, int defaultValue = 0)
        {
            return obj != null && obj.GetType() != typeof(DBNull) ? obj : defaultValue;
        }

        /// <summary>
        /// Format Datetime reads
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        static public DateTime ReadDateTime(dynamic obj)
        {
            return ReadDateTime(obj, new DateTime());
        }

        /// <summary>
        /// Format Datetime reads, setting DBNulls to the default Datetime
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public DateTime ReadDateTime(dynamic obj, DateTime defaultValue)
        {
            return obj != null && obj.GetType() != typeof(DBNull) ? obj : defaultValue;
        }

        /// <summary>
        /// Convert a Datetime to a sting
        /// </summary>
        /// <param name="aDateTime"></param>
        /// <returns></returns>
        static public string ConvertToString(DateTime aDateTime)
        {
            return aDateTime.ToString(Utility.GetCustomConfig("APP_DEFAULT_DATETIME_FORMAT"));
        }

        /// <summary>
        /// Convert a string to Datetime
        /// </summary>
        /// <param name="aDateTime"></param>
        /// <returns></returns>
        static public DateTime ConvertToDate(string aDateTime)
        {
            return DateTime.ParseExact(aDateTime, Utility.GetCustomConfig("APP_DEFAULT_DATETIME_FORMAT"), CultureInfo.InvariantCulture);

        }
    }
}
