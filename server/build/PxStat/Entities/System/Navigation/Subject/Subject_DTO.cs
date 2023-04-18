using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO class for Subjects
    /// </summary>
    internal class Subject_DTO
    {
        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; set; }

        /// <summary>
        /// Subject Value
        /// </summary>
        public string SbjValue { get; set; }

        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Theme Code
        /// </summary>
        public int ThmCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Subject_DTO(dynamic parameters)
        {
            //Cheeck if the parameters are in key value pairs (e.g. JSON-rpc) or in a list (e.g. RESTful)
            if (Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {
                //Key value pairs
                if (parameters.SbjValue != null)
                    this.SbjValue = parameters.SbjValue;

                if (parameters.SbjCode != null)
                    this.SbjCode = parameters.SbjCode;

                if (parameters.LngIsoCode != null)
                    this.LngIsoCode = parameters.LngIsoCode;
                else
                    this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
                if (parameters.ThmCode != null)
                    this.ThmCode = parameters.ThmCode;
            }
            else
            {
                //parameters in a list

                //map the list to the object properties
                List<string> plist = new List<string>() { "LngIsoCode", "SbjCode" };
                for (int i = 0; i < parameters.Count; i++)
                {
                    Type type = this.GetType();

                    PropertyInfo prop = type.GetProperty(plist[i]);

                    prop.SetValue(this, parameters[i], null);
                }
            }

        }
        /// <summary>
        /// Blank constructor
        /// </summary>
        public Subject_DTO() { }
    }
}
