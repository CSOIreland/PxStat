/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build.map = app.build.map || {};
app.build.map.ajax = {};
app.build.map.callback = {};
app.build.map.params = null;
//#endregion

app.build.map.ajax.readMaps = function (params) {
    app.build.map.params = params;
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.ReadCollection",
        {},
        "app.build.map.callback.readMaps",
        null,
        null,
        null,
        { async: false });
};

app.build.map.callback.readMaps = function (data) {
    $("#build-map-modal").find(".modal-header [name=cls-value]").empty().text(app.build.map.params.clsCode + ": " + app.build.map.params.clsValue);
    if ($.fn.dataTable.isDataTable("#build-map-modal table")) {
        app.library.datatable.reDraw("#build-map-modal table", data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            columns: [
                {
                    data: "GmpName"
                },
                {
                    data: "GmpFeatureCount"
                },
                {
                    data: "GmpDescription",
                    visible: false,
                },
                {
                    data: "GlrName",
                    visible: false,
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
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.link.view({ "gmp-code": row.GmpCode }, null, app.label.static["view-map"]);
                    },
                    "width": "1%"
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return $("<button>", {
                            "class": "btn btn-outline-primary btn-sm",
                            "name": "select",
                            "idn": row.GmpCode,
                            "html":
                                $("<i>", {
                                    "class": "fas fa-check-circle"
                                }).get(0).outerHTML + " " + app.label.static["select"]
                        }).get(0).outerHTML;
                    },
                    "width": "1%"
                }
            ],
            drawCallback: function (settings) {
                app.build.map.callback.drawCallbackReadMaps();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#build-map-modal table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.map.callback.drawCallbackReadMaps();
        });
    }
    $("#build-map-modal").modal("show");
};

app.build.map.callback.drawCallbackReadMaps = function () {
    $("#build-map-modal table").find("[name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
        e.preventDefault();
        app.geomap.preview.ajax.readMap($(this).attr("gmp-code"))
    });

    $("#build-map-modal table").find("[name='select']").once("click", function (e) {
        e.preventDefault();
        api.ajax.callback(app.build.map.params.callback, $(this).attr("idn"), app.build.map.params.clsCode);
    });

    // Extra Info
    app.library.datatable.showExtraInfo("#build-map-modal table", app.build.map.drawExtraInfo);
};

app.build.map.drawExtraInfo = function (data) {
    var extraInfo = $("#build-map-modal-templates").find("[name=map-read-extra-info]").clone();
    extraInfo.find("[name=description]").html(app.library.html.parseBbCode(data.GmpDescription));
    extraInfo.find("[name=layer]").text(data.GlrName);
    return extraInfo.show().get(0).outerHTML;
};
