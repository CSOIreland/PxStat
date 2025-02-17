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
app.data.dataset.table.order = null;
app.data.dataset.table.timeColumn = null;
app.data.dataset.table.search = "";
app.data.dataset.table.pivot = {};
app.data.dataset.table.pivot.dimensionCode = null;
app.data.dataset.table.pivot.isMetric = false;
app.data.dataset.table.pivot.variableCodes = [];
app.data.dataset.table.snippet = {};
app.data.dataset.table.snippet.configuration = {};
app.data.dataset.table.saveQuery = {};
app.data.dataset.table.saveQuery.configuration = {};
app.data.dataset.table.saveQuery.ajax = {};
app.data.dataset.table.saveQuery.callback = {};
app.data.dataset.table.snippet.template = {
    "autoupdate": true,
    "matrix": null,
    "fluidTime": [],
    "copyright": true,
    "title": null,
    "link": null,
    "pivot": null,
    "hideColumns": [],
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
    "metadata": {
        "api": {
            "query": {
                "url": null,
                "data": {
                    "jsonrpc": "2.0",
                    "method": "PxStat.Data.Cube_API.ReadMetadata",
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
    "options": {
        "language": app.label.plugin.datatable,
        "search": {
            "search": null
        },
        "dom": "Bfltip",
        "responsive": false,
        "buttons": [
            {
                "extend": "csv",
                "text": app.label.static["download"] + " CSV",
                "className": "export-button",
                "title": app.data.MtrCode + "." + moment(Date.now()).format(app.config.mask.datetime.file),
                "exportOptions": {
                    "columns": []
                }
            },
            {
                "extend": "print",
                "text": app.label.static["print"],
                "className": "export-button",
                "title": "",
                "exportOptions": {
                    "columns": []
                }
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
    $("#data-dataset-table-accordion-collapse-widget").find("[name=widget-snippet-information]").hide();
    $("#data-dataset-table-accordion-collapse-widget [name=include-copyright], #data-dataset-table-accordion-collapse-widget [name=link-to-wip], #data-dataset-table-accordion-collapse-widget [name=include-link], #data-dataset-table-accordion-collapse-widget [name=include-title], #data-dataset-table-accordion-collapse-widget [name=include-pagination], #data-dataset-table-accordion-collapse-widget [name=include-buttons], #data-dataset-table-accordion-collapse-widget [name=include-search], #data-dataset-table-accordion-collapse-widget [name=include-responsive]").bootstrapToggle('disable');
    $("#data-dataset-table-accordion-collapse-widget").find("[name=title-value]").prop('disabled', true);

    //check for WIP
    if (app.data.RlsCode) {
        if (!app.data.isLive) { //is WIP
            $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('off');
            $("#data-dataset-table-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('off');
        }
        $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('disable');
        $("#data-dataset-table-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('disable');
    }
    else {
        $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('disable');
        $("#data-dataset-table-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('disable');
    }



    $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").prop("disabled", true);
    $("#data-dataset-table-accordion-collapse-widget [name=add-custom-configuration]").prop("disabled", true);
    $("#data-dataset-table-accordion-collapse-widget").collapse('hide');

    $("#data-dataset-table-code-toggle").bootstrapToggle('on');
    app.data.dataset.table.totalCount = 1;
    $("#data-dataset-table-nav-content").find("[name=dimension-containers]").empty();

    //render all statistic dimensions first
    $.each(app.data.dataset.metadata.jsonStat.Dimension({ role: "metric" }), function (index, value) {
        app.data.dataset.table.totalCount = app.data.dataset.table.selectionCount = app.data.dataset.table.totalCount * value.length;

        var dimensionContainer = $("#data-dataset-table-templates").find("[name=dimension-container]").clone();
        var dimensionCode = $("<small>", {
            "text": " - " + app.data.dataset.metadata.jsonStat.role.metric[index],
            "name": "dimension-code",
            "class": "d-none"
        }).get(0).outerHTML;
        dimensionContainer.find("[name=dimension-label]").html(value.label + dimensionCode);
        dimensionContainer.find("[name=dimension-count]").text(value.length);
        dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]);
        dimensionContainer.find("[name=select-all]").attr("role", value.role);
        dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]);
        dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]);

        $.each(value.id, function (variableIndex, variable) {
            var option = $('<option>', {
                "value": variable,
                "data-code-true": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "") + " (" + variable + ")",
                "data-code-false": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                "title": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
            });

            dimensionContainer.find("select").append(option);
        });
        dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]).attr("role", value.role);

        $("#data-dataset-table-nav-content").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
    });

    //next render time dimensions
    $.each(app.data.dataset.metadata.jsonStat.Dimension({ role: "time" }), function (index, value) {
        app.data.dataset.table.totalCount = app.data.dataset.table.selectionCount = app.data.dataset.table.totalCount * value.length;

        var dimensionContainer = $("#data-dataset-table-templates").find("[name=dimension-container]").clone();
        var dimensionCode = $("<small>", {
            "text": " - " + app.data.dataset.metadata.jsonStat.role.time[index],
            "name": "dimension-code",
            "class": "d-none"
        }).get(0).outerHTML;
        dimensionContainer.find("[name=dimension-label]").html(value.label + dimensionCode);
        dimensionContainer.find("[name=dimension-count]").text(value.length);
        dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]);
        dimensionContainer.find("[name=select-all]").attr("role", value.role);
        dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]);
        dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]);

        $.each(value.id, function (variableIndex, variable) {
            var option = $('<option>', {
                "data-position": (value.id.length - variableIndex) - 1,
                "value": variable,
                "data-code-true": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "") + " (" + variable + ")",
                "data-code-false": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                "title": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
            });

            dimensionContainer.find("select").append(option);
        });
        dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]).attr("role", value.role);

        //reverse select based on codes so most recent time first
        dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
            return $(x).val() < $(y).val() ? 1 : -1;
        }));
        $("#data-dataset-table-nav-content").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
    });

    //next render classifications
    $.each(app.data.dataset.metadata.jsonStat.Dimension(), function (index, value) {
        if (value.role != "metric" && value.role != "time") {
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
            dimensionContainer.find("[name=select-all]").attr("role", value.role);
            dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]);
            dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]);

            $.each(value.id, function (variableIndex, variable) {
                var option = $('<option>', {
                    "value": variable,
                    "data-code-true": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "") + " (" + variable + ")",
                    "data-code-false": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                    "title": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                    "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                });

                dimensionContainer.find("select").append(option);
            });
            dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("role", value.role);

            $("#data-dataset-table-nav-content").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
        };
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
                if (!select.find("option[value='" + key + "']").is(':selected')) {
                    var textWithCode = thisDimension.Category(key).label + (thisDimension.Category(key).unit ? " (" + thisDimension.Category(key).unit.label + ")" : "") + " (" + key + ")";
                    var textWithoutCode = thisDimension.Category(key).label + (thisDimension.Category(key).unit ? " (" + thisDimension.Category(key).unit.label + ")" : "");
                    if (select.attr("role") == "time") {
                        select.prepend($('<option>', {
                            "value": key,
                            "data-code-true": textWithCode,
                            "data-code-false": textWithoutCode,
                            "title": value,
                            "text": !$('#data-dataset-table-code-toggle').is(':checked') ? textWithCode : textWithoutCode
                        }));
                    }
                    else {
                        select.append($('<option>', {
                            "value": key,
                            "data-code-true": textWithCode,
                            "data-code-false": textWithoutCode,
                            "title": value,
                            "text": !$('#data-dataset-table-code-toggle').is(':checked') ? textWithCode : textWithoutCode
                        }));
                    }

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

        if ($(this).attr("role") == "time") {
            if (select.val().length == 0) {
                $("#data-dataset-table-api-data-connector-content").find("[name=fluid-time]").bootstrapToggle('off').bootstrapToggle('disable');
            }
            else {
                $("#data-dataset-table-api-data-connector-content").find("[name=fluid-time]").bootstrapToggle('enable');
            }
        }

        $("#data-dataset-table-result-wrapper").hide();
        app.data.dataset.table.response = null;
        app.data.dataset.table.jsonStat = null;
        app.data.dataset.customTable.parsedData = null;
        $("#pxwidget-custom-table").empty();
        $('#data-dataset-table-result-custom-available-fields [name="available-fields"]').empty();
        $('#data-dataset-table-result-custom-available-fields [name="row-fields"]').empty();
        $('#data-dataset-table-result-custom-available-fields [name="column-fields"]').empty();
        $("#data-dataset-table-accordion-custom-widget").hide();
        app.data.dataset.table.countSelection()
        app.data.dataset.table.buildApiParams();
    });

    //toggle select all depending on  number of options selected
    $("#data-dataset-table-nav-content").find("select[name=dimension-select]").once('change', function () {
        var selectedDimensionId = $(this).attr("idn");
        var selectedOptions = $(this).val().length;
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
                    if (app.data.dataset.metadata.jsonStat.Dimension(dimension).role == "time") {
                        return $(x).val() < $(y).val() ? 1 : -1;
                    }
                    else {
                        return $(x).text() < $(y).text() ? 1 : -1;
                    }
                case "desc":
                    select.attr("sort", "asc");
                    if (app.data.dataset.metadata.jsonStat.Dimension(dimension).role == "time") {
                        return $(x).val() > $(y).val() ? 1 : -1;
                    }
                    else {
                        return $(x).text() > $(y).text() ? 1 : -1;
                    }

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
                var codeTrue = $(this).data("code-true");
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
                var codeFalse = $(this).data("code-false");
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
        if ($(this).attr("role") == "time") {
            $("#data-dataset-table-api-data-connector-content").find("[name=fluid-time]").bootstrapToggle('enable');
        }
        $("#data-dataset-table-result-wrapper").hide();
        app.data.dataset.table.response = null;
        app.data.dataset.table.jsonStat = null;
        app.data.dataset.customTable.parsedData = null;
        $("#pxwidget-custom-table").empty();
        $('#data-dataset-table-result-custom-available-fields [name="available-fields"]').empty();
        $('#data-dataset-table-result-custom-available-fields [name="row-fields"]').empty();
        $('#data-dataset-table-result-custom-available-fields [name="column-fields"]').empty();
        $("#data-dataset-table-accordion-custom-widget").hide();
        app.data.dataset.table.countSelection();
        app.data.dataset.table.buildApiParams();
    });
    $("#data-dataset-table-result-wrapper").hide();

    //reset api params
    app.data.dataset.table.buildApiParams();

    $("#data-dataset-table-accordion-collapse-widget [name=fluid-time], #data-dataset-table-accordion-collapse-widget [name=include-copyright], #data-dataset-table-accordion-collapse-widget [name=include-link], #data-dataset-table-accordion-collapse-widget [name=include-pagination], #data-dataset-table-accordion-collapse-widget [name=include-buttons], #data-dataset-table-accordion-collapse-widget [name=include-search], #data-dataset-table-accordion-collapse-widget [name=include-responsive]").once("change", function () {
        app.data.dataset.table.callback.drawSnippetCode(true);
    });

    $("#data-dataset-table-accordion-collapse-widget [name=include-title]").once("change", function () {
        app.data.dataset.table.callback.drawSnippetCode(true);
        if ($(this).is(':checked')) {
            $("#data-dataset-table-accordion-collapse-widget").find("[name=title-value]").show();
        } else {
            $("#data-dataset-table-accordion-collapse-widget").find("[name=title-value]").hide();
        }

    });

    $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").once("change", function () {
        app.data.dataset.table.callback.drawSnippetCode(true);
        if (!$(this).is(':checked')) {
            $("#data-dataset-table-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-table-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('disable');
        }
        else {
            $("#data-dataset-table-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('enable');
        }

    });

    $("#data-dataset-table-accordion-collapse-widget [name=link-to-wip]").once("change", function () {
        app.data.dataset.table.callback.drawSnippetCode(true);
    });

    $("#data-dataset-table-accordion-collapse-widget [name=add-custom-configuration]").once("click", function () {
        $("#data-dataset-table-accordion-collapse-widget [name=invalid-json-object]").hide();
        $("#data-dataset-table-accordion-collapse-widget [name=valid-json-object]").hide();
        app.data.dataset.table.callback.drawSnippetCode(true)
    });

    $("#data-dataset-table-accordion-collapse-widget [name=download-snippet]").once("click", function () {
        // Download the snippet file
        app.library.utility.download(app.data.fileNamePrefix + '.' + moment(Date.now()).format(app.config.mask.datetime.file), $("#data-dataset-table-accordion-snippet-code").text(), C_APP_EXTENSION_HTML, C_APP_MIMETYPE_HTML, false, true);
    });

    $("#data-dataset-table-accordion-collapse-widget [name=preview-snippet]").once("click", function () {
        app.library.utility.previewHtml($("#data-dataset-table-accordion-snippet-code").text())
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
    if (app.data.RlsCode) {
        if (!app.data.isLive) {
            $("#data-dataset-table-accordion-collapse-widget").find("[name=link-to-wip-wrapper]").show();
        }
    }


    if (app.data.isLive) {
        //enable RESTful and PxAPIv1 tabs
        $("#data-dataset-table-api-restful-tab").removeClass("disabled");
        $("#data-dataset-table-api-pxapiv1-tab").removeClass("disabled");
        $("#data-dataset-table-api-data-connector-tab").removeClass("disabled");
    }
    else {
        //disable RESTful and PxAPIv1 tabs
        $("#data-dataset-table-api-restful-tab").addClass("disabled");
        $("#data-dataset-table-api-pxapiv1-tab").addClass("disabled");
        $("#data-dataset-table-api-data-connector-tab").addClass("disabled");
    }

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
            "pivot": app.data.dataset.table.pivot.dimensionCode || null,
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
        if ($(this).val().length && $(this).val().length < numVariables) {
            localParams.id.push($(this).attr("idn"));
            localParams.dimension[$(this).attr("idn")] = {
                "category": {
                    "index": $(this).val()
                }
            };
        };
    });

    //new query, empty old api params
    app.data.dataset.table.apiParamsData = {};
    //extend apiParams with local params
    $.extend(true, app.data.dataset.table.apiParamsData, localParams);

    $("#data-dataset-table-api-jsonrpc-post-url").text(app.data.isLive ? app.config.url.api.jsonrpc.public : app.config.url.api.jsonrpc.private);
    $("#data-dataset-table-api-restful-url").text(
        C_APP_API_RESTFUL_READ_DATASET_URL.sprintf([app.config.url.api.restful + "/",
        encodeURI(app.data.MtrCode),
        $("#data-dataset-table-accordion [name=format]").find('option:selected').data('frm-type') || C_APP_FORMAT_TYPE_DEFAULT,
        $("#data-dataset-table-accordion [name=format]").find('option:selected').data('frm-version') || C_APP_FORMAT_VERSION_DEFAULT,
        app.data.dataset.table.pivot.dimensionCode ? app.data.LngIsoCode + "/" + app.data.dataset.table.pivot.dimensionCode : app.data.LngIsoCode])).fadeIn();

    var JsonQuery = {
        "jsonrpc": C_APP_API_JSONRPC_VERSION,
        "method": app.data.isLive ? "PxStat.Data.Cube_API.ReadDataset" : "PxStat.Data.Cube_API.ReadPreDataset",
        "params": null
    };
    var apiParams = $.extend(true, {}, app.data.dataset.table.apiParamsData);
    delete apiParams.m2m;
    JsonQuery.params = apiParams;
    JsonQuery.params.extension.format.type = $("#data-dataset-table-accordion [name=format]").find('option:selected').data('frm-type') || C_APP_FORMAT_TYPE_DEFAULT;
    JsonQuery.params.extension.format.version = $("#data-dataset-table-accordion [name=format]").find('option:selected').data('frm-version') || C_APP_FORMAT_VERSION_DEFAULT;
    var jsonrpcGetUrl = app.data.isLive ? app.config.url.api.jsonrpc.public : app.config.url.api.jsonrpc.private;
    $("#data-dataset-table-api-jsonrpc-get-url").hide().text(encodeURI(jsonrpcGetUrl + C_APP_API_GET_PARAMATER_IDENTIFIER + JSON.stringify(JsonQuery))).fadeIn();
    $("#data-dataset-table-api-jsonrpc-post-body").hide().text(JSON.stringify(JsonQuery, null, "\t")).fadeIn();
    //pxapiv1
    var pxapiv1Query = {
        "query": [],
        "response": {
            "format": null,
            "pivot": app.data.dataset.table.pivot.dimensionCode || null,
            "codes": !$('#data-dataset-table-code-toggle').is(':checked')
        }
    };

    switch ($("#data-dataset-table-accordion [name=format]").find('option:selected').data('frm-type')) {
        case C_APP_TS_FORMAT_TYPE_JSONSTAT:
            switch ($("#data-dataset-table-accordion [name=format]").find('option:selected').data('frm-version')) {
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
            pxapiv1Query.response.format = $("#data-dataset-table-accordion [name=format]").find('option:selected').data('frm-type') ? $("#data-dataset-table-accordion [name=format]").find('option:selected').data('frm-type').toLowerCase() : C_APP_PXAPIV1_JSONSTAT_2X;
            break;
    }

    var dataConnectorQuery = $.extend(true, {}, pxapiv1Query);

    $.each(apiParams.dimension, function (key, value) {
        pxapiv1Query.query.push({
            "code": key,
            "selection": {
                "filter": "item",
                "values": value.category.index
            }
        });
    });


    $("#data-dataset-table-api-pxapiv1-get-url").hide().text(encodeURI(app.config.url.api.restful
        + "/PxStat.Data.Cube_API.PxAPIv1"
        + "/" + JsonQuery.params.extension.language.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.subject.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.product.code
        + "/" + app.data.MtrCode
        + "?query=" + JSON.stringify(pxapiv1Query))).fadeIn();

    $("#data-dataset-table-api-pxapiv1-post-url").hide().text(app.config.url.api.restful
        + "/PxStat.Data.Cube_API.PxAPIv1"
        + "/" + JsonQuery.params.extension.language.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.subject.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.product.code
        + "/" + app.data.MtrCode).fadeIn();

    $("#data-dataset-table-api-pxapiv1-post-body").hide().text(JSON.stringify(pxapiv1Query, null, "\t")).fadeIn();

    //data connector
    $.each(apiParams.dimension, function (key, value) {
        if (app.data.dataset.metadata.jsonStat.Dimension(key).role == "time") {
            var dimensionObj = {
                "code": key,
                "selection": {
                    "filter": $("#data-dataset-table-api-data-connector-content").find("[name=fluid-time]").is(':checked') ? "fluid" : "item",
                    "values": []
                }
            };

            if (!$("#data-dataset-table-nav-content select[idn='" + key + "'] option:selected").length) {
                $("#data-dataset-table-nav-content select[idn='" + key + "'] option").each(function (indexTimeVariable, valueTimeVariable) {
                    dimensionObj.selection.values.push($(this).data("position"))
                });
            }
            else {//one or more time selected
                $("#data-dataset-table-nav-content select[idn='" + key + "'] option:selected").each(function () {
                    dimensionObj.selection.values.push(
                        $("#data-dataset-table-api-data-connector-content").find("[name=fluid-time]").is(':checked')
                            ? $(this).data("position")
                            : $(this).val()
                    )
                });
            }
            dataConnectorQuery.query.push(dimensionObj);
        }
        else {
            dataConnectorQuery.query.push({
                "code": key,
                "selection": {
                    "filter": "item",
                    "values": value.category.index
                }
            });
        }
    });

    //fluid time is checked but no variables are selected or all variables are selected from one dimension
    if ($.isEmptyObject(apiParams.dimension) && $("#data-dataset-table-api-data-connector-content").find("[name=fluid-time]").is(':checked')) {
        var dimensionObj = {
            "code": app.data.dataset.metadata.timeDimensionCode,
            "selection": {
                "filter": "fluid",
                "values": []
            }
        };
        $("#data-dataset-table-nav-content select[idn='" + app.data.dataset.metadata.timeDimensionCode + "'] option:selected").each(function (indexTimeVariable, valueTimeVariable) {
            dimensionObj.selection.values.push($(this).data("position"))
        });
        if (dimensionObj.selection.values.length) {
            dataConnectorQuery.query.push(dimensionObj);
        }

    }

    $("#data-dataset-table-api-data-connector-get-url").hide().text(encodeURI(app.config.url.api.restful
        + "/PxStat.Data.Cube_API.PxAPIv1"
        + "/" + JsonQuery.params.extension.language.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.subject.code
        + "/" + app.data.dataset.metadata.jsonStat.extension.product.code
        + "/" + app.data.MtrCode
        + "?query=" + JSON.stringify(dataConnectorQuery))).fadeIn();
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
            "data-frm-type": format.FrmType,
            "data-frm-version": format.FrmVersion,
            "text": format.FrmType + " (" + format.FrmVersion + ")",
            "value": format.FrmType
        })
        $("#data-dataset-table-accordion [name=format]").append(option);
    });
    $("#data-dataset-table-accordion [name=format]").val(C_APP_FORMAT_TYPE_DEFAULT);

    $("#data-dataset-table-accordion [name=format]").once("change", app.data.dataset.table.buildApiParams);

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
    $("#data-dataset-table-nav-content").find("[name=pivot-dimension-options]").empty();
    $("#data-dataset-table-nav-content").find("[name=selected-pivot]").text(app.label.static["none"]);
    $("#data-dataset-table-nav-content").find("[name=pivot-dimension-options]").append(
        $("<a>", {
            "name": "pivot-option",
            "class": "dropdown-item fw-bold",
            "href": "#",
            "text": app.label.static["none"]
        })
    );

    $.each(data.id, function (index, value) {

        $("#data-dataset-table-nav-content").find("[name=pivot-dimension-options]").append(
            $("<a>", {
                "name": "pivot-option",
                "class": "dropdown-item",
                "href": "#",
                "data-dimension-code": value,
                "text": data.Dimension(value).label
            })
        );
    });

    $("#data-dataset-table-nav-content").find("[name=pivot-dimension-options]").append(
        $("<div>", {
            "class": "dropdown-divider border-info"
        })
    );

    $("#data-dataset-table-nav-content").find("[name=pivot-dimension-options]").append(
        $("<a>", {
            "name": "pivot-advanced-option",
            "class": "dropdown-item",
            "href": "#",
            "html": $("<i>", {
                class: "fas fa-info-circle me-2"
            }).get(0).outerHTML + app.label.static["advanced"]
        })
    );

    $("#data-dataset-table-nav-content").find("[name=pivot-option]").once("click", function (e) {
        e.preventDefault();
        $("#data-dataset-table-nav-content").find("[name=selected-pivot]").text($(this).text());
        app.data.dataset.table.pivot.dimensionCode = $(this).data("dimension-code") || null;
        $("#data-dataset-table-nav-content").find("[name=pivot-dimension-options]").find("[name=pivot-option]").removeClass("fw-bold");
        if (!app.data.dataset.table.pivot.dimensionCode) {
            $("#data-dataset-table-nav-content").find("[name=pivot-dimension-options]").find("[name=pivot-option]").first().addClass("fw-bold");
        }
        else {
            $("#data-dataset-table-nav-content").find("[name=pivot-dimension-options]").find("[name=pivot-option][data-dimension-code='" + app.data.dataset.table.pivot.dimensionCode + "']").addClass("fw-bold");
        }
        app.data.dataset.table.buildApiParams();
        if (app.data.dataset.table.jsonStat) {
            app.data.dataset.table.callback.drawDatatable();
            app.data.dataset.table.callback.drawSnippetCode(true);
        }
    });

    $("#data-dataset-table-nav-content").find("[name=pivot-advanced-option]").once("click", function (e) {
        e.preventDefault();
        $("#data-dataset-table-advanced-pivot").find("[name=explanation]").html(
            app.library.html.parseDynamicLabel("pivoting-end-user-guide-github", [$("<a>", {
                "href": C_APP_URL_GITHUB_EUG_DATA_PIVOT,
                "text": app.label.static["click-here"],
                "target": "_blank"
            }).get(0).outerHTML])
        );
        $("#data-dataset-table-advanced-pivot").modal("show");
    });

    $("#data-dataset-table-advanced-pivot").find("[name=download]").once("click", function (e) {
        e.preventDefault();
        app.data.dataset.table.resultsDownload(C_APP_TS_FORMAT_TYPE_XLSX, C_APP_TS_FORMAT_VERSION_XLSX);
    });

}

