using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// BSO for SubjectLanguage
    /// </summary>
    internal class SubjectLanguage_BSO
    {
        /// <summary>
        /// Create and Update a SubjectLanguage
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="Ado"></param>
        /// <returns></returns>
        internal int CreateOrUpdate(Subject_DTO dto, ADO Ado)
        {
            SubjectLanguage_DTO subjectLanguageDTO = new SubjectLanguage_DTO();
            subjectLanguageDTO.SlgValue = dto.SbjValue;
            subjectLanguageDTO.SbjCode = dto.SbjCode;
            subjectLanguageDTO.LngIsoCode = dto.LngIsoCode;
            SubjectLanguage_ADO subjectLanguageADO = new SubjectLanguage_ADO(Ado);
            return subjectLanguageADO.CreateOrUpdate(subjectLanguageDTO);
        }
    }
}