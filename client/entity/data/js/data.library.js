/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Namespaces
app.data = {};
app.data.ajax = {};
app.data.callback = {};
app.data.collection = {};

app.data.MtrCode = null;
app.data.RlsCode = null;
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
app.data.init = function (LngIsoCode, MtrCode, RlsCode, filenamePrefix) {
    app.data.LngIsoCode = LngIsoCode || app.label.language.iso.code;
    app.data.MtrCode = MtrCode || null;
    app.data.RlsCode = RlsCode || null;
    app.data.fileNamePrefix = filenamePrefix || app.label.static["table"].toLowerCase();
    //add file name prefix
};

/**
* 
*/
app.data.setDataPicker = function () {
    var startDate = moment().subtract(29, 'days');
    app.data.collection.params.datefrom = startDate.format(app.config.mask.date.ajax);
    app.data.ajax.readLatestReleases();
    $('#latest-releases-date-picker span').html(startDate.format(app.config.mask.date.display));
    $('#latest-releases-date-picker').daterangepicker({
        showCustomRangeLabel: false,
        singleDatePicker: true,
        maxDate: new Date(),
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
        app.config.url.api.public,
        "PxStat.Data.Cube_API.ReadCollection",
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
* @param {*} response
*/
app.data.callback.readLatestReleases = function (response) {
    if (response.error) {
        // Handle the Error in the Response first
        api.modal.error(response.error.message);
    } else if (response.data !== undefined || (Array.isArray(response.data.link.item) && !response.data.link.item)) {
        var data = response.data.link.item;
        app.data.callback.drawLatestReleases(data);
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
        app.data.init($(this).attr("lng-iso-code"), $(this).attr("mtr-code"), null, $(this).attr("mtr-code"));
        app.data.dataset.ajax.readMetadata();
    });

    $('[data-toggle="tooltip"]').tooltip();
}


/**
* 
* @param {*} data
*/
app.data.callback.drawLatestReleases = function (data) {
    if ($.fn.dataTable.isDataTable("#data-latest-releases table")) {
        app.library.datatable.reDraw("#data-latest-releases table", data);
    } else {

        var localOptions = {
            order: [[5, 'desc']],
            data: data,
            "lengthMenu": [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
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
                            href: "#",
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<a>", {
                            class: "badge badge-secondary mr-1 text-light",
                            text: data.extension.frequency.name
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<a>", {
                            name: "link",
                            "mtr-code": data.extension.matrix,
                            "lng-iso-code": data.extension.language.code,
                            text: data.label,
                            href: "#",
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return $("<span>", {
                            class: "badge badge-primary p-1",
                            text: data.extension.language.name
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        if (row.extension.emergency) {
                            return $("<i>", {
                                "class": "fas fa-exclamation-triangle text-warning",
                                "data-toggle": "tooltip",
                                "data-placement": "top",
                                "title": app.label.static["emergency-release"],
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
                        return moment(row.updated, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display);

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
    $("#data-collection-api").find("[name=github-link]").attr("href", C_APP_URL_GITHUB_API_CUBE);
    $("#data-collection-api").find("[name=api-url]").text(app.config.url.api.public);
    $("#data-collection-api").find("[name=api-object]").text(function () {
        return JSON.stringify({
            "jsonrpc": C_APP_API_JSONRPC_VERSION,
            "method": "PxStat.Data.Cube_API.ReadCollection",
            "params": app.data.collection.params
        }, null, "\t");
    });
    // Refresh the Prism highlight
    Prism.highlightAll();
}

//#endregion