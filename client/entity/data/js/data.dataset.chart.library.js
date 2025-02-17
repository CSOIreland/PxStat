/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
app.data = app.data || {};
app.data.dataset = app.data.dataset || {};
app.data.dataset.chart = {};
app.data.dataset.chart.seriesId = 0;
app.data.dataset.chart.ajax = {};
app.data.dataset.chart.callback = {};
app.data.dataset.chart.validation = {};
app.data.dataset.chart.configuration = {};
app.data.dataset.chart.saveQuery = {};
app.data.dataset.chart.saveQuery.configuration = {};
app.data.dataset.chart.saveQuery.ajax = {};
app.data.dataset.chart.saveQuery.callback = {};
app.data.dataset.chart.snippetConfig = {

};
app.data.dataset.chart.template = {};
app.data.dataset.chart.template.wrapper = {
    "autoupdate": null,
    "matrix": null,
    "type": null,
    "showPercentage": null,
    "copyright": false,
    "link": null,
    "sort": false,
    "maxNumberValues": null,
    "metadata": {},
    "data": {
        "labels": [],
        "datasets": [],
        "null": app.config.entity.data.datatable.null
    },
    "options": app.config.plugin.chartJs.chart.options,
    "datasetLabels": []
};

app.data.dataset.chart.template.metadata = {
    "xAxis": {},
    "fluidTime": [],
    "api": {
        "query": {
            "url": null,
            "data": {
                "jsonrpc": C_APP_API_JSONRPC_VERSION,
                "method": null,
                "params": {
                    "matrix": null,
                    "release": null,
                    "language": null,
                    "format": {
                        "type": C_APP_FORMAT_TYPE_DEFAULT,
                        "version": C_APP_FORMAT_VERSION_DEFAULT
                    }
                },
                "version": C_APP_API_JSONRPC_VERSION
            }
        },
        "response": {}
    }
}
app.data.dataset.chart.template.dataset = {
    "label": null,
    "pointRadius": 2,
    "pointHoverRadius": 4,
    "maxBarThickness": 90,
    "minBarLength": 1,
    "api": {
        "query": {
            "url": null,
            "data": {
                "jsonrpc": C_APP_API_JSONRPC_VERSION,
                "method": null,
                "params": {
                    "class": C_APP_JSONSTAT_QUERY_CLASS,
                    "id": [],
                    "dimension": {},
                    "extension": {
                        "language": {
                            "code": null
                        },
                        "format": {
                            "type": C_APP_TS_FORMAT_TYPE_JSONSTAT,
                            "version": C_APP_FORMAT_VERSION_DEFAULT
                        },
                        "matrix": null,
                        "release": null,
                        "m2m": false
                    },
                    "version": C_APP_API_JSONRPC_VERSION
                }
            }
        },
        "response": {}
    },
    "data": [],
    "unit": [],
    "decimal": [],
    "fluidTime": [],
    "fill": false
}

app.data.dataset.chart.template.yAxisRight = {
    //move this to top
    "display": true,
    "type": 'linear',
    "position": 'right',
    "id": C_APP_PXWIDGET_CHART_TYPES_DUAL_POSITION[1],
    "gridLines": {
        "drawOnChartArea": false, // only want the grid lines for one axis to show up
    },
    "ticks": {
        "beginAtZero": false,
        "callback": null
    },
    "scaleLabel": {
        "display": false,
        "labelString": null
    }
}

//#endregion

//#region get chart types
app.data.dataset.chart.getChartTypes = function () {
    var chartTypes = [];
    $.each(C_APP_PXWIDGET_CHART_TYPES, function (index, value) {
        chartTypes.push(
            {
                "id": value,
                "text": app.label.static[value]
            }
        );
    });
    $("#data-dataset-chart-properties").find("[name=type]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: chartTypes,
        dropdownParent: $("#data-dataset-chart-properties")
    }).on('select2:select', function (e) {
        switch ($(this).val()) {
            case "pie":
            case "doughnut":
                $("#data-dataset-chart-accordion-options-collapse [name=show-percentage-row]").show();
            //no break as we want to move to the next case regardless
            case "pie":
            case "doughnut":
            case "polarArea":
            case "radar":
                $("#data-dataset-chart-accordion-series-collapse").find("[name=series-toggles]").hide();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=series-stacked]").hide();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=sort-row]").hide();
                $("#data-dataset-chart-accordion-options-collapse [name=xaxis-max-steps]").val("");
                $("#data-dataset-chart-accordion-options-collapse [name=auto-scale-row], #data-dataset-chart-accordion-options-collapse [name=xaxis-max-steps-row]").hide();
                $("#data-dataset-chart-accordion-series-collapse [name=series-dual-axis], #data-dataset-chart-accordion-xaxis-collapse [name=x-axis-label-row]").hide();
                $("#data-dataset-chart-accordion-options-collapse [name=curved-line-row]").hide();
                $("#data-dataset-chart-accordion-xaxis-heading").find("[name=accordion-xaxis-heading]").text(app.label.static["categories"]);
                break;
            case "mixed":
                $("#data-dataset-chart-accordion-series-collapse").find("[name=series-toggles]").show();
                $("#data-dataset-chart-accordion-xaxis-collapse [name=x-axis-label-row]").show();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=series-stacked]").hide();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=sort-row]").hide();
                $("#data-dataset-chart-accordion-xaxis-heading").find("[name=accordion-xaxis-heading]").text(app.label.static["x-axis"]);
                $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label-holder]").text(app.label.static["x-axis-label"]);
                $("#data-dataset-chart-accordion-options-collapse [name=curved-line-row]").show();
                $("#data-dataset-chart-accordion-options-collapse [name=show-percentage-row]").hide();
                $("#data-dataset-chart-accordion-series-collapse").find("[name=y-axis-left-label-holder]").text(app.label.static["y-axis-left"]);
                break;
            case "line":
                $("#data-dataset-chart-accordion-series-collapse").find("[name=series-toggles]").show();
                $("#data-dataset-chart-accordion-xaxis-collapse [name=x-axis-label-row]").show();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=series-stacked]").hide();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=sort-row]").show();
                $("#data-dataset-chart-accordion-xaxis-heading").find("[name=accordion-xaxis-heading]").text(app.label.static["x-axis"]);
                $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label-holder]").text(app.label.static["x-axis-label"]);
                $("#data-dataset-chart-accordion-options-collapse [name=curved-line-row]").show();
                $("#data-dataset-chart-accordion-options-collapse [name=show-percentage-row]").hide();
                $("#data-dataset-chart-accordion-series-collapse").find("[name=y-axis-left-label-holder]").text(app.label.static["y-axis-left"]);
                break;
            case "area":
                $("#data-dataset-chart-accordion-series-collapse").find("[name=series-toggles]").show();
                $("#data-dataset-chart-accordion-xaxis-collapse [name=x-axis-label-row]").show();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=series-stacked]").hide();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=sort-row]").show();
                $("#data-dataset-chart-accordion-xaxis-heading").find("[name=accordion-xaxis-heading]").text(app.label.static["x-axis"]);
                $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label-holder]").text(app.label.static["x-axis-label"]);
                $("#data-dataset-chart-accordion-options-collapse [name=curved-line-row]").show();
                $("#data-dataset-chart-accordion-options-collapse [name=show-percentage-row]").hide();
                $("#data-dataset-chart-accordion-series-collapse").find("[name=y-axis-left-label-holder]").text(app.label.static["y-axis-left"]);
                break;
            case "bar":
                $("#data-dataset-chart-accordion-series-collapse").find("[name=series-toggles]").show();
                $("#data-dataset-chart-accordion-xaxis-collapse [name=x-axis-label-row]").show();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=series-stacked]").show();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=sort-row]").show();
                $("#data-dataset-chart-accordion-xaxis-heading").find("[name=accordion-xaxis-heading]").text(app.label.static["x-axis"]);
                $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label-holder]").text(app.label.static["x-axis-label"]);
                $("#data-dataset-chart-accordion-options-collapse [name=curved-line-row]").hide();
                $("#data-dataset-chart-accordion-options-collapse [name=show-percentage-row]").hide();
                $("#data-dataset-chart-accordion-series-collapse").find("[name=y-axis-left-label-holder]").text(app.label.static["y-axis-left"]);
                break;
            case "horizontalBar":
            case "pyramid":
                $("#data-dataset-chart-accordion-series-collapse").find("[name=series-toggles]").show();
                $("#data-dataset-chart-accordion-xaxis-collapse [name=x-axis-label-row]").show();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=series-stacked]").show();
                $("#data-dataset-chart-accordion-options-collapse").find("[name=sort-row]").show();
                $("#data-dataset-chart-accordion-series-collapse").find("[name=series-dual-axis], [name=right-yaxis-label-row]").hide();
                $("#data-dataset-chart-accordion-xaxis-heading").find("[name=accordion-xaxis-heading]").text(app.label.static["y-axis"]);
                $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label-holder]").text(app.label.static["y-axis-left"]);
                $("#data-dataset-chart-accordion-options-collapse [name=curved-line-row]").hide();
                $("#data-dataset-chart-accordion-options-collapse [name=show-percentage-row]").hide();
                $("#data-dataset-chart-accordion-series-collapse").find("[name=y-axis-left-label-holder]").text(app.label.static["x-axis-label"]);
                break;
            default:

                break;
        }

        app.data.dataset.chart.buildXAxisSelect();

    }).on('select2:clear', function (e) {
        $("#data-dataset-chart-accordion-series-collapses").find("[name=series-toggles]").hide();
    }).prop("disabled", false);

    $("#data-dataset-chart-properties").find("[name=build-chart]").prop("disabled", false)
}
//#endregion get chart types

