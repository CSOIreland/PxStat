/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.build = app.build || {};
app.build.create = app.build.create || {};
app.build.create.search = app.build.create.search || {};

app.build.create.search.ajax = {};
app.build.create.search.callback = {};
app.build.create.search.selectedLanguages = [];
app.build.create.rlsCode = null;


/**
 * 
 */
app.build.create.search.ajax.readMatrixList = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.ReadCodeList",
        null,
        "app.build.create.search.callback.readMatrixList");
};

app.build.create.search.callback.readMatrixList = function (data) {
    // Load select2
    $("#build-create-existing-table-tab-content").find("[name=mtr-code]").empty().append($("<option>")).select2({
        dropdownParent: $('#build-create-import'),
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.build.create.search.mapData(data)
    }).on('select2:select', function (e) {
        if ($.fn.DataTable.isDataTable("#build-create-existing-table-search-result table")) {
            $("#build-create-existing-table-search-result table").DataTable().destroy();
            //cannot use redraw as columns are dynamically created depending on the matrix. Have to destroy and re-initiate 
        }
        app.build.create.search.ajax.readReleaseList();
    }).on('select2:clear', function (e) {
        // Hide the card
        $("#build-create-existing-table-search-result").hide();
    });;

    // Enable and Focus Seach input
    $("#build-create-existing-table-tab-content").find("[name=mtr-code]").prop('disabled', false).focus();


};

/**
 * 
 */
app.build.create.search.ajax.readReleaseList = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Release_API.ReadList",
        {
            "MtrCode": $("#build-create-existing-table-tab-content").find("[name=mtr-code]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.build.create.search.callback.readReleaseList");
};

/**
* 
 * @param {*} data
 */
app.build.create.search.callback.readReleaseList = function (data) {
    // Show the card
    $("#build-create-existing-table-search-result").hide().fadeIn();

    // Set Matrix code in the Header
    $("#build-create-existing-table-search-result .card-header").html($("#build-create-existing-table-tab-content").find("[name=mtr-code]").val());

    // Draw datatable
    if ($.fn.dataTable.isDataTable("#build-create-existing-table-search-result table")) {
        app.library.datatable.reDraw("#build-create-existing-table-search-result table", data);
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
                app.build.create.search.drawCallbackReleaseList();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#build-create-existing-table-search-result table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.create.search.drawCallbackReleaseList();
        });
    }
};

app.build.create.search.drawCallbackReleaseList = function () {
    // Bind Edit click
    $("#build-create-existing-table-search-result table").on("click", "[name=" + C_APP_NAME_LINK_EDIT + "]", function (e) {
        e.preventDefault();
        app.build.create.search.readLanguage($(this).attr("idn"));
    });
}

/**
 * 
 */
app.build.create.search.readLanguage = function (rlsCode) {
    app.build.create.rlsCode = rlsCode;
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Language_API.ReadListByRelease",
        {
            "RlsCode": app.build.create.rlsCode,
            "LngIsoCode": app.label.language.iso.code
        },
        "app.build.create.search.readLanguageOnSuccess",
        null,
        null,
        null,
        { async: false });
};

app.build.create.search.readLanguageOnSuccess = function (data) {
    app.build.create.file.import.content.JsonStat = [];
    app.build.create.search.selectedLanguages = [];
    if (data.length > 1) {
        $("#build-create-modal-select-language .modal-body").empty();
        $(data).each(function (key, value) {
            var languageCheckbox = $("#build-create-existing-table-templates").find("[name=existing-table-language-checkbox-wrapper]").clone();
            languageCheckbox.find("input").attr("id", "build-create-existing-table-language-" + value.LngIsoCode).val(value.LngIsoCode);
            languageCheckbox.find("label").attr("for", "build-create-existing-table-language-" + value.LngIsoCode).text(value.LngIsoName);
            if (value.LngIsoCode == app.config.language.iso.code) {
                languageCheckbox.find("input").prop('disabled', true);
            }
            $("#build-create-modal-select-language .modal-body").append(languageCheckbox);
        });

        // Show the modal
        $("#build-create-modal-select-language").modal("show");
    }
    else {
        app.build.create.search.selectedLanguages = [data[0].LngIsoCode];
        app.build.create.search.ajax.loadRelease();
    }
};

app.build.create.search.ajax.loadRelease = function () {
    $(app.build.create.search.selectedLanguages).each(function (key, value) {
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.private,
            "PxStat.Data.Cube_API.ReadPreMetadata",
            {
                "release": app.build.create.rlsCode,
                "build": true,
                "format": {
                    "type": C_APP_FORMAT_TYPE_DEFAULT,
                    "version": C_APP_FORMAT_VERSION_DEFAULT
                },
                "language": value,
                "m2m": false
            },
            "app.build.create.search.callback.loadRelease",
            null,
            null,
            null,
            {
                async: false,
                timeout: app.config.transfer.timeout
            });
    });
};

app.build.create.search.callback.loadRelease = function (data) {
    var jsonStat = data ? JSONstat(data) : null;
    app.build.create.file.import.content.JsonStat.push(jsonStat);
    //once jsonStat array is the same length as languages array we know we are ready to proceed
    if (app.build.create.search.selectedLanguages.length == app.build.create.file.import.content.JsonStat.length) {
        app.build.create.import.callback.read.drawProperties();
        $("#build-create-import").modal("hide");
    }

};

app.build.create.search.mapData = function (data) {
    $.each(data, function (i, item) {
        // Add ID and TEXT to the list
        data[i].id = item.MtrCode;
        data[i].text = item.MtrCode;
    });
    return data;
};
