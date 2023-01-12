/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build = app.build || {};
app.build.update = app.build.update || {};

app.build.update.dimension = {};
app.build.update.dimension.ajax = {};
app.build.update.dimension.callback = {};
app.build.update.dimension.modal = {};
app.build.update.dimension.validation = {};
app.build.update.dimension.callback = {};
app.build.update.dimension.mapsValid = true;

//#endregion

/**
 * Draw Callback for Datatable
 */
app.build.update.dimension.callback.drawStatistic = function () {
}

/**
 *Draw statistics dimension
 * @param {*} lngIsoCode
 * @param {*} data
 */
app.build.update.dimension.drawStatistic = function (lngIsoCode, data) {

    var table = $("#build-update-dimension-nav-" + lngIsoCode).find("[name=statistics-added-table]");

    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {

        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            ordering: false,
            columns: [
                {
                    data: "SttCode"
                },
                {
                    data: "SttValue"
                },
                {
                    data: "SttUnit"
                },
                {
                    data: "SttDecimal"
                }
            ],
            drawCallback: function (settings) {
                app.build.update.dimension.callback.drawStatistic();
            },

        };

        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.dimension.callback.drawStatistic();
        });
    }
};

/**
 * Draw Callback for Datatable
 * @param {*} table
 * @param {*} lngIsoCode
 */
app.build.update.dimension.callback.drawClassification = function (table, lngIsoCode, classificationsData) {

    $(table).find("[name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
        e.preventDefault();
        //find data
        var clsCode = $(this).attr("cls-code");
        $.each(classificationsData, function (index, value) {
            if (value.ClsCode == clsCode) {
                $("#build-update-edit-classification [name=update-classification]").attr("lng-iso-code", lngIsoCode);
                app.build.update.dimension.modal.viewClassification(value);
                return;
            }
        });
    });

    $(table).find("[name=" + C_APP_NAME_LINK_GEOJSON + "]").once("click", function (e) {
        e.preventDefault();
        app.geomap.draw($(this).attr("geojson-url"), $(this).attr("cls-code"), $(this).attr("cls-value"));
    });
}

/**
 *Draw classification dimension
 * @param {*} lngIsoCode
 */
app.build.update.dimension.drawClassification = function (lngIsoCode, classificationData) {
    //Create accordion for classification
    var table = $("#build-update-dimension-nav-" + lngIsoCode).find("[name=classifications-added-table]");

    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, classificationData);
    } else {

        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: classificationData,
            ordering: false,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.view({ "cls-code": row.ClsCode }, row.ClsCode);
                    }
                },
                {
                    data: "ClsValue"
                },

                {
                    data: null,
                    render: function (data, type, row) {
                        return row.Variable.length;
                    }
                }
            ],
            drawCallback: function (settings) {
                app.build.update.dimension.callback.drawClassification(table, lngIsoCode, classificationData);
            },
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.dimension.callback.drawClassification(table, lngIsoCode, classificationData);
        });
    }
};

/**
 * Draw Callback for Datatable
 */
app.build.update.dimension.callback.drawExistingPeriod = function () {
    //nothing required. Leave for consistency
}

/**
 *Draw periods dimension
 * @param {*} lngIsoCode
 * @param {*} data
 */
app.build.update.dimension.drawExistingPeriod = function (lngIsoCode, data) {
    //sort descending 
    var datatableData = $.extend(true, [], data);
    datatableData.sort(function (a, b) {
        return b.PrdCode - a.PrdCode
    });
    var table = $("#build-update-dimension-nav-" + lngIsoCode).find("[name=periods-existing]");

    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, datatableData);
    } else {

        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: datatableData,
            ordering: false,
            columns: [
                {
                    data: "PrdCode"
                },
                {
                    data: "PrdValue"
                }
            ],
            drawCallback: function (settings) {
                app.build.update.dimension.callback.drawExistingPeriod();
            },

        };

        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.dimension.callback.drawExistingPeriod();
        });
    }
};


