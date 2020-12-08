using API;
using Newtonsoft.Json;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;

namespace PxStat.Data
{

    /// <summary>
    /// DTOs for Cube object
    /// </summary>
    internal class Cube_DTO_ReadCollection
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Cube_DTO_ReadCollection(dynamic parameters)
        {
            if (parameters.language != null)
                this.language = parameters.language;
            else
                this.language = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

            if (parameters.datefrom != null)
            {
                this.datefrom = parameters.datefrom;
                this.datefrom = this.datefrom.Date;
            }

            if (parameters.product != null)
                this.product = parameters.product;

        }

    }

    /// <summary>
    /// DTO for Cube Read
    /// </summary>
    internal class Cube_DTO_Read
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


            if (parameters.matrix != null)
                this.matrix = parameters.matrix;

            if (parameters.release != null)
                this.release = parameters.release;

            if (parameters.language != null)
                this.language = parameters.language;
            else this.language = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

            Format = new Format_DTO_Read();
            if (parameters.format != null)
            {

                if (parameters.format.type != null)
                    Format.FrmType = parameters.format.type;

                Format.FrmDirection = API.Utility.GetCustomConfig("APP_FORMAT_DOWNLOAD_NAME");

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



            // Default language
            if (string.IsNullOrEmpty(this.language))
            {
                this.language = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
            }
            // Default format
            if (string.IsNullOrEmpty(this.Format.FrmType))
            {
                this.Format.FrmType = Utility.GetCustomConfig("APP_DEFAULT_DATASET_FORMAT");
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
