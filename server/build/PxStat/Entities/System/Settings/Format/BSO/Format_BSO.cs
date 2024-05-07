using API;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;

namespace PxStat.System.Settings
{
    internal class Format_BSO : IDisposable
    {
        readonly IADO ado;
        internal Format_BSO(IADO Ado)
        {
            ado = Ado;
        }

        internal List<Format_DTO_Read> Read(Format_DTO_Read format = null)
        {
            var adoFormat = new Format_ADO();
            List<Format_DTO_Read> list = new List<Format_DTO_Read>();

            var result = adoFormat.Read(ado, format ?? new Format_DTO_Read());
            foreach (dynamic d in result.data)
            {
                list.Add(new Format_DTO_Read() { FrmType = d.FrmType, FrmVersion = d.FrmVersion, FrmDirection = d.FrmDirection, FrmMimetype = GetMimetypeForFormat(new Format_DTO_Read() { FrmType = d.FrmType }) });
            }
            return list;
        }
        /// <summary>
        /// Tests if a format exists
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        internal bool Exists(Format_DTO_Read format)
        {
            var adoFormat = new Format_ADO();
            var result = adoFormat.Read(ado, format ?? new Format_DTO_Read());
            return result.hasData;
        }

        internal string GetMimetypeForFormat(Format_DTO_Read format)
        {
            if (format.FrmType == Constants.C_SYSTEM_JSON_STAT_NAME) return Configuration_BSO.GetStaticConfig("APP_JSON_MIMETYPE");
            if (format.FrmType == Constants.C_SYSTEM_XLSX_NAME) return Configuration_BSO.GetStaticConfig("APP_XLSX_MIMETYPE");
            if (format.FrmType == Constants.C_SYSTEM_CSV_NAME) return Configuration_BSO.GetStaticConfig("APP_CSV_MIMETYPE");
            if (format.FrmType == Constants.C_SYSTEM_PX_NAME) return Configuration_BSO.GetStaticConfig("APP_PX_MIMETYPE");
            if (format.FrmType == Constants.C_SYSTEM_SDMX_NAME) return Configuration_BSO.GetStaticConfig("APP_XML_MIMETYPE");
            return null;
        }

        internal string GetFileSuffixForFormat(Format_DTO_Read format)
        {
            if (format.FrmType == Constants.C_SYSTEM_XLSX_NAME) return Configuration_BSO.GetStaticConfig("APP_XLSX_FILE_SUFFIX");
            if (format.FrmType == Constants.C_SYSTEM_CSV_NAME) return Configuration_BSO.GetStaticConfig("APP_CSV_FILE_SUFFIX");
            if (format.FrmType == Constants.C_SYSTEM_PX_NAME) return Configuration_BSO.GetStaticConfig("APP_PX_FILE_SUFFIX");
            if (format.FrmType == Constants.C_SYSTEM_SDMX_NAME) return Configuration_BSO.GetStaticConfig("APP_SDMX_FILE_SUFFIX");
            return null;
        }

        internal string GetMimetypeForFormat(string format)
        {
            if (format.ToUpper() == Constants.C_SYSTEM_JSON_STAT_NAME) return Configuration_BSO.GetStaticConfig("APP_JSON_MIMETYPE");
            if (format.ToUpper() == Constants.C_SYSTEM_XLSX_NAME) return Configuration_BSO.GetStaticConfig("APP_XLSX_MIMETYPE");
            if (format.ToUpper() == Constants.C_SYSTEM_CSV_NAME) return Configuration_BSO.GetStaticConfig("APP_CSV_MIMETYPE");
            if (format.ToUpper() == Constants.C_SYSTEM_PX_NAME) return Configuration_BSO.GetStaticConfig("APP_PX_MIMETYPE");
            if (format.ToUpper() == Constants.C_SYSTEM_SDMX_NAME) return Configuration_BSO.GetStaticConfig("APP_XML_MIMETYPE");
            return null;
        }

        internal string GetFileSuffixForFormat(string format)
        {
            if (format.ToUpper() == Constants.C_SYSTEM_XLSX_NAME) return Configuration_BSO.GetStaticConfig("APP_XLSX_FILE_SUFFIX");
            if (format.ToUpper() == Constants.C_SYSTEM_CSV_NAME) return Configuration_BSO.GetStaticConfig("APP_CSV_FILE_SUFFIX");
            if (format.ToUpper() == Constants.C_SYSTEM_PX_NAME) return Configuration_BSO.GetStaticConfig("APP_PX_FILE_SUFFIX");
            if (format.ToUpper() == Constants.C_SYSTEM_SDMX_NAME) return Configuration_BSO.GetStaticConfig("APP_SDMX_FILE_SUFFIX");
            return null;
        }

        public void Dispose()
        {
           ado?.Dispose();
        }
    }
}