/**
 * Draw Callback for Datatable
 * @param {*} table
 * @param {*} lngIsoCode
 * 
 */
app.build.update.dimension.callback.drawNewPeriod = function (table, lngIsoCode) {
    // Delete action               
    $(table).find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        var params = {
            lngIsoCode: lngIsoCode,
            prdCode: $(this).attr("idn")
        };
        app.build.update.dimension.modal.deleteNewPeriod(params);
    });
}

/**
 *Draw periods dimension
 * @param {*} lngIsoCode
 * @param {*} data
 */
app.build.update.dimension.drawNewPeriod = function (lngIsoCode) {

    var data = null;
    $(app.build.update.data.Dimension).each(function (index, dimension) {
        if (lngIsoCode == dimension.LngIsoCode) {
            data = $.extend(true, [], dimension.Frequency.Period);
        }
    });
    //sort descending 
    data.sort(function (a, b) {
        return b.PrdCode - a.PrdCode
    });
    //enable disable download csv data depending on new periods added 
    if (!data.length) {
        $("#build-update-matrix-data").find("[name=download-data-file-new], [name=download-data-file-all]").addClass("disabled");
    }
    else {
        $("#build-update-matrix-data").find("[name=download-data-file-new], [name=download-data-file-all]").removeClass("disabled");
    }

    var table = $("#build-update-dimension-nav-" + lngIsoCode).find("[name=periods-new]");

    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {

        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            ordering: false,
            columns: [
                {
                    data: "PrdCode"
                },
                {
                    data: "PrdValue"
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.PrdCode }, false);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.build.update.dimension.callback.drawNewPeriod(table, lngIsoCode);
            },

        };

        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.dimension.callback.drawNewPeriod(table, lngIsoCode);
        });

    }
};


/**
 * Confirm deletion of classification for parameters entered.
 *
 * @param {*} params
 */
app.build.update.dimension.modal.deleteNewPeriod = function (params) {
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [params.prdCode]),
        app.build.update.dimension.callback.deleteNewPeriod,
        params
    );
};


/**
 * 
 */
app.build.update.dimension.callback.deleteNewPeriod = function (params) {
    $.each(app.build.update.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == params.lngIsoCode) {
            dimension.Frequency.Period = $.grep(dimension.Frequency.Period, function (value, index) {
                return value.PrdCode == params.prdCode ? false : true;
            });
        }
    });

    app.build.update.dimension.drawNewPeriod(params.lngIsoCode);
};

/**
 * 
 */
app.build.update.dimension.modal.viewClassification = function (classification) {
    //set current classification values
    $("#build-update-edit-classification [name=cls-code]").text(classification.ClsCode);
    $("#build-update-edit-classification [name=cls-value]").text(classification.ClsValue);

    $("#build-update-edit-classification").find("[name=title]").text(classification.ClsValue + ": " + classification.ClsCode);
    if ($.fn.dataTable.isDataTable("#build-update-edit-classification table")) {
        app.library.datatable.reDraw("#build-update-edit-classification table", classification.Variable);
    } else {
        var localOptions = {
            data: classification.Variable,
            ordering: false,
            columns: [
                { data: "VrbCode" },
                { data: "VrbValue" },
            ]
        };
        //Initialize DataTable
        $("#build-update-edit-classification table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions));
    }
    $("#build-update-edit-classification").modal("show");
};

//#region elimination
app.build.update.dimension.drawElimination = function () {
    //get default language jsonStat
    var defaultJSONstat = $.grep(app.build.update.ajax.jsonStat, function (n, i) { // just use arr
        return n.extension.language.code == app.config.language.iso.code;
    });

    var data = [];
    $.each(app.build.update.data.Elimination, function (key, value) {
        var classification = defaultJSONstat[0].Dimension(key);
        data.push({
            "ClsCode": key,
            "ClsValue": classification.label,
            "VrbEliminationCode": value || null,
            "VrbEliminationValue": value ? classification.Category(value).label : null,
        });

    });

    var table = $("#build-update-dimensions").find("[name=classification-elimination-variables]");
    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ "cls-code": row.ClsCode, "cls-value": row.ClsValue, "vrb-elimination-code": row.VrbEliminationCode }, row.ClsCode);
                    }
                },
                {
                    data: "ClsValue"
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.VrbEliminationCode || ""
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.VrbEliminationValue || ""
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ "idn": row.ClsCode, "cls-value": row.ClsValue }, row.VrbEliminationCode ? false : true);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.build.update.dimension.drawEliminationCallback(table);
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.dimension.drawEliminationCallback(table);
        });
    }
}

