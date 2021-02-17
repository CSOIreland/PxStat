using API;
using PxStat.Template;

namespace PxStat.Security
{
    internal class Account_BSO_ReadByToken : BaseTemplate_Read<Account_DTO_Read, Account_VLD_ReadByToken>
    {
        /// <summary>
        /// This method reads only the current logged in user
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_ReadByToken(JSONRPC_API request) : base(request, new Account_VLD_ReadByToken())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (Request.sessionCookie == null)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }
            ADO_readerOutput user = null;

            using (Login_BSO lBso = new Login_BSO())
            {
                user = lBso.ReadBySession(Request.sessionCookie.Value);
                if (user.hasData)
                {
                    if (user.data[0].CcnEmail == null)
                    {
                        DTO.CcnUsername = user.data[0].CcnUsername;
                        ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
                        ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, DTO);
                        if (adDto.CcnDisplayName != null)
                        {
                            user.data[0].CcnEmail = adDto.CcnEmail;
                        }
                    }

                    Response.data = user.data;
                    return true;

                }
                else
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }

            }



        }

    }
}
