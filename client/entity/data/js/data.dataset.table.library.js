/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
app.data = app.data || {};
app.data.dataset = app.data.dataset || {};
app.data.dataset.table = {};
app.data.dataset.table.response = null;
app.data.dataset.table.jsonStat = null;
app.data.dataset.table.ajax = {};
app.data.dataset.table.callback = {};
app.data.dataset.table.selectionCount = null;
app.data.dataset.table.totalCount = null;
app.data.dataset.table.apiParamsData = {}
app.data.dataset.table.pivot = {};
app.data.dataset.table.pivot.dimensionCode = null;
app.data.dataset.table.pivot.isMetric = false;
app.data.dataset.table.pivot.variableCodes = [];
app.data.dataset.table.snippet = {};
app.data.dataset.table.snippet.configuration = {};
app.data.dataset.table.snippet.template = {
    "autoupdate": true,
    "copyright": true,
    "title": null,
    "link": null,
    "pivot": null,
    "internationalisation": {
        "unit": app.label.static["unit"],
        "value": app.label.static["value"]
    },
    "defaultContent": app.config.entity.data.datatable.null,
    "data": {
        "api": {
            "query": {},
            "response": {}
        }
    },
    "options": {
        "language": app.label.plugin.datatable,
        "dom": "Bfltip",
        "buttons": [
            {
                "extend": 'csv',
                "text": app.label.static["download"] + " CSV",
                "className": "export-button",
                "title": app.data.MtrCode + "." + moment(Date.now()).format(app.config.mask.datetime.file)
            },
            {
                "extend": 'print',
                "text": app.label.static["print"],
                "className": "export-button"
            }
        ]
    }
};

