using API;


namespace PxStat
{
    [AllowAPICall]
    /// <summary>
    /// 
    /// </summary>
    public class ApiTest
    {
        
        /// <summary>
        /// This method is public and exposed to the API
        /// It always returns a JSONRPC_Output object
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        public static dynamic BasicTest(JSONRPC_API apiRequest)
        {
            JSONRPC_Output output = new JSONRPC_Output();

            output.data = "Hello from PxStat";

            return output;
        }
    }
}
