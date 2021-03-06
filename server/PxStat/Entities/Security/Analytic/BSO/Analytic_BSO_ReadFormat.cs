﻿using API;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;

namespace PxStat.Security
{
    internal class Analytic_BSO_ReadFormat : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadFormat>
    {
        internal Analytic_BSO_ReadFormat(JSONRPC_API request) : base(request, new Analytic_VLD_ReadFormat())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Analytic_ADO ado = new Analytic_ADO(Ado);
            ADO_readerOutput outputSummary = ado.ReadFormat(DTO,SamAccountName );
            if (outputSummary.hasData)
            {
                Response.data = FormatData(outputSummary.data);
                return true;
            }
            return false;
        }

        private dynamic FormatData(dynamic readData)
        {
            dynamic output = new ExpandoObject();
            var itemDict = output as IDictionary<string, object>;

            foreach (dynamic item in readData)
            {

                itemDict.Add(item.FrmTypeVersion, item.NltCount);


            }


            return output;
        }

    }
}
