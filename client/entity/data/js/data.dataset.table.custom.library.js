/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
app.data = app.data || {};
app.data.dataset = app.data.dataset || {};
app.data.dataset.customTable = {};
app.data.dataset.customTable.ajax = {};
app.data.dataset.customTable.callback = {};
app.data.dataset.customTable.parsedData = null;
app.data.dataset.customTable.pivotedData = null;
app.data.dataset.customTable.datatable = null;
app.data.dataset.customTable.snippet = {};
app.data.dataset.customTable.snippet.template = {
    "autoupdate": true,
    "matrix": null,
    "fluidTime": [],
    "codes": false,
    "link": null,
    "copyright": null,
    "title": null,
    "defaultContent": app.config.entity.data.datatable.null,
    "internationalisation": {
        "unit": app.label.static["unit"],
        "value": app.label.static["value"]
    },
    "rowFields": [],
    "columnFields": [],
    "highlightRow": {
        "dimension": null,
        "variable": null
    },
    "data": {
        "api": {
            "query": {},
            "response": {}
        }
    },
    "metadata": {
        "api": {
            "query": {
                "url": null,
                "data": {
                    "jsonrpc": "2.0",
                    "method": null,
                    "params": {
                        "matrix": null,
                        "language": app.data.LngIsoCode,
                        "format": {
                            "type": C_APP_FORMAT_TYPE_DEFAULT,
                            "version": C_APP_FORMAT_VERSION_DEFAULT
                        }
                    },
                    "version": "2.0"
                }
            },
            "response": {}
        }
    },
    "options": $.extend(true, {}, app.config.plugin.datatable)//preserve default options for use elsewhere

};

/**
 * Create drag and drop area
 */
app.data.dataset.customTable.createPivotControls = function () {
    if (!$('#data-dataset-table-result-custom-available-fields [name="available-fields"]').text().trim().length
        && !$('#data-dataset-table-result-custom-available-fields [name="row-fields"]').text().trim().length
        && !$('#data-dataset-table-result-custom-available-fields [name="column-fields"]').text().trim().length) {
        var headers = $.extend(true, [], app.data.dataset.metadata.jsonStat.id);
        headers.splice(1, 0, app.label.static["unit"]);
        $.each(headers, function (index, value) {
            $('<div>', {
                class: app.data.dataset.customTable.isFieldRedundant(value) ? 'field-item btn btn-sm btn-outline-neutral text-dark' : 'field-item btn btn-sm btn-outline-secondary',
                "data-bs-title": app.data.dataset.customTable.isFieldRedundant(value) ? app.label.static["optional"] : app.label.static["required"],
                "data-bs-toggle": "tooltip",
                draggable: true,
                'data-id': value,
                text: app.data.dataset.metadata.jsonStat.Dimension(value) ? app.data.dataset.metadata.jsonStat.Dimension(value).label : value,
                'data-redundant': app.data.dataset.customTable.isFieldRedundant(value)
            }).appendTo($('#data-dataset-table-result-custom-available-fields [name="available-fields"]'));
        });



        // Event delegation using a parent element
        $('#data-dataset-table-result-custom-available-fields').on('dragstart', '.field-item', function (e) {
            e.originalEvent.dataTransfer.setData('text/plain', $(this).data('id'));
        });

        // Add event listeners for drag and drop
        $('.field-list').on('dragover', function (e) {
            e.preventDefault()
        }).on('drop', app.data.dataset.customTable.drop);
    }
    $('[data-bs-toggle="tooltip"]').tooltip();
};

/**
 * Handle drop event
 * @param {*} e 
 */
app.data.dataset.customTable.drop = function (e) {
    e.preventDefault();
    $("#pxwidget-custom-table").empty();
    $("#data-dataset-table-accordion-custom-widget").hide();
    const fieldIndex = e.originalEvent.dataTransfer.getData('text');
    const $fieldItem = $(`.field-item[data-id="${fieldIndex}"]`);
    const $targetList = $(e.target).closest('.card-body');
    $targetList.append($fieldItem);
};

