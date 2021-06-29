/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build = app.build || {};
app.build.create.dimension = {};
app.build.create.dimension.modal = {};
app.build.create.dimension.ajax = {};
app.build.create.dimension.callback = {};
app.build.create.dimension.validation = {};
app.build.create.dimension.propertiesValid = false;
app.build.create.dimension.periodsManualValid = true;
app.build.create.dimension.mapsValid = true;

//#endregion

//#region build format dropdown
/**
 *Call Ajax for read format
 */
app.build.create.dimension.ajax.readFormat = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.System.Settings.Format_API.Read",
        {
            "LngIsoCode": null,
            "FrmDirection": C_APP_TS_FORMAT_DIRECTION_UPLOAD
        },
        "app.build.create.dimension.callback.readFormat"
    );
};

/**
 * Callback for read
 * @param {*} data
 */
app.build.create.dimension.callback.readFormat = function (data) {
    app.build.create.dimension.callback.drawFormat(data);
};

/**
 * Draw dropdown
 * @param {*} data
 */
app.build.create.dimension.callback.drawFormat = function (data) {
    if (data && Array.isArray(data) && data.length) {
        $.each(data, function (index, format) {
            var formatDropdown = $("#build-create-dimension-metadata-templates").find("[name=create-submit]").clone();
            formatDropdown.attr(
                {
                    "frm-type": format.FrmType,
                    "frm-version": format.FrmVersion
                });

            formatDropdown.find("[name=type]").text(format.FrmType);
            formatDropdown.find("[name=version]").text(format.FrmVersion);

            $("#build-create-matrix-dimensions [name=format-list]").append(formatDropdown);
        });

        $("#build-create-matrix-dimensions").find("[name=create-submit]").once("click", function (e) {
            e.preventDefault();
            $.each(app.build.create.initiate.languages, function (key, value) {
                $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[type=submit]").trigger("click");
            });
            if (app.build.create.dimension.propertiesValid) {
                app.build.create.initiate.data.FrmType = $(this).attr("frm-type");
                app.build.create.initiate.data.FrmVersion = $(this).attr("frm-version");
                app.build.create.dimension.buildDataObject();
            };
        });
    }
};
//#endregion
//#region draw tabs 
app.build.create.dimension.drawTabs = function () {
    //each accordion must have unique id to toggle, each collapse must unique id. Use language code to build unique id's

    $("#build-create-dimensions").find("[name=nav-tab]").empty();
    $("#build-create-dimensions").find("[name=tab-content]").empty();
    // Tabs item item
    $.each(app.build.create.initiate.languages, function (key, value) {
        var tabLanguageItem = $("#build-create-dimension-metadata-templates").find("[name=nav-lng-tab-item]").clone(); // Tabs item item
        //Set values
        tabLanguageItem.attr("lng-iso-code", value.LngIsoCode);
        tabLanguageItem.attr("lng-iso-name", value.LngIsoName);
        tabLanguageItem.attr("id", "build-create-dimension-nav-" + value.LngIsoCode + "-tab");
        tabLanguageItem.attr("href", "#build-create-dimension-nav-" + value.LngIsoCode);
        tabLanguageItem.attr("aria-controls", "nav-" + value.LngIsoCode);
        tabLanguageItem.text(value.LngIsoName);
        if (key === 0) {
            tabLanguageItem.addClass("active show");
        }
        $("#build-create-dimensions #nav-tab").append(tabLanguageItem.get(0).outerHTML);
        var tabContent = $("#build-create-dimension-metadata-templates").find("[name=nav-lng-tab-item-content]").clone();
        tabContent.attr("id", "build-create-dimension-nav-" + value.LngIsoCode);


        tabContent.attr("lng-iso-code", value.LngIsoCode);
        if (key === 0) {
            tabContent.addClass("active show");
        }
        tabContent.find(".accordion").attr("id", "build-create-dimension-accordion-" + value.LngIsoCode);
        $.each(tabContent.find("[name=dimension-collapse]"), function () {
            $(this).find(".collapse").attr("data-parent", "#" + "build-create-dimension-accordion-" + value.LngIsoCode);
            $(this).find(".collapse").attr("id", "build-create-dimension-accordion-collapse-" + $(this).attr("card") + "-" + value.LngIsoCode);
            $(this).find(".card-header").find(".btn-link").attr("data-target", "#build-create-dimension-accordion-collapse-" + $(this).attr("card") + "-" + value.LngIsoCode);
            $(this).find(".card-header").find(".btn-link").attr("aria-controls", "collapse-" + $(this).attr("card"));
        });


        $("#build-create-matrix-dimensions").find("[name=tab-content]").append(tabContent.get(0).outerHTML);


    });
    //Draw tab content for each language
    $.each(app.build.create.initiate.languages, function (key, value) {
        app.build.create.dimension.validation.properties(value.LngIsoCode);
        $("#build-create-dimension-accordion-" + value.LngIsoCode).on('hide.bs.collapse show.bs.collapse', function (e) {
            $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[type=submit]").trigger("click");
            if (!app.build.create.dimension.propertiesValid) { //keep open while invalid
                e.preventDefault();
            }
        });
        $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[name=add-statistics]").once("click", function () {
            $('#build-create-manual-si table').find("tbody").empty();
            $('#build-create-manual-si').find("[name=errors-card]").hide();
            $('#build-create-manual-si').find("[name=errors]").empty();
            $('#build-create-manual-si table').editableTableWidget();
            $("#build-create-manual-si").find("[name=add-statistic-row]").once("click", function () {
                $('#build-create-manual-si table').find("tbody").append(
                    $("<tr>", {
                        "html": $("<th>", {
                            "idn": "row-number",
                            "class": "table-light"
                        }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "code",
                                "label-lookup": "code"
                            }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "value",
                                "label-lookup": "value"
                            }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "unit",
                                "label-lookup": "unit"
                            }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "decimal",
                                "label-lookup": "decimal",
                            }).get(0).outerHTML +
                            $("<th>", {
                                "html": app.library.html.deleteButton()
                            }).get(0).outerHTML
                    }).get(0).outerHTML
                );
                //redraw row numbers
                $('#build-create-manual-si table').find("tbody").find("tr").each(function (index, value) {
                    $(this).find("th").first().text(index + 1);
                });
                $('#build-create-manual-si table').find("button[name=" + C_APP_NAME_LINK_DELETE + "]").once('click', function () {
                    $(this).parent().parent().remove();
                    //redraw row numbers after delete
                    $('#build-create-manual-si table').find("tbody").find("tr").each(function (index, value) {
                        $(this).find("th").first().text(index + 1);
                    });
                    $('#build-create-manual-si').find("[name=errors-card]").hide();
                    $('#build-create-manual-si').find("[name=errors]").empty();
                });
                $('#build-create-manual-si table').editableTableWidget();
            });
            $("#build-create-manual-si").find("[name=add-statistic-row]").trigger("click");
            $("#build-create-statistic").modal("show");
        });
        $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[name=search-classification]").once("click", function () {
            $("#build-create-search-classiication").find("[name=search-classification-language]").text(value.LngIsoName);
            $("#build-create-search-classiication").modal("show");
        });
        $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[name=add-classification]").once("click", function () {
            $('#build-create-manual-classification table').find("tbody").empty();
            $('#build-create-manual-classification').find("[name=errors]").empty();
            $('#build-create-manual-classification').find("[name=errors-card]").hide();
            $('#build-create-manual-classification table').editableTableWidget();
            $("#build-create-manual-classification").find("[name=add-classification-row]").once("click", function () {
                $('#build-create-manual-classification table').find("tbody").append(
                    $("<tr>", {
                        "html": $("<th>", {
                            "idn": "row-number",
                            "class": "table-light"
                        }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "code",
                                "label-lookup": "code"
                            }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "value",
                                "label-lookup": "value"
                            }).get(0).outerHTML +
                            $("<th>", {
                                "html": app.library.html.deleteButton()
                            }).get(0).outerHTML
                    }).get(0).outerHTML
                );
                //redraw row numbers
                $('#build-create-manual-classification table').find("tbody").find("tr").each(function (index, value) {
                    $(this).find("th").first().text(index + 1);
                });
                $('#build-create-manual-classification table').find("button[name=" + C_APP_NAME_LINK_DELETE + "]").once('click', function () {
                    $(this).parent().parent().remove();
                    //redraw row numbers after delete
                    $('#build-create-manual-classification table').find("tbody").find("tr").each(function (index, value) {
                        $(this).find("th").first().text(index + 1);
                    });
                    $('#build-create-manual-classification').find("[name=errors]").empty();
                    $('#build-create-manual-classification').find("[name=errors-card]").hide();
                });
                $('#build-create-manual-classification table').editableTableWidget();

            });
            $("#build-create-manual-classification").find("[name=add-classification-row]").trigger("click");
            $("#build-create-classification").modal("show");
            app.build.create.dimension.validation.manualClassification();
            app.build.create.dimension.validation.uploadClassification();


        });
        $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[name=add-periods]").once("click", function () {

            $('#build-create-manual-periods table').find("tbody").empty();
            $('#build-create-manual-periods').find("[name=manual-periods-errors-card]").hide();
            $('#build-create-manual-periods').find("[name=manual-periods-errors]").empty();

            $('#build-create-manual-periods table').editableTableWidget();

            $("#build-create-manual-periods").find("[name=add-period-row]").once("click", function () {

                $('#build-create-manual-periods table').find("tbody").append(
                    $("<tr>", {
                        "html": $("<th>", {
                            "idn": "row-number",
                            "class": "table-light"
                        }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "code",
                                "label-lookup": "code"
                            }).get(0).outerHTML +
                            $("<td>", {
                                "idn": "value",
                                "label-lookup": "value"
                            }).get(0).outerHTML +
                            $("<th>", {
                                "html": app.library.html.deleteButton()
                            }).get(0).outerHTML
                    }).get(0).outerHTML
                );

                //redraw row numbers
                $('#build-create-manual-periods table').find("tbody").find("tr").each(function (index, value) {
                    $(this).find("th").first().text(index + 1);
                });

                $('#build-create-manual-periods table').find("button[name=" + C_APP_NAME_LINK_DELETE + "]").once('click', function () {
                    $(this).parent().parent().remove();
                    //redraw row numbers after delete
                    $('#build-create-manual-periods table').find("tbody").find("tr").each(function (index, value) {
                        $(this).find("th").first().text(index + 1);
                    });
                    $('#build-create-manual-periods').find("[name=errors-card]").hide();
                    $('#build-create-manual-periods').find("[name=errors]").empty();
                });
                $('#build-create-manual-periods table').editableTableWidget();
            });
            $("#build-create-manual-periods").find("[name=add-period-row]").trigger("click");

            $("#build-create-new-periods").modal("show");
        });

        $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[name=delete-statistics]").once("click", function () {
            api.modal.confirm(
                app.library.html.parseDynamicLabel("confirm-delete-statistics", [$("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-name")]),
                app.build.create.dimension.deleteAllStatistics
            );
        });

        $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[name=delete-classifications]").once("click", function () {
            api.modal.confirm(
                app.library.html.parseDynamicLabel("confirm-delete-classifications", [$("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-name")]),
                app.build.create.dimension.deleteAllClassifications
            );
        });

        $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[name=delete-periods]").once("click", function () {
            api.modal.confirm(
                app.library.html.parseDynamicLabel("confirm-delete-periods", [$("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-name")]),
                app.build.create.dimension.callback.deleteAllPeriods
            );
        });

        $("#build-create-dimension-accordion-" + value.LngIsoCode).find("[name=download-periods]").once("click", app.build.create.dimension.callback.downloadPeriods);



    });
};
//Clear the Dimension tabs.
app.build.create.dimension.clearTabs = function () {
    //clear the properties
    $.each(app.build.create.initiate.languages, function (key, value) {
        app.build.create.dimension.validation.properties(value.LngIsoCode);
        $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
            dimension.Statistic = [];
            dimension.Classification = [];
            dimension.Frequency.Period = [];
        });
        app.build.create.dimension.drawStatistics(value.LngIsoCode);
        app.build.create.dimension.drawClassifications(value.LngIsoCode);
        app.build.create.dimension.drawPeriods(value.LngIsoCode);
        //set default language as active tab
        if (value.LngIsoCode == app.config.language.iso.code) {
            $("#build-create-dimension-nav-" + value.LngIsoCode + "-tab").addClass("active show");
        }
        else {
            $("#build-create-dimension-nav-" + value.LngIsoCode + "-tab").removeClass("active show");
        }
        //set properties accordion to be open
        $("#build-create-dimension-accordion-collapse-properties-" + value.LngIsoCode + "").addClass("show");
        $("#build-create-dimension-accordion-collapse-statistics-" + value.LngIsoCode + "").removeClass("show");
        $("#build-create-dimension-accordion-collapse-classifications-" + value.LngIsoCode + "").removeClass("show");
        $("#build-create-dimension-accordion-collapse-periods-" + value.LngIsoCode + "").removeClass("show");
    });
    //clear the elimination variables
    app.build.create.initiate.data.Elimination = {};
    app.build.create.initiate.data.Map = {};
    app.build.create.dimension.drawElimination();
    app.build.create.dimension.drawMapTable();
};