app.data.dataset.chart.setLegendPosition = function () {
    $("#data-dataset-chart-accordion-options-collapse [name=legend-position]").empty();
    $.each(C_APP_PXWIDGET_CHART_LEGEND_POSITION, function (index, value) {
        $("#data-dataset-chart-accordion-options-collapse [name=legend-position]").append($("<option>", {
            "value": value,
            "text": app.label.static[value]
        }))
    });
    //append none 
    $("#data-dataset-chart-accordion-options-collapse [name=legend-position]").append($("<option>", {
        "text": app.label.static["none"]
    }))
    $("#data-dataset-chart-accordion-options-collapse [name=legend-position]").prop("disabled", false);
    $("#data-dataset-chart-accordion-options-collapse [name=legend-position]").val(app.config.plugin.chartJs.chart.options.legend.position);

    $("#data-dataset-chart-accordion-options-collapse [name=legend-position]").once('change', function () {
        app.data.dataset.chart.resetChart();
    });
}



//#region xAxis
app.data.dataset.chart.buildXAxisSelect = function () {
    $("#data-dataset-chart-properties").find("[name=build-chart], [name=type]").prop("disabled", true)
    $("#data-dataset-chart-accordion").show();
    $("#data-dataset-chart-accordion-xaxis-collapse").collapse('show');

    $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").empty();

    $.each(app.data.dataset.metadata.jsonStat.Dimension({ role: "metric" }), function (index, value) {
        var dimensionContainer = $("#data-dataset-chart-templates").find("[name=dimension-container]").clone();

        dimensionContainer.find("[name=dimension-label]").html(value.label);
        dimensionContainer.find("[name=dimension-count]").text(value.length);
        dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]);
        dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]);
        dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]);

        $.each(value.id, function (variableIndex, variable) {
            var option = $('<option>', {
                "value": variable,
                "title": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
            });

            dimensionContainer.find("select").append(option);
        });
        dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]).attr("role", value.role);

        $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);

    });

    $.each(app.data.dataset.metadata.jsonStat.Dimension({ role: "time" }), function (index, value) {
        var dimensionContainer = $("#data-dataset-chart-templates").find("[name=dimension-container]").clone();

        dimensionContainer.find("[name=dimension-label]").html(value.label);
        dimensionContainer.find("[name=dimension-count]").text(value.length);
        dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]);
        dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]);
        dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]);

        $.each(value.id, function (variableIndex, variable) {
            var option = $('<option>', {
                "value": variable,
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

        $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);

    });

    $.each(app.data.dataset.metadata.jsonStat.Dimension(), function (index, value) {
        if (value.role != "metric" && value.role != "time") {
            var dimensionContainer = $("#data-dataset-chart-templates").find("[name=dimension-container]").clone();

            dimensionContainer.find("[name=dimension-label]").html(value.label);
            dimensionContainer.find("[name=dimension-count]").text(value.length);
            dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]);
            dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]);
            dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]);

            $.each(value.id, function (variableIndex, variable) {
                var option = $('<option>', {
                    "value": variable,
                    "title": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                    "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                });

                dimensionContainer.find("select").append(option);
            });
            dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("role", value.role);

            $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
        }
    });
    //unselect any options from another dimension
    $("#data-dataset-chart-accordion-xaxis-collapse").find("select").once('change', function () {
        var xAxisSelected = false;
        var selectedDimensionId = $(this).attr("idn");
        var selectedOptions = $(this).find("option:selected").length;
        var totalOptions = $(this).find("option").length;
        if (selectedOptions == totalOptions) {
            $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=select-all][idn='" + selectedDimensionId + "']").prop('checked', true);
        }
        else {
            $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=select-all][idn='" + selectedDimensionId + "']").prop('checked', false);
        }
        $("#data-dataset-chart-accordion-xaxis-collapse").find("select").each(function () {
            if ($(this).find("option:selected").length) {
                xAxisSelected = true
            }
            var thisDimensionId = $(this).attr("idn");
            if (selectedDimensionId != thisDimensionId) {
                $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=select-all][idn='" + thisDimensionId + "']").prop('checked', false);
                $(this).find("option:selected").each(function () {
                    $(this).prop("selected", false);
                });

            }


        });
        if (xAxisSelected) {
            $("#data-dataset-chart-accordion-series-heading button").prop("disabled", false);
        }
        else {
            $("#data-dataset-chart-accordion-series-heading button").prop("disabled", true);
        }
        app.data.dataset.chart.resetChart();
    });


    $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-filter]").once("keyup search", function () {
        var dimensionCode = $(this).attr("idn");
        var thisDimension = app.data.dataset.metadata.jsonStat.Dimension(dimensionCode);

        var select = $("#data-dataset-chart-accordion-xaxis-collapse").find("select[idn='" + dimensionCode + "']");
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
                    if (select.attr("role") == "time") {
                        select.prepend($('<option>', {
                            "value": key,
                            "title": value,
                            "text": thisDimension.Category(key).label + (thisDimension.Category(key).unit ? " (" + thisDimension.Category(key).unit.label + ")" : "")
                        }));
                    }
                    else {
                        select.append($('<option>', {
                            "value": key,
                            "title": value,
                            "text": thisDimension.Category(key).label + (thisDimension.Category(key).unit ? " (" + thisDimension.Category(key).unit.label + ")" : "")
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
            $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=select-all][idn='" + dimensionCode + "']").prop("disabled", true);
        }
        else {
            //enable select all
            $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=select-all][idn='" + dimensionCode + "']").prop("disabled", false);

        }

    });

    //sort
    $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=sort-options]").once("click", function () {
        var dimension = $(this).attr("idn");
        var select = $("#data-dataset-chart-accordion-xaxis-collapse").find("select[idn='" + dimension + "']");
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

    $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=select-all]").once("change", function () {
        var selectedDimensionId = $(this).attr("idn");
        var select = $("#data-dataset-chart-accordion-xaxis-collapse").find("select[idn='" + selectedDimensionId + "']");

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

        //unselect all other dimensions and select all checkbok
        $("#data-dataset-chart-accordion-xaxis-collapse").find("select").each(function () {
            var thisDimensionId = $(this).attr("idn");
            if (selectedDimensionId != thisDimensionId) {
                $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=select-all][idn='" + thisDimensionId + "']").prop('checked', false);
                $(this).find("option:selected").each(function () {
                    $(this).prop("selected", false);
                });
            }
        });

        var xAxisSelected = false;

        if (select.find("option:selected").length) {
            xAxisSelected = true
        }
        if (xAxisSelected) {
            $("#data-dataset-chart-accordion-series-heading button").prop("disabled", false);
        }
        else {
            $("#data-dataset-chart-accordion-series-heading button").prop("disabled", true);
        }
    });
    if (app.data.isModal) {
        $("#data-view-modal").animate({
            scrollTop: '+=' + $('#data-dataset-chart-accordion')[0].getBoundingClientRect().top
        }, 1000);
    }
    else {
        $('html, body').animate({
            scrollTop: $("#data-dataset-chart-accordion").offset().top
        }, 1000);
    }
}
//#endregion x Axis

