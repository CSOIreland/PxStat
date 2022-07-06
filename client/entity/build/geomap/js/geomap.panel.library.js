/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces definitions
app.geomap.panel = {};
app.geomap.panel.modal = {};
app.geomap.panel.validation = {};
app.geomap.panel.ajax = {};
app.geomap.panel.callback = {};
//#endregion Namespaces definitions

app.geomap.panel.ajax.readLayers = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoLayer_API.Read",
        {},
        "app.geomap.panel.callback.readLayers");
};

app.geomap.panel.callback.readLayers = function (data) {
    if ($.fn.dataTable.isDataTable("#map-panel table")) {
        app.library.datatable.reDraw("#map-panel table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (_data, _type, row) {
                        if (app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
                            return row.GlrName;
                        }
                        else {
                            return app.library.html.link.edit({ "idn": row.GlrCode, "glr-name": row.GlrName }, row.GlrName);
                        }
                    }
                },
                { data: "GmpCount" },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    visible: app.navigation.user.prvCode == C_APP_PRIVILEGE_ADMINISTRATOR ? true : false,
                    render: function (data, type, row) {
                        var deleteButton = app.library.html.deleteButton({ "idn": row.GlrCode, "glr-name": row.GlrName }, false);
                        if (row.GmpCount > 0) {
                            deleteButton = app.library.html.deleteButton({ "idn": row.GlrCode, "glr-name": row.GlrName }, true);
                        }
                        return deleteButton;
                    },
                    "width": "1%"
                }
            ],
            drawCallback: function (settings) {
                app.geomap.panel.callback.readLayersDrawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#map-panel table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.geomap.panel.callback.readLayersDrawCallback();
        });

    }
};

app.geomap.panel.callback.readLayersDrawCallback = function () {
    // click event update
    $("#map-panel table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        app.geomap.panel.ajax.readUpdate($(this).attr("idn"));
    });
    // click event delete
    $("#map-panel table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [$(this).attr("glr-name")]), app.geomap.panel.ajax.deleteLayer, {
            "idn": $(this).attr("idn"),
            "glrName": $(this).attr("glr-name")
        });
        //app.theme.modal.delete($(this).attr("idn"), $(this).attr("glr-name"));
    });
};

app.geomap.panel.modal.addLayer = function () {
    //validate Create form
    app.geomap.panel.validation.createLayer();
    $("#map-modal-create-layer").modal("show");
};

app.geomap.panel.validation.createLayer = function () {
    $("#map-modal-create-layer form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "glr-name": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#map-modal-create-layer [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.geomap.panel.ajax.createLayer();
        }
    }).resetForm();
};

app.geomap.panel.ajax.createLayer = function () {
    var glrName = $("#map-modal-create-layer [name=glr-name]").val().trim();
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoLayer_API.Create",
        {
            "GlrName": glrName
        },
        "app.geomap.panel.callback.createLayer",
        glrName);
};

app.geomap.panel.callback.createLayer = function (data, glrName) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#map-modal-create-layer").modal("hide");
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [glrName]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

    //Refresh the table
    app.geomap.panel.ajax.readLayers();
};

app.geomap.panel.ajax.readUpdate = function (idn) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoLayer_API.Read",
        {
            "GlrCode": idn
        },
        "app.geomap.panel.callback.readUpdate");
};

app.geomap.panel.callback.readUpdate = function (data) {
    if (data && Array.isArray(data) && data.length) {
        data = data[0];
        //validate Update theme form
        app.geomap.panel.validation.updateLayer();
        //Display of Modal update
        app.geomap.panel.modal.updateLayer(data);
    } else {
        api.modal.information(app.label.static["api-ajax-nodata"]);
        //Refresh the table
        app.geomap.panel.ajax.readLayers();
    }
};

app.geomap.panel.validation.updateLayer = function () {
    $("#map-modal-update-layer form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "glr-name": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#map-modal-update-layer [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.geomap.panel.ajax.updateLayer();
        }
    }).resetForm();
};

app.geomap.panel.modal.updateLayer = function (data) {
    $("#map-modal-update-layer").find("[name='idn']").val(data.GlrCode);
    $("#map-modal-update-layer").find("[name=glr-name]").val(data.GlrName);
    $("#map-modal-update-layer").modal("show");
};

app.geomap.panel.ajax.updateLayer = function () {
    var glrName = $("#map-modal-update-layer").find("[name=glr-name]").val().trim();
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoLayer_API.Update",
        {
            "GlrCode": $("#map-modal-update-layer").find("[name=idn]").val(),
            "GlrName": glrName,
        },
        "app.geomap.panel.callback.updateLayer",
        glrName);
};

app.geomap.panel.callback.updateLayer = function (data, glrName) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#map-modal-update-layer").modal("hide");
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [glrName]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

    //Refresh the table
    app.geomap.panel.ajax.readLayers();
    app.geomap.ajax.read();
};

app.geomap.panel.ajax.deleteLayer = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoLayer_API.Delete",
        {
            "GlrCode": params.idn
        },
        "app.geomap.panel.callback.deleteLayer",
        params.glrName);
};

app.geomap.panel.callback.deleteLayer = function (data, glrName) {
    //Refresh the table
    app.geomap.panel.ajax.readLayers();

    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [glrName]));
    } else api.modal.exception(app.label.static["api-ajax-exception"]);
}