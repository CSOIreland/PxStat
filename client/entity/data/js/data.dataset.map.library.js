/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
app.data = app.data || {};
app.data.dataset = app.data.dataset || {};
app.data.dataset.map = {};
app.data.dataset.map.template = {};
app.data.dataset.map.ajax = {};
app.data.dataset.map.callback = {};
app.data.dataset.map.apiParamsData = {}
app.data.dataset.map.snippetConfig = {};
app.data.dataset.map.configuration = {};
app.data.dataset.map.template.wrapper = {
    "autoupdate": true,
    "copyright": false,
    "link": null,
    "title": null,
    "type": 'choropleth',
    "data": {
        "labels": null,
        "datasets": [
            {
                "api": {
                    "query": {
                        "url": null,
                        "data": {
                            "jsonrpc": C_APP_API_JSONRPC_VERSION,
                            "method": null,
                            "params": {}
                        }
                    },
                    "response": {}
                },
                "outline": null,
                "data": [],
            },
        ],
    },
    "options": app.config.plugin.chartJs.options.map
};
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
    }).on('select2:select', app.data.dataset.map.buildMapConfig).on('select2:clear', function (e) {

    }).prop("disabled", false);
    app.data.dataset.map.buildMapConfig();
};

app.data.dataset.map.buildMapConfig = function () {
    app.data.dataset.map.configuration = {};
    $.extend(true, app.data.dataset.map.configuration, app.data.dataset.map.template.wrapper);
    app.data.dataset.map.buildApiParams();
    app.data.dataset.map.configuration.data.datasets[0].api.query.data.params = app.data.dataset.map.apiParamsData;


    if (app.data.isLive) {
        app.data.dataset.map.configuration.data.datasets[0].api.query.data.params.extension.matrix = app.data.MtrCode;
        app.data.dataset.map.configuration.data.datasets[0].api.query.url = app.config.url.api.jsonrpc.public;
        app.data.dataset.map.configuration.data.datasets[0].api.query.data.method = "PxStat.Data.Cube_API.ReadDataset";
        delete app.data.dataset.map.configuration.data.datasets[0].api.query.data.params.extension.release;
    }
    else {
        app.data.dataset.map.configuration.data.datasets[0].api.query.data.params.extension.release = app.data.RlsCode;
        app.data.dataset.map.configuration.data.datasets[0].api.query.url = app.config.url.api.jsonrpc.private;
        app.data.dataset.map.configuration.data.datasets[0].api.query.data.method = "PxStat.Data.Cube_API.ReadPreDataset";
        delete app.data.dataset.map.configuration.data.datasets[0].api.query.data.params.extension.matrix;
    }

    pxWidget.draw.init(C_APP_PXWIDGET_TYPE_MAP, "pxwidget-map", app.data.dataset.map.configuration, function () {
        app.data.dataset.map.snippetConfig = {};
        $.extend(true, app.data.dataset.map.snippetConfig, pxWidget.draw.params["pxwidget-map"]);
        app.data.dataset.map.renderSnippet();
    });
}

app.data.dataset.map.renderSnippet = function () {
    var snippet = app.config.entity.data.snippet;
    var config = $.extend(true, {}, app.data.dataset.map.snippetConfig);
    config.autoupdate = $("#data-dataset-map-accordion-collapse-widget").find("[name=auto-update]").is(':checked');
    config.link = $("#data-dataset-map-accordion-collapse-widget").find("[name=include-link]").is(':checked') ? app.config.url.application + C_COOKIE_LINK_TABLE + "/" + app.data.MtrCode : null;
    config.copyright = $("#data-dataset-map-accordion-collapse-widget").find("[name=include-copyright]").is(':checked');

    if ($("#data-dataset-map-accordion-collapse-widget").find("[name=include-title]").is(':checked')) {
        config.title = app.data.dataset.metadata.jsonStat.label.trim();
    }

    if ($("#data-dataset-map-accordion-collapse-widget").find("[name=auto-update]").is(':checked')) {
        $.each(config.data.datasets, function (key, value) {
            value.api.response = {};
        });

    } else {
        $.each(config.data.datasets, function (key, value) {
            value.api.query = {};
        });
    }

    //add custom JSON
    try {
        var customOptions = JSON.parse($("#data-dataset-map-accordion [name=custom-config]").val().trim());
        $.extend(true, config, customOptions);
        $("#data-dataset-map-accordion [name=invalid-json-object]").hide();
    } catch (err) {
        $("#data-dataset-map-accordion [name=invalid-json-object]").show();
    }

    snippet = snippet.sprintf([C_APP_URL_PXWIDGET_ISOGRAM, C_APP_PXWIDGET_TYPE_MAP, app.library.utility.randomGenerator('pxwidget'), JSON.stringify(config)]);

    $("#data-dataset-map-accordion-snippet-code").hide().text(snippet.trim()).fadeIn();
    Prism.highlightAll();
};

app.data.dataset.map.buildApiParams = function () {
    var localParams = {
        "class": C_APP_JSONSTAT_QUERY_CLASS,
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
        "version": C_APP_JSONSTAT_QUERY_VERSION,
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

app.data.dataset.map.formatJson = function () {
    $("#data-dataset-map-accordion [name=invalid-json-object]").hide();
    if ($("#data-dataset-map-accordion [name=custom-config]").val().trim().length) {
        var ugly = $("#data-dataset-map-accordion [name=custom-config]").val().trim();
        var obj = null;
        var pretty = null;
        try {
            obj = JSON.parse(ugly);
            pretty = JSON.stringify(obj, undefined, 4);
            $("#data-dataset-map-accordion [name=custom-config]").val(pretty);
        } catch (err) {
            $("#data-dataset-map-accordion [name=invalid-json-object]").show();
        }
    }
}
