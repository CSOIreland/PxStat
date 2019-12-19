/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.source = {};
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
        app.config.url.api.private,
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
 * @param {*} response
 */
app.release.source.callback.read = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        var data = response.data;

        app.release.source.render(data);

        app.release.fileContent = data.MtrInput;
        app.release.fileType = data.FrmType;
        app.release.fileName = data.MtrCode + "_v" + data.RlsVersion + "." + data.RlsRevision + "_" + moment(data.DtgCreateDatetime).format(app.config.mask.datetime.file) + "." + data.FrmType.toLowerCase();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* 
 * @param {*} data
 */
app.release.source.render = function (data) {
    $("#release-source").hide().fadeIn();

    $("#release-source [name=mtr-title]").empty().html(data.MtrTitle);
    $("#release-source [name=dtg-create-datetime]").empty().html(moment(data.DtgCreateDatetime).format(app.config.mask.datetime.display));
    $("#release-source [name=ccn-username]").empty().html(app.library.html.link.user(data.CcnUsernameCreate));

    $("#release-source [name=frq-value]").empty().html(data.FrqValue);
    $("#release-source [name=cpr-value]").empty().html(data.CprValue);

    $("#release-source [name=mtr-official-flag]").empty().html(app.library.html.boolean(data.MtrOfficialFlag, true, true));

    $("#release-source [name=mtr-note]").empty().html(app.library.html.parseBbCode(data.MtrNote));
    $("#release-source [name=dtg-create-datetime]").empty().html(moment(data.DtgCreateDatetime).format(app.config.mask.datetime.display));

    // Display Data in modal (reuse code app.data.dataset)
    $("#release-source").find("[name=view-data]").once("click", function (e) {
        e.preventDefault();
        //DO NOT Pass User SELECTED language from app.label.language.iso.code
        //Drop down list available langues for the Release (Matrix langues)
        app.data.init($("#release-source [name=lng-iso-code] option:selected").val(), null, app.release.RlsCode, app.release.MtrCode);
        app.data.dataset.ajax.readMetadata();
        $('#data-view-modal').modal('show');
    });
};
//#endregion

//#region Language

/**
 * 
 */
app.release.source.ajax.readLanguage = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.System.Settings.Language_API.ReadListByRelease",
        { "RlsCode": app.release.RlsCode },
        "app.release.source.callback.readLanguage",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} response
 */
app.release.source.callback.readLanguage = function (response) {
    $("#release-source [name=lng-iso-code]").empty();

    if (response.error) {
        api.modal.error(response.error.message);
    }
    else if (response.data !== undefined) {
        $.each(response.data, function (index, value) {
            $("#release-source [name=lng-iso-code]").append($('<option>', { value: value.LngIsoCode, text: value.LngIsoName }));
        });
        app.release.source.languageOnChange();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
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
    switch (app.release.fileType.toLowerCase()) {
        case 'json':
            var mimeType = "application/json";
            var fileData = JSON.stringify(app.release.fileContent);
            break;
        default:
            var mimeType = "text/plain";
            var fileData = app.release.fileContent;
            break;
    }

    var blob = new Blob([fileData], { type: mimeType });
    var downloadUrl = URL.createObjectURL(blob);
    var a = document.createElement("a");
    a.href = downloadUrl;
    a.download = app.release.fileName;

    if (document.createEvent) {
        // https://developer.mozilla.org/en-US/docs/Web/API/Document/createEvent
        var event = document.createEvent('MouseEvents');
        event.initEvent('click', true, true);
        a.dispatchEvent(event);
    }
    else {
        a.click();
    }
};
//#endregion

//#region view source

/**
 * 
 */
app.release.source.view = function () {
    if (app.release.fileContent.length > app.config.upload.threshold.soft) {
        api.modal.confirm(app.library.html.parseDynamicLabel("confirm-file", [app.library.utility.formatNumber(Math.ceil(app.release.fileContent.length / 1024)) + " KB"]),
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
