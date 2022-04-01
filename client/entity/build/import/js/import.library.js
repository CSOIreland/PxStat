/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
// Methods
app.build = app.build || {};
app.build.import = {};
app.build.import.ajax = {};
app.build.import.callback = {};
app.build.import.validation = {};
app.build.import.FrqValue = null;
app.build.import.FrqCode = null;

// Properties
app.build.import.file = {};
app.build.import.file.signature = null;
app.build.import.file.name = null;
app.build.import.file.extension = null;
app.build.import.file.type = null;
app.build.import.file.size = null;
app.build.import.file.content = {};
app.build.import.file.content.UTF8 = null;
app.build.import.file.content.Base64 = null;
//#endregion

//#region Miscellaneous
/**
 * Cancel the Upload only
 */
app.build.import.cancel = function () {
    // Change button to Validate
    app.build.import.setValidateBtn();
    // Clear file preview
    $("#build-import-container").find("[name=import-file-preview]").empty();
    $("#build-import-container").find("[name=preview-container]").hide();
    // Clear file errors
    $("#build-import-container").find("[name=select-group-error-holder]").empty();
    // Clear file details
    $("#build-import-container").find("[name=upload-file-name]").empty().hide();
    $("#build-import-container").find("[name=upload-file-tip]").show();
    $("#build-import-container").find("[name=validate]").prop("disabled", true);
    $("#build-import-container").find("[name=preview]").prop("disabled", true);
    // Invalidate frequency
    app.build.import.FrqValue = null;
    app.build.import.FrqCode = null;
    // Invalidate data upload
    app.build.import.file.signature = null;
    app.build.import.file.name = null;
    app.build.import.file.extension = null;
    app.build.import.file.type = null;
    app.build.import.file.size = null;
    app.build.import.file.content.UTF8 = null;
    app.build.import.file.content.Base64 = null;
};

/**
 * Reset entire upload page
 */
app.build.import.reset = function () {
    // Clear Group selection
    $("#build-import-container").find("[name=select-group]").val("").trigger('change');

    // Clear input file 
    $("#build-import-container").find("[name=build-import-file]").val("");
    // Clear Upload
    app.build.import.cancel();
};

/**
 * Change to upload button
 */
app.build.import.setImportBtn = function () {
    $("#build-import-container").find("[name=validate]").prop("disabled", true);
    $("#build-import-container").find("[name=import]").prop("disabled", false);
};

/**
 * Change to validate button
 */
app.build.import.setValidateBtn = function () {
    // Enable Validate Button
    $("#build-import-container").find("[name=validate]").prop("disabled", false);
    $("#build-import-container").find("[name=import]").prop("disabled", true);
};

/**
 * Set up date picker
 */
app.build.import.setDatePicker = function () {
    var start = moment();
    var end = moment();
    app.build.import.ajax.importHistory(start, end);
    $('#build-import-import-history-date-picker span').html(start.format(app.config.mask.date.display) + " - " + end.format(app.config.mask.date.display));

    $('#build-import-import-history-date-picker').daterangepicker({
        maxDate: new Date(),
        ranges: {
            [app.label.static["last-30-days"]]: [moment().subtract(29, 'days'), moment()],
            [app.label.static["last-7-days"]]: [moment().subtract(6, 'days'), moment()]
        },
        locale: app.label.plugin.daterangepicker
    }, function (start, end) {
        $('#build-import-import-history-date-picker span').html(start.format(app.config.mask.date.display) + " - " + end.format(app.config.mask.date.display));
        app.build.import.ajax.importHistory(start, end);
    });
};


//#endregion

//#region Create
/**
 * Custom validation for Files
 */
app.build.import.validation.file = function () {
    if (!app.build.import.file.content.UTF8 || !app.build.import.file.content.Base64) {
        $("#build-import-container").find("[name=validate]").prop("disabled", true);
        $("#build-import-container").find("[name=preview]").prop("disabled", true);
        return false;
    } else {
        $("#build-import-container").find("[name=preview]").prop("disabled", false);
        $("#build-import-container").find("[name=validate]").prop("disabled", false);
        return true;
    }
};

/**
 * Bind validation for the upload
 */
