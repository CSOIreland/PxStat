using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using API;

namespace PxStat.System.Settings
{
    /// <summary>
    /// 
    /// </summary>
    static class SettingsData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="copyrightValue"></param>
        /// <returns></returns>
        static public IList<Copyright> ReadCopyrightRecord(ADO ado, string copyrightValue)
        {

            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@CprValue",value= copyrightValue}
                };

            IList<Copyright> results = new List<Copyright>();
            var reader = ado.ExecuteReaderProcedure("System_Settings_Copyright_Read", inputParams);
            foreach (var element in reader.data)
            {
                results.Add(
                    new Copyright(
                        (string)element.CprCode,
                        (string)element.CprValue,
                        (string)element.CprUrl
                            )
                        );
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="frmType"></param>
        /// <param name="frmVersion"></param>
        /// <returns></returns>
        static public IList<Format> ReadFormatRecord(ADO ado, string frmType, string frmVersion = null)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@FrmType",value= frmType}};

            if (frmVersion != null)
                inputParams.Add(new ADO_inputParams() { name = "@FrmVersion", value = frmVersion });

            IList<Format> results = new List<Format>();
            var reader = ado.ExecuteReaderProcedure("System_Settings_Format_Read", inputParams);
            foreach (var element in reader.data)
            {
                results.Add(
                    new Format(
                        (string)element.FrmType,
                        (string)element.FrmVersion
                            )
                        );
            }

            return results;

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Copyright
    {
        public string CprCode { get; }
        public string CprValue { get; }
        public string CprUrl { get; }

        public Copyright(string copyrightCode, string copyrightValue, string copyrightUrl)
        {
            CprCode = copyrightCode;
            CprValue = copyrightValue;
            CprUrl = copyrightUrl;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Format
    {
        public string Type { get; }
        public string Version { get; }

        public Format(string type, string version)
        {
            Type = type;
            Version = version;
        }
    }
}
