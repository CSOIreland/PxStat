/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build = app.build || {};
app.build.update = app.build.update || {};

app.build.update.dimension = {};
app.build.update.dimension.modal = {};
app.build.update.dimension.callback = {};

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

    $(table).find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        //initiate edit classification validation
        app.build.update.validate.classification();
        e.preventDefault();
        //find data
        var clsCode = $(this).attr("cls-code");
        $.each(classificationsData, function (index, value) {
            if (value.ClsCode == clsCode) {
                $("#build-update-edit-classification [name=update-classification]").attr("lng-iso-code", lngIsoCode);
                app.build.update.dimension.modal.editClassification(value);
                return;
            }
        });
    });

    $(table).find("[name=" + C_APP_NAME_LINK_GEOJSON + "]").once("click", function (e) {
        e.preventDefault();
        app.map.draw($(this).attr("geojson-url"), $(this).attr("cls-code"), $(this).attr("cls-value"));
    });
}

/**
 *Draw classification dimension
 * @param {*} lngIsoCode
 */
app.build.update.dimension.drawClassification = function (lngIsoCode) {
    var classificationsData = [];
    //get classifications from JSON object as this may be have been updated from JSON-stat by user
    $(app.build.update.data.Dimension).each(function (key, value) {
        if (this.LngIsoCode == lngIsoCode) {
            classificationsData = this.Classification;
        }
    });

    //Create accordion for classification
    var table = $("#build-update-dimension-nav-" + lngIsoCode).find("[name=classifications-added-table]");

    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, classificationsData);
    } else {

        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: classificationsData,
            ordering: false,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ "cls-code": row.ClsCode }, row.ClsCode);
                    }
                },
                {
                    data: "ClsValue"
                },

                {
                    data: null,
                    render: function (data, type, row) {
                        if (row.ClsGeoUrl && app.config.plugin.highcharts.enabled) {
                            return app.library.html.link.geoJson({ "geojson-url": row.ClsGeoUrl, "cls-value": row.ClsValue, "cls-code": row.ClsCode }, row.ClsGeoUrl);
                        }
                        else if (row.ClsGeoUrl) {
                            return app.library.html.link.external({}, row.ClsGeoUrl);
                        }
                        else {
                            return null
                        }


                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.Variable.length;
                    }
                }
            ],
            drawCallback: function (settings) {
                app.build.update.dimension.callback.drawClassification(table, lngIsoCode, classificationsData);
            },
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.dimension.callback.drawClassification(table, lngIsoCode, classificationsData);
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

    var table = $("#build-update-dimension-nav-" + lngIsoCode).find("[name=periods-existing]");

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
            data = dimension.Frequency.Period;
        }
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
    api.modal.confirm(app.library.html.parseDynamicLabel("build-delete-time-period", [params.prdCode]),
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
            $.each(dimension.Frequency.Period, function (index, variable) {
                if (variable.PrdCode == params.prdCode) {
                    dimension.Frequency.Period.splice(index, 1);
                }
            });
        }
    });

    app.build.update.dimension.drawNewPeriod(params.lngIsoCode);
};

/**
 * 
 */
app.build.update.dimension.modal.editClassification = function (classification) {
    //set current classification values
    $("#build-update-edit-classification [name=cls-code]").text(classification.ClsCode);
    $("#build-update-edit-classification [name=cls-value]").text(classification.ClsValue);
    $("#build-update-edit-classification [name=cls-geo-url]").val(classification.ClsGeoUrl);

    if (app.config.entity.build.geoJsonLookup.enabled) {
        $("#build-update-edit-classification").find("[name=geojson-lookup]").removeClass("d-none").attr("href", app.config.entity.build.geoJsonLookup.href)
    }

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