//#endregion
//#region statistics


/**
 * Cancel statistics upload
 */
app.build.create.dimension.cancelStatisticUpload = function () {
    //clean up 
    app.build.create.file.statistic.content.UTF8 = null;
    $("#build-create-upload-si").find("[name=errors-card]").hide();
    $("#build-create-upload-si").find("[name=errors]").empty();
    $("#build-create-upload-si").find("[name=build-update-upload-periods-file]").val("");
    $("#build-create-upload-si").find("[name=upload-file-name]").empty().hide();
    $("#build-create-upload-si").find("[name=upload-file-tip]").show();
    $("#build-create-upload-si").find("[name=upload-submit-statistics]").prop("disabled", true);
};

/**
 * Cancel statistics upload
 */
app.build.create.dimension.resetStatisticUpload = function () {
    $("#build-create-upload-statistic-file").val("");
    app.build.create.dimension.cancelStatisticUpload();
};

/**
 * Draw Callback for Datatable
 *  @param {*} table
 *  @param {*} LngIsoCode
 */
app.build.create.dimension.drawCallbackDrawStatistics = function (table, LngIsoCode) {
    // Delete action
    $(table).find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        var params = {
            sttCode: $(this).attr("idn"),
            LngIsoCode: LngIsoCode
        };
        app.build.create.dimension.modal.deleteStatistic(params);
    });
}


/**
 *Draw table to hold the Statistics details passing in the chosen language.
 *
 * @param {*} LngIsoCode
 */
app.build.create.dimension.drawStatistics = function (LngIsoCode) {
    var data = null;
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) { //find the data based on the LngIsoCode
        if (dimension.LngIsoCode == LngIsoCode) {
            data = dimension.Statistic;
        }
    });
    var table = $("#build-create-dimension-accordion-" + LngIsoCode).find("[name=statistics-added-table]");
    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            ordering: false,
            columns: [
                {
                    data: "SttCode"
                },
                {
                    data: "SttValue"
                },
                {
                    data: "SttUnit"
                },
                {
                    data: "SttDecimal"
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.SttCode }, false);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.build.create.dimension.drawCallbackDrawStatistics(table, LngIsoCode);
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.create.dimension.drawCallbackDrawStatistics(table, LngIsoCode);
        });
    }
};

/**
 *Confirm the Deletion of a Statistic based on the parameters passed in
 * and call the deleteStatistic function 
 * @param {*} params
 */
app.build.create.dimension.modal.deleteStatistic = function (params) {
    api.modal.confirm(
        app.library.html.parseDynamicLabel("confirm-delete", [params.sttCode]),
        app.build.create.dimension.deleteStatistic,
        params
    );
};

/**
 * Find the Statistic to Delete by the Language code and
 * redraw the table.
 *
 * @param {*} params
 */
app.build.create.dimension.deleteStatistic = function (params) {
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        //find the data based on the LngIsoCode
        if (dimension.LngIsoCode == params.LngIsoCode) {
            //find the record to delete
            dimension.Statistic = $.grep(dimension.Statistic, function (value, index) {
                return value.SttCode == params.sttCode ? false : true;
            });
        }
    });
    app.build.create.dimension.drawStatistics(params.LngIsoCode);
};

/**
 * Delete all statistics for a language
 * redraw the table.
 *
 * @param {*} lngIsoCode
 */
app.build.create.dimension.deleteAllStatistics = function () {
    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) { //find the data based on the LngIsoCode
            dimension.Statistic = [];
        };
    });
    app.build.create.dimension.drawStatistics(lngIsoCode);
};

/**
 * Call on click of the Add Statistics button to validate and add the statistics entered
 * based on the language selected.
 *
 * @param {*} LngIsoCode
 */
app.build.create.dimension.submitManualStatistic = function () {
    $('#build-create-manual-si').find("[name=errors-card]").hide();
    $('#build-create-manual-si').find("[name=errors]").empty();

    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    //check that all inputs of the statistics are valid first
    if (app.build.create.dimension.manualStatistisInvalid()) {
        return;
    }

    var codes = [];
    var values = [];
    var units = [];
    var decimals = [];
    $('#build-create-manual-si table').find("tbody tr").each(function (index) {
        var row = $(this);
        codes.push(row.find("td[idn=code]").text().trim().replace(/ /g, ''));
        values.push(row.find("td[idn=value]").text().trim());
        units.push(row.find("td[idn=unit]").text().trim());
        decimals.push(row.find("td[idn=decimal]").text().trim());
    });

    //check for duplicate statistics
    if (app.build.create.dimension.callback.hasDuplicateStatistic($.extend(true, [], codes), $.extend(true, [], values), $('#build-create-manual-si'))) {
        return;
    }

    //add statistic
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) { //find the statistic you need based on the LngIsoCode and insert new statistics
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each(codes, function (index, value) {
                dimension.Statistic.push({
                    "SttCode": value,
                    "SttValue": values[index],
                    "SttUnit": units[index],
                    "SttDecimal": decimals[index]
                });
            });
        }
    });
    app.build.create.dimension.drawStatistics(lngIsoCode);
    $("#build-create-statistic").modal("hide");

};

/**
 * Upload and validate a CSV file for statistics.
 *
 * @param {*} LngIsoCode
 */
app.build.create.dimension.submitUploadStatistic = function () {
    $("#build-create-upload-si").find("[name=errors-card]").hide();
    $("#build-create-upload-si").find("[name=errors]").empty();

    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    app.build.create.file.statistic.content.data.JSON = Papa.parse(app.build.create.file.statistic.content.UTF8, {
        header: true,
        skipEmptyLines: true
    });

    if (app.build.create.file.statistic.content.data.JSON.errors.length) {
        $("#build-create-upload-si").find("[name=errors-card]").show();
        $('#build-create-upload-si').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    }

    //check that all inputs of the statistics are valid first
    if (app.build.create.dimension.uploadStatistIsInvalid()) {
        return;
    }

    var csvHeaders = app.build.create.file.statistic.content.data.JSON.meta.fields;

    //check that csv has 2 headers
    if (csvHeaders.length != 4) {
        $('#build-create-upload-si').find("[name=errors-card]").show();
        $('#build-create-upload-si').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    };

    //check that csv headers only contain C_APP_CSV_CODE and C_APP_CSV_VALUE, both case sensitive

    if (jQuery.inArray(C_APP_CSV_CODE, csvHeaders) == -1 || jQuery.inArray(C_APP_CSV_VALUE, csvHeaders) == -1 || jQuery.inArray(C_APP_CSV_UNIT, csvHeaders) == -1 || jQuery.inArray(C_APP_CSV_DECIMAL, csvHeaders) == -1) {
        $('#build-create-upload-si').find("[name=errors-card]").show();
        $('#build-create-upload-si').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return
    };

    //check that csv has data
    if (!app.build.create.file.statistic.content.data.JSON.data.length) {
        $('#build-create-upload-si').find("[name=errors-card]").show();
        $('#build-create-upload-si').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    };

    //check for duplicate codes
    var codes = [];
    var values = [];
    var units = [];
    var decimals = [];
    $(app.build.create.file.statistic.content.data.JSON.data).each(function (key, value) {
        codes.push(value[C_APP_CSV_CODE].replace(/ /g, ''));
        values.push(value[C_APP_CSV_VALUE]);
        units.push(value[C_APP_CSV_UNIT]);
        decimals.push(value[C_APP_CSV_DECIMAL]);
    });

    //check for duplicate statistics
    if (app.build.create.dimension.callback.hasDuplicateStatistic($.extend(true, [], codes), $.extend(true, [], values), $('#build-create-upload-si'))) {
        return;
    }

    //add statistic
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) { //find the statistic you need based on the LngIsoCode and insert new statistics
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each(codes, function (index, value) {
                dimension.Statistic.push({
                    "SttCode": value,
                    "SttValue": values[index],
                    "SttUnit": units[index],
                    "SttDecimal": decimals[index]
                });
            });
        }
    });
    app.build.create.dimension.drawStatistics(lngIsoCode);
    $("#build-create-statistic").modal("hide");
};

app.build.create.dimension.callback.hasDuplicateStatistic = function (codes, values, selector) {

    var codes = codes || [];
    var values = values || [];

    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    //Add previous codes to codes array
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            var statistics = dimension.Statistic;
            $(statistics).each(function (key, value) {
                codes.push(value.SttCode.trim().toLowerCase());
                values.push(value.SttValue.trim().toLowerCase());
            });
        }
    });

    if (app.library.utility.arrayHasDuplicate(codes) || app.library.utility.arrayHasDuplicate(values)) {
        selector.find("[name=errors-card]").show();
        selector.find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["create-duplicate-statistic"]
        }));

        return true
    };

    return false

}

/**
 * Validate the Statistics uploaded by CSV file
 *
 *
 * @returns
 */
