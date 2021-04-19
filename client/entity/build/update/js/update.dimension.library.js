/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build = app.build || {};
app.build.update = app.build.update || {};

app.build.update.dimension = {};
app.build.update.dimension.modal = {};
app.build.update.dimension.validation = {};
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
                        if (row.ClsGeoUrl) {
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