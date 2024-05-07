using API;

namespace PxStat.Subscription
{
    internal class Channel_BSO
    {
        internal ADO_readerOutput Read(IADO ado, string lngIsoCode, string chnCode)
        {
            Channel_ADO cAdo = new Channel_ADO(ado);

            return cAdo.Read(lngIsoCode, chnCode);
        }
    }
}
