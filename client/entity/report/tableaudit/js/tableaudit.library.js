/*******************************************************************************
Custom JS application specific group.library.js
*******************************************************************************/
//#region Namespaces definitions
// Add Namespace Group Data Table
app.tableaudit = {};
app.tableaudit.ajax = {};
app.tableaudit.callback = {};
app.tableaudit.render = {};
app.tableaudit.validation = {};
app.tableaudit.result = [];

app.tableaudit.dateFrom = moment().subtract(29, 'days'), moment(); // Date type, not String
app.tableaudit.dateTo = moment().subtract(1, 'days'); // Date type, not String
//#endregion

//#region set up form
/**
 * Set up date range picker
 */
app.tableaudit.setDatePicker = function () {
    $("#table-audit-date-range span").html(app.tableaudit.dateFrom.format(app.config.mask.date.display) + ' - ' + app.tableaudit.dateTo.format(app.config.mask.date.display));

    $("#table-audit-date-range").daterangepicker({
        startDate: app.tableaudit.dateFrom,
        endDate: app.tableaudit.dateTo,
        maxDate: moment().add(app.config.report["date-validation"].maxDate, 'days'),
        minDate: moment().subtract(app.config.report["date-validation"].minDate, 'days'),
        ranges: {
            [app.label.static["yesterday"]]: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-7-days"]]: [moment().subtract(7, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-30-days"]]: [moment().subtract(30, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-365-days"]]: [moment().subtract(365, 'days'), moment().subtract(1, 'days')]
        },
        locale: app.label.plugin.daterangepicker
    }, function (start, end) {
        // Override default values with the selection
        app.tableaudit.dateFrom = start;
        app.tableaudit.dateTo = end;

        $("#table-audit-date-range span").html(start.format(app.config.mask.date.display) + ' - ' + end.format(app.config.mask.date.display));
        $("#table-audit-results").hide();
        $("#table-audit-results-charts").hide();
    });
}

/**
 * Get list of Reasons to Publish
 */


app.tableaudit.ajax.readReason = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Reason_API.Read",
        { LngIsoCode: app.label.language.iso.code },
        "app.tableaudit.callback.readReason");

    // Enable 
    $("#select-card").find("[name=select-reason]").prop('disabled', false);

    $("#select-card").find("[name=select-reason]").on('select2:select', function (e) {
        $("#table-audit-results").hide();
        $("#table-audit-results-charts").hide();

    });

    $("#select-card").find("[name=select-reason]").on('select2:unselect', function (e) {
        $("#table-audit-results").hide();
        $("#table-audit-results-charts").hide();
    });

};

/**
 * Populate Reasons dropdown
 * @param  {} data
 */
app.tableaudit.callback.readReason = function (data) {
    $("#select-card").find("[name=select-reason]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.tableaudit.callback.mapReasonData(data)
    });

    // Enable and Focus Search input
    $("#select-card").find("[name=select-reason]").prop('disabled', false)

    app.tableaudit.ajax.readGroup();
};

/**
 * Return formatted option for select 
 * @param  {} dataAPI
 */
app.tableaudit.callback.mapReasonData = function (dataAPI) {
    //  var test = data
    $.each(dataAPI, function (i, item) {
        dataAPI[i].id = item.RsnCode;
        dataAPI[i].text = item.RsnValueExternal + " (" + item.RsnValueInternal + ")";
    });
    return dataAPI;
};

/**
 * Get list of Group
 */

app.tableaudit.ajax.readGroup = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Group_API.Read",
        { CcnUsername: null },
        "app.tableaudit.callback.readGroup");
};

/**
 * Populate group dropdown
 * @param  {} data
 */

app.tableaudit.callback.readGroup = function (data) {
    $("#select-card").find("[name=select-group]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.tableaudit.callback.mapGroupData(data)
    });

    // Enable 
    $("#select-card").find("[name=select-group]").prop('disabled', false);

    $("#select-card").find("[name=select-group]").on('select2:select', function (e) {
        $("#table-audit-results").hide();
        $("#table-audit-results-charts").hide();

    });

    $("#select-card").find("[name=select-group]").on('select2:unselect', function (e) {
        $("#table-audit-results").hide();
        $("#table-audit-results-charts").hide();
    });
};

/**
 * Return formatted option for product dropdown
 * @param  {} dataAPI
 */
app.tableaudit.callback.mapGroupData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        dataAPI[i].id = item.GrpCode;
        dataAPI[i].text = item.GrpCode + " (" + item.GrpName + ")";
    });
    return dataAPI;
}

//#endregion

/**
 * Read table audit
 */

