using API;
using Newtonsoft.Json;
using System;
using System.Net;

namespace PxStat.Data
{
    internal class GeoMap_BSO_Read
    {


        internal Static_Output Read(Static_API staticRequest)
        {
            Static_Output output = new Static_Output() { };
            output.mimeType = "application/json";


            ////See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "GeoMap_BSO_Read", "Read", staticRequest.parameters[1]);

            if (cache.hasData)
            {
                output.response = cache.data;
                output.statusCode = HttpStatusCode.OK;
                return output;

            }

            using (GeoMap_BSO bso = new GeoMap_BSO(new ADO("defaultConnection")))
            {
                try
                {
                    var dataRead = bso.Read(staticRequest.parameters[1]);
                    if (dataRead.hasData)
                    {
                        output.statusCode = HttpStatusCode.OK;
                        GeoJson readGeoJson = JsonConvert.DeserializeObject<GeoJson>(dataRead.data[0].GmpGeoJson);
                        MemCacheD.Store_BSO("PxStat.Data", "GeoMap_BSO_Read", "Read", staticRequest.parameters[1], readGeoJson, default(DateTime));

                        output.response = Utility.JsonSerialize_IgnoreLoopingReference(JsonConvert.DeserializeObject<GeoJson>(dataRead.data[0].GmpGeoJson));
                    }
                    else
                    {
                        output.statusCode = HttpStatusCode.NotFound;
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex.Message);
                    Log.Instance.Error(ex.StackTrace);
                    output.statusCode = HttpStatusCode.InternalServerError;
                }
            };



            return output;
        }
    }
}
