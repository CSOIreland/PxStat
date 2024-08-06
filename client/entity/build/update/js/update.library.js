/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build = app.build || {};
app.build.update = {};

app.build.update.ajax = {};
app.build.update.ajax.data = [];
app.build.update.ajax.jsonStat = [];
app.build.update.callback = {};

app.build.update.goTo = {};

app.build.update.matrixLookup = {};
app.build.update.matrixLookup.ajax = {};
app.build.update.matrixLookup.callback = {};

app.build.update.validate = {};
app.build.update.validate.isMatrixPropertyValid = false;
app.build.update.validate.isDimensionPropertyValid = false;
app.build.update.rlsCode = null;
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
    "Data": [],
    "Elimination": {},
    "Map": {}
};
app.build.update.import = false;
//#endregion

/**
 *  Get Frequency Select data from API to populate role type drop down.
 */
app.build.update.ajax.readFrequency = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Frequency_API.Read",
        null,
        "app.build.update.callback.readFrequency");
};

/**
 * Fill dropdown for frequency Select
 * @param {*} data 
 */
app.build.update.callback.readFrequency = function (data) {
    // Set in Properties
    $("#build-update-properties").find("[name=frequency-code]").empty().append($("<option>", {
        "text": app.label.static["select-uppercase"],
        "value": "SELECT",
        "disabled": "disabled",
        "selected": "selected"
    }));

    // Set in Modal
    $("#build-update-modal-frequency").find("[name=frq-code]").empty().append($("<option>", {
        "text": app.label.static["select-uppercase"],
        "disabled": "disabled",
        "selected": "selected"
    }));

    $.each(data, function (key, value) {
        // Set in Properties
        $("#build-update-properties").find("[name=frequency-code]").append($("<option>", {
            "value": value.FrqCode,
            "text": value.FrqCode + " - " + app.label.static[value.FrqValue]
        }));

        // Set in Modal
        $("#build-update-modal-frequency").find("[name=frq-code]").append($("<option>", {
            "value": value.FrqCode,
            "text": value.FrqCode + " - " + app.label.static[value.FrqValue]
        }));
    });
};

/**
 * 
 */
app.build.update.ajax.readCopyright = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
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
app.build.update.callback.readCopyright = function (data) {

    // Load select2
    $("#build-update-properties").find("[name=copyright-code]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.build.update.mapData(data)
    }).on('select2:select', function (e) {
        $("#build-update-properties").find("[type=submit]").trigger("click")
    });
};

app.build.update.mapData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        // Create ID and NAME to the list
        dataAPI[i].id = item.CprCode;
        dataAPI[i].text = item.CprValue;
    });
    return dataAPI;
};

/**
 *Call Ajax for read format
 */
app.build.update.ajax.readFormat = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
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
 * @param {*} data
 */
app.build.update.callback.readFormat = function (data) {
    $.each(data, function (index, format) {
        var formatDropdown = $("#build-update-dimension-metadata-templates").find("[name=update-submit]").clone();
        formatDropdown.attr(
            {
                "frm-type": format.FrmType,
                "frm-version": format.FrmVersion
            });

        formatDropdown.find("[name=type]").text(format.FrmType);
        formatDropdown.find("[name=version]").text(format.FrmVersion);

        $("#build-update-dimensions [name=format-list]").append(formatDropdown);
    });

    $("#build-update-dimensions [name=update-submit]").once("click", function (e) {
        app.build.update.import = false;
        e.preventDefault();
        app.build.update.data.FrmType = $(this).attr("frm-type");
        app.build.update.data.FrmVersion = $(this).attr("frm-version");
        app.build.update.checkForData();
    });

    $("#build-update-dimensions [name=import]").once("click", function (e) {
        app.build.update.import = true;
        e.preventDefault();
        app.build.update.data.FrmType = $(this).attr("frm-type");
        app.build.update.data.FrmVersion = $(this).attr("frm-version");
        app.build.update.checkForData();
    });
};

