/**
 * Draw browser chart using highcharts
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readBrowser = function (data, selector) {
    if (app.config.plugin.highcharts.enabled) {
        var dataArray = [
            {
                name: 'Browsers',
                colorByPoint: true,
                data: []
            }];
        var i = 0;
        $.each(data, function (key, el) {
            dataArray[0].data[i] = { "name": key, "y": el };
            i += 1;
        });
        var buttons = Highcharts.getOptions().exporting.buttons.contextButton.menuItems;
        var highchartLineOptions = {
            chart: {
                renderTo: $(selector)[0],//'browser-pie-char',
                type: 'pie'
            },
            exporting: {
                buttons: {
                    contextButton: {
                        menuItems: buttons.slice(1, 4)
                    }
                },
                chartOptions: {
                    title: {
                        text: "Browsers: " + app.analytic.dateFrom.format(app.config.mask.date.display) + " - " + app.analytic.dateTo.format(app.config.mask.date.display)
                    }
                },
                filename: "Browsers" + app.analytic.dateFrom.format(app.config.mask.datetime.file) + "-" + app.analytic.dateTo.format(app.config.mask.datetime.file)
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                    }
                }
            },
            series: dataArray
        };
        var chr = new Highcharts.Chart(highchartLineOptions);
        chr.setTitle({ text: "" });
    }

    else {
        $(selector).html(C_APP_HIGHCHARTS_NO_LICENSE_MESSAGE);
    }
};

/**
 * Draw language chart using highcharts
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readLanguage = function (data, selector) {
    if (app.config.plugin.highcharts.enabled) {
        var dataArray = [
            {
                name: 'Language',
                colorByPoint: true,
                data: []
            }];
        var i = 0;
        $.each(data, function (key, el) {
            dataArray[0].data[i] = { "name": key, "y": el };
            i += 1;
        });
        var highchartLineOptions = {
            chart: {
                renderTo: $(selector)[0],//'browser-pie-char',
                type: 'pie'
            },
            exporting: {
                chartOptions: {
                    title: {
                        text: "Language"
                    }, subtitle: {
                        text: "exportLabel"
                    },
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                    }
                }
            },
            series: dataArray
        };
        var chr = new Highcharts.Chart(highchartLineOptions);
        chr.setTitle({ text: "" });
    }

    else {
        $(selector).html(C_APP_HIGHCHARTS_NO_LICENSE_MESSAGE);
    }

};

/**
 * Draw OS chart using highcharts
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readOs = function (data, selector) {

    if (app.config.plugin.highcharts.enabled) {
        var dataArray = [
            {
                name: 'OS',
                colorByPoint: true,
                data: []
            }];
        var i = 0;
        $.each(data, function (key, el) {
            dataArray[0].data[i] = { "name": key, "y": el };
            i += 1;
        });
        var buttons = Highcharts.getOptions().exporting.buttons.contextButton.menuItems;
        var highchartLineOptions = {
            chart: {
                renderTo: $(selector)[0],//'browser-pie-char',
                type: 'pie'
            },
            exporting: {
                buttons: {
                    contextButton: {
                        menuItems: buttons.slice(1, 4)
                    }
                },
                chartOptions: {
                    title: {
                        text: "OS: " + app.analytic.dateFrom.format(app.config.mask.date.display) + " - " + app.analytic.dateTo.format(app.config.mask.date.display)
                    }
                },
                filename: "OS" + app.analytic.dateFrom.format(app.config.mask.datetime.file) + "-" + app.analytic.dateTo.format(app.config.mask.datetime.file)
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                    }
                }
            },
            series: dataArray
        };
        var chr = new Highcharts.Chart(highchartLineOptions);
        chr.setTitle({ text: "" });
    }

    else {
        $(selector).html(C_APP_HIGHCHARTS_NO_LICENSE_MESSAGE);
    }


};

/**
 * Draw referrer chart using highcharts
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readReferrer = function (data, selector) {
    if (app.config.plugin.highcharts.enabled) {
        var dataArray = [{
            name: app.label.static["referrer"],
            colorByPoint: true,
            data: []
        }];
        var i = 0;
        var labels = [];
        for (key in data) {
            labels[i] = key;
            dataArray[0].data[i] = { "name": key, "y": data[key] };
            i += 1;
        }
        var buttons = Highcharts.getOptions().exporting.buttons.contextButton.menuItems;
        var highchartLineOptions = {
            chart: {
                renderTo: $(selector)[0],
                type: 'column'
            },
            exporting: {
                buttons: {
                    contextButton: {
                        menuItems: buttons.slice(1, 4)
                    }
                },
                chartOptions: {
                    title: {
                        text: "ReferrerXXX2: " + app.analytic.dateFrom.format(app.config.mask.date.display) + " - " + app.analytic.dateTo.format(app.config.mask.date.display)
                    }
                },
                filename: "ReferrerXXX3" + app.analytic.dateFrom.format(app.config.mask.datetime.file) + "-" + app.analytic.dateTo.format(app.config.mask.datetime.file)
            },
            credits: {
                enabled: false
            },
            yAxis: { title: { text: app.label.static["hits"] } },
            xAxis: { categories: labels },
            series: dataArray
        };
        var chr = new Highcharts.Chart(highchartLineOptions);
        chr.setTitle({ text: "" });
    }
    else {
        $(selector).html(C_APP_HIGHCHARTS_NO_LICENSE_MESSAGE)
    }
};

/**
 * Draw timeline chart using highcharts
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readTimeLine = function (data, selector) {
    if (app.config.plugin.highcharts.enabled) {
        var buttons = Highcharts.getOptions().exporting.buttons.contextButton.menuItems;
        var options = {

            chart: {
                type: 'spline',
                renderTo: $(selector)[0],
            },
            exporting: {
                buttons: {
                    contextButton: {
                        menuItems: buttons.slice(1, 4)
                    }
                },
                chartOptions: {
                    title: {
                        text: "Timeline: " + app.analytic.dateFrom.format(app.config.mask.date.display) + " - " + app.analytic.dateTo.format(app.config.mask.date.display)
                    }
                },
                filename: "Timeline" + app.analytic.dateFrom.format(app.config.mask.datetime.file) + "-" + app.analytic.dateTo.format(app.config.mask.datetime.file)
            },
            title: {
                text: ''
            },
            xAxis: {
                categories: []
            },
            yAxis: {
                title: {
                    text: app.label.static["hits"]
                }
            },
            series: []
        };
        var bots = {
            name: app.label.static["bots"],
            data: []
        };
        var M2M = {
            name: app.label.static["m2m"],
            data: []
        };
        var users = {
            name: app.label.static["users"],
            data: []
        };
        var total = {
            name: app.label.static["total"],
            data: []
        };
        $.each(data, function (index, value) {
            options.xAxis.categories.push(moment(value.date).format(app.config.mask.date.display));
            bots.data.push(value.NltBot);
            M2M.data.push(value.NltM2m);
            users.data.push(value.NltUser);
            total.data.push(value.Total);
        });
        options.series.push(bots, M2M, users, total);
        var chr = new Highcharts.Chart(options);
    }
    else {
        $(selector).html(C_APP_HIGHCHARTS_NO_LICENSE_MESSAGE);
    }
};