app.build.create.dimension.uploadStatistIsInvalid = function () {
    var errors = [];
    //check that each rows has correct data
    $.each(app.build.create.file.statistic.content.data.JSON.data, function (index, value) {
        var rowNum = index + 2;
        if (!value[C_APP_CSV_CODE]) {
            errors.push(app.library.html.parseDynamicLabel("create-dimension-upload-error", [C_APP_CSV_CODE, rowNum]));
        }
        if (!value[C_APP_CSV_VALUE]) {
            errors.push(app.library.html.parseDynamicLabel("create-dimension-upload-error", [C_APP_CSV_VALUE, rowNum]));
        }
        if (!value[C_APP_CSV_UNIT]) {
            errors.push(app.library.html.parseDynamicLabel("create-dimension-upload-error", [C_APP_CSV_UNIT, rowNum]));
        }
        if (!value[C_APP_CSV_DECIMAL]) {
            errors.push(app.library.html.parseDynamicLabel("create-dimension-upload-error", [C_APP_CSV_DECIMAL, rowNum]));
        }
    });
    if (!errors.length) {
        $(app.build.create.file.statistic.content.data.JSON.data).each(function (key, value) {
            //validate code
            if (value[C_APP_CSV_CODE].trim().length == 0) {
                errors.push(app.library.html.parseDynamicLabel("code-mandatory", [key + 2]));
            }
            if (value[C_APP_CSV_CODE].trim().length > 256) {
                errors.push(app.library.html.parseDynamicLabel("code-between", [key + 2]));
            }
            if (C_APP_REGEX_NODOUBLEQUOTE.test(value[C_APP_CSV_CODE])) {
                errors.push(app.library.html.parseDynamicLabel("code-double-quote", [key + 2]));
            }
            //validate value
            if (value[C_APP_CSV_VALUE].trim().length == 0) {
                errors.push(app.library.html.parseDynamicLabel("value-mandatory", [key + 2]));
            }
            if (value[C_APP_CSV_VALUE].trim().length > 256) {
                errors.push(app.library.html.parseDynamicLabel("value-between", [key + 2]));
            }
            if (C_APP_REGEX_NODOUBLEQUOTE.test(value[C_APP_CSV_VALUE])) {
                errors.push(app.library.html.parseDynamicLabel("value-double-quote", [key + 2]));
            }
            //validate unit
            if (value[C_APP_CSV_UNIT].trim().length == 0) {
                errors.push(app.library.html.parseDynamicLabel("unit-mandatory", [key + 2]));
            }
            if (value[C_APP_CSV_UNIT].trim().length > 256) {
                errors.push(app.library.html.parseDynamicLabel("unit-between", [key + 2]));
            }
            if (C_APP_REGEX_NODOUBLEQUOTE.test(value[C_APP_CSV_UNIT])) {
                errors.push(app.library.html.parseDynamicLabel("unit-double-quote", [key + 2]));
            }
            //validate decimal
            var decimal = Number(value[C_APP_CSV_DECIMAL].trim());
            if (!value[C_APP_CSV_DECIMAL].length) {
                errors.push(app.library.html.parseDynamicLabel("decimal-mandatory", [key + 2]));
            }
            else if ($.isNumeric(decimal)) {
                if (Number.isInteger(decimal)) {
                    if (decimal < 0 || decimal > 6) {
                        errors.push(app.library.html.parseDynamicLabel("decimal-between", [key + 2]));
                    }
                }
                else {
                    errors.push(app.library.html.parseDynamicLabel("decimal-integer", [key + 2]));
                }
            }
            else {
                errors.push(app.library.html.parseDynamicLabel("decimal-integer", [key + 2]));
            }
        });
    }


    if (errors.length) {
        $('#build-create-upload-si').find("[name=errors-card]").show()
        $.each(errors, function (index, value) {
            $('#build-create-upload-si').find("[name=errors]").append($("<li>", {
                "class": "list-group-item",
                "html": value
            }));
        });
        return true
    }
    else {
        return false
    }
};
//Validate the statistic entered.
app.build.create.dimension.manualStatistisInvalid = function () {
    var errors = [];

    //check for empty cells
    $('#build-create-manual-si table').find("tbody tr").each(function (index) {
        var row = $(this);
        $(this).find("td").each(function () {
            var column = app.label.static[$(this).attr("label-lookup")];
            var value = $(this).text().trim();
            if (value.length == 0) {
                errors.push(app.library.html.parseDynamicLabel("create-mandatory", [row.find("th[idn=row-number]").text(), column]));
            }
            else {
                if ($(this).attr("idn") == "decimal") {
                    var decimal = Number(value);
                    if ($.isNumeric(decimal)) {
                        if (Number.isInteger(decimal)) {
                            if (decimal < 0 || decimal > 6) {
                                errors.push(app.library.html.parseDynamicLabel("create-between", [row.find("th[idn=row-number]").text(), column]))
                            }
                        }
                        else {
                            errors.push(app.library.html.parseDynamicLabel("create-integer", [row.find("th[idn=row-number]").text(), column]))
                        }
                    }
                    else {
                        errors.push(app.library.html.parseDynamicLabel("create-integer", [row.find("th[idn=row-number]").text(), column]))
                    }
                }
                else { //not decimal column                    
                    if ((value.trim().length > 256 && value.trim().length > 0) || C_APP_REGEX_NODOUBLEQUOTE.test(value)) {
                        errors.push(app.library.html.parseDynamicLabel("create-between-characters", [row.find("th[idn=row-number]").text(), column]))
                    }
                }
            }
        });
    });



    if (errors.length) {
        $('#build-create-manual-si').find("[name=errors-card]").show()
        $.each(errors, function (index, value) {
            $('#build-create-manual-si').find("[name=errors]").append($("<li>", {
                "class": "list-group-item",
                "html": value
            }));
        });
        return true
    }
    else {
        return false
    }
};

//#endregion
//#region classifications 

/**
 * Cancel classification upload
 */
app.build.create.dimension.cancelClassificationUpload = function () {
    //clean up 
    app.build.create.file.classification.content.UTF8 = null;
    $("#build-create-upload-classification").find("[name=errors-card]").hide();
    $("#build-create-upload-classification").find("[name=errors]").empty();


    $("#build-create-upload-classification").find("[name=upload-file-name]").empty().hide();
    $("#build-create-upload-classification").find("[name=upload-file-tip]").show();
    $("#build-create-upload-classification").find("[name=upload-submit-classifications]").prop("disabled", true);
}

/**
 * Reset classification upload
 */
app.build.create.dimension.resetClassificationUpload = function () {
    $("#build-create-upload-classification").find("[name=build-create-upload-classification-file]").val("");
    app.build.create.dimension.cancelClassificationUpload();

};

/**
 * Download existing classification
 *  @param {*} clsCode
 *  @param {*} lngIsoCode
 */
app.build.create.dimension.downloadClassification = function (clsCode, lngIsoCode) {
    var classification = null;
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) { //find the data based on the LngIsoCode
            $(dimension.Classification).each(function (key, value) {
                if (value.ClsCode == clsCode) {
                    classification = value;
                }
            });
        }
    });
    var fileData = [];
    $.each(classification.Variable, function (i, row) {
        fileData.push({ [C_APP_CSV_CODE]: row.VrbCode, [C_APP_CSV_VALUE]: row.VrbValue });
    });

    // Download the file
    app.library.utility.download(clsCode, Papa.unparse(fileData), C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
}

/**
 * Draw Callback for Datatable
 *  @param {*} table
 *  @param {*} LngIsoCode
 */
app.build.create.dimension.drawCallbackDrawClassifications = function (table, LngIsoCode) {

    // Delete action
    $(table).find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        var params = {
            ClsCode: $(this).attr("idn"),
            LngIsoCode: LngIsoCode
        };
        app.build.create.dimension.modal.deleteClassification(params);
    });
    $(table).find("[name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
        e.preventDefault();
        app.build.create.dimension.modal.viewClassification($(this).attr("cls-code"), $(this).attr("lng-iso-code"));
    });
}


/**
 * Draw the Classification tabs for the selected language
 *
 * @param {*} LngIsoCode
 */
app.build.create.dimension.drawClassifications = function (LngIsoCode) {
    var data = [];
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) { //find the data based on the LngIsoCode
        if (dimension.LngIsoCode == LngIsoCode) {
            $.each(dimension.Classification, function (index, classification) { //find the data based on the LngIsoCode
                data.push({
                    "ClsCode": classification.ClsCode,
                    "ClsValue": classification.ClsValue,
                    "VariableLength": classification.Variable.length
                });
            });
        }
    });
    //Create accordion for classification
    var table = $("#build-create-dimension-accordion-" + LngIsoCode).find("[name=classifications-added-table]");
    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            ordering: false,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.view({ "cls-code": row.ClsCode, "lng-iso-code": LngIsoCode }, row.ClsCode);
                    }
                },
                {
                    data: "ClsValue"
                },
                {
                    data: "VariableLength"
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.ClsCode }, false);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.build.create.dimension.drawCallbackDrawClassifications(table, LngIsoCode);
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.create.dimension.drawCallbackDrawClassifications(table, LngIsoCode);
        });
    }
};

/**
 * Confirm deletion of classification for parameters entered.
 *
 * @param {*} params
 */
app.build.create.dimension.modal.deleteClassification = function (params) {
    api.modal.confirm(
        app.library.html.parseDynamicLabel("confirm-delete", [params.ClsCode]),
        app.build.create.dimension.deleteClassification,
        params
    );
};

/**
 *Find a classifications based on code and language entered.
 *
 * @param {*} clsCode
 * @param {*} lngIsoCode
 */
app.build.create.dimension.modal.viewClassification = function (clsCode, lngIsoCode) {
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) { //find the data based on the LngIsoCode
            data = dimension.Classification;
            $(data).each(function (key, value) {
                if (value.ClsCode == clsCode) {
                    app.build.create.dimension.callback.viewClassification(value, lngIsoCode);
                }
            });
        }
    });
};

/**
 * Draw table to display details of classifications searched for
 *
 * @param {*} data
 */
app.build.create.dimension.callback.viewClassification = function (data, lngIsoCode) {
    $("#build-create-view-classification").find("[name=title]").text(data.ClsCode + ": " + data.ClsValue);
    $("#build-create-view-classification").find("[name=download]").attr("idn", data.ClsCode).attr("lng-iso-code", lngIsoCode);
    if ($.fn.dataTable.isDataTable("#build-create-view-classification table")) {
        app.library.datatable.reDraw("#build-create-view-classification table", data.Variable);
    } else {
        var localOptions = {
            data: data.Variable,
            ordering: false,
            columns: [
                { data: "VrbCode" },
                { data: "VrbValue" },
            ],
            //Translate labels language
            language: app.label.plugin.datatable
        };
        //Initialize DataTable
        $("#build-create-view-classification table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions));
    }
    $("#build-create-view-classification").modal("show");
};


/**
 * Delete a classification for a selected language and redraw the table.
 *
 * @param {*} params
 */
app.build.create.dimension.deleteClassification = function (params) {
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        //find the data based on the LngIsoCode
        if (dimension.LngIsoCode == params.LngIsoCode) {
            //find the record to delete
            dimension.Classification = $.grep(dimension.Classification, function (value, index) {
                return value.ClsCode == params.ClsCode ? false : true;
            });
            if (params.LngIsoCode == app.config.language.iso.code) {
                delete app.build.create.initiate.data.Elimination[params.ClsCode];
                delete app.build.create.initiate.data.Map[params.ClsCode];
                app.build.create.dimension.drawElimination();
                app.build.create.dimension.drawMapTable();
            }
        }
    });
    app.build.create.dimension.drawClassifications(params.LngIsoCode);
};

/**
 * Delete all classifications for a selected language and redraw the table.
 *
 * @param {*} lngIsoCode
 */