//#endregion
app.data.dataset.table.drawDimensions = function () {
    //reset snippet code
    $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").empty();
    $("#data-dataset-table-accordion-snippet-code").empty();
    $("#data-dataset-table-accordion-collapse-widget").find("[name=make-selection-message]").show();
    $("#data-dataset-table-accordion-collapse-widget [name=include-copyright], #data-dataset-table-accordion-collapse-widget [name=include-link], #data-dataset-table-accordion-collapse-widget [name=include-title]").bootstrapToggle('disable');


    //check for WIP
    if (app.data.RlsCode) {
        if (!app.data.isLive) { //is WIP
            $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('off');
            $("#data-dataset-table-accordion-collapse-widget [name=wip-widget-warning]").show();
        }
        $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('disable');
    }
    else {
        $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('disable');
    }



    $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").prop("disabled", true);
    $("#data-dataset-table-accordion-collapse-widget [name=add-custom-configuration]").prop("disabled", true);
    $("#data-dataset-table-accordion-collapse-widget").collapse('hide');

    $("#data-dataset-table-code-toggle").bootstrapToggle('on');
    app.data.dataset.table.totalCount = 1;
    $("#data-dataset-table-nav-content").find("[name=dimension-containers]").empty();
    var dimensions = app.data.dataset.metadata.jsonStat.Dimension();
    $.each(dimensions, function (index, value) {
        app.data.dataset.table.totalCount = app.data.dataset.table.selectionCount = app.data.dataset.table.totalCount * value.length;

        var dimensionContainer = $("#data-dataset-table-templates").find("[name=dimension-container]").clone();
        var dimensionCode = $("<small>", {
            "text": " - " + app.data.dataset.metadata.jsonStat.id[index],
            "name": "dimension-code",
            "class": "d-none"
        }).get(0).outerHTML;
        dimensionContainer.find("[name=dimension-label]").html(value.label + dimensionCode);
        dimensionContainer.find("[name=dimension-count]").text(value.length);
        dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]);
        dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]);
        dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]);

        $.each(value.id, function (variableIndex, variable) {
            var option = $('<option>', {
                "value": variable,
                "code-true": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "") + " (" + variable + ")",
                "code-false": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                "title": value.Category(variableIndex).label,
                "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
            });

            dimensionContainer.find("select").append(option);
        });
        dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("role", value.role);

        if (value.role == "time") {
            //reverse select based on codes so most recent time first
            dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
                return $(x).val() < $(y).val() ? 1 : -1;
            }));
        }

        $("#data-dataset-table-nav-content").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);

    });

    $("#data-dataset-table-nav-content").find("[name=data-count-cells]").text(app.library.utility.formatNumber(app.data.dataset.table.totalCount));
    $("#data-dataset-table-nav-content").find("[name=data-total-cells]").text(app.library.utility.formatNumber(app.data.dataset.table.totalCount));


    //search dimension
    $("#data-dataset-table-nav-content").find("[name=dimension-filter]").once("keyup search", function () {
        var dimensionCode = $(this).attr("idn");
        var thisDimension = app.data.dataset.metadata.jsonStat.Dimension(dimensionCode);

        var select = $("#data-dataset-table-nav-content").find("select[idn='" + dimensionCode + "']");
        var filter = $(this).val().toLowerCase();
        select.find('option').each(function () {
            if (!$(this).is(':selected')) {
                $(this).remove();
            }
        });
        var dimensionToSearch = app.data.dataset.variables[dimensionCode];

        $.each(dimensionToSearch, function (key, value) {

            if ((value.toLowerCase().indexOf(filter) > -1) || $(this).is(':selected')) {
                //variable is valid, append to select if not already selected
                if (!select.find("option[value='" + key + "]").is(':selected')) {
                    var textWithCode = thisDimension.Category(key).label + (thisDimension.Category(key).unit ? " (" + thisDimension.Category(key).unit.label + ")" : "") + " (" + key + ")";
                    var textWithoutCode = thisDimension.Category(key).label + (thisDimension.Category(key).unit ? " (" + thisDimension.Category(key).unit.label + ")" : "");
                    select.append($('<option>', {
                        "value": key,
                        "code-true": textWithCode,
                        "code-false": textWithoutCode,
                        "title": value,
                        "text": !$('#data-dataset-table-code-toggle').is(':checked') ? textWithCode : textWithoutCode
                    }));
                };
            }
        });

        if (select.find('option').length == 0) {
            select.append($('<option>', {
                "title": app.label.static["no-results"],
                "text": app.label.static["no-results"],
                "disabled": "disabled"
            }));
            //disable select all
            $("#data-dataset-table-nav-content").find("[name=select-all][idn='" + dimensionCode + "']").prop("disabled", true);
        }
        if (select.find('option:selected').length != select.find('option').length) {
            if ($("#data-dataset-table-nav-content").find("[name=select-all][idn='" + dimensionCode + "']").is(':checked')) {
                $("#data-dataset-table-nav-content").find("[name=select-all][idn='" + dimensionCode + "']").prop("checked", false);
            }

        }
        else {
            //enable select all
            $("#data-dataset-table-nav-content").find("[name=select-all][idn='" + dimensionCode + "']").prop("disabled", false);

        }

    });

    //select all
    $("#data-dataset-table-nav-content").find("[name=select-all]").once("change", function () {
        var selectedDimensionId = $(this).attr("idn");
        var select = $("#data-dataset-table-nav-content").find("select[idn='" + selectedDimensionId + "']");

        if (this.checked) {
            select.find('option').each(function () {
                $(this).prop("selected", true);
            });
        }
        else {
            select.find('option').each(function () {
                $(this).prop("selected", false);
            });
        }
        $("#data-dataset-table-result").hide();
        app.data.dataset.table.response = null;
        app.data.dataset.table.jsonStat = null;
        app.data.dataset.table.countSelection()
        app.data.dataset.table.buildApiParams();
    });

    //toggle select all depending on  number of options selected
    $("#data-dataset-table-nav-content").find("select[name=dimension-select]").once('change', function () {
        var selectedDimensionId = $(this).attr("idn");
        var selectedOptions = $(this).find("option:selected").length;
        var totalOptions = app.data.dataset.metadata.jsonStat.Dimension(selectedDimensionId).Category().length;
        if (selectedOptions == totalOptions) {
            $("#data-dataset-table-nav-content").find("[name=select-all][idn='" + selectedDimensionId + "']").prop('checked', true);
        }
        else {
            $("#data-dataset-table-nav-content").find("[name=select-all][idn='" + selectedDimensionId + "']").prop('checked', false);
        }
    });

    //sort
    $("#data-dataset-table-nav-content").find("[name=sort-options]").once("click", function () {
        var dimension = $(this).attr("idn");
        var select = $("#data-dataset-table-nav-content").find("select[idn='" + dimension + "']");
        var status = select.attr("sort");

        select.html(select.find('option').sort(function (x, y) {
            switch (status) {
                case "asc":
                    select.attr("sort", "desc");
                    return $(x).text() < $(y).text() ? 1 : -1;
                case "desc":
                    select.attr("sort", "asc");
                    return $(x).text() > $(y).text() ? 1 : -1;
                default:
                    break;
            }

        }));

    });

    //show/hide codes
    $('#data-dataset-table-code-toggle').once("change", function () {
        if (!$(this).is(':checked')) {
            $("#data-dataset-table-nav-content").find("[name=dimension-code]").each(function () {
                $(this).removeClass("d-none");
            });

            $("#data-dataset-table-nav-content").find("option").each(function () {
                var codeTrue = $(this).attr("code-true");
                $(this).text(codeTrue);
                $(this).attr("title", codeTrue);

            });

            $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=code]").removeClass("d-none");
        }
        else {
            $("#data-dataset-table-nav-content").find("[name=dimension-code]").each(function () {
                $(this).addClass("d-none");
            });

            $("#data-dataset-table-nav-content").find("option").each(function () {
                var codeFalse = $(this).attr("code-false");
                $(this).text(codeFalse);
                $(this).attr("title", codeFalse);

            });

            $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=code]").addClass("d-none");
        }
        // Trigger the responsivness when changing the lenght of the table cells because of the code
        // https://datatables.net/forums/discussion/44766/responsive-doesnt-immediately-resize-the-table
        $(window).trigger('resize');

        //redraw api code
        app.data.dataset.table.buildApiParams();

    });

    //click on an option, refresh count
    $("#data-dataset-table-nav-content").find("[name=dimension-select]").once('change', function () {
        $("#data-dataset-table-result").hide();
        app.data.dataset.table.response = null;
        app.data.dataset.table.jsonStat = null;
        app.data.dataset.table.countSelection();
        app.data.dataset.table.buildApiParams();
    });
    $("#data-dataset-table-result").hide();

    //reset api params
    app.data.dataset.table.buildApiParams();

    $("#data-dataset-table-accordion-collapse-widget [name=auto-update], #data-dataset-table-accordion-collapse-widget [name=include-copyright], #data-dataset-table-accordion-collapse-widget [name=include-link], #data-dataset-table-accordion-collapse-widget [name=include-title]").once("change", function () {
        app.data.dataset.table.callback.drawSnippetCode(true);
    });

    $("#data-dataset-table-accordion-collapse-widget [name=add-custom-configuration]").once("click", function () {
        $("#data-dataset-table-accordion-collapse-widget [name=invalid-json-object]").hide();
        $("#data-dataset-table-accordion-collapse-widget [name=valid-json-object]").hide();
        app.data.dataset.table.callback.drawSnippetCode(true)
    });

};

