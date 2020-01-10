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
 *Call Ajax for read format
 */
app.build.update.ajax.readFormat = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.System.Settings.Format_API.Read",
        {
            "LngIsoCode": null,
            "FrmDirection": C_APP_TS_FORMAT_DIRECTION_UPLOAD
        },
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
                });

            formatDropdown.find("[name=type]").text(format.FrmType);
            formatDropdown.find("[name=version]").text(format.FrmVersion);

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
app.build.update.callback.cancelData = function () {
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
app.build.update.callback.resetData = function () {
    $("#build-update-matrix-data").find("[name=build-update-upload-data]").val("");
    app.build.update.callback.cancelData();
};

/**
 * Preview csv data as datatable
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
app.build.update.ajax.downloadAllData = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Build.Build_API.ReadDatasetByAllPeriods",
        params,
        "app.build.update.callback.downloadData",
        null,
        null,
        null,
        { "async": false });

};


app.build.update.ajax.downloadNewData = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Build.Build_API.ReadDatasetByNewPeriods",
        params,
        "app.build.update.callback.downloadData",
        null,
        null,
        null,
        { "async": false });

};


app.build.update.ajax.downloadExistingData = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Build.Build_API.ReadDatasetByExistingPeriods",
        params,
        "app.build.update.callback.downloadData",
        null,
        null,
        null,
        { "async": false });

};

app.build.update.callback.downloadData = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        var blob = new Blob([response.data.csv], { type: "text/plain" });
        var downloadUrl = URL.createObjectURL(blob);
        var a = document.createElement("a");
        a.href = downloadUrl;
        a.download = response.data.MtrCode + '.' + moment(Date.now()).format(app.config.mask.datetime.file) + "." + app.label.static["data"].toLowerCase() + ".csv";

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



app.build.update.cancelUpoadPeriod = function () {
    //clean up 
    app.build.update.upload.file.content.period.UTF8 = null;

    $("#build-update-upload-periods").find("[name=file-name]").empty().hide();
    $("#build-update-upload-periods").find("[name=file-tip]").show();
    $("#build-update-upload-periods").find("[name=upload-submit-periods]").prop("disabled", true);
};

app.build.update.resetUpoadPeriod = function () {
    $("#build-update-upload-periods-file").val("");
    app.build.update.cancelUpoadPeriod();
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
    $('#build-update-manual-periods').find("[name=errors-card]").hide();
    $('#build-update-manual-periods').find("[name=errors]").empty();

    var lngIsoCode = $("#build-update-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    if (!app.build.update.validateManualPeriod()) {
        return;
    }

    //get new Periods for validation
    var codes = [];
    var values = [];

    //Append codes and values from new periods
    $('#build-update-manual-periods table').find("tbody tr").each(function (index) {
        codes.push($(this).find("td[idn=code]").text());
        values.push($(this).find("td[idn=value]").text());
    });

    //pass by deep copy
    if (app.build.update.hasDuplicate($.extend(true, [], codes), $.extend(true, [], values), $('#build-update-manual-periods'))) {
        return;
    }
    //add new periods  
    $.each(app.build.update.data.Dimension, function (index, dimension) {
        //find the dimension you need based on the LngIsoCode and insert new statistics
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each(codes, function (index) {
                dimension.Frequency.Period.push(
                    {
                        "PrdCode": codes[index],
                        "PrdValue": values[index]
                    }
                );
            });
        }
    });

    //redraw periods datatable
    app.build.update.dimension.drawNewPeriod(lngIsoCode);
    $("#build-update-new-periods").modal("hide");

};

/**
 * 
 */
app.build.update.validateManualPeriod = function () {
    // Reset flag
    var isPeriodValid = true;

    //check for empty cells
    $('#build-update-manual-periods table').find("tbody tr").each(function (index) {
        var row = $(this);
        $(this).find("td").each(function () {

            var column = $("<span>", {
                "text": $(this).attr("idn"),
                "style": "text-transform:capitalize"
            }).get(0).outerHTML;

            //trim and sanitise
            var value = $(this).text().trim();
            $(this).text(value);

            if (value.length == 0) {
                $('#build-update-manual-periods').find("[name=errors-card]").show();
                $('#build-update-manual-periods').find("[name=errors]").append($("<li>", {
                    "class": "list-group-item",
                    "html": app.library.html.parseDynamicLabel("update-mandatory", [row.find("th[idn=row-number]").text(), column])
                }));
                isPeriodValid = false;
            }
            else if (value.length > 256 || C_APP_REGEX_NODOUBLEQUOTE.test(value)) {
                $('#build-update-manual-periods').find("[name=errors-card]").show();
                $('#build-update-manual-periods').find("[name=errors]").append($("<li>", {
                    "class": "list-group-item",
                    "html": app.library.html.parseDynamicLabel("update-characters", [row.find("th[idn=row-number]").text(), column])
                }));
                isPeriodValid = false;
            }
        });
    });
    return isPeriodValid;
};

app.build.update.hasDuplicate = function (codes, values, selector) {
    codes = codes || [];
    values = values || [];

    var lngIsoCode = $("#build-update-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    //Append codes and values from previous new periods
    $.each(app.build.update.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each($(dimension.Frequency.Period), function (index, period) {
                codes.push(period.PrdCode);
                values.push(period.PrdValue);
            });
        }
    });

    //Append codes and values existing source
    $(app.build.update.ajax.response).each(function (key, value) {
        var data = JSONstat(value);
        if (lngIsoCode == data.extension.language.code) {
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
    if (app.library.utility.arrayHasDuplicate(codes) || app.library.utility.arrayHasDuplicate(values)) {
        selector.find("[name=errors-card]").show();
        selector.find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["build-duplicate-period"]
        }));
        return true;
    }
    else return false;
}

