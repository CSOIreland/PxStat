using API;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{
    /// <summary>
    /// Stores the data and metadata for a dataset
    /// The Dspec class holds that information that may vary by language for each dataset. There may be multiple language versions
    /// 
    /// </summary>
    /// 

    public class Dmatrix : IDmatrix
    {
        //The datapoint values
        public ICollection<dynamic> Cells { get; set; }
        //The datapoint values expressed as a single string
        public string CellsString { get; set; }
        //The matrix code
        public string Code { get; set; }
        //The copyright string
        public ICopyright Copyright { get; set; } = new Copyright_DTO_Create();
        //Created datetime for the matrix
        public DateTime CreatedDateTime { get; set; }
        //Created date in local string format
        public string CreatedDateString { get; set; }
        //List of all languages used by the matrix. Languages are expressed as a 2 letter code, e.g. "en", "fi", "ga"
        public ICollection<string> Languages { get; set; }
        //The Release that this matrix serves
        public Release_DTO Release { get; set; }
        //The individual langauge specific properties of the matrix. There will be one spec per language and at least one in the matrix
        public Dictionary<string, Dspec> Dspecs { get; set; } = new Dictionary<string, Dspec>();
        //The format of the input file 
        //public Format_DTO_Read Format { get; set; }
        //The input file as a string
        public string MtrInput { get; set; }
        //Flags if this is an official statistic
        public bool IsOfficialStatistic { get; set; }
        public int Id { get; set; }
        //Global value for decimal places
        public short Decimals { get; set; }
        public string Units { get; set; }
        //The Format type
        public string FormatType { get; set; }
        // The Format version
        public string FormatVersion { get; set; }
        //The Language
        public string Language { get; set; }
        //A list of boolean values corresponding exactly to Cells. Indicates if they are amended data cells.
        public List<bool> ComparisonReport { get; set; }
       

        /// <summary>
        /// Px Document Dmatrix Constructor
        /// </summary>
        /// <param name="document"></param>
        /// <param name="dto"></param>
        /// <param name="ado"></param>
        /// <param name="utility"></param>        
        public Dmatrix(IDocument document, IUpload_DTO dto, IADO ado)
        {
            LoadMatrixPropertiesFromPxDocument(document, dto, ado);
        }

        public Dmatrix()
        {
        }

        /// <summary>
        /// Load the matrix from the Px file
        /// </summary>
        /// <param name="document"></param>
        /// <param name="dto"></param>
        /// <param name="ado"></param>
        /// <param name="metaData"></param>
        public void LoadMatrixPropertiesFromPxDocument(IDocument document, IUpload_DTO dto, IADO ado)
        {
            FormatType = Configuration_BSO.GetStaticConfig("APP_PX_DEFAULT_FORMAT");

            // The Matrix Code identifies the data cube
            Code = document.GetStringElementValue("MATRIX");

            Copyright.CprValue = document.GetStringElementValue("SOURCE");
            Copyright_BSO cBso = new Copyright_BSO();
            Copyright = cBso.ReadFromValue(Copyright.CprValue, ado);

            // As the Px Document represents a px file, this represents the version of the px format specification, typically a year where the format was introduced
            FormatVersion = document.GetStringElementValue("AXIS-VERSION");

            string officialStat = document.GetStringValueIfExist("OFFICIAL-STATISTICS");
            if (string.IsNullOrEmpty(officialStat))
                IsOfficialStatistic = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.officialStatistics");
            else
                IsOfficialStatistic = officialStat.ToUpper() == Configuration_BSO.GetStaticConfig("APP_PX_TRUE"); ;

            // The data, common to all the different languages
            Cells = document.GetData("DATA");

            // we will use this to obtain the codes of the dimensions (optional)
            var domain = document.GetSingleElementWithSubkeysIfExist("DOMAIN");

            // The Main language of the Px File (other languages might exist)
            Language = document.GetStringValueIfExist("LANGUAGE");

            if (String.IsNullOrEmpty(Language))
            {
                Language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            }
            Dspecs = new Dictionary<string, Dspec>();
            var mainSpec = new Dspec(document, ConvertFactory.Convert(domain), dto)
            {
                Language = Language
            };
            Dspecs.Add(mainSpec.Language, mainSpec);

            Languages = document.GetListOfStringValuesIfExist("LANGUAGES");

            if (Languages != null && Languages.Count > 0)
            {
                var otherLanguages = Languages.Where(t => t != Language).Select(t => t);
                foreach (var otherlanguage in otherLanguages)
                {
                    Dspecs.Add(otherlanguage, new Dspec(document, ConvertFactory.Convert(domain), otherlanguage, dto, mainSpec));
                }
            }
            else
            {
                Languages = new List<string>();
                Languages.Add(Language);
            }
        }

        public IDmatrix GetDmatrixFromPxDocument(IDocument document,  IUpload_DTO uploadDto,List<PxUpload_DTO> specUploads =null)
        {
            Dmatrix dmatrix = new Dmatrix();
            dmatrix.FormatType = Configuration_BSO.GetStaticConfig("APP_PX_DEFAULT_FORMAT");

            // The Matrix Code identifies the data cube
            dmatrix.Code = document.GetStringElementValue("MATRIX");

            dmatrix.Copyright.CprValue = document.GetStringElementValue("SOURCE");
            Copyright_BSO cBso = new Copyright_BSO();

            
            dmatrix.Copyright = cBso.ReadFromValue(dmatrix.Copyright.CprValue);

            // As the Px Document represents a px file, this represents the version of the px format specification, typically a year where the format was introduced
            dmatrix.FormatVersion = document.GetStringElementValue("AXIS-VERSION");

            string officialStat = document.GetStringValueIfExist("OFFICIAL-STATISTICS");
            if (string.IsNullOrEmpty(officialStat))
                dmatrix.IsOfficialStatistic = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.officialStatistics");
            else
                dmatrix.IsOfficialStatistic = officialStat.ToUpper().Equals("YES");

            // The data, common to all the different languages
            dmatrix.Cells = (document.GetData("DATA")).Select(x => x.Value).ToList();

            // we will use this to obtain the codes of the dimensions (optional)
            var domain = document.GetSingleElementWithSubkeysIfExist("DOMAIN");

            // The Main language of the Px File (other languages might exist)
            dmatrix.Language = document.GetStringValueIfExist("LANGUAGE");

            if (String.IsNullOrEmpty(dmatrix.Language))
            {
                dmatrix.Language = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            }
            dmatrix.Languages = document.GetListOfStringValuesIfExist("LANGUAGES");
            this.Languages = dmatrix.Languages;
            dmatrix.Dspecs = new Dictionary<string, Dspec>();
            //var mainSpec = new Dspec(document, ConvertFactory.Convert(domain), dto, MetaData);
            var mainSpec = new Dspec(uploadDto).GetDspecFromPxDocument(document, domain, null, null);

            mainSpec.Language = dmatrix.Language;
            dmatrix.Dspecs.Add(mainSpec.Language, mainSpec);



            if (dmatrix.Languages != null && Languages.Count > 0)
            {
                var otherLanguages = dmatrix.Languages.Where(t => t != dmatrix.Language).Select(t => t);
                
                foreach (var otherlanguage in otherLanguages)
                {
                    if(specUploads!=null)
                    {
                        var thisSpecUpload = specUploads.Where(x => x.LngIsoCode.Equals(otherlanguage)).FirstOrDefault();
                        if(thisSpecUpload!=null)
                        {
                            uploadDto.FrqValueTimeval = thisSpecUpload.FrqValueTimeval ?? uploadDto.FrqValueTimeval;
                            uploadDto.LngIsoCode=thisSpecUpload.LngIsoCode ?? uploadDto.LngIsoCode;
                            uploadDto.FrqCodeTimeval = thisSpecUpload.FrqCodeTimeval ?? uploadDto.FrqCodeTimeval ;
                        }
                    }
                    dmatrix.Dspecs.Add(otherlanguage, new Dspec(uploadDto).GetDspecFromPxDocument(document, domain, otherlanguage, mainSpec));
                }
            }
            else
            {
                dmatrix.Languages = new List<string>
                {
                    dmatrix.Language
                };
            }

            return dmatrix;
        }


    }
}
