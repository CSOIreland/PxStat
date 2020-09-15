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
        /// This function checks an API method and ascertains if the method has a specified associated attribute
        /// </summary>
        /// <param name="method"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        internal static bool MethodHasAttribute(string method, string attributeName, Type[] parameters = null)
        {
            try
            {
                // The method contains the full path, e.g. "PxStat.Security.GroupAccount_API.Read"
                // Create a List<string> of each individual element
                List<string> mList = method.Split('.').ToList<string>();

                //If this can't be parsed then just return 
                if (mList.Count < 2)
                    return false;

                //Get the method name, i.e. everything after the last '.'
                string methodName = mList.Last<string>();

                //Remove the method name from the list
                mList.RemoveAt(mList.Count - 1);

                //Get the class, i.e. everything before the last '.' . This is done by expressing the list as a dot separated string
                string className = string.Join(".", mList);

                //Use some Reflection methods to get info about the class and method
                Type type = Type.GetType(className);

                int mcount = type.GetMethods().Where(x => x.Name == methodName).Count(); ;//.FirstOrDefault();


                if (mcount == 0) return false;
                MethodInfo methodInfo;
                if (mcount > 1)
                {
                    //There is at least one overloaded method. If the calling method has supplied a list of parameters that specifies the overload, try to match it
                    if (parameters != null)
                    {
                        methodInfo = type.GetMethod(methodName, parameters);
                    }
                    else // Otherwise apply the rule that if the attribute is applied to one overload, it applies to all
                        methodInfo = type.GetMethods().Where(x => x.Name == methodName).FirstOrDefault();
                }
                else
                {
                    methodInfo = type.GetMethod(methodName);
                }

                //Get the required "attributeName" attribute if it exists
                var attrNoTrace = methodInfo.CustomAttributes.Where(CustomAttributeData => CustomAttributeData.AttributeType.Name == attributeName).FirstOrDefault();

                //Return true or false depending on whether the attribute was found
                return attrNoTrace != null;
            }
            catch
            {

                //soft fail - continue the process.
                return false;
            }
        }
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

        /// <summary>
        /// Returns Object property names
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static IDictionary<string, object> GetPropertyNamesOfObject(dynamic dto)
        {

            IDictionary<string, object> propertyValues = (IDictionary<string, object>)dto;

            return propertyValues;

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
