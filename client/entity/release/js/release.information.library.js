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
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Release_API.Read",
        {
            "RlsCode": app.release.RlsCode,
            "LngIsoCode": app.label.language.iso.code
        },
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
    app.release.RlsExperimentalFlag = data.RlsExperimentalFlag;
    app.release.RlsArchiveFlag = data.RlsArchiveFlag;

    app.release.isLive = app.release.checkStatusLive(data);
    app.release.isPending = app.release.checkStatusPending(data);
    app.release.isHistorical = app.release.checkStatusHistorical(data);
    app.release.isCancelled = app.release.checkStatusCancelled(data);


    app.release.isWorkInProgress = app.release.checkStatusWorkInProgress(data);
    app.release.isAwaitingResponse = app.release.checkStatusAwaitingResponse(data);
    app.release.isAwaitingSignOff = app.release.checkStatusAwaitingSignOff(data);

    app.release.information.render(data);
    app.release.property.render(data);
    app.release.comment.render(data);
    app.release.reason.render();
    app.release.workflow.modal.signoff.checkNavigation();
    app.release.workflow.read();
    app.release.ajax.checkLiveHasWorkInProgress();
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
    $("#release-information [name=analytical-label]").empty().html(app.library.html.parseStaticLabel(app.config.dataset.analytical.label));
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
            onlabel: app.label.static["true"],
            offlabel: app.label.static["false"],
            onstyle: "light",
            offstyle: "neutral",
            width: C_APP_TOGGLE_LENGTH,
            height: 38
        });
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle(data.RlsAnalyticalFlag ? "on" : "off");
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle('disable');

        // Disable the ability to set the navigation
        $("#release-information [name=navigation]").attr("disabled", true);
    }
    else {
        $("#release-information [name=rls-analytical-flag]").off("change");
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle("destroy").bootstrapToggle({
            onlabel: app.label.static["true"],
            offlabel: app.label.static["false"],
            onstyle: "success text-light",
            offstyle: "warning text-dark",
            width: C_APP_TOGGLE_LENGTH,
            height: 38
        });
        $("#release-information [name=rls-analytical-flag]").bootstrapToggle(data.RlsAnalyticalFlag ? "on" : "off");
        $("#release-information [name=rls-analytical-flag]").once("change", app.release.information.ajax.updateAnalyticalFlag);

        // Enable the ability to set the navigation
        $("#release-information [name=update-navigation]").attr("disabled", false);

        // Enable the ability to set the association but only if core product is set
        if (data.PrcCode) {
            $("#release-information [name=update-association]").attr("disabled", false);
        }
        else {
            $("#release-information [name=update-association]").attr("disabled", true);
        }

    }

    // Enable tooltip
    $('[data-bs-toggle="tooltip"]').tooltip();
};
//#endregion

//#region Toggle Flag
/**
* 
*/
app.release.information.ajax.updateAnalyticalFlag = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
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
    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};



//#endregion