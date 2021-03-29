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

app.analytic.dateFrom = moment().subtract(app.config.entity.analytic.dateRangePicker, 'days'); // Date type, not String
app.analytic.dateTo = moment(); // Date type, not String
//#endregion

//#region set up form
/**
 * Set up date range picker
 */
app.analytic.setDatePicker = function () {
    $("#analytic-date-range span").html(app.analytic.dateFrom.format(app.config.mask.date.display) + ' - ' + app.analytic.dateTo.format(app.config.mask.date.display));

    $("#analytic-date-range").daterangepicker({
        startDate: app.analytic.dateFrom,
        endDate: app.analytic.dateFrom,
        maxDate: new Date(),
        ranges: {
            [app.label.static["today"]]: [moment(), moment()],
            [app.label.static["yesterday"]]: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-7-days"]]: [moment().subtract(6, 'days'), moment()],
            [app.label.static["last-30-days"]]: [moment().subtract(29, 'days'), moment()],
            [app.label.static["last-365-days"]]: [moment().subtract(364, 'days'), moment()],
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
}

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
    $("#select-card").find("[name=select-subject]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.analytic.callback.mapSubjectData(data)
    });

    // Enable and Focus Seach input
    $("#select-card").find("[name=select-subject]").prop('disabled', false)

    $("#select-card").find("[name=select-subject]").on('select2:select', function (e) {
        $("#analytic-results").hide();
        app.analytic.ajax.readProduct();
    });

    $("#select-card").find("[name=select-subject]").on('select2:unselect', function (e) {
        $("#select-card").find("[name=select-product]").empty();
        // Disable product 
        $("#select-card").find("[name=select-product]").prop('disabled', true);
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
    var SbjCode = $("#select-card").find("[name=select-subject]").val();
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
    $("#select-card").find("[name=select-product]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.analytic.callback.mapProductData(data)
    });

    // Enable 
    $("#select-card").find("[name=select-product]").prop('disabled', false);

    $("#select-card").find("[name=select-product]").on('select2:select', function (e) {
        $("#analytic-results").hide();
        app.analytic.PrcCode = e.params.data.PrcCode;
    });

    $("#select-card").find("[name=select-product]").on('select2:unselect', function (e) {
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
    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
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
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readAnalytics",
        null,
        null,
        null,
        { async: false }
    );
    app.analytic.ajax.readBrowser(null, "#analytic-chart [name=browser-pie-chart]");
    app.analytic.ajax.readOs(null, "#analytic-chart [name=operating-system-pie-chart]");
    app.analytic.ajax.readReferrer(null, "#analytic-chart [name=referrer-column-chart]");
    app.analytic.ajax.readUserLanguage(null, "#analytic-chart [name=user-language-column-chart]");
    app.analytic.ajax.readTimeline(null, "#analytic-chart [name=dates-line-chart]");
    app.analytic.ajax.readLanguage(null, "#analytic-chart [name=language-pie-chart]");
    app.analytic.ajax.readFormat(null, "#analytic-chart [name=format-pie-chart]");
    $("#analytic-results").fadeIn();

};

/**
 * Draw Callback for Datatable
 */
app.analytic.drawCallback = function () {
    $('[data-toggle="tooltip"]').tooltip();
    $("#analytic-data").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        app.analytic.ajax.readTimeline($(this).attr("idn"), "#analytic-chart-modal [name=dates-line-chart]");
        app.analytic.ajax.readReferrer($(this).attr("idn"), "#analytic-chart-modal [name=referrer-column-chart]");
        app.analytic.ajax.readUserLanguage($(this).attr("idn"), "#analytic-chart-modal [name=user-language-column-chart]");
        app.analytic.ajax.readBrowser($(this).attr("idn"), "#analytic-chart-modal [name=browser-pie-chart]");
        app.analytic.ajax.readOs($(this).attr("idn"), "#analytic-chart-modal [name=operating-system-pie-chart]");
        app.analytic.ajax.readLanguage($(this).attr("idn"), "#analytic-chart-modal [name=language-pie-chart]");
        app.analytic.ajax.readFormat($(this).attr("idn"), "#analytic-chart-modal [name=format-pie-chart]");
        $("#matrix-chart-modal").find("[name=mtr-title]").text($(this).attr("idn") + " : " + $(this).attr("data-original-title"));
        $("#matrix-chart-modal").find("[name=date-range]").html(app.analytic.dateFrom.format(app.config.mask.date.display)
            + "    " + " - " + app.analytic.dateTo.format(app.config.mask.date.display));
        $("#matrix-chart-modal").modal("show");
    });
}

/**
 * Draw analytics datatable
 * @param  {} data
 */
app.analytic.callback.readAnalytics = function (data) {
    var datePicker = $("#analytic-date-range").data('daterangepicker');
    var startDate = datePicker.startDate ? moment(datePicker.startDate).format(app.config.mask.datetime.file) : "";
    var endDate = datePicker.endDate ? moment(datePicker.endDate).format(app.config.mask.datetime.file) : "";
    var exportFileName = 'analytic'
        + "_" + startDate
        + "_" + endDate
        + "." + moment().format(app.config.mask.datetime.file);

    if ($.fn.dataTable.isDataTable("#analytic-data table")) {
        app.library.datatable.reDraw("#analytic-data table", data);
    } else {

        var localOptions = {
            data: data,
            buttons: [{
                extend: 'csv',
                title: exportFileName,
                exportOptions: {
                    format: {
                        body: function (data, row, column, node) {
                            // Strip HTML
                            return data.toString().replace(C_APP_REGEX_NOHTML, "");
                        }
                    }
                }
            }],
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ idn: row.MtrCode }, row.MtrCode, row.MtrTitle);
                    }
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

        // Invoke DataTables CSV export
        // https://stackoverflow.com/questions/45515559/how-to-call-datatable-csv-button-from-custom-button
        $("#analytic-data").find("[name=csv]").once("click", function () {
            $("#analytic-data table").DataTable().button('.buttons-csv').trigger();
        });
    }
    var totalBot = 0;
    var totalM2M = 0;
    var totalUsers = 0;
    $("#summary-card").find("[name=analytic-sum-bots]").text(function () {
        $.each(data, function (index, value) {
            totalBot = totalBot + value.NltBot;
        });
        return app.library.utility.formatNumber(totalBot)
    });
    $("#summary-card").find("[name=analytic-sum-m2m]").text(function () {
        $.each(data, function (index, value) {
            totalM2M = totalM2M + value.NltM2m;
        });
        return app.library.utility.formatNumber(totalM2M)
    });
    $("#summary-card").find("[name=analytic-sum-users]").text(function () {
        $.each(data, function (index, value) {
            totalUsers = totalUsers + value.NltUser;
        });
        return app.library.utility.formatNumber(totalUsers)
    });
    $("#summary-card").find("[name=analytic-sum-totals]").text(app.library.utility.formatNumber(totalBot + totalM2M + totalUsers));
};
//#endregion

