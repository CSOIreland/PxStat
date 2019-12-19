/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.layout.set(false);
    app.navigation.breadcrumb.set([app.label.static["manage"], app.label.static["email"]]);

    // Populate the Group dropdown
    app.email.ajax.selectGroup();
    //define validation
    app.email.validation.create();
    // Initiate all text areas as tinyMCE
    app.library.utility.initTinyMce();

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();

    //Confirm reset and reset details
    $("#email-container").find("[name=button-reset]").once("click", function () {

        app.email.reset();

    });
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});