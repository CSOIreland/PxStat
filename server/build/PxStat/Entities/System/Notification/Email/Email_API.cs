﻿using API;

namespace PxStat.System.Notification
{
    [AllowAPICall]
    public class Email_API
    {
        /// <summary>
        /// Reads one or more subject
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        /// 
        [NoDemo]
        public static dynamic GroupMessageCreate(JSONRPC_API jsonrpcRequest)
        {
            return new Email_BSO_GroupMessage_Create(jsonrpcRequest).Read().Response;
        }
    }
}