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

    if (!app.config.plugin.subscriber.enabled) {
        $("#data-dataset-selected-table").find("[name=save-table]").remove()
    };

    //cannot use default modal cancel button as this causes parent modal in releases view data to close behind also
    $("#data-dataset-table-confirm-soft").find("[name=cancel]").once("click", function () {
        $("#data-dataset-table-confirm-soft").modal("hide");
    });

    $("#data-dataset-table-confirm-hard").find("[name=cancel]").once("click", function () {
        $("#data-dataset-table-confirm-hard").modal("hide");
    });

    //table tab
    $('#data-dataset-table-nav-tab').on('show.bs.tab', function (e) {
        if (!$("#data-dataset-table-nav-content").html()) {
            api.content.load("#data-dataset-table-nav-content", "entity/data/index.dataset.table.html");
        }
    });

    //chart tab
    $('#data-dataset-chart-nav-tab').on('show.bs.tab', function (e) {
        if (!$("#data-dataset-chart-nav-content").html()) {
            api.content.load("#data-dataset-chart-nav-content", "entity/data/index.dataset.chart.html");
        }
    });

    //map tab
    $('#data-dataset-map-nav-tab').on('show.bs.tab', function (e) {
        if (!$("#data-dataset-map-nav-content").html()) {
            api.content.load("#data-dataset-map-nav-content", "entity/data/index.dataset.map.html");
        }
    });

});