using API;
using PxStat.DataStore;
using PxStat.Template;
using System.Net;

namespace PxStat.Data
{
    internal class Cube_BSO_ReadMetadataMatrix : BaseTemplate_Read<Cube_DTO_ReadMatrixMetadata, Cube_VLD_ReadMatrixMetadata>
    {
        bool _meta;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadMetadataMatrix(IRequest request, bool meta = true) : base(request, new Cube_VLD_ReadMatrixMetadata())
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

            DataReader dr = new DataReader();
            IDmatrix matrix = dr.ReadLiveDataset(DTO.matrix, DTO.language, null, true);
            ApiMetadata amd = new ApiMetadata();
            var result = amd.GetApiMetadata(matrix, DTO.language);

            if(result==null)
            {
                Response.statusCode=HttpStatusCode.NotFound; 
                return false;
            }
            
            result.Title = result.Title + ConvertFactory.GetDimensionValues(result.Title, matrix.Dspecs[DTO.language].Dimensions);
            Response.data = result;
            return true;
        }


    }
}
