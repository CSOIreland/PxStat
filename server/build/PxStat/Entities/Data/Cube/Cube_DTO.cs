using API;
using Newtonsoft.Json;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PxStat.Data
{
    internal class Cube_DTO_ReadMatrixMetadata
    {
        public string language { get; set; }

        public int subject { get; set; }
        public string product { get; set; }
        public string matrix { get; set; }

        public Cube_DTO_ReadMatrixMetadata(dynamic parameters)
        {
            //Cheeck if the parameters are in key value pairs (e.g. JSON-rpc) or in a list (e.g. RESTful)
            if (Resources.Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {
                if (parameters.language != null)
                    this.language = parameters.language;
                if (parameters.matrix != null)
                    this.matrix = parameters.matrix;
            }
            else
            {
                //parameters in a list

                //map the list to the object properties
                List<string> plist = new List<string>() { "", "language", "subject", "product", "matrix" };
                for (int i = 1; i < parameters.Count; i++)
                {
                    if (String.IsNullOrEmpty(parameters[i])) continue;
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

    /// <summary>
    /// DTOs for Cube object
    /// </summary>
    public class Cube_DTO_ReadCollection
    {
        /// <summary>
        /// ISO Language code
        /// </summary>
        public string language { get; set; }


        /// <summary>
        /// Start date of read
        /// </summary>
        public DateTime datefrom { get; set; }

        public string product { get; set; }

        public bool meta { get; set; }

        public int subject { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Cube_DTO_ReadCollection(dynamic parameters)
        {
            //Cheeck if the parameters are in key value pairs (e.g. JSON-rpc) or in a list (e.g. RESTful)
            if (Resources.Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {
                if (parameters.language != null)
                    this.language = parameters.language;
                else
                    this.language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

                if (parameters.datefrom != null)
                {
                    this.datefrom = parameters.datefrom;
                    this.datefrom = this.datefrom.Date;
                }

                if (parameters.product != null)
                    this.product = parameters.product;
            }
            else
            {
                //parameters in a list

                //map the list to the object properties
                List<string> plist = new List<string>() { "", "datefrom", "language" };
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
            if (language == null) language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }

    }


    /// <summary>
    /// DTOs for Cube object
    /// </summary>
    internal class Cube_DTO_ReadCollectionSummary
    {
        /// <summary>
        /// ISO Language code
        /// </summary>
        public string language { get; set; }


        /// <summary>
        /// Start date of read
        /// </summary>
        public DateTime datefrom { get; set; }

        public string product { get; set; }

        public bool meta { get; set; }

        public int subject { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Cube_DTO_ReadCollectionSummary(dynamic parameters)
        {
            //Cheeck if the parameters are in key value pairs (e.g. JSON-rpc) or in a list (e.g. RESTful)
            if (Resources.Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {
                if (parameters.language != null)
                    this.language = parameters.language;
                else
                    this.language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

                if (parameters.datefrom != null)
                {
                    this.datefrom = parameters.datefrom;
                    this.datefrom = this.datefrom.Date;
                }

                if (parameters.product != null)
                    this.product = parameters.product;
            }
            else
            {
                //parameters in a list

                //map the list to the object properties
                List<string> plist = new List<string>() { "", "language", "subject", "product" };
                for (int i = 1; i < parameters.Count; i++)
                {
                    if (i < plist.Count)
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

    /// <summary>
    /// DTO for Cube Read
    /// </summary>
    internal class Cube_DTO_Read : ICube_DTO_Read
    {
        /// <summary>
        /// Matrix Code
        /// </summary>
        public string matrix { get; set; }

        /// <summary>
        /// Release Code
        /// </summary>
        public int release { get; set; }

        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string language { get; set; }



        public Format_DTO_Read Format { get; set; }

        /// <summary>
        /// json-stat role, e.g. time, geo etc
        /// </summary>
        public Role role { get; set; }

        /// <summary>
        /// Dimension
        /// </summary>
        public IList<Dimension> dimension { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Cube_DTO_Read(dynamic parameters)
        {
            Format = new Format_DTO_Read();
            //Cheeck if the parameters are in key value pairs (e.g. JSON-rpc) or in a list (e.g. RESTful)
            if (Resources.Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {

                if (parameters.matrix != null)
                    this.matrix = parameters.matrix;

                if (parameters.release != null)
                    this.release = parameters.release;

                if (parameters.language != null)
                    this.language = parameters.language;
                else this.language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");


                if (parameters.format != null)
                {

                    if (parameters.format.type != null)
                        Format.FrmType = parameters.format.type;

                    Format.FrmDirection = Configuration_BSO.GetStaticConfig("APP_FORMAT_DOWNLOAD_NAME");

                    if (parameters.format.version != null)
                        Format.FrmVersion = parameters.format.version;
                }


                if (parameters.role != null)
                {
                    this.role = GetJsonObject<Role>(parameters.role, Converter.Settings);
                }

                if (parameters.dimension != null)
                {
                    this.dimension = this.GetJsonObject<IList<Dimension>>(parameters.dimension, Converter.Settings);
                }
            }
            else
            {
                if (parameters.Count > 1) matrix = parameters[1];
                if (parameters.Count > 2) Format.FrmType = parameters[2];
                if (parameters.Count > 3) Format.FrmVersion = parameters[3];
                if (parameters.Count > 4) language = parameters[4];
                else language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

                Format.FrmDirection = Configuration_BSO.GetStaticConfig("APP_FORMAT_DOWNLOAD_NAME");
            }



            // Default language
            if (string.IsNullOrEmpty(this.language))
            {
                this.language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            }
            // Default format
            if (string.IsNullOrEmpty(this.Format.FrmType))
            {
                this.Format.FrmType = Configuration_BSO.GetStaticConfig("APP_DEFAULT_DATASET_FORMAT");
            }

        }



        /// <summary>
        /// Gets a json-stat object from parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private T GetJsonObject<T>(dynamic param, JsonSerializerSettings settings)
        {
            string json = JsonConvert.SerializeObject(param);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }


    public class Cube_DTO_ReadMetadata : ICube_DTO_Read
    {
        /// <summary>
        /// Matrix Code
        /// </summary>
        public string matrix { get; set; }

        /// <summary>
        /// Release Code
        /// </summary>
        public int release { get; set; }

        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string language { get; set; }



        public Format_DTO_Read Format { get; set; }

        /// <summary>
        /// json-stat role, e.g. time, geo etc
        /// </summary>
        public Role role { get; set; }

        /// <summary>
        /// Dimension
        /// </summary>
        public IList<Dimension> dimension { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Cube_DTO_ReadMetadata(dynamic parameters)
        {
            Format = new Format_DTO_Read();
            //Cheeck if the parameters are in key value pairs (e.g. JSON-rpc) or in a list (e.g. RESTful)
            if (Resources.Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {

                if (parameters.matrix != null)
                    this.matrix = parameters.matrix;

                if (parameters.release != null)
                    this.release = parameters.release;

                if (parameters.language != null)
                    this.language = parameters.language;
                else this.language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");


                if (parameters.format != null)
                {

                    if (parameters.format.type != null)
                        Format.FrmType = parameters.format.type;

                    Format.FrmDirection = Configuration_BSO.GetStaticConfig("APP_FORMAT_DOWNLOAD_NAME");

                    if (parameters.format.version != null)
                        Format.FrmVersion = parameters.format.version;
                }


                if (parameters.role != null)
                {
                    this.role = GetJsonObject<Role>(parameters.role, Converter.Settings);
                }

                if (parameters.dimension != null)
                {
                    this.dimension = this.GetJsonObject<IList<Dimension>>(parameters.dimension, Converter.Settings);
                }
            }
            else
            {
                if (parameters.Count > 1) matrix = parameters[1];
                if (parameters.Count > 2) language = parameters[2];
                else language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

                this.Format = new Format_DTO_Read() { FrmDirection = "DOWNLOAD", FrmType = "JSON-stat", FrmVersion = "2.0" };

            }



            // Default language
            if (string.IsNullOrEmpty(this.language))
            {
                this.language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            }
            // Default format
            if (string.IsNullOrEmpty(this.Format.FrmType))
            {
                this.Format.FrmType = Configuration_BSO.GetStaticConfig("APP_DEFAULT_DATASET_FORMAT");
            }

        }



        /// <summary>
        /// Gets a json-stat object from parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private T GetJsonObject<T>(dynamic param, JsonSerializerSettings settings)
        {
            string json = JsonConvert.SerializeObject(param);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }


    /// <summary>
    /// Some constants
    /// </summary>
    public sealed class DatasetFormat
    {
        public const string JsonStat = Constants.C_SYSTEM_JSON_STAT_NAME;
        public const string Px = Constants.C_SYSTEM_PX_NAME;
        public const string Csv = Constants.C_SYSTEM_CSV_NAME;
        public const string Xlsx = Constants.C_SYSTEM_XLSX_NAME;
        public const string Sdmx = Constants.C_SYSTEM_SDMX_NAME;
    }
}
