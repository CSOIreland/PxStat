/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["saved-queries"]]]);
    app.navigation.setTitle(app.label.static["saved-queries"]);

    app.savedquery.ajax.getSavedQueries();

    new ClipboardJS("#saved-query-widget-modal-snippet-code-copy-button");


    api.content.load("#modal-entity", "entity/subscription/savedquery/index.modal.html");


    $('#saved-query-widget-modal').once('hidden.bs.modal', function (event) {
        $("#saved-query-widget-modal-snippet-code").collapse('hide');
    });

    $('#saved-query-widget-modal-snippet-code').on('shown.bs.collapse', function (e) {
        $('#saved-query-widget-modal').animate({
            // scrollTop: '+=' + $("#" + e.target.id).parent().find("[name=snippet-code-collapse]")[0].getBoundingClientRect().top
            scrollTop: '+=' + $("#" + e.target.id)[0].getBoundingClientRect().top
        }, 1000);
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});