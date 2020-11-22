/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.dashboard = app.dashboard || {};

app.dashboard.ajax = {};
app.dashboard.callback = {};

app.dashboard.workInProgress = {};
app.dashboard.workInProgress.ajax = {};
app.dashboard.workInProgress.callback = {};

app.dashboard.awaitingResponse = {};
app.dashboard.awaitingResponse.ajax = {};
app.dashboard.awaitingResponse.callback = {};

app.dashboard.awaitingSignoff = {};
app.dashboard.awaitingSignoff.ajax = {};
app.dashboard.awaitingSignoff.callback = {};

app.dashboard.pendinglive = {};
app.dashboard.pendinglive.ajax = {};
app.dashboard.pendinglive.callback = {};

app.dashboard.liveReleases = {};
app.dashboard.liveReleases.ajax = {};
app.dashboard.liveReleases.callback = {};

app.dashboard.render = {};

//#endregion


app.dashboard.ajax.ReadCurrentAccess = function () {
    //Check the privilege of the user 
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Account_API.ReadCurrentAccess",
        { CcnUsername: null },
        "app.dashboard.callback.ReadCurrentAccess",
        null,
        null,
        null,
        { async: false }
    );
};

app.dashboard.callback.ReadCurrentAccess = function (data) {
    switch (data[0].PrvCode) {
        case C_APP_PRIVILEGE_MODERATOR:
            $("#collapse-work-in-progress").collapse('show');
            $("#dashboard-accordion").on('show.bs.collapse', function (e) {
                $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-minus-circle");
            });
            /*  $("#collapse-awaiting-response").collapse('show');
              $("#dashboard-accordion").on('show.bs.collapse', function (e) {
                  $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-minus-circle");
              });*/
            break;
        case C_APP_PRIVILEGE_POWER_USER:
            $("#collapse-awaiting-sign-off").collapse('show');
            $("#dashboard-accordion").on('show.bs.collapse', function (e) {
                $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-minus-circle");
            });
            break;
        case C_APP_PRIVILEGE_ADMINISTRATOR:
        default:
            // Leave all collapsed by default
            break;
    }
};

//#region Work in Progress

/**
*Call Ajax for read
*/
app.dashboard.workInProgress.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadWorkInProgress",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.dashboard.workInProgress.callback.read");
};

/**
 * Callback for read
 * @param {*} data
 */
app.dashboard.workInProgress.callback.read = function (data) {
    app.dashboard.workInProgress.drawDataTable(data);
};

/**
 * Draw Callback for Datatable
 */
app.dashboard.workInProgress.drawCallbackWorkInProgress = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-workinprogress table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_INTERNAL + "]" link.
        $('.tooltip').remove();

        api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": $(this).attr("MtrCode"), "RlsCode": $(this).attr("idn") });
    });

    //view group info
    $("#dashboard-panel-workinprogress table").find("[name=group]").once("click", function (e) {
        e.preventDefault();
        app.library.group.modal.read($(this).attr("idn"));
    });

    //view user info
    $("#dashboard-panel-workinprogress table").find("[name=uploaded-by]").once("click", function (e) {
        e.preventDefault();
        app.library.user.modal.read({ CcnUsername: $(this).attr("idn") });
    });
}

/**
 * Populate Data Table data
 * @param {*} data
 */
app.dashboard.workInProgress.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#dashboard-panel-workinprogress table")) {
        app.library.datatable.reDraw("#dashboard-panel-workinprogress table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var linkGroup = $("<a>", {
                            idn: data.GrpCode,
                            name: "group",
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
                            name: "uploaded-by",
                            "href": "#",
                            "html": data.CcnUsername
                        }).get(0).outerHTML;

                        return userLink;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.DhtDatetime ? moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";

                    }
                }

            ],
            order: [3, 'desc'],
            drawCallback: function (settings) {
                app.dashboard.workInProgress.drawCallbackWorkInProgress();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#dashboard-panel-workinprogress table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.dashboard.workInProgress.drawCallbackWorkInProgress();
        });
    }
};
//#endregion

//#region Read awaitingResponse

