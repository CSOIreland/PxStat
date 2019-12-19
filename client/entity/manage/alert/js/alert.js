/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.layout.set(false);
    app.navigation.breadcrumb.set([app.label.static.manage, app.label.static.alerts]);
    app.alert.ajax.read();

    // Parse warning
    $("#alert-container [name=warning]").find("label").html(app.library.html.parseDynamicLabel("switch-default-language", [app.config.language.iso.name]).sprintf([app.config.language.iso.name]));
    // Initiate all text areas as tinyMCE
    app.library.utility.initTinyMce();
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});