//#region  series
app.data.dataset.chart.addSeries = function () {
    app.data.dataset.chart.resetChart();
    //lock xaxis to not be allowed change dimension
    $("#data-dataset-chart-accordion-xaxis-collapse").find("select").each(function () {
        var thisDimensionId = $(this).attr("idn");
        if (!$(this).find("option:selected").length) {
            $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=select-all][idn='" + thisDimensionId + "'],[name=dimension-filter][idn='" + thisDimensionId + "']").prop("disabled", true);
            $(this).prop("disabled", true)
        }

    });

    //increment seriesId
    app.data.dataset.chart.seriesId++;
    var tabId = "data-dataset-chart-accordion-series-collapse-" + app.data.dataset.chart.seriesId + "-tab";
    var contentId = "data-dataset-chart-accordion-series-collapse-" + app.data.dataset.chart.seriesId + "-content";

    var seriesTab = $("#data-dataset-chart-templates").find("[name=series-tab]").clone();
    seriesTab.attr("series", "series-" + app.data.dataset.chart.seriesId);
    seriesTab.find("a").attr("id", tabId).attr("href", "#" + contentId).attr("aria-controls", contentId);

    var tabContent = $("#data-dataset-chart-templates").find("[name=tab-content]").clone();

    //if ($("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis]").is(":visible")) {
    app.data.dataset.chart.drawDualAxis(tabContent.find("[name=dual-axis-position]"));
    // }

    if ($("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis]").is(':checked')) {

        tabContent.find("[name=dual-axis-position]").prop("disabled", false).val("y-axis-1");
    }
    else {
        tabContent.find("[name=dual-axis-position]").prop("disabled", true);
    }

    switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
        case "pie":
        case "doughnut":
        case "polarArea":
            tabContent.find("[name=dual-axis-holder]").hide();
            break;
        case "mixed":
            tabContent.find("[name=mixed-select-holder]").show();
            $.each(C_APP_PXWIDGET_CHART_TYPES_MIXED, function (index, value) {
                tabContent.find("[name=mixed]").append($("<option>", {
                    "value": value,
                    "text": app.label.static[value]

                }))
            });
            tabContent.find("[name=mixed]").prop("disabled", false);
            break;

        default:
            tabContent.find("[name=series-toggles]").show();
            break;
    }



    tabContent.attr("id", contentId).attr("aria-labelledby", tabId).attr("series", "series-" + app.data.dataset.chart.seriesId);
    tabContent.find("[name=delete-series]").attr("series", "series-" + app.data.dataset.chart.seriesId);

    var xAxisDimensionCode = $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").find("select:enabled").attr("idn");

    $.each(app.data.dataset.metadata.jsonStat.Dimension({ role: "metric" }), function (index, value) {

        if (app.data.dataset.metadata.jsonStat.role.metric[index] != xAxisDimensionCode) {
            var dimensionContainer = $("#data-dataset-chart-templates").find("[name=dimension-container]").clone();
            dimensionContainer.find("select").removeAttr("multiple").attr("series", "series-" + app.data.dataset.chart.seriesId).attr("dimension-name", value.label);
            dimensionContainer.find("[name=select-all]").remove();

            dimensionContainer.find("[name=dimension-label]").html(value.label).addClass("mandatory");
            dimensionContainer.find("[name=dimension-count]").text(value.length);
            dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);
            dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);
            dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);


            dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]).attr("role", value.role);
            dimensionContainer.find("select").attr("name", "select-" + app.data.dataset.metadata.jsonStat.role.metric[index]).attr("role", value.role);

            $.each(value.id, function (variableIndex, variable) {
                var option = $('<option>', {
                    "value": variable,
                    "title": value.Category(variableIndex).label,
                    "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                });
                if (value.id.length == 1) {
                    option.attr("selected", "selected")
                }
                dimensionContainer.find("select").append(option);
            });

            tabContent.find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
        }
    });

    $.each(app.data.dataset.metadata.jsonStat.Dimension({ role: "time" }), function (index, value) {

        if (app.data.dataset.metadata.jsonStat.role.time[index] != xAxisDimensionCode) {
            var dimensionContainer = $("#data-dataset-chart-templates").find("[name=dimension-container]").clone();
            dimensionContainer.find("select").removeAttr("multiple").attr("series", "series-" + app.data.dataset.chart.seriesId).attr("dimension-name", value.label);
            dimensionContainer.find("[name=select-all]").remove();

            dimensionContainer.find("[name=dimension-label]").html(value.label).addClass("mandatory");
            dimensionContainer.find("[name=dimension-count]").text(value.length);
            dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);
            dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);
            dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);


            dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]).attr("role", value.role);
            dimensionContainer.find("select").attr("name", "select-" + app.data.dataset.metadata.jsonStat.role.time[index]).attr("role", value.role);

            $.each(value.id, function (variableIndex, variable) {
                var option = $('<option>', {
                    "value": variable,
                    "title": value.Category(variableIndex).label,
                    "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                });
                if (value.id.length == 1) {
                    option.attr("selected", "selected")
                }
                dimensionContainer.find("select").append(option);
            });

            if (value.role == "time") {
                //reverse select based on codes so most recent time first
                dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
                    return $(x).val() < $(y).val() ? 1 : -1;
                }));
            };

            tabContent.find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
        }
    });

    $.each(app.data.dataset.metadata.jsonStat.Dimension(), function (index, value) {
        if (value.role != "metric" && value.role != "time") {
            if (app.data.dataset.metadata.jsonStat.id[index] != xAxisDimensionCode) {
                var dimensionContainer = $("#data-dataset-chart-templates").find("[name=dimension-container]").clone();
                dimensionContainer.find("select").removeAttr("multiple").attr("series", "series-" + app.data.dataset.chart.seriesId).attr("dimension-name", value.label);
                dimensionContainer.find("[name=select-all]").remove();

                dimensionContainer.find("[name=dimension-label]").html(value.label).addClass("mandatory");
                dimensionContainer.find("[name=dimension-count]").text(value.length);
                dimensionContainer.find("[name=select-all]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);
                dimensionContainer.find("[name=sort-options]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);
                dimensionContainer.find("[name=dimension-filter]").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("series", "series-" + app.data.dataset.chart.seriesId);


                dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("role", value.role);
                dimensionContainer.find("select").attr("name", "select-" + app.data.dataset.metadata.jsonStat.id[index]).attr("role", value.role);

                $.each(value.id, function (variableIndex, variable) {
                    var option = $('<option>', {
                        "value": variable,
                        "title": value.Category(variableIndex).label,
                        "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                    });
                    if (value.id.length == 1) {
                        option.attr("selected", "selected")
                    }
                    dimensionContainer.find("select").append(option);
                });

                if (value.role == "time") {
                    //reverse select based on codes so most recent time first
                    dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
                        return $(x).val() < $(y).val() ? 1 : -1;
                    }));
                };

                tabContent.find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
            }
        };

    });




    //Make this tab the active one
    $("#data-dataset-chart-accordion-series-collapse").find("[name=series-tab-link]").removeClass("active").attr("aria-selected", "false");
    $("#data-dataset-chart-accordion-series-collapse").find("[name=tab-content]").removeClass("show").removeClass("active");
    $("#" + tabId + "").addClass("active").attr("aria-selected", "true");
    $("#" + contentId + "").addClass("show").addClass("active");

    $("#data-dataset-chart-accordion-series-collapse").find("[name=series-tabs]").append(seriesTab);

    $("#data-dataset-chart-accordion-series-collapse").find("[name=series-content]").append(tabContent);




    //Give series a name
    var seriesName = app.label.static["series"] + " " + app.data.dataset.chart.seriesId;
    tabContent.find("[name=name]").val(seriesName);
    $("#data-dataset-chart-accordion-series-collapse").find("[name=series-tabs]").find("[series=series-" + app.data.dataset.chart.seriesId + "] a").text(
        seriesName
    );


    $("#data-dataset-chart-accordion").find("[name=delete-series]").once("click", function () {
        app.data.dataset.chart.deleteSeries($(this).attr("series"));
        //if not stacked
        if (!$("#data-dataset-chart-accordion-options-collapse").find("[name=stacked]").is(':checked')) {
            if ($("#data-dataset-chart-accordion-series-collapse [name=tab-content]").length > 1) {
                $("#data-dataset-chart-accordion-options-collapse").find("[name=sort]").bootstrapToggle('off').bootstrapToggle('disable');
            }
            else {
                $("#data-dataset-chart-accordion-options-collapse").find("[name=sort]").bootstrapToggle('off').bootstrapToggle('enable');
            }
        };


    });

    $("#data-dataset-chart-accordion-series-collapse [name=dimension-container], #data-dataset-chart-accordion-series-collapse [name=tab-content]").find("select").once('change', function () {
        app.data.dataset.chart.resetChart();
    });

    $("#data-dataset-chart-accordion-series-collapse [name=name]").once('keyup', function () {
        app.data.dataset.chart.resetChart();
    });

    //sort
    $("#data-dataset-chart-accordion-series-collapse").find("[name=sort-options]").once("click", function () {
        var dimension = $(this).attr("idn");
        var series = $(this).attr("series");
        var select = $("#data-dataset-chart-accordion-series-collapse").find("select[idn='" + dimension + "'][series='" + series + "']");
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

    $("#data-dataset-chart-accordion-series-collapse").find("[name=dimension-filter]").once("keyup search", function () {
        var dimensionCode = $(this).attr("idn");
        var series = $(this).attr("series");
        var thisDimension = app.data.dataset.metadata.jsonStat.Dimension(dimensionCode);

        var select = $("#data-dataset-chart-accordion-series-collapse").find("select[idn='" + dimensionCode + "'][series='" + series + "']");
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
                    if (select.attr("role") == "time") {
                        select.prepend($('<option>', {
                            "value": key,
                            "title": value,
                            "text": thisDimension.Category(key).label + (thisDimension.Category(key).unit ? " (" + thisDimension.Category(key).unit.label + ")" : "")
                        }));
                    }
                    else {
                        select.append($('<option>', {
                            "value": key,
                            "title": value,
                            "text": thisDimension.Category(key).label + (thisDimension.Category(key).unit ? " (" + thisDimension.Category(key).unit.label + ")" : "")
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
            $("#data-dataset-chart-accordion-series-collapse").find("[name=select-all][idn='" + dimensionCode + "']").prop("disabled", true);
        }
        else {
            //enable select all
            $("#data-dataset-chart-accordion-series-collapse").find("[name=select-all][idn='" + dimensionCode + "']").prop("disabled", false);

        }

    });

    if (app.data.isModal) {
        $('#data-view-modal').animate({
            scrollTop: '+=' + $('#data-dataset-chart-accordion-series-heading')[0].getBoundingClientRect().top
        },
            1000);
    }
    else {
        $('html, body').animate({
            scrollTop: $("#data-dataset-chart-accordion-series-heading").offset().top
        }, 1000);
    }

    switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
        case "pie":
        case "doughnut":
        case "polarArea":
            $("#data-dataset-chart-accordion-series-collapse [name=add-series-footer], #data-dataset-chart-accordion-series-collapse [name=delete-series]").hide();
            break;

        default:
            break;
    }

    $("#data-dataset-chart-accordion [name=view-chart]").prop("disabled", false);

    //for pyramid charts, limit the number of series to 2
    if (
        $("#data-dataset-chart-properties").find("[name=type]").val() == "pyramid"
        && $("#data-dataset-chart-accordion-series-collapse [name=series-tabs] li").length == 2
    ) {
        $("#data-dataset-chart-accordion-series-collapse").find("[name=add-series]").prop("disabled", true);
    }
};

app.data.dataset.chart.dualAxis = function () {
    app.data.dataset.chart.resetChart();
    if ($("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis]").is(':checked')) {
        $("#data-dataset-chart-accordion-options-collapse").find("[name=series-stacked]").hide();
        $("#data-dataset-chart-accordion-series-collapse [name=tab-content] [name=dual-axis-position]").each(function (index, value) {
            app.data.dataset.chart.drawDualAxis(value)
        });
        $("#data-dataset-chart-accordion-series-collapse").find("[name=right-yaxis-label]").prop("disabled", false).val("");

        $("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis-position]").prop("disabled", false);

    }
    else {
        $("#data-dataset-chart-accordion-options-collapse").find("[name=series-stacked]").show();
        $("#data-dataset-chart-accordion-series-collapse").find("[name=right-yaxis-label]").prop("disabled", true).val("");
        $("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis-position]").prop("disabled", true).val("y-axis-1");
    }

}

app.data.dataset.chart.stacked = function () {
    app.data.dataset.chart.resetChart();
    if ($("#data-dataset-chart-accordion-options-collapse").find("[name=stacked]").is(':checked')) {
        $("#data-dataset-chart-accordion-options-collapse").find("[name=stacked-percent]").bootstrapToggle('enable');
        $("#data-dataset-chart-accordion-options-collapse").find("[name=sort]").bootstrapToggle('off').bootstrapToggle('disable');
        $("#data-dataset-chart-accordion-series-collapse [name=dual-axis-holder], #data-dataset-chart-accordion-series-collapse [name=series-dual-axis]").hide();
    }
    else {
        $("#data-dataset-chart-accordion-options-collapse").find("[name=stacked-percent]").bootstrapToggle('off').bootstrapToggle('disable');
        $("#data-dataset-chart-accordion-options-collapse").find("[name=sort]").bootstrapToggle('enable');
        $("#data-dataset-chart-accordion-series-collapse [name=dual-axis-holder], #data-dataset-chart-accordion-series-collapse [name=series-dual-axis], #data-dataset-chart-accordion-series-collapse [name=right-yaxis-label-row]").show();
    }
}

app.data.dataset.chart.drawDualAxis = function (element) {
    $(element).empty();
    $.each(C_APP_PXWIDGET_CHART_TYPES_DUAL_POSITION, function (index, value) {
        $(element).append($("<option>", {
            "value": value,
            "text": app.label.static[value]

        }).get(0).outerHTML)
    });
    $(element).val("y-axis-1")
}

app.data.dataset.chart.deleteSeries = function (series) {
    app.data.dataset.chart.resetChart();

    $("#data-dataset-chart-accordion-series-collapse").find("[name=series-tab][series=" + series + "]").remove();
    $("#data-dataset-chart-accordion-series-collapse").find("[name=tab-content][series=" + series + "]").remove();

    $("#data-dataset-chart-accordion-series-collapse [name=series-tabs] li:first-child a").addClass("active").attr("aria-selected", "true");
    $("#data-dataset-chart-accordion-series-collapse [name=series-content] .tab-pane:first-child").addClass("show").addClass("active");

    if (app.data.isModal) {
        $('#data-view-modal').animate({
            scrollTop: '+=' + $('#data-dataset-chart-accordion-series-heading')[0].getBoundingClientRect().top
        },
            1000);
    }
    else {
        $('html, body').animate({
            scrollTop: $("#data-dataset-chart-accordion-series-heading").offset().top
        }, 1000);
    }

    //get number of series remaining to enable/disable view chart button
    if ($("#data-dataset-chart-properties").find("[name=type]").val() == "pyramid") {
        $("#data-dataset-chart-accordion [name=view-chart]").prop("disabled", true);
    }
    else if (!$("#data-dataset-chart-accordion-series-collapse").find("[name=tab-content]").length) {
        $("#data-dataset-chart-accordion [name=view-chart]").prop("disabled", true);
    }

    else {
        $("#data-dataset-chart-accordion [name=view-chart]").prop("disabled", false);
    }

    //for pyramid charts, limit the number of series to 2
    if (
        $("#data-dataset-chart-properties").find("[name=type]").val() == "pyramid"
        && $("#data-dataset-chart-accordion-series-collapse [name=series-tabs] li").length < 2
    ) {
        $("#data-dataset-chart-accordion-series-collapse").find("[name=add-series]").prop("disabled", false);
    }



}

app.data.dataset.chart.formatJson = function () {
    $("#data-dataset-chart-snippet-code [name=invalid-json-object]").hide();
    if ($("#data-dataset-chart-snippet-code [name=custom-config]").val().trim().length) {
        var ugly = $("#data-dataset-chart-snippet-code [name=custom-config]").val().trim();
        var obj = null;
        var pretty = null;
        try {
            obj = JSON.parse(ugly);
            pretty = JSON.stringify(obj, undefined, 4);
            $("#data-dataset-chart-snippet-code [name=custom-config]").val(pretty);
            app.data.dataset.chart.renderSnippet();
        } catch (err) {
            $("#data-dataset-chart-snippet-code [name=invalid-json-object]").show();
        }
    }
}

//#endregion series

//#region build config
app.data.dataset.chart.buildChartConfig = function (scroll) {
    //show link-to-wip-wrapper toggle if modal

    if (app.data.RlsCode) {
        if (!app.data.isLive) {
            $("#data-dataset-chart-snippet-code").find("[name=link-to-wip-wrapper]").show();
        }
    }

    $('#data-dataset-chart-errors').find("[name=errors]").empty();
    $('#data-dataset-chart-errors').hide();
    $("#data-dataset-chart-accordion-series-collapse canvas").remove();

    app.data.dataset.chart.configuration = {};
    $.extend(true, app.data.dataset.chart.configuration, app.data.dataset.chart.template.wrapper);
    $.extend(true, app.data.dataset.chart.configuration.metadata, app.data.dataset.chart.template.metadata);

    app.data.dataset.chart.configuration.options.scales.yAxes[0].id = C_APP_PXWIDGET_CHART_TYPES_DUAL_POSITION[0];
    if ($("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis]").is(":visible, :checked")) {
        app.data.dataset.chart.configuration.options.scales.yAxes.push(
            $.extend(true, {}, app.data.dataset.chart.template.yAxisRight)
        );
    }

    var isStacked = $("#data-dataset-chart-accordion-options-collapse").find("[name=stacked]").is(':visible, :checked');
    var isStackedPercentage = $("#data-dataset-chart-accordion-options-collapse").find("[name=stacked-percent]").is(':visible, :checked');
    app.data.dataset.chart.configuration.sort = $("#data-dataset-chart-accordion-options-collapse").find("[name=sort]").is(':checked');
    app.data.dataset.chart.configuration.maxNumberValues = $("#data-dataset-chart-accordion-options-collapse").find("[name=max-number-values]").val() ? Math.abs($("#data-dataset-chart-accordion-options-collapse").find("[name=max-number-values]").val()) : null;
    app.data.dataset.chart.configuration.options.scales.xAxes[0].stacked = isStacked;
    app.data.dataset.chart.configuration.options.scales.xAxes[0].ticks.maxTicksLimit = $("#data-dataset-chart-accordion-options-collapse").find("[name=xaxis-max-steps]").val().trim() || null;
    $.each(app.data.dataset.chart.configuration.options.scales.yAxes, function (index, value) {
        value.stacked = isStacked;
    });

    //axes labeling

    switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
        case "horizontalBar":
        case "pyramid":
            app.data.dataset.chart.configuration.options.scales.xAxes[0].scaleLabel.display = $("#data-dataset-chart-accordion-series-collapse").find("[name=left-yaxis-label]").val().trim() ? true : false;
            app.data.dataset.chart.configuration.options.scales.xAxes[0].scaleLabel.labelString = $("#data-dataset-chart-accordion-series-collapse").find("[name=left-yaxis-label]").val().trim() || null;

            app.data.dataset.chart.configuration.options.scales.yAxes[0].scaleLabel.display = $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label]").val().trim() ? true : false;
            app.data.dataset.chart.configuration.options.scales.yAxes[0].scaleLabel.labelString = $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label]").val().trim() || null;

            break;

        default:
            app.data.dataset.chart.configuration.options.scales.xAxes[0].scaleLabel.display = $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label]").val().trim() ? true : false;
            app.data.dataset.chart.configuration.options.scales.xAxes[0].scaleLabel.labelString = $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=x-axis-label]").val().trim() || null;

            app.data.dataset.chart.configuration.options.scales.yAxes[0].scaleLabel.display = $("#data-dataset-chart-accordion-series-collapse").find("[name=left-yaxis-label]").val().trim() ? true : false;
            app.data.dataset.chart.configuration.options.scales.yAxes[0].scaleLabel.labelString = $("#data-dataset-chart-accordion-series-collapse").find("[name=left-yaxis-label]").val().trim() || null;

            if ($("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis]").is(":visible, :checked")) {
                app.data.dataset.chart.configuration.options.scales.yAxes[1].scaleLabel.display = $("#data-dataset-chart-accordion-series-collapse").find("[name=right-yaxis-label]").val().trim() ? true : false;
                app.data.dataset.chart.configuration.options.scales.yAxes[1].scaleLabel.labelString = $("#data-dataset-chart-accordion-series-collapse").find("[name=right-yaxis-label]").val().trim() || null;
            }
            break;
    };

    app.data.dataset.chart.configuration.options.plugins.stacked100.enable = isStackedPercentage;

    app.data.dataset.chart.configuration.options.legend.display = C_APP_PXWIDGET_CHART_LEGEND_POSITION.includes($("#data-dataset-chart-accordion-options-collapse [name=legend-position]").val());

    if (app.data.dataset.chart.configuration.options.legend.display) {
        app.data.dataset.chart.configuration.options.legend.position = $("#data-dataset-chart-accordion-options-collapse [name=legend-position]").val();
    }
    else {
        delete app.data.dataset.chart.configuration.options.legend.position
    }

    if (app.data.isLive) {
        app.data.dataset.chart.configuration.metadata.api.query.data.params.matrix = app.data.MtrCode;
        app.data.dataset.chart.configuration.metadata.api.query.url = app.config.url.api.jsonrpc.public;
        app.data.dataset.chart.configuration.metadata.api.query.data.method = "PxStat.Data.Cube_API.ReadMetadata";
        delete app.data.dataset.chart.configuration.metadata.api.query.data.params.release;
    }
    else {
        app.data.dataset.chart.configuration.metadata.api.query.data.params.release = app.data.RlsCode;
        app.data.dataset.chart.configuration.metadata.api.query.url = app.config.url.api.jsonrpc.private;
        app.data.dataset.chart.configuration.metadata.api.query.data.method = "PxStat.Data.Cube_API.ReadPreMetadata";
        delete app.data.dataset.chart.configuration.metadata.api.query.data.params.matrix;
    }
    switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
        case "mixed":
            app.data.dataset.chart.configuration.type = "bar";
            break;
        case "area":
            app.data.dataset.chart.configuration.type = "line";
            break;
        default:
            app.data.dataset.chart.configuration.type = $("#data-dataset-chart-properties").find("[name=type]").val();
            break;
    }
    //app.data.dataset.chart.configuration.type = $("#data-dataset-chart-properties").find("[name=type]").val() == "mixed" ? "bar" : $("#data-dataset-chart-properties").find("[name=type]").val();

    app.data.dataset.chart.configuration.metadata.api.query.data.params.language = app.data.LngIsoCode;

    var xAxisDimensionCode = $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").find("select:enabled").attr("idn");

    var errors = [];
    app.data.dataset.chart.configuration.metadata.xAxis = {};
    app.data.dataset.chart.configuration.metadata.xAxis[xAxisDimensionCode] = [];
    app.data.dataset.chart.configuration.metadata.xAxis.role = app.data.dataset.metadata.jsonStat.Dimension(xAxisDimensionCode).role;
    //check if not all selected in dimension
    var numVariables = app.data.dataset.metadata.jsonStat.Dimension(xAxisDimensionCode).length;
    var numVariablesSelected = $("#data-dataset-chart-accordion-xaxis-collapse").find("select[idn='" + xAxisDimensionCode + "'] option:selected").length;
    if (numVariables != numVariablesSelected) {
        $("#data-dataset-chart-accordion-xaxis-collapse").find("select[idn='" + xAxisDimensionCode + "'] option:selected").each(function () {
            app.data.dataset.chart.configuration.metadata.xAxis[xAxisDimensionCode].push(this.value);
        });
    };

    if (numVariablesSelected == 1) {
        // disable autoscale as it cannot work with one item on the x-axis
        $("#data-dataset-chart-accordion-options-collapse").find("[name=auto-scale]").bootstrapToggle('off').bootstrapToggle('disable');

    }
    else {
        $("#data-dataset-chart-accordion-options-collapse").find("[name=auto-scale]").bootstrapToggle('enable');
    }

    //area chart
    switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
        case "area":
            app.data.dataset.chart.configuration.options.scales.yAxes[0].stacked = true;

            if ($("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis]").is(":visible, :checked")) {
                app.data.dataset.chart.configuration.options.scales.yAxes[1].stacked = true;
            };
            app.data.dataset.chart.configuration.options.plugins.stacked100.enable = false;

            break;

        default:

            break;
    }

    $("#data-dataset-chart-accordion-series-collapse [name=tab-content]").each(function () {
        var thisDataset = {};

        $.extend(true, thisDataset, app.data.dataset.chart.template.dataset);
        if (app.data.isLive) {
            thisDataset.api.query.data.params.extension.matrix = app.data.MtrCode;
            thisDataset.api.query.url = app.config.url.api.jsonrpc.public;
            thisDataset.api.query.data.method = "PxStat.Data.Cube_API.ReadDataset";
            delete thisDataset.api.query.data.params.extension.release;
        }
        else {
            thisDataset.api.query.data.params.extension.release = app.data.RlsCode;
            thisDataset.api.query.url = app.config.url.api.jsonrpc.private;
            thisDataset.api.query.data.method = "PxStat.Data.Cube_API.ReadPreDataset";
            delete thisDataset.api.query.data.params.extension.matrix;
        }
        thisDataset.api.query.data.params.extension.language.code = app.data.LngIsoCode;


        if ($("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis]").is(':checked')) {
            thisDataset.yAxisID = $(this).find("[name=dual-axis-position]").val();
        };


        var seriesNumber = $("#" + $(this).attr("aria-labelledby") + "").text();

        $(this).find("[name=dimension-containers] select").each(function (index) {
            thisDataset.api.query.data.params.id.push($(this).attr("idn"));
            thisDataset.api.query.data.params.dimension[$(this).attr("idn")] = {};
            thisDataset.api.query.data.params.dimension[$(this).attr("idn")].category = {};
            thisDataset.api.query.data.params.dimension[$(this).attr("idn")].category.index = [$(this).val()];
            if (!$(this).val()) {
                errors.push(app.library.html.parseDynamicLabel("chart-error-series-incomplete", [seriesNumber, $(this).attr("dimension-name")]));
            }
        });

        switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
            case "pie":
            case "doughnut":
            case "polarArea":
                app.data.dataset.chart.configuration.options.title.text.push($(this).find("[name=name]").val().trim());
                break;
            case "mixed":
                thisDataset.type = $(this).find("[name=mixed]").val();
                break;
            default:
                break;
        }
        thisDataset.label = $(this).find("[name=name]").val().trim();
        app.data.dataset.chart.configuration.datasetLabels.push($(this).find("[name=name]").val().trim());

        if (!thisDataset.label.length) {
            switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
                case "pie":
                case "doughnut":
                case "polarArea":
                    break;
                //no need to check name
                default:
                    errors.push(app.library.html.parseDynamicLabel("chart-error-series-name", [seriesNumber]));
                    break;
            }

        }

        switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
            case "area":
                thisDataset.fill = true;
                break;
            default:
                break;
        }

        if (!errors.length) {
            app.data.dataset.chart.configuration.data.datasets.push(thisDataset);
        }
    });


    if (!errors.length) {

        if ($("#data-dataset-chart-accordion-options-collapse").find("[name=xaxis-max-steps]").val().trim()) {
            switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
                case "pie":
                case "doughnut":
                case "polarArea":
                    break;
                default:
                    var number = parseFloat($("#data-dataset-chart-accordion-options-collapse").find("[name=xaxis-max-steps]").val().trim())
                    if (!Number.isInteger(number)) {
                        errors.push(app.label.static["chart-error-xaxis-max-steps"]);
                    }
                    if (!errors.length) {
                        if (number < 0) {
                            errors.push(app.label.static["chart-error-xaxis-max-steps"]);
                        }
                    }
                    break;
            }

        }
    }

    //check for correct dual axis
    if (!errors.length) {
        if ($("#data-dataset-chart-accordion-series-collapse").find("[name=dual-axis]").is(':checked') && app.data.dataset.chart.configuration.data.datasets.length == 1) {
            errors.push(app.label.static["chart-error-dual-axis"]);
        }
    }


    //remove auto-scale
    if (!$("#data-dataset-chart-accordion-options-collapse").find("[name=auto-scale]").is(':checked')) {
        if ($("#data-dataset-chart-properties").find("[name=type]").val() == "horizontalBar") {
            $.each(app.data.dataset.chart.configuration.options.scales.xAxes, function (index, value) {
                value.ticks.beginAtZero = true;
            });
        }
        else {
            $.each(app.data.dataset.chart.configuration.options.scales.yAxes, function (index, value) {
                value.ticks.beginAtZero = true;
            });
        }

    };

    //set lines to either curved or straight
    if (!$("#data-dataset-chart-accordion-options-collapse").find("[name=curved-line]").is(':checked')) {
        //straight line
        app.data.dataset.chart.configuration.options.elements.line.tension = 0;
    };

    if (!errors.length) {
        //set colour scheme
        switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
            case "pie":
            case "doughnut":
            case "polarArea":
            case "radar":
                var numSeriesCategories = app.data.dataset.chart.configuration.metadata.xAxis[xAxisDimensionCode].length;
                if (!app.config.plugin.chartJs.chart.colours.length) {
                    app.data.dataset.chart.configuration.options.plugins.colorschemes.scheme = numSeriesCategories <= 10 ? "tableau.Tableau10" : "tableau.Tableau20";
                }
                else {
                    app.data.dataset.chart.configuration.options.plugins.colorschemes.scheme = app.config.plugin.chartJs.chart.colours;
                }

                break;
            default:
                //set colour scheme depending on number of series        
                var numDatasets = app.data.dataset.chart.configuration.data.datasets.length;
                if (!app.config.plugin.chartJs.chart.colours.length) {
                    app.data.dataset.chart.configuration.options.plugins.colorschemes.scheme = numDatasets <= 10 ? "tableau.Tableau10" : "tableau.Tableau20";
                }
                else {
                    app.data.dataset.chart.configuration.options.plugins.colorschemes.scheme = app.config.plugin.chartJs.chart.colours;
                }

                break;
        }
    };

    if (!errors.length) {
        switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
            case "pie":
            case "doughnut":
            case "polarArea":
            case "radar":
                delete app.data.dataset.chart.configuration.options.scales;
                break;
            case "mixed":
                //set order of datasets so columns always underneath
                $.each(app.data.dataset.chart.configuration.data.datasets, function (key, value) {
                    if (value.type == "line") {
                        value.order = 1
                    }
                    else {
                        value.order = 2
                    }
                });
                break;
            default:
                break;
        }


    }

    //change the tooltip mode for pie/donut etc
    if (!errors.length) {
        switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
            case "pie":
            case "doughnut":
            case "polarArea":
            case "radar":
                app.data.dataset.chart.configuration.options.tooltips.mode = "nearest";
                break;
            default:
                break;
        }
    }
    if (!errors.length) {

        if (app.data.RlsCode) {
            if (app.data.isLive) {
                app.data.dataset.chart.configuration = app.data.dataset.chart.getFluidTime(app.data.dataset.chart.configuration);
            }
        }
        else {
            app.data.dataset.chart.configuration = app.data.dataset.chart.getFluidTime(app.data.dataset.chart.configuration);
        }

        $("#data-dataset-chart-render, #data-dataset-chart-snippet-code").show();
        pxWidget.draw.init(C_APP_PXWIDGET_TYPE_CHART, "pxwidget-chart", app.data.dataset.chart.configuration, function () {
            // Render the Snippet
            app.data.dataset.chart.snippetConfig = {};
            app.data.dataset.chart.saveQuery.configuration = {};
            $.extend(true, app.data.dataset.chart.snippetConfig, pxWidget.draw.params["pxwidget-chart"]);
            app.data.dataset.chart.renderSnippet();
        });

        if (scroll) {
            if (app.data.isModal) {
                $("#data-dataset-chart-render").find("[name=save-query-wrapper]").hide();
                $('#data-view-modal').animate({
                    scrollTop: '+=' + $('#data-dataset-chart-render')[0].getBoundingClientRect().top
                },
                    1000);
            }
            else {
                $("#data-dataset-chart-render").find("[name=save-query-wrapper]").show();
                $('html, body').animate({
                    scrollTop: $("#data-dataset-chart-render").offset().top
                }, 1000);
            }
        }


    } else {
        $('#data-dataset-chart-errors').show();
        $.each(errors, function (index, value) {
            $('#data-dataset-chart-errors').find("[name=errors]").append($("<li>", {
                "class": "list-group-item",
                "html": value
            }));
        });
        if (app.data.isModal) {
            $('#data-view-modal').animate({
                scrollTop: '+=' + $('#data-dataset-chart-errors')[0].getBoundingClientRect().top
            },
                1000);
        }
        else {
            $('html, body').animate({
                scrollTop: $("#data-dataset-chart-errors").offset().top
            }, 1000);
        }
    }
}

