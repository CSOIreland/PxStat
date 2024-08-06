/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.build = app.build || {};
app.build.update = app.build.update || {};
app.build.update.search = app.build.update.search || {};

app.build.update.search.ajax = {};
app.build.update.search.callback = {};
app.build.update.search.selectedLanguages = [];
app.build.update.search.defaultLngJsonStat = [];

/**
 * 
 */
app.build.update.search.ajax.readMatrixList = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.ReadCodeList",
        null,
        "app.build.update.search.callback.readMatrixList");
};

app.build.update.search.callback.readMatrixList = function (data) {
    // Load select2
    $("#build-update-existing-table-tab-content").find("[name=mtr-code]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.build.update.search.mapData(data)
    }).on('select2:select', function (e) {

        if (app.build.update.ajax.jsonStat.length) {
            api.modal.confirm(app.label.static["select-new-existing-table"], app.build.update.search.ajax.readReleaseList)
        }
        else {
            app.build.update.search.ajax.readReleaseList();
        }

    }).on('select2:clear', function (e) {
        // Hide the card
        $("#build-update-existing-table-search-result").hide();
    });

    // Enable and Focus Seach input
    $("#build-update-existing-table-tab-content").find("[name=mtr-code]").prop('disabled', false).focus();


};

/**
 * 
 */
app.build.update.search.ajax.readReleaseList = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Release_API.ReadList",
        {
            "MtrCode": $("#build-update-existing-table-tab-content").find("[name=mtr-code]").val(),
            "LngIsoCode": app.label.language.iso.code
        },
        "app.build.update.search.callback.readReleaseList");
};

/**
* 
 * @param {*} data
 */
app.build.update.search.callback.readReleaseList = function (data) {
    if ($.fn.DataTable.isDataTable("#build-update-existing-table-search-result table")) {
        $("#build-update-existing-table-search-result table").DataTable().destroy();
        //cannot use redraw as columns are dynamically created depending on the matrix. Have to destroy and re-initiate 
    }
    // Show the card
    $("#build-update-existing-table-search-result").hide().fadeIn();
    //hide the dimension details card 
    $("#build-update-dimensions").hide();

    // Set Matrix code in the Header
    $("#build-update-existing-table-search-result .card-header").html($("#build-update-existing-table-tab-content").find("[name=mtr-code]").val());

    // Draw datatable
    if ($.fn.dataTable.isDataTable("#build-update-existing-table-search-result table")) {
        app.library.datatable.reDraw("#build-update-existing-table-search-result table", data);
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
                app.build.update.search.drawCallbackReleaseList();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };

        $("#build-update-existing-table-search-result table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.build.update.search.drawCallbackReleaseList();
        });
    }
};

app.build.update.search.drawCallbackReleaseList = function () {
    // Bind Edit click
    $("#build-update-existing-table-search-result table").on("click", "[name=" + C_APP_NAME_LINK_EDIT + "]", function (e) {
        $("#build-update-upload-file-tab-content").find("[name=file-name]").empty().hide();
        $("#build-update-upload-file-tab-content").find("[name=file-tip]").show();
        $("#build-update-upload-text-tab-content").find("[name=text-content]").val("");
        $("#build-update-upload-file-tab-content").find("[name=file-data-view]").prop("disabled", true);
        $("#build-update-upload-file-input").val("");
        e.preventDefault();
        //we are already working with an update, warn user
        if (app.build.update.ajax.jsonStat.length) {
            api.modal.confirm(app.label.static["select-new-existing-table"], app.build.update.search.readLanguage, $(this).attr("idn"))
        }
        else {
            app.build.update.search.readLanguage($(this).attr("idn"));
        }

    });
}

/**
 * 
 */
app.build.update.search.readLanguage = function (rlsCode) {
    app.build.update.rlsCode = rlsCode;
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Language_API.ReadListByRelease",
        {
            "RlsCode": app.build.update.rlsCode,
            "LngIsoCode": app.label.language.iso.code
        },
        "app.build.update.search.readLanguageOnSuccess",
        null,
        null,
        null,
        { async: false });
};

