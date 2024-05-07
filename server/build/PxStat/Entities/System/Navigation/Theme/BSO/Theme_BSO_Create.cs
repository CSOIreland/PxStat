using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    internal class Theme_BSO_Create : BaseTemplate_Create<Theme_DTO_Create, Theme_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Theme_BSO_Create(JSONRPC_API request) : base(request, new Theme_VLD_Create())
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
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoTheme = new Theme_ADO(Ado);

            //You can only create a theme in the default Language
            DTO.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            //We can't allow duplicate named Themes, so we must check first
            if (adoTheme.Exists(new Theme_DTO_Read() { LngIsoCode = DTO.LngIsoCode, ThmValue = DTO.ThmValue }))
            {
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the Theme - and retrieve the newly created Id
            int newId = adoTheme.Create(DTO, SamAccountName);
            if (newId == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}
