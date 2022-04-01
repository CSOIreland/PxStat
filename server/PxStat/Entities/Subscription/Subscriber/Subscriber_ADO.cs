using API;
using System.Collections.Generic;

namespace PxStat.Subscription
{
    public class Subscriber_ADO
    {
        private ADO _ado;

        internal Subscriber_ADO(ADO ado)
        {
            _ado = ado;
        }
        internal bool Create(string sbrPreference, string subscriberUserId, string lngIsoCode, string sbrKey)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();
            if (sbrPreference != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@Preference", value = sbrPreference });
            }

            inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });

            inputParamList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = lngIsoCode });

            inputParamList.Add(new ADO_inputParams() { name = "@SbrKey", value = sbrKey });

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            _ado.ExecuteNonQueryProcedure("Subscription_Subscriber_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;

        }

        internal bool Update(string subscriberUserId, string lngIsoCode = null, string sbrPreference = null)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@SbrUserId", value = subscriberUserId }

            };

            if (lngIsoCode != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = lngIsoCode });
            }

            if (sbrPreference != null)
                inputParamList.Add(new ADO_inputParams() { name = "@SbrPreference", value = sbrPreference });
            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam
            {
                name = "return",
                value = 0
            };

            //Attempting to create the new entity
            _ado.ExecuteNonQueryProcedure("Subscription_Subscriber_Update", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;

        }

        internal bool UpdateKey(string subscriberUserId, string subscriberKey)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@SbrUserId", value = subscriberUserId },
                new ADO_inputParams() { name = "@SbrKey", value = subscriberKey }
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam
            {
                name = "return",
                value = 0
            };

            //Attempting to create the new entity
            _ado.ExecuteNonQueryProcedure("Subscription_Subscriber_UpdateSubscriberKey", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;

        }

        internal bool Delete(string subscriberUserId)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@SbrUserId", value = subscriberUserId }
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            _ado.ExecuteNonQueryProcedure("Subscription_Subscriber_Delete", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;

        }

        internal ADO_readerOutput Read(string subscriberUserId = null)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();
            if (subscriberUserId != null)
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });

            return _ado.ExecuteReaderProcedure("Subscription_Subscriber_Read", inputParamList);

        }

        internal List<dynamic> ReadSubscriberKeys()
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();

            var result = _ado.ExecuteReaderProcedure("Subscription_Subscriber_ReadKeys", inputParamList);

            return result.data;

        }
    }
}
