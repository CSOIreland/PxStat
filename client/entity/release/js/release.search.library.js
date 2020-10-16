/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.search = {};
app.release.search.ajax = {};
app.release.search.callback = {};
//#endregion

//#region Matrix
/**
 * 
 */
app.release.search.readMatrixList = function () {
    app.release.search.ajax.readMatrixList();
};

/**
 * 
 */
app.release.search.ajax.readMatrixList = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.ReadCodeList",
        null,
        "app.release.search.callback.readMatrixList");
};

/**
* 
 * @param {*} data
 */
app.release.search.callback.readMatrixList = function (data) {
    // Load select2
    $("#release-search").find("[name=mtr-code]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.release.search.mapData(data)
    });

    // Enable and Focus Seach input
    $("#release-search").find("[name=mtr-code]").prop('disabled', false).focus();

    //Update Subject search Search functionality
    $("#release-search").find("[name=mtr-code]").on('select2:select', function (e) {
        var selectedObject = e.params.data;
        if (selectedObject) {
            // Some item from your model is active!
            if (selectedObject.id.toLowerCase() == $("#release-search").find("[name=mtr-code]").val().toLowerCase()) {
                // This means the exact match is found. Use toLowerCase() if you want case insensitive match.

                // Store the MtrCode for later use
                app.release.MtrCode = selectedObject.id;
                app.release.search.readReleaseList();
            }
            else {
                // Hide the list of releases
                $("#release-list-card").hide();
            }
        } else {
            // Hide the list of releases
            $("#release-list-card").hide();
        }
    });

    // goTo 
    app.release.goTo.load();
};
//#endregion

//#region Result

/**
 * 
 */
app.release.search.readReleaseList = function () {
    // Hide any other card
    $("#release-information").hide();
    $("#release-navigation").hide();
    $("#release-source").hide();
    $("#release-property").hide();
    $("#release-comment").hide();
    $("#release-reason").hide();
    $("#release-workflow").hide();
    $("#release-workflow-request").hide();
    $("#release-workflow-response").hide();
    $("#release-workflow-signoff").hide();
    $("#release-workflow-history").hide();

    app.release.search.ajax.readReleaseList();
};

/**
 * 
 */
app.release.search.ajax.readReleaseList = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Release_API.ReadList",
        {
            "MtrCode": app.release.MtrCode,
            LngIsoCode: app.label.language.iso.code
        },
        "app.release.search.callback.readReleaseList");
};

/**
 * Draw Callback for Datatable
 */
app.release.search.drawCallbackReleaseList = function () {
    // Bind Edit click
    $("#release-search-result table").on("click", "[name=" + C_APP_NAME_LINK_EDIT + "]", app.release.search.loadRelease);
}

/**
* 
 * @param {*} data
 */
app.release.search.callback.readReleaseList = function (data) {
    // Show the card
    $("#release-search-result").hide().fadeIn();

    // Set Matrix code in the Header
    $("#release-search-result .card-header").html(app.release.MtrCode);

    // Draw datatable
    if ($.fn.dataTable.isDataTable("#release-search-result table")) {
        app.library.datatable.reDraw("#release-search-result table", data);
    } else {

        var localOptions = {
            order: [[0, 'desc']],
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ idn: row.RlsCode }, row.RlsVersion + "." + row.RlsRevision);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.release.renderStatus(row);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.release.renderRequest(row.RqsCode);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return !row.RlsLiveDatetimeFrom ? row.RlsLiveDatetimeFrom : moment(row.RlsLiveDatetimeFrom).format(app.config.mask.datetime.display);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return !row.RlsLiveDatetimeTo ? row.RlsLiveDatetimeTo : moment(row.RlsLiveDatetimeTo).format(app.config.mask.datetime.display);
                    }
                }
            ],
            drawCallback: function (settings) {
                app.release.search.drawCallbackReleaseList();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#release-search-result table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.search.drawCallbackReleaseList();
        });
    }
};

/**
* 
 * @param {*} e
 */
app.release.search.loadRelease = function (e) {
    e.preventDefault();
    // Store for later use
    app.release.RlsCode = $(this).attr("idn");

    //scroll to the bottom of the search results table to avoid bouncing
    $('html, body').animate({
        scrollTop: $("#release-search-result").prop("scrollHeight")
    }, 1000);

    app.release.load();
};
//#endregion

//#region Select2
/**
 * Map API data to select dropdown data model
 * @param {*} data 
 */
app.release.search.mapData = function (data) {
    $.each(data, function (i, item) {
        // Add ID and TEXT to the list
        data[i].id = item.MtrCode;
        data[i].text = item.MtrCode;
    });
    return data;
};
//#endregion