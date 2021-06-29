/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces definitions
app.geomap = {};
app.geomap.modal = {};
app.geomap.validation = {};
app.geomap.ajax = {};
app.geomap.callback = {};
app.geomap.import = {};
app.geomap.import.file = {};
app.geomap.import.file.content = {};
app.geomap.import.file.content.geojson = null;
app.geomap.import.file.content.geojsonModified = null;
app.geomap.import.file.content.geojsonSimplified = null;
app.geomap.import.file.content.isPointGeometry = false;
app.geomap.import.file.name = null;
app.geomap.import.file.extension = null;
app.geomap.import.file.size = null;
app.geomap.import.file.type = null;

//#endregion

//#region read
app.geomap.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.ReadCollection",
        {},
        "app.geomap.callback.read",
        null,
        null,
        null,
        { async: false });
};

app.geomap.callback.read = function (data) {
    if ($.fn.dataTable.isDataTable("#map-read-container table")) {
        app.library.datatable.reDraw("#map-read-container table", data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            columns: [
                {
                    data: null,
                    render: function (_data, _type, row) {
                        var attributes = { idn: row.GmpCode }; //idn SbjCode
                        return app.library.html.link.edit(attributes, row.GmpName);
                    }
                },
                {
                    data: "GmpFeatureCount"
                },
                {
                    data: "GmpDescription",
                    visible: false,
                },
                {
                    data: "GlrName"
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
                                }).get(0).outerHTML +
                                " " + app.label.static["details"]
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.view({ "gmp-code": row.GmpCode }, null, app.label.static["view-map"]);
                    },
                    "width": "1%"
                },
                {
                    data: null,
                    visible: false,
                    render: function (_data, _type, row) {
                        return app.library.html.link.external({ name: "map-url-" + row.GmpCode }, app.config.url.api.static + "/PxStat.Data.GeoMap_API.Read/" + row.GmpCode)
                    }
                },
                {
                    data: null,
                    render: function (_data, _type, row) {
                        return app.library.html.link.internal({ idn: row.GmpCode, "gmp-name": row.GmpName }, String(row.TableCount));
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        var attributes = { idn: row.GmpCode, "gmp-name": row.GmpName }; //idn SbjCode
                        var deleteButton = app.library.html.deleteButton(attributes, false);
                        if (row.TableCount > 0) {
                            deleteButton = app.library.html.deleteButton(attributes, true);
                        }
                        return deleteButton;
                    },
                    "width": "1%"
                }
            ],
            drawCallback: function (settings) {
                app.geomap.drawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#map-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.geomap.drawCallback();
        });
    }
};

app.geomap.drawCallback = function () {

    // Extra Info
    app.library.datatable.showExtraInfo("#map-read-container table", app.geomap.drawExtraInfo);

    // click event update
    $("#map-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        app.geomap.ajax.readUpdate($(this).attr("idn"));
    });

    $("#map-read-container table").find("[name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
        e.preventDefault();
        app.geomap.preview.ajax.readMap($(this).attr("gmp-code"))
    });


    // Click event "internalLink"
    $("#map-read-container table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();
        var callbackParam = {
            "idn": $(this).attr("idn"),
            "gmpName": $(this).attr("gmp-name")
        };
        // Ajax read 
        app.geomap.ajax.readMatrixByMap(callbackParam);

    });



    $("#map-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function (e) {
        e.preventDefault();

        var params = {
            "idn": $(this).attr("idn"),
            "gmpName": $(this).attr("gmp-name")
        };


        api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [params.gmpName]),
            app.geomap.ajax.deleteMap,
            params
        );
    });

    $('[data-toggle="tooltip"]').tooltip();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
};

app.geomap.drawExtraInfo = function (data) {
    var extraInfo = $("#map-templates").find("[name=map-read-extra-info]").clone();
    extraInfo.find("[name=description]").html(app.library.html.parseBbCode(data.GmpDescription));
    extraInfo.find("[name=restful-url]").html(
        function () {
            return app.library.html.link.external({ name: "map-url-" + data.GmpCode }, app.config.url.api.static + "/PxStat.Data.GeoMap_API.Read/" + data.GmpCode);
        }
    );

    extraInfo.find("[name=restful-url-copy-icon]").html(
        function () {
            return $("<button>", {
                class: "btn btn-outline-primary btn-sm mt-2",
                name: "map-url-copy-icon",
                "data-toggle": "tooltip",
                "data-clipboard-target": "#modal-information [name=map-url-" + data.GmpCode + "]",
                "data-placement": "right",
                "title": "",
                "style": "cursor: grab",
                "data-clipboard-action": "copy",
                "label-tooltip": app.label.static["copy-to-clipboard"],
                html:
                    $("<i>", {
                        class: "far fa-copy"
                    }).get(0).outerHTML + " " + app.label.static["copy-to-clipboard"]
            });
        }
    );
    //initiate all copy to clipboard 
    new ClipboardJS("#modal-information [name=map-url-copy-icon]");
    return extraInfo.show().get(0).outerHTML;


};
//#endregion read

