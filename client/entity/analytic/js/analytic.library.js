/*******************************************************************************
Custom JS application specific group.library.js
*******************************************************************************/
//#region Namespaces definitions
// Add Namespace Group Data Table
app.analytic = {};
app.analytic.ajax = {};
app.analytic.callback = {};
app.analytic.render = {};
app.analytic.validation = {};
app.analytic.result = [];

app.analytic.dateFrom = moment().subtract(app.config.entity.analytic.dateRangePicker, 'days'); // Date type, not String
app.analytic.dateTo = moment().subtract(1, 'days'); // Date type, not String. always default to yesterday 

app.analytic.dateFromModal = moment().subtract(app.config.entity.analytic.dateRangePicker, 'days'); // Date type, not String
app.analytic.dateToModal = moment().subtract(1, 'days'); // Date type, not String. always default to yesterday 
app.analytic.MtrCode = null;

//#endregion

//#region set up form
/**
 * Set up date range picker
 */
app.analytic.setDatePicker = function () {
    $("#analytic-date-range span").html(app.analytic.dateFrom.format(app.config.mask.date.display) + ' - ' + app.analytic.dateTo.format(app.config.mask.date.display));

    $("#analytic-date-range").daterangepicker({
        startDate: app.analytic.dateFrom,
        endDate: app.analytic.dateTo,
        maxDate: app.analytic.dateTo,
        ranges: {
            [app.label.static["today"]]: [moment(), moment()],
            [app.label.static["yesterday"]]: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-7-days"]]: [moment().subtract(7, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-30-days"]]: [moment().subtract(30, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-365-days"]]: [moment().subtract(365, 'days'), moment().subtract(1, 'days')],
            // [app.label.static["this-month"]]: [moment().startOf('month'), moment().endOf('month')],
            // [app.label.static["last-month"]]: [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        locale: app.label.plugin.daterangepicker
    }, function (start, end) {
        // Override default values with the selection
        app.analytic.dateFrom = start;
        app.analytic.dateTo = end;

        $("#analytic-date-range span").html(start.format(app.config.mask.date.display) + ' - ' + end.format(app.config.mask.date.display));
        $("#analytic-results").hide();
    });
};

/**
 * Set up date range picker
 */
app.analytic.setDatePickerModal = function () {
    $("#analytic-date-range-modal span").html(app.analytic.dateFrom.format(app.config.mask.date.display) + ' - ' + app.analytic.dateTo.format(app.config.mask.date.display));
    $("#matrix-chart-modal").find("[name=date-range]").html(app.analytic.dateFrom.format(app.config.mask.date.display)
        + "    " + " - " + app.analytic.dateTo.format(app.config.mask.date.display));

    $("#analytic-date-range-modal").daterangepicker({
        startDate: app.analytic.dateFrom,
        endDate: app.analytic.dateTo,
        maxDate: app.analytic.dateTo,
        ranges: {
            [app.label.static["today"]]: [moment(), moment()],
            [app.label.static["yesterday"]]: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-7-days"]]: [moment().subtract(7, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-30-days"]]: [moment().subtract(30, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-365-days"]]: [moment().subtract(365, 'days'), moment().subtract(1, 'days')],
            // [app.label.static["this-month"]]: [moment().startOf('month'), moment().endOf('month')],
            // [app.label.static["last-month"]]: [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        locale: app.label.plugin.daterangepicker
    }, function (start, end) {
        $("#matrix-chart-modal").find("[name=modal-results]").hide();
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-bots]").empty();
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-m2m]").empty();
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-widgets]").empty();
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-users]").empty();
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-totals]").empty();
        $("#analytic-chart-modal [name=dates-line-chart]").empty();
        $("#analytic-chart-modal [name=referrer-column-chart-canvas]").empty();
        $("#analytic-chart-modal [name=browser-pie-chart-canvas]").empty();
        $("#analytic-chart-modal [name=operating-system-pie-chart-canvas]").empty();
        $("#analytic-chart-modal [name=language-pie-chart-canvas]").empty();
        $("#analytic-chart-modal [name=format-pie-chart-canvas]").empty();
        $("#analytic-chart-modal [name=user-language-column-chart-canvas]").empty();
        // Override default values with the selection
        app.analytic.dateFromModal = start;
        app.analytic.dateToModal = end;
        $("#analytic-date-range-modal span").html(start.format(app.config.mask.date.display) + ' - ' + end.format(app.config.mask.date.display));
        $("#matrix-chart-modal").find("[name=date-range]").html(start.format(app.config.mask.date.display)
            + "    " + " - " + end.format(app.config.mask.date.display));
    });
    app.analytic.dateFromModal = app.analytic.dateFrom;
    app.analytic.dateToModal = app.analytic.dateTo;
    app.analytic.drawCallback.drawModalResults();
};