app.build.update.dimension.drawEliminationCallback = function (table) {
    $('[data-toggle="tooltip"]').tooltip();
    $(table).find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        app.build.update.dimension.modal.updateElimination($(this).attr("cls-value"), $(this).attr("cls-code"), $(this).attr("vrb-elimination-code"))
    });

    $(table).find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        api.modal.confirm(
            app.library.html.parseDynamicLabel("build-delete-elimination", [$(this).attr("cls-value")]),
            app.build.update.dimension.deleteElimination,
            $(this).attr("idn")
        );
    });
}

app.build.update.dimension.validation.updateElimination = function () {
    $("#build-update-modal-elimination form").trigger("reset").validate({
        rules: {
            "variable":
            {
                required: true,
            }
        },
        errorPlacement: function (error, element) {
            $("#build-update-modal-elimination [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.build.update.dimension.modal.saveElimination()
        }
    }).resetForm();
};

app.build.update.dimension.modal.updateElimination = function (clsValue, clsCode, clsEliminationCode) {
    app.build.update.dimension.validation.updateElimination();
    clsEliminationCode = clsEliminationCode || null;
    $("#build-update-modal-elimination").find("[name=cls-value]").text(app.label.static["classification"] + " : " + clsValue);

    //get default language jsonStat
    var defaultJSONstat = $.grep(app.build.update.ajax.jsonStat, function (n, i) { // just use arr
        return n.extension.language.code == app.config.language.iso.code;
    });


    $("#build-update-modal-elimination").find("[name=cls-code]").val(clsCode);
    //get classification details
    var classification = defaultJSONstat[0].Dimension(clsCode);

    var data = [];

    $.each(classification.id, function (index, value) {
        data.push({
            "id": value,
            "text": classification.Category(value).label
        })
    });

    $("#build-update-modal-elimination").find("[name=variable]").empty().append($("<option>")).select2({
        dropdownParent: $('#build-update-modal-elimination'),
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: data
    });

    // Enable and Focus Search input
    $("#build-update-modal-elimination").find("[name=variable]").prop('disabled', false).focus();

    $("#build-update-modal-elimination").find("[name=variable]").val(clsEliminationCode).trigger("change").trigger({
        type: 'select2:select',
        params: {
            data: $("#build-update-modal-elimination").find("[name=variable]").select2('data')[0]
        }
    });

    $("#build-update-modal-elimination").modal("show");

}

app.build.update.dimension.modal.saveElimination = function () {
    app.build.update.data.Elimination[$("#build-update-modal-elimination").find("[name=cls-code]").val()] = $("#build-update-modal-elimination").find("[name=variable]").val();
    app.build.update.dimension.drawElimination();
    $("#build-update-modal-elimination").modal("hide");
}

app.build.update.dimension.deleteElimination = function (clsCode) {
    app.build.update.data.Elimination[clsCode] = null;
    app.build.update.dimension.drawElimination();
}
//#endregion elimination

//#region map
app.build.update.dimension.drawMapTable = function (isSource) {
    isSource = isSource || false;
    var defaultJSONstat = $.grep(app.build.update.ajax.jsonStat, function (n, i) { // just use arr
        return n.extension.language.code == app.config.language.iso.code;
    });
    var mapDetails = [];

    $.each(app.build.update.data.Map, function (key, value) {
        var classification = defaultJSONstat[0].Dimension(key);
        mapDetails.push({
            "clsCode": key,
            "clsValue": classification.label,
            "gmpUrl": value,
            "gmpName": null
        });
    });
    var params = {
        "isSource": isSource,
        "mapDetails": mapDetails
    };
    app.build.update.dimension.ajax.readMapCollection(params);
};

app.build.update.dimension.ajax.readMapCollection = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.ReadCollection",
        {},
        "app.build.update.dimension.callback.readMapCollection",
        params,
        null,
        null,
        { async: false });
};

