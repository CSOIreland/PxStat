using API;
using System.Collections.Generic;

namespace PxStat.Subscription
{

    internal class Channel_ADO
    {
        IADO _ado;
        internal Channel_ADO(IADO ado)
        {
            _ado = ado;
        }
        internal ADO_readerOutput Read(string lngIsoCode, string chnCode = null)
        {

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@LngIsoCode", value = lngIsoCode }
            };

            if (chnCode != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@ChnCode", value = chnCode });
            }

            return _ado.ExecuteReaderProcedure("Subscription_Channel_Read", inputParamList);
        }
    }
}