app.data.dataset.chart.renderSnippet = function () {
    var snippet = app.config.entity.data.snippet;
    //clone configuration to use in save query before adding fluid time
    $.extend(true, app.data.dataset.chart.saveQuery.configuration, app.data.dataset.chart.snippetConfig);
    app.data.dataset.chart.saveQuery.configuration.metadata.api.response = {};
    $.each(app.data.dataset.chart.saveQuery.configuration.data.datasets, function (key, value) {
        value.api.response = {};
    });

    var config = $.extend(true, {}, app.data.dataset.chart.snippetConfig);
    //check for fluid time for xAxis or series
    if ($("#data-dataset-chart-snippet-code").find("[name=fluid-time]").is(':checked')) {
        config = app.data.dataset.chart.getFluidTime(config);
    }
    else {
        //empty fluid time
        config.metadata.fluidTime = [];
        $.each(config.data.datasets, function (key, value) {
            value.fluidTime = []
        });
    }

    config.autoupdate = $("#data-dataset-chart-snippet-code").find("[name=auto-update]").is(':checked');
    config.link = $("#data-dataset-chart-snippet-code").find("[name=include-link]").is(':checked') ? app.config.url.application + C_COOKIE_LINK_TABLE + "/" + app.data.MtrCode : null;
    config.copyright = $("#data-dataset-chart-snippet-code").find("[name=include-copyright]").is(':checked');
    if (!$("#data-dataset-chart-snippet-code").find("[name=include-title]").is(':checked')) {
        config.options.title.text.unshift("");
    }
    else {
        config.options.title.text.unshift($("#data-dataset-chart-snippet-code").find("[name=title-value]").val().trim());
    }

    switch ($("#data-dataset-chart-properties").find("[name=type]").val()) {
        case "pie":
        case "doughnut":
            app.data.dataset.chart.configuration.showPercentage = $("#data-dataset-chart-accordion-options-collapse").find("[name=show-percentage]").is(':checked');
            break;

        default:
            app.data.dataset.chart.configuration.showPercentage = null;
            break;
    }

    if ($("#data-dataset-chart-snippet-code").find("[name=link-to-wip]").is(':checked')) {
        config.metadata.api.response = {};
        config.matrix = app.data.MtrCode;

        //if link to WIP, then make query explicit so it is always a point in time query with no additional variables added at a later date because they selected all variables in initial query
        var xAxisDimensionCode = $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").find("select:enabled").attr("idn");
        config.metadata.xAxis[xAxisDimensionCode] = [];

        $("#data-dataset-chart-accordion-xaxis-collapse").find("select[idn='" + xAxisDimensionCode + "'] option:selected").each(function () {
            config.metadata.xAxis[xAxisDimensionCode].push(this.value);
        });

        $.each(config.data.datasets, function (key, value) {
            value.api.response = {};
            value.unit = [];
            value.decimal = [];
        });

    }
    else {
        if ($("#data-dataset-chart-snippet-code").find("[name=auto-update]").is(':checked')) {
            config.metadata.api.response = {};
            $.each(config.data.datasets, function (key, value) {
                value.api.response = {};
                value.unit = [];
                value.decimal = [];
            });
        } else {
            config.metadata.api.query = {};
            $.each(config.data.datasets, function (key, value) {
                value.api.query = {};
                value.decimal = [];
            });

        }
    }

    //remove date if not live, can be present if prending live
    if (!app.data.isLive) {
        delete config.metadata.api.response.updated;
    }

    //add custom JSON
    try {
        var customOptions = JSON.parse($("#data-dataset-chart-snippet-code [name=custom-config]").val().trim());
        $.extend(true, config, customOptions);
        $("#data-dataset-chart-snippet-code [name=invalid-json-object]").hide();
    } catch (err) {
        $("#data-dataset-chart-accordion-options-collapse").collapse('show');
        $("#data-dataset-chart-snippet-code [name=invalid-json-object]").show();
    }

    snippet = snippet.sprintf([C_APP_URL_PXWIDGET_ISOGRAM, C_APP_PXWIDGET_TYPE_CHART, app.library.utility.randomGenerator('pxwidget'), JSON.stringify(config)]);

    $("#data-pxwidget-snippet-chart-code").hide().text(snippet.trim()).fadeIn();
    Prism.highlightAll();

    $("#data-dataset-chart-render").find("[name=save-query]").prop('disabled', false);
    $("#data-dataset-chart-snippet-code").find("[name=copy-snippet-code]").prop('disabled', false);
    $("#data-dataset-chart-render").find("[name=download-chart]").removeClass('disabled');

    if ($("#data-dataset-chart-snippet-code [name=link-to-wip]").is(':checked')) {
        //disable download HTML button as this won't work with private api due to CORS rules
        $("#data-dataset-chart-snippet-code").find("[name=download-snippet]").prop('disabled', true);
        $("#data-dataset-chart-snippet-code").find("[name=preview-snippet]").prop('disabled', true);
    }
    else {
        //disable download HTML button as this won't work with private api due to CORS rules
        $("#data-dataset-chart-snippet-code").find("[name=download-snippet]").prop('disabled', false);
        $("#data-dataset-chart-snippet-code").find("[name=preview-snippet]").prop('disabled', false);
    }

};

