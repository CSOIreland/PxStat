/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["saved-queries"]]]);
    app.navigation.setTitle(app.label.static["saved-queries"]);

    app.savedquery.ajax.getSavedQueries();

    api.content.load("#modal-entity", "entity/subscription/savedquery/index.modal.html");

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});