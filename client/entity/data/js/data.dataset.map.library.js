/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
app.data = app.data || {};
app.data.dataset = app.data.dataset || {};
app.data.dataset.map = {};
app.data.dataset.map.ajax = {};
app.data.dataset.map.callback = {};
app.data.dataset.map.apiParamsData = {}


//#endregion

app.data.dataset.map.drawDimensions = function () {
    var dimensions = app.data.dataset.metadata.jsonStat.Dimension();
    $("#data-dataset-map-nav-content").find("[name=dimension-containers]").empty();
    $.each(dimensions, function (index, value) {
        if (value.role != "geo") {
            var dimensionContainer = $("#data-dataset-map-templates").find("[name=dimension-container]").clone();
            var dimensionCode = $("<small>", {
                "text": " - " + app.data.dataset.metadata.jsonStat.id[index],
                "name": "dimension-code",
                "class": "d-none"
            }).get(0).outerHTML;
            dimensionContainer.find("[name=dimension-label]").html(value.label + dimensionCode);
            dimensionContainer.find("[name=dimension-count]").text(value.length);
            dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("role", value.role);
            $.each(value.id, function (variableIndex, variable) {
                dimensionContainer.find("select").append($("<option>",
                    {
                        "value": variable,
                        "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                    }
                ));
            });
            if (value.role == "time") {
                //reverse select based on codes so most recent time first
                dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
                    return $(x).val() < $(y).val() ? 1 : -1;
                }));
            }
            $("#data-dataset-map-nav-content").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
        }
    });

    $("#data-dataset-map-nav-content").find("[name=dimension-containers] select").select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"]
    }).on('select2:select', function (e) {
        app.data.dataset.map.ajax.readDataset();
    }).on('select2:clear', function (e) {

    }).prop("disabled", false);
    app.data.dataset.map.ajax.readDataset();
};

app.data.dataset.map.ajax.readDataset = function () {
    app.data.dataset.map.buildApiParams();
    if (app.data.isLive) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.Data.Cube_API.ReadDataset",
            app.data.dataset.map.apiParamsData,
            "app.data.dataset.map.callback.readDataset",
            null,
            null,
            null,
            { async: false }
        );
    }

    else {
        api.ajax.jsonrpc.request(
            app.config.url.api.private,
            "PxStat.Data.Cube_API.ReadPreDataset",
            app.data.dataset.map.apiParamsData,
            "app.data.dataset.map.callback.readDataset",
            null,
            null,
            null,
            { async: false }
        );
    }
};

