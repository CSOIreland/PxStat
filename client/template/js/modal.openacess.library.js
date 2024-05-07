//#region Namespaces 
app.openAccess = {};
app.openAccess.setUp1FACaptcha = null;
app.openAccess.setUp2FACaptcha = null;
app.openAccess.userLoginCaptcha = null;
app.openAccess.forgot1FACaptcha = null;
app.openAccess.reset2FACaptcha - null;
app.openAccess.modal = {};
app.openAccess.ajax = {};
app.openAccess.callback = {};

app.openAccess.validation = {};

//#endregion


//#region set up 1Fa
app.openAccess.ajax.readOpen1FA = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Security.Login_API.ReadOpen1FA",
        {
            "CcnEmail": api.uri.getParam("email")
        },
        "app.openAccess.callback.readOpen1FA");
}

app.openAccess.callback.readOpen1FA = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        app.openAccess.modal.setUp1FA();
    }
    else {
        $('#modal-error').attr("data-backdrop", "static");
        api.modal.error(app.label.static["open-access-error-link-expired"]);

        $('#modal-error').on('hide.bs.modal', function (e) {
            // Force the reload of the application 

            window.location.href = window.location.pathname;
        })
    }
}
/**
 *  Validation function for user set up password
 */
app.openAccess.modal.setUp1FA = function () {
    $("#modal-open-access-setup-1fa").find("[name=name]").text(api.uri.getParam("name"));
    $("#modal-open-access-setup-1fa").find("[name=email]").text(api.uri.getParam("email"));

    app.openAccess.validation.setUp1FA();

    $("#modal-open-access-setup-1fa").modal("show");
};

app.openAccess.validation.setUp1FA = function () {
    $("#modal-open-access-setup-1fa form").trigger("reset").validate({
        rules: {
            "password":
            {
                required: true,
                validPassword: true
            },
            "repeat-password": {
                required: true,
                equalTo: "#modal-open-access-setup-1fa [name=password]"
            }
        },
        messages: {
            "repeat-password": {
                equalTo: app.label.static["invalid-password-match"]
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-open-access-setup-1fa [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            if (grecaptcha.getResponse(app.openAccess.setUp1FACaptcha).length) {
                $("#modal-open-access-setup-1fa").find("[name=captcha-error]").hide();
                app.openAccess.ajax.setUp1FA();
            }
            else {
                $("#modal-open-access-setup-1fa").find("[name=captcha-error]").show();
            }
        }
    }).resetForm();
};

app.openAccess.ajax.setUp1FA = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        api.uri.getParam("method"),
        {
            "LgnToken1Fa": api.uri.getParam("token"),
            "Lgn1Fa": $('#modal-open-access-setup-1fa [name=password]').val(),
            "Captcha": grecaptcha.getResponse(app.openAccess.setUp1FACaptcha),
            "CcnEmail": api.uri.getParam("email")
        },
        "app.openAccess.callback.setUp1FA");
}

app.openAccess.callback.setUp1FA = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        switch (api.uri.getParam("method")) {
            case "PxStat.Security.Login_API.Create1FA":
                api.modal.success(app.library.html.parseDynamicLabel("open-access-create-password-set", [api.uri.getParam("email")]));
                break;
            case "PxStat.Security.Login_API.Update1FA":
                api.modal.success(app.label.static["open-access-update-password-set"]);
                break;
            default:
                break;
        }

        $('#modal-success').on('hide.bs.modal', function (e) {
            // Force the reload of the application 
            $("#modal-open-access-setup-1fa").modal("hide");

        })

    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
}
//#endregion

//#region set up 2Fa
app.openAccess.ajax.readOpen2FA = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Security.Login_API.ReadOpen2FA",
        {
            "CcnEmail": api.uri.getParam("email")
        },
        "app.openAccess.callback.readOpen2FA");
}

app.openAccess.callback.readOpen2FA = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        app.openAccess.modal.setUp2FA();
    }
    else {
        $('#modal-error').attr("data-backdrop", "static");
        api.modal.error(app.label.static["open-access-error-link-expired"]);

        $('#modal-error').on('hide.bs.modal', function (e) {
            // Force the reload of the application 

            window.location.href = window.location.pathname;
        })
    }
}

/**
 * Generate QR code
 */
app.openAccess.modal.setUp2FA = function () {
    app.openAccess.validation.setUp2FA();
    $("#modal-open-access-setup-2fa").find("[name=name]").text(api.uri.getParam("name"));
    $("#modal-open-access-setup-2fa").find("[name=email]").text(api.uri.getParam("email"));
    $("#modal-open-access-setup-2fa").modal("show");
}