app.build.create.dimension.deleteAllClassifications = function () {
    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) { //find the data based on the LngIsoCode
            dimension.Classification = [];
        }
        if (dimension.LngIsoCode == app.config.language.iso.code) {
            app.build.create.initiate.data.Elimination = {};
            app.build.create.initiate.data.Map = {};
            app.build.create.dimension.drawElimination();
            app.build.create.dimension.drawMapTable();
        };
    });
    app.build.create.dimension.drawClassifications(lngIsoCode);
};

//Add criteria to search for a classification.
app.build.create.dimension.searchClassifications = function () {
    // Click event search-classifications-modal-search-input
    $("#build-create-search-classiication").find("[name=classifications-search-input]").on('keyup', function (e) {
        e.preventDefault();
        if (e.keyCode == 13) {
            var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
            app.build.create.dimension.ajax.searchClassifications(lngIsoCode);
        }
    });
    // Click eventsearch-classifications-modal-search-button
    $("#build-create-search-classiication").find("[name=search-classifications-modal-search-button]").on('click', function (e) {
        e.preventDefault();
        var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
        app.build.create.dimension.ajax.searchClassifications(lngIsoCode);
    });
};

/**
 * Search for a classification with search criteria and language code.
 *
 * @param {*} search
 * @param {*} lngIsoCode
 */
app.build.create.dimension.ajax.searchClassifications = function (lngIsoCode) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Classification_API.Search",
        {
            "Search": $("#build-create-search-classiication").find("[name=classifications-search-input]").val().trim(),
            "LngIsoCode": lngIsoCode
        },
        "app.build.create.dimension.callback.searchClassifications",
        null,
        null,
        null,
        { async: false });
};

/**
 * Get classification details returned from Search.
 *
 * @param {*} data
 */
app.build.create.dimension.callback.searchClassifications = function (data) {
    app.build.create.dimension.callback.drawSearchClassifications(data);
};

/**
 * Draw Callback for Datatable
 *  @param {*} data
 */
app.build.create.dimension.drawCallbackSearchClassification = function (data) {
    $('[data-toggle="tooltip"]').tooltip();
    $("#build-create-search-classiication table[name=search-classifications-list-table] [name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
        e.preventDefault();
        app.build.create.dimension.ajax.readClassification($(this).attr("cls-id"));

    });

    $("#build-create-search-classiication table[name=search-classifications-list-table] [name=" + C_APP_NAME_LINK_GEOJSON + "]").once("click", function (e) {
        e.preventDefault();
        app.geomap.preview.ajax.readMap($(this).attr("gmp-code"))
    });
    // Responsive
    $("#search-classifications-modal table[name=search-classifications-list-table]").DataTable().columns.adjust().responsive.recalc();
}

/**
 * Draw the Classification table adding the details returned from the search.
 *
 * @param {*} searchResults
 */
app.build.create.dimension.callback.drawSearchClassifications = function (searchResults) {
    //hide any previous classification variable table
    $("#build-create-search-classiication").find("[name=read-classification-table-container]").hide();

    if ($.fn.dataTable.isDataTable("#build-create-search-classiication table[name=search-classifications-list-table]")) {
        app.library.datatable.reDraw("#build-create-search-classiication table[name=search-classifications-list-table]", searchResults);
    } else {
        var localOptions = {
            data: searchResults,
            columns: [
                { data: "MtrCode" },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.view({ "cls-code": row.ClsCode, "cls-id": row.ClsID }, row.ClsCode);
                    }
                },
                { data: "VrbCount" },
                { data: "ClsValue" },
                {
                    data: null,
                    render: function (data, type, row) {
                        if (row.ClsGeoUrl) {
                            var truncatedUrl = $.trim(row.ClsGeoUrl).substring(0, 20)
                                .trim(this) + "...";
                            return app.library.html.link.geoJson({ "gmp-code": row.GmpCode }, truncatedUrl, app.label.static["view-map"]);
                        }
                        else {
                            return null
                        }

                    }
                }
            ],
            drawCallback: function (settings) {
                app.build.create.dimension.drawCallbackSearchClassification(searchResults);
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        //Initialize DataTable
        $("#build-create-search-classiication table[name=search-classifications-list-table]").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.create.dimension.drawCallbackSearchClassification(searchResults);
        });
    }
    $("#build-create-search-classiication").find("[name=search-classifications-list-table-container]").hide().fadeIn();
};

/**
 * Read Classification from the database
 *
 * @param {*} apiParams
 * @param {*} callbackParams
 */
app.build.create.dimension.ajax.readClassification = function (classificationId) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Classification_API.Read",
        {
            "ClsID": classificationId,
        },
        "app.build.create.dimension.callback.readClassification",
        null,
        null,
        null,
        { async: false });
};

//Create a classification
app.build.create.dimension.callback.buildManualClassification = function () {
    $("#build-create-manual-classification").find("[name=errors]").empty();

    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    var code = $("#build-create-manual-classification").find("[name=cls-code]").val();
    var value = $("#build-create-manual-classification").find("[name=cls-value]").val();

    //check for duplicate classifications

    if (app.build.create.dimension.callback.hasDuplicateClassification(code, value, $("#build-create-manual-classification"))) {
        return;
    }

    //check for duplicate variables
    var variableCodes = [];
    var variableValues = [];

    $('#build-create-manual-classification table').find("tbody tr").each(function (index) {
        //populate codes array to check for duplicates
        variableCodes.push($(this).find("td[idn=code]").text().trim().replace(/ /g, ''));
        variableValues.push($(this).find("td[idn=value]").text().trim());
    });


    if (app.build.create.dimension.callback.classificationHasInvalidVariables(variableCodes, variableValues, $('#build-create-manual-classification'))) {
        return;
    }

    var classification = {
        "ClsCode": code,
        "ClsValue": value,
        "Variable": []
    };

    $.each(variableCodes, function (index, variable) {
        classification.Variable.push({
            "VrbCode": variable,
            "VrbValue": variableValues[index]
        });
    });

    //add classification
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) { //find the classification you need based on the LngIsoCode and insert new statistics
        if (dimension.LngIsoCode == lngIsoCode) {
            dimension.Classification.push(classification);
            if (dimension.LngIsoCode == app.config.language.iso.code) {
                app.build.create.initiate.data.Elimination[classification.ClsCode] = null;
                app.build.create.initiate.data.Map[classification.ClsCode] = null;
                app.build.create.dimension.drawElimination();
                app.build.create.dimension.drawMapTable();
            }
        }
    });
    $("#build-create-manual-classification").find("[name=errors]").empty();
    app.build.create.dimension.drawClassifications(lngIsoCode);
    $("#build-create-classification").modal("hide");
};

/**
 * Get callback from server and Create classifications from uploaded CSV file'
 *
 * @returns
 */
app.build.create.dimension.callback.buildUploadClassification = function () {
    $("#build-create-upload-classification").find("[name=errors-card]").hide();
    $("#build-create-upload-classification").find("[name=errors]").empty();

    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");


    app.build.create.file.classification.content.data.JSON = Papa.parse(app.build.create.file.classification.content.UTF8, {
        header: true,
        skipEmptyLines: true
    });

    if (app.build.create.file.classification.content.data.JSON.errors.length) {
        $("#build-create-upload-classification").find("[name=errors-card]").show();
        $('#build-create-upload-classification').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    }

    var csvHeaders = app.build.create.file.classification.content.data.JSON.meta.fields;

    //check that csv has 2 headers
    if (csvHeaders.length != 2) {
        $("#build-create-upload-classification").find("[name=errors-card]").show();
        $('#build-create-upload-classification').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    };

    //check that csv headers only contain C_APP_CSV_CODE and C_APP_CSV_VALUE, both case sensitive

    if (jQuery.inArray(C_APP_CSV_CODE, csvHeaders) == -1 || jQuery.inArray(C_APP_CSV_VALUE, csvHeaders) == -1) {
        $("#build-create-upload-classification").find("[name=errors-card]").show();
        $('#build-create-upload-classification').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    };

    //check that csv has data
    if (!app.build.create.file.classification.content.data.JSON.data.length) {
        $("#build-create-upload-classification").find("[name=errors-card]").show();
        $('#build-create-upload-classification').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    };

    //check that each rows has correct data
    $.each(app.build.create.file.classification.content.data.JSON.data, function (index, value) {
        if (!value[C_APP_CSV_CODE] || !value[C_APP_CSV_VALUE]) {
            $("#build-create-upload-classification").find("[name=errors-card]").show();
            $('#build-create-upload-classification').find("[name=errors]").append($("<li>", {
                "class": "list-group-item",
                "html": app.label.static["invalid-csv-format"]
            }));
            return;
        }
    });

    var code = $("#build-create-upload-classification").find("[name=cls-code]").val();
    var value = $("#build-create-upload-classification").find("[name=cls-value]").val();

    //check for duplicate classifications
    if (app.build.create.dimension.callback.hasDuplicateClassification(code, value, $("#build-create-upload-classification"))) {
        return;
    }

    //Check for invalid variables
    var variableCodes = [];
    var variableValues = [];
    $(app.build.create.file.classification.content.data.JSON.data).each(function (key, value) {
        variableCodes.push(value[C_APP_CSV_CODE].trim().replace(/ /g, ''));
        variableValues.push(value[C_APP_CSV_VALUE].trim());
    });

    if (app.build.create.dimension.callback.classificationHasInvalidVariables(variableCodes, variableValues, $("#build-create-upload-classification"))) {
        return;
    }

    var classification = {
        "ClsCode": code,
        "ClsValue": value,
        "Variable": []
    };

    $.each(variableCodes, function (index, variable) {
        classification.Variable.push({
            "VrbCode": variable,
            "VrbValue": variableValues[index]
        });
    });

    //add classification
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) { //find the classification you need based on the LngIsoCode and insert new statistics
        if (dimension.LngIsoCode == lngIsoCode) {
            dimension.Classification.push(classification);
            if (dimension.LngIsoCode == app.config.language.iso.code) {
                app.build.create.initiate.data.Elimination[classification.ClsCode] = null;
                app.build.create.initiate.data.Map[classification.ClsCode] = null;
                app.build.create.dimension.drawElimination();
                app.build.create.dimension.drawMapTable();
            }
        }
    });

    $("#build-create-upload-classification").find("[name=errors]").empty();
    $("#build-create-upload-classification").find("[name=errors-card]").hide();
    app.build.create.dimension.drawClassifications(lngIsoCode);
    $("#build-create-classification").modal("hide");
};

app.build.create.dimension.callback.hasDuplicateClassification = function (code, value, selector) {
    var codes = [];
    var values = [];

    codes.push(code);
    values.push(value);

    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    //Add previous codes to codes array
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) { //find the data based on the LngIsoCode
        if (dimension.LngIsoCode == lngIsoCode) {
            var classifications = dimension.Classification;
            $(classifications).each(function (key, value) {
                codes.push(value.ClsCode.trim());
                values.push(value.ClsValue.trim());
            });
        }
    });

    if (app.library.utility.arrayHasDuplicate(codes) || app.library.utility.arrayHasDuplicate(values)) {
        selector.find("[name=errors-card]").show();
        selector.find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["create-duplicate-classification"]
        }));
        return true
    };

    return false
};

