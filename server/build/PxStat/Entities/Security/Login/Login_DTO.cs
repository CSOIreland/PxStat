namespace PxStat.Security
{
    public class Login_DTO_Create
    {
        public string CcnUsername { get; set; }
        public string LngIsoCode { get; set; }
        public string CcnDisplayname { get; set; }

        public string CcnEmail { get; set; }

        public Login_DTO_Create(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                CcnUsername = parameters.CcnUsername;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");


        }

        public Login_DTO_Create()
        {
        }
    }

    public class Login_DTO_Read2FA
    {
        public string LgnToken { get; set; }
        public string LngIsoCode { get; set; }

        public Login_DTO_Read2FA(dynamic parameters)
        {
            if (parameters.LgnToken != null)
                LgnToken = parameters.LgnToken;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }
    }

    public class Login_DTO_Create1FA
    {
        public string LgnToken1Fa { get; set; }
        public string Lgn1Fa { get; set; }
        public string LngIsoCode { get; set; }
        public string CcnUsername { get; set; } // url encode //check public/private for all api's

        public string Captcha { get; set; }
        public string CcnEmail { get; set; }

        public Login_DTO_Create1FA(dynamic parameters)
        {
            if (parameters.LgnToken1Fa != null)
                LgnToken1Fa = parameters.LgnToken1Fa;
            if (parameters.Lgn1Fa != null)
                Lgn1Fa = parameters.Lgn1Fa;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.Captcha != null)
                Captcha = parameters.Captcha;
            if (parameters.CcnEmail != null)
            {
                CcnEmail = parameters.CcnEmail;
                CcnUsername = CcnEmail;
            }
        }

        public Login_DTO_Create1FA()
        {
        }
    }

    public class Login_DTO_InititateLoginForAD
    {
        public string CcnUsername { get; set; }
        public string LngIsoCode { get; set; }

        public Login_DTO_InititateLoginForAD(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                CcnUsername = parameters.CcnUsername;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }
    }

    public class Login_DTO_Update1FA
    {
        public string LgnToken1Fa { get; set; }
        public string Lgn1Fa { get; set; }
        public string CcnEmail { get; set; }
        public string Captcha { get; set; }
        public string CcnUsername { get; set; }

        public Login_DTO_Update1FA(dynamic parameters)
        {
            if (parameters.LgnToken1Fa != null)
                LgnToken1Fa = parameters.LgnToken1Fa;
            if (parameters.Lgn1Fa != null)
                Lgn1Fa = parameters.Lgn1Fa;
            if (parameters.Captcha != null)
                Captcha = parameters.Captcha;
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;

        }
    }


    public class Login_DTO_Update1FA_Session
    {
        public string LngIsoCode { get; set; }
        public string CcnUsername { get; set; }
        public Login_DTO_Update1FA_Session(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }
    }

    public class Login_DTO_InitiateUpdate1Fa
    {
        public string LngIsoCode { get; set; }


        public string CcnEmail { get; set; }

        public Login_DTO_InitiateUpdate1Fa(dynamic parameters)
        {
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }


    }

    public class Login_DTO_ReadOpen1FA
    {
        public string CcnEmail { get; set; }

        public Login_DTO_ReadOpen1FA(dynamic parameters)
        {
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;
        }
    }

    public class Login_DTO_ReadOpen2FA
    {
        public string CcnEmail { get; set; }
        public string CcnUsername { get; set; }

        public Login_DTO_ReadOpen2FA(dynamic parameters)
        {
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;
        }
    }

    public class Login_DTO_Create2FA
    {
        public string LgnToken2Fa { get; set; }
        public string CcnEmail { get; set; }
        public string Captcha { get; set; }
        public string CcnUsername { get; set; }

        public Login_DTO_Create2FA(dynamic parameters)
        {
            if (parameters.LgnToken2Fa != null)
                LgnToken2Fa = parameters.LgnToken2Fa;
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;
            if (parameters.Captcha != null)
                Captcha = parameters.Captcha;

        }

        public Login_DTO_Create2FA()
        {
        }
    }

    public class Login_DTO_Update2FACurrent
    {
        public string CcnUsername { get; set; }

        public string LngIsoCode { get; set; }

        public Login_DTO_Update2FACurrent(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.CcnUsername != null)
                CcnUsername = parameters.CcnUsername;
        }
    }

    public class Login_DTO_Reset
    {
        public string CcnUsername { get; set; }


        public Login_DTO_Reset(dynamic parameters)
        {

            if (parameters.CcnUsername != null)
                CcnUsername = parameters.CcnUsername;
        }
    }

    public class Login_DTO_Update2FA
    {
        public string LgnToken2Fa { get; set; }
        public string LngIsoCode { get; set; }
        public string Captcha { get; set; }
        public string CcnEmail { get; set; }
        public string CcnUsername { get; set; }

        public string CcnDisplayname { get; set; }

        public Login_DTO_Update2FA(dynamic parameters)
        {
            if (parameters.LgnToken2Fa != null)
                LgnToken2Fa = parameters.LgnToken2Fa;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.Captcha != null)
                Captcha = parameters.Captcha;
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;
        }
    }

    public class Login_DTO_InitiateUpdate2FA
    {
        public string CcnUsername { get; set; }
        public string LngIsoCode { get; set; }

        public string CcnEmail { get; set; }
        public string CcnDisplayname { get; set; }


        public Login_DTO_InitiateUpdate2FA(dynamic parameters)
        {
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;

            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

        }

        public Login_DTO_InitiateUpdate2FA()
        {
        }
    }

    public class Login_DTO_Read
    {
        public string LgnToken { get; set; }



        public Login_DTO_Read(dynamic parameters)
        {
            if (parameters.LgnToken != null)
                LgnToken = parameters.LgnToken;

        }
    }

    public class Login_DTO_InitiateForgotten1Fa
    {
        public string CcnEmail { get; set; }
        public string LngIsoCode { get; set; }
        public string Captcha { get; set; }

        public string CcnUsername { get; set; }
        public Login_DTO_InitiateForgotten1Fa(dynamic parameters)
        {
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.Captcha != null)
                Captcha = parameters.Captcha;

        }
    }

    public class Login_DTO_Logout
    {
        public Login_DTO_Logout(dynamic parameters) { }
    }



    public class Login_DTO_Login
    {
        public string CcnEmail { get; set; }

        public string Lgn1Fa { get; set; }
        public string Totp { get; set; }
        public string CcnUsername { get; set; }

        public string Captcha { get; set; }

        public Login_DTO_Login(dynamic parameters)
        {
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;
            if (parameters.Lgn1Fa != null)
                Lgn1Fa = parameters.Lgn1Fa;
            if (parameters.Totp != null)
            {
                Totp = parameters.Totp;
            }
            if (parameters.Captcha != null)
                Captcha = parameters.Captcha;
        }


    }
    public class Login_DTO_InitiateForgotten2FA
    {
        public string CcnEmail { get; set; }
        public string Captcha { get; set; }

        public string CcnUsername { get; set; }
        public string CcnDisplayname { get; set; }
        public string LngIsoCode { get; set; }

        public Login_DTO_InitiateForgotten2FA(dynamic parameters)
        {
            if (parameters.CcnEmail != null)
                CcnEmail = parameters.CcnEmail;
            if (parameters.Captcha != null)
                Captcha = parameters.Captcha;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

        }

    }
}