/**
*Call Ajax for read
*/
app.dashboard.awaitingResponse.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingResponse",
        {
            LngIsoCode: app.label.language.iso.code

        },
        "app.dashboard.awaitingResponse.callback.readOnSuccess",
        null,
        "app.dashboard.awaitingResponse.callback.readOnError",
        null);
};

/**
 * Callback for read
 * @param {*} data
 */
app.dashboard.awaitingResponse.callback.readOnSuccess = function (data) {
    app.dashboard.awaitingResponse.drawDataTable(data);
};

/**
 * Callback for read
 * @param {*} error
 */
app.dashboard.awaitingResponse.callback.readOnError = function (error) {
    app.dashboard.awaitingResponse.drawDataTable();
};

/**
 * Draw Callback for Datatable
 */
app.dashboard.drawCallbackAwaitingResponse = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-awaitingresponse table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_INTERNAL + "]" link.
        $('.tooltip').remove();
        api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": $(this).attr("MtrCode"), "RlsCode": $(this).attr("idn") });
    });

    //view group info
    $("#dashboard-panel-awaitingresponse table").find("[name=group]").once("click", function (e) {
        e.preventDefault();
        app.library.group.modal.read($(this).attr("idn"));
    });

    //view user info
    $("#dashboard-panel-awaitingresponse table").find("[name=requested-by]").once("click", function (e) {
        e.preventDefault();
        app.library.user.modal.read({ CcnUsername: $(this).attr("idn") });
    });

}

/**
 * Populate Data Table data
 * @param {*} data
 */
app.dashboard.awaitingResponse.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#dashboard-panel-awaitingresponse table")) {
        app.library.datatable.reDraw("#dashboard-panel-awaitingresponse table", data);
    } else {
        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var linkGroup = $("<a>", {
                            idn: data.GrpCode,
                            name: "group",
                            href: "#",
                            html: data.GrpCode
                        }).get(0).outerHTML;

                        return linkGroup;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.label.datamodel.request[row.RqsValue]
                    }
                },
                {
                    data: null,
                    type: "html",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsExceptionalFlag, true, true);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.WrqDatetime ? moment(row.WrqDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var userLink = $("<a>", {
                            idn: data.CcnUsername,
                            name: "requested-by",
                            "href": "#",
                            "html": data.CcnUsername
                        }).get(0).outerHTML;

                        return userLink;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.DhtDatetime ? moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }
                }

            ],
            order: [4, 'asc'],
            drawCallback: function (settings) {
                app.dashboard.drawCallbackAwaitingResponse();

            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#dashboard-panel-awaitingresponse table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.dashboard.drawCallbackAwaitingResponse();
        });

    }
};
//#endregion

//#region Read awaitingSignoff

/**
*Call Ajax for read
*/
app.dashboard.awaitingSignoff.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingSignoff",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.dashboard.awaitingSignoff.callback.readOnSuccess",
        null,
        "app.dashboard.awaitingSignoff.callback.readOnError",
        null);
};

/**
 * Callback for read
 * @param {*} data
 */
app.dashboard.awaitingSignoff.callback.readOnSuccess = function (data) {
    app.dashboard.awaitingSignoff.drawDataTable(data);
};

/**
 * Callback for read
 * @param {*} error
 */
app.dashboard.awaitingSignoff.callback.readOnError = function (error) {
    app.dashboard.awaitingSignoff.drawDataTable();
};
/**
 * Draw Callback for Datatable
 */
app.dashboard.drawCallbackAwaitingSignOff = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-awaitingsignoff table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_INTERNAL + "]" link.
        $('.tooltip').remove();
        api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": $(this).attr("MtrCode"), "RlsCode": $(this).attr("idn") });
    });
    //view group info
    $("#dashboard-panel-awaitingsignoff table").find("[name=group]").once("click", function (e) {
        e.preventDefault();
        app.library.group.modal.read($(this).attr("idn"));
    });

    //view user info
    $("#dashboard-panel-awaitingsignoff table").find("[name=approved-by]").once("click", function (e) {
        e.preventDefault();
        app.library.user.modal.read({ CcnUsername: $(this).attr("idn") });
    });
}

/**
 * Populate Data Table data
 * @param {*} data
 */