app.build.import.validation.create = function () {
    $("#build-import-container form").trigger("reset").validate({
        rules: {
            "select-group": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#build-import-container [name=" + element[0].name + "-error-holder]").append(error[0]);
            app.build.import.validation.file();
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            if (!app.build.import.validation.file()) {
                return;
            }
            if (app.build.import.file.signature) {
                app.build.import.ajax.create();
            }
            else {
                app.build.import.ajax.validate();
            }
        }
    }).resetForm();
};

/**
 * Bind validation for frequency validation
 */
app.build.import.validation.frequency = function () {
    $("#build-import-modal-frequency form").trigger("reset").validate({
        rules: {
            "frq-value": {
                required: true
            },
            "frq-code": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#build-import-modal-frequency [name=" + element[0].name + "-error-holder]").append(error[0]);
            app.build.import.validation.file();
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            // Store for later use
            app.build.import.FrqValue = $("#build-import-modal-frequency").find("[name=frq-value]:checked").val();
            app.build.import.FrqCode = $("#build-import-modal-frequency").find("[name=frq-code]").val();
            app.build.import.ajax.validate();
        }
    }).resetForm();
};

/**
 * Call to validate upload
 */
app.build.import.ajax.validate = function () {
    var obj2send = {
        "MtrInput": app.build.import.file.content.Base64,
        "GrpCode": $("#build-import-container").find("[name=select-group]").val(),
        "FrqCodeTimeval": app.build.import.FrqCode,
        "FrqValueTimeval": app.build.import.FrqValue,
        "LngIsoCode": app.label.language.iso.code
    };
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.Validate",
        obj2send,
        "app.build.import.callback.validateOnSuccess",
        null,
        "app.build.import.callback.validateOnError",
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        }
    );

    // Add the progress bar
    api.spinner.progress.start(api.spinner.progress.getTimeout(app.build.import.file.content.Base64.length, app.config.transfer.unitsPerSecond["PxStat.Data.Matrix_API.Validate"]));
};

/**
 * Validate and upload px files
 * @param {*} data
 */
