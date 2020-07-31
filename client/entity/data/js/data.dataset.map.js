/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    $('#data-dataset-map-nav-tab').on('shown.bs.tab', function (e) {
        //check if dimensions are already drawn. If not, draw them
        if (!$("#data-dataset-map-nav-content [name=dimension-containers]")[0].innerHTML) {
            app.data.dataset.map.drawDimensions();
        }
    })
    $('[data-toggle="tooltip"]').tooltip();
    new ClipboardJS("#data-dataset-map-accordion-api [name=copy-api-info], #data-dataset-map-accordion-api [name=copy-api-object]");
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});