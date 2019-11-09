/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build = app.build || {};
app.build.update = {};

app.build.update.ajax = {};
app.build.update.ajax.response = [];
app.build.update.callback = {};

app.build.update.matrixLookup = {};
app.build.update.matrixLookup.ajax = {};
app.build.update.matrixLookup.callback = {};

app.build.update.validate = {};
app.build.update.validate.isPeriodValid = false;
app.build.update.validate.isMatrixPropertyValid = false;
app.build.update.validate.isDimensionPropertyValid = false;

app.build.update.data = {
    "MtrInput": null,
    "FrqCodeTimeval": null,
    "FrqValueTimeval": null,
    "Signature": null,
    "MtrCode": null,
    "FrmType": null,
    "FrmVersion": null,
    "MtrOfficialFlag": null,
    "CprCode": null,
    "Dimension": [],
    "Data": []
};
//#endregion

/**
 *  Get Frequency Select data from API to populate role type drop down.
 */
app.build.update.ajax.readFrequency = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.System.Settings.Frequency_API.Read",
        null,
        "app.build.update.callback.readFrequency");
};

/**
 * Fill dropdown for frequency Select
 * @param {*} response 
 */
app.build.update.callback.readFrequency = function (response) {
    $("#build-update-properties [name=frequency-code]").empty();
    if (response.error) {
        api.modal.error(response.error.message);
    }
    else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        api.modal.information(app.label.static["api-ajax-nodata"]);
        // Do nothing 
    }
    else if (response.data !== undefined) {
        // Set in Properties
        $("#build-update-properties [name=frequency-code]").append($("<option>", {
            "text": app.label.static["select-uppercase"],
            "value": "SELECT",
            "disabled": "disabled",
            "selected": "selected"
        }));

        $.each(response.data, function (key, value) {
            $("#build-update-properties [name=frequency-code]").append($("<option>", {
                "value": value.FrqCode,
                "text": value.FrqCode + " - " + app.label.static[value.FrqValue]
            }));
        });

        // Set in Modal
        $("#build-update-modal-frequency").find("[name=frq-code]").append($("<option>", {
            "text": app.label.static["select-uppercase"],
            "disabled": "disabled",
            "selected": "selected"
        }));

        $.each(response.data, function (key, value) {
            $("#build-update-modal-frequency").find("[name=frq-code]").append($("<option>", {
                "value": value.FrqCode,
                "text": value.FrqCode + " - " + app.label.static[value.FrqValue]
            }));
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.build.update.ajax.readCopyright = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.System.Settings.Copyright_API.Read",
        { CprCode: null },
        "app.build.update.callback.readCopyright",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * 
 */
app.build.update.callback.readCopyright = function (response) {
    $("#build-update-properties [name=copyright-code]").empty();
    if (response.error) {
        api.modal.error(response.error.message);
    }
    else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        api.modal.information(app.label.static["api-ajax-nodata"]);
        // Do nothing 
    }
    else if (response.data !== undefined) {
        data = response.data;
        // Map API data to select dropdown  model for main Subject search and update Subject search
        $("#build-update-properties [name=copyright-code]").append($("<option>", {
            "text": app.label.static["select-uppercase"],
            "value": "SELECT",
            "disabled": "disabled",
            "selected": "selected"
        }));

        $.each(data, function (key, value) {
            $("#build-update-properties [name=copyright-code]").append($("<option>", {
                "value": value.CprCode,
                "text": value.CprValue,
            }));
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.build.update.matrixLookup.ajax.read = function () {
    // Change app.config.language.iso.code to the selected one
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Matrix_API.ReadCodeList",
        {},
        "app.build.update.matrixLookup.ajax.read");
};
/**
 * 
 */
app.build.update.matrixLookup.callback.read = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        // Handle the Data in the Response then
        app.build.update.matrixLookup.drawDatatable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

};

/**
 * Draw Callback for Datatable
 */
app.build.update.matrixLookup.drawCallback = function () {
    // Responsive
    $("#build-update-matrix-lookup table").DataTable().columns.adjust().responsive.recalc();
}

/**
 * 
 */
app.build.update.matrixLookup.drawDatatable = function (data) {
    var searchInput = $("#build-update-properties [name=mtr-value]").val();
    if ($.fn.dataTable.isDataTable("#build-update-matrix-lookup table")) {
        app.library.datatable.reDraw("#build-update-matrix-lookup table", data);
    } else {
        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (_data, _type, row) {
                        return row.MtrCode;
                    }
                },
            ],
            drawCallback: function (settings) {
                app.build.update.matrixLookup.drawCallback();
            }
        };
        //Initialize DataTable
        $("#build-update-matrix-lookup table").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.drawCallbackDrawMatrix();
        });
        window.onresize = function () {
            // Responsive
            $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
        };
    }
    // Responsive
    $("#build-update-matrix-lookup table").DataTable().columns.adjust().responsive.recalc();

    $('#build-update-matrix-lookup').find("input[type=search]").val(searchInput);
    $("#build-update-matrix-lookup table").DataTable().search(searchInput).draw();
    $("#build-update-matrix-lookup").modal("show");
};