app.dashboard.awaitingSignoff.drawDataTable = function (data) {

    if ($.fn.dataTable.isDataTable("#dashboard-panel-awaitingsignoff table")) {
        app.library.datatable.reDraw("#dashboard-panel-awaitingsignoff table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var linkGroup = $("<a>", {
                            idn: data.GrpCode,
                            name: "group",
                            href: "#",
                            html: data.GrpCode
                        }).get(0).outerHTML;

                        return linkGroup;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.label.datamodel.request[row.RqsValue]
                    }
                },
                {
                    data: null,
                    type: "html",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsExceptionalFlag, true, true);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.WrqDatetime ? moment(row.WrqDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";


                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var userLink = $("<a>", {
                            idn: data.CcnUsername,
                            name: "approved-by",
                            "href": "#",
                            "html": data.CcnUsername
                        }).get(0).outerHTML;

                        return userLink;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.DhtDatetime ? moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }
                }

            ],
            order: [4, 'asc'],
            drawCallback: function (settings) {
                app.dashboard.drawCallbackAwaitingSignOff();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#dashboard-panel-awaitingsignoff table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.dashboard.drawCallbackAwaitingSignOff();
        });
    }
};
//#endregion

//#region Read Pending Live

/**
*Call Ajax for read
*/
app.dashboard.pendinglive.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadPendingLive",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.dashboard.pendinglive.callback.readOnSuccess",
        null,
        "app.dashboard.pendinglive.callback.readOnError",
        null);
};

/**
 * Callback for read
 * @param {*} data
 */
app.dashboard.pendinglive.callback.readOnSuccess = function (data) {
    app.dashboard.pendinglive.drawDataTable(data);
};

/**
 * Callback for read
 * @param {*} error
 */
app.dashboard.pendinglive.callback.readOnError = function (error) {
    app.dashboard.pendinglive.drawDataTable();
};

/**
 * Draw Callback for Datatable
 */
app.dashboard.drawCallbackPendingLive = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-pendinglive table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_INTERNAL + "]" link.
        $('.tooltip').remove();
        api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": $(this).attr("MtrCode"), "RlsCode": $(this).attr("idn") });
    });
    //view group info
    $("#dashboard-panel-pendinglive table").find("[name=group]").once("click", function (e) {
        e.preventDefault();
        app.library.group.modal.read($(this).attr("idn"));
    });

    //view user info
    $("#dashboard-panel-pendinglive table").find("[name=approved-by]").once("click", function (e) {
        e.preventDefault();
        app.library.user.modal.read({ CcnUsername: $(this).attr("idn") });
    });
}

/**
 * Populate Data Table data
 * @param {*} data
 */
app.dashboard.pendinglive.drawDataTable = function (data) {

    if ($.fn.dataTable.isDataTable("#dashboard-panel-pendinglive table")) {
        app.library.datatable.reDraw("#dashboard-panel-pendinglive table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var linkGroup = $("<a>", {
                            idn: data.GrpCode,
                            name: "group",
                            href: "#",
                            html: data.GrpCode
                        }).get(0).outerHTML;

                        return linkGroup;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.label.datamodel.request[row.RqsValue]
                    }
                },
                {
                    data: null,
                    type: "html",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsExceptionalFlag, true, true);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.WrqDatetime ? moment(row.WrqDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var userLink = $("<a>", {
                            idn: data.CcnUsername,
                            name: "approved-by",
                            "href": "#",
                            "html": data.CcnUsername
                        }).get(0).outerHTML;

                        return userLink;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.DhtDatetime ? moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }
                }

            ],
            order: [4, 'asc'],
            drawCallback: function (settings) {
                app.dashboard.drawCallbackPendingLive();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#dashboard-panel-pendinglive table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.dashboard.drawCallbackPendingLive();
        });
    }
};
//#endregion

//#region Live releases
/**
*Call Ajax for read
*/
app.dashboard.liveReleases.ajax.read = function () {

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.ReadLive",
        {
            LngIsoCode: app.label.language.iso.code
        },
        "app.dashboard.liveReleases.callback.readOnSuccess",
        null,
        "app.dashboard.liveReleases.callback.readOnError",
        null);
};

/**
 * Callback for read
 * @param {*} data
 */
