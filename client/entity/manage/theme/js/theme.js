/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["themes"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["themes"]);
    app.navigation.setState("#nav-link-theme");


    // Load Modal
    api.content.load("#modal-entity", "entity/manage/theme/index.modal.html");

    //display theme table
    app.theme.ajax.read();

    // Parse warning
    $("#theme-read-container [name=warning]").find("label").html(app.library.html.parseDynamicLabel("switch-default-language", [app.config.language.iso.name]).sprintf([app.config.language.iso.name]));
    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});
