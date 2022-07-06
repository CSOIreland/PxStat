using API;


namespace PxStat.System.Settings
{
    /// <summary>
    /// APIs to control read access to Frequencies. Frequencies are groupings of time periods.
    /// </summary>
    [AllowAPICall]
    public class Frequency_API
    {
        /// <summary>
        /// Reads one or more Frequency
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Frequency_BSO_Read(requestApi).Read().Response;
        }
    }
}