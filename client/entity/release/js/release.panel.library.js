/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.release = app.release || {};

app.release.panel = {};
app.release.panel.workInProgress = {};
app.release.panel.workInProgress.ajax = {};
app.release.panel.workInProgress.callback = {};

app.release.panel.awaitingResponse = {};
app.release.panel.awaitingResponse.ajax = {};
app.release.panel.awaitingResponse.callback = {};

app.release.panel.awaitingSignoff = {};
app.release.panel.awaitingSignoff.ajax = {};
app.release.panel.awaitingSignoff.callback = {};

app.release.panel.pendingLive = {};
app.release.panel.pendingLive.ajax = {};
app.release.panel.pendingLive.callback = {};
//#endregion

//#region Read workInProgress
/**
*Call Ajax for read
*/
app.release.panel.workInProgress.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadWorkInProgress",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.panel.workInProgress.callback.readOnSuccess",
        null,
        "app.release.panel.workInProgress.callback.readOnError",
        null);
};

/**
 * Callback for read
 * @param {*} data
 */
app.release.panel.workInProgress.callback.readOnSuccess = function (data) {
    app.release.panel.workInProgress.callback.drawDataTable(data);
};

/**
 * Callback for read
 * @param {*} error
 */
app.release.panel.workInProgress.callback.readOnError = function (error) {
    app.release.panel.workInProgress.callback.drawDatatable();
};

/**
 * Draw Callback for Datatable
 */
app.release.panel.drawCallbackWorkInProgress = function () {
    $('[data-bs-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#release-panel-workinprogress table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
        $('.tooltip').remove();

        app.release.goTo.load($(this).attr("MtrCode"), $(this).attr("idn"));
    });
}

/**
 * Populate Data Table data
 * @param {*} data
 */
app.release.panel.workInProgress.callback.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#release-panel-workinprogress table")) {
        app.library.datatable.reDraw("#release-panel-workinprogress table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    "data": "MtrCode",
                    "visible": false,
                    "searchable": false
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.edit(attributes, row.MtrCode, row.MtrTitle);
                    },
                    orderData: [0]
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            "data-bs-toggle": "tooltip",
                            "text": row.GrpCode,
                            "title": row.GrpName
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    visible: false,
                    render: function (data, type, row) {
                        return row.DhtDatetime ? moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }
                }
            ],
            order: [3, 'desc'],
            drawCallback: function (settings) {
                app.release.panel.drawCallbackWorkInProgress();
            },
            //Translate labels language
            language: app.label.plugin.datatable,
            "pagingType": "numbers"
        };
        $("#release-panel-workinprogress table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.panel.drawCallbackWorkInProgress();
        });
    }
};
//#endregion

//#region Read awaitingResponse

/**
*Call Ajax for read
*/
app.release.panel.awaitingResponse.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingResponse",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.panel.awaitingResponse.callback.readOnSuccess",
        null,
        "app.release.panel.awaitingResponse.callback.readOnError",
        null);
};

/**
 * Callback for read
 * @param {*} data
 */
app.release.panel.awaitingResponse.callback.readOnSuccess = function (data) {
    app.release.panel.awaitingResponse.callback.drawDataTable(data);
};

/**
 * Callback for read
 * @param {*} error
 */
app.release.panel.awaitingResponse.callback.readOnError = function (error) {
    app.release.panel.awaitingResponse.callback.drawDatatable();
};

/**
 * Draw Callback for Datatable
 */
app.release.panel.drawCallbackAwaitingResponse = function () {
    $('[data-bs-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#release-panel-awaitingresponse table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
        $('.tooltip').remove();

        app.release.goTo.load($(this).attr("MtrCode"), $(this).attr("idn"));
    });
}

/**
 * Populate Data Table data
 * @param {*} data
 */
app.release.panel.awaitingResponse.callback.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#release-panel-awaitingresponse table")) {
        app.library.datatable.reDraw("#release-panel-awaitingresponse table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    "data": "MtrCode",
                    "visible": false,
                    "searchable": false
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.edit(attributes, row.MtrCode, row.MtrTitle);
                    },
                    orderData: [0]
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            "data-bs-toggle": "tooltip",
                            "text": row.GrpCode,
                            "title": row.GrpName
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.label.datamodel.request[row.RqsValue]
                    }
                }
            ],
            order: [0, 'asc'],
            drawCallback: function (settings) {
                app.release.panel.drawCallbackAwaitingResponse();

            },
            //Translate labels language
            language: app.label.plugin.datatable,
            "pagingType": "numbers"
        };

        $("#release-panel-awaitingresponse table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.panel.drawCallbackAwaitingResponse();
        });
    }
};
//#endregion

