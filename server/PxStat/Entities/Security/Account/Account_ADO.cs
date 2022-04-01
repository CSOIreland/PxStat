using API;
using PxStat.Resources;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// ADO classes for Account
    /// </summary>
    internal class Account_ADO : DataAdaptor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal Account_ADO()
        {
        }

        /// <summary>
        /// Reads the account table based on an Account_DTO_Read parameter
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, Account_DTO_Read account, bool adOnly = false, string ccnEmail = null)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            if (!string.IsNullOrEmpty(account.CcnUsername))
            {
                paramList.Add(new ADO_inputParams() { name = "@CcnUsername", value = account.CcnUsername });

            }

            if (!string.IsNullOrEmpty(account.PrvCode))
            {
                paramList.Add(new ADO_inputParams() { name = "@PrvCode", value = account.PrvCode });

            }
            if (adOnly)
            {
                paramList.Add(new ADO_inputParams() { name = "@AdFlag", value = true });
            }

            if (ccnEmail != null)
            {
                paramList.Add(new ADO_inputParams() { name = "@CcnEmail", value = ccnEmail });
            }


            ADO_readerOutput output = ado.ExecuteReaderProcedure("Security_Account_Read", paramList);

            return output;
        }



        /// <summary>
        /// Reads account based on account name
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="CcnUsername"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, string CcnUsername)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            paramList.Add(new ADO_inputParams() { name = "@CcnUsername", value = CcnUsername });

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Security_Account_Read", paramList);

            return output;
        }

        /// <summary>
        /// Take the output from a Read and return it as a list of Account_DTO_Update objects
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        internal List<Account_DTO_Update> ReadAccounts(ADO_readerOutput output)
        {
            if (!output.hasData) return null;

            List<Account_DTO_Update> accounts = new List<Security.Account_DTO_Update>();

            foreach (var item in output.data)
            {
                Account_DTO_Update account = new Account_DTO_Update();

                account.CcnUsername = ReadString(item.CcnUsername);
                account.PrvCode = ReadString(item.PrvCode);
                account.CcnNotificationFlag = ReadBool(item.CcnNotificationFlag);
                account.CcnLockedFlag = ReadBool(item.CcnLockedFlag);
                accounts.Add(account);
            }
            return accounts;
        }

        /// <summary>
        /// Reads users with the supplied privilege and higher
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadMinimumPrivilege(ADO ado, string prvCode, bool readHigherPrivileges = false)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>() { new ADO_inputParams() { name = "@PrvCode", value = prvCode } };

            if (readHigherPrivileges)
            {
                paramList.Add(new ADO_inputParams() { name = "@ReadHigherPrivileges", value = true });

            }

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Security_Account_ReadMinimumPrivilege", paramList);

            return output;
        }


        //


        /// <summary>
        /// Get approvers for a Release
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadReleaseApprovers(ADO ado, Account_DTO_ReadIsApprover dto)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            { new ADO_inputParams() { name = "@RlsCode", value = dto.RlsCode } };


            if (!string.IsNullOrEmpty(dto.CcnUsername))
            {
                paramList.Add(new ADO_inputParams() { name = "@CcnUsername", value = dto.CcnUsername });

            }

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Security_Account_ReadReleaseApprovers", paramList);

            return output;
        }


        /// <summary>
        /// Get a list of users involved in a specific release. The isApprover flag is ignored if it is false
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="rlsCode"></param>
        /// <param name="isApprover"></param>
        /// <param name="prvCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadReleaseUsers(ADO ado, int rlsCode, bool? isApprover, string prvCode = null)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            { new ADO_inputParams() { name = "@RlsCode", value = rlsCode } };

            if (isApprover != null)
            {

                paramList.Add(new ADO_inputParams() { name = "@GccApproveFlag", value = (bool)isApprover });


            }

            if (prvCode != null)
            {
                paramList.Add(new ADO_inputParams() { name = "@PrvCode", value = prvCode });
            }

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Security_Account_ReadReleaseApprovers", paramList);

            return output;
        }


        /// <summary>
        /// Creates a new account in the account table
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="account"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(ADO ado, Account_DTO_Create account, string ccnUsername, bool ccnAdFlag, bool locked = false)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsernameCreator",value=ccnUsername},
                new ADO_inputParams() {name= "@CcnUsernameNewAccount",value=account.CcnUsername},
                new ADO_inputParams() {name= "@PrvCode",value=account.PrvCode},
                new ADO_inputParams() {name="@CcnNotificationFlag",value=account.CcnNotificationFlag  },
                new ADO_inputParams() {name="@CcnLockedFlag",value=locked  },
                new ADO_inputParams() {name="@CcnADFlag",value=ccnAdFlag  },
                new ADO_inputParams() { name ="@LngIsoCode", value=account.LngIsoCode }
            };

            if (account.CcnDisplayName != null)
                inputParamList.Add(new ADO_inputParams() { name = "@CcnDisplayName", value = account.CcnDisplayName });

            if (account.CcnEmail != null)
                inputParamList.Add(new ADO_inputParams() { name = "@CcnEmail", value = account.CcnEmail });

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Account_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Updates an account in the account table
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="account"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal int Update(ADO ado, Account_DTO_Update account, string username)
        {
            //Get the input parameters for database read
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsernameUpdater",value=username},
                new ADO_inputParams() {name= "@UpdatedCcnUsername",value=account.CcnUsername}

            };

            if (account.CcnNotificationFlag != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@CcnNotificationFlag", value = account.CcnNotificationFlag });
            }

            if (account.PrvCode != null)
            {

                inputParamList.Add(new ADO_inputParams() { name = "@PrvCode", value = account.PrvCode });
            }

            if (account.LngIsoCode != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = account.LngIsoCode });
            }

            if (account.CcnLockedFlag != null)
                inputParamList.Add(new ADO_inputParams() { name = "@CcnLockedFlag", value = account.CcnLockedFlag });


            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Account_Update", inputParamList, ref retParam);



            //Assign the returned value for checking and output
            return retParam.value;
        }



        /// <summary>
        /// Deletes an account from the account table
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="account"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(ADO ado, Account_DTO_Delete account, string ccnUsername)
        {
            //Create a list for the input parameters
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsernameDeleter",value=ccnUsername},
                new ADO_inputParams() {name= "@CcnUsernameToBeDeleted",value=account.CcnUsername}
            };


            //Create a return parameter
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "@return";
            retParam.value = 0;

            //Executing the stored procedure
            ado.ExecuteNonQueryProcedure("Security_Account_Delete", inputParamList, ref retParam);


            Log.Instance.Debug("Number of records affected: " + retParam.value);
            return retParam.value;
        }

        /// <summary>
        /// Checks if an account exists
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="CcnUsername"></param>
        /// <returns></returns>
        internal bool Exists(ADO ado, string CcnUsername)
        {
            Account_DTO_Read dto = new Security.Account_DTO_Read();
            dto.CcnUsername = CcnUsername;
            ADO_readerOutput output = this.Read(ado, dto);
            if (output.hasData)
            {
                return true;
            }
            else return false;
        }

        internal bool ExistsByEmail(ADO ado, string ccnEmail)
        {
            Account_DTO_Read dto = new Security.Account_DTO_Read();

            ADO_readerOutput output = this.Read(ado, dto, false, ccnEmail);
            if (output.hasData)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Checks for a given Privilege Code if there are at least one account with that privilege
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="prvCode"></param>
        /// <returns></returns>
        internal bool EnoughPrivilegesInAccounts(ADO ado, string prvCode)
        {
            ADO_inputParams param = new ADO_inputParams();
            param.name = "@PrvCode";
            param.value = prvCode;

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(param);

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;
            ado.ExecuteNonQueryProcedure("Security_Account_PrivilegeCount", paramList, ref retParam); ;

            if (retParam.value > 1)
            {
                //There is more than one of the privilege in the account table 
                return true;
            }
            else
            {
                //There is one or fewer of the privileges in the account table
                return false;
            }
        }
    }
}
