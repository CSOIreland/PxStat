/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.map = app.map || {};

app.map.ClsCode = null;
app.map.ClsValue = null;
app.map.ajax = {};
app.map.callback = {};
//#endregion

/**
 * Draw the GeoJSON map 
 */
app.map.draw = function (geoJsonURL, ClsCode, ClsValue) {
    app.map.ClsCode = ClsCode || null;
    app.map.ClsValue = ClsValue || null;

    if (!app.config.plugin.highcharts.enabled) {
        return;
    }

    $("#build-map-modal").find(".modal-header [name=cls-code]").empty().text(app.map.ClsCode);
    $("#build-map-modal").find(".modal-header [name=cls-value]").empty().text(app.map.ClsValue);

    app.map.ajax.read(geoJsonURL);
}

/** 
 * 
 */
app.map.ajax.read = function (geoJsonURL) {
    $.ajax({
        url: geoJsonURL,
        datatype: 'json',
        beforeSend: function (xhr) {
            api.spinner.start();
        },
        success: function (response) {
            app.map.callback.read(response);
            api.spinner.stop();
        },
        error: function (xhr) {
            api.modal.error(app.label.static["geojson-not-found"]);
            api.spinner.stop();
        }
    });
}

/** 
 * 
 */
app.map.callback.read = function (response) {
    if (response.type != C_APP_GEOJSON_FEATURE_COLLECTION) {
        $("#build-map-modal").find("[name=download-classification]").prop("disabled", true);
        api.modal.error(app.label.static["geojson-invalid-format"]);
    }
    else {
        $("#build-map-modal").modal("show");

        app.map.drawDatatable(response);
        app.map.drawMap(response);

        $("#build-map-modal").find("[name=download-classification]").prop("disabled", false).once("click", function () {
            app.map.download(response);
        });
    };
};

/** 
 * 
 */
app.map.drawDatatable = function (response) {
    var data = [];
    $.each(response.features, function (index, feature) {
        data.push({
            "CODE": feature.properties[app.config.plugin.highmaps.featureIdentifier],
            "VALUE": feature.properties[app.config.plugin.highmaps.featureName],
        });
    });

    if ($.fn.dataTable.isDataTable("#build-map-modal table")) {
        app.library.datatable.reDraw("#build-map-modal table", data);
    }
    else {
        var localOptions = {
            data: data,
            columns: [
                {
                    data: "CODE"
                },
                {
                    data: "VALUE"
                }
            ],
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#build-map-modal table").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions));
    };
}

/** 
 * 
 */
app.map.drawMap = function (response) {
    $("#build-map-modal [name=map]").highcharts("Map", {
        chart: {
            height: 600
        },
        title: {
            text: ""
        },
        mapNavigation: {
            enabled: true,
            buttonOptions: {
                verticalAlign: "bottom"
            }
        },
        legend: {
            enabled: false
        },
        credits: {
            enabled: false
        },
        exporting: {
            enabled: false
        },
        series: [{
            nullColor: '#BCE4E3',
            borderColor: "#1D345C",
            nullInteraction: true,
            mapData: response,
        }],
        tooltip: {
            allowHTML: true,
            formatter: function () {
                return "<h1>" + this.point.properties[app.config.plugin.highmaps.featureIdentifier] + " - " + this.point.properties[app.config.plugin.highmaps.featureName] + "</h1>";
            }
        },
        responsive: {
            rules: [{
                condition: {
                    minHeight: 900
                }
            }]
        }
    });
};

/** 
 * 
 */
app.map.download = function (response) {
    var data = [];
    $.each(response.features, function (index, feature) {
        data.push({
            "CODE": feature.properties[app.config.plugin.highmaps.featureIdentifier],
            "VALUE": feature.properties[app.config.plugin.highmaps.featureName],
        });
    });
    var mimeType = "text/plain";
    var pom = document.createElement('a');
    pom.setAttribute('href', 'data:' + mimeType + ';charset=utf-8,' + encodeURIComponent(Papa.unparse(data)));
    pom.setAttribute('download', app.map.ClsCode + ".csv");
    if (document.createEvent) {
        // https://developer.mozilla.org/en-US/docs/Web/API/Document/createEvent
        var event = document.createEvent('MouseEvents');
        event.initEvent('click', true, true);
        pom.dispatchEvent(event);
    }
    else {
        pom.click();
    }
}