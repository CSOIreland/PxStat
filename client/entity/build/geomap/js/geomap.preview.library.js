/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces definitions
app.geomap = app.geomap || {};
app.geomap.preview = {};
app.geomap.preview.geoJson = {};
app.geomap.preview.ajax = {};
app.geomap.preview.callback = {};

//#endregion

app.geomap.preview.ajax.readMap = function (gmpCode) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.ReadCollection",
        {
            "GmpCode": gmpCode
        },
        "app.geomap.preview.callback.readMap");
};

app.geomap.preview.callback.readMap = function (data) {

    if (data && Array.isArray(data) && data.length) {
        data = data[0];
        $("#map-modal-preview").find("[name=map-name]").text(data.GmpName);
        $("#map-modal-preview").find("[name=gmp-name]").val(data.GmpName);
        $("#map-modal-preview").find("[name=description]").html(app.library.html.parseBbCode(data.GmpDescription));
        $("#map-modal-preview").modal("show");
        app.geomap.preview.ajax.getGeoJSON(data.GmpCode);
    }
    // Handle no data
    else {
        api.modal.information(app.label.static["api-ajax-nodata"]);

    };
};

app.geomap.preview.ajax.getGeoJSON = function (gmpCode) {
    $.ajax({
        url: app.config.url.api.static + "/PxStat.Data.GeoMap_API.Read/" + gmpCode,
        dataType: 'json',
        success: function (data) {
            app.geomap.preview.geoJson = data;
            app.geomap.preview.callback.renderMap(data);
            app.geomap.preview.callback.renderProperties(data);
        },
        error: function (xhr) {
            $("#map-modal-preview").modal("hide");
            api.modal.exception(app.label.static["api-ajax-exception"]);
        }
    });
};


app.geomap.preview.callback.renderMap = async function (data) {

    api.spinner.start();
    // Sleep because of possibile high CPU spike generated next
    await app.library.utility.sleep();

    // Create canvas in parent div
    var mapContainer = $('<div>', {
        "id": "map-modal-preview-container",
        "class": "map-preview-container"
    });
    $("#map-modal-preview-map-content").find("[name=map-modal-preview-wrapper]").empty().append(mapContainer);

    var map = L.map('map-modal-preview-container', {
        maxBounds: app.geomap.preview.getMaxBounds(app.geomap.preview.geoJson)
    });

    map.attributionControl.setPrefix('');

    //add baselayers
    $.each(app.config.entity.map.baseMap.leaflet, function (index, value) {
        L.tileLayer(value.url, value.options).addTo(map);
    });

    $.each(app.config.entity.map.baseMap.esri, function (index, value) {
        L.esri.tiledMapLayer(value).addTo(map);
    });

    var allFeatures = L.geoJson(data, {
        style: {
            color: '#000000',
            weight: .5,
            fillOpacity: 0.1
        },
        onEachFeature: function (feature, layer) {
            if (feature.geometry.type != "Point") {
                layer.on("mouseover", function (e) {
                    var hoverText = "<span class='map-preview-popup'>";
                    $.each(feature.properties, function (key, value) {
                        hoverText += "<b>" + key + "</b> : " + value;

                        var isLastElement = key == feature.properties.length - 1;
                        if (!isLastElement) {
                            hoverText += "<br>"
                        }
                        else {
                            hoverText += "</span>"
                        }

                    });

                    layer.bindTooltip(hoverText).openTooltip(e.latlng);
                    layer.setStyle({
                        'weight': 3
                    });
                });
                layer.on("mouseout", function (e) {
                    layer.setStyle({
                        'weight': .5
                    });

                    layer.closeTooltip();
                });
            }
            else {
                layer.on("click", function (e) {
                    var hoverText = "<span class='map-preview-popup'>";
                    $.each(feature.properties, function (key, value) {
                        hoverText += "<b>" + key + "</b> : " + value;

                        var isLastElement = key == feature.properties.length - 1;
                        if (!isLastElement) {
                            hoverText += "<br>"
                        }
                        else {
                            hoverText += "</span>"
                        }

                    });

                    layer.bindPopup(hoverText).openPopup(e.latlng);
                })
            }
        }
    }).addTo(map);
    map.fitBounds(allFeatures.getBounds());
    map.setMinZoom(map.getZoom());
    api.spinner.stop();
};