//#region import
api.plugin.dragndrop.readFiles = function (files, inputObject) {
    // Read single file only
    var file = files[0];
    if (!file) {
        return;
    };

    // set namespaced variables
    app.geomap.import.file.name = file.name;
    app.geomap.import.file.extension = file.name.match(/\.[0-9a-z]+$/i)[0];
    app.geomap.import.file.type = file.type;
    app.geomap.import.file.size = file.size;

    if ($.inArray(app.geomap.import.file.extension.toLowerCase(), C_APP_UPLOAD_MAP_FILE_ALLOWED_EXTENSION) == -1) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [app.geomap.import.file.extension]));
        return;
    }
    // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
    if ($.inArray(app.geomap.import.file.type.toLowerCase(), C_APP_UPLOAD_MAP_FILE_ALLOWED_TYPE) == -1) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [app.geomap.import.file.type]));
    }
    // Check for the hard limit of the file size
    if (app.geomap.import.file.size > app.config.transfer.threshold.hard) {
        // Show Error
        api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.library.utility.formatNumber(Math.ceil(app.config.transfer.threshold.hard / 1024 / 1024)) + " MB"]));

    }

    // Info on screen 
    inputObject.parent().find("[name=upload-file-tip]").hide();
    inputObject.parent().find("[name=upload-file-name]").html(app.geomap.import.file.name + " (" + app.library.utility.formatNumber(Math.ceil(app.geomap.import.file.size / 1024)) + " KB)").show();

    // Read file into an UTF8 string
    var readerUTF8 = new FileReader();
    readerUTF8.onload = function (e) {
        // Set the file's content
        try {
            app.geomap.import.file.content.geojson = JSON.parse(e.target.result);
            app.geomap.ajax.isFileValid();

        } catch (error) {
            api.modal.error(app.label.static["invalid-geojson-file"]);
            return
        }
    };
    readerUTF8.readAsText(file);
    readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
    readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
    readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
    readerUTF8.addEventListener("loadend", function (e) {
        api.spinner.stop();
    });

};

app.geomap.ajax.isFileValid = function () {
    // Add the progress bar
    api.spinner.progress.start(api.spinner.progress.getTimeout(JSON.stringify(app.geomap.import.file.content.geojson).length, app.config.transfer.unitsPerSecond["PxStat.Data.GeoMap_API.Validate"]));

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.Validate",
        {
            "GmpGeoJson": app.geomap.import.file.content.geojson
        },
        "app.geomap.callback.isFileValid",
        null,
        null,
        null,
        { async: false });

};

app.geomap.callback.isFileValid = function (data) {
    if (data) {
        // Check for signature
        if (data == C_API_AJAX_SUCCESS) {
            $("#map-modal-add").find("[name=import]").prop('disabled', false);

        } else if (Array.isArray(data) && data.length) {
            var errors = $("<ul>", {
                "class": "list-group"
            });
            $.each(data, function (index, value) {
                errors.append(
                    $("<li>", {
                        "class": "list-group-item",
                        "text": value
                    })
                )
            });
            api.modal.error(errors.get(0).outerHTML)

        } else api.modal.exception(app.label.static["api-ajax-exception"]);
    } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

app.geomap.ajax.readLanguage = function () {
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
        "PxStat.System.Settings.Language_API.Read",
        { LngIsoCode: null },
        "app.geomap.callback.readLanguage");
};

