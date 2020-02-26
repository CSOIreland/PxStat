/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Create Namespace
// Create Namespace
app.logging = {};
app.logging.validation = {};
app.logging.ajax = {};
app.logging.callback = {};

//#endregion

//#region input options

/**
 * Set up date picker
 */
app.logging.setDatePicker = function () {
    var start = moment().startOf('day');
    var end = moment();
    $("#logging-input").find("[name=input-date-range]").daterangepicker({
        "opens": 'left',
        "startDate": start.format(app.config.mask.datetime.display),
        "endDate": end.format(app.config.mask.datetime.display),
        "autoUpdateInput": true,
        "timePicker": true,
        "timePicker24Hour": true,
        "locale": app.label.plugin.daterangepicker
    }, function (start, end) {

    });
    // Read logging at start application with default datePicker range]
    app.logging.ajax.read();
};

//#endregion

//#region Read logging
/**
 * Get data from api
 */
app.logging.ajax.read = function () {
    var datePicker = $("#logging-input").find("[name=input-date-range]").data('daterangepicker');
    var start = moment(datePicker.startDate).format(app.config.mask.datetime.ajax);
    var end = moment(datePicker.endDate).format(app.config.mask.datetime.ajax);
    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.Security.Logging.Logging_API.Read",
        {
            "LggDatetimeStart": start,
            "LggDatetimeEnd": end,
        },
        "app.logging.callback.read",
        null,
        null,
        null,
        { async: false });
};

/**
 * Handle data from api
 * @param {*} data 
 */
app.logging.callback.read = function (data) {
    app.logging.drawDataTable(data);
};

/**
 * Draw datatable from result data
 * @param {*} data 
 */
app.logging.drawDataTable = function (data) {
    //Initialize ClipboardJS
    new ClipboardJS('.cpy-btn');
    var datePicker = $("#logging-input").find("[name=input-date-range]").data('daterangepicker');
    var exportFileName = 'logging'
        + "_" + datePicker.startDate ? moment(datePicker.startDate).format(app.config.mask.datetime.file) : ""
            + "_" + datePicker.endDate ? moment(datePicker.endDate).format(app.config.mask.datetime.file) : ""
            + "." + moment().format(app.config.mask.datetime.file);
    if ($.fn.dataTable.isDataTable("#logging-result table")) {
        app.library.datatable.reDraw("#logging-result table", data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            buttons: [{
                extend: 'csv',
                title: exportFileName,
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 7, 8],
                    format: {
                        body: function (data, row, column, node) {
                            // Strip HTML
                            return data.toString().replace(C_APP_REGEX_NOHTML, "");
                        }
                    }
                }
            }],
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.LggDatetime ? moment(row.LggDatetime).format(app.config.mask.datetime.display) : "";
                    }
                },
                {
                    data: "LggThread"
                },
                {
                    data: "LggLevel"
                },
                {
                    data: "LggClass"
                },
                {
                    data: "LggMethod"
                },
                {
                    data: "LggLine"
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
                                }).get(0).outerHTML + " " + app.label.static["information"]
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: "LggMessage",
                    "visible": false,
                    "searchable": true
                },
                {
                    data: "LggException",
                    "visible": false,
                    "searchable": true
                }
            ],

            "order": [[0, "desc"]],
            drawCallback: function (settings) {
                app.logging.drawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#logging-result table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.logging.drawCallback();
        });

        // Invoke DataTables CSV export
        // https://stackoverflow.com/questions/45515559/how-to-call-datatable-csv-button-from-custom-button
        $("#logging-result").find("[name=csv]").once("click", function () {
            $("#logging-result table").DataTable().button('.buttons-csv').trigger();
        });

        // Bootstrap tooltip
        $("body").tooltip({
            selector: '[data-toggle="tooltip"]'
        });
    }
    $("#logging-result").show();
};

/**
 * Draw Callback for Datatable
 */
app.logging.drawCallback = function () {
    // Extra Info
    app.library.datatable.showExtraInfo('#logging-result table', app.logging.drawExtraInformation);
}

/**
 * Draw data in hidden columns
 * @param {*} data 
 */
app.logging.drawExtraInformation = function (data) {
    var randomIdMessage = app.library.utility.randomGenerator();
    var randomIdException = app.library.utility.randomGenerator();
    var details = $("#logging-message-template").find("[name=message-detail]").clone();
    details.removeAttr('id');
    details.find("[name=message-card]").find("[name=message]").find("pre code").text(data.LggMessage).attr("id", "message-" + randomIdMessage);
    details.find("[name=message-card]").find(".cpy-btn").attr("data-clipboard-target", "#message-" + randomIdMessage);
    details.find("[name=exception-card]").find("pre code").text(data.LggException).attr("id", "exception-" + randomIdException);
    details.find("[name=exception-card]").find(".cpy-btn").attr("data-clipboard-target", "#exception-" + randomIdException);
    return details.show().get(0).outerHTML;
};

//#endregion