app.data.dataset.table.countSelection = function () {
    var count = 1;
    $("#data-dataset-table-nav-content [name=dimension-containers]").find("[name=dimension-select]").each(function () {
        var dimension = $(this).attr("idn");
        var totalOptions = $(this).find("option").length;
        var selectedOptions = $(this).find('option:selected').length;

        //if any option clicked check if select all should be ticked or not
        if (totalOptions == selectedOptions) { // all selected
            $("#data-dataset-table-nav-content").find("input[name=select-all][idn='" + dimension + "']").prop('checked', true);
            count = count * totalOptions;
        }
        else if (selectedOptions == 0) {
            count = count * totalOptions;
        }
        else {
            count = count * $(this).find('option:selected').length;
            $("#data-dataset-table-nav-content").find("input[name=select-all][idn='" + dimension + "']").prop('checked', false);
        }
    });
    app.data.dataset.table.selectionCount = count;

    $("#data-dataset-table-nav-content").find("[name=data-count-cells]").text(app.library.utility.formatNumber(app.data.dataset.table.selectionCount));
};

app.data.dataset.table.buildApiParams = function () {

    $("#data-dataset-table-api-jspnrpc-content [name=information-documentation]").html(
        app.library.html.parseDynamicLabel("information-api-documentation", ["JSON-RPC", $("<a>", {
            "href": C_APP_URL_GITHUB_API_CUBE_JSONRPC,
            "text": "GitHub Wiki",
            "target": "_blank"
        }).get(0).outerHTML]));

    $("#data-dataset-table-api-restful-content [name=information-documentation]").html(
        app.library.html.parseDynamicLabel("information-api-documentation", ["RESTful", $("<a>", {
            "href": C_APP_URL_GITHUB_API_CUBE_RESFFUL,
            "text": "GitHub Wiki",
            "target": "_blank"
        }).get(0).outerHTML]));

    $("#data-dataset-table-api-pxapiv1-content [name=information-documentation]").html(
        app.library.html.parseDynamicLabel("information-api-documentation", ["PxAPIv1", $("<a>", {
            "href": C_APP_URL_GITHUB_API_CUBE_PXAPIV1,
            "text": "GitHub Wiki",
            "target": "_blank"
        }).get(0).outerHTML]));

    var localParams = {
        "class": C_APP_JSONSTAT_QUERY_CLASS,
        "id": [],
        "dimension": {},
        "extension": {
            "pivot": $("#data-dataset-table-nav-content select[name=pivot] option:selected").attr("value") || null,
            "codes": !$('#data-dataset-table-code-toggle').is(':checked'),
            "language": {
                "code": app.data.LngIsoCode
            },
            "format": {
                "type": C_APP_FORMAT_TYPE_DEFAULT,
                "version": C_APP_FORMAT_VERSION_DEFAULT
            }
        },
        "version": C_APP_JSONSTAT_QUERY_VERSION,
        "m2m": false
    };

    if (app.data.isLive) {
        localParams.extension.matrix = app.data.MtrCode;
    }
    else {
        localParams.extension.release = app.data.RlsCode;
    }

    $("#data-dataset-table-nav-content").find("[name=dimension-containers]").find("select").each(function (index) {
        var numVariables = app.data.dataset.metadata.jsonStat.Dimension($(this).attr("idn")).Category().length;
        var dimension = {
            "category": {
                "index": []
            }
        };
        $(this).find('option:selected').each(function () {
            dimension.category.index.push(this.value);
        });
        if (dimension.category.index.length != numVariables && dimension.category.index.length > 0) { //only include dimension if not all variables selected
            localParams.id.push($(this).attr("idn"));
            localParams.dimension[$(this).attr("idn")] = dimension;
        }
    });

    //new query, empty old api params
    app.data.dataset.table.apiParamsData = {};
    //extend apiParams with local params
    $.extend(true, app.data.dataset.table.apiParamsData, localParams);

    $("#data-dataset-table-api-jsonrpc-post-url").text(app.config.url.api.jsonrpc.public);
    $("#data-dataset-table-api-restful-url").hide().text(
        C_APP_API_RESTFUL_READ_DATASET_URL.sprintf([app.config.url.restful,
        encodeURI(app.data.MtrCode),
        $("#data-dataset-table-accordion [name=format] option:selected").attr("frm-type"),
        $("#data-dataset-table-accordion [name=format] option:selected").attr("frm-version"),
        $("#data-dataset-table-nav-content select[name=pivot] option:selected").attr("value") ? app.data.LngIsoCode + "/" + $("#data-dataset-table-nav-content select[name=pivot] option:selected").attr("value") : app.data.LngIsoCode])).fadeIn();

    var JsonQuery = {
        "jsonrpc": C_APP_API_JSONRPC_VERSION,
        "method": "PxStat.Data.Cube_API.ReadDataset",
        "params": null
    };
    var apiParams = $.extend(true, {}, app.data.dataset.table.apiParamsData);
    delete apiParams.m2m;
    JsonQuery.params = apiParams;
    JsonQuery.params.extension.format.type = $("#data-dataset-table-accordion [name=format] option:selected").attr("frm-type");
    JsonQuery.params.extension.format.version = $("#data-dataset-table-accordion [name=format] option:selected").attr("frm-version");
    $("#data-dataset-table-api-jsonrpc-get-url").empty().text(encodeURI(app.config.url.api.jsonrpc.public + C_APP_API_GET_PARAMATER_IDENTIFIER + JSON.stringify(JsonQuery))).fadeIn();
    $("#data-dataset-table-api-jsonrpc-post-body").hide().text(JSON.stringify(JsonQuery, null, "\t")).fadeIn();
    //pxapiv1
    var pxapiv1Query = {
        "query": [],
        "response": {
            "format": null

        }
    };

    switch ($("#data-dataset-table-accordion [name=format] option:selected").attr("frm-type")) {
        case C_APP_TS_FORMAT_TYPE_JSONSTAT:
            switch ($("#data-dataset-table-accordion [name=format] option:selected").attr("frm-version")) {
                case C_APP_TS_FORMAT_VERSION_JSONSTAT_2X:
                    pxapiv1Query.response.format = C_APP_PXAPIV1_JSONSTAT_2X;
                    break;
                case C_APP_TS_FORMAT_VERSION_JSONSTAT_1X:
                    pxapiv1Query.response.format = C_APP_PXAPIV1_JSONSTAT_1X;
                    break;

                default:
                    break;
            }
            break;
        default:
            pxapiv1Query.response.format = $("#data-dataset-table-accordion [name=format] option:selected").attr("frm-type") ? $("#data-dataset-table-accordion [name=format] option:selected").attr("frm-type").toLowerCase() : "";
            break;
    }

    $.each(apiParams.dimension, function (key, value) {
        pxapiv1Query.query.push({
            "code": key,
            "selection": {
                "filter": "item",
                "values": value.category.index
            }
        });
    });

    var pivotCodeToInclude = $("#data-dataset-table-nav-content select[name=pivot] option:selected").attr("value")
        ? "/" + $("#data-dataset-table-nav-content select[name=pivot] option:selected").attr("value")
        : "";

    $("#data-dataset-table-api-pxapiv1-get-url").empty().text(encodeURI(app.config.url.api.restful.public
        + "/PxStat.Data.Cube_API.PxAPIv1"
        + "/" + JsonQuery.params.extension.language.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.subject.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.product.code
        + "/" + app.data.MtrCode
        + pivotCodeToInclude
        + "?query=" + JSON.stringify(pxapiv1Query))).fadeIn();

    $("#data-dataset-table-api-pxapiv1-post-url").empty().text(app.config.url.api.restful.public
        + "/PxStat.Data.Cube_API.PxAPIv1"
        + "/" + JsonQuery.params.extension.language.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.subject.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.product.code
        + "/" + app.data.MtrCode
        + pivotCodeToInclude).fadeIn();

    $("#data-dataset-table-api-pxapiv1-post-body").empty().text(JSON.stringify(pxapiv1Query, null, "\t")).fadeIn();


    // Refresh the Prism highlight
    Prism.highlightAll();
};

