/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces
app.data = app.data || {};

app.data.dataview = {};
app.data.dataview.ajax = {};
app.data.dataview.callback = {};
app.data.dataview.apiParamsMap = {};
//#endregion
//#region map data

/**
* 
* @param {*} apiParams
*/
app.data.dataview.ajax.mapMetadata = function (apiParams) {
    if (app.data.MtrCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.Data.Cube_API.ReadMetadata",
            apiParams,
            "app.data.dataview.callback.mapMetadata");
    }

    else if (app.data.RlsCode) {
        delete apiParams.matrix;
        apiParams.release = app.data.RlsCode;
        api.ajax.jsonrpc.request(app.config.url.api.private,
            "PxStat.Data.Cube_API.ReadPreMetadata",
            apiParams,
            "app.data.dataview.callback.mapMetadata");
    }
};

/**
* 
* @param {*} response
*/
app.data.dataview.callback.mapMetadata = function (response) {
    //FIXME: Does not show MAP if User SELECTED language pl (matrix in en only)
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        var data = JSONstat(response.data);
        var geo = data.role.geo[0];

        $("#data-dataview-selected-table").find("[name=map-dimensions]").empty();
        for (i = 0; i < data.length; i++) {
            var dimension = data.Dimension(i);
            if (dimension.label != geo) {
                var dimensionContainer = $("#data-dataset-templates").find("[name=dimension-container-map]").clone();
                dimensionContainer.find("[name=dimension-label]").text(dimension.label);
                dimensionContainer.find("[name=dimension-count]").text(dimension.id.length);
                if (data.Dimension(i).role == "time") {
                    $.each(data.Dimension(i).id, function (index, value) {
                        var option = $('<option>', {
                            value: value,
                            text: data.Dimension(i).Category(index).label,
                            title: data.Dimension(i).Category(index).label
                        });
                        dimensionContainer.find("select").append(option);
                    });
                    dimensionContainer.find("select").attr("idn", data.id[i]).attr("role", data.Dimension(i).role).attr("sort", "desc");
                    //reverse select so most recent time first
                    dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
                        return $(x).text() < $(y).text() ? 1 : -1;
                    }));
                    dimensionContainer.find("select").prop("selectedIndex", 0);
                }
                else {
                    $.each(data.Dimension(i).id, function (index, value) {
                        var option = $('<option>', {
                            value: value,
                            text: data.Dimension(i).Category(index).label + (data.Dimension(i).Category(index).unit ? " (" + data.Dimension(i).Category(index).unit.label + ")" : ""),
                            title: data.Dimension(i).Category(index).label + (data.Dimension(i).Category(index).unit ? " (" + data.Dimension(i).Category(index).unit.label + ")" : ""),
                        });
                        dimensionContainer.find("select").append(option);
                    });
                    dimensionContainer.find("select").attr("idn", data.id[i]).attr("role", data.Dimension(i).role);
                }
                $("#data-dataview-selected-table").find("[name=map-dimensions]").append(dimensionContainer);
            }
        }
        $("#data-dataview-selected-table").find("[name=map-dimensions]").find("select").once("change", function () {
            app.data.dataview.getMapSelection();
        });

        app.data.dataview.getMapSelection();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* 
*/
app.data.dataview.getMapSelection = function () {
    if (app.data.MtrCode) {
        app.data.dataview.apiParamsMap = {
            "matrix": app.data.MtrCode
        };
    }
    else if (app.data.RlsCode) {
        app.data.dataview.apiParamsMap = {
            "release": app.data.RlsCode
        };
    }

    var localParams = {
        "language": app.data.LngIsoCode,
        "FrmType": C_APP_FORMAT_TYPE_DEFAULT,
        "FrmVersion": C_APP_FORMAT_VERSION_DEFAULT,
        "role": {
            "time": [
                $("#data-dataview-selected-table").find("[name=map-container]").find("select[role=time]").attr("idn")
            ],
            "metric": [
                $("#data-dataview-selected-table").find("[name=map-container]").find("select[role=metric]").attr("idn")
            ]
        },
        "dimension": [
            {
                "id": $("#data-dataview-selected-table").find("[name=map-container]").find("select[role=time]").attr("idn"),
                "category": {
                    "index": [
                        $("#data-dataview-selected-table").find("[name=map-container]").find("select[role=time]").val()
                    ]
                }
            },
            {
                "id": $("#data-dataview-selected-table").find("[name=map-container]").find("select[role=metric]").attr("idn"),
                "category": {
                    "index": [
                        $("#data-dataview-selected-table").find("[name=map-container]").find("select[role=metric]").val()
                    ]
                }
            }
        ],
        "m2m": false
    };

    $($("#data-dataview-selected-table").find("[name=map-container]").find("select[role=classification]")).each(function (index) {

        var dimension = {
            "id": $(this).attr("idn"),
            "category": {
                "index": [
                    $(this).val()
                ]
            }
        };
        localParams.dimension.push(dimension);
    });
    //extend apiParams with local params
    $.extend(true, app.data.dataview.apiParamsMap, localParams);
    $("#data-accordion-api").find("[name=api-object]").text(function () {
        var JsonQuery = {
            "jsonrpc": C_APP_API_JSONRPC_VERSION,
            "method": "PxStat.Data.Cube_API.ReadDataset",
            "params": null,
            "id": app.library.utility.randomGenerator()
        };
        var apiParams = $.extend(true, {}, app.data.dataview.apiParamsMap);
        delete apiParams.m2m;
        JsonQuery.params = apiParams;
        return JSON.stringify(JsonQuery, null, "\t");
    });
    // Refresh the Prism highlight
    Prism.highlightAll();
    app.data.dataview.ajax.mapData();
};