/**
 *Call Ajax for read format
 */
app.build.update.ajax.readFormat = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.System.Settings.Format_API.Read",
        { LngIsoCode: null },
        "app.build.update.callback.readFormat"
    );
};

/**
 * Callback for read
 * @param {*} response
 */
app.build.update.callback.readFormat = function (response) {
    if (response.error) {
        // Handle the Error in the Response first
        api.modal.error(response.error.message);
    }

    else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        api.modal.information(app.label.static["api-ajax-nodata"]);
    }
    else if (response.data) {
        $.each(response.data, function (index, format) {
            var formatDropdown = $("#build-update-dimension-metadata-templates").find("[name=create-submit]").clone();
            formatDropdown.attr(
                {
                    "frm-type": format.FrmType,
                    "frm-version": format.FrmVersion
                }).text(format.FrmType + " - " + format.FrmVersion);
            $("#build-update-dimensions [name=format-list]").append(formatDropdown);
        });

        $("#build-update-dimensions").find("[name=create-submit]").once("click", function (e) {
            app.build.update.data.FrmType = $(this).attr("frm-type");
            app.build.update.data.FrmVersion = $(this).attr("frm-version");
            app.build.update.updateOutput();
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.build.update.callback.resetData = function () {
    app.build.update.upload.file.content.data.JSON = null;
    $("#build-update-matrix-data").find("[name=build-update-upload-data]").val("");
    $("#build-update-matrix-data").find("[name=file-name]").empty().hide();
    $("#build-update-matrix-data").find("[name=file-tip]").show();
    $("#build-update-matrix-data").find("[name=preview-data]").prop("disabled", true);
    $("#build-update-matrix-data").find("[name=update]").prop("disabled", false);
};

/**
 * 
 */
app.build.update.callback.previewData = function () {
    api.spinner.start();
    $("#build-update-modal-preview-data").modal("show");
    api.content.load("#build-update-modal-preview-data .modal-body", "entity/build/update/index.data.preview.html");
    api.spinner.stop();
};

/**
 * 
 */
app.build.update.ajax.downloadDataTemplate = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Build.Build_API.ReadTemplate",
        {
            "MtrInput": app.build.update.upload.file.content.source.Base64,
            "FrqCodeTimeval": app.build.update.upload.FrqCode,
            "FrqValueTimeval": app.build.update.upload.FrqValue,
            "Signature": app.build.update.upload.Signature
        },
        "app.build.update.callback.downloadDataTemplate",
        null,
        null,
        null,
        { "async": false });

};

/**
 * 
 */
app.build.update.callback.downloadDataTemplate = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        var blob = new Blob([response.data.template], { type: "text/plain" });
        var downloadUrl = URL.createObjectURL(blob);
        var a = document.createElement("a");
        a.href = downloadUrl;
        a.download = response.data.MtrCode + '.' + moment(Date.now()).format(app.config.mask.datetime.file) + "." + app.label.static["template"].toLowerCase() + ".csv";

        if (document.createEvent) {
            // https://developer.mozilla.org/en-US/docs/Web/API/Document/createEvent
            var event = document.createEvent('MouseEvents');
            event.initEvent('click', true, true);
            a.dispatchEvent(event);
        }
        else {
            a.click();
        }
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.build.update.updateClassification = function () {
    //Get updated classification details
    var clsCode = $("#build-update-edit-classification [name=cls-code]").text();
    var lngIsoCode = $("#build-update-edit-classification [name=update-classification]").attr("lng-iso-code");
    var clsGeoUrl = $("#build-update-edit-classification [name=cls-geo-url]").val();

    $.each(app.build.update.data.Dimension, function (index, dimension) {
        if (lngIsoCode == dimension.LngIsoCode) {
            $.each(dimension.Classification, function (index, classification) {
                if (classification.ClsCode == clsCode) {
                    if (clsGeoUrl) {
                        classification.ClsGeoUrl = clsGeoUrl;
                    }
                    else {
                        classification.ClsGeoUrl = null;
                    }
                }
            });
        }
    });
    //redraw classifications
    app.build.update.dimension.drawClassification(lngIsoCode);
    $("#build-update-edit-classification").modal("hide");

};

/**
 * 
 */
app.build.update.addManualPeriod = function () {
    $('#build-update-manual-periods').find("[name=manual-period-errors-card]").hide();
    $('#build-update-manual-periods').find("[name=manual-period-errors]").empty();
    app.build.update.validateManualPeriod();

    //valid inputs, continue
    if (app.build.update.validate.isPeriodValid) {
        var codes = [];
        var values = [];

        //add previous new periods to array

        $.each(app.build.update.data.Dimension, function (index, dimension) {
            if (dimension.LngIsoCode == $("#build-update-new-periods [name=manual-submit-periods]").attr("lng-iso-code")) {
                $.each($(dimension.Frequency.Period), function (index, period) {
                    codes.push(period.PrdCode);
                    values.push(period.PrdValue);
                });
            }
        });

        //add new periods to array
        $('#build-update-manual-periods table').find("tbody tr").each(function (index) {
            var row = $(this);
            codes.push(row.find("td[idn=code]").text().trim().toLowerCase());
            values.push(row.find("td[idn=value]").text().trim().toLowerCase());
        });

        //Add previous codes to codes array
        $(app.build.update.ajax.response).each(function (key, value) {
            var data = JSONstat(value);
            if ($("#build-update-new-periods").find("[name=manual-submit-periods]").attr("lng-iso-code") == data.extension.language.code) {
                for (i = 0; i < data.length; i++) {
                    if (data.Dimension(i).role == "time") {
                        $.each(data.Dimension(i).id, function (index, period) {
                            codes.push(period);
                            values.push(data.Dimension(i).Category(index).label);
                        });
                    }
                }
            }
        });

        //check for duplicate periods
        if (app.build.update.dimension.checkDuplicate(codes) || app.build.update.dimension.checkDuplicate(values)) {
            $('#build-update-manual-periods').find("[name=manual-period-errors-card]").show();
            $('#build-update-manual-periods').find("[name=manual-period-errors]").append($("<li>", {
                "class": "list-group-item",
                "html": app.label.static["update-a-duplicate-period"]
            }));
            app.build.update.validate.isPeriodValid = false;
        }
    }

    //no duplicates, continue
    if (app.build.update.validate.isPeriodValid) {
        $.each(app.build.update.data.Dimension, function (index, dimension) { //find the statistic you need based on the LngIsoCode and insert new statistics
            if (dimension.LngIsoCode == $("#build-update-new-periods [name=manual-submit-periods]").attr("lng-iso-code")) {
                $.each($('#build-update-manual-periods table').find("tbody tr"), function (index, value) {
                    dimension.Frequency.Period.push(
                        {
                            "PrdCode": $(this).find("td[idn=code]").text().trim(),
                            "PrdValue": $(this).find("td[idn=value]").text().trim()
                        }
                    );
                });
            }
        });
        app.build.update.dimension.drawNewPeriod($("#build-update-new-periods [name=manual-submit-periods]").attr("lng-iso-code"));
        $("#build-update-new-periods").modal("hide");
    }

};

/**
 * 
 */
app.build.update.validateManualPeriod = function () {
    // Reset flag
    app.build.update.validate.isPeriodValid = true;

    //check for empty cells
    $('#build-update-manual-periods table').find("tbody tr").each(function (index) {
        var row = $(this);
        $(this).find("td").each(function () {

            var column = $("<span>", {
                "text": $(this).attr("idn"),
                "style": "text-transform:capitalize"
            }).get(0).outerHTML;

            var value = $(this).text().trim();

            if (value.length == 0) {
                $('#build-update-manual-periods').find("[name=manual-period-errors-card]").show();
                $('#build-update-manual-periods').find("[name=manual-period-errors]").append($("<li>", {
                    "class": "list-group-item",
                    "html": app.library.html.parseDynamicLabel("update-mandatory", [row.find("th[idn=row-number]").text(), column])
                }));
                app.build.update.validate.isPeriodValid = false;
            }
            else {
                if (value.trim().length > 256 && value.trim().length > 0) {
                    $('#build-update-manual-periods').find("[name=manual-period-errors-card]").show();
                    $('#build-update-manual-periods').find("[name=manual-period-errors]").append($("<li>", {
                        "class": "list-group-item",
                        "html": app.library.html.parseDynamicLabel("update-between-characters", [row.find("th[idn=row-number]").text(), column])
                    }));
                    app.build.update.validate.isPeriodValid = false;
                }
            }
        });
    });
};

/**
 * 
 */
app.build.update.addUploadPeriod = function () {
    $('#build-update-upload-periods').find("[name=upload-period-errors-card]").hide();
    $('#build-update-upload-periods').find("[name=upload-period-errors]").empty();

    var lngIsoCode = $("#build-update-new-periods [name=upload-submit-periods]").attr("lng-iso-code");

    app.build.update.upload.file.content.period.data.JSON = Papa.parse(app.build.update.upload.file.content.period.UTF8, {
        header: true,
        skipEmptyLines: true
    });;

    if (app.build.update.upload.file.content.period.data.JSON.meta.fields.length != 2) {
        $("#build-update-upload-periods").find("[name=upload-period-errors-card]").show();
        $('#build-update-upload-periods').find("[name=upload-period-errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    }

    var variableCodes = [];
    var variableValues = [];
    $(app.build.update.upload.file.content.period.data.JSON.data).each(function (key, value) {
        variableCodes.push(value.CODE.trim().toLowerCase());
        variableValues.push(value.VALUE.trim().toLowerCase());
    });

    //add previous added codes and values to array from either new periods or original periods

    $.each(app.build.update.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each(dimension.Frequency.Period, function (index, variable) {
                variableCodes.push(variable.PrdCode);
                variableValues.push(variable.PrdValue);
            });
        }
    });
    //Add previous codes to codes array
    $(app.build.update.ajax.response).each(function (key, value) {
        var responseData = JSONstat(value);
        if ($("#build-update-new-periods").find("[name=manual-submit-periods]").attr("lng-iso-code") == responseData.extension.language.code) {
            for (i = 0; i < responseData.length; i++) {
                if (responseData.Dimension(i).role == "time") {
                    $.each(responseData.Dimension(i).id, function (index, period) {
                        variableCodes.push(period);
                        variableValues.push(responseData.Dimension(i).Category(index).label);
                    });
                }
            }
        }
    });


    //Check for duplicate variable codes
    if (!app.build.update.dimension.checkDuplicate(variableCodes) || !app.build.update.dimension.checkDuplicate(variableValues)) { //no duplicates variables
        var periodsValid = true;
        var periodVariables = [];
        // if (codePosition == -1 || valuePosition == -1) {
        if (app.build.update.upload.file.content.period.data.JSON.meta.fields > 2) {
            periodsValid = false;
            $("#build-update-upload-periods").find("[name=upload-period-errors-card]").show();
            $('#build-update-upload-periods').find("[name=upload-period-errors]").append($("<li>", {
                "class": "list-group-item",
                "html": app.label.static["invalid-csv-format"]
            }));
            return;
        }

        $(app.build.update.upload.file.content.period.data.JSON.data).each(function (key, value) {
            var variableCode = value.CODE.trim();
            var variableValue = value.VALUE.trim();


            if (variableCode.length < 256 && variableCode.length > 0 && variableValue.length < 256 && variableValue.length > 0) { //validate variable
                //populate codes array to check for duplicates
                periodVariables.push({
                    "PrdCode": variableCode,
                    "PrdValue": variableValue
                });
            }

            else {
                periodsValid = false;
                $("#build-update-upload-periods").find("[name=upload-period-errors-card]").show();
                $('#build-update-upload-periods').find("[name=upload-period-errors]").append($("<li>", {
                    "class": "list-group-item",
                    "html": app.library.html.parseDynamicLabel("update-invalid-variable", [key + 2])
                }));
                return;


            }
        });

        if (periodVariables.length == 0) {
            periodsValid = false;

            $("#build-update-upload-periods").find("[name=upload-period-errors-card]").show();
            $('#build-update-upload-periods').find("[name=upload-period-errors]").append($("<li>", {
                "class": "list-group-item",
                "html": app.label.static["update-add-one-time-point"]
            }));
        }
        if (periodsValid) { //everything valid, add periods and redraw table
            $.each(app.build.update.data.Dimension, function (index, dimension) {
                if (dimension.LngIsoCode == lngIsoCode) {
                    $.each(periodVariables, function (index, variable) {
                        dimension.Frequency.Period.push(variable);
                    });
                }

            });
            $("#build-update-upload-periods").find("[name=upload-period-errors]").empty();
            $("#build-update-upload-periods").find("[name=upload-period-errors-card]").hide();
            app.build.update.dimension.drawNewPeriod(lngIsoCode);
            $("#build-update-new-periods").modal("hide");
        }
    }
    else {
        $("#build-update-upload-periods").find("[name=upload-period-errors-card]").show();
        $('#build-update-upload-periods').find("[name=upload-period-errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["update-duplicate-periods"]
        }));
    }
};