app.geomap.callback.readLanguage = function (data) {

    $("#map-modal-add").find("[name=lng-checkbox-group]").empty();

    $("#map-modal-add").find("[name=lng-checkbox-group]").append(
        function () {
            var defaultLng = $("<li>", {
                "class": "list-group-item",
                "html": $("<input>", {
                    "type": "checkbox",
                    "name": "lng-group",
                    "value": app.config.language.iso.code,
                    "lngName": app.config.language.iso.name,
                    "checked": "checked",
                    "disabled": "disabled"
                }).get(0).outerHTML + " " + app.config.language.iso.name
            }).get(0).outerHTML;
            return defaultLng;
        }
    );

    if (data && Array.isArray(data) && data.length) {
        $.each(data, function (index, value) {
            if (value.LngIsoCode != app.config.language.iso.code) {
                $("#map-modal-add").find("[name=lng-checkbox-group]").append(function () {
                    return $("<li>", {
                        "class": "list-group-item",
                        "html": $("<input>", {
                            "type": "checkbox",
                            "name": "lng-group",
                            "value": value.LngIsoCode,
                            "lngName": value.LngIsoName
                        }).get(0).outerHTML + " " + value.LngIsoName
                    }).get(0).outerHTML;
                });
            }
        });
    };
}

app.geomap.ajax.readLayers = function () {
    $("#map-modal-add").find("[name=glr-code]").prop('disabled', true);
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoLayer_API.Read",
        {},
        "app.geomap.callback.readLayers");
};

app.geomap.callback.readLayers = function (data) {
    if (data && Array.isArray(data) && data.length) {
        $("#map-modal-add").find("[name=glr-code]").empty();

        $("#map-modal-add").find("[name=glr-code]").append($("<option>", {
            "text": app.label.static["select-uppercase"],
            "disabled": "disabled",
            "selected": "selected"
        }));
        $.each(data, function (key, value) {
            $("#map-modal-add").find("[name=glr-code]").append($("<option>", {
                "value": value.GlrCode,
                "text": value.GlrName
            }));
        });

        $("#map-modal-add").find("[name=glr-code]").prop('disabled', false);

    } else {
        api.modal.information(app.label.static["api-ajax-nodata"]);
    }
};

app.geomap.setDetails = function () {
    $("#map-modal-add").find("[name=set-details-card]").show();
};

app.geomap.setProperties = function () {
    $("#map-modal-add").find("[name=set-properties]").prop('disabled', true);
    $("#map-modal-add").find("[name=map-upload-file]").prop('disabled', true);
    //disable languages from this point on
    $("#map-modal-add").find("[name=lng-group]").prop('disabled', true);
    $("#map-modal-add").find("[name=import]").prop('disabled', true);
    $("#map-modal-add").find("[name=code-mapping]").empty();

    $("#map-modal-add").find("[name=code-mapping]").append($("<option>", {
        "text": app.label.static["select"],
        "disabled": "disabled",
        "selected": "selected",
        "value": null
    }));


    if (!$.isEmptyObject(app.geomap.import.file.content.geojson.features[0].properties)) {
        $.each(app.geomap.import.file.content.geojson.features[0].properties, function (key, value) {
            // Set in Properties
            $("#map-modal-add").find("[name=code-mapping]").append($("<option>", {
                "text": key,
                "value": key
            }));
        });
    }
    else {
        api.modal.error(app.label.static["geojson-invalid-feature-properties"])
    }

    $("#map-modal-add").find("[name=lng-group]:checked").each(function () {
        var languageMappingRow = $("#map-modal-view-templtes").find("[name=feature-label-mapping]").clone();
        languageMappingRow.find("[name=language]").text($(this).attr("lngName"));
        languageMappingRow.find("select").attr("idn", $(this).val())
        languageMappingRow.find("select").append($("<option>", {
            "text": app.label.static["select"],
            "disabled": "disabled",
            "selected": "selected",
            "value": null
        }));

        $.each(app.geomap.import.file.content.geojson.features[0].properties, function (key, value) {
            languageMappingRow.find("select").append($("<option>", {
                "text": key,
                "value": key
            }));
        });

        $("#map-modal-add").find("[name=feature-label-mappings]").append(languageMappingRow);

    });

    $("#map-modal-add").find("[name=set-properties-card]").show();

    $('#map-modal-add').animate({
        scrollTop: '+=' + $('#map-modal-add [name=set-properties-card]')[0].getBoundingClientRect().top
    }, 1000);

    //only enable view map button once all properties are set
    var numSelects = $("#map-modal-add").find("[name=set-properties-card]").find("select").length;

    $("#map-modal-add").find("[name=set-properties-card]").find("select").once("change", async function () {
        api.spinner.start();
        // Sleep because of possibile high CPU spike generated next
        await app.library.utility.sleep();

        //check if every select has a value before validating codes and moving on
        var mappingPropertiesSet = [];
        $("#map-modal-add").find("[name=set-properties-card]").find("select").each(function (index) {
            if ($(this).val()) {
                mappingPropertiesSet.push($(this).val());
            }
        });

        if (mappingPropertiesSet.length == numSelects) {
            //clone original geojsoninto new object
            app.geomap.import.file.content.geojsonModified = $.extend(true, {}, app.geomap.import.file.content.geojson);
            //get code mapping
            var codeMapping = $("#map-modal-add").find("[name=code-mapping]").val();
            var labelMappings = {};

            $("#map-modal-add").find("[name=feature-label-mappings]").find("select").each(function (index) {
                labelMappings[$(this).attr("idn")] = $(this).val();
            });
            var codes = [];
            var featuresValid = true;
            //loop through features and rebuild
            $.each(app.geomap.import.file.content.geojsonModified.features, function (key, value) {
                //empty features
                value.properties = {};
                //rebuild features
                if (app.geomap.import.file.content.geojson.features[key].properties[codeMapping]) {
                    var featureCode = app.geomap.import.file.content.geojson.features[key].properties[codeMapping].toString()
                    value.properties[C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER] = featureCode;
                    codes.push(featureCode);
                }
                else {
                    featuresValid = false;
                    return false
                }

                $.each(labelMappings, function (keyLabel, valueLabel) {
                    if (app.geomap.import.file.content.geojson.features[key].properties[valueLabel]) {
                        value.properties[keyLabel] = app.geomap.import.file.content.geojson.features[key].properties[valueLabel];
                    }
                    else {
                        featuresValid = false;
                        return false
                    }
                });
            });

            if (featuresValid) {
                //validate that we don't have duplicate codes
                if (app.library.utility.arrayHasDuplicate(codes)) {
                    api.modal.error(app.library.html.parseDynamicLabel("invalid-geojson-mapping", [codeMapping]));
                    $("#map-modal-add").find("[name=view-map]").prop('disabled', true);
                }
                else {
                    $("#map-modal-add").find("[name=view-map]").prop('disabled', false);
                }
            }
            else {
                api.modal.error(app.label.static["geojson-invalid-feature-properties"])
            }
        };
        api.spinner.stop();
    });

};