app.data.dataset.table.drawFormat = function () {
    $("#data-dataset-table-nav-content [name=download-select-dropdown], #data-dataset-table-confirm-soft [name=download-select-dataset], #data-dataset-table-confirm-hard [name=download-select-dataset], #data-dataset-table-accordion [name=format]").empty();
    $.each(app.data.dataset.format.response, function (index, format) {
        var formatDropdown = $("#data-dataset-table-templates").find("[name=download-dataset-format]").clone();
        formatDropdown.attr(
            {
                "frm-type": format.FrmType,
                "frm-version": format.FrmVersion
            });
        formatDropdown.find("[name=type]").text(format.FrmType);
        formatDropdown.find("[name=version]").text(format.FrmVersion);
        $("#data-dataset-table-nav-content [name=download-select-dropdown], #data-dataset-table-confirm-soft [name=download-select-dataset], #data-dataset-table-confirm-hard [name=download-select-dataset] ").append(formatDropdown.get(0).outerHTML);

        //populate api accordion formats
        var option = $("<option>", {
            "frm-type": format.FrmType,
            "frm-version": format.FrmVersion,
            "text": format.FrmType + " (" + format.FrmVersion + ")",
            "value": format.FrmType
        })
        $("#data-dataset-table-accordion [name=format]").append(option);
    });
    $("#data-dataset-table-accordion [name=format]").val(C_APP_FORMAT_TYPE_DEFAULT);

    $("#data-dataset-table-accordion [name=format]").once("change", app.data.dataset.table.buildApiParams);
    app.data.dataset.table.buildApiParams();

    $("#data-dataset-table-confirm-soft [name=download-dataset-format], #data-dataset-table-confirm-hard [name=download-dataset-format]").once("click", function (e) {
        e.preventDefault();
        app.data.dataset.table.resultsDownload($(this).attr("frm-type"), $(this).attr("frm-version"));
        $("#data-dataset-table-confirm-soft, #data-dataset-table-confirm-hard ").modal("hide");
    });
    $("#data-dataset-table-nav-content [name=download-dataset-format]").once("click", function (e) {
        e.preventDefault();
        app.data.dataset.table.resultsDownload($(this).attr("frm-type"), $(this).attr("frm-version"));
    });

}

