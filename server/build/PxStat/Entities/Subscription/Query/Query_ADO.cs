using API;
using System.Collections.Generic;

namespace PxStat.Subscription
{
    public class Query_ADO
    {
        private IADO _ado;

        internal Query_ADO(IADO ado)
        {
            _ado = ado;
        }
        internal int Create(string tagName, string matrix, Snippet snippet, int queryThreshold, string subscriberUserId = null, string ccnUsername = null)
        {
            if (subscriberUserId == null && ccnUsername == null)
            {
                return 0;
            }

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();
            if (tagName != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@TagName", value = tagName });
            }

            if (matrix != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@Matrix", value = matrix });
            }


            if (snippet != null)
            {
                if (snippet.Type != null)
                {
                    inputParamList.Add(new ADO_inputParams() { name = "@SnippetType", value = snippet.Type });
                }
                if (snippet.Query != null)
                {
                    inputParamList.Add(new ADO_inputParams() { name = "@SnippetQuery", value = snippet.Query });
                }
                if (snippet.Isogram != null)
                {
                    inputParamList.Add(new ADO_inputParams() { name = "@SnippetIsogram", value = snippet.Isogram });
                }
                inputParamList.Add(new ADO_inputParams() { name = "@FluidTime", value = snippet.FluidTime });
            }


            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }

            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }

            inputParamList.Add(new ADO_inputParams() { name = "@QueryThreshold", value = queryThreshold });

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            // Attempting to create the new entity
            _ado.ExecuteNonQueryProcedure("Subscription_Query_Create", inputParamList, ref retParam);

            // Assign the returned value for checking and output
            return retParam.value;
        }

        internal bool Delete(string userQueryId, string subscriberUserId = null, string ccnUsername = null)
        {
            if (subscriberUserId == null && ccnUsername == null)
            {
                return false;
            }

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();
            if (userQueryId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@UserQueryId", value = userQueryId });
            }

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

            // Attempting to delete the entity
            _ado.ExecuteNonQueryProcedure("Subscription_Query_Delete", inputParamList, ref retParam);

            // Assign the returned value for checking and output
            return retParam.value > 0;
        }

        internal ADO_readerOutput Read(string userQueryId, string subscriberUserId = null, string ccnUsername = null)
        {

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();
            if (userQueryId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@UserQueryId", value = userQueryId });
            }
            else
            {
                inputParamList.Add(new ADO_inputParams() { name = "@UserQueryId", value = "" });
            }

            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }

            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }

            return _ado.ExecuteReaderProcedure("Subscription_Query_Read", inputParamList);
        }

        internal ADO_readerOutput ReadAll(string subscriberUserId = null, string ccnUsername = null)
        {

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();

            if (subscriberUserId != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@SubscriberUserId", value = subscriberUserId });
            }

            if (ccnUsername != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            }

            return _ado.ExecuteReaderProcedure("Subscription_Query_ReadAll", inputParamList);
        }

    }
}