/**
 * 
 */
app.build.update.updateOutput = function () {
    //trigger matrix properties submit to highlight any errors when file is uploaded
    $("#build-update-properties").find("[type=submit]").trigger("click");

    var validationErrors = [];
    if (!app.build.update.validate.isMatrixPropertyValid) {
        validationErrors.push(app.label.static["update-invalid-table-properties"]);
    }
    //for each language, run validation to check for errors with dimension properties
    $(app.build.update.ajax.response).each(function (key, response) {
        var data = JSONstat(response);
        var lngIsoCode = data.extension.language.code;
        var lngIsoName = data.extension.language.name;
        $("#build-update-dimension-nav-" + lngIsoCode).find("[type=submit]").trigger("click");
        if (!app.build.update.validate.isDimensionPropertyValid) {
            validationErrors.push(app.library.html.parseDynamicLabel("update-invalid-dimension-properties", [lngIsoName]));
        }
    });
    if (!validationErrors.length) {
        //validate for no duplicate dimension labels 
        var dimensionLabels = [];
        $(app.build.update.ajax.response).each(function (key, response) {
            var data = JSONstat(response);
            dimensionLabels.push($("#build-update-dimension-nav-collapse-properties-" + data.extension.language.code + " [name=frequency-value]").val());
            dimensionLabels.push($("#build-update-dimension-nav-collapse-properties-" + data.extension.language.code + " [name=statistic-label]").val());
            for (i = 0; i < data.length; i++) {
                if (data.Dimension(i).role == "classification") {
                    dimensionLabels.push(data.Dimension(i).label);
                }
            }
            if (app.build.update.dimension.checkDuplicate(dimensionLabels)) {
                validationErrors.push(app.library.html.parseDynamicLabel("update-duplicate-dimension-value", [data.extension.language.name]));
            }
        });
    }
    //check that new periods are valid for all languages
    if (!validationErrors.length) {
        var numNewPeriods = [];
        $(app.build.update.data.Dimension).each(function (index, dimension) {
            numNewPeriods.push(this.Frequency.Period.length);
        });

        if (!numNewPeriods.every((val, i, arr) => val === arr[0])) {
            validationErrors.push(app.label.static["update-number-periods-not-equal-languages"]);
        }
    }
    //check that period codes for new periods are the same for all languages
    if (!validationErrors.length) {
        var defaultLngNewPeriodCodes = [];
        //get default language codes
        $.each(app.build.update.data.Dimension, function (index, dimension) {
            if (dimension.LngIsoCode == app.config.language.iso.code) {
                $.each(dimension.Frequency.Period, function (index, period) {
                    defaultLngNewPeriodCodes.push(period.PrdCode);
                });
            }
        });
        //compare other language period codes to default period codes
        $.each(app.build.update.data.Dimension, function (index, dimension) {
            var lngIsoCode = dimension.LngIsoCode;
            var lngIsoName = null;
            //get language name for error description
            $.each(app.build.update.upload.file.content.source.languages, function (key, language) {
                if (lngIsoCode == language.lngIsoCode) {
                    lngIsoName = language.lngIsoName;
                }
            });
            var newPeriodCodes = [];

            if (lngIsoCode != app.config.language.iso.code) {
                $.each(dimension.Frequency.Period, function (index, period) {
                    newPeriodCodes.push(period.PrdCode);
                });
                // If not the same, throw error
                if (defaultLngNewPeriodCodes.toString() != newPeriodCodes.toString()) {
                    validationErrors.push(app.library.html.parseDynamicLabel("update-new-period-codes", [lngIsoName, app.config.language.iso.name]));
                }
            }
        });
    }
    if (!validationErrors.length) {
        //check if new periods added without data
        $.each(app.build.update.data.Dimension, function (index, dimension) {
            var lngIsoCode = dimension.LngIsoCode;
            if (lngIsoCode == app.config.language.iso.code) {
                //get length of default language new period size
                if (dimension.Frequency.Period.length > 0 && !app.build.update.upload.file.content.data.JSON.data) {
                    validationErrors.push(app.label.static["update-new-periods-add-without-data"]);
                }
            }
        });
    }
    if (!validationErrors.length) {
        //check data template is valid if we have data

        if (app.build.update.upload.file.content.data.JSON != null) {
            if (!app.build.update.upload.validate.dataFile()) {
                validationErrors.push(app.label.static["invalid-csv-data-format"]);
            };
        }

    }
    if (!validationErrors.length) {
        var data = null;
        if (app.build.update.upload.file.content.data.JSON != null) {
            data = app.build.update.upload.file.content.data.JSON.data;
        };
        //no errors, proceed to get updated px file from server
        $("#build-update-dimensions").find("[name=update-error-card]").hide();

        //populate json data object
        app.build.update.data = $.extend(app.build.update.data, {
            "MtrInput": app.build.update.upload.file.content.source.Base64,
            "MtrCode": $("#build-update-properties [name=mtr-value]").val(),
            "FrqCodeTimeval": $("#build-update-properties").find("[name=frequency-code]").val(),
            "FrqValueTimeval": $("#build-update-dimension-nav-collapse-properties-" + app.config.language.iso.code + " [name=frequency-value]").val(),
            "MtrOfficialFlag": $("#build-update-properties").find("[name=official-flag]").prop('checked'),
            "CprCode": $("#build-update-properties").find("[name=copyright-code]").val(),
            "Data": data
        });

        //build properties to dimensions 
        $(app.build.update.ajax.response).each(function (key, value) {
            var lngIsoCode = value.extension.language.code;
            var tinyMceId = $("#build-update-dimension-nav-collapse-properties-" + lngIsoCode).find("[name=note-value]").attr("id");
            var noteValue = tinymce.get(tinyMceId).getContent();
            $(app.build.update.data.Dimension).each(function (key, value) {
                if (this.LngIsoCode == lngIsoCode) {
                    this.MtrTitle = $("#build-update-dimension-nav-collapse-properties-" + lngIsoCode + " [name=title-value]").val();
                    this.Frequency.FrqValue = $("#build-update-dimension-nav-collapse-properties-" + lngIsoCode + " [name=frequency-value]").val();
                    this.MtrNote = noteValue;
                    this.StatisticLabel = $("#build-update-dimension-nav-collapse-properties-" + lngIsoCode + " [name=statistic-label]").val();
                }
            });
        });



        //Populate namespace variables
        app.build.update.upload.FrqCode = $("#build-update-properties").find("[name=frequency-code]").val();
        app.build.update.upload.FrqValue = $("#build-update-dimension-nav-collapse-properties-" + app.config.language.iso.code + " [name=frequency-value]").val();
        app.build.update.upload.validate.ajax.read("app.build.update.upload.validate.callback.updateOutput");

    }
    else {
        //show errors to user
        var errorOutput = $("<ul>", {
            class: "list-group"
        });
        $.each(validationErrors, function (_index, value) {
            var error = $("<li>", {
                class: "list-group-item",
                html: value
            });
            errorOutput.append(error);
        });
        api.modal.error(errorOutput);

        $("#build-update-dimensions").find("[name=update-error]").html(errorOutput.get(0).outerHTML);
        $("#build-update-dimensions").find("[name=update-error-card]").fadeIn();
    }
};

