using API;
using PxStat.Security;
using System;
using System.Collections.Generic;


namespace PxStat.Security
{
    internal class Account_BSO
    {

        /// <summary>
        /// Get a list of users connected to this release with a number of filter options
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="isApprover"></param>
        /// <param name="prvCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput getReleaseUsers(int rlsCode, bool isApprover, string prvCode = null)
        {
            ADO ado = new ADO("defaultConnection");
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

        internal ADO_readerOutput getUsersOfPrivilege(string prvCode)
        {
            ADO ado = new ADO("defaultConnection");
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
    }
}