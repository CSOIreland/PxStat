using API;
using Newtonsoft.Json;
using PxStat.Security;
using System;

namespace PxStat.Data
{
    public class GeoMap_DTO_Create
    {
        public string GmpName { get; set; }
        public string GmpDescription { get; set; }
        public GeoJson GeoJson { get; set; }
        public string GmpGeoJson { get; set; }

        public string GlrCode { get; set; }

        public string GmpCode { get; set; }
        public int GmpFeatureCount { get; set; }
        public GeoMap_DTO_Create(dynamic parameters)
        {
            if (parameters.GmpName != null)
                GmpName = parameters.GmpName;
            if (parameters.GmpDescription != null)
                GmpDescription = parameters.GmpDescription;

            if (parameters.GmpGeoJson != null)
            {
                //GeoJson object is created only for validation purposes
                try
                {
                    GeoJson = JsonConvert.DeserializeObject<GeoJson>(parameters.GmpGeoJson.ToString());
                    GmpGeoJson = parameters.GmpGeoJson.ToString();
                    GmpFeatureCount = GeoJson.Features.Length;

                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex.Message);
                }

            }

            if (parameters.GlrCode != null)
                GlrCode = parameters.GlrCode;
            
            GmpCode = Utility.GetSHA256(new Random().Next() + Configuration_BSO.GetStaticConfig("APP_SALSA") + DateTime.Now.Millisecond);

        }
    }

    public class GeoMap_DTO_Validate
    {
        public GeoJson GeoJson { get; set; }
        public string GmpGeoJson { get; set; }
        public string GeoMapErrorMessage { get; set; }
        public GeoMap_DTO_Validate(dynamic parameters)
        {
            if (parameters.GmpGeoJson != null)
            {
                try
                {
                    GeoJson = JsonConvert.DeserializeObject<GeoJson>(parameters.GmpGeoJson.ToString());
                }
                catch (Exception ex)
                {
                    GeoMapErrorMessage = ex.Message;
                }
                GmpGeoJson = parameters.GmpGeoJson.ToString();
            }
        }
    }

    public class GeoMap_DTO_Delete
    {
        public string GmpCode { get; set; }

        public GeoMap_DTO_Delete(dynamic parameters)
        {
            if (parameters.GmpCode != null)
                GmpCode = parameters.GmpCode;
        }
    }

    public class GeoMap_DTO_ReadCollection
    {
        public string GmpCode { get; set; }

        public string GlrCode { get; set; }

        public GeoMap_DTO_ReadCollection(dynamic parameters)
        {
            if (parameters.GmpCode != null)
                GmpCode = parameters.GmpCode;
            if (parameters.GlrCode != null)
            {
                GlrCode = parameters.GlrCode;
            }               
        }
    }

    public class GeoMap_DTO_Update
    {
        public string GmpName { get; set; }
        public string GmpDescription { get; set; }
        public string GmpCode { get; set; }
        public string GlrCode { get; set; }
        public GeoMap_DTO_Update(dynamic parameters)
        {
            if (parameters.GmpName != null)
                GmpName = parameters.GmpName;
            if (parameters.GmpDescription != null)
                GmpDescription = parameters.GmpDescription;
            if (parameters.GmpCode != null)
                GmpCode = parameters.GmpCode;
            if (parameters.GlrCode != null)
                GlrCode = parameters.GlrCode;

        }
    }
}