//#region browser

/**
 * Get browser analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readBrowser = function (MtrCode, selector) {
    MtrCode = MtrCode || null;
    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadBrowser",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val(),
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
                    "scheme": "tableau.Traffic9"
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
app.analytic.ajax.readOs = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadOs",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val(),
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
                    "scheme": "office.Parallax6"
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
app.analytic.ajax.readReferrer = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadReferrer",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val(),
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
app.analytic.ajax.readUserLanguage = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadEnvironmentLanguage",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val(),
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
app.analytic.ajax.readLanguage = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadLanguage",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
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
                    "scheme": "office.Oriel6"
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
app.analytic.ajax.readTimeline = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadTimeline",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val(),
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
        users.data.push(el.NltUser);
        total.data.push(el.Total);
        localConfig.data.labels.push(el.date ? moment(el.date).format(app.config.mask.date.display) : "");
    });
    localConfig.data.datasets = [bots, M2M, users, total];

    var config = $.extend(true, {}, app.config.plugin.chartJs.chart, localConfig);
    new Chart($(selector).find("[name=dates-line-chart-canvas]"), config);
};

//#endregion

//#region validation

/**
 * Validation
 */
app.analytic.validation.select = function () {

    $("#select-card").find("form").trigger("reset").validate({
        rules: {
            "nlt-masked-ip":
            {
                validIpMask: true
            },
        },
        errorPlacement: function (error, element) {
            $("#select-card").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
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
app.analytic.ajax.readFormat = function (MtrCode, selector) {
    MtrCode = MtrCode || null;
    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Analytic_API.ReadFormat",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
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
                    "scheme": "office.Opulent6"
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