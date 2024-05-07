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
app.data.dataset.map.saveQuery = {};
app.data.dataset.map.saveQuery.configuration = {};
app.data.dataset.map.saveQuery.ajax = {};
app.data.dataset.map.saveQuery.callback = {};
app.data.dataset.map.template.wrapper = {
    "autoupdate": true,
    "matrix": null,
    "mapDimension": null,
    "copyright": true,
    "link": null,
    "title": null,
    "borders": true,
    "colorScale": "red",
    "tooltipTitle": null,
    "defaultContent": app.config.entity.data.datatable.null,
    "fullScreen": {
        "title": app.label.static["view-fullscreen"],
        "titleCancel": app.label.static["exit-fullscreen"]
    },
    "data": {
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
                "fluidTime": []
            },
        ]
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
        "mode": null,
        "geojson": null,
        "identifier": null,
        "geometryType": null
    }
};
//#endregion

app.data.dataset.map.getColourSchemes = function () {
    $.each(app.config.plugin.leaflet.colourScale, function (index, value) {
        $("#data-dataset-map-accordion-collapse-widget").find("[name=colour-scale]").append($("<option>", {
            "text": app.label.static[value.name],
            "value": value.value
        }));
    });
};

app.data.dataset.map.getModes = function () {
    $.each(app.config.plugin.leaflet.mode, function (index, value) {
        $("#data-dataset-map-nav-content").find("[name=mode-select]").append($("<option>",
            {
                "value": value.value,
                "text": app.label.static[value.label]
            }
        ));
    });
    //set default
    $("#data-dataset-map-nav-content").find("[name=mode-select] option[value='" + app.config.plugin.leaflet.defaultValue + "']").attr("selected", true);
};



app.data.dataset.map.drawMapToDisplay = function () {
    var geoDimensions = app.data.dataset.metadata.jsonStat.Dimension({ role: "geo" });
    $("#data-dataset-map-nav-content").find("[name=dimension-containers]").empty();
    var geoSelectContainer = $("#data-dataset-map-templates").find("[name=dimension-container-map]").clone();
    geoSelectContainer.find("[name=dimension-label]").text(app.label.static["map"]);
    geoSelectContainer.find("[name=dimension-count]").text(geoDimensions.length);
    $.each(geoDimensions, function (index, value) {
        geoSelectContainer.find("select").attr("name", "geo-select").append($("<option>",
            {
                "value": app.data.dataset.metadata.jsonStat.role.geo[index],
                "title": value.label,
                "text": value.label
            }
        ));

    });

    $("#data-dataset-map-nav-content").find("[name=geo-select-container]").append(geoSelectContainer.get(0).outerHTML);

    $("#data-dataset-map-nav-content [name=geo-select-container] select").select2({
        dropdownParent: $('#data-dataset-map-nav-content'),
        minimumInputLength: 0,
        allowClear: false,
        width: '100%',
        placeholder: app.label.static["start-typing"]
    }).on('select2:select', app.data.dataset.map.drawDimensions).prop("disabled", false);

    app.data.dataset.map.drawDimensions();
};