app.build.update.dimension.callback.readMapCollection = function (data, params) {
    $.each(params.mapDetails, function (index, value) {
        if (value.gmpUrl) {
            var mapUrlSplit = value.gmpUrl.split("/");
            var gmpCode = mapUrlSplit[mapUrlSplit.length - 1];
            var map = $.grep(data, function (n, i) {
                return n.GmpCode == gmpCode;
            });
            if (map.length) {
                value.gmpName = map[0].GmpName;
            }
        }
    });

    //if imported, validate variables
    if (params.isSource) {
        $.each(params.mapDetails, function (index, value) {
            if (value.gmpName) {
                //if from source and has a confirmed name, we need to validate the variables against the classification
                $.ajax({
                    url: value.gmpUrl,
                    dataType: 'json',
                    async: false,
                    success: function (data) {
                        var featureCodes = [];
                        $.each(data.features, function (index, value) {
                            featureCodes.push(value.properties[C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER])
                        });

                        //get classification variable codes
                        var defaultJSONstat = $.grep(app.build.update.ajax.jsonStat, function (n, i) { // just use arr
                            return n.extension.language.code == app.config.language.iso.code;
                        });
                        var classification = defaultJSONstat[0].Dimension(value.clsCode);

                        var invalidClassificationCodes = [];
                        $.each(classification.id, function (index, value) {
                            if (value != app.build.update.data.Elimination[classification[0].ClsCode]) {//don't check elimination variable
                                if ($.inArray(value, featureCodes) == -1) {
                                    invalidClassificationCodes.push(value);
                                }
                            }
                        });
                        if (invalidClassificationCodes.length) {
                            value.gmpName = null;
                        }
                    },
                    error: function (xhr) {
                        value.gmpName = null;
                    }
                });
            }

        });
    };
    app.build.update.dimension.callback.drawMapTable(params.mapDetails);
};

app.build.update.dimension.callback.drawMapTable = function (data) {
    app.build.update.dimension.mapsValid = true;
    var table = $("#build-update-dimensions").find("[name=classification-map]");
    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            ordering: false,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ "cls-code": row.clsCode, "cls-value": row.clsValue }, row.clsCode);
                    }
                },
                {
                    data: "clsValue"
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        if (row.gmpName) {
                            var mapUrlSplit = row.gmpUrl.split("/");
                            var gmpCode = mapUrlSplit[mapUrlSplit.length - 1];
                            return app.library.html.link.view({ "gmp-code": gmpCode }, row.gmpName, app.label.static["view-map"]);
                        }
                        else if (row.gmpUrl) {
                            app.build.update.dimension.mapsValid = false;
                            return $("<i>", {
                                "data-toggle": "tooltip",
                                "data-original-title": app.library.html.parseDynamicLabel("invalid-geojson-in-px-file-tooltip", [row.gmpUrl]),
                                "class": "fas fa-exclamation-triangle text-danger"
                            }).get(0).outerHTML + " " + app.label.static["invalid-geojson-in-px-file"];
                        }
                        else {
                            return ""
                        }
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ "idn": row.clsCode, "gmp-name": row.gmpName }, row.gmpUrl ? false : true);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.build.update.dimension.drawCallbackDrawMapTable();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.dimension.drawCallbackDrawMapTable();
        });
    };

    if (!app.build.update.dimension.mapsValid) {
        $("#build-update-elimination-map-heading-two").find("[name=map-accordion-button]").trigger("click");
        $("#build-update-elimination-map-heading-two").find("[name=warning-icon]").show();
    }
    else {
        $("#build-update-elimination-map-heading-two").find("[name=warning-icon]").hide();
    }
};



