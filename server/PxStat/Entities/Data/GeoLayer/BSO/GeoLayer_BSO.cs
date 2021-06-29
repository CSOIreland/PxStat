using API;
using System.Collections.Generic;

namespace PxStat.Data
{
    internal class GeoLayer_BSO
    {
        internal GeoLayer_BSO() { }

        internal List<dynamic> Read(ADO ado, GeoLayer_DTO_Read dto = null)
        {
            GeoLayer_ADO gAdo = new GeoLayer_ADO(ado);
            if (dto == null) dto = new GeoLayer_DTO_Read();
            return gAdo.Read(dto);

        }

        internal List<dynamic> Read(ADO ado, string glrCode = null, string glrName = null)
        {
            GeoLayer_ADO gAdo = new GeoLayer_ADO(ado);
            GeoLayer_DTO_Read dto = new GeoLayer_DTO_Read() { GlrCode = glrCode, GlrName = glrName };

            return gAdo.Read(dto);

        }
    }
}
