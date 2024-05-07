using API;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{

    public class JsonStatBuilder1_1
    {
        public JsonStatV1_1 Create(IDmatrix matrix, string lngIsoCode, bool showData = true, bool doStatus = false)
        {
            Dspec spec = matrix.Dspecs[lngIsoCode];
            var jsStat = new JsonStatV1_1();

            jsStat.Class = Class.Dataset;
            string urlBase = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' + string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), matrix.Code, matrix.FormatType, matrix.FormatVersion, spec.Language);
            if (matrix.Release != null)
            {
                if (matrix.Release.RlsLiveFlag && matrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                {
                    jsStat.Href = new Uri(urlBase);
                }
            }

            jsStat.Label = spec.Title;

            if (lngIsoCode != null)
            {
                // notes
                jsStat.Note = new List<string>();
                if (spec.Notes != null && spec.Notes.Count > 0)
                {
                    foreach (var note in spec.Notes)
                    {
                        jsStat.Note.Add(new BBCode().Transform(note, true));
                    }
                }
            }

            if (matrix.Release != null)
            {
                if (!string.IsNullOrEmpty(matrix.Release.CmmValue))
                {
                    jsStat.Note.Add(matrix.Release.CmmValue);
                }

                urlBase = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' + string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), matrix.Code, matrix.FormatType, matrix.FormatVersion, spec.Language);

                List<Format_DTO_Read> formats;
                using (Format_BSO fbso = new Format_BSO(AppServicesHelper.StaticADO))
                {
                    formats = fbso.Read(new Format_DTO_Read() { FrmDirection = Format_DTO_Read.FormatDirection.DOWNLOAD.ToString() }); //make this a list of DTO's
                }

                var link = new Link();
                if (matrix.Release != null)
                {
                    if (matrix.Release.RlsLiveFlag && matrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                    {
                        link.Alternate = new List<Alternate>();
                        foreach (var f in formats)
                        {
                            if (f.FrmType != matrix.FormatType || f.FrmVersion != matrix.FormatVersion)
                                link.Alternate.Add(new Alternate() { Href = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' + string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), matrix.Code, f.FrmType, f.FrmVersion, spec.Language), Type = f.FrmMimetype });
                        }
                        jsStat.Link = link;
                    }

                }


            }



            jsStat.Extension = new Dictionary<string, object>();
            jsStat.Extension.Add("matrix", matrix.Code);
            jsStat.Extension.Add("reasons", matrix.Release.Reasons);

            Language_BSO languageBSO = new Language_BSO();
            Language_DTO_Create language = languageBSO.Read(spec.Language);

            jsStat.Extension.Add("language",
                    new
                    {
                        code = language.LngIsoCode,
                        name = language.LngIsoName
                    }
                    );


            if (matrix.Release != null)
            {
                jsStat.Extension.Add("contact",
                    new
                    {
                        name = matrix.Release.GrpContactName,
                        email = matrix.Release.GrpContactEmail,
                        phone = matrix.Release.GrpContactPhone
                    }
                );

                jsStat.Extension.Add("subject",
                    new
                    {
                        code = matrix.Release.SbjCode,
                        value = spec.SbjValue,
                    });

                jsStat.Extension.Add("product",
                    new
                    {
                        code = matrix.Release.PrcCode,
                        value = spec.PrcValue 
                    });
            }

            var anonymousCopyrightType = new
            {
                name = matrix.Copyright.CprValue,
                code = matrix.Copyright.CprCode,
                href = matrix.Copyright.CprUrl

            };
            jsStat.Extension.Add("official", matrix.IsOfficialStatistic);
            jsStat.Extension.Add("copyright", anonymousCopyrightType);

            if (matrix.Release != null)
            {

                jsStat.Extension.Add("exceptional", matrix.Release.RlsExceptionalFlag);

                jsStat.Extension.Add("reservation", matrix.Release.RlsReservationFlag);
                jsStat.Extension.Add("archive", matrix.Release.RlsArchiveFlag);
                jsStat.Extension.Add("experimental", matrix.Release.RlsExperimentalFlag);
                jsStat.Extension.Add("analytical", matrix.Release.RlsAnalyticalFlag);

                if (matrix.Release.RlsLiveDatetimeFrom != default)
                    jsStat.Updated = DataAdaptor.ConvertToString(matrix.Release.RlsLiveDatetimeFrom);
            }

            // cells
            if (matrix.Cells == null)
            {
                matrix.Cells = new List<dynamic>();
            }

            if (!showData)
            {
                matrix.Cells = new List<dynamic>();
            }

            List<string> theId = new List<string>();
            List<long> theSize = new List<long>();
            Role theRole = new Role();

            jsStat.Dimension = new Dictionary<string, object>();

            foreach (var aDimension in spec.Dimensions)
            {
                var theDimension = new DimensionV1_1()
                {
                    Label = aDimension.Value,
                    Category = new Category()
                    {
                        Index = aDimension.Variables.Select(v => v.Code).ToList(),
                        Label = aDimension.Variables.ToDictionary(v => v.Code, v => v.Value)
                    }
                };

                theId.Add(aDimension.Code);
                theSize.Add(aDimension.Variables.Count);
                if (aDimension.GeoFlag && !string.IsNullOrEmpty(aDimension.GeoUrl))
                {

                    if (theRole.Geo == null)
                    {
                        theRole.Geo = new List<string>();
                    }
                    theRole.Geo.Add(aDimension.Code);

                    theDimension.Link = new Link()
                    {
                        Enclosure = new List<Enclosure>() { new Enclosure() { Href = aDimension.GeoUrl, Type = Configuration_BSO.GetStaticConfig("APP_GEO_MIMETYPE") } }
                    };
                }

                jsStat.Dimension.Add(aDimension.Code, theDimension);

            }
            jsStat.Dimension.Add("id", theId);
            jsStat.Dimension.Add("size", theSize);
            jsStat.Dimension.Add("role", theRole);


            jsStat.Value = new JsonStatValue() { AnythingArray = matrix.Cells.Select(c => (ValueElement)c).ToList() };


            return jsStat;
        }
    }
}