app.data.dataset.chart.getFluidTime = function (config) {
    config.metadata.fluidTime = [];
    if (config.metadata.xAxis.role == "time") {

        var dimensionSize = app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id.length;
        var xAxisDimensionCode = $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").find("select:enabled").attr("idn");

        //check for time variables selected
        if (config.metadata.xAxis[xAxisDimensionCode].length) {
            $.each(config.metadata.xAxis[xAxisDimensionCode], function (index, value) {
                //get position of variables selected releative to the end of the array as new time points are added to the end of the array
                var actualPosition = $.inArray(value, app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id);
                var relativePosition = (dimensionSize - actualPosition) - 1;
                config.metadata.fluidTime.push(relativePosition);
            });
        }
        else {
            //get positions of all variables as fluid is true and everything is selected
            //get position of variables selected releative to the end of the array as new time points are added to the end of the array
            $.each(app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id, function (index, value) {
                config.metadata.fluidTime.push(index);
            });
        }
    }
    else {
        //timepoint must be in each series
        $.each(config.data.datasets, function (index, value) {
            value.fluidTime = [];
            var timeCodeSelected = value.api.query.data.params.dimension[app.data.dataset.metadata.timeDimensionCode].category.index[0];
            var dimensionSize = app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id.length;
            //get position of variables selected releative to the end of the array as new time points are added to the end of the array
            var actualPosition = $.inArray(timeCodeSelected, app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id);
            var relativePosition = (dimensionSize - actualPosition) - 1;
            value.fluidTime.push(relativePosition);
        });
    }
    return config;
};

