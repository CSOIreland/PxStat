using API;
using System;

namespace PxStat.DataStore
{

    public class CollectionReader : ICollectionReader
    {
        public dynamic ReadCollection(IADO ado, string languageCode, DateTime DateFrom, string PrcCode = null, bool meta = true)
        {
            return new DataStore_ADO().ReadCollection(ado, languageCode, DateFrom, PrcCode, meta);
        }
    }
}
