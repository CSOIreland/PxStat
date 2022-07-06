using API;

namespace Px5Migrator
{
    /// <summary>
    /// Copyright methods for use outside of API's 
    /// </summary>
    internal class Copyright_BSO
    {
        /// <summary>
        /// Reads a copyright
        /// </summary>
        /// <param name="CprCode"></param>
        /// <returns></returns>


        internal Copyright_DTO_Create ReadFromValue(string CprValue)
        {
            using (IADO ado = IAdoFactory.GetAdo())
            {
                Copyright_DTO_Create dto = new Copyright_DTO_Create();

                Copyright_ADO cAdo = new Copyright_ADO();
                Copyright_DTO_Read readDTO = new Copyright_DTO_Read();
                readDTO.CprValue = CprValue;
                var retval = cAdo.Read(ado, readDTO);
                if (retval.hasData)
                {
                    dto.CprCode = retval.data[0].CprCode;
                    dto.CprValue = CprValue;
                    dto.CprUrl = retval.data[0].CprUrl;
                }

                return dto;
            }
        }



    }
}
