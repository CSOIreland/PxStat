using API;
using PxStat.Security;
using System.Collections.Generic;

namespace PxStat.Subscription
{
    internal class Subscription_ADO
    {
        IADO _ado;
        internal Subscription_ADO(IADO ado)
        {
            _ado = ado;
        }

        internal bool ChannelCreate(string channelCode, string subscriberUserId = null, string ccnUsername = null)
        {
            if (subscriberUserId == null && ccnUsername == null) return false;
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@ChnCode", value = channelCode }
            };

            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }
            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            _ado.ExecuteNonQueryProcedure("Subscription_ChannelSubscription_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;
        }

        internal bool ChannelDelete(string channelCpde, string subscriberUserId = null, string ccnUsername = null)
        {
            if (subscriberUserId == null && ccnUsername == null) return false;
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@ChnCode", value = channelCpde }
            };

            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }
            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;


            _ado.ExecuteNonQueryProcedure("Subscription_ChannelSubscription_Delete", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;
        }

        internal ADO_readerOutput ChannelReadCurrent(string lngIsoCode, string subscriberUserId = null, string ccnUsername = null)
        {
            if (subscriberUserId == null && ccnUsername == null) return null;
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams{ name="@LngIsoCode",value=lngIsoCode}
            };

            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }
            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }

            return _ado.ExecuteReaderProcedure("Subscription_ChannelSubscription_Read", inputParamList);
        }

        internal ADO_readerOutput ChannelRead(string lngIsoCode, string chnCode = null, string uid = null, bool singleLanguage = false)
        {

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams{ name="@LngIsocode",value=lngIsoCode },
                new ADO_inputParams{name="@SingleLanguage", value=singleLanguage}
            };



            if (chnCode != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@ChnCode", value = chnCode });
            }

            if (uid != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SbrUid", value = uid });
            }

            return _ado.ExecuteReaderProcedure("Subscription_Channel_ReadUsers", inputParamList);//Subscription_Channel_ReadUsers
                                                                                                 // return _ado.ExecuteReaderProcedure("Subscription_ChannelSubscription_Read", inputParamList);
        }

        internal bool TableCreate(string tsbTable, string subscriberUserId = null, string ccnUsername = null)
        {
            if (subscriberUserId == null && ccnUsername == null) return false;
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();

            inputParamList.Add(new ADO_inputParams() { name = "@TsbTable", value = tsbTable });

            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }
            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            _ado.ExecuteNonQueryProcedure("Subscription_Table_Subscription_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;

        }

        internal bool TableDelete(string tsbTable, string subscriberUserId = null, string ccnUsername = null)
        {
            if (subscriberUserId == null && ccnUsername == null) return false;
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();

            inputParamList.Add(new ADO_inputParams() { name = "@TsbTable", value = tsbTable });

            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }
            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            _ado.ExecuteNonQueryProcedure("Subscription_Table_Subscription_Delete", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;

        }

        internal ADO_readerOutput TableReadCurrent(string lngIsoCodePreferred, string subscriberUserId = null, string ccnUsername = null)
        {
            if (subscriberUserId == null && ccnUsername == null) return null;
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();

            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }
            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }
            inputParamList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = lngIsoCodePreferred });
            inputParamList.Add(new ADO_inputParams() { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") });

            return _ado.ExecuteReaderProcedure("Subscription_Table_Subscription_ReadCurrent", inputParamList);
        }

        internal ADO_readerOutput TableRead(string tsbTable = null)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();
            if (tsbTable != null)
                inputParamList.Add(new ADO_inputParams() { name = "@TsbTable", value = tsbTable });

            return _ado.ExecuteReaderProcedure("Subscription_Table_Subscription_Read", inputParamList);
        }
    }
}