app.build.create.dimension.callback.classificationHasInvalidVariables = function (variableCodes, variableValues, selector) {
    var isInValid = false;

    if (!variableCodes.length) {
        selector.find("[name=errors-card]").show();
        selector.find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["create-variable"]
        }));
        isInValid = true;
        return isInValid;
    }

    if (app.library.utility.arrayHasDuplicate($.extend(true, [], variableCodes)) || app.library.utility.arrayHasDuplicate($.extend(true, [], variableValues))) {
        selector.find("[name=errors-card]").show();
        selector.find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["create-duplicate-variable"]
        }));
        isInValid = true;
        return isInValid;
    }

    $.each(variableCodes, function (index, variable) {
        if (variable.length > 256
            || !variable.length
            || variableValues[index].length > 256
            || !variableValues[index].length
            || C_APP_REGEX_NODOUBLEQUOTE.test(variable)
            || C_APP_REGEX_NODOUBLEQUOTE.test(variableValues[index])) { //invalidate variable
            var errorRowNo = null;
            switch (selector[0].id) {
                case "build-create-manual-classification":
                    errorRowNo = index + 1
                    break;

                case "build-create-upload-classification":
                    errorRowNo = index + 2
                    break;
            }

            selector.find("[name=errors-card]").show();
            selector.find("[name=errors]").append($("<li>", {
                "class": "list-group-item",
                "html": app.library.html.parseDynamicLabel("create-code-values", [errorRowNo])
            }));
            isInValid = true;
            return;
        }
    });

    return isInValid
}


/**
 * Callback from server after reading data : app.build.create.dimension.callback.readClassification
 * @param  {} data
 */
app.build.create.dimension.callback.readClassification = function (data) {
    app.build.create.dimension.callback.drawClassification(data);

    $('#build-create-search-classiication').animate({
        scrollTop: '+=' + $('#build-create-search-classiication [name=read-classification-table]')[0].getBoundingClientRect().top
    }, 1000);
};


/**
 * Draw Callback for Datatable
 */
app.build.create.dimension.drawCallbackDrawClassification = function () {
    // Responsive
    $("#build-create-search-classiication table[name=read-classification-table]").DataTable().columns.adjust().responsive.recalc();
}


/**
 * Draw table to display classifications.
 *
 * @param {*} data
 * @param {*} callbackParams
 */
app.build.create.dimension.callback.drawClassification = function (classification) {
    $("#build-create-search-classiication").find("[name=read-classification-table-label]").text(classification[0].ClsCode + ": " + classification[0].ClsValue);
    $("#build-create-search-classiication").find("[name=read-classification-table-container]").show();
    if ($.fn.dataTable.isDataTable("#build-create-search-classiication table[name=read-classification-table]")) {
        app.library.datatable.reDraw("#build-create-search-classiication table[name=read-classification-table]", classification);
    } else {
        var localOptions = {
            data: classification,
            columns: [
                { data: "VrbCode" },
                { data: "VrbValue" },
                {
                    data: null,
                    type: "natural",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.VrbEliminationFlag, true);
                    }
                }
            ],
            drawCallback: function (settings) {
                // Responsive             
                app.build.create.dimension.drawCallbackDrawClassification();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        //Initialize DataTable
        $("#build-create-search-classiication table[name=read-classification-table]").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            // Responsive             
            app.build.create.dimension.drawCallbackDrawClassification();
        });
        window.onresize = function () {
            // Responsive
            $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
        };
    }
    // Responsive
    $("#build-create-search-classiication table[name=read-classification-table]").DataTable().columns.adjust().responsive.recalc();
    $("#build-create-search-classiication").find("[name=read-classification-table-container]").hide().fadeIn();
    $("#build-create-search-classiication").find("[name=use-classification]").once("click", function () {
        app.build.create.dimension.callback.useClassification(classification);
    });
    $("#build-create-search-classiication").find("[name=download-classification]").once("click", function () {
        app.build.create.dimension.callback.downloadSearchClassification(classification);
    });

};

/**
 *Callback to add classifications to table
 *
 * @param {*} variables
 */
app.build.create.dimension.callback.useClassification = function (variables) {
    var eliminationVariable = null;
    $.each(variables, function (index, value) {
        if (value.VrbEliminationFlag) {
            eliminationVariable = value.VrbCode;
            return
        }
    });

    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
    var classification = {
        "ClsCode": null,
        "ClsValue": null,
        "ClsGeoUrl": null,
        "Variable": []
    };
    //check if classification already added
    var codes = [];
    var values = [];
    //add this classification code to array
    codes.push(variables[0].ClsCode.toLowerCase());
    values.push(variables[0].ClsValue.toLowerCase());
    //Add previous codes to codes array
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) { //find the data based on the LngIsoCode
        if (dimension.LngIsoCode == lngIsoCode) {
            var classifications = dimension.Classification;
            $(classifications).each(function (key, value) {
                codes.push(value.ClsCode.trim().toLowerCase());
                values.push(value.ClsValue.trim().toLowerCase());
            });
        }
    });
    //Check for duplicate codes
    if (!app.library.utility.arrayHasDuplicate(codes) && !app.library.utility.arrayHasDuplicate(values)) { //classification not already added
        classification.ClsCode = variables[0].ClsCode;
        classification.ClsValue = variables[0].ClsValue;
        classification.ClsGeoUrl = variables[0].ClsGeoUrl;
        $.each(variables, function (key, value) {
            classification.Variable.push({
                "VrbCode": value.VrbCode,
                "VrbValue": value.VrbValue
            });
        });
        //insert classification into API object
        //Add previous codes to codes array
        $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
            if (dimension.LngIsoCode == lngIsoCode) {
                var classifications = dimension.Classification;
                classifications.push(classification);
            }
            // Display Success Modal
            api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [variables[0].ClsCode]));
        });
    }
    else {
        api.modal.error(app.label.static["create-duplicate-classification"]);
    }
    app.build.create.dimension.drawClassifications(lngIsoCode);

    //elimination
    app.build.create.initiate.data.Elimination[variables[0].ClsCode] = eliminationVariable;
    app.build.create.initiate.data.Map[variables[0].ClsCode] = variables[0].ClsGeoUrl;
    app.build.create.dimension.drawElimination();
    app.build.create.dimension.drawMapTable();
};

/**
 * Download classifications in CSV format
 *
 * @param {*} data
 * @param {*} callbackParams
 */
app.build.create.dimension.callback.downloadSearchClassification = function (variables) {
    var fileData = [];
    $.each(variables, function (i, row) {
        fileData.push({ [C_APP_CSV_CODE]: row.VrbCode, [C_APP_CSV_VALUE]: row.VrbValue });
    });

    // Download the file
    //app.library.utility.download(variables[0].ClsCode, encodeURIComponent(Papa.unparse(fileData)), C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
    app.library.utility.download(variables[0].ClsCode, Papa.unparse(fileData), C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
};
//#endregion
//#region periods

/**
 * Cancel Period upload
 */
app.build.create.dimension.cancelPeriodUpload = function () {
    //clean up 
    app.build.create.file.period.content.UTF8 = null;
    $("#build-create-upload-periods").find("[name=errors-card]").hide();
    $("#build-create-upload-periods").find("[name=errors]").empty();


    $("#build-create-upload-periods").find("[name=upload-file-name]").empty().hide();
    $("#build-create-upload-periods").find("[name=upload-file-tip]").show();
    $("#build-create-upload-periods").find("[name=upload-submit-periods]").prop("disabled", true);
}

/**
 * Reset Period upload
 */
app.build.create.dimension.resetPeriodUpload = function () {
    $("#build-create-upload-periods-file").val("");
    app.build.create.dimension.cancelPeriodUpload();
}

app.build.create.dimension.addPeriodsManual = function () {
    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
    $('#build-create-manual-periods').find("[name=errors-card]").hide();
    $('#build-create-manual-periods').find("[name=errors]").empty();
    app.build.create.dimension.periodsManualValid = true;

    if (app.build.create.dimension.manualPeriodsInvalid()) {
        return
    }

    //valid inputs, continue
    // if (app.build.create.dimension.periodsManualValid) {
    var codes = [];
    var values = [];
    //add new periods to array
    $('#build-create-manual-periods table').find("tbody tr").each(function (index) {
        var row = $(this);
        codes.push(row.find("td[idn=code]").text().trim().replace(/ /g, ''));
        values.push(row.find("td[idn=value]").text().trim());
    });

    if (app.build.create.hasDuplicatePeriods($.extend(true, [], codes), $.extend(true, [], values), $('#build-create-manual-periods'))) {
        return;
    }


    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each(codes, function (index, code) {
                dimension.Frequency.Period.push({
                    PrdCode: code,
                    PrdValue: values[index]
                });
            });
        }
    });
    $("#build-create-upload-periods").find("[name=upload-periods-errors]").empty();
    $("#build-create-upload-periods").find("[name=upload-periods-errors-card]").hide();
    app.build.create.dimension.drawPeriods(lngIsoCode);
    $("#build-create-new-periods").modal("hide");

};

app.build.create.dimension.addPeriodsUpload = function () {
    $("#build-create-upload-periods").find("[name=errors]").empty();
    $("#build-create-upload-periods").find("[name=errors-card]").hide();

    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    app.build.create.file.period.content.data.JSON = Papa.parse(app.build.create.file.period.content.UTF8, {
        header: true,
        skipEmptyLines: true
    });

    if (app.build.create.file.period.content.data.JSON.errors.length) {
        $("#build-create-upload-periods").find("[name=errors-card]").show();
        $('#build-create-upload-periods').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return;
    }
    if (app.build.create.dimension.uploadPeriodsInvalid()) {
        return;
    };

    //chech for duplicate period values
    var codes = [];
    var values = [];
    $(app.build.create.file.period.content.data.JSON.data).each(function (key, value) {
        codes.push(value[C_APP_CSV_CODE].trim().toLowerCase().replace(/ /g, ''));
        values.push(value[C_APP_CSV_VALUE].trim().toLowerCase());
    });


    if (app.build.create.hasDuplicatePeriods($.extend(true, [], codes), $.extend(true, [], values), $('#build-create-upload-periods'))) {
        return;
    }

    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each(app.build.create.file.period.content.data.JSON.data, function (index, variable) {
                dimension.Frequency.Period.push({
                    PrdCode: variable[C_APP_CSV_CODE],
                    PrdValue: variable[C_APP_CSV_VALUE]
                });
            });
        }
    });
    $("#build-create-upload-periods").find("[name=upload-periods-errors]").empty();
    $("#build-create-upload-periods").find("[name=upload-periods-errors-card]").hide();
    app.build.create.dimension.drawPeriods(lngIsoCode);
    $("#build-create-new-periods").modal("hide");
};

//Validate the statistic entered.
app.build.create.dimension.manualPeriodsInvalid = function () {
    var errors = [];
    //check for empty cells
    $('#build-create-manual-periods table').find("tbody tr").each(function (index) {
        var row = $(this);
        $(this).find("td").each(function () {

            var column = app.label.static[$(this).attr("label-lookup")];

            var value = $(this).text().trim();

            if (value.length == 0) {
                errors.push(app.library.html.parseDynamicLabel("create-mandatory", [row.find("th[idn=row-number]").text(), column]));
            }
            else {
                if ((value.trim().length > 256 && value.trim().length > 0) || C_APP_REGEX_NODOUBLEQUOTE.test(value)) {
                    errors.push(app.library.html.parseDynamicLabel("create-between-characters", [row.find("th[idn=row-number]").text(), column]));
                }
            }
        });

    });

    if (errors.length) {
        $('#build-create-manual-periods').find("[name=errors-card]").show()
        $.each(errors, function (index, value) {
            $('#build-create-manual-periods').find("[name=errors]").append($("<li>", {
                "class": "list-group-item",
                "html": value
            }));
        });
        return true
    }
    else {
        return false
    }
};