/**
 * @param {*} weight 
 */
app.geomap.viewAddMap = async function (weight) {
    weight = weight || 0;

    api.spinner.start();
    // Sleep because of possibile high CPU spike generated next
    await app.library.utility.sleep();

    $("#map-modal-add").find("[name=view-map-wrapper]").empty();
    var topoJson = topojson.topology({ "collection": app.geomap.import.file.content.geojsonModified });
    var preSimplify = topojson.presimplify(topoJson);
    var simplified = topojson.simplify(preSimplify, weight);
    app.geomap.import.file.content.geojsonSimplified = topojson.feature(simplified, simplified.objects.collection);
    $("#map-modal-add").find("button[type=submit]").prop('disabled', false);

    $("#map-modal-add").find("[name=view-map-card]").show();
    if (app.geomap.import.file.content.isPointGeometry) {
        $("#map-modal-add").find("[name=simplify-range-wrapper]").hide();
    };

    // Create canvas in parent div
    var mapContainer = $('<div>', {
        "id": "map-modal-view-map",
        "class": "map-preview-container"
    });
    $("#map-modal-add").find("[name=view-map-wrapper]").append(mapContainer);

    var map = L.map('map-modal-view-map');
    L.tileLayer('https://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}', {
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
        attribution: 'Google Maps'
    }).addTo(map);

    var allFeatures = L.geoJson(app.geomap.import.file.content.geojsonSimplified, {
        style: {
            color: '#000000',
            weight: .5,
            fillOpacity: 0.1
        },
        onEachFeature: function (feature, layer) {
            layer.bindPopup(function () {
                var popupText = "<span class='map-preview-popup'>";
                $.each(feature.properties, function (key, value) {
                    popupText += "<b>" + key + "</b> : " + value;
                    var isLastElement = key == feature.properties.length - 1;
                    if (!isLastElement) {
                        popupText += "<br>"
                    }
                    else {
                        popupText += "</span>"
                    }
                });
                return popupText
            });
        }
    }).addTo(map);

    map.fitBounds(allFeatures.getBounds());

    $('#map-modal-add').find("[name=file-size]").text(app.library.utility.formatNumber(Math.ceil(JSON.stringify(app.geomap.import.file.content.geojsonSimplified).length / 1024)) + " KB");

    $('#map-modal-add').animate({
        scrollTop: '+=' + $('#map-modal-add [name=view-map-card]')[0].getBoundingClientRect().top
    }, 1000);

    api.spinner.stop();
};

