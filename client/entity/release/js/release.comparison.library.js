/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.release = app.release || {};

app.release.comparison = {};
app.release.comparison.ajax = {};
app.release.comparison.callback = {};

app.release.comparison.previousReleaseData = null;
app.release.comparison.previousMatrixData = null;
app.release.comparison.currentReleaseData = null;
app.release.comparison.currentMatrixData = null;

app.release.comparison.previousReleaseMetaData = null;
app.release.comparison.currentReleaseMetaData = null;
//#endregion

//#region render

/**
* 
*/
app.release.comparison.render = function () {
    //open model action here
    $("#release-comparison-modal [name=mtr-code]").empty().html(app.release.MtrCode);
    $("#release-comparison-modal").modal("show");

    app.release.comparison.ajax.readPreviousRelease();
    app.release.comparison.ajax.readCurrentRelease();
    app.release.comparison.ajax.readPreviousMatrix();
    app.release.comparison.ajax.readCurrentMatrix();
    app.release.comparison.ajax.readAmendment();
    app.release.comparison.ajax.readReasonList();
    app.release.comparison.ajax.readPreviousMetadata();
    app.release.comparison.ajax.readCurrentMetadata();
};
//#endregion

//#region read information
/**
 * Get previous rls code from api
 */
app.release.comparison.ajax.readRlsCodePrevious = function () {
    return api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Compare_API.ReadPreviousRelease",
        { "RlsCode": app.release.RlsCode },
        "app.release.comparison.callback.readRlsCodePrevious",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Populate app.release.RlsCodePrevious and call readPreviousRelease
 * @param {*} data
 */
app.release.comparison.callback.readRlsCodePrevious = function (data) {
    if (data) {
        app.release.RlsCodePrevious = data.RlsCode;
        app.release.LngIsoCodePrevious = data.LngIsoCode;
        $("#release-source").find("[name=compare-release]").prop("disabled", false);
    }
    else {
        app.release.RlsCodePrevious = null;
        app.release.LngIsoCodePrevious = [];
        $("#release-source").find("[name=compare-release]").prop("disabled", true);
    }

};
/**
 * Read previous release information
 */
app.release.comparison.ajax.readPreviousRelease = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Release_API.Read",
        {
            "RlsCode": app.release.RlsCodePrevious,
            "LngIsoCode": $("#release-source [name=lng-iso-code] option:selected").val()
        },
        "app.release.comparison.callback.readPreviousRelease",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Draw previous release information
 * @param  {} data
*/
app.release.comparison.callback.readPreviousRelease = function (data) {
    app.release.comparison.previousReleaseData = data;
    app.release.comparison.callback.styleDifferences();
    //Information
    $("#release-comparison-report [name=rls-version-previous]").empty().html(app.release.comparison.previousReleaseData.RlsVersion);
    $("#release-comparison-report [name=rls-revision-previous]").empty().html(app.release.comparison.previousReleaseData.RlsRevision);
    $("#release-comparison-report [name=status-previous]").empty().html(app.release.renderStatus(app.release.comparison.previousReleaseData));
    $("#release-comparison-report [name=rls-live-datetime-from-previous]").empty().html(app.release.comparison.previousReleaseData.RlsLiveDatetimeFrom ? moment(app.release.comparison.previousReleaseData.RlsLiveDatetimeFrom).format(app.config.mask.datetime.display) : null);
    $("#release-comparison-report [name=rls-live-datetime-to-previous]").empty().html(app.release.comparison.previousReleaseData.RlsLiveDatetimeTo ? moment(app.release.comparison.previousReleaseData.RlsLiveDatetimeTo).format(app.config.mask.datetime.display) : null);
    $("#release-comparison-report [name=rls-exceptional-flag-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsExceptionalFlag, true, true));
    $("#release-comparison-report [name=grp-name-previous]").empty().html(app.library.html.link.group(app.release.comparison.previousReleaseData.GrpCode));
    $("#release-comparison-report [name=sbj-value-previous]").empty().html("(" + app.release.comparison.previousReleaseData.SbjCode + ") " + app.release.comparison.previousReleaseData.SbjValue);
    $("#release-comparison-report [name=prd-value-previous]").empty().html("(" + app.release.comparison.previousReleaseData.PrcCode + ") " + app.release.comparison.previousReleaseData.PrcValue);
    $("#release-comparison-report [name=rls-analytical-flag-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsAnalyticalFlag, true, true));
    $("#release-comparison-report [name=rls-reservation-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsReservationFlag, true, true));
    $("#release-comparison-report [name=rls-experimental-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsExperimentalFlag, true, true));
    $("#release-comparison-report [name=rls-archive-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsArchiveFlag, true, true));
};

/**
 * Read previous Matrix 
 */
app.release.comparison.ajax.readPreviousMatrix = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.Read",
        {
            "RlsCode": app.release.RlsCodePrevious,
            "LngIsoCode": $("#release-source [name=lng-iso-code] option:selected").val()
        },
        "app.release.comparison.callback.readPreviousMatrix",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Draw previous matrix information
 * @param  {} data
*/
app.release.comparison.callback.readPreviousMatrix = function (data) {
    app.release.comparison.previousMatrixData = data;
    app.release.comparison.callback.styleDifferences();
    $("#release-comparison-report [name=mtr-title-previous]").empty().html(app.release.comparison.previousMatrixData.MtrTitle);
    $("#release-comparison-report [name=dtg-create-datetime-previous]").empty().html(app.release.comparison.previousMatrixData.DtgCreateDatetime ? moment(app.release.comparison.previousMatrixData.DtgCreateDatetime).format(app.config.mask.datetime.display) : null);
    $("#release-comparison-report [name=ccn-username-previous]").empty().html(app.library.html.link.user(app.release.comparison.previousMatrixData.CcnUsernameCreate));
    $("#release-comparison-report [name=frq-value-previous]").empty().html(app.release.comparison.previousMatrixData.FrqValue);
    $("#release-comparison-report [name=cpr-value-previous]").empty().html(app.release.comparison.previousMatrixData.CprValue);
    $("#release-comparison-report [name=mtr-official-flag-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousMatrixData.MtrOfficialFlag, true, true));
    $("#release-comparison-report [name=mtr-note-previous]").empty().html(app.library.html.parseBbCode(app.release.comparison.previousMatrixData.MtrNote));
};

/**
 * Read current release information
 */
app.release.comparison.ajax.readCurrentRelease = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Release_API.Read",
        {
            "RlsCode": app.release.RlsCode,
            "LngIsoCode": $("#release-source [name=lng-iso-code] option:selected").val()
        },
        "app.release.comparison.callback.readCurrentRelease",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Draw current release information
 * @param  {} data
*/
app.release.comparison.callback.readCurrentRelease = function (data) {
    app.release.comparison.currentReleaseData = data;
    app.release.comparison.callback.styleDifferences();

    //Information
    $("#release-comparison-report [name=workflow-request-heading]").empty().html(app.release.comparison.currentReleaseData.RqsCode ? app.label.static[app.release.comparison.currentReleaseData.RqsValue] : null);
    $("#release-comparison-report [name=rls-version-current]").empty().html(app.release.comparison.currentReleaseData.RlsVersion);
    $("#release-comparison-report [name=rls-revision-current]").empty().html(app.release.comparison.currentReleaseData.RlsRevision);
    $("#release-comparison-report [name=status-current]").empty().html(app.release.renderStatus(app.release.comparison.currentReleaseData));
    $("#release-comparison-report [name=rls-live-datetime-from-current]").empty().html(app.release.comparison.currentReleaseData.RlsLiveDatetimeFrom ? moment(app.release.comparison.currentReleaseData.RlsLiveDatetimeFrom).format(app.config.mask.datetime.display) : null);
    $("#release-comparison-report [name=wrq-live-datetime-from-current]").empty().html(app.release.comparison.currentReleaseData.RqsCode ? moment(app.release.comparison.currentReleaseData.WrqDatetime).format(app.config.mask.datetime.display) : null);
    $("#release-comparison-report [name=rls-live-datetime-to-current]").empty().html(app.release.comparison.currentReleaseData.RlsLiveDatetimeTo ? moment(app.release.comparison.currentReleaseData.RlsLiveDatetimeTo).format(app.config.mask.datetime.display) : null);
    $("#release-comparison-report [name=rls-exceptional-flag-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsExceptionalFlag, true, true));
    $("#release-comparison-report [name=wrq-exceptional-flag-current]").empty().html(app.release.comparison.currentReleaseData.RqsCode ? app.library.html.boolean(app.release.comparison.currentReleaseData.WrqExceptionalFlag, true, true) : null);
    $("#release-comparison-report [name=grp-name-current]").empty().html(app.library.html.link.group(app.release.comparison.currentReleaseData.GrpCode));
    $("#release-comparison-report [name=sbj-value-current]").empty().html("(" + app.release.comparison.currentReleaseData.SbjCode + ") " + app.release.comparison.currentReleaseData.SbjValue);
    $("#release-comparison-report [name=prc-value-current]").empty().html("(" + app.release.comparison.currentReleaseData.PrcCode + ") " + app.release.comparison.currentReleaseData.PrcValue);
    $("#release-comparison-report [name=rls-analytical-flag-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsAnalyticalFlag, true, true));
    $("#release-comparison-report [name=rls-reservation-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsReservationFlag, true, true));

    $("#release-comparison-report [name=wrq-reservation-current]").empty().html(app.release.comparison.currentReleaseData.RqsCode ? app.library.html.boolean(app.release.comparison.currentReleaseData.WrqReservationFlag, true, true) : null); $("#release-comparison-report [name=rls-experimental-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsExperimentalFlag, true, true));

    $("#release-comparison-report [name=wrq-experimental-current]").empty().html(app.release.comparison.currentReleaseData.RqsCode ? app.library.html.boolean(app.release.comparison.currentReleaseData.WrqExperimentalFlag, true, true) : null);

    $("#release-comparison-report [name=rls-archive-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsArchiveFlag, true, true));

    $("#release-comparison-report [name=wrq-archive-current]").empty().html(app.release.comparison.currentReleaseData.RqsCode ? app.library.html.boolean(app.release.comparison.currentReleaseData.WrqArchiveFlag, true, true) : null);
};



/**
 * Draw current matrix information
*/
app.release.comparison.ajax.readCurrentMatrix = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.Read",
        {
            "RlsCode": app.release.RlsCode,
            "LngIsoCode": $("#release-source [name=lng-iso-code] option:selected").val()
        },
        "app.release.comparison.callback.readCurrentMatrix",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Draw current matrix information
 * @param  {} data
*/
app.release.comparison.callback.readCurrentMatrix = function (data) {
    app.release.comparison.currentMatrixData = data;
    app.release.comparison.callback.styleDifferences();
    $("#release-comparison-report [name=mtr-title-current]").empty().html(app.release.comparison.currentMatrixData.MtrTitle);
    $("#release-comparison-report [name=dtg-create-datetime-current]").empty().html(app.release.comparison.currentMatrixData.DtgCreateDatetime ? moment(app.release.comparison.currentMatrixData.DtgCreateDatetime).format(app.config.mask.datetime.display) : null);
    $("#release-comparison-report [name=ccn-username-current]").empty().html(app.library.html.link.user(app.release.comparison.currentMatrixData.CcnUsernameCreate));
    $("#release-comparison-report [name=frq-value-current]").empty().html(app.release.comparison.currentMatrixData.FrqValue);
    $("#release-comparison-report [name=cpr-value-current]").empty().html(app.release.comparison.currentMatrixData.CprValue);
    $("#release-comparison-report [name=mtr-official-flag-current]").empty().html(app.library.html.boolean(app.release.comparison.currentMatrixData.MtrOfficialFlag, true, true));
    $("#release-comparison-report [name=mtr-note-current]").empty().html(app.library.html.parseBbCode(app.release.comparison.currentMatrixData.MtrNote));
};

/**
* 
*/
app.release.comparison.callback.styleDifferences = function () {
    $("#release-comparison-report [name=analytical-label]").empty().html(app.library.html.parseStaticLabel(app.config.dataset.analytical.label));

    if (app.release.comparison.previousReleaseData &&
        app.release.comparison.previousMatrixData &&
        app.release.comparison.currentReleaseData &&
        app.release.comparison.currentMatrixData) {
        //exceptional
        if (app.release.comparison.currentReleaseData.RqsCode) {
            if (app.release.comparison.previousReleaseData.RlsExceptionalFlag != app.release.comparison.currentReleaseData.RlsExceptionalFlag
                || app.release.comparison.currentReleaseData.RlsExceptionalFlag != app.release.comparison.currentReleaseData.WrqExceptionalFlag
                || app.release.comparison.previousReleaseData.RlsExceptionalFlag != app.release.comparison.currentReleaseData.WrqExceptionalFlag) {
                $("#release-comparison-report").find("[name=exceptional-row]").find("td[name=rls-exceptional-flag-previous],td[name=rls-exceptional-flag-current], td[name=wrq-exceptional-flag-current]").addClass(app.config.entity.release.comparison.differenceClass);
            }
        }
        else {
            if (app.release.comparison.previousReleaseData.RlsExceptionalFlag != app.release.comparison.currentReleaseData.RlsExceptionalFlag) {
                $("#release-comparison-report").find("[name=exceptional-row]").find("td[name=rls-exceptional-flag-previous],td[name=rls-exceptional-flag-current]").addClass(app.config.entity.release.comparison.differenceClass);
            }
        }

        //group
        if ((app.release.comparison.previousReleaseData.GrpCode != app.release.comparison.currentReleaseData.GrpCode) ||
            (app.release.comparison.previousReleaseData.GrpValue != app.release.comparison.currentReleaseData.GrpValue)) {
            $("#release-comparison-report").find("[name=group-row]").find("td[name=grp-name-previous],td[name=grp-name-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //subject
        if ((app.release.comparison.previousReleaseData.SbjCode != app.release.comparison.currentReleaseData.SbjCode) ||
            (app.release.comparison.previousReleaseData.SbjValue != app.release.comparison.currentReleaseData.SbjValue)) {
            $("#release-comparison-report").find("[name=subject-row]").find("td[name=sbj-value-previous], td[name=sbj-value-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //product
        if ((app.release.comparison.previousReleaseData.PrcCode != app.release.comparison.currentReleaseData.PrcCode) ||
            (app.release.comparison.previousReleaseData.PrcValue != app.release.comparison.currentReleaseData.PrcValue)) {
            $("#release-comparison-report").find("[name=product-row]").find("td[name=prd-value-previous], td[name=prd-value-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //analytical
        if (app.release.comparison.previousReleaseData.RlsAnalyticalFlag != app.release.comparison.currentReleaseData.RlsAnalyticalFlag) {
            $("#release-comparison-report").find("[name=analytical-row]").find("td[name=rls-analytical-flag-previous], td[name=rls-analytical-flag-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //title
        if (app.release.comparison.previousMatrixData.MtrTitle != app.release.comparison.currentMatrixData.MtrTitle) {
            $("#release-comparison-report").find("[name=title-row]").find("td[name=mtr-title-previous], td[name=mtr-title-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //frequency
        if (app.release.comparison.previousMatrixData.FrqValue != app.release.comparison.currentMatrixData.FrqValue) {
            $("#release-comparison-report").find("[name=frequency-row]").find("td[name=frq-value-previous], td[name=frq-value-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //copyright
        if (app.release.comparison.previousMatrixData.CprValue != app.release.comparison.currentMatrixData.CprValue) {
            $("#release-comparison-report").find("[name=copyright-row]").find("td[name=cpr-value-previous], td[name=cpr-value-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //official
        if (app.release.comparison.previousMatrixData.MtrOfficialFlag != app.release.comparison.currentMatrixData.MtrOfficialFlag) {
            $("#release-comparison-report").find("[name=official-row]").find("td[name=mtr-official-flag-previous], td[name=mtr-official-flag-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //footnote
        if (app.release.comparison.previousMatrixData.MtrNote != app.release.comparison.currentMatrixData.MtrNote) {
            $("#release-comparison-report").find("[name=footnote-row]").find("td[name=mtr-note-previous], td[name=mtr-note-current]").addClass(app.config.entity.release.comparison.differenceClass);
        }

        //reservation
        if (app.release.comparison.currentReleaseData.RqsCode) {
            if (app.release.comparison.previousReleaseData.RlsReservationFlag != app.release.comparison.currentReleaseData.RlsReservationFlag
                || app.release.comparison.previousReleaseData.RlsReservationFlag != app.release.comparison.currentReleaseData.WrqReservationFlag
                || app.release.comparison.currentReleaseData.RlsReservationFlag != app.release.comparison.currentReleaseData.WrqReservationFlag
            ) {
                $("#release-comparison-report").find("[name=reservation-row]").find("td[name=rls-reservation-previous], td[name=rls-reservation-current], td[name=wrq-reservation-current]").addClass(app.config.entity.release.comparison.differenceClass);
            }
        }
        else {
            if (app.release.comparison.previousReleaseData.RlsReservationFlag != app.release.comparison.currentReleaseData.RlsReservationFlag) {
                $("#release-comparison-report").find("[name=reservation-row]").find("td[name=rls-reservation-previous], td[name=rls-reservation-current]").addClass(app.config.entity.release.comparison.differenceClass);
            }
        }

        //archive
        if (app.release.comparison.currentReleaseData.RqsCode) {
            if (app.release.comparison.previousReleaseData.RlsArchiveFlag != app.release.comparison.currentReleaseData.RlsArchiveFlag
                || app.release.comparison.previousReleaseData.RlsArchiveFlag != app.release.comparison.currentReleaseData.WrqArchiveFlag
                || app.release.comparison.currentReleaseData.RlsArchiveFlag != app.release.comparison.currentReleaseData.WrqArchiveFlag
            ) {
                $("#release-comparison-report").find("[name=archive-row]").find("td[name=rls-archive-previous], td[name=rls-archive-current], td[name=wrq-archive-current]").addClass(app.config.entity.release.comparison.differenceClass);
            }
        }
        else {
            if (app.release.comparison.previousReleaseData.RlsArchiveFlag != app.release.comparison.currentReleaseData.RlsArchiveFlag) {
                $("#release-comparison-report").find("[name=archive-row]").find("td[name=rls-archive-previous], td[name=rls-archive-current]").addClass(app.config.entity.release.comparison.differenceClass);
            }
        }

        //experimental
        if (app.release.comparison.currentReleaseData.RqsCode) {
            if (app.release.comparison.previousReleaseData.RlsExperimentalFlag != app.release.comparison.currentReleaseData.RlsExperimentalFlag
                || app.release.comparison.previousReleaseData.RlsExperimentalFlag != app.release.comparison.currentReleaseData.WrqExperimentalFlag
                || app.release.comparison.currentReleaseData.RlsExperimentalFlag != app.release.comparison.currentReleaseData.WrqExperimentalFlag
            ) {
                $("#release-comparison-report").find("[name=experimental-row]").find("td[name=rls-experimental-previous], td[name=rls-experimental-current], td[name=wrq-experimental-current]").addClass(app.config.entity.release.comparison.differenceClass);
            }
        }
        else {
            if (app.release.comparison.previousReleaseData.RlsExperimentalFlag != app.release.comparison.currentReleaseData.RlsExperimentalFlag) {
                $("#release-comparison-report").find("[name=experimental-row]").find("td[name=rls-experimental-previous], td[name=rls-archive-current]").addClass(app.config.entity.release.comparison.differenceClass);
            }
        }

    }
};

//#endregion

//#region amendment

/**
 * Call read amendment
 */
app.release.comparison.ajax.readAmendment = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Compare_API.ReadAmendment",
        {
            "RlsCode": app.release.RlsCode,
            "LngIsoCode": $("#release-source [name=lng-iso-code] option:selected").val()
        },
        "app.release.comparison.callback.readAmendment",
        null,
        null,
        null,
        {
            async: false
        }
    );
};

/**
 * Call function to draw datatable with data
 * @param  {} data
 */
app.release.comparison.callback.readAmendment = function (data) {
    app.release.comparison.callback.readComparison(data, "#comparison-datatable-amendment");
    app.release.comparison.ajax.readDeletion();
};
//#endregion

//#region deletion

/**
 * Call read deletion
 */
app.release.comparison.ajax.readDeletion = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Compare_API.ReadDeletion",
        {
            "RlsCode": app.release.RlsCode,
            "LngIsoCode": $("#release-source [name=lng-iso-code] option:selected").val()
        },
        "app.release.comparison.callback.readDeletion",
        null,
        null,
        null,
        {
            async: false
        }
    );
};

/**
 * Call function to draw datatable with data
 * @param  {} data
 */
app.release.comparison.callback.readDeletion = function (data) {
    app.release.comparison.callback.readComparison(data, "#comparison-datatable-deletion");
    app.release.comparison.ajax.readAddition();
};
//#endregion

//#region addition

/**
 * Call read addition
 */
app.release.comparison.ajax.readAddition = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Compare_API.ReadAddition",
        {
            "RlsCode": app.release.RlsCode,
            "LngIsoCode": $("#release-source [name=lng-iso-code] option:selected").val()
        },
        "app.release.comparison.callback.readAddition",
        null,
        null,
        null,
        {
            async: false
        }
    );
};

/**
 * Call function to draw datatable with data
 * @param  {} data
 */
app.release.comparison.callback.readAddition = function (data) {
    app.release.comparison.callback.readComparison(data, "#comparison-datatable-addition");
};
//#endregion

/**
 * Draw Callback for Datatable
 */
app.release.comparison.drawCallbackReadComparsion = function () {
    //toggle amendment codes
    if (!$("#comparison-amendment-toggle").prop('checked')) {
        $("#comparison-datatable-amendment").find("[name=code]").removeClass("d-none");
    }
    else {
        $("#comparison-datatable-amendment").find("[name=code]").addClass("d-none");
    }
    $("#comparison-amendment-toggle").once("change", function () {
        if ($(this).prop('checked')) {
            $("#comparison-datatable-amendment").find("[name=code]").addClass("d-none");
        }
        else {
            $("#comparison-datatable-amendment").find("[name=code]").removeClass("d-none");
        }
    });
    //toggle additions codes
    if (!$("#comparison-addition-toggle").prop('checked')) {
        $("#comparison-datatable-addition").find("[name=code]").removeClass("d-none");
    }
    else {
        $("#comparison-datatable-addition").find("[name=code]").addClass("d-none");
    }
    $("#comparison-addition-toggle").once("change", function () {
        if ($(this).prop('checked')) {
            $("#comparison-datatable-addition").find("[name=code]").addClass("d-none");
        }
        else {
            $("#comparison-datatable-addition").find("[name=code]").removeClass("d-none");
        }
    });
    //toggle deletion codes
    if (!$("#comparison-deletion-toggle").prop('checked')) {
        $("#comparison-datatable-deletion").find("[name=code]").removeClass("d-none");
    }
    else {
        $("#comparison-datatable-deletion").find("[name=code]").addClass("d-none");
    }
    $("#comparison-deletion-toggle").once("change", function () {
        if ($(this).prop('checked')) {
            $("#comparison-datatable-deletion").find("[name=code]").addClass("d-none");
        }
        else {
            $("#comparison-datatable-deletion").find("[name=code]").removeClass("d-none");
        }
    });
}

//#region draw comparison table
/**
 * Draw datatable for each comparison
 * @param  {} data
 * @param  {} selector
 */
app.release.comparison.callback.readComparison = function (data, selector) {
    data = data ? JSONstat(data) : null;
    if (data && data.length) {
        // Start the spinner manually because the rendering of the Comparison may take a long time
        api.spinner.start();
        var table = $("#release-comparison-templates").find("[name=comparison-table]").clone();
        $(selector).find("[name=table-wrapper]").html(table);
        if ($.fn.DataTable.isDataTable($(selector).find("[name=comparison-table]"))) {
            $(selector).find("[name=comparison-table]").DataTable().destroy();
            //cannot use redraw as columns are dynamically created depending on the matrix. Have to destroy and re-initiate 
        }
        var exportFileName = data.extension.matrix;
        //set export file name
        switch (selector) {
            case "#comparison-datatable-addition":
                exportFileName = exportFileName + "_" + "addition" + "." + moment().format(app.config.mask.datetime.file);
                break;

            case "#comparison-datatable-deletion":
                exportFileName = exportFileName + "_" + "deletion" + "." + moment().format(app.config.mask.datetime.file);
                break;

            case "#comparison-datatable-amendment":
                exportFileName = exportFileName + "_" + "amendment" + "." + moment().format(app.config.mask.datetime.file);
                break;

            default:
                exportFileName = data.extension.matrix + "." + moment().format(app.config.mask.datetime.file)
                break;
        }
        var jsonTableLabel = data.toTable({
            type: 'arrobj',
            meta: true,
            status: true,
            unit: true,
            content: "label"
        });
        var jsonTableId = data.toTable({
            type: 'arrobj',
            meta: true,
            status: true,
            unit: true,
            content: "id"
        });

        //loop through data arrays to figure out which items you need to remove, status == false
        jsonTableId.data = $.grep(jsonTableId.data, function (value, index) {
            return value.status == "false" ? false : true;
        });
        jsonTableLabel.data = $.grep(jsonTableLabel.data, function (value, index) {
            return value.status == "false" ? false : true;
        });

        var numDimensions = data.length;
        var tableColumns = [];


        for (i = 0; i < numDimensions; i++) { //build columns
            var codeSpan = $('<span>', {
                "name": "code",
                "class": "badge rounded-pill bg-neutral text-dark mx-2 d-none",
                "text": data.id[i]
            }).get(0).outerHTML;

            var tableHeading = $("<th>", {
                "html": data.Dimension(i).label + codeSpan
            });

            //Data Table header
            $(selector).find("[name=comparison-table] thead tr").append(tableHeading);

            tableColumns.push({
                data: data.id[i],
                render: function (data, type, row, meta) {
                    var dimensionLabel = jsonTableId.meta.id[meta.col];
                    var codeSpan = $('<span>', {
                        "name": "code",
                        "class": "badge rounded-pill bg-neutral text-dark mx-2 d-none",
                        "text": jsonTableId.data[meta.row][dimensionLabel]
                    }).get(0).outerHTML;
                    return data + codeSpan;
                }
            });
        }
        var unitHeading = $("<th>",
            {
                "class": "unit"
            });
        unitHeading.html("Unit");
        $(selector).find("[name=comparison-table] thead tr").append(unitHeading);
        tableColumns.push({
            data: 'unit.label'
        });
        var decimalsHeading = $("<th>");
        decimalsHeading.html("Decimals");
        $(selector).find("[name=comparison-table] thead tr").append(decimalsHeading);
        tableColumns.push({
            data: 'unit.decimals'
        });
        tableColumns.push({
            "data": 'value',
            "type": "natural-nohtml",
            "defaultContent": app.config.entity.data.datatable.null,
            "class": "text-end",
            "render": function (data, type, row, meta) {
                return app.library.utility.formatNumber(data, row.unit.decimals);
            }
        });
        var valueHeading = $("<th>",
            {
                "class": "value text-end"
            });
        valueHeading.html("Value");
        $(selector).find("[name=comparison-table] thead tr").append(valueHeading);
        //Draw DataTable with Data Set data
        var localOptions = {
            data: jsonTableLabel.data,
            defaultContent: app.config.entity.data.datatable.null,
            columns: tableColumns,
            buttons: [{
                extend: 'csv',
                title: exportFileName,
                exportOptions: {
                    format: {
                        body: function (data, row, column, node) {
                            //remove hidden codes
                            return data.toString().replace(C_APP_REGEX_NOSPAN, "");
                        },
                        header: function (data, row, column, node) {
                            //remove hidden codes
                            return data.toString().replace(C_APP_REGEX_NOSPAN, "");
                        }
                    }
                }
            }],
            deferRender: true,
            drawCallback: function (settings) {
                app.release.comparison.drawCallbackReadComparsion();
                api.spinner.stop();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $(selector).find("[name=comparison-table]").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.comparison.drawCallbackReadComparsion();
        });

        // Invoke DataTables CSV export
        // https://stackoverflow.com/questions/45515559/how-to-call-datatable-csv-button-from-custom-button
        $(selector).find("[name=csv]").once("click", function () {
            $(selector).find("[name=comparison-table]").DataTable().button('.buttons-csv').trigger();
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion

//#region reasons
/**
 * Read reasons
 */
app.release.comparison.ajax.readReasonList = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.ReasonRelease_API.Read",
        { RlsCode: app.release.RlsCode }, //update to dynamic Rlscode when merging
        "app.release.comparison.callback.readReasonList",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Draw Callback for Datatable
 */
app.release.comparison.drawCallbackReasonList = function () {
    app.library.datatable.showExtraInfo('#release-comparison-reason table', app.release.comparison.drawExtraInformation);
}
/**
 * Draw comparison
 * @param  {} data
 */
app.release.comparison.callback.readReasonList = function (data) {
    // Populate Release Reason
    if ($.fn.dataTable.isDataTable("#release-comparison-reason table")) {
        app.library.datatable.reDraw("#release-comparison-reason table", data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            columns: [
                {
                    data: "RsnCode"
                },
                {
                    data: null,
                    defaultContent: '',
                    sorting: false,
                    searchable: false,
                    "render": function (data, type, row, meta) {
                        return $("<a>", {
                            href: "#",
                            name: C_APP_DATATABLE_EXTRA_INFO_LINK,
                            "idn": meta.row,
                            html:
                                $("<i>", {
                                    "class": "fas fa-info-circle text-info"
                                }).get(0).outerHTML + " " + app.label.static["description"]
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: "RsnValueInternal",
                    "visible": false,
                    "searchable": true
                },
                {
                    data: "RsnValueExternal",
                    "visible": false,
                    "searchable": true
                },
            ],
            drawCallback: function (settings) {
                app.release.comparison.drawCallbackReasonList();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#release-comparison-reason table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.comparison.drawCallbackReasonList();
        });
    }
};

/**
 * Draw extra information
 * @param  {} d
 */
app.release.comparison.drawExtraInformation = function (d) {
    var requestGrid = $("#release-reason [name=extra-information]").clone();
    requestGrid.removeAttr('name');
    requestGrid.find("[name=rsn-value-internal]").empty().html(app.library.html.parseBbCode(d.RsnValueInternal));
    requestGrid.find("[name=rsn-value-external]").empty().html(app.library.html.parseBbCode(d.RsnValueExternal));
    requestGrid.find("[name=cmm-value]").empty().html(app.library.html.parseBbCode(d.CmmValue));
    return requestGrid.show().get(0).outerHTML;
};
//#endregion

//#region metadata
app.release.comparison.ajax.readPreviousMetadata = function () {
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Data.Cube_API.ReadPreMetadata",
        {
            "release": app.release.RlsCodePrevious,
            "format": {
                "type": C_APP_FORMAT_TYPE_DEFAULT,
                "version": C_APP_FORMAT_VERSION_DEFAULT
            },
            "language": $("#release-source [name=lng-iso-code] option:selected").val(),
            "m2m": false
        },
        "app.release.comparison.callback.readPreviousMetadata",
        null,
        null,
        null,
        { async: false });
};

app.release.comparison.callback.readPreviousMetadata = function (data) {
    app.release.comparison.previousReleaseMetaData = data ? JSONstat(data) : null;
    app.release.comparison.callback.compareMetadata();
};

app.release.comparison.ajax.readCurrentMetadata = function () {
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
        "PxStat.Data.Cube_API.ReadPreMetadata",
        {
            "release": app.release.RlsCode,
            "format": {
                "type": C_APP_FORMAT_TYPE_DEFAULT,
                "version": C_APP_FORMAT_VERSION_DEFAULT
            },
            "language": $("#release-source [name=lng-iso-code] option:selected").val(),
            "m2m": false
        },
        "app.release.comparison.callback.readCurrentMetadata",
        null,
        null,
        null,
        { async: false });
};

app.release.comparison.callback.readCurrentMetadata = function (data) {
    app.release.comparison.currentReleaseMetaData = data ? JSONstat(data) : null;
    app.release.comparison.callback.compareMetadata();
};

app.release.comparison.callback.compareMetadata = function () {
    if (!app.release.comparison.previousReleaseMetaData || !app.release.comparison.currentReleaseMetaData) {
        return;
    }
    var previousDimensions = [];
    $.each(app.release.comparison.previousReleaseMetaData.id, function (i, v) {
        var dimension = {
            "code": v,
            "variables": []
        };
        $.each(app.release.comparison.previousReleaseMetaData.Dimension(v).id, function (index, code) {
            dimension.variables.push(code);
        });
        previousDimensions.push(dimension)
    });
    var currentDimensions = [];
    $.each(app.release.comparison.currentReleaseMetaData.id, function (i, v) {
        var dimension = {
            "code": v,
            "variables": []
        };
        $.each(app.release.comparison.currentReleaseMetaData.Dimension(v).id, function (index, code) {
            dimension.variables.push(code);
        });
        currentDimensions.push(dimension)
    });
    //new combined array with deleted/additional dimension/variables
    var dimensionsCompare = {};
    $.each(previousDimensions, function (index, value) {
        dimensionsCompare[value.code] = {
            "previousVariables": [],
            "currentVariables": [],
            "deletedVariableCodes": [],
            "deletedVariableLabels": [],
            "additionalVariableCodes": [],
            "additionalVariableLabels": []
        };
    });

    $.each(currentDimensions, function (index, value) {
        dimensionsCompare[value.code] = {
            "previousVariables": [],
            "currentVariables": [],
            "deletedVariableCodes": [],
            "deletedVariableLabels": [],
            "additionalVariableCodes": [],
            "additionalVariableLabels": []
        };
    });

    //populate variables
    $.each(dimensionsCompare, function (key, value) {
        //find previous variables for this dimension
        var previousDimension = $.grep(previousDimensions, function (n) {
            return n.code == key;
        });

        if (previousDimension[0]) {
            value.previousVariables = previousDimension[0].variables;
        }

        //find current variables for this dimension
        var currentDimension = $.grep(currentDimensions, function (n) {
            return n.code == key;
        });

        if (currentDimension[0]) {
            value.currentVariables = currentDimension[0].variables;
        }
    });

    $.each(dimensionsCompare, function (key, value) {
        //check for deleted variables
        //variables that are in previous release, but not in current
        $.each(value.previousVariables, function (k, v) {
            if ($.inArray(v, value.currentVariables) < 0) {
                dimensionsCompare[key].deletedVariableCodes.push(v);
                dimensionsCompare[key].deletedVariableLabels.push(app.release.comparison.previousReleaseMetaData.Dimension(key).Category(v).label);
            }
        });

        //check for additional variables
        //variables that are in current release, but not in previous
        $.each(value.currentVariables, function (k, v) {
            if ($.inArray(v, value.previousVariables) < 0) {
                dimensionsCompare[key].additionalVariableCodes.push(v);
                dimensionsCompare[key].additionalVariableLabels.push(app.release.comparison.currentReleaseMetaData.Dimension(key).Category(v).label);
            }
        });
    });

    var datatableData = [];

    $.each(dimensionsCompare, function (key, value) {
        datatableData.push({
            "code": key,
            "label": app.release.comparison.currentReleaseMetaData.Dimension(key)
                ? app.release.comparison.currentReleaseMetaData.Dimension(key).label
                : app.release.comparison.previousReleaseMetaData.Dimension(key).label,
            "additions": value.additionalVariableCodes.length,
            "deletions": value.deletedVariableCodes.length
        });
    });


    var table = $("#release-comparison-metadata").find("[name=compare-metadata]");
    if ($.fn.DataTable.isDataTable(table)) {
        app.library.datatable.reDraw(table, datatableData);
    } else {
        var localOptions = {
            data: datatableData,
            columns: [
                {
                    data: "code"
                },
                {
                    data: "label"
                },
                {
                    data: null,
                    createdCell: function (td, cellData, rowData, row, col) {
                        if (cellData.deletions > 0) {
                            $(td).addClass('table-danger')
                        }
                    },
                    render: function (data, type, row) {
                        return app.library.html.link.view({ "dimension-code": row.code, "action": "view-deletions" }, row.deletions.toString());
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.view({ "dimension-code": row.code, "action": "view-additions" }, row.additions.toString());
                    }
                },
            ],
            drawCallback: function (settings) {
                app.release.comparison.callback.drawCallbackMetadata(table, dimensionsCompare);
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $(table).DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.comparison.callback.drawCallbackMetadata(table, dimensionsCompare);
        });
    };

};

app.release.comparison.callback.drawCallbackMetadata = function (table, dimensionsCompare) {
    $(table).find("[name=" + C_APP_NAME_LINK_VIEW + "][action='view-deletions']").once("click", function (e) {
        e.preventDefault();
        app.release.comparison.callback.viewDeletedVariables(dimensionsCompare, $(this).attr("dimension-code"));
    });

    $(table).find("[name=" + C_APP_NAME_LINK_VIEW + "][action='view-additions']").once("click", function (e) {
        e.preventDefault();
        app.release.comparison.callback.viewAddedVariables(dimensionsCompare, $(this).attr("dimension-code"));
    });
};

app.release.comparison.callback.viewDeletedVariables = function (dimensionsCompare, dimension) {
    $("#release-comparison-modal-view-delitions").find("[name=dimension-code]").html(dimension);
    $("#release-comparison-modal-view-delitions").find("[name=dimension-value]").html(
        app.release.comparison.previousReleaseMetaData.Dimension(dimension)
            ? app.release.comparison.previousReleaseMetaData.Dimension(dimension).label
            : app.release.comparison.currentReleaseMetaData.Dimension(dimension).label
    );

    var data = [];
    $.each(dimensionsCompare[dimension].deletedVariableCodes, function (index, value) {
        data.push(
            {
                "code": value,
                "value": dimensionsCompare[dimension].deletedVariableLabels[index]
            }
        )
    });


    if ($.fn.dataTable.isDataTable("#release-comparison-modal-view-delitions table")) {
        app.library.datatable.reDraw("#release-comparison-modal-view-delitions table", data);
    } else {
        var localOptions = {
            data: data,
            ordering: false,
            columns: [
                { data: "code" },
                { data: "value" },
            ]
        };
        //Initialize DataTable
        $("#release-comparison-modal-view-delitions table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions));
    };
    $("#release-comparison-modal-view-delitions").modal("show");
};

app.release.comparison.callback.viewAddedVariables = function (dimensionsCompare, dimension) {
    $("#release-comparison-modal-view-additions").find("[name=dimension-code]").html(dimension);
    $("#release-comparison-modal-view-additions").find("[name=dimension-value]").html(
        app.release.comparison.currentReleaseMetaData.Dimension(dimension)
            ? app.release.comparison.currentReleaseMetaData.Dimension(dimension).label
            : app.release.comparison.previousReleaseMetaData.Dimension(dimension).label
    );

    var data = [];
    $.each(dimensionsCompare[dimension].additionalVariableCodes, function (index, value) {
        data.push(
            {
                "code": value,
                "value": dimensionsCompare[dimension].additionalVariableLabels[index]
            }
        )
    });


    if ($.fn.dataTable.isDataTable("#release-comparison-modal-view-additions table")) {
        app.library.datatable.reDraw("#release-comparison-modal-view-additions table", data);
    } else {
        var localOptions = {
            data: data,
            ordering: false,
            columns: [
                { data: "code" },
                { data: "value" },
            ]
        };
        //Initialize DataTable
        $("#release-comparison-modal-view-additions table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions));
    };
    $("#release-comparison-modal-view-additions").modal("show");
};
//#endregion