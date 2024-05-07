/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["channel"]]]);
    app.navigation.setLayout(false);
    app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["channel"]);
    app.navigation.setState("#nav-link-channel");


    app.channel.ajax.readChannel();
    app.channel.ajax.readLanguage();


    // Initiate all text areas as tinyMCE
    app.plugin.tinyMce.initiate();

    //Confirm reset and reset details
    $("#channel-container").find("[name=button-reset]").once("click", function () {
        $("#channel-container form").trigger("reset");
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});