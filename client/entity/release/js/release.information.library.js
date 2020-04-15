/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.information = {};
app.release.information.ajax = {};
app.release.information.callback = {};
//#endregion

//#region Release

/**
* 
*/
app.release.information.read = function () {
    app.release.information.ajax.read();
};

/**
* 
*/
app.release.information.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Release_API.Read",
        { RlsCode: app.release.RlsCode },
        "app.release.information.callback.read",
        null,
        null,
        null,
        { async: false });
};

/**
* 
* @param {*} data
*/
app.release.information.callback.read = function (data) {
    //Store in namespace for late use
    app.release.SbjCode = data.SbjCode;
    app.release.PrcCode = data.PrcCode;
    app.release.RlsReservationFlag = data.RlsReservationFlag;
    app.release.RlsArchiveFlag = data.RlsArchiveFlag;

    app.release.isLive = app.release.checkStatusLive(data);
    app.release.isPending = app.release.checkStatusPending(data);
    app.release.isHistorical = app.release.checkStatusHistorical(data);


    app.release.isWorkInProgress = app.release.checkStatusWorkInProgress(data);
    app.release.isAwaitingResponse = app.release.checkStatusAwaitingResponse(data);
    app.release.isAwaitingSignOff = app.release.checkStatusAwaitingSignOff(data);

    app.release.information.render(data);
    app.release.property.render(data);
    app.release.comment.render(data);
    app.release.reason.render();
    app.release.workflow.modal.signoff.checkNavigation();
    app.release.workflow.read();
};


/**
* 
* @param {*} data
*/
app.release.information.render = function (data) {
    $("#release-information").hide().fadeIn();
    $("#release-information [name=mtr-code]").empty().html(data.MtrCode);
    $("#release-information [name=rls-version]").empty().html(data.RlsVersion);
    $("#release-information [name=rls-revision]").empty().html(data.RlsRevision);
    $("#release-information [name=status]").empty().html(app.release.renderStatus(data));
    $("#release-information [name=request]").empty().html(app.release.renderRequest(data.RqsCode));
    $("#release-information [name=rls-live-datetime-from]").empty().html(data.RlsLiveDatetimeFrom ? moment(data.RlsLiveDatetimeFrom).format(app.config.mask.datetime.display) : "");
    $("#release-information [name=rls-live-datetime-to]").empty().html(data.RlsLiveDatetimeTo ? moment(data.RlsLiveDatetimeTo).format(app.config.mask.datetime.display) : "");
    $("#release-information [name=rls-exceptional-flag]").empty().html(app.library.html.boolean(data.RlsExceptionalFlag, true, true));

    if (!data.SbjCode || !data.PrcCode) {
        var warningMessage = $("<span>", {
            "class": "text-watermark",
            "text": app.label.static["not-set"]
        }).get(0).outerHTML;
        $("#release-information [name=sbj-value]").empty().html(warningMessage);
        $("#release-information [name=prc-value]").empty().html(warningMessage);
    }
    else {
        $("#release-information [name=sbj-value]").empty().html(data.SbjValue);
        $("#release-information [name=prc-value]").empty().html(data.PrcCode + "(" + data.PrcValue + ")");
    }
    //Link to Group
    $("#release-information [name=grp-name]").empty().html(app.library.html.link.group(data.GrpCode));

    if (app.release.isModerator) {
        $("#release-information [name=rls-analytical-flag]").off("change");
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle("destroy").bootstrapToggle({
            on: app.label.static["true"],
            off: app.label.static["false"],
            onstyle: "light",
            offstyle: "neutral",
            width: C_APP_TOGGLE_LENGTH
        });
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle(data.RlsAnalyticalFlag ? "on" : "off");
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle('disable');

        // Initiate toggle buttons
        $("#release-information [name=rls-dependency-flag]").off("change");
        $("#release-information [name=rls-dependency-flag]").bootstrapToggle("destroy").bootstrapToggle({
            on: app.label.static["true"],
            off: app.label.static["false"],
            onstyle: "light",
            offstyle: "neutral",
            width: C_APP_TOGGLE_LENGTH
        });
        $("#release-information [name=rls-dependency-flag]").bootstrapToggle(data.RlsDependencyFlag ? "on" : "off");
        $("#release-information [name=rls-dependency-flag]").bootstrapToggle('disable');

        // Disable the ability to set the navigation
        $("#release-information [name=navigation]").attr("disabled", true);
    }
    else {
        $("#release-information [name=rls-analytical-flag]").off("change");
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle("destroy").bootstrapToggle({
            on: app.label.static["true"],
            off: app.label.static["false"],
            onstyle: "success",
            offstyle: "warning",
            width: C_APP_TOGGLE_LENGTH
        });
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle(data.RlsAnalyticalFlag ? "on" : "off");
        $("#release-information [name=rls-analytical-flag]").once("change", app.release.information.ajax.updateAnalyticalFlag);

        $("#release-information [name=rls-dependency-flag]").off("change");
        $("#release-information [name=rls-dependency-flag]").bootstrapToggle("destroy").bootstrapToggle({
            on: app.label.static["true"],
            off: app.label.static["false"],
            onstyle: "success",
            offstyle: "warning",
            width: C_APP_TOGGLE_LENGTH
        });
        $("#release-information [name=rls-dependency-flag]").bootstrapToggle(data.RlsDependencyFlag ? "on" : "off");
        $("#release-information [name=rls-dependency-flag]").once("change", app.release.information.ajax.updateDependencyFlag);

        // Enable the ability to set the navigation
        $("#release-information [name=update-navigation]").attr("disabled", false);
    }

    // Enable tooltip
    $('[data-toggle="tooltip"]').tooltip();
};
//#endregion

//#region Toggle Flag
/**
* 
*/
app.release.information.ajax.updateAnalyticalFlag = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Release_API.UpdateAnalyticalFlag",
        { "RlsCode": app.release.RlsCode, "RlsAnalyticalFlag": $("#release-information [name=rls-analytical-flag]").prop("checked") },
        "app.release.information.callback.updateAnalyticalFlag",
        null,
        null,
        null,
        { async: false });
};
/**
* @param {*} data
*/
app.release.information.callback.updateAnalyticalFlag = function (data) {
    if (data == C_APP_API_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

/**
* 
*/
app.release.information.ajax.updateDependencyFlag = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Release_API.UpdateDependencyFlag",
        { "RlsCode": app.release.RlsCode, "RlsDependencyFlag": $("#release-information [name=rls-dependency-flag]").prop("checked") },
        "app.release.information.callback.updateDependencyFlag",
        null,
        null,
        null,
        { async: false });
};

/**
* 
* @param {*} data
*/
app.release.information.callback.updateDependencyFlag = function (data) {
    if (data == C_APP_API_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};
//#endregion