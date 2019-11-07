using API;
using System;


namespace PxStat.System.Settings
{
    /// <summary>
    /// Language methods for use outside of API's 
    /// </summary>
    internal class Language_BSO
    {
        /// <summary>
        /// Read a language
        /// </summary>
        /// <param name="lngIsoCode"></param>
        /// <returns></returns>
        internal Language_DTO_Create Read(string lngIsoCode)
        {
            ADO Ado = new ADO("defaultConnection");
            try
            {
                Language_DTO_Create dto = new Language_DTO_Create();

                Language_ADO lAdo = new Language_ADO(Ado);
                Language_DTO_Read readDTO = new Language_DTO_Read();
                readDTO.LngIsoCode = lngIsoCode;
                var retval = lAdo.Read(readDTO);
                if (retval.hasData)
                {
                    dto.LngIsoCode = lngIsoCode;
                    dto.LngIsoName = retval.data[0].LngIsoName;
                }

                return dto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Ado.Dispose();
            }

        }
    }
}