/**
 * Handle pivot click, check for errors in selection
 */
app.data.dataset.customTable.pivotClick = function () {
    //check fields left in the available fields
    var remainingFields = $('#data-dataset-table-result-custom-available-fields [name="available-fields"]').children().map(function () {
        if (!$(this).data('redundant')) {
            return $(this).text();
        }
    }).get();

    if (remainingFields.length) {//one or more remaining fields has more than one variable, warn user
        var remainingFieldsList = $("<ul>", {
            "class": "list-group mb-2"
        });
        $.each(remainingFields, function (index, value) {
            remainingFieldsList.append(
                $("<li>", {
                    "class": "list-group-item",
                    "text": value
                })
            )
        });
        api.modal.information(
            "<p>" + app.label.static["available-fields-not-included-error"] + "</p>" + remainingFieldsList.get(0).outerHTML + "<p>" + app.label.static["table-invalid"] + "</p>");
    }
    else {
        app.data.dataset.customTable.initTable()
    }


};

/**
 * Build snippet and call widget
 * @returns 
 */
app.data.dataset.customTable.initTable = function () {
    // Get selected row and column fields
    app.data.dataset.customTable.snippet.template.rowFields = $('#data-dataset-table-result-custom-available-fields [name="row-fields"]').children().map(function () {
        //return $(this).text();
        return $(this).data('id')
    }).get();

    app.data.dataset.customTable.snippet.template.columnFields = $('#data-dataset-table-result-custom-available-fields [name="column-fields"]').children().map(function () {
        //return $(this).text();
        return $(this).data('id')
    }).get();
    // Check if both row and column fields are selected
    if (app.data.dataset.customTable.snippet.template.rowFields.length === 0 || app.data.dataset.customTable.snippet.template.columnFields.length === 0) {
        api.modal.error("Please select at least one row field and one column field")
        return;
    }
    var widgetConfig = $.extend(true, {}, app.data.dataset.customTable.snippet.template);

    widgetConfig.options.language = app.label.plugin.datatable;
    widgetConfig.options.iDisplayLength = app.config.entity.data.datatable.length;
    widgetConfig.options.responsive = false;
    widgetConfig.options.order = [];
    widgetConfig.options.scrollX = true;

    //add query to config
    var JsonQuery = {
        "jsonrpc": C_APP_API_JSONRPC_VERSION,
        "method": null,
        "params": $.extend(true, {}, app.data.dataset.table.apiParamsData)
    };

    JsonQuery.params.extension.codes = true;
    JsonQuery.params.extension.pivot = null;

    if (app.data.isLive) {
        widgetConfig.data.api.query.url = app.config.url.api.jsonrpc.public;
        JsonQuery.method = "PxStat.Data.Cube_API.ReadDataset";
        widgetConfig.matrix = app.data.MtrCode;
        widgetConfig.metadata.api.query.data.params.matrix = app.data.MtrCode;
        widgetConfig.metadata.api.query.url = app.config.url.api.jsonrpc.public;
        widgetConfig.metadata.api.query.data.method = "PxStat.Data.Cube_API.ReadMetadata";
    }
    else {
        widgetConfig.data.api.query.url = app.config.url.api.jsonrpc.private;
        JsonQuery.method = "PxStat.Data.Cube_API.ReadPreDataset";
        widgetConfig.release = app.data.RlsCode;
        delete widgetConfig.matrix
        widgetConfig.metadata.api.query.data.params.release = app.data.RlsCode;
        delete widgetConfig.metadata.api.query.data.params.matrix
        widgetConfig.metadata.api.query.url = app.config.url.api.jsonrpc.private;
        widgetConfig.metadata.api.query.data.method = "PxStat.Data.Cube_API.ReadPreMetadata";
    }

    //from pxStat we have the response already at this point so pass the response for efficiency 
    widgetConfig.data.api.response = app.data.dataset.table.response;

    widgetConfig.data.api.query.data = JsonQuery;
    pxWidget.draw.init(C_APP_PXWIDGET_TYPE_TABLE_V2, "pxwidget-custom-table", widgetConfig, function () {
        //show the accordion
        $("#data-dataset-table-accordion-custom-widget").show();
        // Render the Snippet
        app.data.dataset.customTable.snippetConfig = {};
        $.extend(true, app.data.dataset.customTable.snippetConfig, pxWidget.draw.params["pxwidget-custom-table"]);
        app.data.dataset.customTable.populateHighlightRowDimension();
        app.data.dataset.customTable.drawSnippetCode();
    });

    if (app.data.isModal) {
        $('#data-view-modal').animate({
            scrollTop: '+=' + $('#data-dataset-table-result-custom-pivoted-table')[0].getBoundingClientRect().top
        },
            1000);
    }
    else {
        $('html, body').animate({
            scrollTop: $("#data-dataset-table-result-custom-pivoted-table").offset().top
        }, 1000);
    }

};

