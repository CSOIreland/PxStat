using API;
using PxStat.Template;
using System;
using System.Diagnostics;
using System.Web;

namespace PxStat.Security
{
    internal class Login_BSO_Login : BaseTemplate_Read<Login_DTO_Login, Login_VLD_Login>
    {
        /// <param name="request"></param>
        internal Login_BSO_Login(JSONRPC_API request) : base(request, new Login_VLD_Login())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (!ReCAPTCHA.Validate(DTO.Captcha))
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            DTO.CcnUsername = DTO.CcnEmail;
            Login_BSO lBso = new Login_BSO(Ado);

            Account_ADO aAdo = new Account_ADO();

            ADO_readerOutput response = aAdo.Read(Ado, DTO.CcnEmail);

            string user;

            if (!response.hasData)
            {
                //Email address not in the login table, try to get the username from the email address via AD
                ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();

                var adResult = adAdo.GetAdSpecificDataForEmail(DTO.CcnEmail);
                Log.Instance.Debug("AD user found from email - time ms: " + sw.ElapsedMilliseconds);

                if (adResult == null)
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }

                user = adResult.CcnUsername;

                //Now get the user details from the table

                response = aAdo.Read(Ado, user);
                if (!response.hasData)
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }

                if (response.data[0].CcnLockedFlag)
                {
                    Response.error = Label.Get("error.account.locked");
                    return false;
                }


            }
            else
                user = response.data[0].CcnUsername;


            if (response.data[0].Lgn2Fa.Equals(DBNull.Value))
            {
                Response.error = Label.Get("error.authentication");

                return false;
            }

            if (response.data[0].CcnLockedFlag)
            {
                Response.error = Label.Get("error.authentication");

                return false;
            }

            int ccnId = response.data[0].CcnId;
            string login2Fa = response.data[0].Lgn2Fa;

            if (!API.TwoFA.Validate2fa(DTO.Totp, login2Fa))
            {
                Response.error = Label.Get("error.authentication");

                return false;
            }

            response = lBso.Validate1Fa(DTO.Lgn1Fa, user);

            if (!response.hasData)
            {
                //No validation available via the Login table, try Active Directory
                long lValidatePassword = sw.ElapsedMilliseconds;
                if (!ActiveDirectory.IsPasswordValid(user, DTO.Lgn1Fa))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                Log.Instance.Debug("Elaspsed time ValidatePassword: " + (sw.ElapsedMilliseconds - lValidatePassword));

                Log.Instance.Debug("AD validation time ms: " + sw.ElapsedMilliseconds);
                //Get the remaining details from the database
                response = aAdo.Read(Ado, user);

                if (!response.hasData)
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }
            //If we have found an account, credentials are ok, but the account is locked, then we return an account locked error
            //could be AD too
            //IsUserAuthenticated needs to check if the user is locked too


            if (response.data[0].CcnLockedFlag)
            {
                Response.error = Label.Get("error.account.locked");
                return false;
            }


            string sessionToken = Utility.GetRandomSHA256(ccnId.ToString());

            DateTime expiry = DateTime.Now.AddSeconds(Configuration_BSO.GetCustomConfig(ConfigType.global, "session.length"));
            if (!lBso.CreateSession(sessionToken, expiry, user))
            {
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.sessionCookie = new HttpCookie(API.Common.SessionCookieName) { Value = sessionToken };

            Response.data = API.JSONRPC.success;
            long l = sw.ElapsedMilliseconds;
            return true;

        }

    }
}
