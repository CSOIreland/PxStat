
using API;
using Newtonsoft.Json;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace PxStat.Build
{
    /// <summary>
    /// General purpose class for building Px files
    /// </summary>
    internal class Build_BSO
    {
        internal class MarkedCell
        {
            internal dynamic Cell { get; set; }
            internal bool Marked { get; set; }
        }



        /// <summary>
        /// Tests if the user has sufficient build permissions
        /// </summary>
        /// <param name="PrvCode"></param>
        /// <param name="BuildAction"></param>
        /// <returns></returns>
        internal bool HasBuildPermission(IADO ado, string CcnUsername, string BuildAction)
        {
            Account_ADO adoAccount = new Account_ADO();
            ADO_readerOutput result = adoAccount.Read(ado, CcnUsername);
            if (!result.hasData) return false;
            if (result.data == null) return false;
            if (result.data.Count == 0) return false;

            if (result.data[0].PrvCode.Equals(Constants.C_SECURITY_PRIVILEGE_MODERATOR))
            {
                return Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "build." + BuildAction + ".moderator");
            }
            return true;
        }




    }



    }








/// <summary>
/// 
/// </summary>
internal class DimensionDetail_DTO
{
    /// <summary>
    /// 
    /// </summary>
    internal string key { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal string value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal DimensionValue_DTO dimensionValue { get; set; }
}

/// <summary>
/// 
/// </summary>
internal class DimensionValue_DTO
{
    /// <summary>
    /// 
    /// </summary>
    internal List<DimensionDetail_DTO> details { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal string value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal string code { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal DimensionType dimType { get; set; }


    /// <summary>
    /// 
    /// </summary>
    internal DimensionValue_DTO()
    {
        this.details = new List<DimensionDetail_DTO>();
    }
}


internal struct ClassificationVariable
{
    internal int clsId;
    internal int vrbId;

    internal ClassificationVariable(int cls, int vrb)
    {
        clsId = cls;
        vrbId = vrb;
    }

}

/// <summary>
/// 
/// </summary>
enum DimensionType { CLASSIFICATION, PERIOD, STATISTIC }
