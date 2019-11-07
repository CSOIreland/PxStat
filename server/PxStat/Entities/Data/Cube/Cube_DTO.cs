using System;
using System.Collections.Generic;
using API;
using Newtonsoft.Json;
using PxStat.Resources;

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



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Cube_DTO_ReadCollection(dynamic parameters)
        {
            if (parameters.language != null)
                this.language = parameters.language;
            else
                this.language = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");

            if (parameters.datefrom != null)
            {
                this.datefrom = parameters.datefrom;
                this.datefrom = this.datefrom.Date;
            }

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


        /// <summary>
        ///  Format (e.g. px, json-stat)
        /// </summary>
        public string format { get; set; }

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

            if (parameters.format != null)
            {
                this.format = parameters.format;
                if (this.format == Constants.C_SYSTEM_JSON_NAME)
                    this.format = Utility.GetCustomConfig("APP_DEFAULT_DATASET_FORMAT");
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
                this.language = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
            }
            // Default format
            if (string.IsNullOrEmpty(this.format))
            {
                this.format = Utility.GetCustomConfig("APP_DEFAULT_DATASET_FORMAT");
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
    /// 
    /// </summary>
    public sealed class DatasetFormat
    {
        public const string JsonStat = Constants.C_SYSTEM_JSON_STAT_NAME;
        public const string Px = Constants.C_SYSTEM_PX_NAME;
        public const string Csv = Constants.C_SYSTEM_CSV_NAME;
    }
}