app.data.dataset.table.drawPivotDropdown = function () {
    // Local alias
    var data = app.data.dataset.metadata.jsonStat;
    $("#data-dataset-table-nav-content select[name=pivot]").empty();
    $("#data-dataset-table-nav-content select[name=pivot]").append(
        $('<option>', {
            "title": app.label.static["none"],
            "text": app.label.static["none"]
        }
        ));
    $.each(data.id, function (index, value) {
        $("#data-dataset-table-nav-content select[name=pivot]").append(
            $('<option>', {
                "value": value,
                "title": data.Dimension(value).label,
                "text": data.Dimension(value).label
            }
            ));
    });

    $("#data-dataset-table-nav-content select[name=pivot]").once("change", function () {
        $("#data-dataset-table-result").hide();
        app.data.dataset.table.pivot.dimensionCode = $(this).find("option:selected").attr("value") || null;
        app.data.dataset.table.buildApiParams();
        if (app.data.dataset.table.jsonStat) {
            app.data.dataset.table.callback.drawDatatable();
            app.data.dataset.table.callback.drawSnippetCode(true);
        }

    });

}

app.data.dataset.table.confirmHardThreshold = function (pMessage) {
    // Set the body of the Modal
    $("#data-dataset-table-confirm-hard").find('.modal-body > p').empty().html(pMessage);

    // Display the Modal
    $("#data-dataset-table-confirm-hard").modal();
};

app.data.dataset.table.confirmSoftThreshold = function (pMessage, pCallbackMethod, pCallbackParams) {

    // Set the body of the Modal - Empty the container first
    $("#data-dataset-table-confirm-soft").find('.modal-body > p').empty().html(pMessage);

    $("#modal-button-confirm-data").once("click", function () {
        // Run the Callback function
        pCallbackMethod(pCallbackParams);

        // Close the Modal
        $("#data-dataset-table-confirm-soft").modal('hide');
    });

    // Display the Modal
    $("#data-dataset-table-confirm-soft").modal();
};

app.data.dataset.table.ajax.data = function () {
    if (app.data.isLive) {
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.public,
            "PxStat.Data.Cube_API.ReadDataset",
            app.data.dataset.table.apiParamsData,
            "app.data.dataset.table.callback.data",
            null,
            null,
            null,
            { async: false }
        );
    }

    else {
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.private,
            "PxStat.Data.Cube_API.ReadPreDataset",
            app.data.dataset.table.apiParamsData,
            "app.data.dataset.table.callback.data",
            null,
            null,
            null,
            { async: false }
        );
    }

};