/**
 * Get list of subjects
 */
app.analytic.ajax.readSubject = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Subject_API.Read",
        { SbjCode: null },
        "app.analytic.callback.readSubject");
};

/**
 * Populate subjects dropdown
 * @param  {} data
 */
app.analytic.callback.readSubject = function (data) {
    $("#analytic-select-card").find("[name=select-subject]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.analytic.callback.mapSubjectData(data)
    });

    // Enable and Focus Seach input
    $("#analytic-select-card").find("[name=select-subject]").prop('disabled', false)

    $("#analytic-select-card").find("[name=select-subject]").on('select2:select', function (e) {
        $("#analytic-results").hide();
        app.analytic.ajax.readProduct();
    });

    $("#analytic-select-card").find("[name=select-subject]").on('select2:unselect', function (e) {
        $("#analytic-select-card").find("[name=select-product]").empty();
        // Disable product 
        $("#analytic-select-card").find("[name=select-product]").prop('disabled', true);
        $("#analytic-results").hide();
    });
};

/**
 * Return formatted option for select 
 * @param  {} dataAPI
 */
app.analytic.callback.mapSubjectData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        dataAPI[i].id = item.SbjCode;
        dataAPI[i].text = item.SbjValue + " (" + item.SbjValue + ")";
    });
    return dataAPI;
}

/**
 * Get list of products for the subject
 */
app.analytic.ajax.readProduct = function () {
    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Product_API.Read",
        { SbjCode: SbjCode },
        "app.analytic.callback.readProduct");
};

/**
 * Populate products dropdown
 * @param  {} data
 */
app.analytic.callback.readProduct = function (data) {
    $("#analytic-select-card").find("[name=select-product]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.analytic.callback.mapProductData(data)
    });

    // Enable 
    $("#analytic-select-card").find("[name=select-product]").prop('disabled', false);

    $("#analytic-select-card").find("[name=select-product]").on('select2:select', function (e) {
        $("#analytic-results").hide();
        app.analytic.PrcCode = e.params.data.PrcCode;
    });

    $("#analytic-select-card").find("[name=select-product]").on('select2:unselect', function (e) {
        $("#analytic-results").hide();
        app.analytic.PrcCode = null;
    });
};

/**
 * Return formatted option for product dropdown
 * @param  {} dataAPI
 */
app.analytic.callback.mapProductData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        dataAPI[i].id = item.PrcCode;
        dataAPI[i].text = item.PrcCode + " (" + item.PrcValue + ")";
    });
    return dataAPI;
}

//#endregion

//#region get table data

/**
 * Read analytics
 */
app.analytic.ajax.readAnalytics = function () {
    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#analytic-select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.spinner.start();
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.Read",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "NltInternalNetworkMask": $("#analytic-select-card").find("[name=nlt-masked-ip]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.analytic.callback.readAnalytics",
        null,
        null,
        null,
        { async: false }
    );
    app.analytic.ajax.readBrowser(null, "#analytic-chart [name=browser-pie-chart]", app.analytic.dateFrom, app.analytic.dateTo);
    app.analytic.ajax.readOs(null, "#analytic-chart [name=operating-system-pie-chart]", app.analytic.dateFrom, app.analytic.dateTo);
    app.analytic.ajax.readReferrer(null, "#analytic-chart [name=referrer-column-chart]", app.analytic.dateFrom, app.analytic.dateTo);
    app.analytic.ajax.readUserLanguage(null, "#analytic-chart [name=user-language-column-chart]", app.analytic.dateFrom, app.analytic.dateTo);
    app.analytic.ajax.readTimeline(null, "#analytic-chart [name=dates-line-chart]", app.analytic.dateFrom, app.analytic.dateTo);
    app.analytic.ajax.readLanguage(null, "#analytic-chart [name=language-pie-chart]", app.analytic.dateFrom, app.analytic.dateTo);
    app.analytic.ajax.readFormat(null, "#analytic-chart [name=format-pie-chart]", app.analytic.dateFrom, app.analytic.dateTo);
    $("#analytic-results").fadeIn();

};

/**
 * Draw Callback for Datatable
 */