app.geomap.renderMapProperties = function () {

    if ($.fn.DataTable.isDataTable("#map-modal-add-preview-properties-content [name=datatable]")) {
        // Must clear first then destroy and later on re-initiate 
        $("#map-modal-add-preview-properties-content").find("[name=datatable]").DataTable().clear().destroy();

        //clean pervious table drawing
        $("#map-modal-add-preview-properties-content").find("[name=datatable]").find("[name=header-row]").empty();
        $("#map-modal-add-preview-properties-content").find("[name=datatable]").find("tbody").empty();
    }

    var tableData = [];
    $.each(app.geomap.import.file.content.geojsonModified.features, function (key, value) {
        tableData.push(value.properties)
    });


    //build classification download dropdown 
    $("#map-modal-add-preview-properties-content").find("[name=download-classification-selection]").empty();
    $.each(tableData[0], function (key, value) {
        if (key != C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER) {
            $("#map-modal-add-preview-properties-content").find("[name=download-classification-selection]").append(
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

    $("#map-modal-add-preview-properties-content").find("[name=download-classification-language").once("click", function (e) {
        e.preventDefault();
        app.geomap.downloadClassification($(this).attr("lng-iso-code"), tableData)
    })


    var tableColumns = [];
    $.each(tableData[0], function (key, value) {
        var tableHeading = $("<th>", {
            "text": key
        });

        $("#map-modal-add-preview-properties-content").find("[name=datatable]").find("[name=header-row]").append(tableHeading);

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
    $("#map-modal-add-preview-properties-content").find("[name=datatable]").off().DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {

    });

};

app.geomap.downloadClassification = function (lngIsoCode, data) {
    var fileData = [];
    $.each(data, function (index, value) {
        fileData.push(
            {
                [C_APP_CSV_CODE]: value[C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER],
                [C_APP_CSV_VALUE]: value[lngIsoCode]
            }
        );
    });
    // Download the file
    app.library.utility.download(
        $("#map-modal-add").find("[name=gmp-name]").val().toLowerCase().replace(/\s/g, "_") + "_" + lngIsoCode,
        Papa.unparse(fileData),
        C_APP_EXTENSION_CSV,
        C_APP_MIMETYPE_CSV
    );
}

app.geomap.validation.addMap = function () {
    $("#map-modal-add form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        ignore: "[name=simplify-range]",
        rules: {
            "glr-code": {
                required: true
            },
            "gmp-name": {
                required: true
            },
            "gmp-description": {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#map-modal-add [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.geomap.ajax.addMap();
        }
    }).resetForm();

};

app.geomap.ajax.addMap = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.Create",
        {
            "GmpDescription": $("#map-modal-add").find("[name=gmp-description]").val(),
            "GmpName": $("#map-modal-add").find("[name=gmp-name]").val(),
            "GlrCode": $("#map-modal-add").find("[name=glr-code]").val(),
            "GmpGeoJson": app.geomap.import.file.content.geojsonSimplified
        },
        "app.geomap.callback.addMap",
        $("#map-modal-add").find("[name=gmp-name]").val(),
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        }
    );
};

app.geomap.callback.addMap = function (data, gmpName) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#map-modal-add").modal("hide");
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [gmpName]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

    //Refresh the table
    app.geomap.ajax.read();
    app.geomap.panel.ajax.readLayers();
};

app.geomap.modal.addMapReset = function () {
    // tidy up
    app.geomap.import.file.content.geojson = null;
    app.geomap.import.file.content.geojsonModified = null;
    app.geomap.import.file.content.geojsonSimplified = null;
    app.geomap.import.file.content.isPointGeometry = false;

    $("#map-modal-add").find("[name=feature-label-mappings]").empty();
    $("#map-modal-add").find("[name=import]").prop('disabled', true);
    $("#map-modal-add").find("[name=view-map]").prop('disabled', true);
    $("#map-modal-add").find("button[type=submit]").prop('disabled', true);
    $("#map-modal-add").find("[name=set-properties]").prop('disabled', false);
    $("#map-modal-add").find("[name=simplify-range-value]").text("0")

    $("#map-modal-add form").trigger("reset");

    $("#map-modal-add").find("[name=upload-file-name]").empty().hide();
    $("#map-modal-add").find("[name=upload-file-tip]").show();
    $("#map-modal-add").find("[name=map-upload-file]").val("");

    $("#map-modal-add").find("[name=map-upload-file]").prop('disabled', false);

    $('#map-modal-add-preview-map-content').tab('show');
    $("#map-modal-add-preview-properties-tab").removeClass("active");
    $("#map-modal-add-preview-map-tab").addClass("active");

    $("#map-modal-add-preview-properties-content").removeClass("active show");
    $("#map-modal-add-preview-map-content").addClass("active show");

    $("#map-modal-add [name=set-properties-card], #map-modal-add [name=view-map-card], #map-modal-add [name=set-details-card]").hide();
    $("#map-modal-add").find("[name=simplify-range-wrapper]").show();
}

//#endregion import

//#region delete map

app.geomap.ajax.deleteMap = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.Delete",
        {
            "GmpCode": params.idn
        },
        "app.geomap.callback.deleteMap",
        params.gmpName);
};