app.data.dataset.table.confirmHardThreshold = function (pMessage) {
    // Set the body of the Modal
    $("#data-dataset-table-confirm-hard").find('.modal-body > p').empty().html(pMessage);

    // Display the Modal
    $("#data-dataset-table-confirm-hard").modal("show");
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
    $("#data-dataset-table-confirm-soft").modal("show");
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
            {
                async: false,
                timeout: app.config.transfer.timeout
            });
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
            {
                async: false,
                timeout: app.config.transfer.timeout
            });
    }

};

app.data.dataset.table.callback.data = function (response) {
    if (response) {
        app.data.dataset.table.response = response;
        app.data.dataset.table.jsonStat = response ? JSONstat($.extend(true, {}, response)) : null;
        if (app.data.dataset.table.jsonStat && app.data.dataset.table.jsonStat.length) {
            app.data.dataset.table.order = null;
            app.data.dataset.table.search = "";
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


app.data.dataset.table.callback.drawHiddenColumsCheckBoxes = function () {
    $("#data-dataset-table-accordion-collapse-widget").find("[name=hide-column-checkbox-container]").empty();
    var columnsToHide = [];

    //check the table has more than one row before allowing hidden columns
    if ($("#data-dataset-table-nav-content").find("[name=datatable]").DataTable().rows().count() != 1) {
        $.each(app.data.dataset.table.jsonStat.Dimension(), function (index, value) {
            //Only check non pivoted columns
            if (app.data.dataset.table.jsonStat.id[index] != app.data.dataset.table.pivot.dimensionCode) {
                if (value.id.length == 1) {
                    columnsToHide.push(app.data.dataset.table.jsonStat.id[index]);
                }
            }

        });

        //if pivoted column is not statistic, add option to hide unit column

        if (!app.data.dataset.table.pivot.dimensionCode || !app.data.dataset.table.pivot.isMetric) {
            var units = [];
            //check all units to see if that column can be hidden
            $.each(app.data.dataset.table.jsonStat.Dimension({ role: "metric" })[0].Category(), function (index, value) {
                units.push(value.unit.label)
            });

            var unitCanBeHidden = true;
            if (units.length > 1) {
                $.each(units, function (index, value) {
                    if (index != units.length - 1) {
                        if (value != units[index + 1]) {
                            unitCanBeHidden = false;
                            return false
                        }
                    }
                });
            }
            if (unitCanBeHidden) {
                columnsToHide.push("UNIT")
            }
        };

        //build checkboxes
        $.each(columnsToHide, function (index, value) {
            var dimensionLabel = null;
            if (value == C_APP_CSV_UNIT) {
                dimensionLabel = app.label.static["unit"];
            }
            else {
                dimensionLabel = app.data.dataset.table.jsonStat.Dimension(value).label;
            }

            var checkBox = $("#data-dataset-table-templates").find("[name=hide-column-checkbox]").clone();
            checkBox.find("[name=hide-column]").val(value).attr("id", "data-dataset-table-accordion-heading-widget-hide-checxbox-" + value);
            checkBox.find("label").attr("for", "data-dataset-table-accordion-heading-widget-hide-checxbox-" + value).text(dimensionLabel);
            $("#data-dataset-table-accordion-collapse-widget").find("[name=hide-column-checkbox-container]").append(checkBox);
        });
    }

    if (!columnsToHide.length) {
        $("#data-dataset-table-accordion-collapse-widget").find("[name=no-columns-to-hide]").show();
    }
    else {
        $("#data-dataset-table-accordion-collapse-widget").find("[name=no-columns-to-hide]").hide();
    }

    //add change event listener 
    $("#data-dataset-table-accordion-collapse-widget").find("[name=hide-column-checkbox-container] [name=hide-column]").once("change", function () {
        app.data.dataset.table.callback.drawSnippetCode(true);
    });
};



app.data.dataset.table.callback.drawSnippetCode = function (widgetEnabled) { //change to snippet
    if (widgetEnabled) {
        $("#data-dataset-table-accordion-collapse-widget").find("[name=make-selection-message]").hide();
        $("#data-dataset-table-accordion-collapse-widget").find("[name=widget-snippet-information]").show();
        $("#data-dataset-table-accordion-collapse-widget [name=include-copyright], #data-dataset-table-accordion-collapse-widget [name=include-link], #data-dataset-table-accordion-collapse-widget [name=include-title], #data-dataset-table-accordion-collapse-widget [name=include-pagination], #data-dataset-table-accordion-collapse-widget [name=include-buttons], #data-dataset-table-accordion-collapse-widget [name=include-search], #data-dataset-table-accordion-collapse-widget [name=include-responsive]").bootstrapToggle('enable');

        if (app.data.RlsCode) {
            if (!app.data.isLive) {
                $("#data-dataset-table-accordion-collapse-widget [name=link-to-wip]").bootstrapToggle('enable');
            }
        }

        $("#data-dataset-table-accordion-collapse-widget").find("[name=title-value]").prop('disabled', false);
        $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").prop("disabled", false);
        $("#data-dataset-table-accordion-collapse-widget [name=add-custom-configuration]").prop("disabled", false);

        if ($("#data-dataset-table-accordion-collapse-widget [name=link-to-wip]").is(':checked')) {
            //disable download HTML button as this won't work with private api due to CORS rules
            $("#data-dataset-table-accordion-collapse-widget").find("[name=download-snippet]").prop('disabled', true);
            $("#data-dataset-table-accordion-collapse-widget").find("[name=preview-snippet]").prop('disabled', true);
        }
        else {
            $("#data-dataset-table-accordion-collapse-widget").find("[name=download-snippet]").prop('disabled', false);
            $("#data-dataset-table-accordion-collapse-widget").find("[name=preview-snippet]").prop('disabled', false);
        }

        //check for WIP
        if (!app.data.RlsCode) {
            $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('enable');
            $("#data-dataset-table-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('enable');
        }
        else {
            if (app.data.isLive) {
                $("#data-dataset-table-accordion-collapse-widget [name=auto-update]").bootstrapToggle('enable');
                $("#data-dataset-table-accordion-collapse-widget [name=fluid-time]").bootstrapToggle('enable');
            }
        }

        app.data.dataset.table.snippet.configuration = {};
        app.data.dataset.table.saveQuery.configuration = {};
        $.extend(true, app.data.dataset.table.snippet.configuration, app.data.dataset.table.snippet.template);
        //clone configuration to use in save query, we should always end up here the first time at least to 
        $.extend(true, app.data.dataset.table.saveQuery.configuration, app.data.dataset.table.snippet.configuration);

        app.data.dataset.table.snippet.configuration.autoupdate = $("#data-dataset-table-accordion-collapse-widget").find("[name=auto-update]").is(':checked');
        app.data.dataset.table.snippet.configuration.copyright = $("#data-dataset-table-accordion-collapse-widget").find("[name=include-copyright]").is(':checked');
        app.data.dataset.table.snippet.configuration.title = $("#data-dataset-table-accordion-collapse-widget").find("[name=include-title]").is(':checked') ? $("#data-dataset-table-accordion-collapse-widget").find("[name=title-value]").val().trim() : "";
        app.data.dataset.table.snippet.configuration.link = $("#data-dataset-table-accordion-collapse-widget").find("[name=include-link]").is(':checked') ? app.config.url.application + C_COOKIE_LINK_TABLE + "/" + app.data.MtrCode : null;
        app.data.dataset.table.snippet.configuration.pivot = app.data.dataset.table.pivot.dimensionCode;
        app.data.dataset.table.saveQuery.configuration.pivot = app.data.dataset.table.pivot.dimensionCode;
        app.data.dataset.table.snippet.configuration.options.search.search = app.data.dataset.table.search;
        app.data.dataset.table.snippet.configuration.options.order = app.data.dataset.table.order || [];
        app.data.dataset.table.saveQuery.configuration.options.order = app.data.dataset.table.order || [];

        //always make save queries responsive

        app.data.dataset.table.saveQuery.configuration.options.responsive = true;

        if (!$("#data-dataset-table-accordion-collapse-widget").find("[name=include-pagination]").is(':checked')) {
            app.data.dataset.table.snippet.configuration.options.paging = false;
            app.data.dataset.table.snippet.configuration.options.dom = "Bft";
        }

        if (!$("#data-dataset-table-accordion-collapse-widget").find("[name=include-buttons]").is(':checked')) {
            app.data.dataset.table.snippet.configuration.options.dom = app.data.dataset.table.snippet.configuration.options.dom.replace('B', '');
        }

        if (!$("#data-dataset-table-accordion-collapse-widget").find("[name=include-search]").is(':checked')) {
            app.data.dataset.table.snippet.configuration.options.dom = app.data.dataset.table.snippet.configuration.options.dom.replace('f', '');
        }

        if ($("#data-dataset-table-accordion-collapse-widget").find("[name=include-responsive]").is(':checked')) {
            app.data.dataset.table.snippet.configuration.options.responsive = true;
        }
        else {
            app.data.dataset.table.snippet.configuration.options.responsive = false;
        }

        $("#data-dataset-table-accordion-collapse-widget").find("[name=hide-column-checkbox-container] [name=hide-column]:checked").each(function () {
            app.data.dataset.table.snippet.configuration.hideColumns.push($(this).val());
        });

        //add query to save query config
        var JsonQuery = {
            "jsonrpc": C_APP_API_JSONRPC_VERSION,
            "method": "PxStat.Data.Cube_API.ReadDataset",
            "params": null
        };

        var widgetParams = $.extend(true, {}, app.data.dataset.table.apiParamsData);
        JsonQuery.params = widgetParams;
        JsonQuery.params.extension.format.type = C_APP_FORMAT_TYPE_DEFAULT;
        JsonQuery.params.extension.format.version = C_APP_FORMAT_VERSION_DEFAULT;

        if (app.data.isLive) {
            JsonQuery.params.extension.matrix = app.data.MtrCode;
            app.data.dataset.table.snippet.configuration.data.api.query.url = app.config.url.api.jsonrpc.public;
            app.data.dataset.table.saveQuery.configuration.data.api.query.url = app.config.url.api.jsonrpc.public;
            JsonQuery.method = "PxStat.Data.Cube_API.ReadDataset";
            delete JsonQuery.params.extension.release;
        }
        else {
            JsonQuery.params.extension.release = app.data.RlsCode;
            JsonQuery.method = "PxStat.Data.Cube_API.ReadPreDataset";
            delete JsonQuery.params.extension.matrix;
        }

        //add query to saved query object
        app.data.dataset.table.saveQuery.configuration.data.api.query.data = JsonQuery;


        if ($("#data-dataset-table-accordion-collapse-widget").find("[name=link-to-wip]").is(':checked')) {
            app.data.dataset.table.snippet.configuration.matrix = app.data.MtrCode;
            app.data.dataset.table.snippet.configuration.data.api.response = {};
            app.data.dataset.table.snippet.configuration.metadata.api.query = {};

            app.data.dataset.table.snippet.configuration.data.api.query.url = app.config.url.api.jsonrpc.private;

            app.data.dataset.table.snippet.configuration.data.api.response = {};

            //if link to WIP, then make query explicit so it is always a point in time query with no additional variables added at a later date because they selected all variables in initial query

            var linkToWipQuery = $.extend(true, {}, JsonQuery);
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
            app.data.dataset.table.snippet.configuration.data.api.query.data = linkToWipQuery;
        }

        else {
            //add query to widget snippet object if auto update
            if ($("#data-dataset-table-accordion-collapse-widget").find("[name=auto-update]").is(':checked')) {
                //add query to snippet object
                app.data.dataset.table.snippet.configuration.data.api.query.data = JsonQuery;

                if ($("#data-dataset-table-accordion-collapse-widget").find("[name=fluid-time]").is(':checked')) {
                    app.data.dataset.table.snippet.configuration = app.data.dataset.table.callback.setFluidTime(app.data.dataset.table.snippet.configuration);
                    app.data.dataset.table.snippet.configuration.metadata.api.query.data.params.matrix = app.data.MtrCode;
                    app.data.dataset.table.snippet.configuration.metadata.api.query.url = app.config.url.api.jsonrpc.public;
                }
                else {
                    app.data.dataset.table.snippet.configuration.metadata.api.query = {};
                }

            }
            else {
                app.data.dataset.table.snippet.configuration.data.api.response = app.data.dataset.table.response;
                app.data.dataset.table.snippet.configuration.metadata.api.query = {};
            }
        }

    }

    else {
        $("#data-dataset-table-accordion-collapse-widget").find("[name=make-selection-message]").show();
        $("#data-dataset-table-accordion-collapse-widget").find("[name=widget-snippet-information]").hide();
    }

    //remove date if not live, can be present if prending live
    if (!app.data.isLive) {
        delete app.data.dataset.table.snippet.configuration.data.api.response.updated;
    }

    //add custom config if something there
    if ($("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val().trim().length) {
        try {
            app.data.dataset.table.callback.formatJson();
            var customOptions = JSON.parse($("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val().trim());
            $.extend(true, app.data.dataset.table.snippet.configuration, customOptions);
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
    };
};

app.data.dataset.table.callback.setFluidTime = function (configuration) {
    var timeValuesSelected = [];
    var dimensionSize = app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id.length;
    //check for time variables selected
    if (configuration.data.api.query.data.params.dimension[app.data.dataset.metadata.timeDimensionCode]) {
        timeValuesSelected = configuration.data.api.query.data.params.dimension[app.data.dataset.metadata.timeDimensionCode].category.index;
        $.each(timeValuesSelected, function (index, value) {
            //get position of variables selected releative to the end of the array as new time points are added to the end of the array
            var actualPosition = $.inArray(value, app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id);
            var relativePosition = (dimensionSize - actualPosition) - 1;
            configuration.fluidTime.push(relativePosition);
        });
    }
    else {
        //get positions of all variables as fluid is true and everything is selected
        //get position of variables selected releative to the end of the array as new time points are added to the end of the array
        $.each(app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id, function (index, value) {
            configuration.fluidTime.push(index);
        });
    }
    return configuration;
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
};

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
            "class": $('#data-dataset-table-code-toggle').is(':checked') ? "badge rounded-pill bg-neutral text-dark mx-2 fw-bold d-none" : "badge rounded-pill bg-neutral text-dark mx-2 fw-bold",
            "text": data.id[i]
        }).get(0).outerHTML;

        var tableHeading = $("<th>", {
            "html": codeSpan + data.Dimension(i).label
        });

        $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=header-row]").append(tableHeading);


        if (data.Dimension(data.id[i]).role == "time") {
            app.data.dataset.table.timeColumn = i;
        };

        // Append datatable column
        tableColumns.push({
            data: data.id[i],
            "visible": data.id[i] == app.data.dataset.table.pivot.dimensionCode ? false : true,
            "searchable": data.id[i] == app.data.dataset.table.pivot.dimensionCode ? false : true,
            "orderable": data.Dimension(data.id[i]).role == "metric" ? false : true,
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
                            "class": $('#data-dataset-table-code-toggle').is(':checked') ? "badge rounded-pill bg-neutral text-dark mx-2 d-none" : "badge rounded-pill bg-neutral text-dark mx-2",
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
            "data": "unit.label"
        });
    }

    // Populate Pivoted columns
    if (app.data.dataset.table.pivot.dimensionCode) {
        $.each(app.data.dataset.table.pivot.variableCodes, function (index, value) {
            tableColumns.push({
                "data": value,
                "type": "data",
                "class": "text-end fw-bold",
                "defaultContent": app.config.entity.data.datatable.null,
                "render": function (cell, type, row, meta) {
                    // If pivoting by Statitic then the decimals may be different within the same row
                    //patch re https://github.com/CSOIreland/PxStat/issues/537
                    if (app.data.dataset.table) {
                        return app.library.utility.formatNumber(cell, (app.data.dataset.table.pivot.isMetric ? data.Dimension(app.data.dataset.table.pivot.dimensionCode).Category(value).unit.decimals : row.unit.decimals));
                    }
                    else {
                        return null
                    }
                }
            });
            var codeSpan = $('<span>', {
                "name": "code",
                "class": $('#data-dataset-table-code-toggle').is(':checked') ? "badge rounded-pill bg-neutral text-dark mx-2 d-none" : "badge rounded-pill bg-neutral text-dark mx-2",
                "text": value
            }).get(0).outerHTML;

            // If pivoting by Statistic then concatenate the Unit to the Heading
            var pivotHeading = $("<th>",
                {
                    "html": data.Dimension(app.data.dataset.table.pivot.dimensionCode).Category(value).label
                        + (app.data.dataset.table.pivot.isMetric ? " (" + data.Dimension(app.data.dataset.table.pivot.dimensionCode).Category(value).unit.label + ")" : "")
                        + codeSpan,
                    "class": "text-end text-light bg-primary"
                });

            $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=header-row]").append(pivotHeading);
        });
    } else {
        tableColumns.push({
            "data": "value",
            "type": "data",
            "class": "text-end fw-bold",
            "defaultContent": app.config.entity.data.datatable.null,
            "render": function (cell, type, row, meta) {
                return app.library.utility.formatNumber(cell, row.unit.decimals);
            }
        });

        var valueHeading = $("<th>",
            {
                "html": app.label.static["value"],
                "class": "text-end text-light bg-primary"
            });
        $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=header-row]").append(valueHeading);
    }

    //Draw DataTable with Data Set data
    var localOptions = {
        iDisplayLength: app.config.entity.data.datatable.length,
        order: [],
        data: jsonTable.data,
        columns: tableColumns,
        drawCallback: function (settings) {
            app.data.dataset.table.drawCallbackDrawDataTable();
        },
        //Translate labels language
        language: app.label.plugin.datatable
    };

    //Set the default ordering to the time column if it exists, may be pivoted by time
    if (app.data.dataset.table.pivot.dimensionCode) {
        if (data.Dimension(app.data.dataset.table.pivot.dimensionCode).role == "time") {
            localOptions.order = [];
        }
        else {
            localOptions.order = [[app.data.dataset.table.timeColumn, "desc"]];
        }

    }
    else {
        localOptions.order = [[app.data.dataset.table.timeColumn, "desc"]];
    }


    // Initiate DataTable
    $("#data-dataset-table-nav-content").find("[name=datatable]").off().DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
        app.data.dataset.table.drawCallbackDrawDataTable();
    });

    $("#data-dataset-table-nav-content").find("[name=datatable]").on('order.dt', function (e) {
        app.data.dataset.table.order = $("#data-dataset-table-nav-content").find("[name=datatable]").DataTable().order();
        app.data.dataset.table.callback.drawSnippetCode(true);
    });

    $("#data-dataset-table-nav-content").find("[name=datatable]").on('search.dt', function () {
        app.data.dataset.table.search = $("#data-dataset-table-nav-content").find("[name=datatable]").DataTable().search();
        app.data.dataset.table.callback.drawSnippetCode(true);
    });

    $('[data-bs-toggle="tooltip"]').tooltip();

    //scroll to top of datatable -- Data entity

    if (app.data.isModal) {
        if (!app.data.isLive) { //is WIP
            if (!app.config.entity.data.display.wipWidgetStandard) {
                //hide widget 
                $("#data-dataset-table-accordion-collapse-widget").find('[name="widget-hidden-wrapper"]').show();
                $("#data-dataset-table-accordion-collapse-widget").find('[name="widget-display-wrapper"]').hide();
            }
            else {
                //show widget 
                $("#data-dataset-table-accordion-collapse-widget").find('[name="widget-hidden-wrapper"]').hide();
                $("#data-dataset-table-accordion-collapse-widget").find('[name="widget-display-wrapper"]').show();
            }
        }
        else {//live table
            //show widget 
            $("#data-dataset-table-accordion-collapse-widget").find('[name="widget-hidden-wrapper"]').hide();
            $("#data-dataset-table-accordion-collapse-widget").find('[name="widget-display-wrapper"]').show();
        }
        $("#data-dataset-table-result").find("[name=save-query-wrapper]").hide();
        $('#data-view-modal').animate({
            scrollTop: '+=' + $('#data-dataset-table-result-tabs')[0].getBoundingClientRect().top
        },
            1000);
    }
    else {
        $("#data-dataset-table-result").find("[name=save-query-wrapper]").show();
        $('html, body').animate({
            scrollTop: $("#data-dataset-table-result-tabs").offset().top
        }, 1000);
    }

    app.data.dataset.table.callback.drawHiddenColumsCheckBoxes();
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

    var spliceOffset = 0;

    $.each(arrobjTable.data, function (indexData, rowData) {
        // Get all values to pivot
        if ($.inArray(rowData[app.data.dataset.table.pivot.dimensionCode], app.data.dataset.table.pivot.variableCodes) == -1) {
            app.data.dataset.table.pivot.variableCodes.push(rowData[app.data.dataset.table.pivot.dimensionCode]);
        }

        // Reduce the data by the pivot size
        if (rowData[app.data.dataset.table.pivot.dimensionCode] != app.data.dataset.table.pivot.variableCodes[0]) {
            reducedTable.data.splice(indexData - spliceOffset, 1);
            pivotedTable.data.splice(indexData - spliceOffset, 1);
            spliceOffset++;
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
    if (!$('#data-dataset-table-code-toggle').prop('checked')) {
        $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=code]").removeClass("d-none");
    }
    else {
        $("#data-dataset-table-nav-content").find("[name=datatable]").find("[name=code]").addClass("d-none");
    }


    api.spinner.stop();
    //default to standard results
    $('#data-dataset-table-result-tab-standard').tab('show');
    $("#data-dataset-table-result-wrapper").fadeIn();

    setTimeout(function () {
        $("#data-dataset-table-result-tabs").find("i.fa-bell").removeClass("fa-beat-fade text-danger");
    }, 5000)

};

app.data.dataset.table.resultsDownload = function (format, version) {
    var apiParams = $.extend(true, {}, app.data.dataset.table.apiParamsData);
    apiParams.extension.format.type = format;
    apiParams.extension.format.version = version;
    app.data.dataset.ajax.downloadDataset(apiParams, app.data.dataset.table.selectionCount);
};

//#region save query
app.data.dataset.table.saveQuery.drawSaveQueryModal = function () {
    app.data.dataset.saveQuery.validation.drawSaveQuery();
    $("#data-dataset-save-query").modal("show");
};

app.data.dataset.table.saveQuery.ajax.saveQuery = function () {

    //now clone it into new variable for ajax which may or may not need fluid time
    var saveQueryConfig = $.extend(true, {}, app.data.dataset.table.saveQuery.configuration);
    if ($("#data-dataset-save-query").find("[name=fluid-time]").is(':checked')) {
        saveQueryConfig = app.data.dataset.table.callback.setFluidTime(saveQueryConfig);
        saveQueryConfig.metadata.api.query.data.params.matrix = app.data.MtrCode;
        saveQueryConfig.metadata.api.query.url = app.config.url.api.jsonrpc.public;
    }
    else {
        saveQueryConfig.metadata.api.query = {};
        saveQueryConfig.fluidTime = [];
    }

    saveQueryConfig.options.search.search = $("#data-dataset-table-nav-content").find("[name=datatable]").DataTable().search();
    saveQueryConfig.options.order = $("#data-dataset-table-nav-content").find("[name=datatable]").DataTable().order();
    saveQueryConfig.title = "";

    var tagName = $("#data-dataset-save-query").find("[name=name]").val().replace(C_APP_REGEX_NOHTML, "");
    var base64snippet = nacl.util.encodeBase64(nacl.util.decodeUTF8(JSON.stringify(saveQueryConfig)));

    if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess) {
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.private,
            "PxStat.Subscription.Query_API.Create",
            {
                "TagName": tagName,
                "Matrix": app.data.MtrCode,
                "Snippet": {
                    "Type": C_APP_PXWIDGET_TYPE_TABLE,
                    "Query": base64snippet,
                    "FluidTime": $("#data-dataset-save-query").find("[name=fluid-time]").is(':checked'),
                    "Isogram": C_APP_URL_PXWIDGET_ISOGRAM
                }
            },
            "app.data.dataset.table.saveQuery.callback.saveQuery",
            tagName
        );
    }
    else if (app.navigation.user.isSubscriberAccess) {
        app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
            api.ajax.jsonrpc.request(
                app.config.url.api.jsonrpc.private,
                "PxStat.Subscription.Query_API.Create",
                {
                    "Uid": app.auth.firebase.user.details.uid,
                    "AccessToken": accessToken,
                    "TagName": tagName,
                    "Matrix": app.data.MtrCode,
                    "Snippet": {
                        "Type": C_APP_PXWIDGET_TYPE_TABLE,
                        "Query": base64snippet,
                        "FluidTime": $("#data-dataset-save-query").find("[name=fluid-time]").is(':checked'),
                        "Isogram": C_APP_URL_PXWIDGET_ISOGRAM
                    }
                },
                "app.data.dataset.table.saveQuery.callback.saveQuery",
                tagName
            );
        }).catch(tokenerror => {
            api.modal.error(tokenerror);
        });
    }
};

app.data.dataset.table.saveQuery.callback.saveQuery = function (data, tagName) {
    $("#data-dataset-save-query").modal("hide");
    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [tagName]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};
//#endregion
