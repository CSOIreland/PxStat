/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
app.data = {};
app.data.ajax = {};
app.data.callback = {};
app.data.collection = {};

app.data.MtrCode = null;
app.data.PrdCode = null;
app.data.RlsCode = null;
app.data.isModal = null;
app.data.isLive = null;
app.data.isSearch = null;
app.data.isCopyrightGoTo = null;
app.data.fileNamePrefix = null;
app.data.LngIsoCode = app.label.language.iso.code;
app.data.collection.params = {
    language: app.data.LngIsoCode,
    datefrom: null
};

// GoTo
app.data.goTo = {};
app.data.goTo.Search = null;
app.data.goTo.PrcCode = null;
app.data.goTo.CprCode = null;
app.data.goTo.MtrCode = null;
//#endregion


//#region

/**
* 
* @param {*} LngIsoCode
* @param {*} MtrCode
* @param {*} RlsCode
* @param {*} filenamePrefix
*/
app.data.init = function (LngIsoCode, MtrCode, RlsCode, filenamePrefix, isModal, isLive) {
    app.data.LngIsoCode = LngIsoCode || app.label.language.iso.code;
    app.data.MtrCode = MtrCode || null;
    app.data.RlsCode = RlsCode || null;
    app.data.fileNamePrefix = filenamePrefix || app.label.static["table"].toLowerCase();
    app.data.isModal = isModal;
    app.data.isLive = isLive;
};

/**
* 
*/
app.data.setDatePicker = function () {
    var startDate = moment().subtract(app.config.entity.data.lastUpdatedTables.defaultNumDaysFrom, 'days');
    app.data.collection.params.datefrom = startDate.format(app.config.mask.date.ajax);
    app.data.ajax.readLatestReleases();
    $('#latest-releases-date-picker span').html(startDate.format(app.config.mask.date.display));

    $('#latest-releases-date-picker').daterangepicker({
        showCustomRangeLabel: false,
        singleDatePicker: true,
        maxDate: new Date(),
        minDate: moment().subtract(29, 'days'),
        ranges: {
            [app.label.static["last-30-days"]]: [moment().subtract(29, 'days'), moment()],
            [app.label.static["last-7-days"]]: [moment().subtract(6, 'days'), moment()],
        },
        locale: app.label.plugin.daterangepicker
    }, function (start) {
        app.data.collection.params.datefrom = start.format(app.config.mask.date.ajax);
        $('#latest-releases-date-picker span').html(start.format(app.config.mask.date.display));
        app.data.ajax.readLatestReleases();

    });

};

/**
* 
*/
app.data.ajax.readLatestReleases = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Data.Cube_API.ReadMetaCollection",
        app.data.collection.params,
        "app.data.callback.readLatestReleases",
        null,
        null,
        null,
        { async: false }
    );
};