app.build.create.dimension.uploadPeriodsInvalid = function () {
    if (app.build.create.file.period.content.data.JSON.errors.length) {
        $('#build-create-upload-periods').find("[name=errors-card]").show()
        $('#build-create-upload-periods').find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["invalid-csv-format"]
        }));
        return true
    }

    var errors = [];

    //check that csv contains data
    if (!app.build.create.file.period.content.data.JSON.data.length) {
        errors.push(app.label.static["update-time-point"]);
    };

    var csvHeaders = app.build.create.file.period.content.data.JSON.meta.fields;

    //check that csv headers contain C_APP_CSV_CODE and C_APP_CSV_VALUE, both case sensitive
    if (jQuery.inArray(C_APP_CSV_CODE, csvHeaders) == -1 || jQuery.inArray(C_APP_CSV_VALUE, csvHeaders) == -1) {
        app.library.html.parseDynamicLabel("invalid-csv-format-code-value", [C_APP_CSV_CODE, C_APP_CSV_VALUE]);
    };

    //check that each rows has correct data
    $.each(app.build.create.file.period.content.data.JSON.data, function (index, value) {
        var rowNum = index + 2;
        if (!value[C_APP_CSV_CODE]) {
            errors.push(app.library.html.parseDynamicLabel("create-dimension-upload-error", [C_APP_CSV_CODE, rowNum]));
        }

        if (!value[C_APP_CSV_VALUE]) {
            errors.push(app.library.html.parseDynamicLabel("create-dimension-upload-error", [C_APP_CSV_VALUE, rowNum]));
        }
    });

    //trim and sanitise
    $(app.build.create.file.period.content.data.JSON.data).each(function (key, value) {
        value[C_APP_CSV_CODE] = value[C_APP_CSV_CODE].trim();
        value[C_APP_CSV_VALUE] = value[C_APP_CSV_VALUE].trim();

        //check that variable lengths are valid
        if (value[C_APP_CSV_CODE].length > 256 ||
            value[C_APP_CSV_CODE].length == 0 ||
            value[C_APP_CSV_VALUE].length > 256 ||
            value[C_APP_CSV_VALUE].length == 0 ||
            C_APP_REGEX_NODOUBLEQUOTE.test(value[C_APP_CSV_CODE]) ||
            C_APP_REGEX_NODOUBLEQUOTE.test(value[C_APP_CSV_VALUE])) {
            errors.push(app.library.html.parseDynamicLabel("update-invalid-variable", [key + 2]));
        }
    });

    if (errors.length) {
        $('#build-create-upload-periods').find("[name=errors-card]").show()
        $.each(errors, function (index, value) {
            $('#build-create-upload-periods').find("[name=errors]").append($("<li>", {
                "class": "list-group-item",
                "html": value
            }));
        });
        return true
    }
    else {
        return false
    }
};

app.build.create.hasDuplicatePeriods = function (codes, values, selector) {
    codes = codes || [];
    values = values || [];
    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");

    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            $.each(dimension.Frequency.Period, function (index, variable) {
                codes.push(variable.PrdCode);
                values.push(variable.PrdValue);
            });
        }
    });

    if (app.library.utility.arrayHasDuplicate(codes) || app.library.utility.arrayHasDuplicate(values)) {
        selector.find("[name=errors-card]").show();
        selector.find("[name=errors]").append($("<li>", {
            "class": "list-group-item",
            "html": app.label.static["build-duplicate-period"]
        }));
        return true;
    }

    return false;

}

/**
 * Draw Callback for Datatable
 * @param {*} table
 * @param {*} lngIsoCode
 */
app.build.create.dimension.drawCallbackDrawPeriods = function (table, lngIsoCode) {
    // Delete action               
    $(table).find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        var params = {
            lngIsoCode: lngIsoCode,
            prdCode: $(this).attr("idn")
        };
        app.build.create.dimension.modal.deletePeriod(params);
    });
}

/**
 *Draw periods dimension
 * @param {*} lngIsoCode
 * @param {*} data
 */
app.build.create.dimension.drawPeriods = function (lngIsoCode) {
    var data = [];
    $(app.build.create.initiate.data.Dimension).each(function (index, dimension) {
        if (lngIsoCode == dimension.LngIsoCode) {
            data = $.extend(true, [], dimension.Frequency.Period);
        }
    });
    //sort descending 
    data.sort(function (a, b) {
        return b.PrdCode - a.PrdCode
    });

    var table = $("#build-create-dimension-accordion-" + lngIsoCode).find("[name=periods-added-table]");
    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            ordering: false,
            columns: [
                {
                    data: "PrdCode"
                },
                {
                    data: "PrdValue"
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.PrdCode }, false);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.build.create.dimension.drawCallbackDrawPeriods(table, lngIsoCode);
            },

        };

        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.create.dimension.drawCallbackDrawPeriods(table, lngIsoCode);
        });
    }

};

/**
 * Confirm deletion of classification for parameters entered.
 *
 * @param {*} params
 */
app.build.create.dimension.modal.deletePeriod = function (params) {
    api.modal.confirm(
        app.library.html.parseDynamicLabel("confirm-delete", [params.prdCode]),
        app.build.create.dimension.callback.deletePeriod,
        params
    );
};

app.build.create.dimension.callback.deletePeriod = function (params) {
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == params.lngIsoCode) {
            dimension.Frequency.Period = $.grep(dimension.Frequency.Period, function (value, index) {
                return value.PrdCode == params.prdCode ? false : true;
            });
        }
    });
    app.build.create.dimension.drawPeriods(params.lngIsoCode);
};

app.build.create.dimension.callback.deleteAllPeriods = function () {
    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            dimension.Frequency.Period = [];
        }
    });
    app.build.create.dimension.drawPeriods(lngIsoCode);
};

app.build.create.dimension.callback.downloadPeriods = function () {
    var lngIsoCode = $("#build-create-matrix-dimensions").find("[name=nav-lng-tab-item].active").attr("lng-iso-code");
    var periods = null;
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        if (dimension.LngIsoCode == lngIsoCode) {
            periods = dimension.Frequency.Period;
        }
    });
    var fileData = [];

    if (periods.length) {
        $.each(periods, function (i, row) {
            fileData.push({ [C_APP_CSV_CODE]: row.PrdCode, [C_APP_CSV_VALUE]: row.PrdValue });
        });
    }
    else {
        fileData.push({ [C_APP_CSV_CODE]: "", [C_APP_CSV_VALUE]: "" });
    }

    // Download the file
    app.library.utility.download(app.build.create.initiate.data.FrqCode, Papa.unparse(fileData), C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
};


//#endregion
//#region submit object