app.data.dataset.map.callback.readDataset = function (data) {
    var data = data ? JSONstat(data) : null;
    if (data && data.length) {
        var chartTitle = app.data.fileNamePrefix + ": " +
            $("#data-dataset-map-nav-content").find("[name=dimension-containers]").find("select[role=metric]").find("option:selected").text() + ", " +
            $("#data-dataset-map-nav-content").find("[name=dimension-containers]").find("select[role=time]").find("option:selected").text() + ", ";
        var totalClassifications = $("#data-dataset-map-nav-content").find("[name=dimension-containers]").find("select[role=classification]").length;
        $("#data-dataset-map-nav-content").find("[name=dimension-containers]").find("select[role=classification]").each(function (index) {
            if (index != totalClassifications - 1) {
                chartTitle = chartTitle + $(this).find("option:selected").text() + ", ";
            }
            else {
                chartTitle = chartTitle + $(this).find("option:selected").text();
            }
        });
        //for each region, create a data point
        var mapData = [];
        var values = []; //used to find min/max for color scale
        $.each(data.Dimension({ role: "geo" })[0].id, function (indexGeo, valueGeo) {
            var dataQueryObj = {};
            var mapDataObj = {
                code: valueGeo,
                value: null,
                tooltip: {
                    [data.Dimension(data.role.geo[0]).label]: data.Dimension(data.role.geo[0]).Category(valueGeo).label
                }
            };
            dataQueryObj[data.role.geo[0]] = valueGeo;
            $.each(data.Dimension(), function (indexDimension, valueDimension) {
                if (valueDimension.role != "geo") {
                    dataQueryObj[data.id[indexDimension]] = $("#data-dataset-map-nav-content").find("[name=dimension-containers]").find("select[idn='" + data.id[indexDimension] + "']").val();
                    mapDataObj.tooltip[valueDimension.label] = $("#data-dataset-map-nav-content").find("[name=dimension-containers]").find("select[idn='" + data.id[indexDimension] + "'] option:selected").text();
                }
            });
            if (valueGeo != "-") {
                mapDataObj.value = data.Data(dataQueryObj).value;
                mapData.push(mapDataObj);
                values.push(data.Data(dataQueryObj).value);
            }

        });

        var buttons = Highcharts.getOptions().exporting.buttons.contextButton.menuItems;
        $.getJSON(data.Dimension({ role: "geo" })[0].link.enclosure[0].href, function (geojson) {
            // Initiate the chart
            Highcharts.mapChart($("#data-dataset-map-nav-content").find("[name=highmap-container]")[0], {
                chart: {
                    map: geojson,
                    height: 500,
                    backgroundColor: '#FFFFFF'
                },
                title: {
                    text: null
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
                    min: Math.min.apply(Math, values.filter(function (el) {
                        return el != null;
                    })),
                    max: Math.max.apply(Math, values.filter(function (el) {
                        return el != null;
                    })),
                    type: 'linear',
                    minColor: "#FFFF00",
                    maxColor: "#ff0000"
                },
                series: [{
                    data: mapData,
                    nullInteraction: true,
                    joinBy: ["AREA_ID", 'code'],
                    states: {
                        hover: {
                            borderColor: 'gray'
                        }
                    }
                }],
                tooltip: {
                    useHTML: true,
                    formatter: function () {
                        if (this.point.options.tooltip) {
                            var tooltip = $("<span>", {});
                            $.each(this.point.options.tooltip, function (key, value) {
                                tooltip.append(
                                    $("<span>", {
                                        text: key + " : " + value
                                    })
                                )
                                tooltip.append($("<br>"))
                            });
                            var value = this.point.options.value == null ? ".." : this.point.options.value;
                            tooltip.append(
                                $("<span>", {
                                    text: value == ".." ? app.label.static["value"] + " : " + app.library.utility.formatNumber(value) + " (" + app.label.static["confidential"] + ")" : app.label.static["value"] + " : " + app.library.utility.formatNumber(value)
                                })
                            )
                            return tooltip.get(0).innerHTML;
                        }
                        else {
                            return app.label.static["not-available"]
                        }
                    },
                },
                exporting: {
                    buttons: {
                        contextButton: {
                            menuItems: buttons.slice(3, 5)
                        }
                    },
                    allowHTML: true,
                    filename: data.label.replace(/ /g, "_").toLowerCase() + '.' + moment(Date.now()).format(app.config.mask.datetime.file),
                    chartOptions: {
                        title: {
                            text: chartTitle
                        }
                    }
                }
            });
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
}

app.data.dataset.map.buildApiParams = function () {
    var localParams = {
        "class": "query",
        "id": [],
        "dimension": {},
        "extension": {
            "language": {
                "code": app.data.LngIsoCode
            },
            "format": {
                "type": C_APP_FORMAT_TYPE_DEFAULT,
                "version": C_APP_FORMAT_VERSION_DEFAULT
            }
        },
        "version": "2.0",
        "m2m": false
    };

    if (app.data.isLive) {
        localParams.extension.matrix = app.data.MtrCode;
    }
    else {
        localParams.extension.release = app.data.RlsCode;
    }

    $("#data-dataset-map-nav-content").find("[name=dimension-containers]").find("select").each(function (index) {
        var dimension = {
            "category": {
                "index": []
            }
        };
        dimension.category.index.push($(this).val());
        localParams.id.push($(this).attr("idn"));
        localParams.dimension[$(this).attr("idn")] = dimension;
    });

    //new query, empty old api params
    app.data.dataset.map.apiParamsData = {};
    //extend apiParams with local params
    $.extend(true, app.data.dataset.map.apiParamsData, localParams);
};