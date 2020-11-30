/*******************************************************************************
Custom JS application specific // 
*******************************************************************************/
//#region Namespaces definitions
app.cache = {};
app.cache.ajax = {};
app.cache.callback = {};
app.cache.render = {};
app.cache.render.chartOptions = {
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
    plugins: {
        colorschemes: {
            scheme: "tableau.Tableau10"
        }
    }
};
////#endregion



app.cache.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.cache_API.Read",
        {},
        "app.cache.callback.read",
        null,
        null,
        null,
        { async: false }
    );
};

app.cache.callback.read = function (data) {
    app.cache.render.summary(data);
    app.cache.render.count(data);
    app.cache.render.byte(data);
    app.cache.render.efficiency(data);
    app.cache.render.usedMemory(data);
}

app.cache.render.summary = function (data) {
    var duration = Math.floor(moment.duration(parseInt(data.Uptime), 'seconds').asDays());
    $("#cache-read-container").find("[name=cache-summary]").find("[name=uptime]").text(app.library.html.parseDynamicLabel("days", [duration]));
    $("#cache-read-container").find("[name=cache-summary]").find("[name=time]").text(moment.unix(data.ServerTime).format(app.config.mask.datetime.display));
    $("#cache-read-container").find("[name=cache-summary]").find("[name=version]").text(data.Version);
    $("#cache-read-container").find("[name=cache-summary]").find("[name=active-items]").text(app.library.utility.formatNumber(data.ItemCount));
    $("#cache-read-container").find("[name=cache-summary]").find("[name=total-items]").text(app.library.utility.formatNumber(data.TotalItems));
    $("#cache-read-container").find("[name=cache-summary]").find("[name=active-connections]").text(app.library.utility.formatNumber(data.ConnectionCount));
    $("#cache-read-container").find("[name=cache-summary]").find("[name=total-connections]").text(app.library.utility.formatNumber(data.TotalConnections));

}

app.cache.render.count = function (data) {

    new Chart($("#cache-read-container").find("[name=cache-chart]").find("[name=count]"), {
        type: 'pie',
        data: {
            labels: [app.label.static["set-count"], app.label.static["get-count"]],
            datasets: [{
                data: [data.SetCount, data.GetCount]
            }]
        },
        options: app.cache.render.chartOptions,
    });
};
app.cache.render.byte = function (data) {

    new Chart($("#cache-read-container").find("[name=cache-chart]").find("[name=chart-byte]"), {
        type: 'pie',
        data: {
            labels: [app.label.static["bytes-input"], app.label.static["bytes-output"]],
            datasets: [{
                label: "Bytes",
                data: [data.BytesRead, data.BytesWritten]
            }]
        },
        options: app.cache.render.chartOptions,
    });
}

app.cache.render.efficiency = function (data) {
    new Chart($("#cache-read-container").find("[name=cache-chart]").find("[name=chart-efficiency"), {
        type: 'pie',
        data: {
            labels: [app.label.static["hits"], app.label.static["misses"]],
            datasets: [{
                label: "MB",
                data: [data.GetHits, data.GetMisses]
            }]
        },
        options: app.cache.render.chartOptions,
    });
}

app.cache.render.usedMemory = function (data) {

    new Chart($("#cache-read-container").find("[name=cache-chart]").find("[name=chart-used-memory]"), {
        type: 'pie',
        data: {
            labels: [app.label.static["available-bytes"], app.label.static["used-bytes"]],
            datasets: [{
                label: "MB",
                data: [data.MaxBytes - data.UsedBytes, data.UsedBytes]
            }]
        },
        options: app.cache.render.chartOptions,
    });
}



//#region load config
app.cache.ajax.flushCache = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Cache_API.FlushAll",
        {},
        "app.cache.callback.flushCache",
        null,
        null,
        null
    );
};
app.cache.callback.flushCache = function (data) {
    if (data == C_APP_API_SUCCESS) {
        $("#modal-success").on('hide.bs.modal', function (e) {
            app.plugin.backbutton.check = false;
            window.location.href = window.location.pathname;
        });
        api.modal.success(app.label.static["success-cache-flush"]);

    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

//#endregion