using API;
using System;

namespace PxStat.Data
{
    internal class GeoMap_BSO : IDisposable
    {
        IADO ado;

        internal GeoMap_BSO(IADO ado)
        {
            this.ado = ado;
        }

        internal GeoMap_BSO()
        {
            this.ado = AppServicesHelper.StaticADO;
        }

        internal ADO_readerOutput Read(string gmpCode)
        {
            GeoMap_ADO gAdo = new GeoMap_ADO(ado);
            var readData = gAdo.Read(gmpCode);

            return readData;
        }

        public void Dispose()
        {
            ado?.Dispose();
        }
    }
}
