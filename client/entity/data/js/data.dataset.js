/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();

    //back to search results
    $("#data-dataset-row").find("[name=back-to-select-results] button").once("click", function () {
        app.data.dataset.callback.back();
    });

    //cannot use default modal cancel button as this causes parent modal in releases view data to close behind also
    $("#data-dataview-confirm-soft").find("[name=cancel]").once("click", function () {
        $("#data-dataview-confirm-soft").modal("hide");
    });

    $("#data-dataview-confirm-hard").find("[name=cancel]").once("click", function () {
        $("#data-dataview-confirm-hard").modal("hide");
    });
});