/**
 * 
 */
app.build.update.addUploadPeriod = function () {
    $('#build-update-upload-periods').find("[name=errors-card]").hide();
    $('#build-update-upload-periods').find("[name=errors]").empty();

    var lngIsoCode = $("#build-update-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
    app.build.update.upload.file.content.period.data.JSON = Papa.parse(app.build.update.upload.file.content.period.UTF8, {
        header: true,
        skipEmptyLines: true
    });

    if (!app.build.update.validateUploadPeriod()) {
        return;
    }

    //get new Periods for validation
    var codes = [];
    var values = [];

    //Append codes and values from new periods
    $.each(app.build.update.upload.file.content.period.data.JSON.data, function (key, value) {
        codes.push(value[C_APP_CSV_CODE]);
        values.push(value[C_APP_CSV_VALUE]);
    });

    //pass by deep copy
    if (app.build.update.hasDuplicate($.extend(true, [], codes), $.extend(true, [], values), $('#build-update-upload-periods'))) {
        return;
    }

    //everything valid, add periods and redraw table
    $.each(app.build.update.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each(codes, function (index) {
                dimension.Frequency.Period.push(
                    {
                        "PrdCode": codes[index],
                        "PrdValue": values[index]
                    }
                );
            });
        }
    });


    app.build.update.dimension.drawNewPeriod(lngIsoCode);
    $("#build-update-new-periods").modal("hide");
};

/**
 * 
 */