/**
* 
*/
app.data.dataview.ajax.mapData = function () {
    if (app.data.MtrCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.Data.Cube_API.ReadDataset",
            app.data.dataview.apiParamsMap,
            "app.data.dataview.callback.mapData",
            null,
            null,
            null,
            { async: false }
        );
    }

    else if (app.data.RlsCode) {
        delete app.data.dataview.apiParamsMap.matrix;
        app.data.dataview.apiParamsMap.release = app.data.RlsCode;
        api.ajax.jsonrpc.request(
            app.config.url.api.private,
            "PxStat.Data.Cube_API.ReadPreDataset",
            app.data.dataview.apiParamsMap,
            "app.data.dataview.callback.mapData",
            null,
            null,
            null,
            { async: false }
        );
    }
};

/**
* 
* @param {*} response
*/
app.data.dataview.callback.mapData = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        var ds = JSONstat(response.data);
        var statisticLabel = ds.role.metric[0];
        var chartTitle = app.data.fileNamePrefix + ": " +
            $("#data-dataview-selected-table").find("[name=map-dimensions]").find("[name=dimension-select][role=metric]").find("option:selected").text() + ", " +
            $("#data-dataview-selected-table").find("[name=map-dimensions]").find("[name=dimension-select][role=time]").find("option:selected").text() + ", ";
        var totalClassifications = $("#data-dataview-selected-table").find("[name=map-dimensions]").find("[name=dimension-select][role=classification]").length;
        $("#data-dataview-selected-table").find("[name=map-dimensions]").find("[name=dimension-select][role=classification]").each(function (index) {
            if (index != totalClassifications - 1) {
                chartTitle = chartTitle + $(this).find("option:selected").text() + ", ";
            }
            else {
                chartTitle = chartTitle + $(this).find("option:selected").text();
            }
        });
        for (i = 0; i < ds.length; i++) {
            if (ds.Dimension(i).link) {
                var mapUrl = ds.Dimension(i).link.enclosure[0].href;
                var codes = ds.Dimension(i).id;
                var values = ds.value;
                var valuesMinMax = []; //to set min and max of colour axis
                var data = [];
                for (x = 0; x < codes.length; x++) {
                    if (codes[x] != '-') {
                        valuesMinMax.push(values[x]);
                        var statisticValue = $("#data-dataview-selected-table").find("[name=dimension-container-map]").find("[name=dimension-select][role=metric]").find("option:selected").val();
                        var dataPoint = {
                            label: ds.Dimension(i).label,
                            name: ds.Dimension(i).Category(codes[x]).label,
                            value: values[x],
                            code: codes[x],
                            unit: ds.Dimension(statisticLabel).Category(statisticValue).unit.label
                        };
                        data.push(dataPoint);
                    }
                }
            }
            var buttons = Highcharts.getOptions().exporting.buttons.contextButton.menuItems;
            Highcharts.setOptions({
                lang: {
                    thousandsSep: app.config.separator.thousand.display,
                    decimalPoint: app.config.separator.decimal.display
                }
            });
            $.getJSON(mapUrl, function (geojson) {
                // Initiate the chart
                Highcharts.mapChart($('#data-dataview-selected-table').find('[name=highmap-container]')[0], {
                    chart: {
                        map: geojson,
                        height: 500
                    },
                    title: {
                        text: ''
                    },
                    credits: {
                        enabled: false
                    },
                    mapNavigation: {
                        enabled: true,
                        buttonOptions: {
                            verticalAlign: 'bottom'
                        }
                    },
                    colorAxis: {
                        min: Math.min.apply(Math, valuesMinMax),
                        max: Math.max.apply(Math, valuesMinMax),
                        type: 'linear',
                        minColor: app.config.plugin.highmaps.minColor,
                        maxColor: app.config.plugin.highmaps.maxColor
                    },
                    series: [{
                        data: data,
                        nullInteraction: true,
                        joinBy: [app.config.plugin.highmaps.featureIdentifier, 'code'],
                        states: {
                            hover: {
                                borderColor: 'gray'
                            }
                        }
                    }],
                    tooltip: {
                        useHTML: app.config.plugin.highmaps.useHTML,
                        formatter: function () {
                            var label = this.point.options.label || app.label.static["not-available"];
                            var name = this.point.options.name || app.label.static["not-available"];
                            var unit = this.point.options.unit || app.label.static["not-available"];
                            var value = this.point.options.value == null ? app.config.entity.data.datatable.null : this.point.options.value;
                            var dataViewMapLabel = $("#data-dataview-templates").find("[name=data-view-map-label]").clone();
                            dataViewMapLabel.find("[name=label]").text(label);
                            dataViewMapLabel.find("[name=name]").text(name);
                            dataViewMapLabel.find("[name=unit]").text(unit);
                            dataViewMapLabel.find("[name=value]").text(app.library.utility.formatNumber(value));
                            return dataViewMapLabel.get(0).innerHTML;
                        },
                    },
                    exporting: {
                        buttons: {
                            contextButton: {
                                menuItems: buttons.slice(3, 5)
                            }
                        },
                        allowHTML: true,
                        filename: app.data.fileNamePrefix + '.' + moment(Date.now()).format(app.config.mask.datetime.file),
                        chartOptions: {
                            title: {
                                text: chartTitle
                            }
                        }
                    },
                });
            });
        }
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion
//#region get table data