app.data.dataset.map.drawDimensions = function () {
    var mapToDisplayId = $("#data-dataset-map-nav-content [name=geo-select-container] select").val();
    $("#data-dataset-map-nav-content").find("[name=dimension-containers]").empty();

    $.each(app.data.dataset.metadata.jsonStat.Dimension({ role: "metric" }), function (index, value) {
        if (app.data.dataset.metadata.jsonStat.role.metric[index] != mapToDisplayId) {
            var dimensionContainer = $("#data-dataset-map-templates").find("[name=dimension-container]").clone();
            dimensionContainer.find("[name=dimension-label]").html(value.label);
            dimensionContainer.find("[name=dimension-count]").text(value.length);
            dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.role.metric[index]).attr("role", value.role);
            $.each(value.id, function (variableIndex, variable) {
                dimensionContainer.find("select").append($("<option>",
                    {
                        "value": variable,
                        "title": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                        "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                    }
                ));
            });

            $("#data-dataset-map-nav-content").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
        }
    });

    $.each(app.data.dataset.metadata.jsonStat.Dimension({ role: "time" }), function (index, value) {
        if (app.data.dataset.metadata.jsonStat.role.time[index] != mapToDisplayId) {
            var dimensionContainer = $("#data-dataset-map-templates").find("[name=dimension-container]").clone();
            dimensionContainer.find("[name=dimension-label]").html(value.label);
            dimensionContainer.find("[name=dimension-count]").text(value.length);
            dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.role.time[index]).attr("role", value.role);
            $.each(value.id, function (variableIndex, variable) {
                dimensionContainer.find("select").append($("<option>",
                    {
                        "value": variable,
                        "title": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                        "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                    }
                ));
            });
            //reverse select based on codes so most recent time first
            dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
                return $(x).val() < $(y).val() ? 1 : -1;
            }));
            $("#data-dataset-map-nav-content").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
        };
    });

    $.each(app.data.dataset.metadata.jsonStat.Dimension(), function (index, value) {
        if (app.data.dataset.metadata.jsonStat.id[index] != mapToDisplayId) {
            if (value.role != "metric" && value.role != "time") {
                var dimensionContainer = $("#data-dataset-map-templates").find("[name=dimension-container]").clone();
                dimensionContainer.find("[name=dimension-label]").html(value.label);
                dimensionContainer.find("[name=dimension-count]").text(value.length);
                dimensionContainer.find("select").attr("idn", app.data.dataset.metadata.jsonStat.id[index]).attr("role", value.role);
                $.each(value.id, function (variableIndex, variable) {
                    dimensionContainer.find("select").append($("<option>",
                        {
                            "value": variable,
                            "title": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : ""),
                            "text": value.Category(variableIndex).label + (value.Category(variableIndex).unit ? " (" + value.Category(variableIndex).unit.label + ")" : "")
                        }
                    ));
                });

                $("#data-dataset-map-nav-content").find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
            }
        };
    });

    $("#data-dataset-map-nav-content [name=dimension-containers] select").select2({
        dropdownParent: $('#data-dataset-map-nav-content'),
        minimumInputLength: 0,
        allowClear: false,
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
    app.data.dataset.map.configuration.mapDimension = $("#data-dataset-map-nav-content [name=geo-select-container] select").val();
    app.data.dataset.map.configuration.options.mode = $("#data-dataset-map-nav-content").find("[name=mode-select]").val();
    app.data.dataset.map.configuration.baseMap = app.config.plugin.leaflet.baseMap;

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

    if (app.data.isModal) {
        $("#data-dataset-map-nav-content").find("[name=save-query-wrapper]").hide();
    }
    else {
        $("#data-dataset-map-nav-content").find("[name=save-query-wrapper]").show();
    }
}

app.data.dataset.map.renderSnippet = function () {

    if (app.data.RlsCode) {
        if (!app.data.isLive) {
            $("#data-dataset-map-accordion-collapse-widget").find("[name=link-to-wip-wrapper]").show();
        }
    }


    var snippet = app.config.entity.data.snippet;
    var config = $.extend(true, {}, app.data.dataset.map.snippetConfig);
    app.data.dataset.map.saveQuery.configuration = {};
    $.extend(true, app.data.dataset.map.saveQuery.configuration, app.data.dataset.map.snippetConfig);

    app.data.dataset.map.saveQuery.configuration.link = app.config.url.application + C_COOKIE_LINK_TABLE + "/" + app.data.MtrCode;
    app.data.dataset.map.saveQuery.configuration.copyright = true;


    config.autoupdate = $("#data-dataset-map-accordion-collapse-widget").find("[name=auto-update]").is(':checked');
    config.link = $("#data-dataset-map-accordion-collapse-widget").find("[name=include-link]").is(':checked') ? app.config.url.application + C_COOKIE_LINK_TABLE + "/" + app.data.MtrCode : null;
    config.copyright = $("#data-dataset-map-accordion-collapse-widget").find("[name=include-copyright]").is(':checked');
    config.colorScale = $("#data-dataset-map-accordion-collapse-widget").find("[name=colour-scale]").val();


    if ($("#data-dataset-map-accordion-collapse-widget").find("[name=link-to-wip]").is(':checked')) {
        config.matrix = app.data.MtrCode;
        $.each(config.data.datasets, function (key, value) {
            value.api.response = {};
        });
        config.metadata.api.query = {};
    }
    else {
        if ($("#data-dataset-map-accordion-collapse-widget").find("[name=fluid-time]").is(':checked')) {
            config = app.data.dataset.map.getFluidTime(config);
        }
        else {
            config.metadata.api.query = {};
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
    }

    if ($("#data-dataset-map-accordion-collapse-widget").find("[name=include-title]").is(':checked')) {
        config.title = $("#data-dataset-map-accordion-collapse-widget").find("[name=title-value]").val().trim();
    }

    config.borders = $("#data-dataset-map-accordion-collapse-widget").find("[name=include-borders]").is(':checked');

    //remove date if not live, can be present if prending live
    if (!app.data.isLive) {
        delete config.data.datasets[0].api.response.updated;
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
    if (config.options.geometryType != "Point") {
        $("#data-dataset-map-nav-content").find("[name=mode-select-wrapper]").show();
        $("#data-dataset-map-accordion-collapse-widget").find("[name=include-borders-wrapper]").show();
    }
    $("#data-dataset-map-accordion-snippet-code").hide().text(snippet.trim()).fadeIn();
    Prism.highlightAll();

    //Add map disclainer if exists
    var disclaimer = "";
    $.each(app.config.plugin.leaflet.baseMap.esri, function (index, value) {
        if (value.disclaimer.length) {
            if (index > 0) {
                disclaimer += "<br>"
            }
            disclaimer += value.disclaimer.trim();
        }
    });
    if (disclaimer.length) {
        $("#data-dataset-map-accordion-collapse-widget").find("[name=map-discalimer]").find("[data-bs-toggle=popover]").popover({
            "content": disclaimer,
            "html": true
        });
        $("#data-dataset-map-accordion-collapse-widget").find("[name=map-discalimer]").show();
    }
};

app.data.dataset.map.getFluidTime = function (config) {
    config.metadata.api.query.data.params.matrix = app.data.MtrCode;
    config.metadata.api.query.url = app.config.url.api.jsonrpc.public;
    var dimensionSize = app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id.length;
    var timePointSelected = config.data.datasets[0].api.query.data.params.dimension[app.data.dataset.metadata.timeDimensionCode].category.index[0];
    var actualPosition = $.inArray(timePointSelected, app.data.dataset.metadata.jsonStat.Dimension(app.data.dataset.metadata.timeDimensionCode).id);
    var relativePosition = (dimensionSize - actualPosition) - 1;
    config.data.datasets[0].fluidTime = [relativePosition];
    return config;
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

//#region save query
app.data.dataset.map.saveQuery.drawSaveQueryModal = function () {
    app.data.dataset.saveQuery.validation.drawSaveQuery();
    $("#data-dataset-save-query").modal("show");
};

app.data.dataset.map.saveQuery.ajax.saveQuery = function () {

    //now clone it into new variable for ajax which may or may not need fluid time
    var saveQueryConfig = $.extend(true, {}, app.data.dataset.map.saveQuery.configuration);
    if ($("#data-dataset-save-query").find("[name=fluid-time]").is(':checked')) {
        saveQueryConfig = app.data.dataset.map.getFluidTime(saveQueryConfig);
        saveQueryConfig.metadata.api.query.data.params.matrix = app.data.MtrCode;
        saveQueryConfig.metadata.api.query.url = app.config.url.api.jsonrpc.public;
    }
    else {
        saveQueryConfig.metadata.api.query = {};
        saveQueryConfig.fluidTime = [];
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
                    "Type": C_APP_PXWIDGET_TYPE_MAP,
                    "Query": base64snippet,
                    "FluidTime": $("#data-dataset-save-query").find("[name=fluid-time]").is(':checked'),
                    "Isogram": C_APP_URL_PXWIDGET_ISOGRAM
                }
            },
            "app.data.dataset.map.saveQuery.callback.saveQuery",
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
                        "Type": C_APP_PXWIDGET_TYPE_MAP,
                        "Query": base64snippet,
                        "FluidTime": $("#data-dataset-save-query").find("[name=fluid-time]").is(':checked'),
                        "Isogram": C_APP_URL_PXWIDGET_ISOGRAM
                    }
                },
                "app.data.dataset.map.saveQuery.callback.saveQuery",
                tagName
            );
        }).catch(tokenerror => {
            api.modal.error(tokenerror);
        });
    }
};

app.data.dataset.map.saveQuery.callback.saveQuery = function (data, tagName) {
    $("#data-dataset-save-query").modal("hide");
    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [tagName]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};
//#endregion
