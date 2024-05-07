/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["alerts"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["alerts"]);
    app.navigation.setState("#nav-link-alert");


    // Load Modal
    api.content.load("#modal-entity", "entity/manage/alert/index.modal.html");

    app.alert.ajax.read();

    // Parse warning
    $("#alert-container [name=warning]").find("label").html(app.library.html.parseDynamicLabel("switch-default-language", [app.config.language.iso.name]).sprintf([app.config.language.iso.name]));
    // Initiate all text areas as tinyMCE
    app.plugin.tinyMce.initiate();
    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});