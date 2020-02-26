/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
// Methods
app.build = app.build || {};
app.build.upload = {};
app.build.upload.ajax = {};
app.build.upload.callback = {};
app.build.upload.validation = {};
app.build.upload.FrqValue = null;
app.build.upload.FrqCode = null;

// Properties
app.build.upload.file = {};
app.build.upload.file.signature = null;
app.build.upload.file.name = null;
app.build.upload.file.extension = null;
app.build.upload.file.type = null;
app.build.upload.file.size = null;
app.build.upload.file.content = {};
app.build.upload.file.content.UTF8 = null;
app.build.upload.file.content.Base64 = null;
//#endregion

//#region Miscellaneous
/**
 * Cancel the Upload only
 */
app.build.upload.cancel = function () {
    // Change button to Validate
    app.build.upload.setValidateBtn();
    // Clear file preview
    $("#build-upload-container").find("[name=upload-file-preview]").empty();
    $("#build-upload-container").find("[name=upload-file-preview-card]").hide();
    // Clear file errors
    $("#build-upload-container").find("[name=upload-select-group-error-holder]").empty();
    // Clear file details
    $("#build-upload-container").find("[name=upload-file-name]").empty().hide();
    $("#build-upload-container").find("[name=upload-file-tip]").show();
    $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", true);
    $("#build-upload-container").find("[name=upload-btn-preview]").prop("disabled", true);
    // Invalidate frequency
    app.build.upload.FrqValue = null;
    app.build.upload.FrqCode = null;
    // Invalidate data upload
    app.build.upload.file.signature = null;
    app.build.upload.file.name = null;
    app.build.upload.file.extension = null;
    app.build.upload.file.type = null;
    app.build.upload.file.size = null;
    app.build.upload.file.content.UTF8 = null;
    app.build.upload.file.content.Base64 = null;
};

/**
 * Reset entire upload page
 */
app.build.upload.reset = function () {
    // Clear Group selection
    $("#build-upload-container").find("[name=upload-select-group]").val("").trigger('change');

    // Clear input file 
    $("#build-upload-container").find("[name=build-upload-file]").val("");
    // Clear Upload
    app.build.upload.cancel();
};

/**
 * Change to upload button
 */
app.build.upload.setUploadBtn = function () {
    $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", true);
    $("#build-upload-container").find("[name=upload-btn-upload]").prop("disabled", false);
};

/**
 * Change to validate button
 */
app.build.upload.setValidateBtn = function () {
    // Enable Validate Button
    $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", false);
    $("#build-upload-container").find("[name=upload-btn-upload]").prop("disabled", true);
};

/**
 * Set up date picker
 */
app.build.upload.setDatePicker = function () {
    var start = moment();
    var end = moment();
    app.build.upload.ajax.uploadHistory(start, end);
    $('#build-upload-uploadHistory-date-picker span').html(start.format(app.config.mask.date.display) + " - " + end.format(app.config.mask.date.display));

    $('#build-upload-uploadHistory-date-picker').daterangepicker({
        maxDate: new Date(),
        ranges: {
            [app.label.static["last-30-days"]]: [moment().subtract(29, 'days'), moment()],
            [app.label.static["last-7-days"]]: [moment().subtract(6, 'days'), moment()]
        },
        locale: app.label.plugin.daterangepicker
    }, function (start, end) {
        $('#build-upload-uploadHistory-date-picker span').html(start.format(app.config.mask.date.display) + " - " + end.format(app.config.mask.date.display));
        app.build.upload.ajax.uploadHistory(start, end);
    });
};


//#endregion

//#region Create
/**
 * Custom validation for Files
 */
app.build.upload.validation.file = function () {
    if (!app.build.upload.file.content.UTF8 || !app.build.upload.file.content.Base64) {
        $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", true);
        $("#build-upload-container").find("[name=upload-btn-preview]").prop("disabled", true);
        return false;
    } else {
        $("#build-upload-container").find("[name=upload-btn-preview]").prop("disabled", false);
        $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", false);
        return true;
    }
};

