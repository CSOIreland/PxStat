/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Create Namespace
app.theme = {};
app.theme.ajax = {};
app.theme.callback = {};
app.theme.modal = {};
app.theme.validation = {};
//#endregion

//#region Read theme

/**
 * Ajax read call
 */
app.theme.ajax.read = function () {
    // Change app.config.language.iso.code to the selected one
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Theme_API.Read",
        {
            SbjCode: null,
            LngIsoCode: app.label.language.iso.code
        },
        "app.theme.callback.read");
};

/**
 * * Callback theme read
 * @param  {} data
 */
app.theme.callback.read = function (data) {
    app.theme.drawDataTable(data);
};

/**
 * Draw Callback for Datatable
 */
app.theme.drawCallback = function () {
    $('[data-toggle="tooltip"]').tooltip();
    // click event update
    $("#theme-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        var idn = $(this).attr("idn");
        app.theme.ajax.readUpdate(idn);
    });
    // click event delete
    $("#theme-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        app.theme.modal.delete($(this).attr("idn"), $(this).attr("thm-value"));
    });
}

/**
 * Draw table
 * @param {*} data
 */
app.theme.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#theme-read-container table")) {
        app.library.datatable.reDraw("#theme-read-container table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (_data, _type, row) {
                        return app.library.html.link.edit({ "idn": row.ThmCode, "thm-value": row.ThmValue }, row.ThmValue);
                    }
                },
                { data: "SbjCount" },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        var deleteButton = app.library.html.deleteButton({ "idn": row.ThmCode, "thm-value": row.ThmValue }, false);
                        if (row.SbjCount > 0) {
                            deleteButton = app.library.html.deleteButton({ "idn": row.ThmCode, "thm-value": row.ThmValue }, true);
                        }
                        return deleteButton;
                    },
                    "width": "1%"
                }
            ],
            drawCallback: function (settings) {
                app.theme.drawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        // theme must be first created in the default language
        if (app.label.language.iso.code == app.config.language.iso.code) {
            // Enable the button
            $("#theme-read-container").find("[name=button-create]").prop("disabled", false);
            // Hide warning
            $("#theme-read-container").find("[name=warning]").hide();
            // Create new theme button click event
            $("#theme-read-container").find("[name=button-create]").once("click", function () {
                app.theme.modal.create();
            });
        } else {
            // Disable the button
            $("#theme-read-container").find("[name=button-create]").prop("disabled", true);
            // Show warning
            $("#theme-read-container").find("[name=warning]").show();
        }

        $("#theme-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.theme.drawCallback();
        });

    }
};

//#endregion

//#region Create theme

/**
 * Show create new theme modal
 * */
app.theme.modal.create = function () {
    //validate Create theme form
    app.theme.validation.create();
    $("#theme-modal-create").modal("show");
};

/**
 * Validation for create new theme
 * */
app.theme.validation.create = function () {
    //allUppper, allLower, onlyAlpha, onlyNum
    $("#theme-modal-create form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "thm-value": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#theme-modal-create [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.theme.ajax.create();
        }
    }).resetForm();
};

/**
 * Ajax for create new theme
 */
app.theme.ajax.create = function () {
    $("#theme-modal-create").modal("hide");
    // A theme is created always against the default Language
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Theme_API.Create",
        {
            ThmValue: $("#theme-modal-create").find("[name=thm-value]").val()
        },
        "app.theme.callback.createOnSuccess",
        $("#theme-modal-create").find("[name=thm-value]").val(),
        "app.theme.callback.createOnError",
        null,
        { async: false }
    );
};

/**
 * callback for create new theme
 * @param  {} data
 * @param  {} callbackParam
 */
app.theme.callback.createOnSuccess = function (data, callbackParam) {
    //Refresh the table
    app.theme.ajax.read();

    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * callback for create new theme
 * @param  {} error
 */
app.theme.callback.createOnError = function (error) {
    //Refresh the table
    app.theme.ajax.read();
};

//#endregion

//#region Update theme
/**
 * get Update details from ajax
 * @param  {} idn 
 */
app.theme.ajax.readUpdate = function (idn) {
    // Change app.config.language.iso.code to the selected one
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Theme_API.Read",
        {
            ThmCode: idn,
            LngIsoCode: app.label.language.iso.code
        },
        "app.theme.callback.readUpdate");
};

/**
 * Show modal after ajax call
 * @param  {} data
 */
app.theme.callback.readUpdate = function (data) {
    if (data && Array.isArray(data) && data.length) {
        data = data[0];
        //validate Update theme form
        app.theme.validation.update();
        //Display of Modal update
        app.theme.modal.update(data);
    } else {
        api.modal.information(app.label.static["api-ajax-nodata"]);
        // Force reload
        app.theme.ajax.read();
    }
};

/**
 * Display of Modal update
 * @param {*} themeRecord
 */
app.theme.modal.update = function (themeRecord) {
    $("#theme-modal-update").find("[name='idn']").val(themeRecord.ThmCode);
    $("#theme-modal-update").find("[name=thm-value]").val(themeRecord.ThmValue);
    $("#theme-modal-update").modal("show");
};

/**
 * Validation of update
 */
app.theme.validation.update = function () {
    $("#theme-modal-update form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "thm-value": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#theme-modal-update [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.theme.ajax.update();
        }
    }).resetForm();
};

/**
 * ajax to update
 */
app.theme.ajax.update = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Theme_API.Update",
        {
            "ThmCode": $("#theme-modal-update").find("[name=idn]").val(),
            "ThmValue": $("#theme-modal-update").find("[name=thm-value]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.theme.callback.updateOnSuccess",
        $("#theme-modal-update").find("[name=thm-value]").val(),
        "app.theme.callback.updateOnError",
        null,
        { async: false }
    );
};

/**
 * callback after ajax update
 * @param  {} data
 * @param  {} callbackParam
 */
app.theme.callback.updateOnSuccess = function (data, callbackParam) {
    $("#theme-modal-update").modal("hide");
    //Refresh the table
    app.theme.ajax.read();

    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

/**
 * callback after ajax update
 * @param  {} error
 */
app.theme.callback.updateOnError = function (error) {
    $("#theme-modal-update").modal("hide");
    //Refresh the table
    app.theme.ajax.read();
};

//#endregion

//#region Delete theme

/**
 * Modal to confirm delete
 * @param  {} idn
 * @param  {} SbjValue
 */
app.theme.modal.delete = function (idn, ThmValue) {
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [ThmValue]), app.theme.ajax.delete, {
        idn: idn,
        ThmValue: ThmValue
    });
};

/**
 * Ajax to delete
 * @param  {} objToSend 
 */
app.theme.ajax.delete = function (params) {
    // A theme is deleted always against the default Language
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Theme_API.Delete",
        {
            "ThmCode": params.idn
        },
        "app.theme.callback.deleteOnSuccess",
        params.ThmValue,
        "app.theme.callback.deleteOnError",
        null,
        { async: false }
    );
};

/**
 * Callback after delete
 * @param  {} data
 * @param  {} callbackParam
 */
app.theme.callback.deleteOnSuccess = function (data, callbackParam) {
    //Refresh the table
    app.theme.ajax.read();

    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [callbackParam]));
    } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Callback after delete
 * @param  {} error
 */
app.theme.callback.deleteOnError = function (error) {
    //Refresh the table
    app.theme.ajax.read();
};
//#endregion