app.data.dataset.chart.resetChart = function () {
    //hide the chart
    $("#pxwidget-chart, #data-pxwidget-snippet-chart-code").empty();
    $("#data-dataset-chart-render, #data-dataset-chart-snippet-code").hide();
    $("#data-dataset-chart-accordion-options-collapse").find("[name=max-number-values]").val("");
    if (!$("#data-dataset-chart-accordion-options-collapse [name=sort]").is(':checked')) {
        $("#data-dataset-chart-accordion-options-collapse").find("[name=max-number-values]").prop("disabled", true);
    }
    else {
        $("#data-dataset-chart-accordion-options-collapse").find("[name=max-number-values]").prop("disabled", false);
    }
}

app.data.dataset.chart.resetAll = function () {
    //reset chart seriesId
    app.data.dataset.chart.seriesId = 0;
    //hide the chart
    $("#pxwidget-chart, #data-pxwidget-snippet-chart-code").empty();
    $("#data-dataset-chart-render, #data-dataset-chart-snippet-code, #data-dataset-chart-accordion, #data-dataset-chart-errors").hide();

    //reset build card
    $("#data-dataset-chart-accordion-xaxis-collapse").find("[name=dimension-containers]").empty();
    $("#data-dataset-chart-accordion-series-collapse").find("[name=series-tabs]").empty();
    $("#data-dataset-chart-accordion-series-collapse").find("[name=series-content]").empty();
    $("#data-dataset-chart-snippet-code [name=custom-config]").val(JSON.stringify({ "options": {} }));
    app.data.dataset.chart.formatJson();

    //disable add series accordion and view button
    $("#data-dataset-chart-accordion-series-heading button").prop("disabled", true);
    $("#data-dataset-chart-accordion-series-collapse").find("[name=add-series]").prop("disabled", false);
    $("#data-dataset-chart-accordion [name=view-chart]").prop("disabled", true);
    $("#data-dataset-chart-accordion-series-collapse [name=add-series-footer]").show();

    //reset accordion
    $('#data-dataset-chart-accordion-xaxis-collapse, #data-dataset-chart-accordion-series-collapse, #data-dataset-chart-accordion-options-collapse').collapse("hide");
    $('#data-dataset-chart-accordion-xaxis-collapse').collapse("show");

    $("#data-dataset-chart-accordion-options-collapse [name=xaxis-max-steps], #data-dataset-chart-accordion-xaxis-collapse [name=x-axis-label], #data-dataset-chart-accordion-series-collapse [name=right-yaxis-label], #data-dataset-chart-accordion-series-collapse [name=left-yaxis-label]").val("");

    $("#data-dataset-chart-accordion-options-collapse [name=auto-scale-row], #data-dataset-chart-accordion-options-collapse [name=curved-line-row], #data-dataset-chart-accordion-options-collapse [name=sort-row], #data-dataset-chart-accordion-options-collapse [name=xaxis-max-steps-row]").show();

    //reset chart type select
    app.data.dataset.chart.getChartTypes();
    app.data.dataset.chart.setLegendPosition();

    $("#data-dataset-chart-accordion-series-collapse [name=dual-axis]").bootstrapToggle("off");
    $("#data-dataset-chart-accordion-options-collapse [name=stacked]").bootstrapToggle("off");

    //need to reset options, unbind listeners first
    $("#data-dataset-chart-snippet-code [name=auto-update], #data-dataset-chart-snippet-code [name=fluid-time], #data-dataset-chart-snippet-code [name=link-to-wip], #data-dataset-chart-snippet-code [name=include-title], #data-dataset-chart-snippet-code [name=include-copyright], #data-dataset-chart-snippet-code [name=include-link],#data-dataset-chart-accordion-options-collapse [name=auto-scale], #data-dataset-chart-accordion-options-collapse [name=curved-line], #data-dataset-chart-accordion-options-collapse [name=sort], #data-dataset-chart-accordion-options-collapse [name=show-percentage]").unbind("change");
    //reset toggles
    $("#data-dataset-chart-snippet-code [name=fluid-time]").bootstrapToggle('enable');
    $("#data-dataset-chart-snippet-code [name=auto-update], #data-dataset-chart-snippet-code [name=fluid-time], #data-dataset-chart-snippet-code [name=include-title], #data-dataset-chart-snippet-code [name=include-copyright], #data-dataset-chart-snippet-code [name=include-link], #data-dataset-chart-accordion-options-collapse [name=auto-scale], #data-dataset-chart-accordion-options-collapse [name=curved-line], #data-dataset-chart-accordion-options-collapse [name=sort], #data-dataset-chart-accordion-options-collapse [name=show-percentage]").bootstrapToggle("on");
    $("#data-dataset-chart-snippet-code [name=link-to-wip]").bootstrapToggle("off");
    //reset listeners
    if (app.data.RlsCode) {
        if (!app.data.isLive) {
            $("#data-dataset-chart-snippet-code").find("[name=auto-update]").bootstrapToggle('off');
            $("#data-dataset-chart-snippet-code").find("[name=auto-update]").bootstrapToggle('disable');
            $("#data-dataset-chart-snippet-code").find("[name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-chart-snippet-code").find("[name=fluid-time]").bootstrapToggle('disable');
        }
    }

    $("#data-dataset-chart-snippet-code [name=fluid-time], #data-dataset-chart-snippet-code [name=link-to-wip], #data-dataset-chart-snippet-code [name=include-title], #data-dataset-chart-snippet-code [name=include-copyright], #data-dataset-chart-snippet-code [name=include-link]").once("change", function () {
        app.data.dataset.chart.renderSnippet();
    });

    $("#data-dataset-chart-snippet-code").find("[name=title-value]").val(app.data.dataset.metadata.jsonStat.label.trim());

    $("#data-dataset-chart-accordion-options-collapse [name=auto-scale], #data-dataset-chart-accordion-options-collapse [name=curved-line], #data-dataset-chart-accordion-options-collapse [name=sort], #data-dataset-chart-accordion-options-collapse [name=show-percentage]").once("change", app.data.dataset.chart.resetChart);

    $("#data-dataset-chart-snippet-code [name=auto-update]").once("change", function () {
        app.data.dataset.chart.renderSnippet();
        if (!$(this).is(':checked')) {
            $("#data-dataset-chart-snippet-code [name=fluid-time]").bootstrapToggle('off');
            $("#data-dataset-chart-snippet-code [name=fluid-time]").bootstrapToggle('disable');
        } else {
            $("#data-dataset-chart-snippet-code [name=fluid-time]").bootstrapToggle('enable');
        }
    });

    $("#data-dataset-chart-accordion-series-collapse").find("[name=series-toggles]").hide();

    //scroll to chart type
    if (app.data.isModal) {
        $('#data-view-modal').animate({
            scrollTop: '+=' + $('#data-dataset-chart-properties')[0].getBoundingClientRect().top
        }, 1000);
    }
    else {
        $('html, body').animate({
            scrollTop: $("#data-dataset-chart-properties").offset().top
        }, 1000);
    }
};
//#endregion build config

