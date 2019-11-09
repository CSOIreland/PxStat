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

app.dashboard.liveReleases = {};
app.dashboard.liveReleases.ajax = {};
app.dashboard.liveReleases.callback = {};

app.dashboard.render = {};

//#endregion


app.dashboard.ajax.ReadCurrentAccess = function () {
    //Check the privilege of the user 
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Security.Account_API.ReadCurrentAccess",
        { CcnUsername: null },
        "app.dashboard.callback.ReadCurrentAccess",
        null,
        null,
        null,
        { async: false }
    );
};

app.dashboard.callback.ReadCurrentAccess = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data) {
        response.data = response.data[0];
        switch (response.data.PrvCode) {
            case C_APP_PRIVILEGE_MODERATOR:
                $("#collapse-work-in-progress").collapse('show');
                $("#dashboard-accordion").on('show.bs.collapse', function (e) {
                    $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-minus-circle");
                });
                $("#collapse-awaiting-response").collapse('show');
                $("#dashboard-accordion").on('show.bs.collapse', function (e) {
                    $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-minus-circle");
                });
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
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#region Work in Progress

/**
*Call Ajax for read
*/
app.dashboard.workInProgress.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadWorkInProgress",
        null,
        "app.dashboard.workInProgress.callback.read");
};

/**
 * Callback for read
 * @param {*} response
 */
app.dashboard.workInProgress.callback.read = function (response) {
    if (response.error) {
        app.dashboard.workInProgress.callback.drawDatatable();
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.dashboard.workInProgress.callback.drawDataTable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.dashboard.workInProgress.drawCallbackWorkInProgress = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-workinprogress table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
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
app.dashboard.workInProgress.callback.drawDataTable = function (data) {
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
                        return app.library.html.link.edit(attributes, row.MtrCode);
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
                    data: "Request",

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
                        return $("<span>", {
                            class: "text-muted",
                            text: moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display)
                        }).get(0).outerHTML;
                    }
                }

            ],
            order: [4, 'desc'],
            drawCallback: function (settings) {
                app.dashboard.workInProgress.drawCallbackWorkInProgress();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#dashboard-panel-workinprogress table").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
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
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingResponse",
        null,
        "app.dashboard.awaitingResponse.callback.read");
};

/**
 * Callback for read
 * @param {*} response
 */
app.dashboard.awaitingResponse.callback.read = function (response) {
    if (response.error) {
        app.dashboard.awaitingResponse.callback.drawDatatable();
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {

        app.dashboard.awaitingResponse.callback.drawDataTable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.dashboard.drawCallbackAwaitingResponse = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-awaitingresponse table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
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
app.dashboard.awaitingResponse.callback.drawDataTable = function (data) {
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
                        return app.library.html.link.edit(attributes, row.MtrCode);
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
                        return app.label.static[row.RqsValue]
                    }
                },
                {
                    data: null,
                    type: "html",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsEmergencyFlag, true, true);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            class: "text-muted",
                            text: moment(row.WrqDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display)
                        }).get(0).outerHTML;
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
                        return $("<span>", {
                            class: "text-muted",
                            text: moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display)
                        }).get(0).outerHTML;
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

        $("#dashboard-panel-awaitingresponse table").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
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
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingSignoff",
        null,
        "app.dashboard.awaitingSignoff.callback.read");
};



/**
 * Callback for read
 * @param {*} response
 */