app.geomap.callback.deleteMap = function (data, gmpName) {
    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [gmpName]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

    //Refresh the table
    app.geomap.ajax.read();
    app.geomap.panel.ajax.readLayers();
}
//#endregion delete map

//#region matrix by map
app.geomap.ajax.readMatrixByMap = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.ReadByGeoMap",
        {
            "GmpCode": params.idn,
            "LngIsoCode": app.label.language.iso.code
        },
        "app.geomap.callback.readMatrixByMap",
        params);
};

app.geomap.callback.readMatrixByMap = function (data, params) {
    $("#map-release-modal").find("[name=gmp-name]").text(params.gmpName);
    if ($.fn.dataTable.isDataTable("#map-release-modal table")) {
        app.library.datatable.reDraw("#map-release-modal table", data);
    } else {
        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
                        return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
                    }
                }
            ],
            drawCallback: function (settings) {
                app.geomap.callback.drawCallbackReadMatrixByMap();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#map-release-modal table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.geomap.callback.drawCallbackReadMatrixByMap();
        });
    };
    $("#map-release-modal").modal("show");
};

app.geomap.callback.drawCallbackReadMatrixByMap = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Release version link click redirect to 
    $("#map-release-modal table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();
        //Set the code
        var MtrCode = $(this).attr("MtrCode");

        $("#map-release-modal").modal("hide");

        //Wait for the modal to close
        $("#map-release-modal").on('hidden.bs.modal', function (e) {
            //Unbind the event for next call
            $("#map-release-modal").off('hidden.bs.modal');

            api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": MtrCode });
        })


    });
};
//#endregion matrix by map

//#region update map
app.geomap.ajax.readUpdate = function (idn) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.ReadCollection",
        {
            "GmpCode": idn
        },
        "app.geomap.callback.readUpdate");
};

app.geomap.callback.readUpdate = function (data) {
    if (data && Array.isArray(data) && data.length) {
        data = data[0];
        app.geomap.modal.updateMap(data);
    }
    // Handle no data
    else {
        api.modal.information(app.label.static["api-ajax-nodata"]);

    }

};

app.geomap.modal.updateMap = function (data) {
    app.geomap.validation.updateMap();
    $("#map-modal-update").find("[name=idn]").val(data.GmpCode);
    $("#map-modal-update").find("[name=gmp-name]").val(data.GmpName);
    var tinyMceDescription = $("#map-modal-update").find("[name=gmp-description]").attr("id");
    tinymce.get(tinyMceDescription).setContent(data.GmpDescription);
    $("#map-modal-update").modal("show");
};

app.geomap.validation.updateMap = function () {
    $("#map-modal-update form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        ignore: [],
        rules: {
            "gmp-name": {
                required: true
            },
            "gmp-description": {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#map-modal-update [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.geomap.ajax.updateMap();
        }
    }).resetForm();
};

app.geomap.ajax.updateMap = function () {
    var gmpName = $("#map-modal-update").find("[name=gmp-name]").val().trim();
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.Update",
        {
            "GmpCode": $("#map-modal-update").find("[name=idn]").val(),
            "GmpName": gmpName,
            "GmpDescription": $("#map-modal-update").find("[name=gmp-description]").val().trim(),
        },
        "app.geomap.callback.updateMap",
        gmpName);
};

app.geomap.callback.updateMap = function (data, gmpName) {
    if (data == C_API_AJAX_SUCCESS) {
        $("#map-modal-update").modal("hide");
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [gmpName]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

    //Refresh the table
    app.geomap.ajax.read();
};

//#endregion update map