//Build the PX file
app.build.create.dimension.buildDataObject = function () {
    var errors = [];
    var numStatistics = [];
    var numClassifications = [];
    //app.build.create.dimension.dataObjectValid
    $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
        var dimensionCodes = [];
        var dimensionValues = [];
        var lngIsoCode = dimension.LngIsoCode;
        var lngIsoName = null;
        $.each(app.build.create.initiate.languages, function (key, value) {
            if (lngIsoCode == value.LngIsoCode) {
                lngIsoName = value.LngIsoName;
            }
        });
        //MtrTitle
        dimension.MtrTitle = $("#build-create-dimension-accordion-" + lngIsoCode).find("[name=title-value]").val().trim();
        //StatisticLabel
        dimension.StatisticLabel = $("#build-create-dimension-accordion-" + lngIsoCode).find("[name=statistic-label]").val().trim();
        //Note
        var tinyMceId = $("#build-create-dimension-accordion-" + lngIsoCode).find("[name=note-value]").attr("id");
        dimension.MtrNote = tinymce.get(tinyMceId).getContent();
        //Frequency Label
        dimension.Frequency.FrqValue = $("#build-create-dimension-accordion-" + lngIsoCode).find("[name=frequency-value]").val().trim();

        //check for at least one statistic one classification and one period
        if (!dimension.Statistic.length) {
            errors.push(app.library.html.parseDynamicLabel("create-statistic", [lngIsoName]));
            return;
        }
        if (!dimension.Classification.length) {
            errors.push(app.library.html.parseDynamicLabel("create_classification", [lngIsoName]));
            return;
        }
        if (!dimension.Frequency.Period.length) {
            errors.push(app.library.html.parseDynamicLabel("create-period", [lngIsoName]));
            return;
        }
        //get length of statistics and classifications per language
        numStatistics.push(dimension.Statistic.length);
        numClassifications.push(dimension.Classification.length);
        //check statistic codes per language
        $.each(dimension.Classification, function (key, value) {
            dimensionCodes.push(value.ClsCode);
            dimensionValues.push(value.ClsValue);
        });
        dimensionValues.push(dimension.StatisticLabel);
        dimensionValues.push(dimension.Frequency.FrqValue);
        if (app.library.utility.arrayHasDuplicate(dimensionCodes)) {
            errors.push(app.library.html.parseDynamicLabel("create-dimension-code", [lngIsoName]));
        }
        if (app.library.utility.arrayHasDuplicate(dimensionValues)) {
            errors.push(app.library.html.parseDynamicLabel("create-dimension-value", [lngIsoName]));
        }
    });
    if (!errors.length) {
        //check if number of statistics are equal for all languages
        if (!numStatistics.every((val, i, arr) => val === arr[0])) {
            errors.push(app.label.static["create-statistic-equal"]);
        }
        //check if number of classifications are equal for all languages
        if (!numClassifications.every((val, i, arr) => val === arr[0])) {
            errors.push(app.label.static["create-classification-equal"]);
        }
    }
    if (!errors.length) {
        //compare default language statistic codes, statistical decimal and classification codes to every other language. 
        var defaultLngStatisticCodes = [];
        var defaultLngStatisticDecimals = [];
        var defaultLngClassificationCodes = [];
        $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
            if (dimension.LngIsoCode == app.config.language.iso.code) {
                $.each(dimension.Statistic, function (index, statistic) {
                    defaultLngStatisticCodes.push(statistic.SttCode);
                    defaultLngStatisticDecimals.push(statistic.SttDecimal);
                });

                $.each(dimension.Classification, function (index, statistic) {
                    defaultLngClassificationCodes.push(statistic.ClsCode);
                });
            }
        });
        $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
            var lngIsoCode = dimension.LngIsoCode;
            var lngIsoName = null;
            $.each(app.build.create.initiate.languages, function (key, value) {
                if (lngIsoCode == value.LngIsoCode) {
                    lngIsoName = value.LngIsoName;
                }
            });
            var statisticCodes = [];
            var statisticDecimals = [];
            var classificationCodes = [];
            if (lngIsoCode != app.config.language.iso.code) {
                $.each(dimension.Statistic, function (index, statistic) {
                    statisticCodes.push(statistic.SttCode);
                    statisticDecimals.push(statistic.SttDecimal);
                });
                // If not the same, throw error
                if (!app.library.utility.arraysEqual(defaultLngStatisticCodes, statisticCodes, true)) {
                    errors.push(app.label.static["create-statistic-code"]);
                }
                if (!app.library.utility.arraysEqual(defaultLngStatisticDecimals, statisticDecimals, true)) {
                    errors.push(app.label.static["create-statistic-decimal"]);
                }

                $.each(dimension.Classification, function (index, classification) {
                    classificationCodes.push(classification.ClsCode);
                });
                // If not the same, throw error
                if (!app.library.utility.arraysEqual(defaultLngClassificationCodes, classificationCodes, true)) {
                    errors.push(app.label.static["create-classification-code"]);
                }
            }
        });
        if (!errors.length) {


            //check that variable codes for each classification are the same accross all languages
            //loop through each classification
            $.each(defaultLngClassificationCodes, function (index, classification) {
                var defaultVariableCodes = [];
                var compareVariableCodes = [];
                var compareLngIsoName = null;
                var defaultLngClassificationCode = null;


                //loop through dimensions and find variable codes for default language for this classification
                $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
                    if (dimension.LngIsoCode == app.config.language.iso.code) { //default language
                        //loop through the classifications in default language to get specific classification and then put variables in array
                        $.each(dimension.Classification, function (index, dimensionClassification) {
                            if (dimensionClassification.ClsCode == classification) {
                                //this is the classification we are looking for, put variable codes into array
                                $.each(dimensionClassification.Variable, function (index, variable) {
                                    defaultVariableCodes.push(variable.VrbCode)
                                });

                            }
                        });
                    }

                });

                //loop through dimensions and find variable codes for other languages for this classification
                $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
                    if (dimension.LngIsoCode != app.config.language.iso.code) { //not default language
                        $.each(app.build.create.initiate.languages, function (key, value) {
                            if (dimension.LngIsoCode == value.LngIsoCode) {
                                compareLngIsoName = value.LngIsoName;
                            }
                        });

                        //loop through the classifications in this language to get specific classification and then put variables in array
                        $.each(dimension.Classification, function (index, dimensionClassification) {
                            if (dimensionClassification.ClsCode == classification) {
                                defaultLngClassificationCode = dimensionClassification.ClsCode;
                                //this is the classification we are looking for, put variable codes into array
                                compareVariableCodes = [];
                                $.each(dimensionClassification.Variable, function (index, variable) {
                                    compareVariableCodes.push(variable.VrbCode)
                                });

                            }
                        });
                        //compare variable array of default language against this language
                        if (!app.library.utility.arraysEqual(defaultVariableCodes, compareVariableCodes, true)) {
                            errors.push(app.library.html.parseDynamicLabel("create-classification-variable-code", [defaultLngClassificationCode]));
                        }

                    }

                });

            });
        };

        //validate period codes
        if (!errors.length) {
            var numPeriods = [];
            $(app.build.create.initiate.data.Dimension).each(function (index, dimension) {
                numPeriods.push(dimension.Frequency.Period.length);
            });

            if (!numPeriods.every((val, i, arr) => val === arr[0])) {
                errors.push(app.label.static["build-not-equal-languages"]);
            }
        }

        //check that period codes for are the same for all languages
        if (!errors.length) {
            var defaultLngPeriodCodes = [];
            //get default language codes
            $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
                if (dimension.LngIsoCode == app.config.language.iso.code) {
                    $.each(dimension.Frequency.Period, function (index, period) {
                        defaultLngPeriodCodes.push(period.PrdCode);
                    });
                }
            });
            //compare other language period codes to default period codes
            $.each(app.build.create.initiate.data.Dimension, function (index, dimension) {
                var lngIsoCode = dimension.LngIsoCode;
                var lngIsoName = null;
                //get language name for error description
                $.each(app.build.create.initiate.languages, function (key, value) {
                    if (lngIsoCode == value.LngIsoCode) {
                        lngIsoName = value.LngIsoName;
                    }
                });
                var periodCodes = [];

                if (lngIsoCode != app.config.language.iso.code) {
                    $.each(dimension.Frequency.Period, function (index, period) {
                        periodCodes.push(period.PrdCode);
                    });
                    // If not the same, throw error
                    if (!app.library.utility.arraysEqual(defaultLngPeriodCodes, periodCodes, true)) {
                        errors.push(app.label.static["build-period-codes"]);
                    }
                }
            });
        }

    }
    if (!errors.length) {
        var dimension = null;
        $.each(app.build.create.initiate.data.Dimension, function (index, value) {
            if (value.LngIsoCode == app.config.language.iso.code) {
                dimension = value;
            }
        });
        var numClassificationVariables = 1;
        $.each(dimension.Classification, function (index, value) {
            numClassificationVariables = numClassificationVariables * this.Variable.length
        });

        var numCells = numClassificationVariables * dimension.Statistic.length * dimension.Frequency.Period.length;

        if (numCells > C_APP_CREATE_UPDATE_HARD_THRESHOLD) {
            errors.push(app.library.html.parseDynamicLabel("build-threshold-exceeded", [app.library.utility.formatNumber(numCells), app.library.utility.formatNumber(C_APP_CREATE_UPDATE_HARD_THRESHOLD)]));
        }
        if (!errors.length) {
            if (!app.build.create.dimension.mapsValid) {
                errors.push(app.label.static["build-update-map-error"]);
            }
        }

        if (!errors.length) {
            //if under soft threshold - go to 2170, else run confirm, pass name of function & params - app.build.create.initiate.data
            if (numCells < app.config.entity.build.threshold.soft) {
                app.build.create.dimension.ajax.create(app.build.create.initiate.data);
            }
            else {
                api.modal.confirm(
                    app.library.html.parseDynamicLabel("confirm-update-csv-download", [app.library.utility.formatNumber(numCells)]),
                    app.build.create.dimension.ajax.create,
                    app.build.create.initiate.data
                );

            }
        }

    }

    if (errors.length) {
        var errorOutput = $("<ul>", {
            class: "list-group"
        });
        $.each(errors, function (_index, value) {
            var error = $("<li>", {
                class: "list-group-item",
                html: value
            });
            errorOutput.append(error);
        });
        api.modal.error(errorOutput);
    }

};


/**
 *Get px file
 *
 * @param {*} params
 */
app.build.create.dimension.ajax.create = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Build.Build_API.Create",
        params,
        "app.build.create.dimension.callback.create",
        params.FrmType,
        null,
        null,
        { async: false });
};

/**
 *Download px file
 *
 * @param {*} data
 */
app.build.create.dimension.callback.create = function (data, format) {
    if (data && Array.isArray(data) && data.length) {
        var fileName = $("#build-create-initiate-setup [name=mtr-value]").val() + "." + moment(Date.now()).format(app.config.mask.datetime.file);

        switch (format) {
            case C_APP_TS_FORMAT_TYPE_JSONSTAT:
                $.each(data, function (index, file) {
                    var jsonStat = file ? JSONstat(file) : null;
                    if (jsonStat && jsonStat.length) {
                        // Download the file
                        app.library.utility.download(fileName + "." + jsonStat.extension.language.code, JSON.stringify(file), C_APP_EXTENSION_JSON, C_APP_MIMETYPE_JSON);
                    } else {
                        api.modal.exception(app.label.static["api-ajax-exception"]);
                        return false;
                    }
                });
                break;
            case C_APP_TS_FORMAT_TYPE_PX:
                $.each(data, function (index, file) {
                    // Download the file
                    app.library.utility.download(fileName, file, C_APP_EXTENSION_PX, C_APP_MIMETYPE_PX);
                });
                break;
            default:
                api.modal.exception(app.label.static["api-ajax-exception"]);
                break;
        };
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

//#endregion
//#region validation

/**
 * Validate the properties entered for the Matrix
 * @param {*} LngIsoCode
 */
app.build.create.dimension.validation.properties = function (LngIsoCode) {
    $("#build-create-dimension-accordion-collapse-properties-" + LngIsoCode).find("form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "title-value": {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            },
            "frequency-value": {
                required: true,
                notEqual: $("#build-create-dimension-accordion-collapse-properties-" + LngIsoCode).find("[name=statistic-label]"),
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            },
            "statistic-label": {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            }
        },
        invalidHandler: function (event, validator) {
            app.build.create.dimension.propertiesValid = false;
        },
        errorPlacement: function (error, element) {
            $("#build-create-dimension-accordion-collapse-properties-" + LngIsoCode).find('[name=' + element[0].name + '-error-holder]').append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.build.create.dimension.propertiesValid = true;
        }
    }).resetForm();
};
//reset classifications
app.build.create.dimension.validation.manualClassification = function () {
    $("#build-create-manual-classification").find("form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "cls-code": {
                required: true,
                validDimensionCode: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            },
            "cls-value": {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#build-create-manual-classification").find('[name=' + element[0].name + '-error-holder]').append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.build.create.dimension.callback.buildManualClassification();
        }
    }).resetForm();
};
//reset upload classifications
app.build.create.dimension.validation.uploadClassification = function () {
    $("#build-create-upload-classification").find("form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "cls-code": {
                required: true,
                validDimensionCode: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            },
            "cls-value": {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true);
                    $(this).val(value);
                    return value;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#build-create-upload-classification").find('[name=' + element[0].name + '-error-holder]').append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.build.create.dimension.callback.buildUploadClassification();
        }
    }).resetForm();
};

//#endregion
//#region elimination variables
app.build.create.dimension.drawElimination = function () {
    var defaultDimension = $.grep(app.build.create.initiate.data.Dimension, function (n, i) { // just use arr
        return n.LngIsoCode == app.config.language.iso.code;
    });
    var data = [];

    $.each(app.build.create.initiate.data.Elimination, function (key, value) {
        var classification = $.grep(defaultDimension[0].Classification, function (n, i) { // just use arr
            return n.ClsCode == key;
        });
        if (classification[0]) {
            var eliminationVariable = $.grep(classification[0].Variable, function (n, i) { // just use arr
                return n.VrbCode == value;
            });

            data.push({
                "ClsCode": classification[0].ClsCode,
                "ClsValue": classification[0].ClsValue,
                "VrbEliminationCode": eliminationVariable.length ? eliminationVariable[0].VrbCode : null,
                "VrbEliminationValue": eliminationVariable.length ? eliminationVariable[0].VrbValue : null,
            });
        }

    });

    var table = $("#build-create-matrix-dimensions").find("[name=classification-elimination-variables]");
    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            ordering: false,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ "cls-code": row.ClsCode, "cls-value": row.ClsValue, "vrb-elimination-code": row.VrbEliminationCode }, row.ClsCode);
                    }
                },
                {
                    data: "ClsValue"
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.VrbEliminationCode || ""
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.VrbEliminationValue || ""
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ "idn": row.ClsCode, "cls-value": row.ClsValue }, row.VrbEliminationCode ? false : true);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.build.create.dimension.drawCallbackElimination(table);
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.create.dimension.drawCallbackElimination(table);
        });
    }

}

app.build.create.dimension.drawCallbackElimination = function (table) {
    $('[data-toggle="tooltip"]').tooltip();
    $(table).find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        app.build.create.dimension.modal.updateElimination($(this).attr("cls-value"), $(this).attr("cls-code"), $(this).attr("vrb-elimination-code"))
    });

    $(table).find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        api.modal.confirm(
            app.library.html.parseDynamicLabel("build-delete-elimination", [$(this).attr("cls-value")]),
            app.build.create.dimension.deleteElimination,
            $(this).attr("idn")
        );
    });
}