/**
 * 
 */
app.build.update.callback.cancelData = function () {
    app.build.update.upload.file.content.data.JSON = null;
    app.build.update.data.Data = [];
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
        app.config.url.api.jsonrpc.private,
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
        {
            async: false,
            timeout: app.config.transfer.timeout
        });

};

/**
 * 
 */
app.build.update.callback.downloadDataTemplate = function (data) {
    var fileName = data.MtrCode + '.' + moment(Date.now()).format(app.config.mask.datetime.file) + "." + app.label.static["template"].toLowerCase();
    // Download the file
    app.library.utility.download(fileName, data.template, C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
};


app.build.update.ajax.downloadData = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Build.Build_API.ReadDataset",
        params,
        "app.build.update.callback.downloadData",
        null,
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        });

};

app.build.update.callback.downloadData = function (data) {
    var fileName = data.MtrCode + '.' + moment(Date.now()).format(app.config.mask.datetime.file) + "." + app.label.static["data"].toLowerCase();
    // Download the file
    app.library.utility.download(fileName, data.csv, C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
    $('#build-update-download-csv-file').modal("hide");
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
    $("#build-update-upload-periods").find("[name=errors-card]").hide();
    $("#build-update-upload-periods").find("[name=errors]").empty();
    app.build.update.cancelUpoadPeriod();
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
        codes.push($(this).find("td[idn=code]").text().replace(/ /g, ''));
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

            //sort periods
            dimension.Frequency.Period.sort(function (a, b) {
                return a.PrdCode - b.PrdCode
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
    $(app.build.update.ajax.jsonStat).each(function (key, jsonStat) {
        if (lngIsoCode == jsonStat.extension.language.code) {
            for (i = 0; i < jsonStat.length; i++) {
                if (jsonStat.Dimension(i).role == "time") {
                    $.each(jsonStat.Dimension(i).id, function (index, period) {
                        codes.push(period);
                        values.push(jsonStat.Dimension(i).Category(index).label);
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
        codes.push(value[C_APP_CSV_CODE].replace(/ /g, ''));
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
                        "PrdCode": codes[index].trim(),
                        "PrdValue": values[index].trim()
                    }
                );
            });
            //sort periods
            dimension.Frequency.Period.sort(function (a, b) {
                return a.PrdCode - b.PrdCode
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

app.build.update.checkForData = function () {
    //check if new periods added without data
    $.each(app.build.update.data.Dimension, function (index, dimension) {
        var lngIsoCode = dimension.LngIsoCode;
        if (lngIsoCode == app.config.language.iso.code) {
            //get length of default language new period size
            if (dimension.Frequency.Period.length > 0 && !app.build.update.upload.file.content.data.JSON) {
                api.modal.confirm(
                    app.label.static["update-periods-without-data"],
                    app.build.update.updateOutput
                );
            }
            else {
                app.build.update.updateOutput();
            }
        }
    });
}

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
    $(app.build.update.ajax.jsonStat).each(function (key, jsonStat) {
        var lngIsoCode = jsonStat.extension.language.code;
        var lngIsoName = jsonStat.extension.language.name;
        $("#build-update-dimension-nav-" + lngIsoCode).find("[type=submit]").trigger("click");
        if (!app.build.update.validate.isDimensionPropertyValid) {
            validationErrors.push(app.library.html.parseDynamicLabel("update-dimension-properties", [lngIsoName]));
        };
    });
    if (!validationErrors.length) {
        //validate for no duplicate dimension labels 
        $(app.build.update.ajax.jsonStat).each(function (key, jsonStat) {
            var dimensionLabels = [];
            dimensionLabels.push($("#build-update-dimension-nav-collapse-properties-" + jsonStat.extension.language.code + " [name=frequency-value]").val());
            dimensionLabels.push($("#build-update-dimension-nav-collapse-properties-" + jsonStat.extension.language.code + " [name=statistic-label]").val());
            for (i = 0; i < jsonStat.length; i++) {
                if (jsonStat.Dimension(i).role == "classification") {
                    dimensionLabels.push(jsonStat.Dimension(i).label);
                }
            }
            if (app.library.utility.arrayHasDuplicate(dimensionLabels)) {
                validationErrors.push(app.library.html.parseDynamicLabel("build-dimension", [jsonStat.extension.language.name]));
            }
        });
    }

    if (!validationErrors.length) {
        //check that everything is OK with new periods
        if (!app.build.update.isPeriodsDimensionsValid()) {
            validationErrors.push(app.label.static["build-update-period-error"]);
        }
    };

    if (!validationErrors.length) {
        if (!app.build.update.dimension.mapsValid) {
            validationErrors.push(app.label.static["build-update-map-error"]);
        }
    };

    if (!validationErrors.length) {
        var numCells = 1;
        var dimension = null;
        $.each(app.build.update.data.Dimension, function (index, value) {
            if (value.LngIsoCode == app.config.language.iso.code) {
                dimension = value;
            }
        });
        var numClassificationVariables = 1;
        $.each(dimension.Classification, function (index, value) {
            numClassificationVariables = numClassificationVariables * this.Variable.length
        });

        //get number of statistics and existing periods
        var numStatistics = null;
        var numPeriods = null;
        $(app.build.update.ajax.jsonStat).each(function (key, jsonStat) {
            if (jsonStat.extension.language.code == app.config.language.iso.code) {
                numStatistics = jsonStat.Dimension({ role: "metric" })[0].length;
                numPeriods = jsonStat.Dimension({ role: "time" })[0].length;
            }
        });

        //add new periods
        numPeriods = numPeriods + dimension.Frequency.Period.length;

        var numCells = numClassificationVariables * numStatistics * numPeriods;
        if (numCells > C_APP_CREATE_UPDATE_HARD_THRESHOLD) {
            validationErrors.push(app.library.html.parseDynamicLabel("build-threshold-exceeded", [app.library.utility.formatNumber(numCells), app.library.utility.formatNumber(C_APP_CREATE_UPDATE_HARD_THRESHOLD)]));
        }

        if (!validationErrors.length) {
            if (numCells < app.config.entity.build.threshold.soft) {
                app.build.update.confirmUpdateOutput();
            }
            else {
                api.modal.confirm(
                    app.library.html.parseDynamicLabel("confirm-update-csv-download", [app.library.utility.formatNumber(numCells)]),
                    app.build.update.confirmUpdateOutput
                );
            }
        }
    }
    if (validationErrors.length) {
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
    }
};

app.build.update.confirmUpdateOutput = function () {
    //populate json data object
    app.build.update.data = $.extend(true, app.build.update.data, {
        "MtrInput": app.build.update.upload.file.content.source.Base64,
        "MtrCode": $("#build-update-properties [name=mtr-value]").val(),
        "FrqCodeTimeval": $("#build-update-properties").find("[name=frequency-code]").val(),
        "FrqValueTimeval": $("#build-update-dimension-nav-collapse-properties-" + app.config.language.iso.code + " [name=frequency-value]").val(),
        "MtrOfficialFlag": $("#build-update-properties").find("[name=official-flag]").prop('checked'),
        "CprCode": $("#build-update-properties").find("[name=copyright-code]").val(),
        "Data": app.build.update.upload.file.content.data.JSON == null ? [] : app.build.update.upload.file.content.data.JSON.data
    });

    //build properties to dimensions 
    $(app.build.update.ajax.jsonStat).each(function (key, jsonStat) {
        var lngIsoCode = jsonStat.extension.language.code;
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

    if (app.build.update.rlsCode) {
        app.build.update.ajax.updateOutputRls();
    }
    else {
        app.build.update.upload.validate.ajax.read({ "callback": "app.build.update.upload.validate.callback.updateOutput", "unitsPerSecond": app.config.transfer.unitsPerSecond["PxStat.Build.Build_API.Update"] });
    }
}

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
    var params = $.extend(true, {}, app.build.update.data);

    //replace data with reduced structure
    params.Data = app.build.update.reduceData();
    // Merge Signature into Data
    params.Signature = app.build.update.upload.Signature;
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Build.Build_API.Update",
        params,
        "app.build.update.callback.updateOutput",
        params.FrmType,
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        });

};

app.build.update.ajax.updateOutputRls = function () {
    var params = $.extend(true, {}, app.build.update.data);
    //replace data with reduced structure
    params.Data = app.build.update.reduceData();
    delete params.MtrCode;
    delete params.MtrInput;
    delete params.Signature;
    params.RlsCode = app.build.update.rlsCode;


    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Build.Build_API.UpdateByRelease",
        params,
        "app.build.update.callback.updateOutputRls",
        params.FrmType,
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        });
};

app.build.update.callback.updateOutputRls = function (data, frmType) {
    if (data.file && data.file.length) {

        //If you try and update a px file with a data csv file that has no valid records, display an error modal instead of the report modal.
        if (!data.report.length && app.build.update.upload.file.content.data.JSON) {
            api.modal.information(app.label.static["invalid-csv-data-file"]);
            app.build.update.callback.downloadFile(data.file, frmType);
        }

        else if (data.report && data.report.length) {
            app.build.update.report.drawReport(data, frmType);
        }
        else {
            app.build.update.callback.downloadFile(data.file, frmType);
        }
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

app.build.update.reduceData = function () {
    //clone data object to tidy up before sending to server
    var params = $.extend(true, {}, app.build.update.data);

    //remove data properties not need by api, value columns may be included in data csv template
    var dimensionCodes = [];
    $(app.build.update.ajax.jsonStat).each(function (key, jsonStat) {
        if (jsonStat.extension.language.code == app.config.language.iso.code) {
            //get dimension codes from default JsonStat
            for (i = 0; i < jsonStat.length; i++) {
                if (jsonStat.Dimension(i).role != "time") {
                    //don't add the time code to dimension code array 
                    dimensionCodes.push(jsonStat.id[i]);
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

    //refactor data format to reduce payload
    var dataTbl = [];
    var dataTblHeaders = dimensionCodes;
    dataTblHeaders.push(C_APP_CSV_VALUE);
    dataTbl.push(dataTblHeaders);
    $(params.Data).each(function (keyObject, dataObject) {
        var tblRow = [];
        $.each(dataTblHeaders, function (index, value) {
            tblRow.push(dataObject[value])
        });
        dataTbl.push(tblRow);
    });
    return dataTbl
};

/**
 *Download updated bx file from data
 *
 * @param {*} data
 */
app.build.update.callback.updateOutput = function (data, frmType) {
    if (data.file && data.file.length) {

        //If you try and update a px file with a data csv file that has no valid records, display an error modal instead of the report modal.
        if (!data.report.length && app.build.update.upload.file.content.data.JSON) {
            api.modal.information(app.label.static["invalid-csv-data-file"]);
            app.build.update.callback.downloadFile(data.file, frmType);
        }

        else if (data.report && data.report.length) {
            app.build.update.report.drawReport(data, frmType);
        }
        else {
            app.build.update.callback.downloadFile(data.file, frmType);
        }
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }

};


app.build.update.callback.downloadFile = function (data, format) {
    if (app.build.update.import) {
        api.content.goTo("entity/build/import/", "#nav-link-build", "#nav-link-import", { "fileImport": data[0] });
    }
    else {
        var fileName = $("#build-update-properties [name=mtr-value]").val() + "." + moment(Date.now()).format(app.config.mask.datetime.file);

        switch (format) {
            case C_APP_TS_FORMAT_TYPE_PX:
                $.each(data, function (index, file) {
                    // Download the file
                    app.library.utility.download(fileName, file, C_APP_EXTENSION_PX, C_APP_MIMETYPE_PX);
                });
                break;
            case C_APP_TS_FORMAT_TYPE_JSONSTAT:
                $.each(data, function (index, file) {
                    // Download the file
                    app.library.utility.download(fileName, JSON.stringify(file), C_APP_EXTENSION_JSON, C_APP_MIMETYPE_JSON);
                });
                break;
            default:
                api.modal.exception(app.label.static["api-ajax-exception"]);
                break;
        };
    }


    $('#build-update-modal-view-report').modal('hide')
}
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
                validMatrix: true,
                normalizer: function (value) {
                    value = value.sanitise(C_SANITISE_UPPERCASE);
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

            app.build.update.upload.validate.ajax.read({ "callback": "app.build.update.upload.validate.callback.uploadSource", "unitsPerSecond": app.config.transfer.unitsPerSecond["PxStat.Build.Build_API.Read"] });
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

app.build.update.validate.dataPeriods = function () {
    $("#build-update-download-csv-file").find("form").trigger("reset").validate({
        rules: {
            "period-select": { required: true }
        },
        errorPlacement: function (error, element) {
            $("#build-update-download-csv-file [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.build.update.downloadCsv();
        }
    }).resetForm();
}

//#region Download CSV modal

app.build.update.downloadCsvModal = function () {
    var allPeriods = app.build.update.getAllPeriods();

    var numAllPeriods = 0;

    $.each(allPeriods, function (key, value) {
        $('#build-update-download-csv-file').find("[name=period-select]").append(
            $("<option>", {
                "value": key,
                "text": value
            })
        );
        numAllPeriods++
    });

    //put num variables into pill
    $("#build-update-download-csv-file").find("[name=dimension-count]").text(numAllPeriods);
    app.build.update.sortPeriods();

    //select all
    $("#build-update-download-csv-file").find("[name=select-all]").once("change", function () {
        if (this.checked) {
            $("#build-update-download-csv-file select").find('option').each(function () {
                $(this).prop("selected", true);
            });
        }
        else {
            $("#build-update-download-csv-file select").find('option').each(function () {
                $(this).prop("selected", false);
            });
        }
    });

    //Period Search
    $("#build-update-download-csv-file").find("[name=period-filter]").once("keyup search", function () {
        var select = $("#build-update-download-csv-file select");
        select.find('option').each(function () {
            if (!$(this).is(':selected')) {
                $(this).remove();
            }
        });

        var periodsToSearch = app.build.update.getAllPeriods();
        var filter = $(this).val().toLowerCase();
        $.each(periodsToSearch, function (key, value) {
            if ((value.toLowerCase().indexOf(filter) > -1) || $(this).is(':selected')) {
                //variable is valid, append to select if not already selected
                if (!select.find("option[value=" + key + "]").is(':selected')) {
                    select.append($('<option>', {
                        "value": key,
                        "title": value,
                        "text": value
                    }));
                };
            }
        });

        //if no match display no results - option disabled
        if (select.find('option').length == 0) {
            select.append($('<option>', {
                "title": app.label.static["no-results"],
                "text": app.label.static["no-results"],
                "disabled": "disabled"
            }));
            //disable select all
            $("#build-update-download-csv-file").find("[name=select-all]").prop("disabled", true);
        }
        //if option is checked, uncheck
        if (select.find('option:selected').length != select.find('option').length) {
            if ($("#build-update-download-csv-file").find("[name=select-all]").is(':checked')) {
                $("#build-update-download-csv-file").find("[name=select-all]").prop("checked", false);
            }

        }
        else {
            //enable select all
            $("#build-update-download-csv-file").find("[name=select-all]").prop("disabled", false);

        }

    });


    //sort
    $("#build-update-download-csv-file").find("[name=sort-options]").once("click", app.build.update.sortPeriods);
    app.build.update.validate.dataPeriods();
    $('#build-update-download-csv-file').modal("show");
    $('#build-update-download-csv-file').on('hide.bs.modal', function (e) {
        $("#build-update-download-csv-file select").attr("sort", "asc");
        $("#build-update-download-csv-file").find("[name=select-all]").prop("checked", false);
        $("#build-update-download-csv-file").find("[name=labels]").bootstrapToggle("off");
        $('#build-update-download-csv-file').find("[name=period-select]").empty();
        $('#build-update-download-csv-file').find("[name=period-filter]").val("");
    })
};

app.build.update.getAllPeriods = function () {

    var newPeriods = {};
    $(app.build.update.data.Dimension).each(function (key, value) {
        if (value.LngIsoCode == app.config.language.iso.code) {
            $(value.Frequency.Period).each(function (k, v) {
                newPeriods[v.PrdCode] = v.PrdValue
            })
        }
    });
    var existingPeriods = {};
    $(app.build.update.ajax.jsonStat).each(function (key, value) {
        if (value.extension.language.code == app.config.language.iso.code) {
            var jsonStat = JSONstat(value);
            var timeDimension = jsonStat.Dimension({ role: "time" })[0];
            //get dimension label from time dimension
            $("#build-update-download-csv-file").find("[name=dimension-label]").text($("#build-update-dimension-nav-collapse-properties-" + app.config.language.iso.code + " [name=frequency-value]").val());
            $(timeDimension.id).each(function (k, v) {
                existingPeriods[v] = timeDimension.Category(v).label
            });
        }
    });


    return $.extend(true, {}, newPeriods, existingPeriods);

}

app.build.update.sortPeriods = function () {


    var select = $("#build-update-download-csv-file select")
    var status = select.attr("sort");
    select.html(select.find('option').sort(function (x, y) {
        switch (status) {
            case "asc":
                select.attr("sort", "desc");
                return $(x).val() < $(y).val() ? 1 : -1;
            case "desc":
                select.attr("sort", "asc");
                return $(x).val() > $(y).val() ? 1 : -1;
            default:
                break;
        }

    }));

}

app.build.update.downloadCsv = function () {

    $(app.build.update.ajax.jsonStat).each(function (key, value) {
        if (value.extension.language.code == app.config.language.iso.code) {
            var jsonStat = JSONstat(value);
            var dimensions = jsonStat.Dimension();
            var numDatapoints = 1;
            $(dimensions).each(function (k, v) {
                if (v.role != "time") {
                    numDatapoints = numDatapoints * v.length
                }
            })
            numDatapoints = numDatapoints * $("#build-update-download-csv-file select").find('option:selected').length
            if (numDatapoints > C_APP_CREATE_UPDATE_HARD_THRESHOLD) {
                api.modal.error(app.library.html.parseDynamicLabel("build-threshold-exceeded", [app.library.utility.formatNumber(numDatapoints), app.library.utility.formatNumber(C_APP_CREATE_UPDATE_HARD_THRESHOLD)]));
            }

            else if (numDatapoints > app.config.entity.build.threshold.soft) {
                if (app.build.update.rlsCode) {
                    app.build.update.search.ajax.downloadCsvData();
                }
                else {
                    api.modal.confirm(
                        app.library.html.parseDynamicLabel("confirm-update-csv-download", [app.library.utility.formatNumber(numDatapoints)]),
                        app.build.update.upload.validate.ajax.read,
                        { "callback": "app.build.update.upload.validate.callback.downloadData", "unitsPerSecond": app.config.transfer.unitsPerSecond["PxStat.Build.Build_API.ReadDataset"] });
                }
            }
            else {
                if (app.build.update.rlsCode) {
                    app.build.update.search.ajax.downloadCsvData();
                }
                else {
                    app.build.update.upload.validate.ajax.read({ "callback": "app.build.update.upload.validate.callback.downloadData", "unitsPerSecond": app.config.transfer.unitsPerSecond["PxStat.Build.Build_API.ReadDataset"] });
                }
            }
        }
    })
}
//#endregion
