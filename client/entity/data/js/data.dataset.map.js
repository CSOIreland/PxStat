/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.data.dataset.map.getModes();
    app.data.dataset.map.drawMapToDisplay();
    app.data.dataset.map.getColourSchemes();
    if (app.data.isLive) {
        $("#data-dataset-map-nav-content").find("[name=confidential-data-warning]").hide();
    }
    else {
        $("#data-dataset-map-nav-content").find("[name=confidential-data-warning]").show();
    }

    if (!app.config.plugin.subscriber.enabled) {
        $("#data-dataset-map-nav-content").find("[name=save-query]").remove();
    };

    new ClipboardJS("#data-dataset-map-accordion-api [name=copy-api-info], #data-dataset-map-accordion-api [name=copy-api-object]");
    $('#data-dataset-map-accordion-collapse-widget [name=auto-update],#data-dataset-map-accordion-collapse-widget [name=fluid-time], #data-dataset-map-accordion-collapse-widget [name=link-to-wip], #data-dataset-map-accordion-collapse-widget [name=include-copyright], #data-dataset-map-accordion-collapse-widget [name=include-link], #data-dataset-map-accordion-collapse-widget [name=include-title], #data-dataset-map-accordion-collapse-widget [name=include-borders]').bootstrapToggle("destroy").bootstrapToggle({
        onlabel: app.label.static["true"],
        offlabel: app.label.static["false"],
        onstyle: "primary",
        offstyle: "warning text-dark",
        height: 37,
        width: C_APP_TOGGLE_LENGTH
    });

    $("#data-dataset-map-accordion-collapse-widget").find("[name=title-value]").val(app.data.dataset.metadata.jsonStat.label.trim());

    $('#data-dataset-map-accordion').on('shown.bs.collapse', function (e) {
        if (app.data.isModal) {
            $('#data-view-modal').animate({
                scrollTop: '+=' + $("#" + e.target.id).parent().getBoundingClientRect().top
            }, 1000);
        }
        else {
            $('html, body').animate({
                scrollTop: $("#" + e.target.id).parent().offset().top
            }, 1000);
        }
    });

    $("#data-dataset-map-nav-content").find("[name=download-chart]").once("click", function (e) {
        e.preventDefault();
        pxWidget.map.easyPrint.printMap('CurrentSize', app.data.fileNamePrefix + "_" + moment(Date.now()).format('DDMMYYYYHHmmss'))
    });

    $("#data-dataset-map-accordion [name=download-snippet]").once("click", function () {
        // Download the snippet file
        app.library.utility.download(app.data.fileNamePrefix + '.' + moment(Date.now()).format(app.config.mask.datetime.file), $("#data-dataset-map-accordion-snippet-code").text(), C_APP_EXTENSION_HTML, C_APP_MIMETYPE_HTML, false, true);
    });

    $("#data-dataset-map-accordion [name=preview-snippet]").once("click", function () {
        app.library.utility.previewHtml($("#data-dataset-map-accordion-snippet-code").text())
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
            $("#data-dataset-map-accordion").find("[name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-map-accordion").find("[name=fluid-time]").bootstrapToggle('disable');
        }
    };

    $("#data-dataset-map-accordion-collapse-widget [name=fluid-time], #data-dataset-map-accordion-collapse-widget [name=include-borders], #data-dataset-map-accordion-collapse-widget [name=include-copyright], #data-dataset-map-accordion-collapse-widget [name=include-link], #data-dataset-map-accordion-collapse-widget [name=colour-scale]").once("change", function () {
        app.data.dataset.map.renderSnippet();
    });

    $("#data-dataset-map-accordion-collapse-widget [name=include-title]").once("change", function () {
        app.data.dataset.map.renderSnippet();
        if ($(this).is(':checked')) {
            $("#data-dataset-map-accordion-collapse-widget").find("[name=title-value]").show();
        } else {
            $("#data-dataset-map-accordion-collapse-widget").find("[name=title-value]").hide();
        }
    });

    $("#data-dataset-map-accordion-collapse-widget").find('input[name="title-value"]').once('keyup', function () {
        if ($(this).val().trim().length) {
            window.clearTimeout($(this).data('timer'));
            $(this).data('timer', window.setTimeout(app.data.dataset.map.renderSnippet, 400));
        } else {
            app.data.dataset.map.renderSnippet(); //maybe clear input
            window.clearTimeout($(this).data('timer'));
        }

    });

    $("#data-dataset-map-accordion-collapse-widget [name=auto-update]").once("change", function () {
        app.data.dataset.map.renderSnippet();
        if (!$(this).is(':checked')) {
            $("#data-dataset-map-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-map-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('disable');
        } else {
            $("#data-dataset-map-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('enable');
        }
    });

    $("#data-dataset-map-accordion-collapse-widget [name=link-to-wip]").once("change", function () {
        app.data.dataset.map.renderSnippet();
        if ($(this).is(':checked')) {
            //disable download HTML button as this won't work with private api due to CORS rules
            $("#data-dataset-map-accordion").find("[name=download-snippet]").prop('disabled', true);
            $("#data-dataset-map-accordion").find("[name=preview-snippet]").prop('disabled', true);
        }
        else {
            //disable download HTML button as this won't work with private api due to CORS rules
            $("#data-dataset-map-accordion").find("[name=download-snippet]").prop('disabled', false);
            $("#data-dataset-map-accordion").find("[name=preview-snippet]").prop('disabled', false);
        }
    });

    $("#data-dataset-map-nav-content").find("[name=save-query]").once("click", function () {
        //check that we have a user to save the table against
        if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess || app.navigation.user.isSubscriberAccess) {
            app.data.dataset.map.saveQuery.drawSaveQueryModal();
        }
        else {
            $("#modal-subscriber-login").modal("show");
        }

    });

    $("#data-dataset-map-nav-content").find("[name=mode-select]").once("change", function () {
        app.data.dataset.map.buildMapConfig();
    });


    new ClipboardJS("#data-dataset-map-accordion-collapse-widget [name=copy-snippet-code]");
    $('[data-bs-toggle="tooltip"]').tooltip();

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
    app.library.html.parseDynamicPopover("#data-dataset-map-accordion-collapse-widget [label-popover-dynamic=link-to-wip]", "link-to-wip", [app.config.corsDomain]);

});