app.geomap.preview.getMaxBounds = function (geoJson) {
    var enveloped = turf.envelope(geoJson);
    var height = (enveloped.bbox[1] - enveloped.bbox[3]);
    var width = (enveloped.bbox[0] - enveloped.bbox[2]);
    return [
        [enveloped.bbox[1] + (height / 2), enveloped.bbox[2] - (width / 2)],
        [enveloped.bbox[3] - (height / 2), enveloped.bbox[0] + (width / 2)]
    ];
};

app.geomap.preview.callback.renderProperties = function (data) {
    //build classification download dropdown 
    $("#map-modal-preview-properties-content").find("[name=download-classification-selection]").empty();
    $.each(data.features[0].properties, function (key, value) {
        if (key != C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER) {
            $("#map-modal-preview-properties-content").find("[name=download-classification-selection]").append(
                $("<a>", {
                    "class": "dropdown-item",
                    "name": "download-classification-language",
                    "href": "#",
                    "lng-iso-code": key,
                    "text": key
                })
            )
        }
    });

    $("#map-modal-preview-properties-content").find("[name=download-classification-language").once("click", function (e) {
        e.preventDefault();
        app.geomap.preview.downloadClassification($(this).attr("lng-iso-code"), data)
    })


    if ($.fn.DataTable.isDataTable("#map-modal-preview-properties-content [name=datatable]")) {
        // Must clear first then destroy and later on re-initiate 
        $("#map-modal-preview-properties-content").find("[name=datatable]").DataTable().clear().destroy();

        //clean pervious table drawing
        $("#map-modal-preview-properties-content").find("[name=datatable]").find("[name=header-row]").empty();
        $("#map-modal-preview-properties-content").find("[name=datatable]").find("tbody").empty();
    }

    var tableData = [];
    $.each(data.features, function (key, value) {
        tableData.push(value.properties)
    });
    var tableColumns = [];
    $.each(tableData[0], function (key, value) {
        var tableHeading = $("<th>", {
            "text": key
        });

        $("#map-modal-preview-properties-content").find("[name=datatable]").find("[name=header-row]").append(tableHeading);

        tableColumns.push(
            {
                data: key
            }
        )

    });

    var localOptions = {
        data: tableData,
        columns: tableColumns,
        drawCallback: function (settings) {

        },
        //Translate labels language
        language: app.label.plugin.datatable
    };

    // Initiate DataTable
    $("#map-modal-preview-properties-content").find("[name=datatable]").off().DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {

    });

    $('#map-modal-preview').on('hide.bs.modal', function (event) {
        $('#map-modal-preview-map-content').show();
        $("#map-modal-preview-properties-tab").removeClass("active");
        $("#map-modal-preview-map-tab").addClass("active");

        $("#map-modal-preview-properties-content").removeClass("active show");
        $("#map-modal-preview-map-content").addClass("active show");

        $("#map-modal-preview-map-content").find("[name=map-modal-preview-wrapper]").empty()

    })
}

app.geomap.preview.downloadClassification = function (lngIsoCode, data) {
    var fileData = [];
    $.each(data.features, function (index, value) {
        fileData.push(
            {
                [C_APP_CSV_CODE]: value.properties[C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER],
                [C_APP_CSV_VALUE]: value.properties[lngIsoCode]
            }
        );
    });
    // Download the file
    app.library.utility.download(
        $("#map-modal-preview").find("[name=gmp-name]").val().toLowerCase().replace(/\s/g, "_") + "_" + lngIsoCode,
        Papa.unparse(fileData),
        C_APP_EXTENSION_CSV,
        C_APP_MIMETYPE_CSV
    );
};