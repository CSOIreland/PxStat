/*******************************************************************************
Privacy
*******************************************************************************/
$(document).ready(function () {

    $("#modal-read-privacy").find("[name=allow-cookies]").bootstrapToggle({
        onlabel: app.label.static["true"],
        offlabel: app.label.static["false"],
        onstyle: "primary",
        offstyle: "warning text-dark",
        height: 38,
        width: C_APP_TOGGLE_LENGTH
    });
    $("#modal-read-privacy").find("[name=allow-cookies]").bootstrapToggle(Cookies.get(C_COOKIE_CONSENT) == app.plugin.cookiconsent.true ? "on" : "off");
    $("#modal-read-privacy").find("[name=allow-cookies]").once("change", function () {
        if ($("#modal-read-privacy").find("[name=allow-cookies]").prop("checked")) {
            app.plugin.cookiconsent.allow(true);
        } else {
            app.plugin.cookiconsent.deny(true);
        }
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});