app.analytic.drawCallback = function () {
    $('[data-bs-toggle="tooltip"]').tooltip();
    $("#analytic-data").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        app.analytic.MtrCode = $(this).attr("idn");
        app.analytic.setDatePickerModal();
    });
}

app.analytic.drawCallback.drawModalResults = function () {
    app.analytic.ajax.readTimeline(app.analytic.MtrCode, "#analytic-chart-modal [name=dates-line-chart]", app.analytic.dateFromModal, app.analytic.dateToModal);
    app.analytic.ajax.readReferrer(app.analytic.MtrCode, "#analytic-chart-modal [name=referrer-column-chart]", app.analytic.dateFromModal, app.analytic.dateToModal);
    app.analytic.ajax.readUserLanguage(app.analytic.MtrCode, "#analytic-chart-modal [name=user-language-column-chart]", app.analytic.dateFromModal, app.analytic.dateToModal);
    app.analytic.ajax.readBrowser(app.analytic.MtrCode, "#analytic-chart-modal [name=browser-pie-chart]", app.analytic.dateFromModal, app.analytic.dateToModal);
    app.analytic.ajax.readOs(app.analytic.MtrCode, "#analytic-chart-modal [name=operating-system-pie-chart]", app.analytic.dateFromModal, app.analytic.dateToModal);
    app.analytic.ajax.readLanguage(app.analytic.MtrCode, "#analytic-chart-modal [name=language-pie-chart]", app.analytic.dateFromModal, app.analytic.dateToModal);
    app.analytic.ajax.readFormat(app.analytic.MtrCode, "#analytic-chart-modal [name=format-pie-chart]", app.analytic.dateFromModal, app.analytic.dateToModal);
    $("#matrix-chart-modal").find("[name=mtr-code]").text(app.analytic.MtrCode);
    $("#matrix-chart-modal").modal("show");
    $("#matrix-chart-modal").find("[name=modal-results]").show();
};

/**
 * Draw analytics datatable
 * @param  {} data
 */
app.analytic.callback.readAnalytics = function (data) {
    app.analytic.result = data;

    if ($.fn.dataTable.isDataTable("#analytic-data table")) {
        app.library.datatable.reDraw("#analytic-data table", data);
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
                        return app.library.html.link.edit({ idn: row.MtrCode }, row.MtrCode);
                    },
                    orderData: [0]
                },
                { data: "SbjValue" },

                {
                    data: null,
                    render: function (data, type, row) {
                        return row.PrcCode + "(" + row.PrcValue + ")";

                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.PublishDate ? moment(row.PublishDate).format(app.config.mask.datetime.display) : "";
                    }
                },
                { data: "NltBot" },
                { data: "NltM2m" },
                { data: "NltWidget" },
                { data: "NltUser" },
                { data: "Total" }
            ],
            drawCallback: function (settings) {
                api.spinner.stop();
                app.analytic.drawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#analytic-data table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.analytic.drawCallback();
        });

    }
    var totalBot = 0;
    var totalWidgets = 0;
    var totalM2M = 0;
    var totalUsers = 0;
    $("#analytic-results").find("[name=summary-card]").find("[name=analytic-sum-bots]").text(function () {
        $.each(data, function (index, value) {
            totalBot = totalBot + value.NltBot;
        });
        return app.library.utility.formatNumber(totalBot)
    });
    $("#analytic-results").find("[name=summary-card]").find("[name=analytic-sum-widgets]").text(function () {
        $.each(data, function (index, value) {
            totalWidgets = totalWidgets + value.NltWidget;
        });
        return app.library.utility.formatNumber(totalWidgets)
    });
    $("#analytic-results").find("[name=summary-card]").find("[name=analytic-sum-m2m]").text(function () {
        $.each(data, function (index, value) {
            totalM2M = totalM2M + value.NltM2m;
        });
        return app.library.utility.formatNumber(totalM2M)
    });
    $("#analytic-results").find("[name=summary-card]").find("[name=analytic-sum-users]").text(function () {
        $.each(data, function (index, value) {
            totalUsers = totalUsers + value.NltUser;
        });
        return app.library.utility.formatNumber(totalUsers)
    });
    $("#analytic-results").find("[name=summary-card]").find("[name=analytic-sum-totals]").text(app.library.utility.formatNumber(totalBot + totalM2M + totalUsers + totalWidgets));


    // Scroll to the top section
    $('html, body').animate({
        scrollTop: $("#analytic-results").offset().top
    }, 1000);

};

