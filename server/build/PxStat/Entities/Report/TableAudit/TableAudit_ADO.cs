using API;
using PxStat.Security;
using System;
using System.Collections.Generic;


namespace PxStat.Report
{
    /// <summary>
    /// ADO methods for TableAudit
    /// </summary>
    internal class TableAudit_ADO
    {
        /// <summary>
        /// Read results from database
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="DatetimeStart"></param>
        /// <param name="DatetimeEnd"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(IADO ado, TableAudit_DTO_Read dto, string userName)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@StartDate",value=dto.DateFrom},

                // Set time to 11:59:59
                new ADO_inputParams() {name= "@EndDate",value=dto.DateTo.AddDays(1).AddSeconds(-1)}

            };
            if (dto.GrpCode != null && dto.GrpCode.Count != 0)
            {
                var grpCodes = String.Join(",", dto.GrpCode);
                inputParamList.Add(new ADO_inputParams() { name = "@GrpCode", value = grpCodes });
            }

            if (dto.RsnCode != null && dto.RsnCode.Count != 0)
            {
                var rsnCodes = String.Join(",", dto.RsnCode);
                inputParamList.Add(new ADO_inputParams() { name = "@RsnCode", value = rsnCodes });
            }

            inputParamList.Add(new ADO_inputParams
            {
                name = "@LngIsoCode", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code")
            });

            var reader = ado.ExecuteReaderProcedure("Report_TableAudit", inputParamList);

            return reader;

        }


    }
}
