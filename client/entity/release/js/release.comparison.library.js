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

};
//#endregion

//#region read information
/**
 * Get previous rls code from api
 */
app.release.comparison.ajax.readRlsCodePrevious = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
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
 * @param {*} response
 */
app.release.comparison.callback.readRlsCodePrevious = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        app.release.RlsCodePrevious = null;
        $("#release-source").find("[name=compare-release]").prop("disabled", true);
    } else if (response.data) {
        app.release.RlsCodePrevious = response.data;
        $("#release-source").find("[name=compare-release]").prop("disabled", false);

    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

};
/**
 * Read previous release information
 */
app.release.comparison.ajax.readPreviousRelease = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
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
 * @param  {} response
*/
app.release.comparison.callback.readPreviousRelease = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.release.comparison.previousReleaseData = response.data;
        app.release.comparison.callback.styleDifferences();
        //Information
        $("#release-comparison-report [name=rls-version-previous]").empty().html(app.release.comparison.previousReleaseData.RlsVersion);
        $("#release-comparison-report [name=rls-revision-previous]").empty().html(app.release.comparison.previousReleaseData.RlsRevision);
        $("#release-comparison-report [name=status-previous]").empty().html(app.release.renderStatus(app.release.comparison.previousReleaseData));
        $("#release-comparison-report [name=rls-live-datetime-from-previous]").empty().html(app.release.comparison.previousReleaseData.RlsLiveDatetimeFrom ? moment(app.release.comparison.previousReleaseData.RlsLiveDatetimeFrom).format(app.config.mask.datetime.display) : "");
        $("#release-comparison-report [name=rls-live-datetime-to-previous]").empty().html(app.release.comparison.previousReleaseData.RlsLiveDatetimeTo ? moment(app.release.comparison.previousReleaseData.RlsLiveDatetimeTo).format(app.config.mask.datetime.display) : "");
        $("#release-comparison-report [name=rls-emergency-flag-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsEmergencyFlag, true, true));
        $("#release-comparison-report [name=grp-name-previous]").empty().html(app.library.html.link.group(app.release.comparison.previousReleaseData.GrpCode));
        $("#release-comparison-report [name=sbj-value-previous]").empty().html("(" + app.release.comparison.previousReleaseData.SbjCode + ") " + app.release.comparison.previousReleaseData.SbjValue);
        $("#release-comparison-report [name=prc-value-previous]").empty().html("(" + app.release.comparison.previousReleaseData.PrcCode + ") " + app.release.comparison.previousReleaseData.PrcValue);
        $("#release-comparison-report [name=rls-analytical-flag-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsAnalyticalFlag, true, true));
        $("#release-comparison-report [name=rls-dependency-flag-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsDependencyFlag, true, true));
        $("#release-comparison-report [name=rls-reservation-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsReservationFlag, true, true));
        $("#release-comparison-report [name=rls-archive-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousReleaseData.RlsArchiveFlag, true, true));

    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Read previous Matrix 
 */
app.release.comparison.ajax.readPreviousMatrix = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
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
 * @param  {} response
*/
app.release.comparison.callback.readPreviousMatrix = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.release.comparison.previousMatrixData = response.data;
        app.release.comparison.callback.styleDifferences();
        $("#release-comparison-report [name=mtr-title-previous]").empty().html(app.release.comparison.previousMatrixData.MtrTitle);
        $("#release-comparison-report [name=dtg-create-datetime-previous]").empty().html(moment(app.release.comparison.previousMatrixData.DtgCreateDatetime).format(app.config.mask.datetime.display));
        $("#release-comparison-report [name=ccn-username-previous]").empty().html(app.library.html.link.user(app.release.comparison.previousMatrixData.CcnUsernameCreate));
        $("#release-comparison-report [name=frq-value-previous]").empty().html(app.release.comparison.previousMatrixData.FrqValue);
        $("#release-comparison-report [name=cpr-value-previous]").empty().html(app.release.comparison.previousMatrixData.CprValue);
        $("#release-comparison-report [name=mtr-official-flag-previous]").empty().html(app.library.html.boolean(app.release.comparison.previousMatrixData.MtrOfficialFlag, true, true));
        $("#release-comparison-report [name=mtr-note-previous]").empty().html(app.library.html.parseBbCode(app.release.comparison.previousMatrixData.MtrNote));

    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Read current release information
 */
app.release.comparison.ajax.readCurrentRelease = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
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
 * @param  {} response
*/
app.release.comparison.callback.readCurrentRelease = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.release.comparison.currentReleaseData = response.data;
        app.release.comparison.callback.styleDifferences();
        //Information
        $("#release-comparison-report [name=rls-version-current]").empty().html(app.release.comparison.currentReleaseData.RlsVersion);
        $("#release-comparison-report [name=rls-revision-current]").empty().html(app.release.comparison.currentReleaseData.RlsRevision);
        $("#release-comparison-report [name=status-current]").empty().html(app.release.renderStatus(app.release.comparison.currentReleaseData));
        $("#release-comparison-report [name=request-current]").empty().html(app.release.renderRequest(app.release.comparison.currentReleaseData.RqsCode));
        $("#release-comparison-report [name=rls-live-datetime-from-current]").empty().html(app.release.comparison.currentReleaseData.RlsLiveDatetimeFrom ? moment(app.release.comparison.currentReleaseData.RlsLiveDatetimeFrom).format(app.config.mask.datetime.display) : "");
        $("#release-comparison-report [name=rls-live-datetime-to-current]").empty().html(app.release.comparison.currentReleaseData.RlsLiveDatetimeTo ? moment(app.release.comparison.currentReleaseData.RlsLiveDatetimeTo).format(app.config.mask.datetime.display) : "");
        $("#release-comparison-report [name=rls-emergency-flag-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsEmergencyFlag, true, true));
        $("#release-comparison-report [name=grp-name-current]").empty().html(app.library.html.link.group(app.release.comparison.currentReleaseData.GrpCode));
        $("#release-comparison-report [name=sbj-value-current]").empty().html("(" + app.release.comparison.currentReleaseData.SbjCode + ") " + app.release.comparison.currentReleaseData.SbjValue);
        $("#release-comparison-report [name=prc-value-current]").empty().html("(" + app.release.comparison.currentReleaseData.PrcCode + ") " + app.release.comparison.currentReleaseData.PrcValue);
        $("#release-comparison-report [name=rls-analytical-flag-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsAnalyticalFlag, true, true));
        $("#release-comparison-report [name=rls-dependency-flag-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsDependencyFlag, true, true));
        $("#release-comparison-report [name=rls-reservation-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsReservationFlag, true, true));
        $("#release-comparison-report [name=rls-archive-current]").empty().html(app.library.html.boolean(app.release.comparison.currentReleaseData.RlsArchiveFlag, true, true));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};



/**
 * Draw current matrix information
*/
app.release.comparison.ajax.readCurrentMatrix = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
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
 * @param  {} response
*/
app.release.comparison.callback.readCurrentMatrix = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        app.release.comparison.currentMatrixData = response.data;
        app.release.comparison.callback.styleDifferences();
        $("#release-comparison-report [name=mtr-title-current]").empty().html(app.release.comparison.currentMatrixData.MtrTitle);
        $("#release-comparison-report [name=dtg-create-datetime-current]").empty().html(moment(app.release.comparison.currentMatrixData.DtgCreateDatetime).format(app.config.mask.datetime.display));
        $("#release-comparison-report [name=ccn-username-current]").empty().html(app.library.html.link.user(app.release.comparison.currentMatrixData.CcnUsernameCreate));
        $("#release-comparison-report [name=frq-value-current]").empty().html(app.release.comparison.currentMatrixData.FrqValue);
        $("#release-comparison-report [name=cpr-value-current]").empty().html(app.release.comparison.currentMatrixData.CprValue);
        $("#release-comparison-report [name=mtr-official-flag-current]").empty().html(app.library.html.boolean(app.release.comparison.currentMatrixData.MtrOfficialFlag, true, true));
        $("#release-comparison-report [name=mtr-note-current]").empty().html(app.library.html.parseBbCode(app.release.comparison.currentMatrixData.MtrNote));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* 
*/
app.release.comparison.callback.styleDifferences = function () {
    if (app.release.comparison.previousReleaseData &&
        app.release.comparison.previousMatrixData &&
        app.release.comparison.currentReleaseData &&
        app.release.comparison.currentMatrixData) {
        //emergency
        if (app.release.comparison.previousReleaseData.RlsEmergencyFlag != app.release.comparison.currentReleaseData.RlsEmergencyFlag) {
            $("#release-comparison-report").find("[name=emergency-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=emergency-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //group
        if ((app.release.comparison.previousReleaseData.GrpCode != app.release.comparison.currentReleaseData.GrpCode) ||
            (app.release.comparison.previousReleaseData.GrpValue != app.release.comparison.currentReleaseData.GrpValue)) {
            $("#release-comparison-report").find("[name=group-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=group-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //subject
        if ((app.release.comparison.previousReleaseData.SbjCode != app.release.comparison.currentReleaseData.SbjCode) ||
            (app.release.comparison.previousReleaseData.SbjValue != app.release.comparison.currentReleaseData.SbjValue)) {
            $("#release-comparison-report").find("[name=subject-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=subject-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //product
        if ((app.release.comparison.previousReleaseData.PrcCode != app.release.comparison.currentReleaseData.PrcCode) ||
            (app.release.comparison.previousReleaseData.PrcValue != app.release.comparison.currentReleaseData.PrcValue)) {
            $("#release-comparison-report").find("[name=product-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=product-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //analytical
        if (app.release.comparison.previousReleaseData.RlsAnalyticalFlag != app.release.comparison.currentReleaseData.RlsAnalyticalFlag) {
            $("#release-comparison-report").find("[name=analytical-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=analytical-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //dependency
        if (app.release.comparison.previousReleaseData.RlsDependencyFlag != app.release.comparison.currentReleaseData.RlsDependencyFlag) {
            $("#release-comparison-report").find("[name=dependency-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=dependency-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //title
        if (app.release.comparison.previousMatrixData.MtrTitle != app.release.comparison.currentMatrixData.MtrTitle) {
            $("#release-comparison-report").find("[name=title-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=title-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //frequency
        if (app.release.comparison.previousMatrixData.FrqValue != app.release.comparison.currentMatrixData.FrqValue) {
            $("#release-comparison-report").find("[name=frequency-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=frequency-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //copyright
        if (app.release.comparison.previousMatrixData.CprValue != app.release.comparison.currentMatrixData.CprValue) {
            $("#release-comparison-report").find("[name=copyright-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=copyright-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //official
        if (app.release.comparison.previousMatrixData.MtrOfficialFlag != app.release.comparison.currentMatrixData.MtrOfficialFlag) {
            $("#release-comparison-report").find("[name=official-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=official-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //footnote
        if (app.release.comparison.previousMatrixData.MtrNote != app.release.comparison.currentMatrixData.MtrNote) {
            $("#release-comparison-report").find("[name=footnote-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=footnote-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }


        //reservation
        if (app.release.comparison.previousReleaseData.RlsReservationFlag != app.release.comparison.currentReleaseData.RlsReservationFlag) {
            $("#release-comparison-report").find("[name=reservation-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=reservation-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
        }

        //archive
        if (app.release.comparison.previousReleaseData.RlsArchiveFlag != app.release.comparison.currentReleaseData.RlsArchiveFlag) {
            $("#release-comparison-report").find("[name=archive-row]").find("td").addClass(app.config.entity.release.comparison.differenceClass);
        }
        else {
            $("#release-comparison-report").find("[name=archive-row]").find("td").removeClass(app.config.entity.release.comparison.differenceClass);
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
        app.config.url.api.private,
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
 * Call function to draw datatable with response
 * @param  {} response
 */
app.release.comparison.callback.readAmendment = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) { // null is for datatable usually  
        app.release.comparison.callback.readComparison(response, "#comparison-datatable-amendment");
        app.release.comparison.ajax.readDeletion();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

};
//#endregion

//#region deletion

/**
 * Call read deletion
 */
app.release.comparison.ajax.readDeletion = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
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
 * Call function to draw datatable with response
 * @param  {} response
 */
app.release.comparison.callback.readDeletion = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) { // null is for datatable usually
        app.release.comparison.callback.readComparison(response, "#comparison-datatable-deletion");
        app.release.comparison.ajax.readAddition();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

};
//#endregion

//#region addition

/**
 * Call read addition
 */
app.release.comparison.ajax.readAddition = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
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
 * Call function to draw datatable with response
 * @param  {} response
 */
app.release.comparison.callback.readAddition = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) { // null is for datatable usually
        app.release.comparison.callback.readComparison(response, "#comparison-datatable-addition");
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);


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
 * @param  {} response
 * @param  {} selector
 */
app.release.comparison.callback.readComparison = function (response, selector) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data) {
        // Start the spinner manually because the rendering of the Comparison may take a long time
        api.spinner.start();
        var table = $("#release-comparison-templates").find("[name=comparison-table]").clone();
        $(selector).find("[name=table-wrapper]").html(table);
        if ($.fn.DataTable.isDataTable($(selector).find("[name=comparison-table]"))) {
            $(selector).find("[name=comparison-table]").DataTable().destroy();
            //cannot use redraw as columns are dynamically created depending on the matrix. Have to destroy and re-initiate 
        }
        //Create object form JSONstat.
        var ds = JSONstat(response.data);
        var exportFileName = ds.extension.matrix;
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
                exportFileName = ds.extension.matrix + "." + moment().format(app.config.mask.datetime.file)
                break;
        }
        var jsonTableLabel = ds.toTable({
            type: 'arrobj',
            meta: true,
            status: true,
            unit: true,
            content: "label"
        });
        var jsonTableId = ds.toTable({
            type: 'arrobj',
            meta: true,
            status: true,
            unit: true,
            content: "id"
        });

        //loop through data arrays to figure out which items you need to remove, status == undefined
        var rowsToRemoveLabel = [];
        var rowsToRemoveId = [];
        $.each(jsonTableLabel.data, function (index, element) {
            var status = element.status;
            if (status == "false") {
                rowsToRemoveLabel.push(index);
            }
        });

        $.each(jsonTableId.data, function (index, element) {
            var status = element.status;
            if (status == "false") {
                rowsToRemoveId.push(index);
            }
        });

        //Now remove unwanted items from array
        for (var i = rowsToRemoveLabel.length - 1; i >= 0; i--) {
            jsonTableLabel.data.splice(rowsToRemoveLabel[i], 1);
        }

        for (var i = rowsToRemoveId.length - 1; i >= 0; i--) {
            jsonTableId.data.splice(rowsToRemoveId[i], 1);
        }

        var numDimensions = ds.length;
        var tableColumns = [];


        for (i = 0; i < numDimensions; i++) { //build columns
            var codeSpan = $('<span>', {
                "name": "code",
                "class": "badge badge-pill badge-neutral mx-2 d-none",
                "text": ds.id[i]
            }).get(0).outerHTML;

            var tableHeading = $("<th>", {
                "html": ds.Dimension(i).label + codeSpan
            });

            //Data Table header
            $(selector).find("[name=comparison-table] thead tr").append(tableHeading);

            tableColumns.push({
                data: ds.id[i],
                render: function (data, type, row, meta) {
                    var dimensionLabel = jsonTableId.meta.id[meta.col];
                    var codeSpan = $('<span>', {
                        "name": "code",
                        "class": "badge badge-pill badge-neutral mx-2 d-none",
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
            "class": "text-right",
            "render": function (data, type, row, meta) {
                return app.library.utility.formatNumber(data, app.config.separator.decimal.display, app.config.separator.thousand.display, row.unit.decimals);
            }
        });
        var valueHeading = $("<th>",
            {
                "class": "value text-right"
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
        app.config.url.api.private,
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
 * Draw comparison responses
 * @param  {} response
 */
app.release.comparison.callback.readReasonList = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        var data = response.data;
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
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
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