/**
 * render snippet code
 */
app.data.dataset.customTable.drawSnippetCode = function () {
    if ($("#data-dataset-table-custom-accordion-collapse-widget [name=link-to-wip]").is(':checked')) {
        //disable download HTML button as this won't work with private api due to CORS rules
        $("#data-dataset-table-custom-accordion-collapse-widget").find("[name=download-snippet]").prop('disabled', true);
        $("#data-dataset-table-custom-accordion-collapse-widget").find("[name=preview-snippet]").prop('disabled', true);
    }
    else {
        $("#data-dataset-table-custom-accordion-collapse-widget").find("[name=download-snippet]").prop('disabled', false);
        $("#data-dataset-table-custom-accordion-collapse-widget").find("[name=preview-snippet]").prop('disabled', false);
    }
    var snippet = app.config.entity.data.snippet;
    var config = $.extend(true, {}, app.data.dataset.customTable.snippetConfig);


    if ($("#data-dataset-table-accordion-custom-widget").find("[name=link-to-wip]").is(':checked')) {
        config.matrix = app.data.MtrCode;
        config.data.api.response = {};
        config.metadata.api.query = {};

        config.data.api.query.url = app.config.url.api.jsonrpc.private;

        config.data.api.response = {};

        //if link to WIP, then make query explicit so it is always a point in time query with no additional variables added at a later date because they selected all variables in initial query

        var linkToWipQuery = $.extend(true, {}, config.data.api.query.data);
        linkToWipQuery.params.id = [];
        linkToWipQuery.params.dimension = {};

        $("#data-dataset-table-nav-content").find("[name=dimension-containers]").find("select").each(function (index) {
            if ($(this).val().length) {
                linkToWipQuery.params.id.push($(this).attr("idn"));
                linkToWipQuery.params.dimension[$(this).attr("idn")] = {
                    "category": {
                        "index": $(this).val()
                    }
                };
            };
        });
        config.data.api.query.data = linkToWipQuery;
    }
    else {
        //auto update
        config.autoupdate = $("#data-dataset-table-custom-accordion-collapse-widget").find("[name=auto-update]").is(':checked');
        if ($("#data-dataset-table-custom-accordion-collapse-widget").find("[name=auto-update]").is(':checked')) {
            config.metadata.api.response = {};
            config.data.api.response = {};
        } else {
            config.metadata.api.query = {};
            config.data.api.query = {};
        }
    }

    config.options.dom = "Bfltip";
    if (!$("#data-dataset-table-accordion-custom-widget").find("[name=include-pagination]").is(':checked')) {
        config.options.paging = false;
        config.options.dom = "Bft";
    }

    if (!$("#data-dataset-table-accordion-custom-widget").find("[name=include-buttons]").is(':checked')) {
        config.options.dom = config.options.dom.replace('B', '');
    }

    if (!$("#data-dataset-table-accordion-custom-widget").find("[name=include-search]").is(':checked')) {
        config.options.dom = config.options.dom.replace('f', '');
    }

    config.highlightRow.dimension = $('#data-dataset-table-accordion-custom-widget [name="highlighted-dimension-code"]').val() || null;
    config.highlightRow.variable = $('#data-dataset-table-accordion-custom-widget [name="highlighted-variable-code"]').val() || [];

    delete config.options.lengthMenu;
    delete config.options.aLengthMenu;
    delete config.options.iDisplayLength;

    config.copyright = $("#data-dataset-table-accordion-custom-widget").find("[name=include-copyright]").is(':checked');
    config.title = $("#data-dataset-table-accordion-custom-widget").find("[name=include-title]").is(':checked') ? $("#data-dataset-table-accordion-custom-widget").find("[name=title-value]").val().trim() : null;
    config.link = $("#data-dataset-table-accordion-custom-widget").find("[name=include-link]").is(':checked') ? app.config.url.application + C_COOKIE_LINK_TABLE + "/" + app.data.MtrCode : null;

    if ($("#data-dataset-table-accordion-custom-widget").find("[name=fluid-time]").is(':checked')) {
        config = app.data.dataset.table.callback.setFluidTime(config);
        config.metadata.api.query.data.params.matrix = app.data.MtrCode;
        config.metadata.api.query.url = app.config.url.api.jsonrpc.public;
    }
    else {
        config.metadata.api.query = {};
        config.fluidTime = [];
    }

    //add custom config if something there
    if ($("#data-dataset-table-accordion-custom-widget [name=custom-config]").val().trim().length) {
        try {
            app.data.dataset.customTable.formatJson();
            var customOptions = JSON.parse($("#data-dataset-table-accordion-custom-widget [name=custom-config]").val().trim());
            $.extend(true, config, customOptions);
            $("#data-dataset-table-accordion-custom-widget [name=invalid-json-object]").hide();
            $("#data-dataset-table-accordion-custom-widget [name=valid-json-object]").show();
        } catch (err) {
            $("#data-dataset-table-accordion-custom-widget").collapse('show');
            $("#data-dataset-table-accordion-custom-widget [name=invalid-json-object]").show();
            $("#data-dataset-table-accordion-custom-widget [name=valid-json-object]").hide();
        }
    }

    snippet = snippet.sprintf([C_APP_URL_PXWIDGET_ISOGRAM, C_APP_PXWIDGET_TYPE_TABLE_V2, app.library.utility.randomGenerator('pxwidget'), JSON.stringify(config)]);
    $("#data-dataset-custom-table-accordion-snippet-code").hide().text(snippet.trim()).fadeIn();
    Prism.highlightAll();
};

