reCaptchaOnLoad = function () {
    app.openAccess.setUp1FACaptcha = grecaptcha.render('modal-open-access-setup-1fa-captcha', {
        'sitekey': app.config.entity.openAccess.recaptcha.siteKey,
        "hl": app.label.language.iso.code
    });

    app.openAccess.setUp2FACaptcha = grecaptcha.render('modal-open-access-setup-2fa-captcha', {
        'sitekey': app.config.entity.openAccess.recaptcha.siteKey
    });

    app.openAccess.userLoginCaptcha = grecaptcha.render('modal-open-access-login-captcha', {
        'sitekey': app.config.entity.openAccess.recaptcha.siteKey,
        "expired-callback": app.openAccess.callback.loginCaptchaExpired,
        "hl": app.label.language.iso.code
    });

    app.openAccess.forgot1FACaptcha = grecaptcha.render('modal-open-access-forgot-1fa-captcha', {
        'sitekey': app.config.entity.openAccess.recaptcha.siteKey,
        "hl": app.label.language.iso.code
    });

    app.openAccess.reset2FACaptcha = grecaptcha.render('modal-open-access-reset-2fa-captcha', {
        'sitekey': app.config.entity.openAccess.recaptcha.siteKey,
        "hl": app.label.language.iso.code
    });
};