app.dashboard.awaitingSignoff.callback.read = function (response) {
    if (response.error) {
        app.dashboard.awaitingSignoff.callback.drawDatatable();
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.dashboard.awaitingSignoff.callback.drawDataTable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
/**
 * Draw Callback for Datatable
 */
app.dashboard.drawCallbackAwaitingSignOff = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-awaitingsignoff table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        //Remove tool tip after click at "[name=" + C_APP_NAME_LINK_EDIT + "]" link.
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
app.dashboard.awaitingSignoff.callback.drawDataTable = function (data) {

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
                        return app.library.html.link.edit(attributes, row.MtrCode);
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
                        return app.label.static[row.RqsValue]
                    }
                },
                {
                    data: null,
                    type: "html",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsEmergencyFlag, true, true);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        if (row.WrqDatetime == null) {
                            return ""
                        } else {
                            return $("<span>", {
                                class: "text-muted",
                                text: moment(row.WrqDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display)
                            }).get(0).outerHTML;
                        }
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
                        return $("<span>", {
                            class: "text-muted",
                            text: moment(row.DhtDatetime, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display)
                        }).get(0).outerHTML;
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
        $("#dashboard-panel-awaitingsignoff table").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.dashboard.drawCallbackAwaitingSignOff();
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
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadLive",
        null,
        "app.dashboard.liveReleases.callback.read");
};



/**
 * Callback for read
 * @param {*} response
 */
app.dashboard.liveReleases.callback.read = function (response) {
    if (response.error) {
        app.dashboard.liveReleases.callback.drawDatatable();
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.dashboard.liveReleases.callback.drawDataTable(response.data);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
/**
 * Draw Callback for Datatable
 */
app.dashboard.drawCallbackliveReleases = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Edit link click
    $("#dashboard-panel-livereleases table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();

        $('.tooltip').remove();
        api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": $(this).attr("mtr-code"), "RlsCode": $(this).attr("rls-code") });
    });
    //view group info
    $("#dashboard-panel-livereleases table").find("[name=group]").once("click", function (e) {
        e.preventDefault();
        app.library.group.modal.read($(this).attr("idn"));
    });

    /* $("#dashboard-panel-livereleases table").find("[name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
         e.preventDefault();
 
         $('.tooltip').remove();
         api.content.goTo("entity/analytic", null, "#nav-link-analytic", { "SbjCode": $(this).attr("MtrCode") });
     });*/

    $("#dashboard-panel-livereleases table").find("[name=" + C_APP_NAME_LINK_ANALYTIC + "]").once("click", function (e) {
        e.preventDefault();
        debugger
        //$('.tooltip').remove();
        //api.content.goTo("entity/analytic", null, "#nav-link-analytic", { "SbjCode": $(this).attr("MtrCode") });
        app.analytic.ajax.readBrowser($(this).attr("mtr-code"), "#analytic-chart-modal [name=browser-pie-chart]");
        app.analytic.ajax.readOs($(this).attr("mtr-code"), "#analytic-chart-modal [name=operating-system-pie-chart]");
        app.analytic.ajax.readReferrer($(this).attr("mtr-code"), "#analytic-chart-modal [name=referrer-column-chart]");
        app.analytic.ajax.readTimeLine($(this).attr("mtr-code"), "#analytic-chart-modal [name=dates-line-chart]");
        app.analytic.ajax.readLanguage($(this).attr("mtr-code"), "#analytic-chart-modal [name=language-pie-chart]");
        $("#matrix-chart-modal").find("[name=mtr-title]").text($(this).attr("mtr-code") + " : " + $(this).attr("mtr-title"));
        $("#matrix-chart-modal").modal("show");

    });
}

/**
 * Populate Data Table data
 * @param {*} data
 */
app.dashboard.liveReleases.callback.drawDataTable = function (data) {
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
                        return app.library.html.link.edit(attributes, row.MtrCode);
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
                        return $("<span>", {
                            class: "text-muted",
                            text: moment(row.RlsLiveDatimeFrom, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display)
                        }).get(0).outerHTML;
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
                    type: "html",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.MtrOfficialFlag, true, true);
                    }
                },
                {
                    data: null,
                    type: "html",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.RlsReservationFlag, true, true);

                    }
                },
                {
                    data: null,
                    type: "html",
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
        $("#dashboard-panel-livereleases table").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.dashboard.drawCallbackliveReleases();
        });
    }
};
//#endregion
