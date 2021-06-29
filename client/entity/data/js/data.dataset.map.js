/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.data.dataset.map.drawMapToDisplay();

    if (app.data.isLive) {
        $("#data-dataset-map-nav-content").find("[name=confidential-data-warning]").hide();
    }
    else {
        $("#data-dataset-map-nav-content").find("[name=confidential-data-warning]").show();
    }

    new ClipboardJS("#data-dataset-map-accordion-api [name=copy-api-info], #data-dataset-map-accordion-api [name=copy-api-object]");
    $('#data-dataset-map-accordion-collapse-widget [name=auto-update], #data-dataset-map-accordion-collapse-widget [name=include-copyright], #data-dataset-map-accordion-collapse-widget [name=include-link], #data-dataset-map-accordion-collapse-widget [name=include-title], #data-dataset-map-accordion-collapse-widget [name=include-borders]').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "primary",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH
    });

    $('#data-dataset-map-accordion').on('show.bs.collapse', function (e) {
        $("#" + e.target.id).parent().find(".card-header i[name=accordion-icon]").removeClass().addClass("fas fa-minus-circle");
    });

    $('#data-dataset-map-accordion').on('hide.bs.collapse', function (e) {
        $("#" + e.target.id).parent().find(".card-header i[name=accordion-icon]").removeClass().addClass("fas fa-plus-circle");
    });

    $('#data-dataset-map-accordion').on('shown.bs.collapse', function (e) {
        if (app.data.isModal) {
            $('#data-view-modal').animate({
                scrollTop: '+=' + $("#" + e.target.id).parent().find(".card-header")[0].getBoundingClientRect().top
            }, 1000);
        }
        else {
            $('html, body').animate({
                scrollTop: $("#" + e.target.id).parent().find(".card-header").offset().top
            }, 1000);
        }
    });

    $("#data-dataset-map-nav-content").find("[name=download-chart]").once("click", function (e) {
        e.preventDefault();
        var url_base64jp = $("#pxwidget-map canvas")[0].toDataURL();
        var filename = (app.data.dataset.metadata.jsonStat.label.trim().replace(/ /g, "_") + "." + new Date().getTime()).toLocaleLowerCase();
        app.library.utility.download(filename, url_base64jp, "png", "image/png", true);
    });

    $("#data-dataset-map-accordion [name=download-snippet]").once("click", function () {
        // Download the snippet file
        app.library.utility.download(app.data.fileNamePrefix + '.' + moment(Date.now()).format(app.config.mask.datetime.file), $("#data-dataset-map-accordion-snippet-code").text(), C_APP_EXTENSION_HTML, C_APP_MIMETYPE_HTML);
    });

    //format advanced options
    $("#data-dataset-map-accordion [name=add-custom-configuration]").once("click", function () {
        app.data.dataset.map.formatJson();
        app.data.dataset.map.renderSnippet();
    });
    $("#data-dataset-map-accordion [name=custom-config]").val(JSON.stringify({ "options": {} }));
    app.data.dataset.map.formatJson();
    $("#data-dataset-map-accordion [name=valid-json-object], #data-dataset-map-accordion [name=invalid-json-object]").hide();

    if (app.data.RlsCode) {
        if (!app.data.isLive) {
            $("#data-dataset-map-accordion").find("[name=auto-update]").bootstrapToggle('off');
            $("#data-dataset-map-accordion").find("[name=auto-update]").bootstrapToggle('disable');
        }
    }
    $("#data-dataset-map-accordion-collapse-widget [name=auto-update], #data-dataset-map-accordion-collapse-widget [name=include-title], #data-dataset-map-accordion-collapse-widget [name=include-borders], #data-dataset-map-accordion-collapse-widget [name=include-copyright], #data-dataset-map-accordion-collapse-widget [name=include-link]").once("change", function () {
        app.data.dataset.map.renderSnippet();
    });



    new ClipboardJS("#data-dataset-map-accordion-collapse-widget [name=copy-snippet-code]");
    $('[data-toggle="tooltip"]').tooltip();

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});