app.build.update.validateUploadPeriod = function () {
    //check that csv contains data
    if (!app.build.update.upload.file.content.period.data.JSON.data.length) {
        $("#build-update-upload-periods").find("[name=errors-card]").show();
        $('#build-update-upload-periods').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["update-time-point"]
        }));
        return false;
    };

    var csvHeaders = app.build.update.upload.file.content.period.data.JSON.meta.fields;

    //check that csv headers contain C_APP_CSV_CODE and C_APP_CSV_VALUE, both case sensitive
    if (jQuery.inArray(C_APP_CSV_CODE, csvHeaders) == -1 || jQuery.inArray(C_APP_CSV_VALUE, csvHeaders) == -1) {
        $("#build-update-upload-periods").find("[name=errors-card]").show();
        $('#build-update-upload-periods').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return false;
    };

    var isLengthValid = true;
    //trim and sanitise
    $(app.build.update.upload.file.content.period.data.JSON.data).each(function (key, value) {
        value[C_APP_CSV_CODE] = value[C_APP_CSV_CODE].trim();
        value[C_APP_CSV_VALUE] = value[C_APP_CSV_VALUE].trim();

        //check that variable lengths are valid
        if (value[C_APP_CSV_CODE].length > 256 ||
            value[C_APP_CSV_CODE].length == 0 ||
            value[C_APP_CSV_VALUE].length > 256 ||
            value[C_APP_CSV_VALUE].length == 0 ||
            C_APP_REGEX_NODOUBLEQUOTE.test(value[C_APP_CSV_CODE]) ||
            C_APP_REGEX_NODOUBLEQUOTE.test(value[C_APP_CSV_VALUE])) {
            $("#build-update-upload-periods").find("[name=errors-card]").show();
            $('#build-update-upload-periods').find("[name=errors]").append($("<li>", {
                "class": "list-group-item",
                "html": app.library.html.parseDynamicLabel("update-invalid-variable", [key + 2])
            }));
            isLengthValid = false;

        }
    });

    if (!isLengthValid) {
        return false;
    }


    return true;
};
/**
 * 
 */
