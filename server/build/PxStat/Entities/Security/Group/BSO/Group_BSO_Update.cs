using API;
using Newtonsoft.Json;
using PxStat.Data;
using PxStat.DataStore;
using PxStat.Resources;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Updates a Group
    /// </summary>
    internal class Group_BSO_Update : BaseTemplate_Update<Group_DTO_Update, Group_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Group_BSO_Update(JSONRPC_API request) : base(request, new Group_VLD_Update())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            var adoGroup = new Group_ADO();

            if (!DTO.GrpCodeNew.Equals(DTO.GrpCodeOld))
            {
                //We are changing the Group Code but first we must check if the new Group Code exists already (we can't have duplicates)
                if (adoGroup.Exists(Ado, DTO.GrpCodeNew))
                {
                    //This Group exists already, we can't proceed
                    Log.Instance.Debug("Group exists already - create request refused");
                    Response.error = Label.Get("error.duplicate");
                    return false;
                }
            }

            //Update the Group - and retrieve the number of updated rows
            int nUpdated = adoGroup.Update(Ado, DTO, SamAccountName);
            if (nUpdated == 0)
            {
                Log.Instance.Debug("Can't update the Group");
                Response.error = Label.Get("error.update");
                return false;
            }
            FlushAssociatedMatrixes(Ado, DTO);
            Log.Instance.Debug("Group updated: " + JsonConvert.SerializeObject(DTO));

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];

            return true;
        }

        /// <summary>
        /// If an update has taken place, we must flush the caches for all associated matrixes (because the contact details are part of the matrix)
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="dto"></param>
        private void FlushAssociatedMatrixes(IADO Ado, Group_DTO_Update dto)
        {
            DataStore_ADO mAdo = new DataStore_ADO();

            //Get all the matrixes for Group
            var readGroupAccess = mAdo.ReadByGroup(Ado,dto.GrpCodeOld, Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"));

            if (!readGroupAccess.hasData) return;


            //look maybe at ensuring there are no dupes (or maybe a switch to first return only live data..)
            foreach (var matrix in readGroupAccess.data)
            {
                
              Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.MtrCode);
              Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + matrix.MtrCode);

            }


        }
    }
}