app.build.update.search.readLanguageOnSuccess = function (data) {
    app.build.update.ajax.jsonStat = [];
    app.build.update.search.selectedLanguages = [];
    if (data.length > 1) {
        $("#build-update-modal-select-language .modal-body").empty();
        $(data).each(function (key, value) {
            var languageCheckbox = $("#build-update-existing-table-templates").find("[name=existing-table-language-checkbox-wrapper]").clone();
            languageCheckbox.find("input").attr("id", "build-update-existing-table-language-" + value.LngIsoCode).val(value.LngIsoCode);
            languageCheckbox.find("label").attr("for", "build-update-existing-table-language-" + value.LngIsoCode).text(value.LngIsoName);
            if (value.LngIsoCode == app.config.language.iso.code) {
                languageCheckbox.find("input").prop('disabled', true);
            }
            $("#build-update-modal-select-language .modal-body").append(languageCheckbox);
        });

        // Show the modal
        $("#build-update-modal-select-language").modal("show");
    }
    else {
        app.build.update.search.selectedLanguages = [data[0].LngIsoCode];
        app.build.update.search.ajax.loadRelease();
    }
};

app.build.update.search.ajax.loadRelease = function () {
    $(app.build.update.search.selectedLanguages).each(function (key, value) {
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.private,
            "PxStat.Data.Cube_API.ReadPreMetadata",
            {
                "release": app.build.update.rlsCode,
                "build": true,
                "format": {
                    "type": C_APP_FORMAT_TYPE_DEFAULT,
                    "version": C_APP_FORMAT_VERSION_DEFAULT
                },
                "language": value,
                "m2m": false
            },
            "app.build.update.search.callback.loadRelease",
            null,
            null,
            null,
            {
                async: false,
                timeout: app.config.transfer.timeout
            });
    });
};

app.build.update.search.callback.loadRelease = function (data) {
    var jsonStat = data ? JSONstat(data) : null;
    if (jsonStat.extension.language.code == app.config.language.iso.code) {
        //put default language on it's own first
        app.build.update.search.defaultLngJsonStat.push(jsonStat)
    }
    else {
        app.build.update.ajax.jsonStat.push(jsonStat);
    }
    //once jsonStat arrays are the same length as languages array we know we are ready to proceed
    if (app.build.update.search.selectedLanguages.length == app.build.update.ajax.jsonStat.length + app.build.update.search.defaultLngJsonStat.length) {
        //insert default language at start of array
        app.build.update.ajax.jsonStat.splice(0, 0, app.build.update.search.defaultLngJsonStat[0]);
        app.build.update.search.defaultLngJsonStat = [];
        app.build.update.upload.drawProperties();
    }

};

app.build.update.search.mapData = function (data) {
    $.each(data, function (i, item) {
        // Add ID and TEXT to the list
        data[i].id = item.MtrCode;
        data[i].text = item.MtrCode;
    });
    return data;
};

app.build.update.search.ajax.downloadCsvTemplate = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Build.Build_API.ReadTemplateByRelease",
        {
            "RlsCode": app.build.update.rlsCode,
            "FrqCodeTimeval": app.build.update.upload.FrqCode,
            "FrqValueTimeval": app.build.update.upload.FrqValue
        },
        "app.build.update.search.callback.downloadCsvTemplate",
        null,
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        });
};

app.build.update.search.callback.downloadCsvTemplate = function (data) {
    var fileName = data.MtrCode + '.' + moment(Date.now()).format(app.config.mask.datetime.file) + "." + app.label.static["template"].toLowerCase();
    // Download the file
    app.library.utility.download(fileName, data.template, C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
};

app.build.update.search.ajax.downloadCsvData = function () {
    var params = {
        "RlsCode": app.build.update.rlsCode,
        "FrqValueTimeval": app.build.update.upload.FrqValue,
        "FrqCodeTimeval": app.build.update.upload.FrqCode,
        "Signature": app.build.update.upload.Signature,
        "Labels": $("#build-update-download-csv-file [name=labels]").is(":checked"),
        "Dimension": []
    };
    var dimension =
    {
        "LngIsoCode": app.label.language.iso.code,

        "Frequency": {
            "FrqValue": $("#build-update-dimension-nav-collapse-properties-" + app.label.language.iso.code + " [name=frequency-value]").val(),
            "Period": []
        }
    }

    $("#build-update-download-csv-file select").find('option:selected').each(function () {
        dimension.Frequency.Period.push(
            {
                "PrdCode": $(this).val(),
                "PrdValue": $(this).text()
            }
        )

    });
    params.Dimension.push(dimension);


    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Build.Build_API.ReadDatasetByRelease",
        params,
        "app.build.update.search.callback.downloadCsvData",
        null,
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        });
};

app.build.update.search.callback.downloadCsvData = function (data) {
    var fileName = data.MtrCode + '.' + moment(Date.now()).format(app.config.mask.datetime.file) + "." + app.label.static["data"].toLowerCase();
    // Download the file
    app.library.utility.download(fileName, data.csv, C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
    $('#build-update-download-csv-file').modal("hide");
};
