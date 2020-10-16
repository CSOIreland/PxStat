/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.release = app.release || {};

app.release.workflow.modal = app.release.workflow.modal || {};

app.release.workflow.modal.signoff = {};
app.release.workflow.modal.signoff.modal = {};
app.release.workflow.modal.signoff.validation = {};
app.release.workflow.modal.signoff.ajax = {};
app.release.workflow.modal.signoff.callback = {};
app.release.workflow.modal.signoff.RqsCode = null;
app.release.workflow.modal.signoff.SgnCode = null;
//#endregion

//#region Request

/**
 * 
 */
app.release.workflow.modal.signoff.render = function () {
    app.release.workflow.modal.signoff.ajax.read();
};

/**
 * 
 */
app.release.workflow.modal.signoff.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.Read",
        { "RlsCode": app.release.RlsCode },
        "app.release.workflow.modal.signoff.callback.read");
};

/**
* 
 * @param {*} data
 */
app.release.workflow.modal.signoff.callback.read = function (data) {
    if (data) {
        data = data[0];

        // Store for later use
        app.release.workflow.modal.signoff.RqsCode = data.RqsCode;

        switch (data.RqsCode) {
            case C_APP_TS_REQUEST_PUBLISH:
                $("#request-workflow-modal-signoff-publish [name=rqs-value]").html(app.label.datamodel.request[data.RqsValue]);

                $("#request-workflow-modal-signoff-publish [name=wrq-exceptional-flag]").html(app.library.html.boolean(data.WrqExceptionalFlag, true, true));
                $("#request-workflow-modal-signoff-publish [name=wrq-datetime]").html(data.WrqDatetime ? moment(data.WrqDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-publish [name=wrq-reservation-flag]").html(app.library.html.boolean(data.WrqReservationFlag, true, true));
                $("#request-workflow-modal-signoff-publish [name=wrq-archive-flag]").html(app.library.html.boolean(data.WrqArchiveFlag, true, true));

                $("#request-workflow-modal-signoff-publish [name=rqs-create-username]").html(app.library.html.link.user(data.RqsCcnCreateUsername));
                $("#request-workflow-modal-signoff-publish [name=rqs-dtg-create-datetime]").html(data.RqsDtgCreateDatetime ? moment(data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-publish [name=rqs-cmm-value]").html(app.library.html.parseBbCode(data.RqsCmmValue));

                $("#request-workflow-modal-signoff-publish [name=rsp-create-username]").html(app.library.html.link.user(data.RspCcnCreateUsername));
                $("#request-workflow-modal-signoff-publish [name=rsp-dtg-create-datetime]").html(data.RspDtgCreateDatetime ? moment(data.RspDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-publish [name=rsp-cmm-value]").html(app.library.html.parseBbCode(data.RspCmmValue));
                break;
            case C_APP_TS_REQUEST_PROPERTY:
                $("#request-workflow-modal-signoff-flag [name=rqs-value]").html(app.label.datamodel.request[data.RqsValue]);

                $("#request-workflow-modal-signoff-flag [name=wrq-reservation-flag]").html(app.library.html.boolean(data.WrqReservationFlag, true, true));
                $("#request-workflow-modal-signoff-flag [name=wrq-archive-flag]").html(app.library.html.boolean(data.WrqArchiveFlag, true, true));

                $("#request-workflow-modal-signoff-flag [name=rqs-create-username]").html(app.library.html.link.user(data.RqsCcnCreateUsername));
                $("#request-workflow-modal-signoff-flag [name=rqs-dtg-create-datetime]").html(data.RqsDtgCreateDatetime ? moment(data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-flag [name=rqs-cmm-value]").html(app.library.html.parseBbCode(data.RqsCmmValue));

                $("#request-workflow-modal-signoff-flag [name=rsp-create-username]").html(app.library.html.link.user(data.RspCcnCreateUsername));
                $("#request-workflow-modal-signoff-flag [name=rsp-dtg-create-datetime]").html(data.RspDtgCreateDatetime ? moment(data.RspDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-flag [name=rsp-cmm-value]").html(app.library.html.parseBbCode(data.RspCmmValue));
                break;
            case C_APP_TS_REQUEST_DELETE:
                $("#request-workflow-modal-signoff-delete [name=rqs-value]").html(app.label.datamodel.request[data.RqsValue]);

                $("#request-workflow-modal-signoff-delete [name=rqs-create-username]").html(app.library.html.link.user(data.RqsCcnCreateUsername));
                $("#request-workflow-modal-signoff-delete [name=rqs-dtg-create-datetime]").html(data.RqsDtgCreateDatetime ? moment(data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-delete [name=rqs-cmm-value]").html(app.library.html.parseBbCode(data.RqsCmmValue));

                $("#request-workflow-modal-signoff-delete [name=rsp-create-username]").html(app.library.html.link.user(data.RspCcnCreateUsername));
                $("#request-workflow-modal-signoff-delete [name=rsp-dtg-create-datetime]").html(data.RspDtgCreateDatetime ? moment(data.RspDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-delete [name=rsp-cmm-value]").html(app.library.html.parseBbCode(data.RspCmmValue));
                break;
            case C_APP_TS_REQUEST_ROLLBACK:
                $("#request-workflow-modal-signoff-rollback [name=rqs-value]").html(app.label.datamodel.request[data.RqsValue]);

                $("#request-workflow-modal-signoff-rollback [name=rqs-create-username]").html(app.library.html.link.user(data.RqsCcnCreateUsername));
                $("#request-workflow-modal-signoff-rollback [name=rqs-dtg-create-datetime]").html(data.RqsDtgCreateDatetime ? moment(data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-rollback [name=rqs-cmm-value]").html(app.library.html.parseBbCode(data.RqsCmmValue));

                $("#request-workflow-modal-signoff-rollback [name=rsp-create-username]").html(app.library.html.link.user(data.RspCcnCreateUsername));
                $("#request-workflow-modal-signoff-rollback [name=rsp-dtg-create-datetime]").html(data.RspDtgCreateDatetime ? moment(data.RspDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-signoff-rollback [name=rsp-cmm-value]").html(app.library.html.parseBbCode(data.RspCmmValue));
                break;
        }
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

app.release.workflow.modal.signoff.create = function () {
    switch (app.release.workflow.modal.signoff.RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            $("#request-workflow-modal-signoff-publish").modal("show");
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            $("#request-workflow-modal-signoff-flag").modal("show");
            break;
        case C_APP_TS_REQUEST_DELETE:
            $("#request-workflow-modal-signoff-delete").modal("show");
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            $("#request-workflow-modal-signoff-rollback").modal("show");
            break;
    }

    app.release.workflow.modal.signoff.validation.create();
};

/**
 * 
 */
app.release.workflow.modal.signoff.validation.create = function () {
    switch (app.release.workflow.modal.signoff.RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            $("#request-workflow-modal-signoff-publish form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-signoff-publish [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.signoff.ajax.create();
                    $("#request-workflow-modal-signoff-publish").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            $("#request-workflow-modal-signoff-flag form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-signoff-flag [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.signoff.ajax.create();
                    $("#request-workflow-modal-signoff-flag").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_DELETE:
            $("#request-workflow-modal-signoff-delete form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-signoff-delete [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.signoff.ajax.create();
                    $("#request-workflow-modal-signoff-delete").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            $("#request-workflow-modal-signoff-rollback form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-signoff-rollback [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.signoff.ajax.create();
                    $("#request-workflow-modal-signoff-rollback").modal("hide");
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
app.release.workflow.modal.signoff.ajax.create = function () {
    var tinyMceId = null;
    switch (app.release.workflow.modal.signoff.RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            tinyMceId = $("#request-workflow-modal-signoff-publish [name=sgn-cmm-value]").attr("id");
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            tinyMceId = $("#request-workflow-modal-signoff-flag [name=sgn-cmm-value]").attr("id");
            break;
        case C_APP_TS_REQUEST_DELETE:
            tinyMceId = $("#request-workflow-modal-signoff-delete [name=sgn-cmm-value]").attr("id");
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            tinyMceId = $("#request-workflow-modal-signoff-rollback [name=sgn-cmm-value]").attr("id");
            break;
    }

    var CmmValue = tinymce.get(tinyMceId).getContent();
    obj2send = {
        "RlsCode": app.release.RlsCode,
        "SgnCode": app.release.workflow.modal.signoff.SgnCode,
        "CmmValue": CmmValue,
    };

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.WorkflowSignoff_API.Create",
        obj2send,
        "app.release.workflow.modal.signoff.callback.create",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} data
 */
app.release.workflow.modal.signoff.callback.create = function (data) {
    if (data == C_APP_API_SUCCESS) {
        switch (app.release.workflow.modal.signoff.RqsCode) {
            case C_APP_TS_REQUEST_DELETE:
            case C_APP_TS_REQUEST_ROLLBACK:
                // Nowhere to go since the Release has been deleted and perhaps the entire Matrix as well
                var goToParams = {};
                break;
            case C_APP_TS_REQUEST_ROLLBACK:
                // Nowhere to go since the Release has been deleted but a Matrix must still exist
                var goToParams = {
                    "MtrCode": app.release.MtrCode
                };
                break;
            default:
                var goToParams = {
                    "RlsCode": app.release.RlsCode,
                    "MtrCode": app.release.MtrCode
                };
                break;
        }
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [""]));
        $("#modal-success").one('hidden.bs.modal', function (e) {
            // Force page reload
            api.content.goTo("entity/release/", "#nav-link-release", "#nav-link-release", goToParams);
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.release.workflow.modal.signoff.checkNavigation = function () {
    // Check if Navigation is set
    if (app.release.SbjCode && app.release.PrcCode) {
        $("#request-workflow-modal-signoff-publish [name=navigation-warning]").hide();

        // Enable buttons
        $("#request-workflow-modal-signoff-publish [name=button-reject]").prop('disabled', false);
        $("#request-workflow-modal-signoff-publish [name=button-approve]").prop('disabled', false);
    } else {
        $("#request-workflow-modal-signoff-publish [name=navigation-warning]").show();

        // Disable buttons
        $("#request-workflow-modal-signoff-publish [name=button-reject]").prop('disabled', true);
        $("#request-workflow-modal-signoff-publish [name=button-approve]").prop('disabled', true);
    }
};
//#endregion