
using API;
using FluentValidation;
using PxStat.Data;
using PxStat.Entities.BuildData;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxStat.Build
{


    /// <summary>
    /// Custom validations for  PxBuild
    /// </summary>
    internal static class CustomValidations
    {

        public static object Enumerations { get; private set; }

        internal static bool NotReservedWord(string strWord)
        {

            return (!Constants.C_SYSTEM_RESERVED_WORD().Contains(strWord.ToLower()));

        }

        internal static bool ReadDatasetHasEnoughParameters(IRequest api)
        {
            return api.parameters.Count >= Convert.ToInt32(Configuration_BSO.GetStaticConfig("APP_REST_READ_DATASET_PARAMETER_COUNT"));
        }

        internal static bool ReadMetadataHasEnoughParameters(IRequest api)
        {
            return api.parameters.Count >= Convert.ToInt32(Configuration_BSO.GetStaticConfig("APP_REST_READ_METADATA_PARAMETER_COUNT"));
        }

        internal static bool ReadCollectionHasEnoughParameters(IRequest api)
        {
            return api.parameters.Count >= Convert.ToInt32(Configuration_BSO.GetStaticConfig("APP_REST_READ_COLLECTION_PARAMETER_COUNT"));
        }

        internal static bool FormatExistsReadDataset(IRequest api)
        {
            if (api.parameters.Count < Convert.ToInt32(Configuration_BSO.GetStaticConfig("APP_REST_READ_DATASET_PARAMETER_COUNT"))) return false;
            Format_DTO_Read dto = new Format_DTO_Read() { FrmType = api.parameters[2], FrmVersion = api.parameters[3] == "" ? "none" : api.parameters[3] };
            return CustomValidations.FormatExists(dto);
        }



        internal static bool LanguageCode(RESTful_API api)
        {
            if (api.parameters.Count >= 5)
            {
                if (api.parameters[4].Length != 2) return false;
            }
            return true;
        }


        internal static bool ValidateIgnoreEscapeChars(string readString)
        {

            return Regex.IsMatch(Regex.Escape(readString), Configuration_BSO.GetStaticConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS"));

        }

        internal static bool FormatForReadPreMetadata(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.DOWNLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat)) return false;
            return true;

        }

        internal static bool FormatForReadMetadata(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.DOWNLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat)) return false;
            return true;

        }

        internal static bool FormatForReadPreDataset(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.DOWNLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.CSV) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.XLSX)) return false;
            return true;

        }


        internal static bool FormatForReadDataset(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.DOWNLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.CSV) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.XLSX) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.SDMX)) return false;

            bool exists;
            using (Format_BSO bso = new Format_BSO(AppServicesHelper.StaticADO))
            {
                exists = bso.Exists(dto);
            }
            return exists;


        }

        internal static bool FormatForBuildUpdate(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.UPLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX)) return false;
            return true;

        }

        internal static bool FormatForBuildCreate(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.UPLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX)) return false;
            return true;

        }

        internal static bool SignatureMatch(Build_DTO_ReadTemplate dto)
        {

            var signature = Utility.GetMD5(Configuration_BSO.GetStaticConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }
        internal static bool SignatureMatch(Build_DTO_Read dto)
        {

            var signature = Utility.GetMD5(Configuration_BSO.GetStaticConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }

        internal static bool SignatureMatch(dynamic dto)
        {
            var v = dto.GetSignatureDTO();
            var signature = Utility.GetMD5(Configuration_BSO.GetStaticConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }

 


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool PeriodsNotRepeated(dynamic dto)
        {
            foreach (var per in dto.Periods)
            {
                if (!CustomValidations.PeriodCodeNotRepeatedInList(dto.Periods)) return false;
            }
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="periods"></param>
        /// <returns></returns>
        internal static bool PeriodCodeNotRepeatedInList(List<PeriodRecordDTO_Create> periods)
        {
            var query = from prds in periods
                        group prds by prds.Code into g
                        select new
                        {
                            code = g.First().Code,
                            codeCount = g.Count()
                        };
            var result = from pers in query where pers.codeCount > 1 select pers;
            return result.Count() == 0;
        }


        internal static bool PeriodValueNotRepeatedInList(List<PeriodRecordDTO_Create> periods)
        {
            var query = from prds in periods
                        group prds by prds.Value into g
                        select new
                        {
                            code = g.First().Value,
                            codeCount = g.Count()
                        };
            var result = from pers in query where pers.codeCount > 1 select pers;
            return result.Count() == 0;
        }
        /// <summary>
        /// Validates that a variable code is not repeated within a given classification
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool VariableCodeNotRepeated(IList<VariableRecordDTO_Create> dto)
        {
            var query = from vars in dto
                        group vars by vars.Code into g
                        select new
                        {
                            Vrb = g.First().Code,
                            VrbCount = g.Count()
                        };
            var result = from vars in query where vars.VrbCount > 1 select vars;
            return result.Count() == 0;
        }

        /// <summary>
        /// Validates that a variable value is not repeated within a given classification
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool VariableValueNotRepeated(IList<VariableRecordDTO_Create> dto)
        {
            var query = from vars in dto
                        group vars by vars.Value into g
                        select new
                        {
                            Vrb = g.First().Value,
                            VrbCount = g.Count()
                        };
            var result = from vars in query where vars.VrbCount > 1 select vars;
            return result.Count() == 0;
        }












        /// <summary>
        /// Checks if a format type and version is represented in the database
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool FormatExists(Format_DTO_Read dto)
        {
            bool exists = false;
            using (IADO Ado = AppServicesHelper.StaticADO)
            {
                try
                {
                    Format_ADO adoFormat = new Format_ADO();
                    Format_DTO_Read dtoFormat = new Format_DTO_Read();
                    dtoFormat.FrmType = dto.FrmType;
                    dtoFormat.FrmVersion = dto.FrmVersion;
                    dtoFormat.FrmDirection = dto.FrmDirection;
                    var result = adoFormat.Read(Ado, dtoFormat);
                    exists = result.hasData;
                }
                catch
                {
                    exists = false;
                }
            }
   

            return exists;

        }

        internal static bool FormatExists(dynamic dto)
        {
            bool exists = false;

            using (IADO Ado = AppServicesHelper.StaticADO)
            {
                try
                {
                    Format_ADO adoFormat = new Format_ADO();
                    Format_DTO_Read dtoFormat = new Format_DTO_Read();
                    dtoFormat.FrmType = dto.Format.FrmType;
                    dtoFormat.FrmVersion = dto.Format.FrmVersion;
                    var result = adoFormat.Read(Ado, dtoFormat);
                    exists = result.hasData;
                }
                catch
                {
                    exists = false;
                }

            }
            return exists;

        }

        //Build_DTO_ReadTemplate



        internal static bool FrequencyCodeExists(dynamic dto)
        {
            Frequency_BSO bso = new Frequency_BSO();
            Frequency_DTO dtoFreq = bso.Read(dto.FrqCodeTimeval);
            return (dtoFreq.FrqCode != null);

        }


    }
}