app.build.import.callback.validateOnSuccess = function (data) {
    // Hide Frequency modal anyway
    $("#build-import-modal-frequency").modal("hide");
    if (data) {
        // Check for signature
        if (data.Signature) {
            app.build.import.file.signature = data.Signature;
            // Change button to Upload
            app.build.import.setImportBtn();
            api.modal.success(app.library.html.parseDynamicLabel("success-file-validated", [app.build.import.file.name]));
        } else if (Array.isArray(data.FrqValueCandidate) && data.FrqValueCandidate.length) {
            // Populate the Frequency list
            $("#build-import-modal-frequency").find("[name=frequency-radio-group]").empty();

            $.each(data.FrqValueCandidate, function (key, value) {
                $("#build-import-modal-frequency").find("[name=frequency-radio-group]").append(function () {
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
            $("#build-import-modal-frequency").modal("show");

        } else api.modal.exception(app.label.static["api-ajax-exception"]);
    } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Validate and upload px files
 * @param {*} error
 */
app.build.import.callback.validateOnError = function (error) {
    // Hide Frequency modal anyway
    $("#build-import-modal-frequency").modal("hide");

    // Disable Validate Button
    $("#build-import-container").find("[name=validate]").prop("disabled", true);
};

/**
 * Call to get frequency codes
 * 
 */

app.build.import.ajax.getFrequencyCodes = function () {
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Frequency_API.Read",
        null,
        "app.build.import.callback.getFrequencyCodes");
}

/**
 * Call to get frequency codes
 * 
 */

app.build.import.callback.getFrequencyCodes = function (data) {
    if (data && Array.isArray(data) && data.length) {
        // Map API data to select dropdown  model for main Subject search and update Subject search
        $("#build-import-modal-frequency").find("[name=frq-code]").empty().append($("<option>", {
            "text": app.label.static["select-uppercase"],
            "disabled": "disabled",
            "selected": "selected"
        }));

        $.each(data, function (key, value) {
            $("#build-import-modal-frequency").find("[name=frq-code]").append($("<option>", {
                "value": value.FrqCode,
                "text": value.FrqCode + " - " + app.label.static[value.FrqValue]
            }));
        });

        $("#build-import-modal-frequency").find("[name=frq-code]").prop('disabled', false);
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
}

/**
 * Call to create upload
 */
app.build.import.ajax.create = function (overwrite) {
    // Default params
    overwrite = overwrite || false;
    var obj2send = {
        "MtrInput": app.build.import.file.content.Base64,
        "GrpCode": $("#build-import-container").find("[name=select-group]").val(),
        "FrqValueTimeval": app.build.import.FrqValue,
        "FrqCodeTimeval": app.build.import.FrqCode,
        "Signature": app.build.import.file.signature,
        "Overwrite": overwrite,
        "LngIsoCode": app.label.language.iso.code
    };
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.Create",
        obj2send,
        "app.build.import.callback.createOnSuccess",
        null,
        "app.build.import.callback.createOnError",
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        }
    );
    // Add the progress bar
    api.spinner.progress.start(api.spinner.progress.getTimeout(app.build.import.file.content.Base64.length, app.config.transfer.unitsPerSecond["PxStat.Data.Matrix_API.Create"]));
};

/**
 * Create and upload px files
 * @param {*} data
 */
app.build.import.callback.createOnSuccess = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        app.build.import.reset();
        // No Duplicate found, upload completed
        api.modal.success(app.library.html.parseDynamicLabel("success-file-imported", [""]));
        app.build.import.setDatePicker();
    } else {
        // Duplicate found, prompt to complete
        api.modal.confirm(data, app.build.import.ajax.create, true);
    }
};

/**
 * Create and upload px files
 * @param {*} error
 */
app.build.import.callback.createOnError = function (error) {
    // Change button to Upload
    app.build.import.setValidateBtn();
    // Disable Validate Button
    $("#build-import-container").find("[name=validate]").prop("disabled", true);
};
//#endregion

//#region Read
/**
 * populate the preview modal
 */
app.build.import.preview = function () {
    api.spinner.start();
    $("#build-import-modal-preview").modal("show");
    api.content.load("#build-import-modal-preview .modal-body", "entity/build/import/index.preview.html");
    api.spinner.stop();
};

/**
 *Read content of file
 * @param {*} file
 * @param {*} inputObject
 */
api.plugin.dragndrop.readFiles = function (files, inputObject) {
    // Cancel upload first
    app.build.import.cancel();

    // Read single file only
    var file = files[0];
    if (!file) {
        return;
    }

    // set namespaced variables
    app.build.import.file.name = file.name;
    app.build.import.file.extension = file.name.match(/\.[0-9a-z]+$/i)[0];
    app.build.import.file.type = file.type;
    app.build.import.file.size = file.size;

    // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
    if ($.inArray(app.build.import.file.extension.toLowerCase(), C_APP_UPLOAD_FILE_ALLOWED_EXTENSION) == -1) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [app.build.import.file.extension]));
        // Disable Validate Button
        $("#build-import-container").find("[name=validate]").prop("disabled", true);
        // Disable Preview Button
        $("#build-import-container").find("[name=preview]").prop("disabled", true);
        // Reset type
        app.build.import.file.extension = null;
        return;
    }
    // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
    if ($.inArray(app.build.import.file.type.toLowerCase(), C_APP_UPLOAD_FILE_ALLOWED_TYPE) == -1) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [app.build.import.file.type]));
        // Disable Validate Button
        $("#build-import-container").find("[name=validate]").prop("disabled", true);
        // Disable Preview Button
        $("#build-import-container").find("[name=preview]").prop("disabled", true);
        return;
    }

    // Check for the hard limit of the file size
    if (app.build.import.file.size > app.config.transfer.threshold.hard) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.library.utility.formatNumber(Math.ceil(app.config.transfer.threshold.hard / 1024 / 1024)) + " MB"]));
        // Disable Validate Button
        $("#build-import-container").find("[name=validate]").prop("disabled", true);
        // Disable Preview Button
        $("#build-import-container").find("[name=preview]").prop("disabled", true);
        return;
    }
    // Info on screen 
    inputObject.parent().find("[name=upload-file-tip]").hide();
    inputObject.parent().find("[name=upload-file-name]").html(file.name + " (" + app.library.utility.formatNumber(Math.ceil(file.size / 1024)) + " KB)").show();

    // Read file into an UTF8 string
    var readerUTF8 = new FileReader();
    readerUTF8.onload = function (e) {
        // Set the file's content
        app.build.import.file.content.UTF8 = e.target.result;
    };
    readerUTF8.readAsText(file);
    readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
    readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
    readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
    readerUTF8.addEventListener("loadend", function (e) {
        // Validate the file
        app.build.import.validation.file();
        api.spinner.stop();
    });

    // Read file into a Base64 string
    var readerBase64 = new FileReader();
    readerBase64.onload = function (e) {
        // Set the file's content
        app.build.import.file.content.Base64 = e.target.result;
    };
    readerBase64.readAsDataURL(file);
    readerBase64.addEventListener("loadstart", function (e) { api.spinner.start(); });
    readerBase64.addEventListener("error", function (e) { api.spinner.stop(); });
    readerBase64.addEventListener("abort", function (e) { api.spinner.stop(); });
    readerBase64.addEventListener("loadend", function (e) {
        // Validate the file
        app.build.import.validation.file();
        api.spinner.stop();
    });

};

