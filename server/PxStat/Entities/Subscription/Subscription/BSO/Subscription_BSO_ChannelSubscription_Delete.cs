﻿using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_ChannelSubscription_Delete : BaseTemplate_Delete<Subscription_DTO_ChannelSubscriptionDelete, Subscription_VLD_ChannelSubscriptionDelete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_ChannelSubscription_Delete(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelSubscriptionDelete())
        {
        }
        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }
        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (SamAccountName == null)
            {

                if (!API.Firebase.Authenticate(DTO.Uid, DTO.AccessToken))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }
            Subscription_ADO ado = new Subscription_ADO(Ado);

            if (ado.ChannelDelete(DTO.ChnCode, DTO.Uid, SamAccountName))
            {
                Response.data = JSONRPC.success;
                return true;
            }
            Log.Instance.Debug("Can't delete Subscription");
            Response.error = Label.Get("error.delete");
            return false;
        }
    }

}