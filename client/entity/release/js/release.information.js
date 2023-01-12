/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    // Bind events
    $("#release-information").find("[name=update-navigation]").once("click", function () {
        app.release.navigation.read();
    });
    $("#release-information").find("[name=update-association]").once("click", function () {
        app.release.navigation.ajax.associationRead();
    });
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});

