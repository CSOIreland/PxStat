using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Updates a theme
    /// </summary>
    class Theme_BSO_Update : BaseTemplate_Update<Theme_DTO_Update, Theme_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Theme_BSO_Update(JSONRPC_API request) : base(request, new Theme_VLD_Update())
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
            var adoTheme = new Theme_ADO(Ado);

            int nUpdatedThemeId;

            //We can't allow duplicate named Themes, so we must check first
            if (adoTheme.UpdateExists(new Theme_DTO_Read() { LngIsoCode = DTO.LngIsoCode, ThmCode = DTO.ThmCode, ThmValue = DTO.ThmValue }))
            {
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            if (DTO.LngIsoCode != Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"))
            {
                ThemeLanguage_BSO themeLanguageBso = new ThemeLanguage_BSO();
                nUpdatedThemeId = themeLanguageBso.CreateOrUpdate(DTO, Ado);
                if (nUpdatedThemeId == 0)
                {
                    Log.Instance.Debug("Update of ThemeLanguage failed");
                    Response.error = Label.Get("error.update");
                    return false;
                }
            }
            else
                nUpdatedThemeId = adoTheme.Update(DTO, SamAccountName);

            if (nUpdatedThemeId == 0)
            {
                Log.Instance.Debug("Update of Theme failed");
                Response.error = Label.Get("error.update");
                return false;
            }



            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];

            return true;
        }
    }
}
