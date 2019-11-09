/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();

    //back to search results
    $("#data-dataset-row").find("[name=back-to-select-results]").once("click", function () {
        app.data.dataset.callback.back();
    });
});