app.analytic.callback.downloadResults = function () {
    var datePicker = $("#analytic-date-range").data('daterangepicker');
    var startDate = datePicker.startDate ? moment(datePicker.startDate).format(app.config.mask.datetime.file) : "";
    var endDate = datePicker.endDate ? moment(datePicker.endDate).format(app.config.mask.datetime.file) : "";
    var exportFileName = 'analytic'
        + "_" + startDate
        + "_" + endDate
        + "." + moment().format(app.config.mask.datetime.file);

    var jsonToCSV = {
        "fields": [
            app.label.static["table"],
            app.label.static["subject"],
            app.label.static["product"],
            app.label.static["date-published"],
            app.label.static["bots"],
            app.label.static["m2m"],
            app.label.static["widgets"],
            app.label.static["users"],
            app.label.static["total"]
        ],
        "data": [
        ]
    };

    $.each(app.analytic.result, function (index, value) {
        jsonToCSV.data.push(
            [
                value.MtrCode,
                value.SbjValue,
                value.PrcValue,
                value.PublishDate ? moment(value.PublishDate).format(app.config.mask.datetime.display) : "",
                value.NltBot,
                value.NltM2m,
                value.NltWidget,
                value.NltUser,
                value.Total
            ]
        );
    });
    // Download the file
    app.library.utility.download(exportFileName, Papa.unparse(jsonToCSV, { quotes: true }), C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
};
//#endregion

//#region browser

/**
 * Get browser analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readBrowser = function (MtrCode, selector, dateFrom, dateTo) {
    MtrCode = MtrCode || null;
    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#analytic-select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadBrowser",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#analytic-select-card").find("[name=nlt-masked-ip]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.analytic.callback.readBrowser",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw browser pie chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readBrowser = function (data, selector) {
    $(selector).empty();
    $(selector).append(
        $("<canvas>", {
            "name": "browser-pie-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localConfig = {
        "type": 'pie',
        "data": {
            "datasets": [{
                "data": [],
                "label": app.label.static["hits"],
            }],
            "labels": []
        },
        "options": {
            "tooltips": {
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        label += ': ';
                        var totalValues = 0;
                        for (var i = 0; i < data.datasets[tooltipItem.datasetIndex].data.length; i++) {
                            totalValues += data.datasets[tooltipItem.datasetIndex].data[i] << 0;
                        }
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                        var percentage = ((value / totalValues) * 100).toFixed(1);
                        label += app.library.utility.formatNumber(value) + " (" + percentage + "%)";
                        return label;
                    }
                }
            },
            "plugins": {
                "colorschemes": {
                    "scheme": "tableau.Tableau20"
                }
            }
        }
    };

    $.each(data, function (key, el) {
        localConfig.data.datasets[0].data.push(el);
        localConfig.data.labels.push(key);
    });
    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    delete config.options.scales
    new Chart($(selector).find("[name=browser-pie-chart-canvas]"), config);
};


//#endregion

//#region OS

/**
 * Get OS analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readOs = function (MtrCode, selector, dateFrom, dateTo) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#analytic-select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadOs",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#analytic-select-card").find("[name=nlt-masked-ip]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.analytic.callback.readOs",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw Os pie chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readOs = function (data, selector) {
    $(selector).empty();
    $(selector).append(
        $("<canvas>", {
            "name": "operating-system-pie-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localConfig = {
        "type": 'pie',
        "data": {
            "datasets": [{
                "data": [],
                "label": app.label.static["hits"],
            }],
            "labels": []
        },
        "options": {
            "tooltips": {
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        label += ': ';
                        var totalValues = 0;
                        for (var i = 0; i < data.datasets[tooltipItem.datasetIndex].data.length; i++) {
                            totalValues += data.datasets[tooltipItem.datasetIndex].data[i] << 0;
                        }
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                        var percentage = ((value / totalValues) * 100).toFixed(1);
                        label += app.library.utility.formatNumber(value) + " (" + percentage + "%)";
                        return label;
                    }
                }
            },
            "plugins": {
                "colorschemes": {
                    "scheme": "tableau.Tableau20"
                }
            }
        }
    };

    $.each(data, function (key, el) {
        localConfig.data.datasets[0].data.push(el);
        localConfig.data.labels.push(key);
    });

    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    delete config.options.scales
    new Chart($(selector).find("[name=operating-system-pie-chart-canvas]"), config);
};

//#endregion

//#region Referrer

/**
 * Get referrer analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readReferrer = function (MtrCode, selector, dateFrom, dateTo) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#analytic-select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadReferrer",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#analytic-select-card").find("[name=nlt-masked-ip]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.analytic.callback.readReferrer",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw referrer chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readReferrer = function (data, selector) {
    $(selector).empty();
    $(selector).append(
        $("<canvas>", {
            "name": "referrer-column-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localConfig = {
        "type": 'bar',
        "data": {
            "datasets": [{
                "data": [],
                "label": app.label.static["hits"],
            }],
            "labels": []
        },
        "options": {
            "tooltips": {
                "intersect": false,
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        label += ': ';
                        var value = app.library.utility.formatNumber(data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index]);
                        label += value;
                        return label;
                    }
                }
            },
            "plugins": {
                "colorschemes": {
                    "scheme": "tableau.Tableau20"
                }
            }
        }
    };

    $.each(data, function (key, el) {
        localConfig.data.labels.push(key);
        localConfig.data.datasets[0].data.push(el);
    });

    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    config.options.scales.yAxes[0].type = "logarithmic";
    new Chart($(selector).find("[name=referrer-column-chart-canvas]"), config);

};

//#endregion

//#region user language

/**
 * Get referrer analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readUserLanguage = function (MtrCode, selector, dateFrom, dateTo) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#analytic-select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadEnvironmentLanguage",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#analytic-select-card").find("[name=nlt-masked-ip]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.analytic.callback.readUserLanguage",
        selector,
        null,
        null,
        { async: false }
    );

    ;
}

/**
 * Draw referrer chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readUserLanguage = function (data, selector) {
    $(selector).empty();
    $(selector).append(
        $("<canvas>", {
            "name": "user-language-column-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localConfig = {
        "type": 'bar',
        "data": {
            "datasets": [{
                "data": [],
                "label": app.label.static["iso-language"],
            }],
            "labels": []
        },
        "options": {
            "tooltips": {
                "intersect": false,
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        label += ': ';
                        var value = app.library.utility.formatNumber(data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index]);
                        label += value;
                        return label;
                    }
                }
            },
            "plugins": {
                "colorschemes": {
                    "scheme": "tableau.Tableau20"
                }
            }
        }
    };

    $.each(data, function (key, el) {
        localConfig.data.labels.push(key);
        localConfig.data.datasets[0].data.push(el);
    });

    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    config.options.scales.yAxes[0].type = "logarithmic";
    new Chart($(selector).find("[name=user-language-column-chart-canvas]"), config);
};

//#endregion

//#region language

/**
 * Get language analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readLanguage = function (MtrCode, selector, dateFrom, dateTo) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#analytic-select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadLanguage",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#analytic-select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readLanguage",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw language pie chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readLanguage = function (data, selector) {
    $(selector).empty();
    $(selector).append(
        $("<canvas>", {
            "name": "language-pie-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localConfig = {
        "type": 'pie',
        "data": {
            "datasets": [{
                "data": [],
                "label": app.label.static["hits"],
            }],
            "labels": []
        },
        "options": {
            "tooltips": {
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        label += ': ';
                        var totalValues = 0;
                        for (var i = 0; i < data.datasets[tooltipItem.datasetIndex].data.length; i++) {
                            totalValues += data.datasets[tooltipItem.datasetIndex].data[i] << 0;
                        }
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                        var percentage = ((value / totalValues) * 100).toFixed(1);
                        label += app.library.utility.formatNumber(value) + " (" + percentage + "%)";
                        return label;
                    }
                }
            },
            "plugins": {
                "colorschemes": {
                    "scheme": "tableau.Tableau20"
                }
            }
        }
    };

    $.each(data, function (key, el) {
        localConfig.data.datasets[0].data.push(el);
        localConfig.data.labels.push(key);
    });

    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    delete config.options.scales
    new Chart($(selector).find("[name=language-pie-chart-canvas]"), config);
};

//#endregion

//#region timeline

/**
 * Get timeline analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readTimeline = function (MtrCode, selector, dateFrom, dateTo) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#analytic-select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadTimeline",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#analytic-select-card").find("[name=nlt-masked-ip]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.analytic.callback.readTimeline",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw timeline chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readTimeline = function (data, selector) {
    if (selector == "#analytic-chart-modal [name=dates-line-chart]") {
        //draw summary in modal
        var totalBot = 0;
        var totalWidgets = 0;
        var totalM2M = 0;
        var totalUsers = 0;
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-bots]").text(function () {
            $.each(data, function (index, value) {
                totalBot = totalBot + value.NltBot;
            });
            return app.library.utility.formatNumber(totalBot)
        });
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-widgets]").text(function () {
            $.each(data, function (index, value) {
                totalWidgets = totalWidgets + value.NltWidget;
            });
            return app.library.utility.formatNumber(totalWidgets)
        });
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-m2m]").text(function () {
            $.each(data, function (index, value) {
                totalM2M = totalM2M + value.NltM2m;
            });
            return app.library.utility.formatNumber(totalM2M)
        });
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-users]").text(function () {
            $.each(data, function (index, value) {
                totalUsers = totalUsers + value.NltUser;
            });
            return app.library.utility.formatNumber(totalUsers)
        });
        $("#matrix-chart-modal").find("[name=summary-card]").find("[name=analytic-sum-totals]").text(app.library.utility.formatNumber(totalBot + totalM2M + totalUsers + totalWidgets));

    }

    $(selector).empty();
    $(selector).append(
        $("<canvas>", {
            "name": "dates-line-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localConfig = {
        "type": 'line',
        "data": {
            "datasets": [],
            "labels": []
        },
        "options": {
            "tooltips": {
                "mode": "index",
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.datasets[tooltipItem.datasetIndex].label + " - " + data.labels[tooltipItem.index];
                        label += ': ';
                        var value = app.library.utility.formatNumber(data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index]);
                        label += value;
                        return label;
                    }
                }
            },
            "plugins": {
                "colorschemes": {
                    "scheme": "tableau.Tableau20"
                }
            }
        }
    };

    var bots = {
        label: app.label.static["bots"],
        data: [],
        fill: false
    };
    var M2M = {
        label: app.label.static["m2m"],
        data: [],
        fill: false
    };
    var widgets = {
        label: app.label.static["widgets"],
        data: [],
        fill: false
    };
    var users = {
        label: app.label.static["users"],
        data: [],
        fill: false
    };
    var total = {
        label: app.label.static["total"],
        data: [],
        fill: false
    };

    $.each(data, function (key, el) {
        bots.data.push(el.NltBot);
        M2M.data.push(el.NltM2m);
        widgets.data.push(el.NltWidget);
        users.data.push(el.NltUser);
        total.data.push(el.total);
        localConfig.data.labels.push(el.date ? moment(el.date).format(app.config.mask.date.display) : "");
    });
    localConfig.data.datasets = [bots, M2M, widgets, users, total];
    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    new Chart($(selector).find("[name=dates-line-chart-canvas]"), config);
};

//#endregion

//#region validation

/**
 * Validation
 */
app.analytic.validation.select = function () {

    $("#analytic-select-card").find("form").trigger("reset").validate({
        rules: {
            "nlt-masked-ip":
            {
                validIpMask: true
            },
        },
        errorPlacement: function (error, element) {
            $("#analytic-select-card").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.analytic.ajax.readAnalytics();
        }
    }).resetForm();
};

//#endregion

//#region format
/**
 * Get format analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readFormat = function (MtrCode, selector, dateFrom, dateTo) {
    MtrCode = MtrCode || null;
    var SbjCode = $("#analytic-select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#analytic-select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadFormat",
        {
            "DateFrom": dateFrom.format(app.config.mask.date.ajax),
            "DateTo": dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#analytic-select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readFromat",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw fromat pie chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readFromat = function (data, selector) {
    $(selector).empty();
    $(selector).append(
        $("<canvas>", {
            "name": "format-pie-chart-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localConfig = {
        "type": 'pie',
        "data": {
            "datasets": [{
                "data": [],
                "label": app.label.static["hits"],
            }],
            "labels": []
        },
        "options": {
            "tooltips": {
                "callbacks": {
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        label += ': ';
                        var totalValues = 0;
                        for (var i = 0; i < data.datasets[tooltipItem.datasetIndex].data.length; i++) {
                            totalValues += data.datasets[tooltipItem.datasetIndex].data[i] << 0;
                        }
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                        var percentage = ((value / totalValues) * 100).toFixed(1);
                        label += app.library.utility.formatNumber(value) + " (" + percentage + "%)";
                        return label;
                    }
                }
            },
            "plugins": {
                "colorschemes": {
                    "scheme": "tableau.Tableau20"
                }
            }
        }
    };

    $.each(data, function (key, el) {
        localConfig.data.datasets[0].data.push(el);
        localConfig.data.labels.push(key);
    });

    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    delete config.options.scales
    new Chart($(selector).find("[name=format-pie-chart-canvas]"), config);
};


//#endregion