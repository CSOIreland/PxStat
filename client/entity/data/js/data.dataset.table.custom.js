/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    $("#data-dataset-table-accordion-custom-widget").find("[name=title-value]").val(app.data.dataset.metadata.jsonStat.label.trim());

    $("#data-dataset-table-accordion-custom-widget [name=include-title]").once("change", function () {
        app.data.dataset.customTable.drawSnippetCode();
        if ($(this).is(':checked')) {
            $("#data-dataset-table-accordion-custom-widget").find("[name=title-value]").show();
        } else {
            $("#data-dataset-table-accordion-custom-widget").find("[name=title-value]").hide();
        }

    });

    $('#data-dataset-table-accordion-custom-widget input[name="title-value"]').once('keyup', function () {
        if ($(this).val().trim().length) {
            window.clearTimeout($(this).data('timer'));
            $(this).data('timer', window.setTimeout(function () {
                app.data.dataset.customTable.drawSnippetCode()
            }, 400));
        } else {
            app.data.dataset.customTable.drawSnippetCode(); //maybe clear input
            window.clearTimeout($(this).data('timer'));
        }
    });


    $('#data-dataset-table-accordion-custom-widget [name=auto-update], #data-dataset-table-accordion-custom-widget [name=fluid-time], #data-dataset-table-accordion-custom-widget [name=link-to-wip], #data-dataset-table-accordion-custom-widget [name=include-copyright], #data-dataset-table-accordion-custom-widget [name=include-link], #data-dataset-table-accordion-custom-widget [name=include-title], #data-dataset-table-accordion-custom-widget [name=include-pagination], #data-dataset-table-accordion-custom-widget [name=include-buttons], #data-dataset-table-accordion-custom-widget [name=include-search]').bootstrapToggle("destroy").bootstrapToggle({
        onlabel: app.label.static["true"],
        offlabel: app.label.static["false"],
        onstyle: "success text-light",
        offstyle: "warning text-dark",
        height: 38,
        style: "text-light",
        width: C_APP_TOGGLE_LENGTH
    })

    $('#data-dataset-table-accordion-custom-widget [name=auto-update]').once("change", function () {
        if (!$(this).is(':checked')) {
            $("#data-dataset-table-accordion-custom-widget [name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-table-accordion-custom-widget [name=fluid-time]").bootstrapToggle('disable');
        }
        else {
            $("#data-dataset-table-accordion-custom-widget [name=fluid-time]").bootstrapToggle('enable');
        }
        //only render the snippet if the table is already drawn
        if ($('#pxwidget-custom-table').text().trim().length) {
            app.data.dataset.customTable.drawSnippetCode()
        }
    });

    $('#data-dataset-table-accordion-custom-widget [name=fluid-time]').once("change", function () {
        //only render the snippet if the table is already drawn
        if ($('#pxwidget-custom-table').text().trim().length) {
            app.data.dataset.customTable.drawSnippetCode()
        }
    });

    $('#data-dataset-table-accordion-custom-widget [name=include-copyright],'
        + ' #data-dataset-table-accordion-custom-widget [name=link-to-wip],'
        + ' #data-dataset-table-accordion-custom-widget [name=include-link],'
        + '#data-dataset-table-accordion-custom-widget [name=include-pagination], '
        + '#data-dataset-table-accordion-custom-widget [name=include-buttons], '
        + '#data-dataset-table-accordion-custom-widget [name=include-search]').once("change", function () {
            app.data.dataset.customTable.drawSnippetCode()
        })

    if (app.data.RlsCode) {
        if (!app.data.isLive) { //is WIP
            $("#data-dataset-table-accordion-custom-widget [name=auto-update]").bootstrapToggle('off');
            $("#data-dataset-table-accordion-custom-widget [name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-table-accordion-custom-widget [name=auto-update]").bootstrapToggle('disable');
            $("#data-dataset-table-accordion-custom-widget [name=fluid-time]").bootstrapToggle('disable');
        }
    }

    $("#data-dataset-table-accordion-custom-widget [name=custom-config]").val(
        function () {
            var ugly = JSON.stringify({ "options": {} });
            var obj = JSON.parse(ugly)
            return JSON.stringify(obj, undefined, 4);
        }
    );

    $("#data-dataset-table-accordion-custom-widget [name=add-custom-configuration]").once("click", function () {
        $("#data-dataset-table-accordion-custom-widget [name=invalid-json-object]").hide();
        $("#data-dataset-table-accordion-custom-widget [name=valid-json-object]").hide();
        app.data.dataset.customTable.drawSnippetCode();
    });

    if (app.data.RlsCode) {
        if (!app.data.isLive) {
            $("#data-dataset-table-accordion-custom-widget").find("[name=link-to-wip-wrapper]").show();
        }
    }



    new ClipboardJS("#data-dataset-table-accordion-custom-widget [name=copy-snippet-code]");

    $("#data-dataset-table-accordion-custom-widget [name=download-snippet]").once("click", function () {
        // Download the snippet file
        app.library.utility.download(app.data.fileNamePrefix + '.' + moment(Date.now()).format(app.config.mask.datetime.file), $("#data-dataset-custom-table-accordion-snippet-code").text(), C_APP_EXTENSION_HTML, C_APP_MIMETYPE_HTML, false, true);
    });

    $("#data-dataset-table-accordion-custom-widget [name=preview-snippet]").once("click", function () {
        app.library.utility.previewHtml($("#data-dataset-custom-table-accordion-snippet-code").text())
    });

    $('#data-dataset-table-accordion-custom-widget [name="highlighted-dimension-code"]').once("change", app.data.dataset.customTable.populateHighlightRowVariable);
    $('#data-dataset-table-accordion-custom-widget [name="highlighted-variable-code"]').once("change", app.data.dataset.customTable.drawSnippetCode);


    // Handle click on 'Apply Pivot' button
    $('#data-dataset-table-result-custom-available-fields [name="apply-pivot"]').once('click', app.data.dataset.customTable.pivotClick);
    app.library.html.parseDynamicPopover("#data-dataset-table-accordion-custom-widget [label-popover-dynamic=link-to-wip]", "link-to-wip", [app.config.corsDomain]);

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});