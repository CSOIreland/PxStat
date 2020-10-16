/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.release = app.release || {};

app.release.workflow.history = {};
app.release.workflow.history.render = {};
app.release.workflow.history.ajax = {};
app.release.workflow.history.callback = {};
//#endregion

app.release.workflow.history.read = function () {
    app.release.workflow.history.ajax.read();
};

//#region Ajax/Callback

/**
 * 
 */
app.release.workflow.history.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadHistory",
        { RlsCode: app.release.RlsCode },
        "app.release.workflow.history.callback.read",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} data
 */
app.release.workflow.history.callback.read = function (data) {
    app.release.workflow.history.callback.drawDataTable(data);
};

/**
 * Draw Callback for Datatable
 */
app.release.workflow.history.drawCallback = function () {
    $('[data-toggle="tooltip"]').tooltip();
    $("td.details-request-control i.fa.plus-control").css({ "color": "forestgreen" });
    app.library.datatable.showExtraInfo('#release-workflow-history table', app.release.workflow.history.render.extraInfo);

    //Delete Request button click event. Passing function reference.
    $("#release-workflow-history table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.release.workflow.history.delete);
    // Bootstrap tooltip
    $('[data-toggle="tooltip"]').tooltip();
}


/**
* 
 * @param {*} data
 */
app.release.workflow.history.callback.drawDataTable = function (data) {
    $("#release-workflow-history").hide().fadeIn();
    if ($.fn.dataTable.isDataTable("#release-workflow-history table")) {
        app.library.datatable.reDraw("#release-workflow-history table", data);
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
                    visible: true,
                    render: function (data, type, row) {
                        return app.label.datamodel.request[row.RqsValue];
                    }
                },
                {
                    data: "WrqDtgCreateDatetime",
                    visible: true,
                    render: function (data, type, row) {
                        return row.WrqDtgCreateDatetime ? moment(row.WrqDtgCreateDatetime).format(app.config.mask.datetime.display) : "";
                    }
                },
                {
                    data: "WrqDtgCreateCcnUsername",
                    visible: false
                },
                {
                    data: null,
                    defaultContent: '',
                    sorting: false,
                    searchable: false,
                    "render": function (data, type, row, meta) {
                        return $("<a>", {
                            href: "#",
                            name: C_APP_DATATABLE_EXTRA_INFO_LINK,
                            "idn": meta.row,
                            html:
                                $("<i>", {
                                    "class": "fas fa-info-circle text-info"
                                }).get(0).outerHTML +
                                " " + app.label.static["details"]
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: "WrqDatetime",
                    visible: false,
                    render: function (data, type, row) {
                        return row.WrqDatetime ? moment(row.WrqDatetime).format(app.config.mask.datetime.display) : "";
                    }
                },
                {
                    data: "WrqCmmValue",
                    visible: false,
                },
                {
                    data: null,
                    defaultContent: '',
                    type: "natural",
                    "render": function (data, type, row) {
                        return app.release.workflow.history.render.reply(row.RspCode, row.RspValue);
                    },
                },
                {
                    data: "WrsCmmValue",
                    visible: false
                },
                {
                    data: "WrsDtgCreateCcnUsername",
                    visible: false
                },
                {
                    data: "WrsDtgCreateDatetime",
                    visible: false,
                    render: function (data, type, row) {
                        return row.WrsDtgCreateDatetime ? moment(row.WrsDtgCreateDatetime).format(app.config.mask.datetime.display) : "";
                    }
                },
                {
                    data: null,
                    defaultContent: '',
                    type: "natural",
                    "render": function (data, type, row) {
                        return app.release.workflow.history.render.reply(row.SgnCode, row.SgnValue);
                    },
                },
                {
                    data: "WsgDtgCreateCcnUsername",
                    visible: false
                },
                {
                    data: "WsgDtgCreateDatetime",
                    visible: false,
                    render: function (data, type, row) {
                        return row.WsgDtgCreateDatetime ? row.WsgDtgCreateDatetime : "";
                    }
                },
                {
                    data: "WsgCmmValue",
                    visible: false
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.RlsCode }, row.RspCode ? true : false);
                    },
                    "width": "1%"
                }
            ],
            drawCallback: function (settings) {
                app.release.workflow.history.drawCallback();

            },
            //Translate labels language
            language: app.label.plugin.datatable,
            "order": [[1, "desc"]]
        };
        $("#release-workflow-history table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.workflow.history.drawCallback();
        });
    }
};
//#endregion

//#region Render
/**
 * Generate an HTML workflow replay (Response or Publish)
 * @param {*} requestType
 * @param {*} textTooltip
 */
app.release.workflow.history.render.reply = function (requestType, textTooltip) {

    switch (requestType) {
        case C_APP_TS_RESPONSE_APPROVED:
            return $("<a>", {
                "data-toggle": "tooltip",
                "data-original-title": textTooltip ? app.label.datamodel.response[textTooltip] : "", //textTooltip,
                html:
                    $("<i>", {
                        class: "fas fa-check-circle text-success"
                    }).get(0).outerHTML
            }).get(0).outerHTML;
            break;
        case C_APP_TS_SIGNOFF_APPROVED:
            return $("<a>", {
                "data-toggle": "tooltip",
                "data-original-title": textTooltip ? app.label.datamodel.signoff[textTooltip] : "", //textTooltip,
                html:
                    $("<i>", {
                        class: "fas fa-check-circle text-success"
                    }).get(0).outerHTML
            }).get(0).outerHTML;
            break;
        case C_APP_TS_RESPONSE_REJECTED:
            return $("<a>", {
                "data-toggle": "tooltip",
                "data-original-title": textTooltip ? app.label.datamodel.response[textTooltip] : "", //textTooltip,
                html:
                    $("<i>", {
                        class: "fas fa-times-circle text-danger"
                    }).get(0).outerHTML
            }).get(0).outerHTML;
            break;
        case C_APP_TS_SIGNOFF_REJECTED:
            return $("<a>", {
                "data-toggle": "tooltip",
                "data-original-title": textTooltip ? app.label.datamodel.signoff[textTooltip] : "", //textTooltip,
                html:
                    $("<i>", {
                        class: "fas fa-times-circle text-danger"
                    }).get(0).outerHTML
            }).get(0).outerHTML;
            break;

        default:
            return $("<a>", {
                "data-toggle": "tooltip",
                "data-original-title": app.label.static["pending"], //textTooltip,
                html:
                    $("<i>", {
                        class: "fas fa-question-circle text-info"
                    }).get(0).outerHTML
            }).get(0).outerHTML;
            break;
    }
};

