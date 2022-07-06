using API;

namespace PxStat.System.Navigation
{
    [AllowAPICall]
    public class Keyword_API
    {
        public static dynamic ReadSynonym(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_BSO_ReadSynonym(jsonrpcRequest).Read().Response;
        }
    }
}
