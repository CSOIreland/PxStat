using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// BSO for SubjectLanguage
    /// </summary>
    internal class ThemeLanguage_BSO
    {
        /// <summary>
        /// Create and Update a SubjectLanguage
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="Ado"></param>
        /// <returns></returns>
        internal int CreateOrUpdate(Theme_DTO_Update dto, IADO Ado)
        {
            ThemeLanguage_DTO themeLanguageDTO = new ThemeLanguage_DTO
            {
                ThmValue = dto.ThmValue,
                ThmCode = dto.ThmCode,
                LngIsoCode = dto.LngIsoCode
            };
            ThemeLanguage_ADO themeLanguageADO = new ThemeLanguage_ADO(Ado);
            return themeLanguageADO.CreateOrUpdate(themeLanguageDTO);

        }
    }
}