/**
 * Read data
 */
app.build.import.ajax.selectGroup = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Group_API.ReadAccess",
        { CcnUsername: null },
        "app.build.import.callback.selectGroup"
    );
};

/**
 * Set upload Select Group
 * @param {*} data
 */
app.build.import.callback.selectGroup = function (data) {
    // Load select2
    $("#build-import-container").find("[name=select-group]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.build.import.callback.mapData(data)
    });

    // Enable and Focus Search input
    $("#build-import-container").find("[name=select-group]").prop('disabled', false).focus();
};

/**
 * Create proper data source
 * @param {*} dataAPI
 */
app.build.import.callback.mapData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        dataAPI[i].id = item.GrpCode;
        dataAPI[i].text = item.GrpCode + " (" + item.GrpName + ")";
    });
    return dataAPI;
};

/**
 * Read Upload History
 * @param  {} dateFrom
 * @param  {} dateTo
  */
app.build.import.ajax.importHistory = function (dateFrom, dateTo) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.ReadHistory",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.build.import.callback.importHistory",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Draw Callback for Datatable
 */
app.build.import.drawCallbackimportHistory = function () {
    $('[data-toggle="tooltip"]').tooltip();
    //Edit link click
    $("#build-import-history table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at  "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
        $('.tooltip').remove();
        api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": $(this).attr("MtrCode"), "RlsCode": $(this).attr("idn") });
    });

    //view group info
    $("#build-import-history table").find("[name=grp-name]").once("click", function (e) {
        e.preventDefault();
        app.library.group.modal.read($(this).attr("idn"));
    });

    //view user info
    $("#build-import-history table").find("[name=user-name]").once("click", function (e) {
        e.preventDefault();
        app.library.user.modal.ajax.read({ CcnUsername: $(this).attr("idn") });
    });

}


/**
 * Draw Upload History
 * @param  {} data
  */
app.build.import.callback.importHistory = function (data) {
    if ($.fn.dataTable.isDataTable("#build-import-history table")) {
        app.library.datatable.reDraw("#build-import-history table", data);
    } else {
        var localOptions = {
            order: [[6, 'desc']],
            data: data,
            columns: [
                {
                    data: null,
                    render: function (_data, _type, row) {
                        return app.library.html.tooltip(row.MtrCode, row.MtrTitle);
                    }

                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ idn: row.RlsCode, MtrCode: row.MtrCode }, row.RlsVersion + "." + row.RlsRevision);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.release.renderStatus(row);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.release.renderRequest(row.RqsCode);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var linkGroup = $("<a>", {
                            idn: data.GrpCode,
                            name: "grp-name",
                            href: "#",
                            html: data.GrpCode
                        }).get(0).outerHTML;

                        return linkGroup;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var userLink = $("<a>", {
                            idn: data.CcnUsername,
                            name: "user-name",
                            "href": "#",
                            "html": data.CcnUsername
                        }).get(0).outerHTML;

                        return userLink;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.CreateDateTime ? moment(row.CreateDateTime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";

                    }
                }
            ],
            drawCallback: function (settings) {
                app.build.import.drawCallbackimportHistory();
            },

            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#build-import-history table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.import.drawCallbackimportHistory();
        });

    }
};
//#endregion