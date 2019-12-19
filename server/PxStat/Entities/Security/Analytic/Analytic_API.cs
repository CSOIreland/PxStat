using API;


namespace PxStat.Security
{
    /// <summary>
    /// API's to read analytics. Analytics provide data regarding web activity to the site.
    /// </summary>
    public class Analytic_API
    {
        /// <summary>
        /// Read a summary of analytics
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Analytic_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Read an analytic report based on Operating System
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadOs(JSONRPC_API requestApi)
        {
            return new Analytic_BSO_ReadOs(requestApi).Read().Response;
        }

        /// <summary>
        /// Reads an analytic report based on Browser
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadBrowser(JSONRPC_API requestApi)
        {
            return new Analytic_BSO_ReadBrowser(requestApi).Read().Response;
        }

        /// <summary>
        /// Reads an analytic report based on Dates
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadTimeline(JSONRPC_API requestApi)
        {
            return new Analytic_BSO_ReadTimeline(requestApi).Read().Response;
        }

        /// <summary>
        /// Reads an analytic report based on Referrers
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadReferrer(JSONRPC_API requestApi)
        {
            return new Analytic_BSO_ReadReferrer(requestApi).Read().Response;
        }

        /// <summary>
        /// Reads an analytic report based on Languages
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadLanguage(JSONRPC_API requestApi)
        {
            return new Analytic_BSO_ReadLanguage(requestApi).Read().Response;
        }

        public static dynamic ReadFormat(JSONRPC_API requestApi)
        {
            return new Analytic_BSO_ReadFormat(requestApi).Read().Response;
        }
    }
}
