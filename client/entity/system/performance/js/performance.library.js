/*******************************************************************************
Custom JS application specific // 
*******************************************************************************/
//#region Namespaces definitions
app.performance = {};
app.performance.ajax = {};
app.performance.callback = {};
app.performance.chartOptions = {
    tooltips: {
        callbacks: {
            label: function (tooltipItem, data) {
                var label = "";
                label = " " + data.labels[tooltipItem.index];
                label += ': ';
                var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index] === null ? data.null : app.library.utility.formatNumber(data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index]);
                label += value;
                return label.trim();
            }
        }
    },
    scales: {
        xAxes: [{
            ticks: {
                maxTicksLimit: null
            },
            gridLines: {
                display: false
            }
        }],
        yAxes: [{
            ticks: {
                beginAtZero: true
            },
            display: true,
            scaleLabel: {
                display: true,
                labelString: null
            }
        }]
    },
    plugins: {
        colorschemes: {
            scheme: "tableau.Tableau10"
        }
    }
};

////#endregion


//#region input options

/**
 * Set up date picker
 */
app.performance.setDatePicker = function () {
    var start = moment().startOf('day');
    var end = moment();
    $("#performance-input").find("[name=input-date-range]").daterangepicker({
        "opens": 'left',
        "startDate": start.format(app.config.mask.date.dateRangePicker),
        "endDate": end.format(app.config.mask.date.dateRangePicker),
        "maxSpan": {
            "days": 31
        },
        "autoUpdateInput": true,
        "locale": app.label.plugin.daterangepicker
    }, function (start, end) {
        app.performance.ajax.read();
    });
    // Read performance at start application with default datePicker range]
    app.performance.ajax.read();
};

// Ajax Calls for performance data & rendering charts
app.performance.ajax.read = function (data) {
    app.performance.ajax.readProcessorUtilisation();
    app.performance.ajax.readMemoryAvailable();
};
//#endregion



////#region Ajax
app.performance.ajax.readProcessorUtilisation = function () {
    var datePicker = $("#performance-input").find("[name=input-date-range]").data('daterangepicker');
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Performance_API.ReadPrfProcessorPercentage",
        {
            "PrfDatetimeStart": moment(datePicker.startDate).format(app.config.mask.datetime.ajax),
            "PrfDatetimeEnd": moment(datePicker.endDate).format(app.config.mask.datetime.ajax)
        },
        "app.performance.callback.readProcessorUtilisation",
        null,
        null,
        null,
        { async: false });
}
app.performance.callback.readProcessorUtilisation = function (response) {
    $("#performance-charts").find("[name=chart-processor-utilisation]").empty();
    $("#performance-charts").find("[name=chart-processor-utilisation]").append(
        $("<canvas>", {
            "name": "chart-processor-utilisation-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localOptions = $.extend(true, {}, app.performance.chartOptions);
    localOptions.scales.yAxes[0].scaleLabel.labelString = "%";
    if (response.datetime.length > 24) {
        localOptions.scales.xAxes[0].ticks.maxTicksLimit = 24;
    };
    var chartOptions = {
        type: 'line',
        data: {
            labels: response.datetime,
            datasets: []
        },
        options: localOptions
    };
    $.each(response.server, function (index, value) {
        var dataset = {
            data: value[Object.keys(value)[0]],
            label: Object.keys(value)[0],
            fill: false,
            showLine: false
        };
        chartOptions.data.datasets.push(dataset)

    });
    new Chart($("#performance-charts").find("[name=chart-processor-utilisation-canvas]"), chartOptions);
}

app.performance.ajax.readMemoryAvailable = function () {
    var datePicker = $("#performance-input").find("[name=input-date-range]").data('daterangepicker');
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Security.Performance_API.ReadPrfMemoryAvailable",
        {
            "PrfDatetimeStart": moment(datePicker.startDate).format(app.config.mask.datetime.ajax),
            "PrfDatetimeEnd": moment(datePicker.endDate).format(app.config.mask.datetime.ajax)
        },
        "app.performance.callback.readMemoryAvailable",
        null,
        null,
        null,
        { async: false });
}
app.performance.callback.readMemoryAvailable = function (response) {
    $("#performance-charts").find("[name=chart-memory-available]").empty();
    $("#performance-charts").find("[name=chart-memory-available]").append(
        $("<canvas>", {
            "name": "chart-memory-available-canvas",
            "style": "width: 100%; height: 400px"
        })
    );
    var localOptions = $.extend(true, {}, app.performance.chartOptions);
    localOptions.scales.yAxes[0].scaleLabel.labelString = "MB";
    if (response.datetime.length > 24) {
        localOptions.scales.xAxes[0].ticks.maxTicksLimit = 24;
    };
    var chartOptions = {
        type: 'line',
        data: {
            labels: response.datetime,
            datasets: []
        },
        options: localOptions
    };
    $.each(response.server, function (index, value) {
        var dataset = {
            data: value[Object.keys(value)[0]],
            label: Object.keys(value)[0],
            fill: false,
            showLine: false
        };
        chartOptions.data.datasets.push(dataset)

    });
    new Chart($("#performance-charts").find("[name=chart-memory-available-canvas]"), chartOptions);
}


////#endregion Ajax

//#region flush Server
app.performance.ajax.flushServer = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Performance_API.Delete",
        {},
        "app.performance.callback.flushServer",
        null,
        null,
        null
    );
};
app.performance.callback.flushServer = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#modal-success").on('hide.bs.modal', function (e) {

            window.location.href = window.location.pathname;
        });
        api.modal.success(app.label.static["success-performance-flush"]);

    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

//#endregion
