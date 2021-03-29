/*******************************************************************************
Custom JS application specific // 
*******************************************************************************/
//#region Namespaces definitions
app.database = {};
app.database.ajax = {};
app.database.callback = {};

////#endregion



app.database.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Database_API.Read",
        {},
        "app.database.callback.readOnSuccess",
        null,
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        }
    );
};


app.database.callback.readOnSuccess = function (data) {
    if (data) {
        app.database.callback.drawDatatable(data);
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

app.database.callback.drawDatatable = function (data) {
    if ($.fn.dataTable.isDataTable("#database-read-container table")) {
        app.library.datatable.reDraw("#database-read-container table", data);
    } else {


        var localOptions = {
            data: data,
            columns: [
                { "data": "table" },
                { "data": "index" },
                { "data": "type" },
                { "data": "rows" },
                { "data": "size" },
                { "data": "fragmentation" }
            ],
            "order": [[5, "desc"]],
            drawCallback: function (settings) {
                app.database.callback.drawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#database-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.database.callback.drawCallback();
        });
    }
}



app.database.callback.drawCallback = function () {
    $("#database-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        app.database.ajax.readByTable($(this).attr("idn"));
    });
}


app.database.ajax.reorganise = function (idn) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Database_API.Update",
        {},
        "app.database.callback.reorganise",
        null,
        null,
        null
    );
};

app.database.callback.reorganise = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#database-modal-index-reorganise").modal("hide");
        api.modal.success(app.label.static["success-index-reorganise"]);
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};
//#endregion