//#region Read awaitingSignoff

/**
*Call Ajax for read
*/
app.release.panel.awaitingSignoff.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingSignoff",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.panel.awaitingSignoff.callback.readOnSuccess",
        null,
        "app.release.panel.awaitingSignoff.callback.readOnError",
        null);
};

/**
 * Callback for read
 * @param {*} data
 */
app.release.panel.awaitingSignoff.callback.readOnSuccess = function (data) {
    app.release.panel.awaitingSignoff.callback.drawDataTable(data);
};

/**
 * Callback for read
 * @param {*} error
 */
app.release.panel.awaitingSignoff.callback.readOnError = function (error) {
    app.release.panel.awaitingSignoff.callback.drawDatatable();
};

/**
 * Draw Callback for Datatable
 */
app.release.panel.drawCallbackAwaitingSignOff = function () {
    $('[data-bs-toggle="tooltip"]').tooltip();
    //Edit link click
    $("#release-panel-awaitingsignoff table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
        $('.tooltip').remove();

        app.release.goTo.load($(this).attr("MtrCode"), $(this).attr("idn"));
    });
}


/**
 * Populate Data Table data
 * @param {*} data
 */
app.release.panel.awaitingSignoff.callback.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#release-panel-awaitingsignoff table")) {
        app.library.datatable.reDraw("#release-panel-awaitingsignoff table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    "data": "MtrCode",
                    "visible": false,
                    "searchable": false
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.edit(attributes, row.MtrCode, row.MtrTitle);
                    },
                    orderData: [0]
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            "data-bs-toggle": "tooltip",
                            "text": row.GrpCode,
                            "title": row.GrpName
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.label.datamodel.request[row.RqsValue]
                    }
                }
            ],
            order: [0, 'asc'],
            drawCallback: function (settings) {
                app.release.panel.drawCallbackAwaitingSignOff();
            },
            //Translate labels language
            language: app.label.plugin.datatable,
            "pagingType": "numbers"
        };
        $("#release-panel-awaitingsignoff table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.panel.drawCallbackAwaitingSignOff();
        });
    }
};
//#endregion

//#region Read pending live

/**
*Call Ajax for read
*/
app.release.panel.pendingLive.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadPendingLive",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.panel.pendingLive.callback.readOnSuccess",
        null,
        "app.release.panel.pendingLive.callback.readOnError",
        null);
};

/**
 * Callback for read
 * @param {*} data
 */
app.release.panel.pendingLive.callback.readOnSuccess = function (data) {
    app.release.panel.pendingLive.callback.drawDataTable(data);
};

/**
 * Callback for read
 * @param {*} error
 */
app.release.panel.pendingLive.callback.readOnError = function (error) {
    app.release.panel.pendingLive.callback.drawDatatable();
};

/**
 * Draw Callback for Datatable
 */
app.release.panel.drawCallbackPendingLive = function () {
    $('[data-bs-toggle="tooltip"]').tooltip();
    //Edit link click
    $("#release-panel-pendinglive table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
        $('.tooltip').remove();

        app.release.goTo.load($(this).attr("MtrCode"), $(this).attr("idn"));
    });
}


/**
 * Populate Data Table data
 * @param {*} data
 */
app.release.panel.pendingLive.callback.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#release-panel-pendinglive table")) {
        app.library.datatable.reDraw("#release-panel-pendinglive table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    "data": "MtrCode",
                    "visible": false,
                    "searchable": false
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.edit(attributes, row.MtrCode, row.MtrTitle);
                    },
                    orderData: [0]
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            "data-bs-toggle": "tooltip",
                            "text": row.GrpCode,
                            "title": row.GrpName
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.label.datamodel.request[row.RqsValue]
                    }
                }
            ],
            order: [0, 'asc'],
            drawCallback: function (settings) {
                app.release.panel.drawCallbackPendingLive();
            },
            //Translate labels language
            language: app.label.plugin.datatable,
            "pagingType": "numbers"
        };
        $("#release-panel-pendinglive table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.panel.drawCallbackPendingLive();
        });
    }
};
//#endregion