using API;
using PxStat.Data;
using PxStat.DataStore;
using PxStat.JsonStatSchema;
using PxStat.Template;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_ReadDatasetByRelease : BaseTemplate_Read<DBuild_DTO_UpdateByRelease, DBuild_ReadDatasetByRelease_VLD>
    {
        internal DBuild_BSO_ReadDatasetByRelease(JSONRPC_API request) : base(request, new DBuild_ReadDatasetByRelease_VLD())
        {

        }
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
            // Get the ReleaseDTO
            Release_ADO adoRelease = new Release_ADO(Ado);
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(DTO.RlsCode, SamAccountName));

            // Get the matrix from the ReleaseDTO
            IDmatrix dmatrix = new Dmatrix();
            IDmatrix qmatrix = new Dmatrix();
            dmatrix = dmatrix.GetMultiLanguageMatrixFromRelease(Ado, dtoRelease);

            //Merge the changes in DTO (new variables and generate new cells) into dmatrix
            //Map the DTO to a DBuild_DTO_UpdatePublish
            DBuild_DTO_UpdatePublish dtop = new DBuild_DTO_UpdatePublish();
            dtop.Dspecs = DTO.Dspecs;
            dtop.RlsCode = DTO.RlsCode;
            dtop.ChangeData = DTO.ChangeData;
            dtop.LngIsoCode = DTO.LngIsoCode;

            DBuild_BSO dBuild_BSO = new DBuild_BSO();
            var updatedMatrix = dBuild_BSO.UpdateMatrixWithNewMetadata(ref dmatrix, dtop);

            dmatrix.Cells = dBuild_BSO.MergeOldAndNewCells(dmatrix, updatedMatrix);



            
              DataReader dr = new DataReader();

            //Get a query by mapping parts of the existing DTO to a CubeQuery_DTO
            CubeQuery_DTO query = null;
            using (var b = new DBuild_BSO())
            {
                query = b.MapReadToQueryDto(DTO, dmatrix);
            }

            dmatrix = dr.QueryDataset(query, dmatrix);
             

            FlatTableBuilder ftb = new FlatTableBuilder();
            DataTable dt = null;



            if (!DTO.Labels)
            {
                dt = ftb.GetMatrixDataTableCodesOnly(dmatrix, DTO.Dspecs[0].Language, true);
            }
            else
            {
                dt = ftb.GetMatrixDataTableCodesAndLabels(dmatrix, DTO.Dspecs[0].Language, true);
            }

            dynamic result = new ExpandoObject();
            result.csv = ftb.GetCsv(dt, "\"", null, true);
            result.MtrCode = dmatrix.Code;
            Response.data = result;
            return true;
        }

        private IDmatrix GetQueryMatrix(DBuild_DTO_UpdateByRelease dto, ref IDmatrix matrix, string lngIsoCode)
        {
            var dspec = dto.Dspecs.Where(x => x.Language.Equals(lngIsoCode)).First();
            if (dspec != null)
            {

                var tlist = dspec.StatDimensions.Where(x => x.Role.Equals("TIME")).First();



                if (tlist != null)
                {
                    if (tlist.Variables.Count == 0) return matrix;

                    var coreTime = matrix.Dspecs[lngIsoCode].Dimensions.Where(x => x.Role.Equals("TIME")).FirstOrDefault();
                    if (coreTime == null) { return matrix; }
                    List<IDimensionVariable> variables = new List<IDimensionVariable>();
                    foreach (var item in coreTime.Variables)
                    {
                        if (tlist.Variables.Select(x => x.Code).ToList().Contains(item.Code))
                            variables.Add(item);
                    }
                    coreTime.Variables = variables;


                }
                else return matrix;
            }
            else return null;

            return matrix;
        }

    }


}
