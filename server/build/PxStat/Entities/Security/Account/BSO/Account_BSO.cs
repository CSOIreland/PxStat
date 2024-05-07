using API;
using System;
using System.Linq;

namespace PxStat.Security
{
    public enum AuthenticationType { none, local, windows }
    public class Account_BSO : IDisposable
    {
        IADO ado;

        public void Dispose()
        {
           
        }
        public Account_BSO(IADO aDO)
        {
            ado = aDO;
        }

        public Account_BSO()
        { }
        /// <summary>
        /// Get a list of users connected to this release with a number of filter options
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="isApprover"></param>
        /// <param name="prvCode"></param>
        /// <returns></returns>
        public ADO_readerOutput getReleaseUsers(int rlsCode, bool? isApprover, string prvCode = null)
        {
            using (ado = AppServicesHelper.StaticADO)
            {
                try
                {
                    Account_ADO aAdo = new Account_ADO();
                    var result = aAdo.ReadReleaseUsers(ado, rlsCode, isApprover, prvCode);
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// Get all users of a given privilege
        /// </summary>
        /// <param name="prvCode"></param>
        /// <returns></returns>
        public ADO_readerOutput getUsersOfPrivilege(string prvCode)
        {
            using (ado = AppServicesHelper.StaticADO)
            {
                try
                {

                    Account_ADO aAdo = new Account_ADO();
                    var result = aAdo.ReadMinimumPrivilege(ado, prvCode);
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }



        public ADO_readerOutput ReadCurrentAccess(IADO Ado, string ccnUsername)
        {


            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoAccount = new Account_ADO();

            ADO_readerOutput result = adoAccount.Read(Ado, ccnUsername);
            if (result.hasData)
            {

                // Set the cache based on the data returned
               AppServicesHelper.CacheD.Store_BSO<dynamic>("PxStat.Security", "Account_API", "ReadCurrentAccesss", ccnUsername, result.data, new DateTime());

                return result;
            }

            Log.Instance.Debug("No Account data found");
            return result;
        }

        public bool IsModerator(IADO ado, string ccnUsername)
        {
            Account_ADO accountAdo = new Account_ADO();
            Account_DTO_Read dto = new Account_DTO_Read();
            dto.CcnUsername = ccnUsername;
            ADO_readerOutput output = accountAdo.Read(ado, dto);


            if (!output.hasData) return false;
            else
            {
                dynamic account = output.data.First();
                return (account.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_MODERATOR));
            }
        }

        public bool IsAdministrator(IADO ado, string ccnUsername)
        {
            if (ccnUsername == null) return false;
            Account_ADO accountAdo = new Account_ADO();
            Account_DTO_Read dto = new Account_DTO_Read();
            dto.CcnUsername = ccnUsername;
            ADO_readerOutput output = accountAdo.Read(ado, dto);


            if (!output.hasData)
                return false;
            else
            {
                dynamic account = output.data.First();
                return (account.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR));
            }
        }

        /// <summary>
        /// Checks if the user is registered on the system
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>


        public bool IsRegistered(IADO ado, string ccnUsername)
        {
            Account_ADO accountAdo = new Account_ADO();
            Account_DTO_Read dto = new Account_DTO_Read();
            dto.CcnUsername = ccnUsername;
            ADO_readerOutput output = accountAdo.Read(ado, dto);
            return output.hasData;
        }

        /// <summary>
        /// Checks if the user is a power user - based on the username parameter
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsPowerUser(IADO ado, string ccnUsername)
        {
            Account_ADO accountAdo = new Account_ADO();
            Account_DTO_Read dto = new Account_DTO_Read();
            dto.CcnUsername = ccnUsername;
            ADO_readerOutput output = accountAdo.Read(ado, dto);


            if (!output.hasData) return false;
            else
            {
                dynamic account = output.data.First();
                return (account.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_POWER_USER));
            }
        }
    }
}