/**
 *Get updated px file from server
 *
 */
app.build.update.ajax.updateOutput = function () {
    //remove ClsValue and classification variables before sending to API 
    $(app.build.update.data.Dimension).each(function (index, dimension) {
        $(dimension.Classification).each(function (index, classification) {
            delete classification.ClsValue;
            delete classification.Variable;
        });

    });

    // Merge Signature into Data
    app.build.update.data.Signature = app.build.update.upload.Signature;

    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Build.Build_API.Update",
        app.build.update.data,
        "app.build.update.callback.updateOutput",
        app.build.update.data.FrmType,
        null,
        null,
        { async: false });

};

/**
 *Download updated bx file from response
 *
 * @param {*} response
 */
app.build.update.callback.updateOutput = function (response, format) {

    if (response.error) {
        api.modal.error(response.error.message);
    }

    else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        api.modal.information(app.label.static["api-ajax-nodata"]);
    }
    else if (response.data) {

        var mimeType = "";
        var fileExtension = "";
        var fileName = $("#build-update-properties [name=mtr-value]").val() + "." + moment(Date.now()).format(app.config.mask.datetime.file);

        switch (format) {
            case C_APP_TS_FORMAT_JSON_STAT:
                mimeType = "application/json";
                fileExtension = "." + C_APP_EXTENSION_JSON_STAT;
                break;
            case C_APP_TS_FORMAT_PX:
                mimeType = "text/plain";
                fileExtension = "." + C_APP_EXTENSION_PX;
                break;
            default:
                api.modal.exception(app.label.static["api-ajax-exception"]);
                return;
                break;
        };

        $.each(response.data, function (index, file) {
            var fileData = null;
            switch (format) {
                case C_APP_TS_FORMAT_JSON_STAT:
                    fileData = JSON.stringify(file);
                    //Append language iso code
                    fileName += "." + JSONstat(file).extension.language.code;
                    break;
                case C_APP_TS_FORMAT_PX:
                    fileData = file;
                    break;
            };

            var blob = new Blob([fileData], { type: mimeType });
            var downloadUrl = URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = downloadUrl;
            a.download = fileName + fileExtension;
            if (document.createEvent) {
                // https://developer.mozilla.org/en-US/docs/Web/API/Document/createEvent
                var event = document.createEvent('MouseEvents');
                event.initEvent('click', true, true);
                a.dispatchEvent(event);
            }
            else {
                a.click();
            }
        });

        api.modal.success(app.label.static["create-source-downloaded"]);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

};