/**
 * Bind validation for the upload
 */
app.build.upload.validation.create = function () {
    $("#build-upload-container form").trigger("reset").validate({
        rules: {
            "upload-select-group": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#build-upload-container [name=" + element[0].name + "-error-holder]").append(error[0]);
            app.build.upload.validation.file();
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            if (!app.build.upload.validation.file()) {
                return;
            }
            if (app.build.upload.file.signature) {
                app.build.upload.ajax.create();
            }
            else {
                app.build.upload.ajax.validate();
            }
        }
    }).resetForm();
};

/**
 * Bind validation for frequency validation
 */
app.build.upload.validation.frequency = function () {
    $("#build-upload-modal-frequency form").trigger("reset").validate({
        rules: {
            "frq-value": {
                required: true
            },
            "frq-code": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#build-upload-modal-frequency [name=" + element[0].name + "-error-holder]").append(error[0]);
            app.build.upload.validation.file();
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            // Store for later use
            app.build.upload.FrqValue = $("#build-upload-modal-frequency").find("[name=frq-value]:checked").val();
            app.build.upload.FrqCode = $("#build-upload-modal-frequency").find("[name=frq-code]").val();
            app.build.upload.ajax.validate();
        }
    }).resetForm();
};

/**
 * Call to validate upload
 */
app.build.upload.ajax.validate = function () {
    var obj2send = {
        "MtrInput": app.build.upload.file.content.Base64,
        "GrpCode": $("#build-upload-container").find("[name=upload-select-group]").val(),
        "FrqCodeTimeval": app.build.upload.FrqCode,
        "FrqValueTimeval": app.build.upload.FrqValue
    };
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Matrix_API.Validate",
        obj2send,
        "app.build.upload.callback.validateOnSuccess",
        null,
        "app.build.upload.callback.validateOnError",
        null,
        {
            async: false,
            timeout: app.config.upload.timeout
        }
    );

    // Add the progress bar
    api.spinner.progress.start(api.spinner.progress.getTimeout(app.build.upload.file.content.Base64.length, app.config.upload.unitsPerSecond.validate));
};

/**
 * Validate and upload px files
 * @param {*} data
 */
