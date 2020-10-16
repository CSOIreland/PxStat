/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.release = app.release || {};

app.release.workflow = {};

app.release.workflow.request = {};
app.release.workflow.request.ajax = {};
app.release.workflow.request.callback = {};

app.release.workflow.response = {};
app.release.workflow.response.ajax = {};
app.release.workflow.response.callback = {};

app.release.workflow.signoff = {};
app.release.workflow.signoff.ajax = {};
app.release.workflow.signoff.callback = {};
//#endregion

//#region Request

/**
 * 
 */
app.release.workflow.request.render = function () {
    // Load the allowed Request types
    app.release.workflow.request.ajax.readType();
    $("#release-workflow-request").hide().fadeIn();
    $("#release-workflow-response").hide();
    $("#release-workflow-signoff").hide();
};

/**
 * 
 */
app.release.workflow.request.ajax.readType = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Request_API.Read",
        { "RlsCode": app.release.RlsCode },
        "app.release.workflow.request.callback.readType",
        null,
        null,
        null,
        { async: false });
};

/**
 * 
 * @param {*} data
 */
app.release.workflow.request.callback.readType = function (data) {
    $("#release-workflow-request [name=rqs-code]").empty();
    // Add default option
    var defaultOption = $("<option>", {
        "value": "",
        "text": app.label.static["select-uppercase"],
    });
    $("#release-workflow-request [name=rqs-code]").append(defaultOption);
    $.each(data, function (i, row) {
        var option = $("<option>", {
            "value": row.RqsCode,
            "text": app.label.datamodel.request[row.RqsValue]
        });
        $("#release-workflow-request [name=rqs-code]").append(option);
    });
};
//#endregion

//#region Awaiting Response 

/**
 * 
 */
app.release.workflow.read = function () {
    // Check first if it's Awaiting Response
    if (app.release.isAwaitingResponse) {
        // Toggle Reason buttons
        app.release.reason.toggle();
        app.release.workflow.response.render();
        app.release.workflow.modal.response.render();
    }
    else if (app.release.isAwaitingSignOff) {
        // Toggle Reason buttons
        app.release.reason.toggle();
        app.release.workflow.signoff.render();
        app.release.workflow.modal.signoff.render();
    }

    else {
        // Toggle Reason buttons
        app.release.reason.toggle();
        // If it is no Awaiting Signoff, then it is Awaiting Request
        app.release.workflow.request.render();
    }



    //app.release.workflow.response.ajax.read();
};

/**
 * 
 */
app.release.workflow.response.render = function () {
    $("#release-workflow-request").hide();
    $("#release-workflow-response").hide();
    $("#release-workflow-signoff").hide();
    if (!app.release.isModerator || app.release.isApprover) {
        $("#release-workflow-response").fadeIn();
    }
};

//#endregion

//#region Awaiting Signoff 

/**
 * 
 */
app.release.workflow.signoff.render = function () {
    $("#release-workflow-request").hide();
    $("#release-workflow-response").hide();
    $("#release-workflow-signoff").hide();
    if (!app.release.isModerator) {
        $("#release-workflow-signoff").fadeIn();
    }
};

//#endregion