/**
 * Render highlight row dropdown
 */
app.data.dataset.customTable.populateHighlightRowDimension = function () {
    $('#data-dataset-table-accordion-custom-widget [name="highlighted-variable-code"]').empty()
    $('#data-dataset-table-accordion-custom-widget [name="highlighted-dimension-code"]').empty().append($("<option>", {
        "text": app.label.static["select-uppercase"],
        "disabled": "disabled",
        "selected": "selected"
    }))
    $.each(app.data.dataset.table.jsonStat.id, function (index, value) {
        if (app.data.dataset.customTable.snippet.template.rowFields.indexOf(value) >= 0) {
            $('#data-dataset-table-accordion-custom-widget [name="highlighted-dimension-code"]').append($("<option>", {
                "value": value,
                "text": app.data.dataset.metadata.jsonStat.Dimension(value).label
            }));
        }
    });
};

/**
 * Render highlight row dropdown
 */
app.data.dataset.customTable.populateHighlightRowVariable = function () {
    //get variables from selected dimension from the JSON-stat results
    $('#data-dataset-table-accordion-custom-widget [name="highlighted-variable-code"]').empty().append($("<option>", {
        "text": app.label.static["select-uppercase"],
        "disabled": "disabled"
    }));
    $.each(app.data.dataset.table.jsonStat.Dimension($('#data-dataset-table-accordion-custom-widget [name="highlighted-dimension-code"]').val()).id, function (index, value) {
        $('#data-dataset-table-accordion-custom-widget [name="highlighted-variable-code"]').append($("<option>", {
            "text": app.data.dataset.table.jsonStat.Dimension($('#data-dataset-table-accordion-custom-widget [name="highlighted-dimension-code"]').val()).Category(value).label,
            "value": value
        })).select2({
            width: '100%',
            dropdownParent: app.data.isModal ? $('#data-dataset-table-accordion-custom-widget') : null,
            placeholder: app.label.static["start-typing"]
        });;
    });

    $('#data-dataset-table-accordion-custom-widget [name="highlighted-variable-code"]').show();

};