app.openAccess.validation.setUp2FA = function () {
    $("#modal-open-access-setup-2fa form").trigger("reset").validate({
        rules: {
            "smart-device":
            {
                required: true
            },
            "authenticator-app":
            {
                required: true
            },
            "secure-location":
            {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-open-access-setup-2fa [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            if (grecaptcha.getResponse(app.openAccess.setUp2FACaptcha).length) {
                $("#modal-open-access-setup-2fa").find("[name=captcha-error]").hide();
                app.openAccess.ajax.setUp2FA();
            }
            else {
                $("#modal-open-access-setup-2fa").find("[name=captcha-error]").show();
            }
        }
    }).resetForm();
};

app.openAccess.ajax.setUp2FA = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        api.uri.getParam("method"),
        {
            "LgnToken2Fa": api.uri.getParam("token"),
            "CcnEmail": api.uri.getParam("email"),
            "Captcha": grecaptcha.getResponse(app.openAccess.setUp2FACaptcha)
        },
        "app.openAccess.callback.setUp2FA");
}

app.openAccess.callback.setUp2FA = function (data) {
    if (data) {
        $("#modal-open-access-setup-2fa [name=qr-generate-row], #modal-open-access-setup-2fa [name=captcha-row], #modal-open-access-setup-2fa [name=secure-location-row], #modal-open-access-setup-2fa [name=authenticator-app-row], #modal-open-access-setup-2fa [name=smart-device-row], #modal-open-access-setup-2fa [name=open-access-2fa-checks]").hide();
        $("#modal-open-access-setup-2fa").find("[name=qr-row]").fadeIn();
        $('#modal-open-access-setup-2fa [name=qr-code]').qrcode("otpauth://totp/" + app.config.entity.openAccess.authenticator + "?secret=" + data);
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }

}
//#endregion

//#region user login
app.openAccess.modal.login = function () {
    app.openAccess.validation.login();
    $("#modal-open-access-user-login").find("[name=login]").show();
    $("#modal-open-access-user-login").find("[name=verify]").hide();
    if (!app.config.security.adOpenAccess) {
        $("#modal-open-access-user-login").find("[name=ad-user-disabled]").show().html(
            app.library.html.parseDynamicLabel("open-access-ad-user-disabled", [app.config.organisation])
        );
    }
    //set timeout to close modal as per session config
    $("#modal-open-access-user-login").modal("show").on('hide.bs.modal', function (e) {
        $("#modal-open-access-user-login [name=login] form").trigger("reset");
        grecaptcha.reset(app.openAccess.userLoginCaptcha)
    });

    $("#modal-open-access-user-login").find("[name=forgotten-password]").once("click", function (e) {
        e.preventDefault();
        app.openAccess.modal.forgot1FA();
    });

    $("#modal-open-access-user-login").find("[name=reset-2fa]").once("click", function (e) {
        e.preventDefault();
        app.openAccess.modal.reset2fa();
    });
};

/**
 *  Validation function for User Login
 */