app.build.upload.callback.validateOnSuccess = function (data) {
    // Hide Frequency modal anyway
    $("#build-upload-modal-frequency").modal("hide");
    if (data) {
        // Check for signature
        if (data.Signature) {
            app.build.upload.file.signature = data.Signature;
            // Change button to Upload
            app.build.upload.setUploadBtn();
            api.modal.success(app.library.html.parseDynamicLabel("success-file-validated", [app.build.upload.file.name]));
        } else if (Array.isArray(data.FrqValueCandidate) && data.FrqValueCandidate.length) {
            // Populate the Frequency list
            $("#build-upload-modal-frequency").find("[name=frequency-radio-group]").empty();

            $.each(data.FrqValueCandidate, function (key, value) {
                $("#build-upload-modal-frequency").find("[name=frequency-radio-group]").append(function () {
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
            $("#build-upload-modal-frequency").modal("show");

        } else api.modal.exception(app.label.static["api-ajax-exception"]);
    } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Validate and upload px files
 * @param {*} error
 */
app.build.upload.callback.validateOnError = function (error) {
    // Hide Frequency modal anyway
    $("#build-upload-modal-frequency").modal("hide");

    // Disable Validate Button
    $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", true);
};

/**
 * Call to get frequency codes
 * 
 */

app.build.upload.ajax.getFrequencyCodes = function () {
    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.System.Settings.Frequency_API.Read",
        null,
        "app.build.upload.callback.getFrequencyCodes");
}

/**
 * Call to get frequency codes
 * 
 */

app.build.upload.callback.getFrequencyCodes = function (data) {
    if (data && Array.isArray(data) && data.length) {
        // Map API data to select dropdown  model for main Subject search and update Subject search
        $("#build-upload-modal-frequency").find("[name=frq-code]").empty().append($("<option>", {
            "text": app.label.static["select-uppercase"],
            "disabled": "disabled",
            "selected": "selected"
        }));

        $.each(data, function (key, value) {
            $("#build-upload-modal-frequency").find("[name=frq-code]").append($("<option>", {
                "value": value.FrqCode,
                "text": value.FrqCode + " - " + app.label.static[value.FrqValue]
            }));
        });

        $("#build-upload-modal-frequency").find("[name=frq-code]").prop('disabled', false);
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
}

/**
 * Call to create upload
 */
app.build.upload.ajax.create = function (overwrite) {
    // Default params
    overwrite = overwrite || false;
    var obj2send = {
        "MtrInput": app.build.upload.file.content.Base64,
        "GrpCode": $("#build-upload-container").find("[name=upload-select-group]").val(),
        "FrqValueTimeval": app.build.upload.FrqValue,
        "FrqCodeTimeval": app.build.upload.FrqCode,
        "Signature": app.build.upload.file.signature,
        "Overwrite": overwrite
    };
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Matrix_API.Create",
        obj2send,
        "app.build.upload.callback.createOnSuccess",
        null,
        "app.build.upload.callback.createOnError",
        null,
        {
            async: false,
            timeout: app.config.upload.timeout
        }
    );
    // Add the progress bar
    api.spinner.progress.start(api.spinner.progress.getTimeout(app.build.upload.file.content.Base64.length, app.config.upload.unitsPerSecond.upload));
};

/**
 * Create and upload px files
 * @param {*} data
 */
app.build.upload.callback.createOnSuccess = function (data) {
    if (data == C_APP_API_SUCCESS) {
        app.build.upload.reset();
        // No Duplicate found, upload completed
        api.modal.success(app.library.html.parseDynamicLabel("success-file-uploaded", [""]));
        app.build.upload.setDatePicker();
    } else {
        // Duplicate found, prompt to complete
        api.modal.confirm(data, app.build.upload.ajax.create, true);
    }
};

/**
 * Create and upload px files
 * @param {*} error
 */
app.build.upload.callback.createOnError = function (error) {
    // Change button to Upload
    app.build.upload.setValidateBtn();
    // Disable Validate Button
    $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", true);
};
//#endregion

//#region Read
/**
 * populate the preview modal
 */
app.build.upload.preview = function () {
    api.spinner.start();
    $("#build-upload-modal-preview").modal("show");
    api.content.load("#build-upload-modal-preview .modal-body", "entity/build/upload/index.preview.html");
    api.spinner.stop();
};

/**
 *Read content of file
 * @param {*} file
 * @param {*} inputObject
 */
api.plugin.dragndrop.readFiles = function (files, inputObject) {
    // Cancel upload first
    app.build.upload.cancel();

    // Read single file only
    var file = files[0];
    if (!file) {
        return;
    }

    // set namespaced variables
    app.build.upload.file.name = file.name;
    app.build.upload.file.extension = file.name.match(/\.[0-9a-z]+$/i)[0];
    app.build.upload.file.type = file.type;
    app.build.upload.file.size = file.size;

    // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
    if ($.inArray(app.build.upload.file.extension.toLowerCase(), C_APP_UPLOAD_FILE_ALLOWED_EXTENSION) == -1) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [app.build.upload.file.extension]));
        // Disable Validate Button
        $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", true);
        // Disable Preview Button
        $("#build-upload-container").find("[name=upload-btn-preview]").prop("disabled", true);
        // Reset type
        app.build.upload.file.extension = null;
        return;
    }
    // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
    if ($.inArray(app.build.upload.file.type.toLowerCase(), C_APP_UPLOAD_FILE_ALLOWED_TYPE) == -1) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [app.build.upload.file.type]));
        // Disable Validate Button
        $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", true);
        // Disable Preview Button
        $("#build-upload-container").find("[name=upload-btn-preview]").prop("disabled", true);
        return;
    }

    // Check for the hard limit of the file size
    if (app.build.upload.file.size > app.config.upload.threshold.hard) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.library.utility.formatNumber(Math.ceil(app.config.upload.threshold.hard / 1024 / 1024)) + " MB"]));
        // Disable Validate Button
        $("#build-upload-container").find("[name=upload-btn-validate]").prop("disabled", true);
        // Disable Preview Button
        $("#build-upload-container").find("[name=upload-btn-preview]").prop("disabled", true);
        return;
    }
    // Info on screen 
    inputObject.parent().find("[name=upload-file-tip]").hide();
    inputObject.parent().find("[name=upload-file-name]").html(file.name + " (" + app.library.utility.formatNumber(Math.ceil(file.size / 1024)) + " KB)").show();

    // Read file into an UTF8 string
    var readerUTF8 = new FileReader();
    readerUTF8.onload = function (e) {
        // Set the file's content
        app.build.upload.file.content.UTF8 = e.target.result;
    };
    readerUTF8.readAsText(file);
    readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
    readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
    readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
    readerUTF8.addEventListener("loadend", function (e) {
        // Validate the file
        app.build.upload.validation.file();
        api.spinner.stop();
    });

    // Read file into a Base64 string
    var readerBase64 = new FileReader();
    readerBase64.onload = function (e) {
        // Set the file's content
        app.build.upload.file.content.Base64 = e.target.result;
    };
    readerBase64.readAsDataURL(file);
    readerBase64.addEventListener("loadstart", function (e) { api.spinner.start(); });
    readerBase64.addEventListener("error", function (e) { api.spinner.stop(); });
    readerBase64.addEventListener("abort", function (e) { api.spinner.stop(); });
    readerBase64.addEventListener("loadend", function (e) {
        // Validate the file
        app.build.upload.validation.file();
        api.spinner.stop();
    });

};

