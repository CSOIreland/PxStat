using API;
using Autofac;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{

    public class JsonStatBuilder2_0
    {
        IContainer Container;

        public JsonStatBuilder2_0()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ADO>().As<IADO>().WithParameter("connectionName", "defaultConnection");
            Container = builder.Build();
        }

        public JsonStat Create(IDmatrix matrix, string lngIsoCode, bool showData = true, bool doStatus = false)
        {
            Dspec spec = new Dspec();
            if (matrix.Dspecs.TryGetValue(lngIsoCode, out var dspec))
            {
                spec = matrix.Dspecs[lngIsoCode];
            }

            var jsStat = new JsonStat();
            jsStat.Id = new List<string>();
            jsStat.Role = new Role();
            jsStat.Size = new List<long>();


            jsStat.Version = Version.The20;
            jsStat.Class = Class.Dataset;
            string urlBase = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.api.restful") + '/' + string.Format(Utility.GetCustomConfig("APP_RESTFUL_DATASET"), Utility.GetCustomConfig("APP_READ_DATASET_API"), matrix.Code, matrix.FormatType, matrix.FormatVersion, spec.Language);

            if (matrix.Release != null)
            {
                if (matrix.Release.RlsLiveFlag && matrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                    jsStat.Href = new Uri(urlBase);
            }

            lngIsoCode = lngIsoCode ?? Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

            if (lngIsoCode != null)
            {
                // notes
                jsStat.Note = new List<string>();
                if (spec.Notes != null && spec.Notes.Count > 0)
                {
                    foreach (var note in spec.Notes)
                    {
                        if (!string.IsNullOrEmpty(note))
                        {
                            jsStat.Note.Add(note);
                        }
                        else
                        {
                            jsStat.Note.Add("");
                        }
                    }
                }
            }

            jsStat.Label = spec.Title;

            // the release note is now appended to the other notes from the px file
            if (matrix.Release != null)
            {
                if (!string.IsNullOrEmpty(matrix.Release.CmmValue))
                {
                    jsStat.Note.Add(matrix.Release.CmmValue);
                }

                urlBase = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.api.restful") + '/' + string.Format(Utility.GetCustomConfig("APP_RESTFUL_DATASET"), Utility.GetCustomConfig("APP_READ_DATASET_API"), matrix.Code, matrix.FormatType, matrix.FormatVersion, spec.Language);

                List<Format_DTO_Read> formats;
                using (var scope = Container.BeginLifetimeScope())
                {
                    IADO ado = scope.Resolve<IADO>();
                    using (Format_BSO fbso = new Format_BSO(ado))
                    {
                        formats = fbso.Read(new Format_DTO_Read() { FrmDirection = Format_DTO_Read.FormatDirection.DOWNLOAD.ToString() }); //make this a list of DTO's
                    }
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
                                link.Alternate.Add(new Alternate() { Href = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.api.restful") + '/' + string.Format(Utility.GetCustomConfig("APP_RESTFUL_DATASET"), Utility.GetCustomConfig("APP_READ_DATASET_API"), matrix.Code, f.FrmType, f.FrmVersion, spec.Language), Type = f.FrmMimetype });
                        }
                        jsStat.Link = link;
                    }
                }


            }

            jsStat.Extension = new Dictionary<string, object>();
            jsStat.Extension.Add("matrix", matrix.Code);
            if (matrix.Release?.Reasons != null)
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
            jsStat.Extension.Add("elimination", spec.GetEliminationObject());

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
                        value = spec.SbjValue 
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


            jsStat.Dimension = new Dictionary<string, Dimension>();


            var statDim = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault();
            jsStat.Role.Metric = new List<string> { statDim.Code };




            var timeDim = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault();
            jsStat.Role.Time = new List<string> { timeDim.Code };

            foreach (var aDimension in spec.Dimensions)

            {
                Dictionary<string, Unit> statUnit = null;
                if (aDimension.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC)
                {
                    //Try catch block to diagnose why sometimes statDim.Variables looks like it has duplicate codes
                    try
                    {
                        statUnit = statDim.Variables.ToDictionary(v => v.Code, v => new Unit() { Decimals = v.Decimals, Label = v.Unit, Position = Position.End });
                    }
                    catch(Exception ex)
                    {
                        Log.Instance.Error($"Error parsing statistic variables for dataset {matrix.Code}");
                        Log.Instance.Error($"Variables for {matrix.Code} are: {Utility.JsonSerialize_IgnoreLoopingReference(statDim.Variables)}");
                        throw ex;
                    }
                }
                var theDimension = new Dimension()
                {
                    Label = aDimension.Value,
                    Category = new Category()
                    {
                        Index = aDimension.Variables.Select(v => v.Code).ToList(),
                        Label = aDimension.Variables.ToDictionary(v => v.Code, v => v.Value),
                        Unit = statUnit ?? null

                    }

                };



                jsStat.Dimension.Add(aDimension.Code, theDimension);
                jsStat.Id.Add(aDimension.Code);
                jsStat.Size.Add(aDimension.Variables.Count);

                if (aDimension.GeoFlag && !string.IsNullOrEmpty(aDimension.GeoUrl))
                {
                    if (jsStat.Role.Geo == null)
                    {
                        jsStat.Role.Geo = new List<string>();
                    }
                    jsStat.Role.Geo.Add(aDimension.Code);

                    theDimension.Link = new Link()
                    {
                        Enclosure = new List<Enclosure>() { new Enclosure() { Href = aDimension.GeoUrl, Type = Utility.GetCustomConfig("APP_GEO_MIMETYPE") } }
                    };
                }
            }

            if (doStatus)
            {
                jsStat.Status = new Status() { UnionArray = matrix.ComparisonReport.Select(c => c ? "true" : "false").ToList() };

                List<string> theList = new List<string>();
                for (int i = 0; i < matrix.Cells.Count; ++i)
                {
                    theList.Add(i % 2 == 0 ? "true" : "false");
                }

            }


            jsStat.Value = new JsonStatValue() { AnythingArray = matrix.Cells.Select(c => (ValueElement)c).ToList() };

            return jsStat;


        }

    }
}