app.build.update.updateOutput = function () {
    //trigger matrix properties submit to highlight any errors when file is uploaded
    $("#build-update-properties").find("[type=submit]").trigger("click");

    var validationErrors = [];
    if (!app.build.update.validate.isMatrixPropertyValid) {
        validationErrors.push(app.label.static["update-properties"]);
    }
    //for each language, run validation to check for errors with dimension properties
    $(app.build.update.ajax.response).each(function (key, response) {
        var data = JSONstat(response);
        var lngIsoCode = data.extension.language.code;
        var lngIsoName = data.extension.language.name;
        $("#build-update-dimension-nav-" + lngIsoCode).find("[type=submit]").trigger("click");
        if (!app.build.update.validate.isDimensionPropertyValid) {
            validationErrors.push(app.library.html.parseDynamicLabel("update-dimension-properties", [lngIsoName]));
        }
    });
    if (!validationErrors.length) {
        //validate for no duplicate dimension labels 

        $(app.build.update.ajax.response).each(function (key, response) {
            var dimensionLabels = [];
            var data = JSONstat(response);
            dimensionLabels.push($("#build-update-dimension-nav-collapse-properties-" + data.extension.language.code + " [name=frequency-value]").val());
            dimensionLabels.push($("#build-update-dimension-nav-collapse-properties-" + data.extension.language.code + " [name=statistic-label]").val());
            for (i = 0; i < data.length; i++) {
                if (data.Dimension(i).role == "classification") {
                    dimensionLabels.push(data.Dimension(i).label);
                }
            }
            if (app.library.utility.arrayHasDuplicate(dimensionLabels)) {
                validationErrors.push(app.library.html.parseDynamicLabel("build-dimension", [data.extension.language.name]));
            }
        });
    }

    //check that everything is OK with new periods
    if (!app.build.update.isPeriodsDimensionsValid()) {
        validationErrors.push(app.label.static["build-update-period-error"]);
    }

    if (!validationErrors.length) {
        //check if new periods added without data
        $.each(app.build.update.data.Dimension, function (index, dimension) {
            var lngIsoCode = dimension.LngIsoCode;
            if (lngIsoCode == app.config.language.iso.code) {
                //get length of default language new period size
                if (dimension.Frequency.Period.length > 0 && !app.build.update.upload.file.content.data.JSON) {
                    validationErrors.push(app.label.static["update-periods-without-data"]);
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
        app.build.update.data = $.extend(true, app.build.update.data, {
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
        app.build.update.upload.validate.ajax.read("app.build.update.upload.validate.callback.updateOutput", app.config.upload.unitsPerSecond.update);

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

app.build.update.isPeriodsDimensionsValid = function () {
    var isValid = true;
    //check that the number of new periods are the same for all languages
    var numNewPeriods = [];
    $(app.build.update.data.Dimension).each(function (index, dimension) {
        numNewPeriods.push(this.Frequency.Period.length);
    });

    if (!numNewPeriods.every((val, i, arr) => val === arr[0])) {
        isValid = false;
    }

    //check that period codes for new periods are the same for all languages
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
                isValid = false;
            }
        }
    });

    return isValid;
}
/**
 *Get updated px file from server
 *
 */
app.build.update.ajax.updateOutput = function () {
    //clone data object to tidy up before sending to server
    var params = $.extend(true, {}, app.build.update.data);

    //remove ClsValue and classification variables before sending to API 
    $(params.Dimension).each(function (index, dimension) {
        $(dimension.Classification).each(function (index, classification) {
            delete classification.ClsValue;
            delete classification.Variable;
        });
    });

    //remove data properties not need by api, value columns may be included in data csv template
    var dimensionCodes = [];
    $(app.build.update.ajax.response).each(function (key, value) {
        var data = JSONstat(value);
        if (data.extension.language.code == app.config.language.iso.code) {
            //get dimension codes from default JsonStat
            for (i = 0; i < data.length; i++) {
                if (data.Dimension(i).role != "time") {
                    //don't add the time code to dimension code array 
                    dimensionCodes.push(data.id[i]);
                }
            };
        };
    });

    //add frequency code from select as this may have changed
    dimensionCodes.push($("#build-update-properties [name=frequency-code]").val());

    //Loop through data object and remove any properties that are not dimensions or the value property
    $(params.Data).each(function (keyObject, dataObject) {
        $.each(dataObject, function (key, property) {
            if (jQuery.inArray(key, dimensionCodes) == -1 && key != C_APP_CSV_VALUE) {
                delete dataObject[key]
            };

        });
    });

    // Merge Signature into Data
    params.Signature = app.build.update.upload.Signature;
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Build.Build_API.Update",
        params,
        "app.build.update.callback.updateOutput",
        params.FrmType,
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
            case C_APP_TS_FORMAT_TYPE_JSONSTAT:
                mimeType = "application/json";
                fileExtension = C_APP_EXTENSION_JSONSTAT;
                break;
            case C_APP_TS_FORMAT_TYPE_PX:
                mimeType = "text/plain";
                fileExtension = C_APP_EXTENSION_PX;
                break;
            default:
                api.modal.exception(app.label.static["api-ajax-exception"]);
                return;
                break;
        };

        $.each(response.data, function (index, file) {
            var fileData = null;
            switch (format) {
                case C_APP_TS_FORMAT_TYPE_JSONSTAT:
                    fileData = JSON.stringify(file);
                    //Append language iso code
                    fileName += "." + JSONstat(file).extension.language.code;
                    break;
                case C_APP_TS_FORMAT_TYPE_PX:
                    fileData = file;
                    break;
            };

            var blob = new Blob([fileData], { type: mimeType });
            var downloadUrl = URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = downloadUrl;
            a.download = fileName + '.' + fileExtension;
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

        api.modal.success(app.label.static["update-success"]);
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
        submitHandler: function (form) {
            $(form).sanitiseForm();
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
        submitHandler: function (form) {
            $(form).sanitiseForm();
            $("#build-update-modal-frequency").modal("hide");
            // Store for later use
            app.build.update.upload.FrqValue = $("#build-update-modal-frequency").find("[name=frq-value]:checked").val();
            app.build.update.upload.FrqCode = $("#build-update-modal-frequency").find("[name=frq-code]").val();

            app.build.update.upload.validate.ajax.read("app.build.update.upload.validate.callback.uploadSource", app.config.upload.unitsPerSecond.read);
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
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            },
            "frequency-value": {
                required: true,
                notEqual: $("#build-update-dimension-nav-" + LngIsoCode).find("[name=statistic-label]"),
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            },
            "statistic-label": {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            },
        },
        invalidHandler: function (event, validator) {
            app.build.update.validate.isDimensionPropertyValid = false;
        },
        errorPlacement: function (error, element) {
            $("#build-update-dimension-nav-" + LngIsoCode).find('[name=' + element[0].name + '-error-holder]').append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
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
                url: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#build-update-edit-classification").find('[name=' + element[0].name + '-error-holder]').append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.build.update.updateClassification();
        }
    }).resetForm();
};