/**
 *Validation for matrix properties
 *
 */
app.build.update.validate.matrixProperty = function () {
    $("#build-update-properties").find("form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "mtr-value": {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(C_SANITISE_UPPERCASE, C_APP_REGEX_ALPHANUMERIC_DIACRITIC);
                    $(this).val(value);
                    return value;
                }
            },
            'frequency-code': {
                required: true,
            },
            "copyright-code": {
                required: true
            },
        },
        errorPlacement: function (error, element) {
            $("#build-update-properties [name=" + element[0].name + "-error-holder").empty().append(error[0]);
        },
        invalidHandler: function (event, validator) {
            app.build.update.validate.isMatrixPropertyValid = false;
        },
        submitHandler: function () {
            app.build.update.validate.isMatrixPropertyValid = true;
        }
    }).resetForm();
};

/**
 * Bind validation for frequency validation
 */
app.build.update.validate.frequencyModal = function () {
    $("#build-update-modal-frequency form").trigger("reset").validate({
        rules: {
            "frq-value": {
                required: true
            },
            "frq-code": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#build-update-modal-frequency [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function () {
            $("#build-update-modal-frequency").modal("hide");
            // Store for later use
            app.build.update.upload.FrqValue = $("#build-update-modal-frequency").find("[name=frq-value]:checked").val();
            app.build.update.upload.FrqCode = $("#build-update-modal-frequency").find("[name=frq-code]").val();

            app.build.update.upload.validate.ajax.read("app.build.update.upload.validate.callback.uploadSource");
        }
    }).resetForm();
};

