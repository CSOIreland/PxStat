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
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadWorkInProgress",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.panel.workInProgress.callback.read");
};

/**
 * Callback for read
 * @param {*} response
 */
app.release.panel.workInProgress.callback.read = function (response) {
    if (response.error) {
        app.release.panel.workInProgress.callback.drawDatatable();
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.release.panel.workInProgress.callback.drawDataTable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.release.panel.drawCallbackWorkInProgress = function () {
    $('[data-toggle="tooltip"]').tooltip();

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
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.edit(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            "data-toggle": "tooltip",
                            "text": row.GrpCode,
                            "title": row.GrpName
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.DhtDatetime ? moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }
                }
            ],
            order: [2, 'desc'],
            drawCallback: function (settings) {
                app.release.panel.drawCallbackWorkInProgress();
            },
            //Translate labels language
            language: app.label.plugin.datatable
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
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingResponse",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.panel.awaitingResponse.callback.read");
};

/**
 * Callback for read
 * @param {*} response
 */
app.release.panel.awaitingResponse.callback.read = function (response) {
    if (response.error) {
        app.release.panel.awaitingResponse.callback.drawDatatable();
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.release.panel.awaitingResponse.callback.drawDataTable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.release.panel.drawCallbackAwaitingResponse = function () {
    $('[data-toggle="tooltip"]').tooltip();

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
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.edit(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            "data-toggle": "tooltip",
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
            language: app.label.plugin.datatable
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
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingSignoff",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.panel.awaitingSignoff.callback.read");
};

/**
 * Callback for read
 * @param {*} response
 */
app.release.panel.awaitingSignoff.callback.read = function (response) {
    if (response.error) {
        app.release.panel.awaitingSignoff.callback.drawDatatable();
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.release.panel.awaitingSignoff.callback.drawDataTable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.release.panel.drawCallbackAwaitingSignOff = function () {
    $('[data-toggle="tooltip"]').tooltip();
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
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.edit(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            "data-toggle": "tooltip",
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
            language: app.label.plugin.datatable
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
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadPendingLive",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.panel.pendingLive.callback.read");
};

/**
 * Callback for read
 * @param {*} response
 */
app.release.panel.pendingLive.callback.read = function (response) {
    if (response.error) {
        app.release.panel.pendingLive.callback.drawDatatable();
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.release.panel.pendingLive.callback.drawDataTable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.release.panel.drawCallbackPendingLive = function () {
    $('[data-toggle="tooltip"]').tooltip();
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
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.edit(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            "data-toggle": "tooltip",
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
            language: app.label.plugin.datatable
        };
        $("#release-panel-pendinglive table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.panel.drawCallbackPendingLive();
        });
    }
};
//#endregion