app.tableaudit.ajax.readTableAudit = function () {

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Report.TableAudit_API.Read",
        {
            "DateFrom": app.tableaudit.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.tableaudit.dateTo.format(app.config.mask.date.ajax),
            "GrpCode": $("#select-card").find("[name=select-group]").val() || null,
            "RsnCode": $("#select-card").find("[name=select-reason]").val() || null
        },
        "app.tableaudit.callback.readTableAudit",
        null,
        null,
        null,
        { async: false }
    );
};



/**
 * Draw table audit datatable
 * @param  {} data
 */
app.tableaudit.callback.readTableAudit = function (data) {
    $("#table-audit-results").fadeIn();

    app.tableaudit.result = data;
    $("#table-audit-results-charts").show();
    app.tableaudit.render.reasonsChart();
    app.tableaudit.render.exceptionalChart();

    if ($.fn.dataTable.isDataTable("#table-audit-data table")) {
        app.library.datatable.reDraw("#table-audit-data table", data);
    } else {

        var localOptions = {
            order: [[3, 'desc'], [1, 'asc']],
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
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
                        return app.library.html.link.baseConstructor({ idn: row.MtrCode }, row.MtrCode, row.MtrTitle);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.baseConstructor({ idn: row.RlsVersion }, row.RlsVersion + "." + row.RlsRevision);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.RlsLiveDatetimeFrom ? moment(row.RlsLiveDatetimeFrom).format(app.config.mask.datetime.display) : "";
                    }
                },
                {
                    data: null,
                    type: "natural",
                    render: function (data, type, row) {
                        return app.library.html.boolean(!row.RlsExceptionalFlag, true, true);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.baseConstructor({ idn: row.FrqValue }, row.FrqValue, row.FrqCode);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.baseConstructor({ idn: row.GrpName }, row.GrpName, row.GrpCode);
                    }
                },
                {
                    data: null,
                    defaultContent: '',
                    searchable: false,
                    render: function (data, type, row, meta) {
                        return $("<a>", {
                            href: "#",
                            name: C_APP_DATATABLE_EXTRA_INFO_LINK,
                            "idn": meta.row,
                            html:
                                $("<i>", {
                                    "class": "fas fa-info-circle text-info"
                                }).get(0).outerHTML + " " + row.RsnCode
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    sorting: false,
                    render: function (data, type, row, meta) {
                        return app.library.html.link.edit({ idn: meta.row }, app.label.static["information"]);
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
                    data: "MtrNote",
                    "visible": false,
                    "searchable": true
                }
            ],
            drawCallback: function (settings) {
                api.spinner.stop();
                app.tableaudit.drawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#table-audit-data table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.tableaudit.drawCallback();
        });

    }

};

/**
 * Draw Callback for Datatable
 */
app.tableaudit.drawCallback = function () {
    $('[data-toggle="tooltip"]').tooltip();
    $("#table-audit-data").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        var idn = $(this).attr("idn");
        var rowIndexObj = $("[" + C_APP_DATATABLE_ROW_INDEX + "='" + idn + "']");

        //get data to display
        var dataTable = $("#table-audit-data table").DataTable();
        var dataTableRow = dataTable.row(rowIndexObj);
        var data = dataTableRow.data();

        $("#table-audit-info-modal").find("[name=mtr-title]").text(data.MtrCode);
        $("#table-audit-info-modal").find("[name=table-audit-footnote-info]").html(app.library.html.parseBbCode(data.MtrNote));
        $("#table-audit-info-modal").modal("show");
    });
    // Extra Info
    app.library.datatable.showExtraInfo("#table-audit-data table", app.tableaudit.drawExtraInformation);

};

/**
 * Draw extra information after you click on description link
 * @param  {*} data
 */
app.tableaudit.drawExtraInformation = function (data) {


    //clone template from html not reuse dynamically
    var requestGrid = $("#table-audit-reason-templates").find("[name=table-audit-reason-div-description]").clone();
    requestGrid.removeAttr('id');
    requestGrid.find("[name=reason-div-description-int-desc]").empty().html(app.library.html.parseBbCode(data.RsnValueInternal));
    requestGrid.find("[name=reason-div-description-ext-desc]").empty().html(app.library.html.parseBbCode(data.RsnValueExternal));
    return requestGrid.show().get(0).outerHTML;
};

//#endregion

