/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["subscriber"]]]);
    app.navigation.setLayout(false);
    app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["subscriber"]);
    app.subscriber.ajax.readSubscriber();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});