app.data.dataset.table.callback.data = function (response) {
    if (response) {
        app.data.dataset.table.response = response;
        app.data.dataset.table.jsonStat = response ? JSONstat(response) : null;
        if (app.data.dataset.table.jsonStat && app.data.dataset.table.jsonStat.length) {
            app.data.dataset.table.callback.drawDatatable();
            app.data.dataset.table.callback.drawSnippetCode(true);
        }
        // Handle Exception
        else api.modal.exception(app.label.static["api-ajax-exception"]);
    }
    // Handle Exception
    else {
        app.data.dataset.table.response = null;
        app.data.dataset.table.jsonStat = null;
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

app.data.dataset.table.callback.drawSnippetCode = function (widgetEnabled) { //change to snippet
    if (widgetEnabled) {
        $("#data-dataset-table-accordion-collapse-widget").find("[name=make-selection-message]").hide();
        $("#data-dataset-table-accordion-collapse-widget [name=include-copyright], #data-dataset-table-accordion-collapse-widget [name=include-link], #data-dataset-table-accordion-collapse-widget [name=include-title]").bootstrapToggle('enable');
        $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").prop("disabled", false);
        $("#data-dataset-table-accordion-collapse-widget [name=add-custom-configuration]").prop("disabled", false);
        //check for WIP
        if (!app.data.RlsCode) {
            $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('enable');
        }
        else {
            if (app.data.isLive) {
                $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('enable');
            }
        }

        app.data.dataset.table.snippet.configuration = {};
        $.extend(true, app.data.dataset.table.snippet.configuration, app.data.dataset.table.snippet.template);
        app.data.dataset.table.snippet.configuration.autoupdate = $("#data-dataset-table-accordion-collapse-widget").find("[name=auto-update]").is(':checked');
        app.data.dataset.table.snippet.configuration.copyright = $("#data-dataset-table-accordion-collapse-widget").find("[name=include-copyright]").is(':checked');
        app.data.dataset.table.snippet.configuration.title = $("#data-dataset-table-accordion-collapse-widget").find("[name=include-title]").is(':checked');
        app.data.dataset.table.snippet.configuration.link = $("#data-dataset-table-accordion-collapse-widget").find("[name=include-link]").is(':checked') ? app.config.url.application + C_COOKIE_LINK_TABLE + "/" + app.data.MtrCode : null;
        app.data.dataset.table.snippet.configuration.pivot = app.data.dataset.table.pivot.dimensionCode;


        if ($("#data-dataset-table-accordion-collapse-widget").find("[name=auto-update]").is(':checked')) {
            var JsonQuery = {
                "jsonrpc": C_APP_API_JSONRPC_VERSION,
                "method": "PxStat.Data.Cube_API.ReadDataset",
                "params": null
            };

            var widgetParams = $.extend(true, {}, app.data.dataset.table.apiParamsData);
            delete widgetParams.m2m;
            JsonQuery.params = widgetParams;
            JsonQuery.params.extension.format.type = C_APP_FORMAT_TYPE_DEFAULT;
            JsonQuery.params.extension.format.version = C_APP_FORMAT_VERSION_DEFAULT;


            if (app.data.isLive) {
                JsonQuery.params.extension.matrix = app.data.MtrCode;
                app.data.dataset.table.snippet.configuration.data.api.query.url = app.config.url.api.jsonrpc.public;
                JsonQuery.method = "PxStat.Data.Cube_API.ReadDataset";
                delete JsonQuery.params.extension.release;
            }
            else {
                JsonQuery.params.extension.release = app.data.RlsCode;
                app.data.dataset.table.snippet.configuration.data.api.query.url = app.config.url.api.jsonrpc.private;
                JsonQuery.method = "PxStat.Data.Cube_API.ReadPreDataset";
                delete JsonQuery.params.extension.matrix;
            }


            //add query to snippet object
            app.data.dataset.table.snippet.configuration.data.api.query.data = JsonQuery;

        }
        else {
            app.data.dataset.table.snippet.configuration.data.api.response = app.data.dataset.table.response;
        }
    }

    else {
        $("#data-dataset-table-accordion-collapse-widget").find("[name=make-selection-message]").show();
    }

    //add custom config if something there
    if ($("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val().trim().length) {
        try {
            app.data.dataset.table.callback.formatJson();
            var customOptions = JSON.parse($("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val().trim());
            $.extend(true, app.data.dataset.table.snippet.configuration.options, customOptions);
            $("#data-dataset-table-accordion-collapse-widget [name=invalid-json-object]").hide();
            $("#data-dataset-table-accordion-collapse-widget [name=valid-json-object]").show();
        } catch (err) {
            $("#data-dataset-table-accordion-collapse-widget").collapse('show');
            $("#data-dataset-table-accordion-collapse-widget [name=invalid-json-object]").show();
            $("#data-dataset-table-accordion-collapse-widget [name=valid-json-object]").hide();
        }
    }

    //draw snippet code
    if (widgetEnabled) {
        var snippet = app.config.entity.data.snippet;
        snippet = snippet.sprintf([C_APP_URL_PXWIDGET_ISOGRAM, C_APP_PXWIDGET_TYPE_TABLE, app.library.utility.randomGenerator('pxwidget'), JSON.stringify(app.data.dataset.table.snippet.configuration)]);
        $("#data-dataset-table-accordion-snippet-code").hide().text(snippet.trim()).fadeIn();
        Prism.highlightAll();
    }


};

app.data.dataset.table.callback.formatJson = function () {
    $("#data-dataset-table-accordion-collapse-widget [name=invalid-json-object]").hide();
    if ($("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val().trim().length) {
        var ugly = $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val().trim();
        var obj = null;
        var pretty = null;
        try {
            obj = JSON.parse(ugly);
            pretty = JSON.stringify(obj, undefined, 4);
            $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val(pretty);
        } catch (err) {
            $("#data-dataset-table-accordion-collapse-widget [name=invalid-json-object]").show();
        }
    }
}

app.data.dataset.table.callback.drawDatatable = function () {
    api.spinner.start();

    if ($.fn.DataTable.isDataTable("#data-dataset-table-nav-content [name=datatable]")) {
        // Cannot use redraw as columns are dynamically created depending on the matrix or pivot. 
        // Must clear first then destroy and later on re-initiate 
        $("#data-dataset-table-nav-content").find("[name=datatable]").DataTable().clear().destroy();

        //clean pervious table drawing
        $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=header-row]").empty();
        $("#data-dataset-table-nav-content").find("[name=datatable]").find("tbody").empty();
    }

    // Local alias
    var data = app.data.dataset.table.jsonStat;

    // Parse data-id to array-of-object
    var jsonTable = data.toTable({
        type: 'arrobj',
        meta: true,
        unit: true,
        content: "id"
    });

    // Pivot table on demand
    jsonTable = app.data.dataset.table.pivot.compute(jsonTable);

    // Reset and Populate columns with Dimensions
    var tableColumns = [];
    $.each(data.id, function (i, v) {
        // Draw heading
        var codeSpan = $('<span>', {
            "name": "code",
            "class": $('#data-dataset-table-code-toggle').is(':checked') ? "badge badge-pill badge-neutral mx-2 font-weight-bold d-none" : "badge badge-pill badge-neutral mx-2 font-weight-bold",
            "text": data.id[i]
        }).get(0).outerHTML;

        var tableHeading = $("<th>", {
            "html": codeSpan + data.Dimension(i).label
        });

        $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=header-row]").append(tableHeading);

        // Append datatable column
        tableColumns.push({
            data: data.id[i],
            "visible": data.id[i] == app.data.dataset.table.pivot.dimensionCode ? false : true,
            "searchable": data.id[i] == app.data.dataset.table.pivot.dimensionCode ? false : true,
            render: function (cell, type, row, meta) {
                //alternative to using "createdCell" and data-order attribute which does not work with render
                //depending on request type, return either the code to sort if the time column, or the label for any other column
                //https://stackoverflow.com/questions/51719676/datatables-adding-data-order
                switch (type) {
                    case "sort":
                        return data.Dimension(meta.col).role == "time" ? cell : data.Dimension(data.id[i]).Category(cell).label;
                        break;

                    default:
                        var codeSpan = $('<span>', {
                            "name": "code",
                            "class": $('#data-dataset-table-code-toggle').is(':checked') ? "badge badge-pill badge-neutral mx-2 d-none" : "badge badge-pill badge-neutral mx-2",
                            "text": cell
                        }).get(0).outerHTML;
                        return codeSpan + data.Dimension(data.id[i]).Category(cell).label;
                        break;
                }
            }
        });
    });

    // The column Unit is irrelevent if pivoting by Statitic
    if (!app.data.dataset.table.pivot.dimensionCode || !app.data.dataset.table.pivot.isMetric) {
        // Populate Unit column
        var unitHeading = $("<th>", {
            "html": app.label.static["unit"],
            "class": "text-dark bg-default"
        });

        $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=header-row]").append(unitHeading);

        tableColumns.push({
            "data": "unit.label",
            "type": "data"
        });
    }

    // Populate Pivoted columns
    if (app.data.dataset.table.pivot.dimensionCode) {
        $.each(app.data.dataset.table.pivot.variableCodes, function (index, value) {
            tableColumns.push({
                "data": value,
                "type": "data",
                "class": "text-right font-weight-bold",
                "defaultContent": app.config.entity.data.datatable.null,
                "render": function (cell, type, row, meta) {
                    // If pivoting by Statitic then the decimals may be different within the same row
                    return app.library.utility.formatNumber(cell, (app.data.dataset.table.pivot.isMetric ? data.Dimension(app.data.dataset.table.pivot.dimensionCode).Category(value).unit.decimals : row.unit.decimals));
                }
            });
            var codeSpan = $('<span>', {
                "name": "code",
                "class": $('#data-dataset-table-code-toggle').is(':checked') ? "badge badge-pill badge-neutral mx-2 d-none" : "badge badge-pill badge-neutral mx-2",
                "text": value
            }).get(0).outerHTML;

            // If pivoting by Statistic then concatenate the Unit to the Heading
            var pivotHeading = $("<th>",
                {
                    "html": data.Dimension(app.data.dataset.table.pivot.dimensionCode).Category(value).label
                        + (app.data.dataset.table.pivot.isMetric ? " (" + data.Dimension(app.data.dataset.table.pivot.dimensionCode).Category(value).unit.label + ")" : "")
                        + codeSpan,
                    "class": "text-right text-light bg-primary"
                });

            $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=header-row]").append(pivotHeading);
        });
    } else {
        tableColumns.push({
            "data": "value",
            "type": "data",
            "class": "text-right font-weight-bold",
            "defaultContent": app.config.entity.data.datatable.null,
            "render": function (cell, type, row, meta) {
                return app.library.utility.formatNumber(cell, row.unit.decimals);
            }
        });

        var valueHeading = $("<th>",
            {
                "html": app.label.static["value"],
                "class": "text-right text-light bg-primary"
            });
        $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=header-row]").append(valueHeading);
    }

    // $("#data-dataset-table-nav-content [name=result-table]").html(dataContainer.get(0).outerHTML);
    //Draw DataTable with Data Set data
    var localOptions = {
        iDisplayLength: app.config.entity.data.datatable.length,
        data: jsonTable.data,
        columns: tableColumns,
        drawCallback: function (settings) {
            app.data.dataset.table.drawCallbackDrawDataTable();
        },
        //Translate labels language
        language: app.label.plugin.datatable
    };

    // Initiate DataTable
    $("#data-dataset-table-nav-content").find("[name=datatable]").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
        app.data.dataset.table.drawCallbackDrawDataTable();
    });

    $('[data-toggle="tooltip"]').tooltip();
    $('#code-toggle').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["false"],
        off: app.label.static["true"],
        onstyle: "tertiary",
        offstyle: "neutral",
        width: C_APP_TOGGLE_LENGTH
    });

    //scroll to top of datatable -- Data entity

    if (app.data.isModal) {
        $('#data-view-modal').animate({
            scrollTop: '+=' + $('#data-dataset-table-nav-content [name=result-table]')[0].getBoundingClientRect().top
        },
            1000);
    }
    else {
        $('html, body').animate({
            scrollTop: $("#data-dataset-table-nav-content [name=result-table]").offset().top
        }, 1000);
    }
};