//#region save query
app.data.dataset.chart.saveQuery.drawSaveQueryModal = function () {
    app.data.dataset.saveQuery.validation.drawSaveQuery();
    $("#data-dataset-save-query").modal("show");
};

app.data.dataset.chart.saveQuery.ajax.saveQuery = function () {
    //now clone it into new variable for ajax which may or may not need fluid time
    var saveQueryConfig = $.extend(true, {}, app.data.dataset.chart.saveQuery.configuration);
    if ($("#data-dataset-save-query").find("[name=fluid-time]").is(':checked')) {
        saveQueryConfig = app.data.dataset.chart.getFluidTime(saveQueryConfig);
    }
    else {
        //empty fluid time
        saveQueryConfig.metadata.fluidTime = [];
        $.each(saveQueryConfig.data.datasets, function (key, value) {
            value.fluidTime = []
        });
    }

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
                    "Type": C_APP_PXWIDGET_TYPE_CHART,
                    "Query": base64snippet,
                    "FluidTime": $("#data-dataset-save-query").find("[name=fluid-time]").is(':checked'),
                    "Isogram": C_APP_URL_PXWIDGET_ISOGRAM
                }
            },
            "app.data.dataset.chart.saveQuery.callback.saveQuery",
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
                        "Type": C_APP_PXWIDGET_TYPE_CHART,
                        "Query": base64snippet,
                        "FluidTime": $("#data-dataset-save-query").find("[name=fluid-time]").is(':checked'),
                        "Isogram": C_APP_URL_PXWIDGET_ISOGRAM
                    }
                },
                "app.data.dataset.chart.saveQuery.callback.saveQuery",
                tagName
            );
        }).catch(tokenerror => {
            api.modal.error(tokenerror);
        });
    }
};

app.data.dataset.chart.saveQuery.callback.saveQuery = function (data, tagName) {
    $("#data-dataset-save-query").modal("hide");
    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [tagName]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};
//#endregion