/**
 * Check if a dimension is redundant
 * @param {*} property 
 * @returns 
 */
app.data.dataset.customTable.isFieldRedundant = function (property) {
    if (property == app.label.static["unit"]) {
        var units = [];

        //find the select for the dimension id and get number of variables selected
        var lengthSelected = $('#data-dataset-table-nav-content [name="dimension-containers"]').find('[name="dimension-select"][idn="STATISTIC"]').find('option:selected').length;
        if (lengthSelected) { //loop through selected options
            $('#data-dataset-table-nav-content [name="dimension-containers"]').find('[name="dimension-select"][idn="STATISTIC"]').find('option:selected').each(function (index) {
                units.push(app.data.dataset.metadata.jsonStat.Dimension({ role: "metric" })[0].Category($(this).val()).unit.label)
            });
        }
        else {//loop through all options
            $('#data-dataset-table-nav-content [name="dimension-containers"]').find('[name="dimension-select"][idn="STATISTIC"]').find('option').each(function (index) {
                units.push(app.data.dataset.metadata.jsonStat.Dimension({ role: "metric" })[0].Category($(this).val()).unit.label)
            });
        }
        //get unique units to see if we have more than one
        var unique = units.filter(function (itm, i, a) {
            return i == units.indexOf(itm);
        });
        if (unique.length == 1) {
            return true
        }
        else {
            return false
        }

    }
    else {
        //find the select for the dimension id and get number of variables selected
        var length = $('#data-dataset-table-nav-content [name="dimension-containers"]').find('[name="dimension-select"][idn="' + property + '"]').find('option').length;
        var lengthSelected = $('#data-dataset-table-nav-content [name="dimension-containers"]').find('[name="dimension-select"][idn="' + property + '"]').find('option:selected').length;
        if (lengthSelected == 1) {
            return true
        }
        else if (lengthSelected == 0 && length == 1) {
            return true
        }
        else if (lengthSelected > 0) {
            return false
        }

        else if (lengthSelected == 0 && length > 0) {
            return false
        }

    }
}

/**
 * Format custom JSON
 */
app.data.dataset.customTable.formatJson = function () {
    $("#data-dataset-table-accordion-custom-widget [name=invalid-json-object]").hide();
    if ($("#data-dataset-table-accordion-custom-widget [name=custom-config]").val().trim().length) {
        var ugly = $("#data-dataset-table-accordion-custom-widget [name=custom-config]").val().trim();
        var obj = null;
        var pretty = null;
        try {
            obj = JSON.parse(ugly);
            pretty = JSON.stringify(obj, undefined, 4);
            $("#data-dataset-table-accordion-custom-widget [name=custom-config]").val(pretty);
        } catch (err) {
            $("#data-dataset-table-accordion-custom-widget [name=invalid-json-object]").show();
        }
    }
};