app.dashboard.liveReleases.callback.readOnSuccess = function (data) {
    app.dashboard.liveReleases.drawDataTable(data);
};

/**
 * Callback for read
 * @param {*} error
 */
app.dashboard.liveReleases.callback.readOnError = function (error) {
    app.dashboard.liveReleases.drawDataTable();
};
/**
 * Draw Callback for Datatable
 */
app.dashboard.drawCallbackliveReleases = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-livereleases table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();

        $('.tooltip').remove();
        api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": $(this).attr("mtr-code"), "RlsCode": $(this).attr("rls-code") });
    });
    //view group info
    $("#dashboard-panel-livereleases table").find("[name=group]").once("click", function (e) {
        e.preventDefault();
        app.library.group.modal.read($(this).attr("idn"));
    });

    $("#dashboard-panel-livereleases table").find("[name=" + C_APP_NAME_LINK_ANALYTIC + "]").once("click", function (e) {
        e.preventDefault();
        app.analytic.ajax.readBrowser($(this).attr("mtr-code"), "#analytic-chart-modal [name=browser-pie-chart]");
        app.analytic.ajax.readUserLanguage($(this).attr("mtr-code"), "#analytic-chart-modal [name=user-language-column-chart]");
        app.analytic.ajax.readOs($(this).attr("mtr-code"), "#analytic-chart-modal [name=operating-system-pie-chart]");
        app.analytic.ajax.readReferrer($(this).attr("mtr-code"), "#analytic-chart-modal [name=referrer-column-chart]");
        app.analytic.ajax.readTimeline($(this).attr("mtr-code"), "#analytic-chart-modal [name=dates-line-chart]");
        app.analytic.ajax.readLanguage($(this).attr("mtr-code"), "#analytic-chart-modal [name=language-pie-chart]");
        app.analytic.ajax.readFormat($(this).attr("mtr-code"), "#analytic-chart-modal [name=format-pie-chart]");
        $("#matrix-chart-modal").find("[name=mtr-title]").text($(this).attr("mtr-code") + " : " + $(this).attr("mtr-title"));// + "          "
        //  + app.label.static["from"] + " : " + app.analytic.dateFrom.format(app.config.mask.date.display)
        //  + "    " + app.label.static["to"] + " : " + app.analytic.dateTo.format(app.config.mask.date.display));
        $("#matrix-chart-modal").find("[name=date-range]").html(app.analytic.dateFrom.format(app.config.mask.date.display)
            + "    " + " - " + app.analytic.dateTo.format(app.config.mask.date.display));
        $("#matrix-chart-modal").modal("show");

    });
}

/**
 * Populate Data Table data
 * @param {*} data
 */
app.dashboard.liveReleases.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#dashboard-panel-livereleases table")) {
        app.library.datatable.reDraw("#dashboard-panel-livereleases table", data);
    } else {
        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { "rls-code": row.RlsCode, "mtr-code": row.MtrCode };
                        return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var linkGroup = $("<a>", {
                            idn: data.GrpCode,
                            name: "group",
                            href: "#",
                            html: data.GrpCode
                        }).get(0).outerHTML;

                        return linkGroup;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.RlsLiveDatimeFrom ? moment(row.RlsLiveDatimeFrom, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }

                },
                {
                    data: null,
                    type: "html",
                    render: function (data, type, row) {
                        var attributes = { "mtr-code": row.MtrCode, "mtr-title": row.MtrTitle };
                        return app.library.html.link.analytic(attributes);
                    }
                },
                {
                    data: null,
                    type: "natural",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.MtrOfficialFlag, true, true);
                    }
                },
                {
                    data: null,
                    type: "natural",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsReservationFlag, true, true);

                    }
                },
                {
                    data: null,
                    type: "natural",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsExperimentalFlag, true, true);

                    }
                },
                {
                    data: null,
                    type: "natural",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsArchiveFlag, true, true);

                    }
                }

            ],
            order: [2, 'desc'],
            drawCallback: function (settings) {
                app.dashboard.drawCallbackliveReleases();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#dashboard-panel-livereleases table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.dashboard.drawCallbackliveReleases();
        });
    }
};
//#endregion
