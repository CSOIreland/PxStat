using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO for Product
    /// </summary>
    internal class Product_DTO
    {
        #region Properties
        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; internal set; }

        /// <summary>
        /// The new product code when a product code is being updated
        /// </summary>
        public string PrcCodeNew { get; internal set; }

        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; private set; }

        /// <summary>
        /// Product Value
        /// </summary>
        public string PrcValue { get; set; }

        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string LngIsoCode { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Product_DTO(dynamic parameters)
        {
            //Cheeck if the parameters are in key value pairs (e.g. JSON-rpc) or in a list (e.g. RESTful)
            if (Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {
                if (parameters.PrcCode != null)
                    this.PrcCode = parameters.PrcCode;

                if (parameters.PrcCodeNew != null)
                    this.PrcCodeNew = parameters.PrcCodeNew;

                if (parameters.SbjCode != null)
                    this.SbjCode = parameters.SbjCode;

                if (parameters.PrcValue != null)
                    this.PrcValue = parameters.PrcValue;

                if (parameters.LngIsoCode != null)
                    this.LngIsoCode = parameters.LngIsoCode;
                else
                    this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
            }
            else
            {
                //parameters in a list

                //map the list to the object properties
                List<string> plist = new List<string>() { "", "LngIsoCode", "SbjCode", "PrcCode" };
                for (int i = 1; i < parameters.Count; i++)
                {
                    dynamic value;
                    Type type = this.GetType();

                    PropertyInfo prop = type.GetProperty(plist[i]);
                    var ptype = prop.PropertyType;
                    switch (ptype.Name)
                    {
                        case ("Int32"):
                            value = Convert.ToInt32(parameters[i]);
                            break;
                        case ("Boolean"):
                            value = Convert.ToBoolean(parameters[i]);
                            break;
                        case ("DateTime"):
                            value = Convert.ToDateTime(parameters[i]);
                            break;
                        default:
                            value = parameters[i];
                            break;
                    }

                    prop.SetValue(this, value, null);
                }
            }
        }
    }
}