/**
 * Pivot a dataset by a Classification or Time code 
 * @param {*} arrobjTable 
 */
app.data.dataset.table.pivot.compute = function (arrobjTable) {
    // Init
    app.data.dataset.table.pivot.isMetric = false;
    app.data.dataset.table.pivot.variableCodes = [];

    if (!app.data.dataset.table.pivot.dimensionCode) {
        return arrobjTable;
    }

    // Check if pivoting by Statistic
    app.data.dataset.table.pivot.isMetric = arrobjTable.meta.dimensions[app.data.dataset.table.pivot.dimensionCode].role == "metric" ? true : false;

    // Clone tables
    var reducedTable = $.extend(true, {}, arrobjTable);
    var pivotedTable = $.extend(true, {}, arrobjTable);

    $.each(arrobjTable.data, function (indexData, rowData) {
        // Get all values to pivot
        if ($.inArray(rowData[app.data.dataset.table.pivot.dimensionCode], app.data.dataset.table.pivot.variableCodes) == -1) {
            app.data.dataset.table.pivot.variableCodes.push(rowData[app.data.dataset.table.pivot.dimensionCode]);
        }

        // Reduce the data by the pivot size
        if (rowData[app.data.dataset.table.pivot.dimensionCode] != app.data.dataset.table.pivot.variableCodes[0]) {
            reducedTable.data.splice(indexData, 1);
            pivotedTable.data.splice(indexData, 1);
        }
    });

    $.each(arrobjTable.data, function (indexData, rowData) {
        $.each(reducedTable.data, function (indexReduced, rowReduced) {
            var match = true;

            // Match the reduced data against the data
            $.each(rowReduced, function (key, value) {
                if ($.inArray(key, [app.data.dataset.table.pivot.dimensionCode, 'unit', 'value']) == -1 && rowData[key] != value) {
                    match = false;
                    return false;
                }
            });

            if (match) {
                // remove pivoted value
                delete pivotedTable.data[indexReduced]['value'];

                // append pivoted column 
                var pivotedColumn = rowData[app.data.dataset.table.pivot.dimensionCode]
                pivotedTable.data[indexReduced][pivotedColumn] = rowData.value;
                return false;
            }
        });
    });

    // remove pivoted rows and re-arrange array
    arrobjTable.data = [];
    $.each(pivotedTable.data, function (indexPivoted, rowPivoted) {
        if (!rowPivoted.hasOwnProperty('value'))
            arrobjTable.data.push(rowPivoted);
    });

    return arrobjTable;
}

app.data.dataset.table.drawCallbackDrawDataTable = function () {
    //check whether to show codes or not every time the table is drawn
    if (!$('#code-toggle').prop('checked')) {
        $("#data-view-container").find("[name=datatable]").find("[name=code]").removeClass("d-none");
    }
    else {
        $("#data-view-container").find("[name=datatable]").find("[name=code]").addClass("d-none");
    }


    api.spinner.stop();

    $("#data-dataset-table-result").fadeIn();

}


app.data.dataset.table.resultsDownload = function (format, version) {
    var apiParams = $.extend(true, {}, app.data.dataset.table.apiParamsData);
    apiParams.extension.format.type = format;
    apiParams.extension.format.version = version;
    app.data.dataset.ajax.downloadDataset(apiParams);
}


