/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["email"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["email"]);
    app.navigation.setState("#nav-link-email");


    // Initiate all text areas as tinyMCE
    app.plugin.tinyMce.initiate();

    // Populate the Group dropdown
    app.email.ajax.selectGroup();

    //define validation after tinyMCE initiated
    app.email.validation.create();

    //Confirm reset and reset details
    $("#email-container").find("[name=button-reset]").once("click", function () {
        app.email.reset();
    });
    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});