using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PxStat.Resources
{
    /// <summary>
    /// 
    /// </summary>
    internal class MethodReader
    {


        /// <summary>
        /// For a dynamic object, this function tells us if it has a specified property
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static bool DynamicHasProperty(dynamic dto, string propertyName)
        {
            Type typeOfDynamic = dto.GetType();
            return typeOfDynamic.GetProperties().Where(p => p.Name.Equals(propertyName)).Any();
        }

        /// <summary>
        /// For a dynamic object, this function returns the string value of a specified property
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static string GetStringValueOfProperty(dynamic dto, string propertyName)
        {
            Type typeOfDynamic = dto.GetType();
            IEnumerable<PropertyInfo> iList = typeOfDynamic.GetProperties().Where(p => p.Name.Equals(propertyName));
            foreach (PropertyInfo prop in iList)
            {
                return prop.GetValue(dto).ToString();
            }
            return "";
        }



    }

    /// <summary>
    /// Class for reading Enum Description etc
    /// </summary>
    internal static class EnumInfo
    {
        public static string GetEnumDescription(this Enum GenericEnum)
        {

            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((DescriptionAttribute)_Attribs.ElementAt(0)).Description;

                }
            }
            return GenericEnum.ToString();

        }
    }
}