/**
 * Read data
 */
app.build.upload.ajax.selectGroup = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Security.Group_API.ReadAccess",
        { CcnUsername: null },
        "app.build.upload.callback.selectGroup"
    );
};

/**
 * Set upload Select Group
 * @param {*} data
 */
app.build.upload.callback.selectGroup = function (data) {
    // Load select2
    $("#build-upload-container").find("[name=upload-select-group]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.build.upload.callback.mapData(data)
    });

    // Enable and Focus Search input
    $("#build-upload-container").find("[name=upload-select-group]").prop('disabled', false).focus();
};

/**
 * Create proper data source
 * @param {*} dataAPI
 */
app.build.upload.callback.mapData = function (dataAPI) {
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
app.build.upload.ajax.uploadHistory = function (dateFrom, dateTo) {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Matrix_API.ReadHistory",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.build.upload.callback.uploadHistory",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Draw Callback for Datatable
 */
app.build.upload.drawCallbackUploadHistory = function () {
    $('[data-toggle="tooltip"]').tooltip();
    //Edit link click
    $("#build-upload-history table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at  "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
        $('.tooltip').remove();
        api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": $(this).attr("MtrCode"), "RlsCode": $(this).attr("idn") });
    });

    //view group info
    $("#build-upload-history table").find("[name=grp-name]").once("click", function (e) {
        e.preventDefault();
        app.library.group.modal.read($(this).attr("idn"));
    });

    //view user info
    $("#build-upload-history table").find("[name=user-name]").once("click", function (e) {
        e.preventDefault();
        app.library.user.modal.read({ CcnUsername: $(this).attr("idn") });
    });

}


/**
 * Draw Upload History
 * @param  {} data
  */
app.build.upload.callback.uploadHistory = function (data) {
    if ($.fn.dataTable.isDataTable("#build-upload-history table")) {
        app.library.datatable.reDraw("#build-upload-history table", data);
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
                app.build.upload.drawCallbackUploadHistory();
            },

            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#build-upload-history table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.upload.drawCallbackUploadHistory();
        });

    }
};
//#endregion