/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.source = {};
app.release.data = {};
app.release.source.ajax = {};
app.release.source.callback = {};
//#endregion

//#region Source

/**
 * 
 */
app.release.source.read = function () {
    app.release.source.ajax.readLanguage();
};
app.release.source.ajax.read = function (LngIsoCode) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.Read",
        { RlsCode: app.release.RlsCode, LngIsoCode: LngIsoCode },
        "app.release.source.callback.read",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} data
 */
app.release.source.callback.read = function (data) {
    app.release.source.render(data);

    app.release.fileContent = data.MtrInput;
    app.release.fileType = data.FrmType;
    app.release.fileName = data.MtrCode + "_" + data.RlsVersion + "_" + data.RlsRevision + "_" + (data.DtgCreateDatetime ? moment(data.DtgCreateDatetime).format(app.config.mask.datetime.file) : "");
};

/**
* 
 * @param {*} data
 */
app.release.source.render = function (data) {
    $("#release-source").hide().fadeIn();

    $("#release-source [name=mtr-title]").empty().html(data.MtrTitle);
    $("#release-source [name=dtg-create-datetime]").empty().html(data.DtgCreateDatetime ? moment(data.DtgCreateDatetime).format(app.config.mask.datetime.display) : "");
    $("#release-source [name=ccn-username]").empty().html(app.library.html.link.user(data.CcnUsernameCreate));

    $("#release-source [name=frq-value]").empty().html(data.FrqValue);
    $("#release-source [name=cpr-value]").empty().html(data.CprValue);

    $("#release-source [name=mtr-official-flag]").empty().html(app.library.html.boolean(data.MtrOfficialFlag, true, true));

    $("#release-source [name=mtr-note]").empty().html(app.library.html.parseBbCode(data.MtrNote));
    $("#release-source [name=dtg-create-datetime]").empty().html(data.DtgCreateDatetime ? moment(data.DtgCreateDatetime).format(app.config.mask.datetime.display) : "");

};
//#endregion

//#region Language

/**
 * 
 */
app.release.source.ajax.readLanguage = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Settings.Language_API.ReadListByRelease",
        { "RlsCode": app.release.RlsCode },
        "app.release.source.callback.readLanguageOnSuccess",
        null,
        "app.release.source.callback.readLanguageOnError",
        null,
        { async: false });
};

/**
* 
 * @param {*} data
 */
app.release.source.callback.readLanguageOnSuccess = function (data) {
    $("#release-source [name=lng-iso-code]").empty();
    $.each(data, function (index, value) {
        $("#release-source [name=lng-iso-code]").append($('<option>', { value: value.LngIsoCode, text: value.LngIsoName }));
    });
    app.release.source.languageOnChange();
};

/**
* 
 * @param {*} error
 */
app.release.source.callback.readLanguageOnError = function (error) {
    $("#release-source [name=lng-iso-code]").empty();
};

/**
 * 
 */
app.release.source.languageOnChange = function () {
    var LngIsoCode = $("#release-source [name=lng-iso-code] option:selected").val();
    app.release.source.ajax.read(LngIsoCode);
};
//#endregion

//#region Download

/**
 * 
 */
app.release.source.download = function () {
    switch (app.release.fileType) {
        case C_APP_TS_FORMAT_TYPE_PX:
            // Download the file
            app.library.utility.download(app.release.fileName, app.release.fileContent, C_APP_EXTENSION_PX, C_APP_MIMETYPE_PX);
            break;
        case C_APP_TS_FORMAT_TYPE_JSONSTAT:
            // Download the file
            app.library.utility.download(app.release.fileName, JSON.stringify(app.release.fileContent), C_APP_EXTENSION_JSON, C_APP_MIMETYPE_JSON);
            break;
        default:
            api.modal.exception(app.label.static["api-ajax-exception"]);
            break;
    }
};
//#endregion

//#region view source

/**
 * 
 */
app.release.source.view = function () {
    if (app.release.fileContent.length > app.config.transfer.threshold.soft) {
        api.modal.confirm(app.library.html.parseDynamicLabel("confirm-preview", [app.library.utility.formatNumber(Math.ceil(app.release.fileContent.length / 1024)) + " KB"]),
            app.release.source.callback.view)
    }
    else {
        // Preview file content   
        app.release.source.callback.view();
    }
};

app.release.source.callback.view = function () {
    api.spinner.start();
    $("#release-source-preview").modal("show");
    api.content.load("#release-source-preview .modal-body", "entity/release/index.source.preview.html");
    api.spinner.stop();
};
//#endregion

//#region view data

/**
 * 
 */
app.release.data.view = function () {
    //DO NOT Pass User SELECTED language from app.label.language.iso.code
    //Drop down list available langues for the Release (Matrix langues)
    app.data.init(
        $("#release-source [name=lng-iso-code] option:selected").val(),
        app.release.MtrCode,
        app.release.RlsCode,
        app.release.MtrCode,
        true,
        app.release.isLive,
    );

    app.data.dataset.draw();

    //reset modal for clean opening if user changes release
    $('#data-view-modal').on('hide.bs.modal', function (e) {
        $("#data-dataset-table-nav-content").empty()
        $("#data-dataset-chart-nav-content").empty()
        $("#data-dataset-map-nav-content").empty()
        $('#data-dataset-map-nav-tab').hide();
        $('#data-dataset-table-nav-tab').show()
    })
};

//#endregion
