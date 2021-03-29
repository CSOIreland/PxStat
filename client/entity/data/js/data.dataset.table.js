/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.data.dataset.table.drawDimensions();
    app.data.dataset.table.drawFormat();
    app.data.dataset.table.drawPivotDropdown();
    if (app.data.isLive) {
        $("#data-dataset-table-nav-content").find("[name=confidential-data-warning]").hide();
    }
    else {
        $("#data-dataset-table-nav-content").find("[name=confidential-data-warning]").show();
    }
    //show codes
    $('#data-dataset-table-code-toggle').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["false"],
        off: app.label.static["true"],
        onstyle: "tertiary",
        offstyle: "neutral",
        width: C_APP_TOGGLE_LENGTH
    });

    $('#data-dataset-table-accordion-collapse-widget [name=auto-update], #data-dataset-table-accordion-collapse-widget [name=include-copyright], #data-dataset-table-accordion-collapse-widget [name=include-link], #data-dataset-table-accordion-collapse-widget [name=include-title], #data-dataset-table-accordion-collapse-widget [name=include-pagination], #data-dataset-table-accordion-collapse-widget [name=include-responsive]').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "primary",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH
    });

    $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val(JSON.stringify({ "options": {} }));
    app.data.dataset.table.callback.formatJson();
    $("#data-dataset-table-accordion-collapse-widget [name=valid-json-object]").hide();

    //reset
    $("#data-dataset-table-nav-content").find("[name=reset]").once("click", function () {
        $("#data-dataset-table-nav-content select[name=pivot]").val($("#data-dataset-table-nav-content select[name=pivot] option:first").val());
        app.data.dataset.table.drawDimensions();
    });

    //show data
    $("#data-dataset-table-nav-content").find("[name=show-data]").once("click", function () {
        if (app.data.dataset.table.selectionCount >= app.config.entity.data.threshold.hard) {
            app.data.dataset.table.confirmHardThreshold(app.library.html.parseDynamicLabel("error-read-exceeded", [app.library.utility.formatNumber(app.data.dataset.table.selectionCount), app.config.entity.data.threshold.hard]));
        }
        else if (app.data.dataset.table.selectionCount >= app.config.entity.data.threshold.soft) {
            app.data.dataset.table.confirmSoftThreshold(app.library.html.parseDynamicLabel("confirm-read", [app.library.utility.formatNumber(app.data.dataset.table.selectionCount)]), app.data.dataset.table.ajax.data);
        }
        else {
            //AJAX call get Data Set
            app.data.dataset.table.ajax.data();
        }
    });

    $('#data-dataset-table-accordion').on('show.bs.collapse', function (e) {
        $("#" + e.target.id).parent().find(".card-header i[name=accordion-icon]").removeClass().addClass("fas fa-minus-circle");
    });

    $('#data-dataset-table-accordion').on('hide.bs.collapse', function (e) {
        $("#" + e.target.id).parent().find(".card-header i[name=accordion-icon]").removeClass().addClass("fas fa-plus-circle");
    });

    $('#data-dataset-table-accordion').on('shown.bs.collapse', function (e) {
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

    $('[data-toggle="tooltip"]').tooltip();
    new ClipboardJS("#data-dataset-table-accordion [name=copy-api-info], #data-dataset-table-accordion [name=copy-api-object], #data-dataset-table-accordion [name=copy-snippet-code]");
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});