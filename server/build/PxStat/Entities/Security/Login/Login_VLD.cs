using API;
using FluentValidation;

namespace PxStat.Security
{
    internal class Login_VLD_Create : AbstractValidator<Login_DTO_Create>
    {
        internal Login_VLD_Create()
        {
            RuleFor(f => f.CcnUsername).Length(1, 256).When(f => !string.IsNullOrEmpty(f.CcnUsername)).WithMessage("Invalid  username").WithName("AccountUsernameValidation");
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    internal class Login_VLD_Read2FA : AbstractValidator<Login_DTO_Read2FA>
    {
        internal Login_VLD_Read2FA()
        {
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    internal class Login_VLD_Reset : AbstractValidator<Login_DTO_Reset>
    {
        internal Login_VLD_Reset()
        {
            RuleFor(f => f.CcnUsername).Length(1, 256).When(f => !string.IsNullOrEmpty(f.CcnUsername)).WithMessage("Invalid  username").WithName("AccountUsernameValidation");

        }
    }
    internal class Login_VLD_Create2FA : AbstractValidator<Login_DTO_Create2FA>
    {
        internal Login_VLD_Create2FA()
        {
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            RuleFor(x => x.CcnEmail).Matches(emailRegex);
            RuleFor(x => x.LgnToken2Fa).NotEmpty();
        }
    }


    internal class Login_VLD_Update2FACurrent : AbstractValidator<Login_DTO_Update2FACurrent>
    {
        internal Login_VLD_Update2FACurrent()
        {

        }
    }

    internal class Login_VLD_InitiateUpdate2FA : AbstractValidator<Login_DTO_InitiateUpdate2FA>
    {
        internal Login_VLD_InitiateUpdate2FA()
        {
            RuleFor(f => f.CcnUsername).Length(1, 256).WithMessage("Invalid  username").WithName("AccountUsernameValidation");
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    internal class Login_VLD_Update2FA : AbstractValidator<Login_DTO_Update2FA>
    {
        internal Login_VLD_Update2FA()
        {
            RuleFor(f => f.CcnUsername).Length(1, 256).WithMessage("Invalid  username").WithName("AccountUsernameValidation");
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    internal class Login_VLD_InitiateUpdate1Fa : AbstractValidator<Login_DTO_InitiateUpdate1Fa>
    {
        internal Login_VLD_InitiateUpdate1Fa()
        {
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            RuleFor(x => x.CcnEmail).Matches(emailRegex);
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    internal class Login_VLD_InitiateLoginForAD : AbstractValidator<Login_DTO_InititateLoginForAD>
    {
        internal Login_VLD_InitiateLoginForAD()
        {
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).WithMessage("Invalid  username").WithName("AccountUsernameValidation");
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    internal class Login_VLD_Read : AbstractValidator<Login_DTO_Read>
    {
        internal Login_VLD_Read()
        {
            RuleFor(x => x.LgnToken).NotEmpty();
        }
    }

    //Update1FA_Session
    internal class Login_VLD_Update1FA_Session : AbstractValidator<Login_DTO_Update1FA_Session>
    {
        internal Login_VLD_Update1FA_Session()
        {

            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");


        }

    }

    internal class Login_VLD_ReadOpen1FA : AbstractValidator<Login_DTO_ReadOpen1FA>
    {
        internal Login_VLD_ReadOpen1FA()
        {
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            RuleFor(x => x.CcnEmail).Matches(emailRegex);
        }
    }

    internal class Login_VLD_ReadOpen2FA : AbstractValidator<Login_DTO_ReadOpen2FA>
    {
        internal Login_VLD_ReadOpen2FA()
        {
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            RuleFor(x => x.CcnEmail).Matches(emailRegex);
        }
    }

    internal class Login_VLD_Create1FA : AbstractValidator<Login_DTO_Create1FA>
    {
        internal Login_VLD_Create1FA()
        {
            RuleFor(x => x.Captcha).NotEmpty();
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            string passwordRegex = Configuration_BSO.GetCustomConfig(ConfigType.global, "regex.password");
            RuleFor(x => x.CcnEmail).NotEmpty().Matches(emailRegex).WithMessage("Invalid email");
            //RuleFor(f => f.Lgn1FA).NotEmpty().MinimumLength(8).WithMessage("Invalid password").WithName("1FAValidation");
            RuleFor(f => f.LgnToken1Fa).NotEmpty();
            RuleFor(f => f.Lgn1Fa).Matches(passwordRegex);
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");

        }

    }
    //Login_DTO_UpdateForgotten1FA
    internal class Login_VLD_Update1FA : AbstractValidator<Login_DTO_Update1FA>
    {
        internal Login_VLD_Update1FA()
        {
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            string passwordRegex = Configuration_BSO.GetCustomConfig(ConfigType.global, "regex.password");
            RuleFor(f => f.Lgn1Fa).NotEmpty().MinimumLength(8).WithMessage("Invalid password").WithName("1FAValidation");
            RuleFor(f => f.LgnToken1Fa).NotEmpty();
            RuleFor(f => f.Lgn1Fa).Matches(passwordRegex);
            RuleFor(x => x.CcnEmail).NotEmpty().Matches(emailRegex).WithMessage("Invalid email");

        }

    }
    //
    internal class Login_VLD_Login_InitiateForgotten1Fa : AbstractValidator<Login_DTO_InitiateForgotten1Fa>
    {

        internal Login_VLD_Login_InitiateForgotten1Fa()
        {
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            RuleFor(x => x.CcnEmail).NotEmpty().Matches(emailRegex).WithMessage("Invalid email");
            RuleFor(x => x.Captcha).NotEmpty();
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }

    }

    internal class Login_VLD_Logout : AbstractValidator<Login_DTO_Logout>
    {
        internal Login_VLD_Logout() { }
    }



    internal class Login_VLD_Login : AbstractValidator<Login_DTO_Login>
    {
        internal Login_VLD_Login()
        {
            RuleFor(x => x.Lgn1Fa).NotEmpty();
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            RuleFor(x => x.CcnEmail).NotEmpty().Matches(emailRegex).WithMessage("Invalid email");
            RuleFor(x => x.Totp).NotEmpty();
            RuleFor(x => x.Captcha).NotEmpty();
        }
    }

    internal class Login_VLD_InitiateForgotten2FA : AbstractValidator<Login_DTO_InitiateForgotten2FA>
    {
        internal Login_VLD_InitiateForgotten2FA()
        {
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            RuleFor(x => x.CcnEmail).NotEmpty().Matches(emailRegex).WithMessage("Invalid email");
            RuleFor(x => x.Captcha).NotEmpty();
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }


}
