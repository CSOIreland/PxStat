/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    // Bind events
    $("#release-source").find("[name=lng-iso-code]").once("change", app.release.source.languageOnChange);
    $("#release-source").find("[name=download-source]").once("click", app.release.source.download);
    $("#release-source").find("[name=view-source]").once("click", app.release.source.view);
    $("#release-source").find("[name=view-data]").once("click", app.release.data.view);

    // Load HTML for Data Set Modal 
    api.content.load("#overlay", "entity/data/index.modal.html", null, true);
    api.content.load("#data-dataset-row", "entity/data/index.dataset.html");
    api.content.load("#data-dataview-row", "entity/data/index.dataview.html");

    app.library.html.parseStaticLabel();
});

