using System.Collections.Generic;

namespace PxStat.Resources
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Constants
    {
        #region Properties
        /// <summary>
        /// Text constant for administrator privilege
        /// </summary>
        internal const string C_SECURITY_PRIVILEGE_ADMINISTRATOR = "ADMINISTRATOR";

        /// <summary>
        /// Text constant for power user privilege
        /// </summary>
        internal const string C_SECURITY_PRIVILEGE_POWER_USER = "POWER_USER";

        /// <summary>
        /// Text constant for moderator privilege
        /// </summary>
        internal const string C_SECURITY_PRIVILEGE_MODERATOR = "MODERATOR";

        /// <summary>
        /// Authenticated user constant
        /// </summary>
        internal const string C_SECURITY_TRACE_AUTHENTICATED = "AUTHENTICATED";

        /// <summary>
        /// Registerd user contstant
        /// </summary>
        internal const string C_SECURITY_TRACE_REGISTERED = "REGISTERED";

        /// <summary>
        /// Anonymous user constant
        /// </summary>
        internal const string C_SECURITY_TRACE_ANONYMOUS = "ANONYMOUS";

        internal const string C_SYSTEM_PX_NAME = "PX";
        internal const string C_SYSTEM_JSON_STAT_NAME = "JSON-stat";
        internal const string C_SYSTEM_JSON_NAME = "json";
        internal const string C_SYSTEM_CSV_NAME = "CSV";


        private const string C_SYSTEM_CODE_RESERVED_WORD_VALUE = "Value";
        private const string C_SYSTEM_CODE_RESERVED_WORD_UNIT = "Unit";


        /// <summary>
        /// List of user authentication types
        /// </summary>
        /// <returns></returns>
        internal static List<string> C_SECURITY_TRACE_TYPE()
        {
            return new List<string> { C_SECURITY_TRACE_AUTHENTICATED, C_SECURITY_TRACE_REGISTERED, C_SECURITY_TRACE_ANONYMOUS };
        }

        internal static List<string> C_SYSTEM_RESERVED_WORD()
        {
            return new List<string> { C_SYSTEM_CODE_RESERVED_WORD_VALUE, C_SYSTEM_CODE_RESERVED_WORD_UNIT, C_SYSTEM_CODE_RESERVED_WORD_VALUE.ToLower(), C_SYSTEM_CODE_RESERVED_WORD_UNIT.ToLower() };
        }

        /// <summary>
        /// Workflow rejected constant
        /// </summary>
        internal const string C_WORKFLOW_STATUS_REJECT = "REJECTED";

        /// <summary>
        /// Workflow approved constant
        /// </summary>
        internal const string C_WORKFLOW_STATUS_APPROVE = "APPROVED";

        /// <summary>
        /// Workflow publish request constant
        /// </summary>
        internal const string C_WORKFLOW_REQUEST_PUBLISH = "PUBLISH";

        /// <summary>
        /// Workflow request flag constant
        /// </summary>
        internal const string C_WORKFLOW_REQUEST_PROPERTY = "PROPERTY";

        /// <summary>
        /// Workflow request rollback constant
        /// </summary>
        internal const string C_WORKFLOW_REQUEST_ROLLBACK = "ROLLBACK";

        /// <summary>
        /// Workflow delete constant
        /// </summary>
        internal const string C_WORKFLOW_REQUEST_DELETE = "DELETE";

        /// <summary>
        /// Constants applying to work in progress requests
        /// </summary>
        /// <returns></returns>
        internal static List<string> C_WORKFLOW_REQUEST_WIP()
        {
            return new List<string> { C_WORKFLOW_REQUEST_PUBLISH, C_WORKFLOW_REQUEST_DELETE };
        }

        /// <summary>
        /// Valid request types applying to currently live workflow requests with previous releases
        /// </summary>
        /// <returns></returns>
        internal static List<string> C_WORKFLOW_REQUEST_LIVE_NOW_WITH_PREVIOUS()
        {
            return new List<string> { C_WORKFLOW_REQUEST_PROPERTY, C_WORKFLOW_REQUEST_ROLLBACK, C_WORKFLOW_REQUEST_DELETE };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static List<string> C_WORKFLOW_REQUEST_LIVE_NOW_WITHOUT_PREVIOUS()
        {
            return new List<string> { C_WORKFLOW_REQUEST_PROPERTY, C_WORKFLOW_REQUEST_DELETE };
        }

        /// <summary>
        /// Valid request types applying to currently live workflow requests without previous releases
        /// </summary>
        /// <returns></returns>
        internal static List<string> C_WORKFLOW_REQUEST_LIVE_NEXT_WITH_PREVIOUS()
        {
            return new List<string> { C_WORKFLOW_REQUEST_PROPERTY, C_WORKFLOW_REQUEST_ROLLBACK, C_WORKFLOW_REQUEST_DELETE };
        }

        /// <summary>
        /// Valid request types applying to future live workflow requests without previous releases
        /// </summary>
        /// <returns></returns>
        internal static List<string> C_WORKFLOW_REQUEST_LIVE_NEXT_WITHOUT_PREVIOUS()
        {
            return new List<string> { C_WORKFLOW_REQUEST_PROPERTY, C_WORKFLOW_REQUEST_DELETE };
        }

        /// <summary>
        /// 
        /// </summary>
        // Cas Repositories(Compare And Swap)
        internal const string C_CAS_NAVIGATION_READ = "PxStat.System.Navigation.Navigation_API.Read";
        internal const string C_CAS_NAVIGATION_SEARCH = "PxStat.System.Navigation.Navigation_API.Search";

        internal const string C_CAS_DATA_COMPARE_READ_AMENDMENT = "PxStat.Data.Compare_API.ReadAmendment";
        internal const string C_CAS_DATA_COMPARE_READ_DELETION = "PxStat.Data.Compare_API.ReadDeletion";
        internal const string C_CAS_DATA_COMPARE_READ_ADDITION = "PxStat.Data.Compare_API.ReadAddition";
        internal const string C_CAS_DATA_COMPARE_READ_PREVIOUS_RELEASE = "PxStat.Data.Compare_API.ReadPreviousRelease";

        internal const string C_CAS_DATA_CUBE_READ_PRE_DATASET = "PxStat.Data.Cube_API.ReadPreDataset";
        internal const string C_CAS_DATA_CUBE_READ_PRE_METADATA = "PxStat.Data.Cube_API.ReadPreMetadata";

        internal const string C_CAS_DATA_CUBE_READ_DATASET = "PxStat.Data.Cube_API.ReadDataset";
        internal const string C_CAS_DATA_CUBE_READ_METADATA = "PxStat.Data.Cube_API.ReadMetadata";
        internal const string C_CAS_DATA_CUBE_READ_COLLECTION = "PxStat.Data.Cube_API.ReadCollection";
        #endregion
    }
}