/**
* 
* @param {*} data
*/
app.data.callback.readLatestReleases = function (data) {
    if (data && data.link && Array.isArray(data.link.item)) {
        app.data.callback.drawLatestReleases(data.link.item);
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.data.callback.drawCallbackDrawLatestReleases = function () {
    $("#data-latest-releases table [name=link]").once('click', function (e) {
        e.preventDefault();
        $("#data-latest-releases").remove();
        $("#data-accordion-collection-api").hide();
        //collapse navigation so filter abd sort visible at top of screen
        $("#data-navigation").find(".navbar-collapse").collapse("hide");
        app.data.init($(this).attr("lng-iso-code"), $(this).attr("mtr-code"), null, $(this).attr("mtr-code"), false, true);
        app.data.dataset.draw();
    });

    $('[data-toggle="tooltip"]').tooltip();

    //populate colection api  
    app.data.callback.drawCollectionApiDetails();
    $("#data-accordion-collection-api").show();
}


/**
* 
* @param {*} data
*/
app.data.callback.drawLatestReleases = function (data) {
    if ($.fn.dataTable.isDataTable("#data-latest-releases table")) {
        app.library.datatable.reDraw("#data-latest-releases table", data);
        // Trigger the responsivness when changing the lenght of the table cells because of the code
        // https://datatables.net/forums/discussion/44766/responsive-doesnt-immediately-resize-the-table
        $(window).trigger('resize');
    } else {
        var localOptions = {
            order: [[4, 'desc'], [0, 'asc']],
            data: data,
            "pageLength": app.config.entity.data.lastUpdatedTables.defaultPageLength,
            createdRow: function (row, data, dataIndex) {
                $(row).attr("mtr-code", data.extension.matrix).attr("lng-iso-code", data.extension.language.code);
            },
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<a>", {
                            name: "link",
                            "mtr-code": data.extension.matrix,
                            "lng-iso-code": data.extension.language.code,
                            text: data.extension.matrix,
                            href: app.config.url.application + C_COOKIE_LINK_TABLE + "/" + data.extension.matrix,
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        var titleRow = $("#data-latest-releases-templates").find("[name=title-column]").clone();
                        // Workaround for a (weird) Chrome bug when downloading and printing when not in the data entity
                        if (!titleRow.length)
                            return "";

                        //get title
                        titleRow.find("[name=title]").html($("<a>", {
                            name: "link",
                            "mtr-code": data.extension.matrix,
                            "lng-iso-code": data.extension.language.code,
                            text: data.label,
                            href: app.config.url.application + C_COOKIE_LINK_TABLE + "/" + data.extension.matrix,
                        }).get(0).outerHTML);

                        //get time dimension
                        titleRow.find("[name=time]").text(data.dimension[data.role.time[0]].label);

                        //get classifications and time span
                        $.each(data.dimension, function (index, dimension) {
                            //classifications
                            if (index != data.role.time[0] && index != data.role.metric[0]) {
                                var classificationPill = $("#data-latest-releases-templates").find("[name=classification]").clone();
                                // Workaround for a (weird) Chrome bug when downloading and printing when not in the data entity
                                if (!classificationPill.length)
                                    return "";

                                classificationPill.text(dimension.label);
                                titleRow.find("[name=classifications]").append(classificationPill.get(0).outerHTML);
                            }

                            //time span
                            if (index == data.role.time[0]) {

                                titleRow.find("[name=time-span]").text(
                                    function () {
                                        if (dimension.category.index.length > 1) {
                                            var firstTimeVal = dimension.category.index[0];
                                            var lastTimeVal = dimension.category.index[dimension.category.index.length - 1];
                                            return dimension.category.label[firstTimeVal] + " - " + dimension.category.label[lastTimeVal];
                                        }
                                        else {
                                            return dimension.category.label[dimension.category.index[0]];
                                        }

                                    }
                                );
                            }
                        });

                        return titleRow.get(0).outerHTML
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            class: "badge badge-primary",
                            text: data.extension.language.name
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        if (row.extension.exceptional) {
                            return $("<i>", {
                                "class": "fas fa-exclamation-triangle text-warning",
                                "data-toggle": "tooltip",
                                "data-placement": "top",
                                "title": app.label.static["exceptional-release"],
                            }).get(0).outerHTML;
                        }
                        else {
                            return null;
                        }
                    },
                    "width": "1%"
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.updated ? moment(row.updated, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";
                    }
                }
            ],
            drawCallback: function (settings) {
                app.data.callback.drawCallbackDrawLatestReleases();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#data-latest-releases").show().find("table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.data.callback.drawCallbackDrawLatestReleases();
        });

    }
};

/**
 * Draw collection API details
 */
app.data.callback.drawCollectionApiDetails = function () {

    $("#data-dataset-collection-api-jspnrpc-content [name=information-documentation]").html(
        app.library.html.parseDynamicLabel("information-api-documentation", ["JSON-RPC", $("<a>", {
            "href": C_APP_URL_GITHUB_API_CUBE_JSONRPC,
            "text": "GitHub Wiki",
            "target": "_blank"
        }).get(0).outerHTML]));

    $("#data-dataset-collection-api-restful-content [name=information-documentation]").html(
        app.library.html.parseDynamicLabel("information-api-documentation", ["RESTful", $("<a>", {
            "href": C_APP_URL_GITHUB_API_CUBE_RESFFUL,
            "text": "GitHub Wiki",
            "target": "_blank"
        }).get(0).outerHTML]));

    var query = JSON.stringify({
        "jsonrpc": C_APP_API_JSONRPC_VERSION,
        "method": "PxStat.Data.Cube_API.ReadCollection",
        "params": app.data.collection.params
    }, null, "\t");

    $("#data-dataset-collection-api-jsonrpc-post-url").text(app.config.url.api.jsonrpc.public);
    $("#data-dataset-collection-api-obj").hide().text(query).fadeIn();

    $("#data-dataset-collection-api-jsonrpc-get-url").empty().text(encodeURI(app.config.url.api.jsonrpc.public + C_APP_API_GET_PARAMATER_IDENTIFIER + query)).fadeIn();

    $("#data-dataset-collection-api-restful-url").hide().text(C_APP_API_RESTFUL_READ_COLLECTION_URL.sprintf([app.config.url.api.restful + "/", app.data.collection.params.datefrom, app.data.LngIsoCode])).fadeIn();

    // Refresh the Prism highlight
    Prism.highlightAll();
}

//#endregion