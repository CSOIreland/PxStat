using API;
using System;

namespace Px5Migrator
{
    internal class GeoMap_BSO : IDisposable
    {
        ADO ado;

        internal GeoMap_BSO(ADO ado)
        {
            this.ado = ado;
        }

        internal GeoMap_BSO()
        {
            this.ado = new ADO("defaultConnection");
        }

        internal ADO_readerOutput Read(string gmpCode)
        {
            GeoMap_ADO gAdo = new GeoMap_ADO(ado);
            var readData = gAdo.Read(gmpCode);

            return readData;
        }

        public void Dispose()
        {
            ado.Dispose();
        }
    }
}
