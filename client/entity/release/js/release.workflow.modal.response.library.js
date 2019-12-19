/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.release = app.release || {};

app.release.workflow.modal = app.release.workflow.modal || {};

app.release.workflow.modal.response = {};
app.release.workflow.modal.response.modal = {};
app.release.workflow.modal.response.validation = {};
app.release.workflow.modal.response.ajax = {};
app.release.workflow.modal.response.callback = {};
app.release.workflow.modal.response.RqsCode = null;
app.release.workflow.modal.response.RspCode = null;
//#endregion

//#region Request

/**
 * 
 */
app.release.workflow.modal.response.render = function () {
    app.release.workflow.modal.response.ajax.read();
};

/**
 * 
 */
app.release.workflow.modal.response.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Workflow.Workflow_API.Read",
        { "RlsCode": app.release.RlsCode },
        "app.release.workflow.modal.response.callback.read");
};

/**
* 
 * @param {*} response
 */
app.release.workflow.modal.response.callback.read = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    }
    else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {

        api.modal.information(app.label.static["api-ajax-nodata"]);

        app.release.workflow.request.render();
    }
    else if (response.data) {
        response.data = response.data[0];
        // Store for later use
        app.release.workflow.modal.response.RqsCode = response.data.RqsCode;
        switch (response.data.RqsCode) {
            case C_APP_TS_REQUEST_PUBLISH:
                $("#request-workflow-modal-response-publish [name=rqs-value]").html(app.label.datamodel.request[response.data.RqsValue]);

                $("#request-workflow-modal-response-publish [name=wrq-emergency-flag]").html(app.library.html.boolean(response.data.WrqEmergencyFlag, true, true));
                $("#request-workflow-modal-response-publish [name=wrq-datetime]").html(moment(response.data.WrqDatetime).format(app.config.mask.datetime.display));
                $("#request-workflow-modal-response-publish [name=wrq-reservation-flag]").html(app.library.html.boolean(response.data.WrqReservationFlag, true, true));
                $("#request-workflow-modal-response-publish [name=wrq-archive-flag]").html(app.library.html.boolean(response.data.WrqArchiveFlag, true, true));

                $("#request-workflow-modal-response-publish [name=rqs-create-username]").html(app.library.html.link.user(response.data.RqsCcnCreateUsername));
                $("#request-workflow-modal-response-publish [name=rqs-dtg-create-datetime]").html(moment(response.data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display));
                $("#request-workflow-modal-response-publish [name=rqs-cmm-value]").html(app.library.html.parseBbCode(response.data.RqsCmmValue));
                break;
            case C_APP_TS_REQUEST_PROPERTY:
                $("#request-workflow-modal-response-flag [name=rqs-value]").html(app.label.datamodel.request[response.data.RqsValue]);

                $("#request-workflow-modal-response-flag [name=wrq-reservation-flag]").html(app.library.html.boolean(response.data.WrqReservationFlag, true, true));
                $("#request-workflow-modal-response-flag [name=wrq-archive-flag]").html(app.library.html.boolean(response.data.WrqArchiveFlag, true, true));

                $("#request-workflow-modal-response-flag [name=rqs-create-username]").html(app.library.html.link.user(response.data.RqsCcnCreateUsername));
                $("#request-workflow-modal-response-flag [name=rqs-dtg-create-datetime]").html(moment(response.data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display));
                $("#request-workflow-modal-response-flag [name=rqs-cmm-value]").html(app.library.html.parseBbCode(response.data.RqsCmmValue));
                break;
            case C_APP_TS_REQUEST_DELETE:
                $("#request-workflow-modal-response-delete [name=rqs-value]").html(app.label.datamodel.request[response.data.RqsValue]);

                $("#request-workflow-modal-response-delete [name=rqs-create-username]").html(app.library.html.link.user(response.data.RqsCcnCreateUsername));
                $("#request-workflow-modal-response-delete [name=rqs-dtg-create-datetime]").html(moment(response.data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display));
                $("#request-workflow-modal-response-delete [name=rqs-cmm-value]").html(app.library.html.parseBbCode(response.data.RqsCmmValue));
                break;
            case C_APP_TS_REQUEST_ROLLBACK:
                $("#request-workflow-modal-response-rollback [name=rqs-value]").html(app.label.datamodel.request[response.data.RqsValue]);

                $("#request-workflow-modal-response-rollback [name=rqs-create-username]").html(app.library.html.link.user(response.data.RqsCcnCreateUsername));
                $("#request-workflow-modal-response-rollback [name=rqs-dtg-create-datetime]").html(moment(response.data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display));
                $("#request-workflow-modal-response-rollback [name=rqs-cmm-value]").html(app.library.html.parseBbCode(response.data.RqsCmmValue));
                break;
        }
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.release.workflow.modal.response.create = function () {
    switch (app.release.workflow.modal.response.RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            $("#request-workflow-modal-response-publish").modal("show");
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            $("#request-workflow-modal-response-flag").modal("show");
            break;
        case C_APP_TS_REQUEST_DELETE:
            $("#request-workflow-modal-response-delete").modal("show");
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            $("#request-workflow-modal-response-rollback").modal("show");
            break;
    }

    app.release.workflow.modal.response.validation.create();
};

/**
 * 
 */
app.release.workflow.modal.response.validation.create = function () {
    switch (app.release.workflow.modal.response.RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            $("#request-workflow-modal-response-publish form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-response-publish [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.response.ajax.create();
                    $("#request-workflow-modal-response-publish").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            $("#request-workflow-modal-response-flag form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-response-flag [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.response.ajax.create();
                    $("#request-workflow-modal-response-flag").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_DELETE:
            $("#request-workflow-modal-response-delete form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-response-delete [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.response.ajax.create();
                    $("#request-workflow-modal-response-delete").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            $("#request-workflow-modal-response-rollback form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-response-rollback [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.response.ajax.create();
                    $("#request-workflow-modal-response-rollback").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            break;
    }
};

/**
 * 
 */
app.release.workflow.modal.response.ajax.create = function () {
    var tinyMceId = null;
    switch (app.release.workflow.modal.response.RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            tinyMceId = $("#request-workflow-modal-response-publish [name=rsp-cmm-value]").attr("id");
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            tinyMceId = $("#request-workflow-modal-response-flag [name=rsp-cmm-value]").attr("id");
            break;
        case C_APP_TS_REQUEST_DELETE:
            tinyMceId = $("#request-workflow-modal-response-delete [name=rsp-cmm-value]").attr("id");
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            tinyMceId = $("#request-workflow-modal-response-rollback [name=rsp-cmm-value]").attr("id");
            break;
    }

    var CmmValue = tinymce.get(tinyMceId).getContent();
    obj2send = {
        "RlsCode": app.release.RlsCode,
        "RspCode": app.release.workflow.modal.response.RspCode,
        "CmmValue": CmmValue,
    };

    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Workflow.WorkflowResponse_API.Create",
        obj2send,
        "app.release.workflow.modal.response.callback.create",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} response
 */
app.release.workflow.modal.response.callback.create = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    }
    else if (response.data == C_APP_API_SUCCESS) {
        var goToParams = {
            "RlsCode": app.release.RlsCode,
            "MtrCode": app.release.MtrCode
        };
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [""]));
        $("#modal-success").one('hidden.bs.modal', function (e) {
            // Force page reload
            api.content.goTo("entity/release/", "#nav-link-release", "#nav-link-release", goToParams);
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion