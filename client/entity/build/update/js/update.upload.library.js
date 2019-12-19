/*******************************************************************************
Custom JS application specific
*******************************************************************************/


//#region Add Namespace
app.build = app.build || {};
app.build.update = app.build.update || {};

app.build.update.upload = {};
app.build.update.upload.FrqCode = null;
app.build.update.upload.FrqValue = null;
app.build.update.upload.Signature = null;

app.build.update.upload.ajax = {};
app.build.update.upload.callback = {};

app.build.update.upload.validate = {};
app.build.update.upload.validate.ajax = {};
app.build.update.upload.validate.callback = {};

app.build.update.upload.file = {};
app.build.update.upload.file.content = {};

app.build.update.upload.file.content.source = {};
app.build.update.upload.file.content.source.UTF8 = null;
app.build.update.upload.file.content.source.Base64 = null;
app.build.update.upload.file.content.source.name = null;
app.build.update.upload.file.content.source.size = null;
app.build.update.upload.file.content.source.languages = [];

app.build.update.upload.file.content.period = {};
app.build.update.upload.file.content.period.UTF8 = null;
app.build.update.upload.file.content.period.data = {};
app.build.update.upload.file.content.period.data.JSON = null;

app.build.update.upload.file.content.data = {};
app.build.update.upload.file.content.data.JSON = null;
app.build.update.upload.file.content.data.name = null;
app.build.update.upload.file.content.data.size = null;


//#region Matrix Lookup

/**
 * Ajax read call
 */
app.build.update.upload.ajax.matrixLookup = function () {
    // Change app.config.language.iso.code to the selected one
    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.Data.Matrix_API.ReadCodeList",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.build.update.upload.callback.matrixLookup");
};

/**
* * Callback subject read
* @param  {} response
*/
app.build.update.upload.callback.matrixLookup = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        // Handle the Data in the Response then
        app.build.update.upload.callback.drawMatrix(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.build.update.upload.callback.drawCallbackDrawMatrix = function () {
    $('[data-toggle="tooltip"]').tooltip();
    // Responsive
    $("#build-create-initiate-matrix-lookup table").DataTable().columns.adjust().responsive.recalc();
}


/**
* Draw table
* @param {*} data
*/
app.build.update.upload.callback.drawMatrix = function (data) {
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
                        return app.library.html.tooltip(row.MtrCode, row.MtrTitle);
                    }
                },
            ],
            drawCallback: function (settings) {
                app.build.update.upload.callback.drawCallbackDrawMatrix();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        //Initialize DataTable
        $("#build-update-matrix-lookup table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.upload.callback.drawCallbackDrawMatrix();
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
    $('#build-update-matrix-lookup').modal("show");
};

//#endregion




/**
 * Custom validation for Files
 */
app.build.update.upload.validate.sourceFile = function () {
    if (!app.build.update.upload.file.content.source.UTF8 || !app.build.update.upload.file.content.source.Base64) {
        //Disable 'Update Metadata' button and set freq code and copyright code
        $("#build-update-upload-file").find("[name=upload-source-file]").prop("disabled", true);
        $("#build-update-upload-file").find("[name=file-data-view]").prop("disabled", true);
        return false;
    } else {
        //enable 'Update Metadata' button and set freq code and copyright code
        $("#build-update-upload-file").find("[name=upload-source-file]").prop("disabled", false);
        $("#build-update-upload-file").find("[name=file-data-view]").prop("disabled", false);
        return true;
    }
};

/**
 * Custom validation for Files
 */
app.build.update.upload.validate.periodFile = function () {
    if (!app.build.update.upload.file.content.period.UTF8) {
        //Disable 
        $("#build-update-upload-periods").find("[name=upload-submit-periods]").prop("disabled", true);
        return false;
    } else {
        // Enable 
        $("#build-update-upload-periods").find("[name=upload-submit-periods]").prop("disabled", false);
        return true;
    }

};

/**
 * Custom validation for Files
 */
app.build.update.upload.validate.dataFile = function () {
    var isValid = true;

    if (!app.build.update.upload.file.content.data.JSON) {
        isValid = false;
    }

    //check for errors in csv
    if (app.build.update.upload.file.content.data.JSON.errors.length) {
        isValid = false;
    }


    //get default language JsonStat
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

    var csvHeaders = app.build.update.upload.file.content.data.JSON.meta.fields;

    //check that dimension codes are in csv header codes
    $(dimensionCodes).each(function (key, code) {
        if (jQuery.inArray(code, csvHeaders) == -1) {
            isValid = false;
            return false;
        };
    });

    return isValid;
};


/**
 *
 *
 * @param {*} files
 * @param {*} inputObject
 * @returns
 */
api.plugin.dragndrop.readFiles = function (files, inputObject) {
    // Read single file only
    var file = files[0];
    if (!file) {
        return;
    }

    // info on screen 
    inputObject.parent().find("[name=file-tip]").hide();
    inputObject.parent().find("[name=file-name]").html(file.name + " (" + app.library.utility.formatNumber(Math.ceil(file.size / 1024)) + " KB)").show();

    var uploadId = inputObject.attr("id");

    switch (uploadId) {
        case "build-update-upload-file-input":

            //always hide the dimension details card every time you drpp a file in
            $("#build-update-dimensions").hide();

            //clean up
            app.build.update.upload.FrqCode = null;
            app.build.update.upload.FrqValue = null;
            app.build.update.upload.Signature = null;
            app.build.update.upload.file.content.source.UTF8 = null;
            app.build.update.upload.file.content.source.Base64 = null;

            $("#build-update-upload-file").find("[name=upload-error]").empty();
            $("#build-update-upload-file").find("[name=upload-error-card]").hide();

            //Reset Frequency and Copyright dropdowns every time you drop a new file in
            $("#build-update-properties [name=frequency-code]").val();
            $("#build-update-properties [name=copyright-code]").val();

            $("#build-update-upload-file").find("[name=upload-source-file]").prop("disabled", true);
            $("#build-update-upload-file").find("[name=file-data-view]").prop("disabled", true);

            app.build.update.upload.file.content.source.name = file.name;
            app.build.update.upload.file.content.source.size = file.size;
            var fileExt = file.name.match(/\.[0-9a-z]+$/i)[0];

            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(fileExt.toLowerCase(), C_APP_UPDATEDATASET_FILE_ALLOWED_EXTENSION) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [fileExt]));
                return;
            }
            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(file.type.toLowerCase(), C_APP_UPDATEDATASET_FILE_ALLOWED_TYPE) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [file.type]));
                return;
            }

            // Check for the hard limit of the file size
            if (file.size > app.config.upload.threshold.hard) {
                // Show Error    
                api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.library.utility.formatNumber(Math.ceil(app.config.upload.threshold.hard / 1024 / 1024)) + " MB"]));
                return;
            }

            // Read file into an UTF8 string
            var readerUTF8 = new FileReader();
            readerUTF8.onload = function (e) {
                app.build.update.upload.file.content.source.UTF8 = e.target.result;
            };
            readerUTF8.readAsText(file);
            readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("loadend", function (e) {
                // Validate the file
                app.build.update.upload.validate.sourceFile();
                api.spinner.stop();
            });

            // Read file into a Base64 string
            var readerBase64 = new FileReader();
            readerBase64.onload = function (e) {
                // Set the file's content
                app.build.update.upload.file.content.source.Base64 = e.target.result;
            };
            readerBase64.readAsDataURL(file);
            readerBase64.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerBase64.addEventListener("error", function (e) { api.spinner.stop(); });
            readerBase64.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerBase64.addEventListener("loadend", function (e) {
                // Validate the file
                app.build.update.upload.validate.sourceFile();
                api.spinner.stop();
            });
            break;

        case "build-update-upload-periods-file":
            //clean up

            $('#build-update-upload-periods').find("[name=errors-card]").hide();
            $('#build-update-upload-periods').find("[name=errors]").empty();

            app.build.update.upload.file.content.period.UTF8 = null;
            $("#build-update-upload-periods").find("[name=upload-submit-periods]").prop("disabled", true);

            var fileExt = file.name.match(/\.[0-9a-z]+$/i)[0];

            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(fileExt.toLowerCase(), C_APP_UPDATEDATASET_DATA_FILE_ALLOWED_EXTENSION) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [fileExt]));
                return;
            }
            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(file.type.toLowerCase(), C_APP_UPDATEDATASET_DATA_FILE_ALLOWED_TYPE) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [file.type]));
                return;
            }

            // Check for the hard limit of the file size
            if (file.size > app.config.upload.threshold.hard) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.library.utility.formatNumber(Math.ceil(app.config.upload.threshold.hard / 1024 / 1024)) + " MB"]));
                return;
            }

            // Read file into an UTF8 string
            var readerUTF8 = new FileReader();
            readerUTF8.onload = function (e) {
                app.build.update.upload.file.content.period.UTF8 = e.target.result;
            };
            readerUTF8.readAsText(file);
            readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("loadend", function (e) {
                // Validate the file
                app.build.update.upload.validate.periodFile();
                api.spinner.stop();
            });

            break;

        case "build-update-upload-data":
            //clean up
            app.build.update.upload.file.content.data.name = file.name;
            app.build.update.upload.file.content.data.size = file.size;

            var fileExt = file.name.match(/\.[0-9a-z]+$/i)[0];

            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(fileExt.toLowerCase(), C_APP_UPDATEDATASET_DATA_FILE_ALLOWED_EXTENSION) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [fileExt]));
                return;
            }
            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(file.type.toLowerCase(), C_APP_UPDATEDATASET_DATA_FILE_ALLOWED_TYPE) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [file.type]));
                return;
            }

            // Check for the hard limit of the file size
            if (file.size > app.config.upload.threshold.hard) {
                // Show Error               
                api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.library.utility.formatNumber(Math.ceil(app.config.upload.threshold.hard / 1024 / 1024)) + " MB"]));
                return;
            }

            // Read file into an UTF8 string
            var readerUTF8 = new FileReader();
            readerUTF8.onload = function (e) {
                app.build.update.upload.file.content.data.JSON = Papa.parse(e.target.result, {
                    header: true,
                    skipEmptyLines: true
                });
            };

            readerUTF8.readAsText(file);
            readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("loadend", function (e) {
                // Validate the file
                if (app.build.update.upload.validate.dataFile()) {
                    $("#build-update-dimensions").find("[name=update]").prop("disabled", false);
                    $("#build-update-matrix-data").find("[name=preview-data]").prop("disabled", false);
                } else {
                    // Something went wrong
                    api.modal.error(app.label.static["invalid-csv-format"]);
                    $("#build-update-dimensions").find("[name=update]").prop("disabled", true);
                }

                api.spinner.stop();
            });
            break;

        default:
            break;
    }
};

/**
 *
 */
app.build.update.upload.validate.ajax.read = function (callbackOnSuccess, unitsPerSecond) {

    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Build.Build_API.Validate",
        {
            "FrqCodeTimeval": app.build.update.upload.FrqCode,
            "FrqValueTimeval": app.build.update.upload.FrqValue,
            "MtrInput": app.build.update.upload.file.content.source.Base64
        },
        callbackOnSuccess,
        null,
        null,
        null,
        {
            async: false,
            timeout: app.config.upload.timeout
        }
    );
    // Add the progress bar
    api.spinner.progress.start(api.spinner.progress.getTimeout(app.build.update.upload.file.content.source.Base64.length, unitsPerSecond));
};

/**
 * 
 */
app.build.update.upload.validate.callback.uploadSource = function (response) {
    if (response.error) {
        var errorOutput = $("<ul>", {
            class: "list-group"
        });
        if (Array.isArray(response.error.message)) {
            $.each(response.error.message, function (_index, value) {
                var error = $("<li>", {
                    class: "list-group-item",
                    html: value.ErrorMessage
                });
                errorOutput.append(error);
            });
        } else {
            var error = $("<li>", {
                class: "list-group-item",
                html: response.error.message
            });
            errorOutput.append(error);
        }

        $("#build-update-upload-file").find("[name=upload-error]").html(errorOutput.get(0).outerHTML);
        $("#build-update-upload-file").find("[name=upload-error-card]").fadeIn();
        api.modal.error(errorOutput);
    } else if (response.data) {
        if (response.data.Signature) {
            // Store for later use
            app.build.update.upload.Signature = response.data.Signature;
            app.build.update.upload.ajax.read();
        }
        else {
            // Populate the Frequency list
            $("#build-update-modal-frequency").find("[name=frequency-radio-group]").empty();

            $.each(response.data.FrqValueCandidate, function (key, value) {
                $("#build-update-modal-frequency").find("[name=frequency-radio-group]").append(function () {
                    return $("<li>", {
                        "class": "list-group-item",
                        "html": $("<input>", {
                            "type": "radio",
                            "name": "frq-value",
                            "value": value
                        }).get(0).outerHTML + " " + value
                    }).get(0).outerHTML;
                });
            });
            // Run validation
            app.build.update.validate.frequencyModal();
            // Show the modal
            $("#build-update-modal-frequency").modal("show");
        }
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
}

/**
 * 
 */
app.build.update.upload.validate.callback.downloadDataTemplate = function (response) {
    // N.B. This is a silent validation to cathc hacks only
    if (response.data && response.data.Signature) {
        // Store for later use
        app.build.update.upload.Signature = response.data.Signature;
        app.build.update.ajax.downloadDataTemplate();
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
}

/**
 * 
 */
app.build.update.upload.validate.callback.downloadAllData = function (response) {
    // N.B. This is a silent validation to cathc hacks only
    if (response.data && response.data.Signature) {
        // Store for later use
        app.build.update.upload.Signature = response.data.Signature;

        var params = {
            "MtrInput": app.build.update.upload.file.content.source.Base64,
            "FrqValueTimeval": app.build.update.upload.FrqValue,
            "FrqCodeTimeval": app.build.update.upload.FrqCode,
            "Signature": app.build.update.upload.Signature,
            "Dimension": []
        };

        //get new periods
        $.each(app.build.update.data.Dimension, function (index, dimension) {
            var language = {
                "LngIsoCode": dimension.LngIsoCode,
                "Frequency": {
                    "Period": []
                }
            }
            language.Frequency.Period = dimension.Frequency.Period;
            params.Dimension.push(language);
        });

        app.build.update.ajax.downloadAllData(params);
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
}

/**
 * 
 */
app.build.update.upload.validate.callback.downloadNewData = function (response) {
    // N.B. This is a silent validation to cathc hacks only
    if (response.data && response.data.Signature) {
        // Store for later use
        app.build.update.upload.Signature = response.data.Signature;

        var params = {
            "MtrInput": app.build.update.upload.file.content.source.Base64,
            "FrqValueTimeval": app.build.update.upload.FrqValue,
            "FrqCodeTimeval": app.build.update.upload.FrqCode,
            "Signature": app.build.update.upload.Signature,
            "Dimension": []
        };

        //get new periods
        $.each(app.build.update.data.Dimension, function (index, dimension) {
            var language = {
                "LngIsoCode": dimension.LngIsoCode,
                "Frequency": {
                    "Period": []
                }
            }
            language.Frequency.Period = dimension.Frequency.Period;
            params.Dimension.push(language);
        });

        app.build.update.ajax.downloadNewData(params);
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
}

/**
 * 
 */
app.build.update.upload.validate.callback.downloadExistingData = function (response) {
    // N.B. This is a silent validation to cathc hacks only
    if (response.data && response.data.Signature) {
        // Store for later use
        app.build.update.upload.Signature = response.data.Signature;

        var params = {
            "MtrInput": app.build.update.upload.file.content.source.Base64,
            "FrqValueTimeval": app.build.update.upload.FrqValue,
            "FrqCodeTimeval": app.build.update.upload.FrqCode,
            "Signature": app.build.update.upload.Signature,
            "Dimension": []
        };

        //get new periods
        $.each(app.build.update.data.Dimension, function (index, dimension) {
            var language = {
                "LngIsoCode": dimension.LngIsoCode
            }
            params.Dimension.push(language);
        });

        app.build.update.ajax.downloadExistingData(params);
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
}

/**
 * 
 */
app.build.update.upload.validate.callback.updateOutput = function (response) {
    // N.B. This is a silent validation to cathc hacks only
    if (response.data && response.data.Signature) {
        // Store for later use
        app.build.update.upload.Signature = response.data.Signature;
        app.build.update.ajax.updateOutput();
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
}

/**
 *Get JSON-stat from px file
 *
 */
app.build.update.upload.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Build.Build_API.Read",
        {
            "FrqCodeTimeval": app.build.update.upload.FrqCode,
            "FrqValueTimeval": app.build.update.upload.FrqValue,
            "MtrInput": app.build.update.upload.file.content.source.Base64,
            "Signature": app.build.update.upload.Signature
        },
        "app.build.update.upload.callback.readInput",
        null,
        null,
        null,
        { async: false });
};

/**  
 * 
 */
app.build.update.upload.callback.readInput = function (response) {
    //put JSON-stat into namespace variable for future use    
    if (response.error) {
        var errorOutput = $("<ul>", {
            class: "list-group"
        });
        if (Array.isArray(response.error.message)) {
            $.each(response.error.message, function (_index, value) {
                var error = $("<li>", {
                    class: "list-group-item",
                    html: value.ErrorMessage
                });
                errorOutput.append(error);
            });
        } else {
            var error = $("<li>", {
                class: "list-group-item",
                html: response.error.message
            });
            errorOutput.append(error);
        }
        api.modal.error(errorOutput);

        $("#build-update-upload-file").find("[name=upload-error]").html(errorOutput.get(0).outerHTML);
        $("#build-update-upload-file").find("[name=upload-error-card]").fadeIn();
    }

    else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        api.modal.information(app.label.static["api-ajax-nodata"]);
        // Do nothing 
    }
    else if (response.data !== undefined) {
        app.build.update.ajax.response = response.data;
        $("#build-update-upload-file").find("[name=upload-error-card]").hide();
        app.build.update.upload.drawProperties();
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.build.update.upload.drawProperties = function () {
    $("#build-update-dimensions").find("[name=update-error-card]").hide();
    $("#build-update-upload-file").find("[name=upload-source-file]").prop("disabled", true);
    $("#build-update-dimensions").fadeIn();
    $("#build-update-properties [name=lng-list-group]").empty();

    //clear tinymce in template
    if (tinymce.editors.length) {
        tinymce.remove();
    }

    //empty any previous tabs
    $("#build-update-matrix-dimensions [name=nav-tab]").empty();
    $("#build-update-matrix-dimensions").find("[name=tab-content]").empty();

    app.build.update.data.Dimension = [];
    app.build.update.upload.file.content.source.languages = [];
    $(app.build.update.ajax.response).each(function (key, value) {
        var data = JSONstat(value);
        app.build.update.upload.file.content.source.languages.push(
            {
                lngIsoCode: data.extension.language.code,
                lngIsoName: data.extension.language.name
            }
        );
        var lngIsoCode = data.extension.language.code;

        app.build.update.data.Dimension.push({
            "LngIsoCode": lngIsoCode,
            "MtrTitle": null,
            "MtrNote": null,
            "StatisticLabel": null,
            "Classification": [],
            "Frequency": {
                "FrqValue": null,
                "Period": []
            }
        });

        // Set properties from the Default language
        if (lngIsoCode == app.config.language.iso.code) {
            //set matrix properties from default language JSON-stat
            $("#build-update-properties [name=mtr-value]").val(data.extension.matrix);

            //set frequency code to value from px file
            $("#build-update-properties [name=frequency-code] > option").each(function () {
                if (this.value == data.role.time[0]) {
                    $("#build-update-properties [name=frequency-code]").val(this.value);
                }
            });

            //set copyright to value from px file
            $("#build-update-properties [name=copyright-code] > option").each(function () {
                if (this.text == data.extension.copyright.name) {
                    $("#build-update-properties [name=copyright-code]").val(this.value);
                }
            });

            //set official statistics to value from px file
            if (data.extension.official) {
                $("#build-update-properties [name=official-flag]").prop('checked', true);
            }

            else {
                $("#build-update-properties [name=official-flag]").prop('checked', false);
            }
        }

        $("#build-update-properties [name=official-flag]").bootstrapToggle("destroy").bootstrapToggle({
            on: app.label.static["true"],
            off: app.label.static["false"],
            onstyle: "success",
            offstyle: "warning",
            width: C_APP_TOGGLE_LENGTH
        });

        var lngIsoValue = data.extension.language.name;
        $("#build-update-properties [name=lng-list-group]").append(
            $("<li>", {
                "class": "list-group-item",
                "text": lngIsoValue + " (" + lngIsoCode + ")"
            })
        );

        //draw dimensions

        //set up tab for each language
        //each accordion must have unique id to toggle, each collapse must unique id. Use language code to build unique id's
        var tabLanguageItem = $("#build-update-dimension-metadata-templates").find("[name=nav-lng-tab-item]").clone(); // Tabs item item
        //Set values
        tabLanguageItem.attr("lng-iso-code", lngIsoCode);
        tabLanguageItem.attr("id", "build-update-dimension-nav-" + lngIsoCode + "-tab");
        tabLanguageItem.attr("href", "#build-update-dimension-nav-" + lngIsoCode);
        tabLanguageItem.attr("aria-controls", "nav-" + lngIsoCode);
        tabLanguageItem.text(lngIsoValue);

        //set first tab to be active
        if (key === 0) {
            tabLanguageItem.addClass("active show");
        }

        $("#build-update-matrix-dimensions [name=nav-tab]").append(tabLanguageItem.get(0).outerHTML);

        //set up tab content for each language
        var tabContent = $("#build-update-dimension-metadata-templates").find("[name=nav-lng-tab-item-content]").clone();
        tabContent.attr("id", "build-update-dimension-nav-" + lngIsoCode);
        tabContent.attr("lng-iso-code", lngIsoCode);

        //set first tab content to be active
        if (key === 0) {
            tabContent.addClass("active show");
        }

        tabContent.find(".accordion").attr("id", "build-update-dimension-nav-" + lngIsoCode);
        $.each(tabContent.find("[name=dimension-collapse]"), function () {
            $(this).find(".collapse").attr("data-parent", "#" + "build-update-dimension-nav-" + lngIsoCode);
            $(this).find(".collapse").attr("id", "build-update-dimension-nav-collapse-" + $(this).attr("card") + "-" + lngIsoCode);
            $(this).find(".card-header").find(".btn-link").attr("data-target", "#build-update-dimension-nav-collapse-" + $(this).attr("card") + "-" + lngIsoCode);
            $(this).find(".card-header").find(".btn-link").attr("aria-controls", "collapse-" + $(this).attr("card"));
        });

        $("#build-update-matrix-dimensions").find("[name=tab-content]").append(tabContent.get(0).outerHTML);

        //initiate validation after tab content drawn
        app.build.update.validate.dimensionProperty(data.extension.language.code);

        //set content of tab from JSON-stat
        var statisticsData = [];
        var periodsDataExisting = [];
        $("#build-update-dimension-nav-collapse-properties-" + lngIsoCode + " [name=title-value]").val(data.label);
        for (i = 0; i < data.length; i++) {

            var dimension = data.Dimension(i);
            if (data.Dimension(i).role == "metric") {
                $("#build-update-dimension-nav-collapse-properties-" + lngIsoCode + " [name=statistic-label]").val(dimension.label);

                //set up statistics object for datatable
                $.each(data.Dimension(i).id, function (index, value) {
                    statisticsData.push(
                        {
                            "SttCode": value,
                            "SttValue": data.Dimension(i).Category(index).label,
                            "SttUnit": data.Dimension(i).Category(index).unit.label,
                            "SttDecimal": data.Dimension(i).Category(index).unit.decimals
                        }
                    );
                });
            }

            if (data.Dimension(i).role == "time") {
                $("#build-update-dimension-nav-collapse-properties-" + lngIsoCode + " [name=frequency-value]").val(dimension.label);
                //set up periods object for datatable
                $.each(data.Dimension(i).id, function (index, value) {
                    periodsDataExisting.push(
                        {
                            "PrdCode": value,
                            "PrdValue": data.Dimension(i).Category(index).label
                        }
                    );
                });
            }
            if (data.Dimension(i).role == "classification") {
                var mapUrl = null;
                if (data.Dimension(i).link) {
                    mapUrl = data.Dimension(i).link.enclosure[0].href;
                }
                //classification datatable object read from app.build.update.data as we might be updating this object
                $.each(app.build.update.data.Dimension, function (dimensionIndex, value) {
                    if (value.LngIsoCode == lngIsoCode) {
                        var classification = {
                            "ClsCode": data.id[i],
                            "ClsValue": data.Dimension(i).label,
                            "ClsGeoUrl": mapUrl,
                            "Variable": []
                        };


                        $.each(data.Dimension(i).id, function (index, value) {
                            classification.Variable.push(
                                {
                                    "VrbCode": value,
                                    "VrbValue": data.Dimension(i).Category(index).label
                                }
                            );
                        });

                        this.Classification.push(classification);
                    }
                });
            }
        };

        //Don't use tinymce set content as 'once' change event within app.library.utility.initTinyMce won't trigger
        $("#build-update-dimension-nav-collapse-properties-" + lngIsoCode + " [name=note-value]").val(data.note.join(" "));

        //draw dimension datatables
        app.build.update.dimension.drawStatistic(lngIsoCode, statisticsData);
        app.build.update.dimension.drawClassification(lngIsoCode);
        app.build.update.dimension.drawExistingPeriod(lngIsoCode, periodsDataExisting);
        app.build.update.dimension.drawNewPeriod(lngIsoCode);

        $("#build-update-dimension-nav-" + lngIsoCode).on('hide.bs.collapse show.bs.collapse', function (e) {
            $("#build-update-dimension-nav-" + lngIsoCode).find("[type=submit]").trigger("click");
            if (!app.build.update.validate.isDimensionPropertyValid) {
                //keep properties card open while properties are invalid
                e.preventDefault();
            }
        });

        $("#build-update-dimension-nav-" + lngIsoCode).find("[name=add-periods]").once("click", function () {

            $('#build-update-manual-periods table').find("tbody").empty();

            $('#build-update-manual-periods table').editableTableWidget();

            $("#build-update-manual-periods").find("[name=add-period-row]").once("click", function () {

                $('#build-update-manual-periods table').find("tbody").append(
                    $("<tr>", {
                        "html": $("<th>", {
                            "idn": "row-number",
                            "class": "table-light"
                        }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "code"
                            }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "value"
                            }).get(0).outerHTML +
                            $("<th>", {
                                "html": app.library.html.deleteButton()
                            }).get(0).outerHTML
                    }).get(0).outerHTML
                );

                //redraw row numbers
                $('#build-update-manual-periods table').find("tbody").find("tr").each(function (index, value) {
                    $(this).find("th").first().text(index + 1);
                });

                $('#build-update-manual-periods table').find("button[name=" + C_APP_NAME_LINK_DELETE + "]").once('click', function () {
                    $(this).parent().parent().remove();
                    //redraw row numbers after delete
                    $('#build-update-manual-periods table').find("tbody").find("tr").each(function (index, value) {
                        $(this).find("th").first().text(index + 1);
                    });
                    $('#build-update-manual-periods').find("[name=manual-si-errors-card]").hide();
                    $('#build-update-manual-periods').find("[name=manual-si-errors]").empty();
                });
                $('#build-update-manual-periods table').editableTableWidget();
            });
            $("#build-update-manual-periods").find("[name=add-period-row]").trigger("click");

            $("#build-update-new-periods").modal("show");
        });
    });

    //if any matrix properties change trigger submit button to run validation
    $("#build-update-properties [name=mtr-value],#build-update-properties [name=frequency-code], #build-update-properties [name=copyright-code]").once('change', function () {
        $("#build-update-properties").find("[type=submit]").trigger("click");
    });

    //scroll to matrix properties
    $('html, body').animate({ scrollTop: $('#build-update-dimensions').offset().top }, 1000);

    //trigger matrix properties submit to highlight any errors when file is uploaded
    $("#build-update-properties").find("[type=submit]").trigger("click");

    //enable download template if valid frqCode
    if ($("#build-update-properties").find("[name=frequency-code]").val()) {
        $("#build-update-matrix-data").find("[name=download-data-template], [name=download-data-file]").prop('disabled', false);
    };

    //enable download template when changing frqCode
    $("#build-update-properties").find("[name=frequency-code]").once('change', function () {
        $("#build-update-matrix-data").find("[name=download-data-template], [name=download-data-file]").prop('disabled', false);
    });

    // Initiate all text areas as tinyMCE
    app.library.utility.initTinyMce();
};

/**  
 * 
 */
app.build.update.upload.previewSource = function () {
    api.spinner.start();
    $("#build-update-modal-preview-source").modal("show");
    api.content.load("#build-update-modal-preview-source .modal-body", "entity/build/update/index.upload.preview.html");
    api.spinner.stop();
};

/**  
 * 
 */
app.build.update.upload.reset = function () {
    //reload all content into page
    api.content.load("#body", "entity/build/update/index.html");
};

