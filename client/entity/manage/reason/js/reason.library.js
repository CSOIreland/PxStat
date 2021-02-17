/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Create Namespace
// Create Namespace
app.reason = {};
app.reason.ajax = {};
app.reason.callback = {};
app.reason.modal = {};
app.reason.validation = {};

//#endregion

/*******************************************************************************
Functions 
*******************************************************************************/
//#region functions reason

/**
 * Draw extra information after you click on description link
 * @param  {*} data
 */
app.reason.drawExtraInformation = function (data) {
    //clone template from html not reuse dynamically
    var requestGrid = $("#reason-templates").find("[name=reason-div-description]").clone();
    requestGrid.removeAttr('id');
    requestGrid.find("[name=reason-div-description-int-desc]").empty().html(app.library.html.parseBbCode(data.RsnValueInternal));
    requestGrid.find("[name=reason-div-description-ext-desc]").empty().html(app.library.html.parseBbCode(data.RsnValueExternal));

    return requestGrid.show().get(0).outerHTML;
};

//#endregion

//#region Read reason

/**
 * Draw Callback for Datatable
 */
app.reason.drawCallback = function () {
    // Delete
    $("#reason-table-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.reason.modal.delete);
    // Update
    $("#reason-table-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        var idn = $(this).attr("idn");
        app.reason.ajax.readReason(idn);
    });

    // Subject must be first created in the default language
    if (app.label.language.iso.code == app.config.language.iso.code) {
        // Enable the button
        $("#reason-table-read-container").find("[name=button-create]").prop("disabled", false);
        // Hide warning
        $("#reason-table-read-container").find("[name=warning]").hide();
        // Create new subject button click event
        $("#reason-table-read-container").find("[name=button-create]").once("click", app.reason.modal.create);
    } else {
        // Disable the button
        $("#reason-table-read-container").find("[name=button-create]").prop("disabled", true);
        // Show warning
        $("#reason-table-read-container").find("[name=warning]").show();
    }
    // Extra Info
    app.library.datatable.showExtraInfo("#reason-table-read-container table", app.reason.drawExtraInformation);
}

/**
 * Create Reason DataTable and get JASON data
 * @param {*} data 
 */
app.reason.drawReasonDataTable = function (data) {
    if ($.fn.DataTable.isDataTable("#reason-table-read-container table")) {
        app.library.datatable.reDraw("#reason-table-read-container table", data);
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
                        return app.library.html.link.edit({ idn: row.RsnCode }, row.RsnCode);
                    }
                },
                {
                    data: null,
                    defaultContent: '',
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row, meta) {
                        return $("<a>", {
                            href: "#",
                            name: C_APP_DATATABLE_EXTRA_INFO_LINK,
                            "idn": meta.row,
                            html:
                                $("<i>", {
                                    "class": "fas fa-info-circle text-info"
                                }).get(0).outerHTML + " " + app.label.static.information
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: "RsnValueInternal",
                    "visible": false,
                    "searchable": true
                },
                {
                    data: "RsnValueExternal",
                    "visible": false,
                    "searchable": true
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.RsnCode }, false);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.reason.drawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#reason-table-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.reason.drawCallback();
        });
    }
};

/**
   * Get data from API and Draw the Data Table for Reason. Ajax call. 
   */
app.reason.ajax.read = function () {
    // Get data from API and Draw the Data Table for Reason 
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Reason_API.Read",
        {
            RsnCode: null,
            LngIsoCode: app.label.language.iso.code
        },
        "app.reason.callback.readOnSuccess",
        null,
        "app.reason.callback.readOnError",
        null);
};

/**
 * Callback function when the Reason Read call is successful.
 * @param {*} data
 */
app.reason.callback.readOnSuccess = function (data) {
    app.reason.drawReasonDataTable(data);
    $("modal-create-reason").modal("hide");
};

/**
 * Callback function when the Reason Read call is successful.
 * @param {*} error
 */
app.reason.callback.readOnError = function (error) {
    app.reason.drawReasonDataTable();
    $("#reason-modal-create").modal("hide");
};

//#endregion