app.build.update.dimension.drawCallbackDrawMapTable = function () {
    $("#build-update-dimensions").find("[name=classification-map]").find("[name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
        e.preventDefault();
        app.geomap.preview.ajax.readMap($(this).attr("gmp-code"))
    });

    $("#build-update-dimensions").find("[name=classification-map]").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        var params = {
            "clsCode": $(this).attr("cls-code"),
            "clsValue": $(this).attr("cls-value"),
            "callback": "app.build.update.dimension.selectMap"
        };
        app.build.map.ajax.readMaps(params);
    });

    $("#build-update-dimensions").find("[name=classification-map]").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {

        if ($(this).attr("gmp-name")) {
            api.modal.confirm(
                app.library.html.parseDynamicLabel("confirm-delete-map", [$(this).attr("gmp-name")]),
                app.build.update.dimension.deleteMap,
                $(this).attr("idn")
            );
        }
        else {
            app.build.update.dimension.deleteMap($(this).attr("idn"));
        }
    });
};

app.build.update.dimension.selectMap = function (gmpCode, clsCode) {
    $.ajax({
        url: app.config.url.api.static + "/PxStat.Data.GeoMap_API.Read/" + gmpCode,
        dataType: 'json',
        success: function (data) {
            app.build.update.dimension.validateMap(data, gmpCode, clsCode);
        },
        error: function (xhr) {
            api.modal.exception(app.label.static["api-ajax-exception"]);
        }
    });
};

app.build.update.dimension.validateMap = function (data, gmpCode, clsCode) {
    //get feature codes from map
    var featureCodes = [];
    $.each(data.features, function (index, value) {
        featureCodes.push(value.properties[C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER])
    });

    //get classification variable codes
    var defaultJSONstat = $.grep(app.build.update.ajax.jsonStat, function (n, i) { // just use arr
        return n.extension.language.code == app.config.language.iso.code;
    });
    var classification = defaultJSONstat[0].Dimension(clsCode);

    var invalidClassificationCodes = [];
    $.each(classification.id, function (index, value) {
        if (value != app.build.update.data.Elimination[clsCode]) {//don't check elimination variable
            if ($.inArray(value, featureCodes) == -1) {
                invalidClassificationCodes.push(value);
            }
        }
    });

    if (!invalidClassificationCodes.length) {
        var params = {
            "clsCode": clsCode,
            "gmpCode": gmpCode
        };
        app.build.update.dimension.addMap(params);
    }
    else {
        var errorsList = $("<ul>", {
            "class": "list-group"
        });
        $.each(invalidClassificationCodes, function (index, value) {
            errorsList.append(
                $("<li>", {
                    "class": "list-group-item",
                    "html": "<b>" + value + "</b>: " + classification.Category(value).label
                })
            )
        });


        api.modal.confirm(
            app.label.static["invalid-geojson-variables-confirm"] + errorsList.get(0).outerHTML,
            app.build.update.dimension.addMap,
            {
                "gmpCode": gmpCode,
                "clsCode": clsCode
            }
        );
    }
};

app.build.update.dimension.addMap = function (params) {
    app.build.update.data.Map[params.clsCode] = app.config.url.api.static + "/PxStat.Data.GeoMap_API.Read/" + params.gmpCode;
    app.build.update.dimension.drawMapTable();
    $("#build-map-modal").modal("hide");
};

app.build.update.dimension.deleteMap = function (clsCode) {
    app.build.update.data.Map[clsCode] = null;
    app.build.update.dimension.drawMapTable();
};
//#endregion map