/**
* 
 * @param {*} row
 */
app.release.workflow.history.render.extraInfo = function (row) {
    //clone template from html not reuse dynamically
    var grid = $("#release-workflow-history-extra-info").clone();
    // Request
    grid.find("[name=rqs-value]").empty().html(app.label.datamodel.request[row.RqsValue]); //Translation
    grid.find("[name=wrq-exceptional-flag]").empty().html(app.library.html.boolean(row.WrqExceptionalFlag, true, true));
    grid.find("[name=wrq-datetime]").empty().html(row.WrqDatetime ? moment(row.WrqDatetime).format(app.config.mask.datetime.display) : "");
    grid.find("[name=wrq-reservation-flag]").empty().html(app.library.html.boolean(row.WrqReservationFlag, true, true));
    grid.find("[name=wrq-archive-flag]").empty().html(app.library.html.boolean(row.WrqArchiveFlag, true, true));
    grid.find("[name=wrq-cmm-value]").empty().html(app.library.html.parseBbCode(row.WrqCmmValue));
    grid.find("[name=wrq-create-datetime]").empty().html(row.WrqDtgCreateDatetime ? moment(row.WrqDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
    grid.find("[name=wrq-create-username]").empty().html(app.library.html.link.user(row.WrqDtgCreateCcnUsername));

    // Response
    grid.find("[name=rsp-value]").empty().html(app.release.workflow.history.render.reply(row.RspCode, row.RspValue));
    grid.find("[name=wrs-cmm-value]").empty().html(app.library.html.parseBbCode(row.WrsCmmValue));
    grid.find("[name=wrs-create-datetime]").empty().html(row.WrsDtgCreateDatetime ? moment(row.WrsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
    grid.find("[name=wrs-create-username]").empty().html(app.library.html.link.user(row.WrsDtgCreateCcnUsername));

    // Signoff
    grid.find("[name=sgn-value]").empty().html(app.release.workflow.history.render.reply(row.SgnCode, row.SgnValue));
    grid.find("[name=sgn-cmm-value]").empty().html(app.library.html.parseBbCode(row.WsgCmmValue)); //No Translation - User comment
    grid.find("[name=sgn-create-datetime]").empty().html(row.WsgDtgCreateDatetime ? moment(row.WsgDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
    grid.find("[name=sgn-create-ccn-username]").empty().html(app.library.html.link.user(row.WsgDtgCreateCcnUsername));

    // Remove non-relevant data
    switch (row.RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            // Do nothing
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            grid.find("[name=wrq-exceptional-flag]").parent().remove();
            grid.find("[name=wrq-datetime]").parent().remove();
            break;
        case C_APP_TS_REQUEST_DELETE:
        case C_APP_TS_REQUEST_ROLLBACK:
            grid.find("[name=wrq-exceptional-flag]").parent().remove();
            grid.find("[name=wrq-datetime]").parent().remove();
            grid.find("[name=wrq-reservation-flag]").parent().remove();
            grid.find("[name=wrq-archive-flag]").parent().remove();
            break;
    }

    // Remove blank Response
    if (row.RspCode == null) {
        grid.find("[name=card-response]").remove();
    }

    // Remove blank Signoff
    if (row.SgnCode == null) {
        grid.find("[name=card-signoff]").remove();
    }

    return grid.show();
};

/**
 * Delete User 
 */
app.release.workflow.history.delete = function () {
    var idn = $(this).attr("idn");
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [""]), app.release.workflow.history.ajax.delete, idn);
};

/**
 * AJAX call to Delete a specific entry 
 * On AJAX success delete (Do reload table)
 * @param {*} idn
 */
app.release.workflow.history.ajax.delete = function (idn) {
    // Call the API by passing the idn to delete User from DB
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.WorkflowRequest_API.Delete",
        { RlsCode: idn },
        "app.release.workflow.history.callback.deleteOnSuccess",
        null,
        "app.release.workflow.history.callback.deleteOnError",
        null,
        { async: false }
    );
};

/**
* Callback from server for Delete request
* @param {*} data
*/
app.release.workflow.history.callback.deleteOnSuccess = function (data) {
    //Redraw Data Table Workflow History with fresh data.
    app.release.workflow.history.read();

    if (data == C_APP_API_SUCCESS) {
        var goToParams = {
            "RlsCode": app.release.RlsCode,
            "MtrCode": app.release.MtrCode
        };
        // Display Success Modal
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [""]));
        $("#modal-success").one('hidden.bs.modal', function (e) {
            // Force page reload
            api.content.goTo("entity/release/", "#nav-link-release", "#nav-link-release", goToParams);
        });
    }
    // Handle Exception
    else
        api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* Callback from server for Delete request
* @param {*} error
*/
app.release.workflow.history.callback.deleteOnError = function (error) {
    //Redraw Data Table Workflow History with fresh data.
    app.release.workflow.history.read();
};
//#endregion