//#region Delete reason
/**
 * Display Modal Delete Reason
 */
app.reason.modal.delete = function () {
    var idn = $(this).attr("idn");
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [idn]), app.reason.ajax.delete, idn);
};

/**
 * AJAX call to Delete a specific entry 
 * On AJAX success delete (Do reload table)
 * @param {*} idn
 */
app.reason.ajax.delete = function (idn) {
    // Get the indemnificator to delete
    var apiParams =
    {
        RsnCode: idn
    };
    // Call the API by passing the idn to delete reason from DB
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Reason_API.Delete",
        apiParams,
        "app.reason.callback.deleteOnSuccess",
        idn,
        "app.reason.callback.deleteOnError",
        null,
        { async: false }
    );
};

/**
* Callback from server for Delete Reason
* @param {*} data
* @param {*} idn
*/
app.reason.callback.deleteOnSuccess = function (data, idn) {
    //Redraw Data Table Reason with fresh data.
    app.reason.ajax.read();

    if (data == C_API_AJAX_SUCCESS) {
        // Display Success Modal
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [idn]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* Callback from server for Delete Reason
* @param {*} error
*/
app.reason.callback.deleteOnError = function (error) {
    //Redraw Data Table Reason with fresh data.
    app.reason.ajax.read();
};

//#endregion

//#region Create reason
/**
 * Display Modal Create
 */
app.reason.modal.create = function () {
    app.reason.validation.create();
    // MUST be specific ID !!!
    var tinyMceIntIdInternal = $("#reason-modal-create").find("[name=reason-modal-create-rsn-value-internal]").attr("id");
    tinymce.get(tinyMceIntIdInternal).setContent("");
    var tinyMceExtIdExternal = $("#reason-modal-create").find("[name=reason-modal-create-rsn-value-external]").attr("id");
    tinymce.get(tinyMceExtIdExternal).setContent("");
    $("#reason-modal-create").modal("show");
};

/**
 * Create Reason Callback
 */
app.reason.ajax.create = function () {
    var rsncodecreate = $("#reason-modal-create").find("[name=rsn-code]").val();
    var rsntextintcreate = $("#reason-modal-create").find("[name=reason-modal-create-rsn-value-internal]").val();
    var rsntextextcreate = $("#reason-modal-create").find("[name=reason-modal-create-rsn-value-external]").val();
    var apiParams = {
        RsnCode: rsncodecreate,
        RsnValueInternal: rsntextintcreate,
        RsnValueExternal: rsntextextcreate,
    };
    var callbackParam = {
        RsnCode: rsncodecreate,
    };
    // CAll Ajax to Create Reason. Do Redraw Data Table for Create Reason.
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Reason_API.Create",
        apiParams,
        "app.reason.callback.createOnSuccess",
        callbackParam,
        "app.reason.callback.createOnError",
        null,
        { async: false }
    );
};

/**
* Create Reason to Table after Ajax success call
* @param {*} data
* @param {*} callbackParam
*/
app.reason.callback.createOnSuccess = function (data, callbackParam) {
    //Redraw Data Table for Create Reason
    app.reason.ajax.read();
    $("#reason-modal-create").modal("hide");

    if (data == C_API_AJAX_SUCCESS) {
        //Close modal
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.RsnCode]));
    } else
        api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* Create Reason to Table after Ajax success call
* @param {*} error
* @param {*} callbackParam
*/
app.reason.callback.createOnError = function (error) {
    //Redraw Data Table for Create Reason
    app.reason.ajax.read();
    $("#reason-modal-create").modal("hide");

};

//#endregion

//#region Update reason

/**
 * Ajax call read data for Reason
 * @param {*} idn 
 */
app.reason.ajax.readReason = function (idn) {
    // Get data from API and Draw the Data Table for Reason. Populate date to the modal "reason-modal-update"
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Reason_API.Read",
        {
            RsnCode: idn,
            LngIsoCode: app.label.language.iso.code
        },
        "app.reason.callback.readReason");
};