app.openAccess.validation.login = function () {
    $("#modal-open-access-user-login [name=login] form").trigger("reset").validate({
        rules: {
            "email":
            {
                required: true,
                validEmailAddress: true
            },
            "password":
            {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-open-access-user-login [name=login] [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            if (grecaptcha.getResponse(app.openAccess.userLoginCaptcha).length) {
                $("#modal-open-access-user-login").find("[name=captcha-error]").hide();
                app.openAccess.modal.verify();
            }
            else {
                $("#modal-open-access-user-login").find("[name=captcha-error]").show();
            }
        }
    }).resetForm();
};

app.openAccess.modal.verify = function () {
    app.openAccess.validation.verify();
    $("#modal-open-access-user-login [name=verify]").find("[name=email]").text($("#modal-open-access-user-login [name=login]").find("[name=email]").val())
    $("#modal-open-access-user-login [name=login]").hide();
    $("#modal-open-access-user-login [name=verify]").show();
    $("#modal-open-access-user-login [name=verify]").find("[name=authentication-code]").focus();

}

app.openAccess.validation.verify = function () {
    $("#modal-open-access-user-login [name=verify] form").trigger("reset").validate({
        rules: {
            "authentication-code":
            {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-open-access-user-login [name=verify] [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            if (grecaptcha.getResponse(app.openAccess.userLoginCaptcha).length) {
                app.openAccess.ajax.login()
            }
            else {
                $("#modal-open-access-user-login").find("[name=login]").show();
                $("#modal-open-access-user-login").find("[name=verify]").hide();
            }

        }
    }).resetForm();
};

app.openAccess.ajax.login = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Security.Login_API.Login",
        {
            "CcnEmail": $("#modal-open-access-user-login").find("[name=login]").find("[name=email]").val(),
            "Lgn1Fa": $("#modal-open-access-user-login").find("[name=login]").find("[name=password]").val(),
            "Totp": $("#modal-open-access-user-login").find("[name=verify]").find("[name=authentication-code]").val(),
            "Captcha": grecaptcha.getResponse(app.openAccess.userLoginCaptcha)
        },
        "app.openAccess.callback.login",
        null,
        null,
        null,
        { async: false });
}

app.openAccess.callback.login = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#modal-open-access-user-login").modal("hide");
        // Force the reload of the application 

        window.location.href = window.location.pathname;
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
}

app.openAccess.callback.loginCaptchaExpired = function () {
    $("#modal-open-access-user-login").find("[name=login]").show();
    $("#modal-open-access-user-login").find("[name=verify]").hide();
}
//#endregion

//#region logout
app.openAccess.modal.logout = function () {
    //clean up language cookie. It will be reset again if the user logs in
    Cookies.remove(C_COOKIE_LANGUAGE);

    api.cookie.session.end();
}
//#endregion logout
//#region forgot password
app.openAccess.modal.forgot1FA = function () {
    $("#modal-open-access-user-login").modal("hide");
    app.openAccess.validation.forgot1FA();
    $("#modal-open-access-forgot-1fa").modal("show").on('hide.bs.modal', function (e) {
        grecaptcha.reset(app.openAccess.forgot1FACaptcha)
    });
}

/**
 *  Validation function for forgot
 */
app.openAccess.validation.forgot1FA = function () {
    $("#modal-open-access-forgot-1fa form").trigger("reset").validate({
        rules: {
            "email":
            {
                required: true,
                validEmailAddress: true
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-open-access-forgot-1fa [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            if (grecaptcha.getResponse(app.openAccess.forgot1FACaptcha).length) {
                $("#modal-open-access-forgot-1fa").find("[name=captcha-error]").hide();
                app.openAccess.ajax.forgot1FA();
            }
            else {
                $("#modal-open-access-forgot-1fa").find("[name=captcha-error]").show();
            }
        }
    }).resetForm();
};

app.openAccess.ajax.forgot1FA = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Security.Login_API.InitiateForgotten1FA",
        {
            "CcnEmail": $("#modal-open-access-forgot-1fa").find("[name=email]").val(),
            "LngIsoCode": app.label.language.iso.code,
            "Captcha": grecaptcha.getResponse(app.openAccess.forgot1FACaptcha)
        },
        "app.openAccess.callback.forgot1FA",
        $("#modal-open-access-forgot-1fa").find("[name=email]").val()
    );
}

app.openAccess.callback.forgot1FA = function (data, email) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#modal-open-access-forgot-1fa").modal("hide");
        api.modal.success(app.library.html.parseDynamicLabel("open-access-initiate-password-set", [email]));
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }

}
//#endregion


//#region reset 2fa
app.openAccess.modal.reset2fa = function () {
    $("#modal-open-access-user-login").modal("hide");
    app.openAccess.validation.reset2fa();
    $("#modal-open-access-reset-2fa").modal("show").on('hide.bs.modal', function (e) {
        grecaptcha.reset(app.openAccess.reset2FACaptcha);
    });;
}

/**
 *  Validation function for forgot
 */
app.openAccess.validation.reset2fa = function () {
    $("#modal-open-access-reset-2fa form").trigger("reset").validate({
        rules: {
            "email":
            {
                required: true,
                validEmailAddress: true
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-open-access-reset-2fa [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            if (grecaptcha.getResponse(app.openAccess.reset2FACaptcha).length) {
                $("#modal-open-access-reset-2fa").find("[name=captcha-error]").hide();
                app.openAccess.ajax.reset2fa();
            }
            else {
                $("#modal-open-access-reset-2fa").find("[name=captcha-error]").show();
            }
        }
    }).resetForm();
};

app.openAccess.ajax.reset2fa = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Security.Login_API.InitiateForgotten2FA",
        {
            "CcnEmail": $("#modal-open-access-reset-2fa").find("[name=email]").val(),
            "LngIsoCode": app.label.language.iso.code,
            "Captcha": grecaptcha.getResponse(app.openAccess.reset2FACaptcha)
        },
        "app.openAccess.callback.reset2fa",
        $("#modal-open-access-forgot-2fa").find("[name=email]").val()
    );
}

app.openAccess.callback.reset2fa = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#modal-open-access-reset-2fa").modal("hide");
        api.modal.success(app.library.html.parseDynamicLabel("open-access-initiate-2fa-set", [$("#modal-open-access-reset-2fa").find("[name=email]").val()]))
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }

}
//#endregion

//#region session warning
api.cookie.session.confirmExtension = function () {
    if (!$("#modal-confirm").is(":visible")) {

        api.modal.confirm(app.label.static["open-access-confirm-session-extension"], app.openAccess.ajax.extendSession)
        $("#modal-confirm").find("[data-dismiss=modal]").once("click", function () {
            api.cookie.session.end();
        });
    }
};

app.openAccess.ajax.extendSession = function () {
    $("#modal-confirm").find("[data-dismiss=modal]").off();
    // Extend session by calling a silent API
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Account_API.ReadCurrent"
    );
};

//#endregion session warning
