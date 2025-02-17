/*******************************************************************************
Footer
*******************************************************************************/
$(document).ready(function () {
    // Cookie Consent - Allow button
    $("#cookie").find("[name=cookie-allow]").once("click", function () {
        app.plugin.cookiconsent.allow(true);
    });

    // Cookie Consent - Allow button
    $("#cookie").find("[name=cookie-reject]").once("click", function () {
        app.plugin.cookiconsent.deny();
    });

    // Bind Manage Cookie click event
    $("#footer, #cookie").find("[name=cookie-manage]").once("click", function (e) {
        e.preventDefault();

        // Load the Privacy (language specific) into the Modal
        api.content.load("#modal-read-privacy .modal-body", "internationalisation/privacy/" + app.label.language.iso.code + ".html");
        $("#modal-read-privacy").modal("show").on("shown.bs.modal", function () {

            if (!Cookies.get(C_COOKIE_CONSENT)) {
                // Set default deny option if none yet
                app.plugin.cookiconsent.deny(false);
            }

            // Scroll to the Cookie section
            $("#modal-read-privacy").clearQueue().animate({
                scrollTop: '+=' + $("#modal-read-privacy").find("[name=cookies]")[0].getBoundingClientRect().top
            }, 1000);
        });
    });

    if (!Cookies.get(C_COOKIE_CONSENT)) {
        // Cookie Consent - Show Banner
        $("#cookie").find("[name=cookie-banner]").fadeIn();
    } else if (Cookies.get(C_COOKIE_CONSENT) == app.plugin.cookiconsent.true) {
        // Cookie Consent - Allowed
        app.plugin.cookiconsent.allow()
    }

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});