using API;
using System;

namespace PxStat.Security
{
    internal enum AuthenticationType { none, local, windows }
    internal class Account_BSO : IDisposable
    {
        ADO ado;

        public void Dispose()
        {
            if (ado != null)
                ado.Dispose();
        }
        internal Account_BSO(ADO aDO)
        {
            ado = aDO;
        }

        internal Account_BSO()
        { }
        /// <summary>
        /// Get a list of users connected to this release with a number of filter options
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="isApprover"></param>
        /// <param name="prvCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput getReleaseUsers(int rlsCode, bool? isApprover, string prvCode = null)
        {
            ado = new ADO("defaultConnection");
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
            finally
            {
                ado.Dispose();
            }
        }


        /// <summary>
        /// Get all users of a given privilege
        /// </summary>
        /// <param name="prvCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput getUsersOfPrivilege(string prvCode)
        {
            ado = new ADO("defaultConnection");
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
            finally
            {
                ado.Dispose();
            }
        }



        internal ADO_readerOutput ReadCurrentAccess(ADO Ado, string ccnUsername)
        {


            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoAccount = new Account_ADO();

            ADO_readerOutput result = adoAccount.Read(Ado, ccnUsername);
            if (result.hasData)
            {

                // Set the cache based on the data returned
                MemCacheD.Store_BSO<dynamic>("PxStat.Security", "Account_API", "ReadCurrentAccesss", ccnUsername, result.data, new DateTime());

                return result;
            }

            Log.Instance.Debug("No Account data found");
            return result;
        }


    }
}
