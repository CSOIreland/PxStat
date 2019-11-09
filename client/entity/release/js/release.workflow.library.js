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
        app.config.url.api.private,
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
 * @param {*} response
 */
app.release.workflow.request.callback.readType = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        $("#release-workflow-request [name=rqs-code]").empty();
        // Add default option
        var defaultOption = $("<option>", {
            "value": "",
            "text": app.label.static["select-uppercase"],
        });
        $("#release-workflow-request [name=rqs-code]").append(defaultOption);
        $.each(response.data, function (i, row) {
            var option = $("<option>", {
                "value": row.RqsCode,
                "text": app.label.static[row.RqsValue]
            });
            $("#release-workflow-request [name=rqs-code]").append(option);
        });
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion

//#region Awaiting Response 

/**
 * 
 */
app.release.workflow.read = function () {
    // Check first if it's Awaiting Response
    app.release.workflow.response.ajax.read();
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

/**
 * 
 */
app.release.workflow.response.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingResponse",
        { "RlsCode": app.release.RlsCode },
        "app.release.workflow.response.callback.read",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} response
 */
app.release.workflow.response.callback.read = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        // If it is no Awaiting Response, then check if it is Awaiting Signoff
        app.release.workflow.signoff.ajax.read();
    } else if (response.data) {
        // Store for later use
        app.release.isWorkflowInProgress = true;
        // Toggle Reason buttons
        app.release.reason.toggle();
        app.release.workflow.response.render();
        app.release.workflow.modal.response.render();
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
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

/**
 * 
 */
app.release.workflow.signoff.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.ReadAwaitingSignoff",
        { "RlsCode": app.release.RlsCode },
        "app.release.workflow.signoff.callback.read",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} response
 */
app.release.workflow.signoff.callback.read = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        // Store for later use
        app.release.isWorkflowInProgress = false;
        // Toggle Reason buttons
        app.release.reason.toggle();
        // If it is no Awaiting Signoff, then it is Awaiting Request
        app.release.workflow.request.render();
    } else if (response.data) {
        // Store for later use
        app.release.isWorkflowInProgress = true;
        // Toggle Reason buttons
        app.release.reason.toggle();
        app.release.workflow.signoff.render();
        app.release.workflow.modal.signoff.render();
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion