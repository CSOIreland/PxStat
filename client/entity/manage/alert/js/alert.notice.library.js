/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
// Add Namespace 
app.alert = app.alert || {};

app.alert.notice = {};
app.alert.notice.ajax = {};
app.alert.notice.callback = {};
//#endregion

//#region read alerts
/**
 * Read for alert.notice
 */
app.alert.notice.ajax.read = function () {
    //ReadLive is available to everybody but only reads alerts with a date in the past
    api.ajax.jsonrpc.request(
        app.config.url.api.public,
        "PxStat.System.Navigation.Alert_API.ReadLive",
        { LngIsoCode: app.label.language.iso.code },
        "app.alert.notice.callback.read");
};

/**
 * Callback for read alert.notice
 * @param {*} data
 */
app.alert.notice.callback.read = function (data) {
    $.each(data, function (i, item) {
        var listItem = $("#alert-notice-template").find("[name=alert-item]").clone();
        listItem.find("[name=alert-message]").html(app.library.html.parseBbCode(item.LrtMessage));
        listItem.find("[name=alert-date]").html(app.library.html.parseBbCode(item.LrtDatetime ? moment(item.LrtDatetime).format(app.config.mask.datetime.dateRangePicker) : ""));
        $("#alert-list-group").append(listItem);
    });
};


//#endregion