/**
* 
*/
app.data.dataview.ajax.data = function () {
    if (app.data.MtrCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.Data.Cube_API.ReadDataset",
            app.data.dataset.apiParamsData,
            "app.data.dataview.callback.data",
            null,
            null,
            null,
            { async: false }
        );
    }

    else if (app.data.RlsCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.private,
            "PxStat.Data.Cube_API.ReadPreDataset",
            app.data.dataset.apiParamsData,
            "app.data.dataview.callback.data",
            null,
            null,
            null,
            { async: false }
        );
    }
};


app.data.dataview.ajax.format = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.System.Settings.Format_API.Read",
        {
            "LngIsoCode": app.data.LngIsoCode,
            "FrmDirection": C_APP_TS_FORMAT_DIRECTION_DOWNLOAD
        },
        "app.data.dataview.callback.format"
    );
}

app.data.dataview.callback.format = function (response) {
    if (response.error) {
        // Handle the Error in the Response first
        api.modal.error(response.error.message);
    }

    else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        api.modal.information(app.label.static["api-ajax-nodata"]);
    }
    else if (response.data) {
        $("#data-dataview-row [name=download-select-dataset]").empty();
        $.each(response.data, function (index, format) {
            var formatDropdown = $("#data-dataview-templates").find("[name=download-dataset-format]").clone();
            formatDropdown.attr(
                {
                    "frm-type": format.FrmType,
                    "frm-version": format.FrmVersion
                });
            formatDropdown.find("[name=type]").text(format.FrmType);
            formatDropdown.find("[name=version]").text(format.FrmVersion);

            $("#data-view-container [name=download-select-dataset]").append(formatDropdown);
        });

        $("#data-dataview-row").find("[name=download-dataset-format]").once("click", function (e) {
            e.preventDefault();
            app.data.dataview.callback.resultsDownload($(this).attr("frm-type"), $(this).attr("frm-version"));
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

}
/**
* 
* @param {*} response
*/
app.data.dataview.callback.data = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        var ds = JSONstat(response.data);
        app.data.dataview.callback.drawDatatable(ds);
        app.data.dataview.ajax.format();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.data.dataview.drawCallbackDrawDataTable = function () {
    //check whether to show codes or not every time the table is drawn
    if (!$('#code-toggle').prop('checked')) {
        $("#data-view-container").find("[name=datatable]").find("[name=code]").removeClass("d-none");
    }
    else {
        $("#data-view-container").find("[name=datatable]").find("[name=code]").addClass("d-none");
    }

    //Data Table data Show Codes button
    $('#code-toggle').once("change", function () {
        if (!$(this).prop('checked')) {
            $("#data-view-container").find("[name=datatable]").find("[name=code]").removeClass("d-none");
        }
        else {
            $("#data-view-container").find("[name=datatable]").find("[name=code]").addClass("d-none");
        }
    });

    api.spinner.stop();
    $("#data-view-container").fadeIn();
}

/**
 * Download results
 */
app.data.dataview.callback.resultsDownload = function (format, version) {
    var apiParams = $.extend(true, {}, app.data.dataset.apiParamsData);
    apiParams.FrmType = format;
    apiParams.FrmVersion = version;
    app.data.dataset.ajax.downloadDataset(apiParams);
}

/**
* 
* @param {*} ds
*/
app.data.dataview.callback.drawDatatable = function (ds) {
    api.spinner.start();

    if ($.fn.DataTable.isDataTable("#data-view-container [name=datatable]")) {
        $("#data-view-container").find("[name=datatable]").DataTable().destroy();
        //cannot use redraw as columns are dynamically created depending on the matrix. Have to destroy and re-initiate 
    }
    var dataContainer = $("#data-dataview-templates").find("[name=data-view]").clone();
    var jsonTableLabel = ds.toTable({
        type: 'arrobj',
        meta: true,
        unit: true,
        content: "label"
    });
    var jsonTableId = ds.toTable({
        type: 'arrobj',
        meta: true,
        unit: true,
        content: "id"
    });
    var numDimensions = ds.length;
    var tableColumns = [];
    for (i = 0; i < numDimensions; i++) { //build columns
        var codeSpan = $('<span>', {
            "name": "code",
            "class": "badge badge-pill badge-neutral mx-2 d-none",
            "text": ds.id[i]
        }).get(0).outerHTML;

        var tableHeading = $("<th>", {
            "html": ds.Dimension(i).label + codeSpan
        });
        //Data Table header
        dataContainer.find("[name=datatable]").find("[name=header-row]").append(tableHeading);

        tableColumns.push({
            data: ds.id[i],
            "createdCell": function (td, cellData, rowData, row, col) {
                $(td).attr("data-order", cellData);
            },
            render: function (data, type, row, meta) {
                var dimensionLabel = jsonTableId.meta.id[meta.col];
                var codeSpan = $('<span>', {
                    "name": "code",
                    "class": "badge badge-pill badge-neutral mx-2 d-none",
                    "text": jsonTableId.data[meta.row][dimensionLabel]
                }).get(0).outerHTML;
                return data + codeSpan;
            }
        });
    }
    var unitHeading = $("<th>",
        {
            "class": "unit"
        });
    unitHeading.html("Unit");
    dataContainer.find("[name=datatable]").find("[name=header-row]").append(unitHeading);
    tableColumns.push({
        "data": 'unit.label',
        "createdCell": function (td, cellData, rowData, row, col) {
            $(td).attr("data-order", cellData);
        },
    });
    tableColumns.push({
        "data": 'value',
        "type": "natural-nohtml",
        "class": "text-right",
        "defaultContent": app.config.entity.data.datatable.null,
        "render": function (data, type, row, meta) {
            return app.library.utility.formatNumber(data, app.config.separator.decimal.display, app.config.separator.thousand.display, row.unit.decimals);
        }
    });
    var valueHeading = $("<th>",
        {
            "class": "value text-right"
        });
    valueHeading.html("Value");
    dataContainer.find("[name=datatable]").find("[name=header-row]").append(valueHeading);
    $("#data-view-container").html(dataContainer.get(0).outerHTML);
    //Draw DataTable with Data Set data
    var localOptions = {
        iDisplayLength: app.config.entity.data.datatable.length,
        data: jsonTableLabel.data,
        columns: tableColumns,
        drawCallback: function (settings) {
            app.data.dataview.drawCallbackDrawDataTable();
        },
        //Translate labels language
        language: app.label.plugin.datatable
    };
    $("#data-view-container").find("[name=datatable]").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
        app.data.dataview.drawCallbackDrawDataTable();
    });

    $('[data-toggle="tooltip"]').tooltip();
    $('#code-toggle').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["false"],
        off: app.label.static["true"],
        onstyle: "tertiary",
        offstyle: "neutral",
        width: C_APP_TOGGLE_LENGTH
    });

    //scroll to top of table
    if (app.data.MtrCode) {
        $('html, body').animate({
            scrollTop: $("#data-view-container").offset().top
        }, 1000);
    }
    //Need to fix this
    /* else if (app.data.RlsCode) {
        $('html, body').animate({
            scrollTop: $("#data-view-container").offset().top
        },
            1000);
    } */
};


//#endregion