/**
* Populate Reason data to Update Modal (reason-modal-update)
* @param {*} data 
*/
app.reason.callback.readReason = function (data) {
    if (data && Array.isArray(data) && data.length) {
        data = data[0];

        app.reason.modal.updateReason(data);
    }
    // Handle no data
    else {
        api.modal.information(app.label.static["api-ajax-nodata"]);
        // Force reload
        app.reason.ajax.read();
    }
};

/**
* Modal Update Reason
* @param {*} reasonRecord 
*/
app.reason.modal.updateReason = function (reasonRecord) {
    //Add validation and reset form
    app.reason.validation.update();
    //Get fields values at reason-modal-update Modal

    $("#reason-modal-update").find("[name=rsn-code]").text(reasonRecord.RsnCode);
    // MUST be ID !!!
    var tinyMceIntId = $("#reason-modal-update").find("[name=reason-modal-update-rsn-value-internal]").attr("id");
    var tinyMceExtId = $("#reason-modal-update").find("[name=reason-modal-update-rsn-value-external]").attr("id");
    tinymce.get(tinyMceIntId).setContent(reasonRecord.RsnValueInternal);
    tinymce.get(tinyMceExtId).setContent(reasonRecord.RsnValueExternal);
    //Show Update Modal
    $("#reason-modal-update").modal('show');
};

/**
* Ajax call Update Reason
* Save updated Reason via AJAX call.
*/
app.reason.ajax.update = function () {
    //Get fields values at reason-modal-update Modal
    var rsncodeupdate = $("#reason-modal-update").find("[name=rsn-code]").text();
    var rsntextintupdate = $("#reason-modal-update").find("[name=reason-modal-update-rsn-value-internal]").val();
    var rsntextextupdate = $("#reason-modal-update").find("[name=reason-modal-update-rsn-value-external]").val();
    var apiParams = {
        RsnCode: rsncodeupdate,
        RsnValueInternal: rsntextintupdate,
        RsnValueExternal: rsntextextupdate,
        LngIsoCode: app.label.language.iso.code
    };
    var callbackParam = {
        RsnCode: rsncodeupdate,
    };
    // CAll Ajax to Update the Reason. Get the fresh new data. Redraw table
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Reason_API.Update",
        apiParams,
        "app.reason.callback.updateOnSuccess",
        callbackParam,
        "app.reason.callback.updateOnError",
        null,
        { async: false }
    );
};

/**
 * Call back after update complete
 * @param  {*} data
 * @param  {*} callbackParam 
 */
app.reason.callback.updateOnSuccess = function (data, callbackParam) {
    app.reason.ajax.read();
    $("#reason-modal-update").modal("hide");

    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.RsnCode]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

/**
 * Call back after update complete
 * @param  {*} error
 */
app.reason.callback.updateOnError = function (error) {
    app.reason.ajax.read();
    $("#reason-modal-update").modal("hide");
};

//#endregion

//#region validation
/**
 * Validate Create Reason - Modal - Create Reason
 */
app.reason.validation.create = function () {
    //Sanitizing input GrpCode (allUppper, allLower, onlyAlpha, onlyNum)
    $("#reason-modal-create").find("form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        ignore: [],
        rules: {
            "rsn-code":
            {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(C_SANITISE_UPPERCASE, C_APP_REGEX_ALPHANUMERIC);
                    $(this).val(value);
                    return value;
                }
            },
            "reason-modal-create-rsn-value-internal":
            {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            },
            "reason-modal-create-rsn-value-external":
            {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            },
        },
        errorPlacement: function (error, element) {
            $("#reason-modal-create").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.reason.ajax.create();
        }
    }).resetForm();
};

/**
 * Validate Update Reason - Modal Update Reason
 */
app.reason.validation.update = function () {
    $("#reason-modal-update").find("form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        ignore: [],
        rules: {
            "rsn-code":
            {
                required: true
            },
            "reason-modal-update-rsn-value-internal":
            {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            },
            "reason-modal-update-rsn-value-external":
            {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            },
        },
        errorPlacement: function (error, element) {
            $("#reason-modal-update").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.reason.ajax.update();
        }
    }).resetForm();
};
//#endregion