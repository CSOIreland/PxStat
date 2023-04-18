/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.data.dataset.chart.getChartTypes();
    app.data.dataset.chart.setLegendPosition();
    if (app.data.isLive) {
        $("#data-dataset-chart-nav-content").find("[name=confidential-data-warning]").hide();
    }
    else {
        $("#data-dataset-chart-nav-content").find("[name=confidential-data-warning]").show();
    }
    $("#data-dataset-chart-accordion-series-collapse").find("[name=add-series]").once("click", function () {
        app.data.dataset.chart.addSeries();
        if ($("#data-dataset-chart-accordion-series-collapse [name=tab-content]").length > 1) {
            $("#data-dataset-chart-accordion-options-collapse").find("[name=sort]").bootstrapToggle('off').bootstrapToggle('disable');
        }
        else {
            $("#data-dataset-chart-accordion-options-collapse").find("[name=sort]").bootstrapToggle('off').bootstrapToggle('enable');
        }
    });

    if (!app.config.plugin.subscriber.enabled) {
        $("#data-dataset-chart-render").find("[name=save-query]").remove();
    };

    //add series if none already
    $('#data-dataset-chart-accordion-series-collapse').on('shown.bs.collapse', function () {
        if (!$("#data-dataset-chart-accordion-series-collapse [name=series-tabs] li").length) {
            $("#data-dataset-chart-accordion-series-collapse").find("[name=add-series]").trigger("click");
        }
        //check for pyramid and number of series before enabling

        if ($("#data-dataset-chart-properties").find("[name=type]").val() == "pyramid"
            && $("#data-dataset-chart-accordion-series-collapse [name=series-tabs] li").length != 2) {
            $("#data-dataset-chart-accordion [name=view-chart]").prop("disabled", true);
        }
        else {
            $("#data-dataset-chart-accordion [name=view-chart]").prop("disabled", false);
        }
    });

    //format advanced options
    $("#data-dataset-chart-snippet-code [name=format-json]").once("click", app.data.dataset.chart.formatJson);

    $("#data-dataset-chart-snippet-code [name=download-snippet]").once("click", function () {
        // Download the snippet file
        app.library.utility.download(app.data.fileNamePrefix + '.' + moment(Date.now()).format(app.config.mask.datetime.file), $("#data-pxwidget-snippet-chart-code").text(), C_APP_EXTENSION_HTML, C_APP_MIMETYPE_HTML, false, true);
    });

    $("#data-dataset-chart-snippet-code [name=custom-config]").val(JSON.stringify({ "options": {} }));
    app.data.dataset.chart.formatJson();
    $("#data-dataset-chart-snippet-code [name=valid-json-object]").hide();

    //Changing plus to minus
    $("#data-dataset-chart-accordion").on('shown.bs.collapse', function (e) {
        $("#" + e.target.id).parent().find("[name=accordion-icon]").removeClass().addClass("fas fa-minus-circle");
        //scroll to top of content
        if (app.data.isModal) {
            $('#data-view-modal').animate({
                scrollTop: '+=' + $("#" + e.target.id).parent()[0].getBoundingClientRect().top
            },
                1000);
        }
        else {
            $('html, body').animate({
                scrollTop: $("#" + e.target.id).parent().offset().top
            }, 1000);
        }
    });
    //Changing minus to plus
    $("#data-dataset-chart-accordion").on('hidden.bs.collapse', function (e) {
        $("#" + e.target.id).parent().find("[name=accordion-icon]").removeClass().addClass("fas fa-plus-circle");
    });

    $("#data-dataset-chart-accordion-series-collapse [name=dual-axis], #data-dataset-chart-accordion-options-collapse [name=stacked], #data-dataset-chart-accordion-options-collapse [name=stacked-percent], #data-dataset-chart-snippet-code [name=auto-update],#data-dataset-chart-snippet-code [name=fluid-time], #data-dataset-chart-snippet-code [name=link-to-wip], #data-dataset-chart-snippet-code [name=include-title], #data-dataset-chart-snippet-code [name=include-copyright], #data-dataset-chart-snippet-code [name=include-link], #data-dataset-chart-accordion-options-collapse [name=auto-scale],#data-dataset-chart-accordion-options-collapse [name=curved-line], #data-dataset-chart-accordion-options-collapse [name=sort]").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "primary",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH
    });

    $("#data-dataset-chart-accordion-series-collapse [name=dual-axis]").once("change", app.data.dataset.chart.dualAxis);


    $("#data-dataset-chart-accordion-options-collapse [name=stacked]").once("change", app.data.dataset.chart.stacked);
    $("#data-dataset-chart-accordion-options-collapse [name=stacked-percent]").once("change", app.data.dataset.chart.resetChart);


    if (app.data.RlsCode) {
        if (!app.data.isLive) {
            $("#data-dataset-chart-snippet-code").find("[name=auto-update]").bootstrapToggle('off');
            $("#data-dataset-chart-snippet-code").find("[name=auto-update]").bootstrapToggle('disable');
            $("#data-dataset-chart-snippet-code").find("[name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-chart-snippet-code").find("[name=fluid-time]").bootstrapToggle('disable');
        }
    }

    $("#data-dataset-chart-accordion").find("[name=view-chart]").once("click", function () {
        app.data.dataset.chart.buildChartConfig(true);
    });

    $("#data-dataset-chart-snippet-code").find("[name=title-value]").val(app.data.dataset.metadata.jsonStat.label.trim());

    $("#data-dataset-chart-snippet-code").find('input[name="title-value"]').once('keyup', function () {
        if ($(this).val().trim().length) {
            window.clearTimeout($(this).data('timer'));
            $(this).data('timer', window.setTimeout(app.data.dataset.chart.renderSnippet, 400));
        } else {
            app.data.dataset.chart.renderSnippet() //maybe clear input
            window.clearTimeout($(this).data('timer'));
        }
    });

    $("#data-dataset-chart-snippet-code [name=fluid-time], #data-dataset-chart-snippet-code [name=include-copyright], #data-dataset-chart-snippet-code [name=include-link]").once("change", function () {
        app.data.dataset.chart.renderSnippet();
    });

    $("#data-dataset-chart-snippet-code [name=link-to-wip]").once("change", function () {
        app.data.dataset.chart.renderSnippet();
    });


    $("#data-dataset-chart-snippet-code [name=include-title]").once("change", function () {
        app.data.dataset.chart.renderSnippet();
        if ($(this).is(':checked')) {
            $("#data-dataset-chart-snippet-code").find('input[name="title-value"]').show();
        } else {
            $("#data-dataset-chart-snippet-code").find('input[name="title-value"]').hide();
        }
    });

    $("#data-dataset-chart-snippet-code [name=auto-update]").once("change", function () {
        app.data.dataset.chart.renderSnippet();
        if (!$(this).is(':checked')) {
            $("#data-dataset-chart-snippet-code [name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-chart-snippet-code [name=fluid-time]").bootstrapToggle('disable');
        } else {
            $("#data-dataset-chart-snippet-code [name=fluid-time]").bootstrapToggle('enable');
        }
    });


    $("#data-dataset-chart-accordion-options-collapse [name=auto-scale], #data-dataset-chart-accordion-options-collapse [name=curved-line], #data-dataset-chart-accordion-options-collapse [name=sort]").once("change", app.data.dataset.chart.resetChart);


    $("#data-dataset-chart-render").find("[name=download-chart]").once("click", function (e) {
        e.preventDefault();
        var url_base64jp = $("#pxwidget-chart canvas")[0].toDataURL();
        var filename = (app.data.dataset.metadata.jsonStat.label.trim().replace(/ /g, "_") + "." + new Date().getTime()).toLocaleLowerCase();
        app.library.utility.download(filename, url_base64jp, "png", "image/png", true);
    });

    //Confirm reset and reset details
    $("#data-dataset-chart-properties [name=reset], #data-dataset-chart-accordion [name=reset]").once("click", function () {
        api.modal.confirm(
            app.label.static["reset-page"],
            app.data.dataset.chart.resetAll
        );
    });

    $("#data-dataset-chart-render").find("[name=save-query]").once("click", function () {
        //check that we have a user to save the table against
        if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess || app.navigation.user.isSubscriberAccess) {
            app.data.dataset.chart.saveQuery.drawSaveQueryModal();
        }
        else {
            $("#modal-subscriber-login").modal("show");
        }

    });

    new ClipboardJS("#data-dataset-chart-snippet-code [name=copy-snippet-code]");
    $('[data-toggle="tooltip"]').tooltip();

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
    app.library.html.parseDynamicPopover("#data-dataset-chart-snippet-code [label-popover-dynamic=link-to-wip]", "link-to-wip", [app.config.corsdDomain]);
});