/**
 * Validate the properties entered for the Matrix
 * @param {*} LngIsoCode
 */
app.build.update.validate.dimensionProperty = function (LngIsoCode) {
    $("#build-update-dimension-nav-" + LngIsoCode).find("form").trigger("reset").validate({
        rules: {
            "title-value": {
                required: true
            },
            "frequency-value": {
                required: true,
                notEqual: $("#build-update-dimension-nav-" + LngIsoCode).find("[name=statistic-label]")
            },
            "statistic-label": {
                required: true
            },
        },
        invalidHandler: function (event, validator) {
            app.build.update.validate.isDimensionPropertyValid = false;
        },
        errorPlacement: function (error, element) {
            $("#build-update-dimension-nav-" + LngIsoCode).find('[name=' + element[0].name + '-error-holder]').append(error[0]);
        },
        submitHandler: function (form) {
            app.build.update.validate.isDimensionPropertyValid = true;
        }
    }).resetForm();
};

/**
 *Validation for edit classification
 *
 */
app.build.update.validate.classification = function () {
    $("#build-update-edit-classification").find("form").trigger("reset").validate({
        rules: {
            "cls-geo-url": {
                url: true
            }
        },
        errorPlacement: function (error, element) {
            $("#build-update-edit-classification").find('[name=' + element[0].name + '-error-holder]').append(error[0]);
        },
        submitHandler: function (form) {
            app.build.update.updateClassification();
        }
    }).resetForm();
};
