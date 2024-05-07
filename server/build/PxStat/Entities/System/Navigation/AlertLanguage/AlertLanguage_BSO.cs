using API;
using PxStat.System.Navigation;


namespace PxStat.Entities.System.Navigation.AlertLanguage
{
    /// <summary>
    /// Class for managing other language versions of alerts
    /// </summary>
    internal class AlertLanguage_BSO
    {
        internal int CreateOrUpdate(Alert_DTO dto, IADO ado)
        {
            AlertLanguage_ADO alAdo = new AlertLanguage_ADO(ado);
            return alAdo.CreateOrUpdate(dto);
        }
    }
}