app.build.create.dimension.validation.elimination = function () {
    $("#build-create-modal-elimination form").trigger("reset").validate({
        rules: {
            "variable":
            {
                required: true,
            }
        },
        errorPlacement: function (error, element) {
            $("#build-create-modal-elimination [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.build.create.dimension.modal.saveElimination()
        }
    }).resetForm();
};

app.build.create.dimension.modal.updateElimination = function (clsValue, clsCode, vrbEliminationCode) {
    app.build.create.dimension.validation.elimination();
    vrbEliminationCode = vrbEliminationCode || null;
    $("#build-create-modal-elimination").find("[name=cls-value]").text(app.label.static["classification"] + " : " + clsValue);
    //get classification details
    $("#build-create-modal-elimination").find("[name=cls-code]").val(clsCode);
    var defaultDimension = $.grep(app.build.create.initiate.data.Dimension, function (n, i) { // just use arr
        return n.LngIsoCode == app.config.language.iso.code;
    });

    var classification = $.grep(defaultDimension[0].Classification, function (n, i) { // just use arr
        return n.ClsCode == clsCode;
    });
    var data = [];

    $.each(classification[0].Variable, function (index, value) {
        data.push({
            "id": value.VrbCode,
            "text": value.VrbValue
        })
    });

    $("#build-create-modal-elimination").find("[name=variable]").empty().append($("<option>")).select2({
        dropdownParent: $('#build-create-modal-elimination'),
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: data
    });

    // Enable and Focus Search input
    $("#build-create-modal-elimination").find("[name=variable]").prop('disabled', false).focus();

    $("#build-create-modal-elimination").find("[name=variable]").val(vrbEliminationCode).trigger("change").trigger({
        type: 'select2:select',
        params: {
            data: $("#build-create-modal-elimination").find("[name=variable]").select2('data')[0]
        }
    });

    $("#build-create-modal-elimination").modal("show");

}

app.build.create.dimension.modal.saveElimination = function () {
    app.build.create.initiate.data.Elimination[$("#build-create-modal-elimination").find("[name=cls-code]").val()] = $("#build-create-modal-elimination").find("[name=variable]").val();
    app.build.create.dimension.drawElimination();
    $("#build-create-modal-elimination").modal("hide");
}

app.build.create.dimension.deleteElimination = function (clsCode) {
    app.build.create.initiate.data.Elimination[clsCode] = null;
    app.build.create.dimension.drawElimination();
}

//#endregion elimination variables

//#region maps
app.build.create.dimension.drawMapTable = function (isImported) {
    isImported = isImported || false;
    var defaultDimension = $.grep(app.build.create.initiate.data.Dimension, function (n, i) { // just use arr
        return n.LngIsoCode == app.config.language.iso.code;
    });
    var mapDetails = [];
    $.each(app.build.create.initiate.data.Map, function (key, value) {
        var classification = $.grep(defaultDimension[0].Classification, function (n, i) { // just use arr
            return n.ClsCode == key;
        });
        if (classification[0]) {
            mapDetails.push({
                "clsCode": classification[0].ClsCode,
                "clsValue": classification[0].ClsValue,
                "gmpUrl": value,
                "gmpName": null
            });
        };
    });

    var params = {
        "mapDetails": mapDetails,
        "isImported": isImported
    };
    app.build.create.dimension.ajax.readMapCollection(params);
};

app.build.create.dimension.ajax.readMapCollection = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.GeoMap_API.ReadCollection",
        {},
        "app.build.create.dimension.callback.readMapCollection",
        params,
        null,
        null,
        { async: false });
};

app.build.create.dimension.callback.readMapCollection = function (data, params) {
    if (data) {
        $.each(params.mapDetails, function (index, value) {
            if (value.gmpUrl) {
                var mapUrlSplit = value.gmpUrl.split("/");
                var gmpCode = mapUrlSplit[mapUrlSplit.length - 1];
                var map = $.grep(data, function (n, i) {
                    return n.GmpCode == gmpCode;
                });
                if (map.length) {
                    value.gmpName = map[0].GmpName;
                }

            }
        });
        //if imported, validate variables
        if (params.isImported) {
            $.each(params.mapDetails, function (index, value) {
                if (value.gmpName) {
                    //if imported and has a confirmed name, we need to validate the variables against the classification
                    $.ajax({
                        url: value.gmpUrl,
                        dataType: 'json',
                        async: false,
                        success: function (data) {
                            //get feature codes from map
                            var featureCodes = [];
                            $.each(data.features, function (index, value) {
                                featureCodes.push(value.properties[C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER])
                            });
                            //get classification codes
                            var defaultDimension = $.grep(app.build.create.initiate.data.Dimension, function (n, i) {
                                return n.LngIsoCode == app.config.language.iso.code;
                            });
                            var classification = $.grep(defaultDimension[0].Classification, function (n, i) {
                                return n.ClsCode == value.clsCode;
                            });

                            var invalidClassificationCodes = [];
                            $.each(classification[0].Variable, function (index, value) {
                                if (value.VrbCode != app.build.create.initiate.data.Elimination[classification[0].ClsCode]) {//don't check elimination variable
                                    if ($.inArray(value.VrbCode, featureCodes) == -1) {
                                        invalidClassificationCodes.push(value.VrbCode);
                                    }
                                }
                            });

                            if (invalidClassificationCodes.length) {
                                value.gmpName = null;
                            }
                        },
                        error: function (xhr) {
                            value.gmpName = null;
                        }
                    });
                }

            });
        };

        app.build.create.dimension.callback.drawMapTable(params.mapDetails);
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

app.build.create.dimension.callback.drawMapTable = function (data) {
    app.build.create.dimension.mapsValid = true;
    var table = $("#build-create-matrix-dimensions").find("[name=classification-map]");
    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            ordering: false,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ "cls-code": row.clsCode, "cls-value": row.clsValue }, row.clsCode);
                    }
                },
                {
                    data: "clsValue"
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        if (row.gmpName) {
                            var mapUrlSplit = row.gmpUrl.split("/");
                            var gmpCode = mapUrlSplit[mapUrlSplit.length - 1];
                            return app.library.html.link.view({ "gmp-code": gmpCode }, row.gmpName, app.label.static["view-map"]);
                        }
                        else if (row.gmpUrl) {
                            app.build.create.dimension.mapsValid = false;
                            return $("<i>", {
                                "data-toggle": "tooltip",
                                "data-original-title": app.library.html.parseDynamicLabel("invalid-geojson-in-px-file-tooltip", [row.gmpUrl]),
                                "class": "fas fa-exclamation-triangle text-danger"
                            }).get(0).outerHTML + " " + app.label.static["invalid-geojson-in-px-file"];
                        }
                        else {
                            return ""
                        }
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ "idn": row.clsCode, "gmp-name": row.gmpName }, row.gmpUrl ? false : true);
                    },
                    "width": "1%"
                },
            ],
            drawCallback: function (settings) {
                app.build.create.dimension.drawCallbackDrawMapTable();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.create.dimension.drawCallbackDrawMapTable();
        });
    }

    if (!app.build.create.dimension.mapsValid) {
        $("#build-create-elimination-map-heading-two").find("[name=map-accordion-button]").trigger("click");
        $("#build-create-matrix-dimensions").find("[name=warning-icon]").show();
    }
    else {
        $("#build-create-matrix-dimensions").find("[name=warning-icon]").hide();
    }
};

app.build.create.dimension.drawCallbackDrawMapTable = function () {
    $('[data-toggle="tooltip"]').tooltip();
    $("#build-create-matrix-dimensions").find("[name=classification-map]").find("[name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
        e.preventDefault();
        app.geomap.preview.ajax.readMap($(this).attr("gmp-code"))
    });

    $("#build-create-matrix-dimensions").find("[name=classification-map]").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        var params = {
            "clsCode": $(this).attr("cls-code"),
            "clsValue": $(this).attr("cls-value"),
            "callback": "app.build.create.dimension.ajax.validateSelectedMap"
        };
        app.build.map.ajax.readMaps(params);
    });

    $("#build-create-matrix-dimensions").find("[name=classification-map]").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        if ($(this).attr("gmp-name")) {
            api.modal.confirm(
                app.library.html.parseDynamicLabel("confirm-delete-map", [$(this).attr("gmp-name")]),
                app.build.create.dimension.deleteMap,
                $(this).attr("idn")
            );
        }
        else {
            app.build.create.dimension.deleteMap($(this).attr("idn"));
        }

    });
}

app.build.create.dimension.ajax.validateSelectedMap = function (gmpCode, clsCode) {
    $.ajax({
        url: app.config.url.api.static + "/PxStat.Data.GeoMap_API.Read/" + gmpCode,
        dataType: 'json',
        success: function (data) {
            app.build.create.dimension.callback.validateSelectedMap(data, gmpCode, clsCode);
        },
        error: function (xhr) {
            api.modal.exception(app.label.static["api-ajax-exception"]);
        }
    });
};

app.build.create.dimension.callback.validateSelectedMap = function (data, gmpCode, clsCode) {

    //get feature codes from map
    var featureCodes = [];
    $.each(data.features, function (index, value) {
        featureCodes.push(value.properties[C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER])
    });
    //get classification codes
    var defaultDimension = $.grep(app.build.create.initiate.data.Dimension, function (n, i) {
        return n.LngIsoCode == app.config.language.iso.code;
    });
    var classification = $.grep(defaultDimension[0].Classification, function (n, i) {
        return n.ClsCode == clsCode;
    });

    var invalidClassificationCodes = [];
    $.each(classification[0].Variable, function (index, value) {
        if (value.VrbCode != app.build.create.initiate.data.Elimination[clsCode]) {//don't check elimination variable
            if ($.inArray(value.VrbCode, featureCodes) == -1) {
                invalidClassificationCodes.push(value.VrbCode);
            }
        }
    });

    if (!invalidClassificationCodes.length) {
        app.build.create.initiate.data.Map[clsCode] = app.config.url.api.static + "/PxStat.Data.GeoMap_API.Read/" + gmpCode;
        app.build.create.dimension.drawMapTable();
        $("#build-map-modal").modal("hide");
    }
    else {

        var errorsList = $("<ul>", {
            "class": "list-group"
        });
        $.each(invalidClassificationCodes, function (index, value) {
            var variableValue = $.grep(classification[0].Variable, function (n, i) {
                return n.VrbCode == value;
            });

            errorsList.append(
                $("<li>", {
                    "class": "list-group-item",
                    "html": "<b>" + value + "</b>: " + variableValue[0].VrbValue
                })
            )
        });
        var eliminationMessage = "";
        if (invalidClassificationCodes.length == 1) {
            eliminationMessage = app.label.static["invalid-geojson-variables-suggestion"]
        };
        api.modal.error(app.label.static["invalid-geojson-variables"]
            + errorsList.get(0).outerHTML
            + eliminationMessage)
    }

};

app.build.create.dimension.deleteMap = function (clsCode) {
    app.build.create.initiate.data.Map[clsCode] = null;
    app.build.create.dimension.drawMapTable();
}
//#endregion maps