app.tableaudit.callback.downloadResults = function () {
    var datePicker = $("#table-audit-date-range").data('daterangepicker');
    var startDate = datePicker.startDate ? moment(datePicker.startDate).format(app.config.mask.datetime.file) : "";
    var endDate = datePicker.endDate ? moment(datePicker.endDate).format(app.config.mask.datetime.file) : "";
    var exportFileName = 'tableaudit'
        + "_" + startDate
        + "_" + endDate
        + "." + moment().format(app.config.mask.datetime.file);

    var jsonToCSV = {
        "fields": [
            app.label.static["table"],
            app.label.static["title"],
            app.label.static["release"],
            app.label.static["update-date"],
            app.label.static["scheduled"],
            app.label.static["frequency-code"],
            app.label.static["frequency-value"],
            app.label.static["group-code"],
            app.label.static["group-name"],
            app.label.static["reason-code"],
            app.label.static["reason-value-internal"],
            app.label.static["reason-value-external"]
        ],
        "data": [
        ]
    };

    //order csv data by update date
    var recorderForSorting = $.extend(true, [], app.tableaudit.result);
    var sortedRecords = recorderForSorting.sort(function (a, b) {
        // Turn your strings into dates, and then subtract them
        // to get a value that is either negative, positive, or zero.
        return new Date(b.RlsLiveDatetimeFrom) - new Date(a.RlsLiveDatetimeFrom);
    });

    $.each(sortedRecords, function (index, value) {
        jsonToCSV.data.push(
            [
                value.MtrCode,
                value.MtrTitle,
                value.RlsVersion + "." + value.RlsRevision,
                value.RlsLiveDatetimeFrom ? moment(value.RlsLiveDatetimeFrom).format(app.config.mask.datetime.display) : "",
                !value.RlsExceptionalFlag,
                value.FrqCode,
                value.FrqValue,
                value.GrpCode,
                value.GrpName,
                value.RsnCode,
                value.RsnValueInternal,
                value.RsnValueExternal
            ]
        );
    });
    // Download the file
    app.library.utility.download(exportFileName, Papa.unparse(jsonToCSV, { quotes: true }), C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
};



/**
 * Draw reasons pie chart
 * @param  {} data
 * @param  {} selector
 */
app.tableaudit.render.reasonsChart = function () {


    var allReasons = [];

    $.each(app.tableaudit.result, function (index, value) {
        allReasons.push(value.RsnCode)
    });

    var uniqueReasons = [];
    $.each(allReasons, function (index, value) {
        if (!uniqueReasons.includes(value)) {
            uniqueReasons.push(value);
        }
    });
    var data = [];
    $.each(uniqueReasons, function (index, value) {
        var count = 0;

        $.each(app.tableaudit.result, function (index2, value2) {
            if (value == value2.RsnCode) {
                count++
            }
        });
        data.push(count);

    });


    $("#table-audit-results-reasons-pie").empty();
    $("#table-audit-results-reasons-pie").append(
        $("<canvas>", {
            "name": "pie-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );

    var localConfig = {
        "type": 'pie',
        "data": {
            "datasets": [{
                "data": data,
                "label": app.label.static["reasons"]
            }],
            "labels": uniqueReasons
        },
        "options": {
            "tooltips": {
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        label += ': ' + data.datasets[0].data[tooltipItem.index];
                        return label;
                    }
                }
            },
            "plugins": {
                "colorschemes": {
                    "scheme": "tableau.Tableau10"
                }
            }
        }

    };

    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    delete config.options.scales
    new Chart($("#table-audit-results-reasons-pie").find("[name=pie-chart-canvas]"), config);

};


/**
 * Draw exceptional pie chart
 * @param  {} data
 * @param  {} selector
 */
app.tableaudit.render.exceptionalChart = function () {

    var countExceptional = 0;
    var countNonExceptional = 0;

    $.each(app.tableaudit.result, function (index, value) {
        if (value.RlsExceptionalFlag) {
            countExceptional++
        }
        else {
            countNonExceptional++
        }
    });

    $("#table-audit-results-exceptional-pie").empty();
    $("#table-audit-results-exceptional-pie").append(
        $("<canvas>", {
            "name": "pie-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );

    var localConfig = {
        "type": 'pie',
        "data": {
            "datasets": [{
                "data": [countNonExceptional, countExceptional],
                "label": app.label.static["exceptional"],
                "backgroundColor": ["#019E79", "#e50000"]
            }],
            "labels": [app.label.static["scheduled"], app.label.static["exceptional"]]
        },
        "options": {
            "tooltips": {
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        label += ': ' + data.datasets[0].data[tooltipItem.index];
                        return label;
                    }
                }
            }
        }

    };

    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    delete config.options.scales
    new Chart($("#table-audit-results-exceptional-pie").find("[name=pie-chart-canvas]"), config);


};

//#endregion
