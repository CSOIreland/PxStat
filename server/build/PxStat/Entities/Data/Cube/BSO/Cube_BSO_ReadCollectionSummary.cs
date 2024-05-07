using System.Collections.Generic;
using System.Data;
using System.Linq;
using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class Cube_BSO_ReadCollectionSummary : BaseTemplate_Read<Cube_DTO_ReadCollectionSummary, Cube_VLD_ReadCollectionSummary>
    {
        bool _meta;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadCollectionSummary(IRequest request, bool meta = true) : base(request, new Cube_VLD_ReadCollectionSummary())
        {
            _meta = meta;
        }

        /// <summary>
        /// Test authentication
        /// </summary>
        /// <returns></returns>
        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //so that caches don't get mixed up..
            DTO.meta = _meta;


            Cube_BSO cBso = new Cube_BSO(Ado);

            // cache store is done in the following function
            //Response.data = cBso.ExecuteReadCollection(Ado, DTO, _meta);

            Response.data = cBso.ReadCollection(DTO.language, DTO.subject.ToString(), DTO.product);

            if(Response.data == null) 
            {
                return false;
            }

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Key");
            dataTable.Columns.Add("Value");
            dataTable.Columns.Add("Attribute");

            

            foreach (var data in Response.data)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["Key"] = data.MtrId;
                dataRow["Value"] = data.MtrTitle;
                dataTable.Rows.Add(dataRow);
            }

            List<dynamic> results = cBso.ReadTitleUpdate(dataTable);

            Response.data = from d in (List<dynamic>) Response.data
                join i in (List<dynamic>) results on d.MtrId equals i.MtrId
                select new
                {
                    CprCode = d.CprCode,
                    CprUrl = d.CprUrl,
                    CprValue = d.CprValue,
                    ExceptionalFlag = d.ExceptionalFlag,
                    FrqCode = d.FrqCode,
                    FrqValue = d.FrqValue,
                    LngIsoCode = d.LngIsoCode,
                    LngIsoName = d.LngIsoName,
                    MtrCode = d.MtrCode,
                    MtrTitle = i.MtrTitle,
                    RlsCode = d.RlsCode,
                    RlsLiveDatatimeFrom = d.RlsLiveDatatimeFrom,
                    RlsLiveDatatimeTo = d.RlsLiveDatatimeTo
                };

            return true;
        }
    }
}
