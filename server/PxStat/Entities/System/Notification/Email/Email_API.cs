using API;

namespace PxStat.System.Notification
{
    public class Email_API
    {
        /// <summary>
        /// Reads one or more subject
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic GroupMessageCreate(JSONRPC_API jsonrpcRequest)
        {
            return new Email_BSO_GroupMessage_Create(jsonrpcRequest).Read().Response;
        }
    }
}