using API;
using CSO.Email;
using CSO.TwoFA;
using System;

namespace PxStat.Security
{
    internal class Login_BSO : IDisposable
    {
        IADO ado;

        internal Login_BSO()
        {
            ado = AppServicesHelper.StaticADO;
        }

        internal Login_BSO(IADO Ado)
        {
            ado = Ado;
        }

        public void Dispose()
        {
           ado?.Dispose();
        }

        internal bool Update1FA(Login_DTO_Create1FA dto, string NewToken)
        {
            Login_ADO lAdo = new Login_ADO(ado);
            return lAdo.Update1FA(dto, NewToken) > 0;
        }

        internal string Update1FaTokenForUser(string CcnUsername, string Token)
        {
            Login_ADO lAdo = new Login_ADO(ado);
            if (lAdo.Update1FaTokenForUser(CcnUsername, Token) > 0)
                return Token;
            else return null;
        }

        internal string Update2FA(Login_DTO_Create2FA dto)
        {
            Login_ADO lAdo = new Login_ADO(ado);
            string login2FA = TwoFA.GenerateSharedSecret();
            if (lAdo.Update2FA(dto, login2FA) > 0)
                return login2FA;
            else
                return null;

        }



        internal int UpdateInvitationToken2Fa(string ccnUsername, string token)
        {
            Login_ADO lAdo = new Login_ADO(ado);
            return lAdo.UpdateInvitationToken2Fa(ccnUsername, token);
        }





        internal bool CreateLogin(Login_DTO_Create dto, string samAccountName, string token = null)
        {
            Login_ADO lAdo = new Login_ADO(ado);

            return lAdo.CreateLogin(dto, samAccountName, token) > 0;

        }



        internal void SendResetEmail(Login_DTO_Create dto, string token)
        {
            Resources.BBCode bbc = new Resources.BBCode();


            using (eMail email = new eMail())
            {
                string Body = "";
                string Subject = "";
                string InvitationUrl = "";

                Body = Label.Get("email.body.account-reset", dto.LngIsoCode);
                Subject = Label.Get("email.subject.account-reset", dto.LngIsoCode);
                InvitationUrl = "[url=" + Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.application") + Configuration_BSO.GetStaticConfig("APP_COOKIELINK_INVITATION_1FA") + '/' + dto.CcnUsername + '/' + token + "]" + "[/url]";


                Body = Body + Environment.NewLine + InvitationUrl;
                Body = bbc.Transform(Body, true);

                email.Body = Body;
                email.Subject = Subject;
                email.To.Add(dto.CcnEmail);

                email.Send(ApiServicesHelper.ApiConfiguration.Settings, Log.Instance);
            }
        }





        internal ADO_readerOutput ReadByToken1Fa(string token, string ccnUsername)
        {
            Login_ADO lAdo = new Login_ADO(ado);
            return lAdo.ReadBy1FaToken(token, ccnUsername);
        }

        internal ADO_readerOutput ReadByToken2Fa(string token, string ccnUsername)
        {
            Login_ADO lAdo = new Login_ADO(ado);
            return lAdo.ReadBy2FaToken(token, ccnUsername);
        }




        internal ADO_readerOutput ReadBySession(string token)
        {
            Login_ADO lAdo = new Login_ADO(ado);
            return lAdo.ReadBySession(token);
        }



        internal ADO_readerOutput Validate1Fa(string Login1Fa, string CcnUsername)
        {
            Login_ADO lAdo = new Login_ADO(ado);
            return lAdo.Validate1Fa(CcnUsername, Login1Fa);
        }

        internal bool CreateSession(string LgnSession, DateTime expiry, string CcnUsername)
        {
            Login_ADO lAdo = new Login_ADO(ado);

            return lAdo.CreateSession(LgnSession, expiry, CcnUsername); ;
        }

        internal static void ExtendSession(IADO extendAdo ,string CcnUsername)
        {
            
            try
            {

                DateTime expiry = DateTime.Now.AddSeconds(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "session.length"));

                Login_ADO lAdo = new Login_ADO(extendAdo);

                lAdo.ExtendSession(CcnUsername, expiry);
            }
            catch (Exception ex)
            {
                //Swallow the error but log the error message.
                Log.Instance.Error("Failed to extend the user session - error message: " + ex.Message);
            }



        }
    }
}
