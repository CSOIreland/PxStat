/**
 * Draw browser chart using google
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readBrowser = function (data, selector) {
    google.charts.setOnLoadCallback(function () {
        var dataArray = [['Desc', 'Counts']];
        $.each(data, function (index, value) {
            dataArray.push([index, value]);
        });
        var options = {
            legend: {
                position: 'bottom'
            }
        };
        var toChart = google.visualization.arrayToDataTable(dataArray);
        var chart = new google.visualization.PieChart($(selector)[0]);
        chart.draw(toChart, options);
    });
};

/**
 * Draw language chart using google
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readLanguage = function (data, selector) {
    google.charts.setOnLoadCallback(function () {
        var dataArray = [['Desc', 'Counts']];
        $.each(data, function (index, value) {
            dataArray.push([index, value]);
        });
        var options = {
            legend: {
                position: 'bottom'
            }
        };
        var toChart = google.visualization.arrayToDataTable(dataArray);
        var chart = new google.visualization.PieChart($(selector)[0]);
        chart.draw(toChart, options);
    });
};

/**
 * Draw OS chart using google
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readOs = function (data, selector) {
    google.charts.setOnLoadCallback(function () {
        var dataArray = [['Desc', 'Counts']];
        $.each(data, function (index, value) {
            dataArray.push([index, value]);
        });
        var options = {
            legend: {
                position: 'bottom'
            }
        };
        var toChart = google.visualization.arrayToDataTable(dataArray);
        var chart = new google.visualization.PieChart($(selector)[0]);
        chart.draw(toChart, options);
    });


};

/**
 * Draw referrer chart using google
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readReferrer = function (data, selector) {
    google.charts.setOnLoadCallback(function () {
        var dataArray = [['Desc', 'Counts']];
        $.each(data, function (index, value) {
            dataArray.push([index, value]);
        });
        var options = {};
        var toChart = google.visualization.arrayToDataTable(dataArray);
        var chart = new google.visualization.ColumnChart($(selector)[0]);
        chart.draw(toChart, options);
    });
};

/**
 * Draw timeline chart using google
 * @param  {} data
 * @param  {} selector
 */
app.analytic.render.readTimeLine = function (data, selector) {
    google.charts.setOnLoadCallback(function () {
        //var dataArray = [['Date', 'bots', 'users', 'm2m', 'total']];
        var dataArray = [['Date', app.label.static["bots"], app.label.static["users"], app.label.static["m2m"], app.label.static["total"]]];
        $.each(data, function (index, value) {
            dataArray.push([moment(value.date).format(app.config.mask.date.display), value.NltBot, value.NltUser, value.NltM2m, value.Total]);
        });
        var options = {
            hAxis: { slantedText: true, slantedTextAngle: 50 }
        };
        var toChart = google.visualization.arrayToDataTable(dataArray);
        var chart = new google.visualization.LineChart($(selector)[0]);
        chart.draw(toChart, options);
    });

};