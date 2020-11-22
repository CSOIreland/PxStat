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
app.release.workflow.modal.response.fastrackSignoff = false;
//#endregion

//#region Request

app.release.workflow.modal.response.ajax.ReadCurrentAccess = function () {
    //Check the privilege of the user 
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Account_API.ReadCurrentAccess",
        { CcnUsername: null },
        "app.release.workflow.modal.response.callback.ReadCurrentAccess",
        null,
        null,
        null,
        { async: false }
    );
};

app.release.workflow.modal.response.callback.ReadCurrentAccess = function (data) {
    //set to safest workflow
    app.release.workflow.modal.response.fastrackSignoff = false;
    switch (data[0].PrvCode) {
        case C_APP_PRIVILEGE_MODERATOR:
            //do nothing, moderator cannot fastrackSignoff
            break;
        case C_APP_PRIVILEGE_POWER_USER:

            if (app.config.workflow.fastrack.signoff.poweruser) {
                app.release.workflow.modal.response.fastrackSignoff = true;
            }

            break;
        case C_APP_PRIVILEGE_ADMINISTRATOR:
            if (app.config.workflow.fastrack.signoff.administrator) {
                app.release.workflow.modal.response.fastrackSignoff = true;
            }

            break;
        default:
            app.release.workflow.modal.response.fastrackSignoff = false;
            break;
    }

    app.release.workflow.modal.response.create();
};

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
        app.config.url.api.jsonrpc.private,
        "PxStat.Workflow.Workflow_API.Read",
        { "RlsCode": app.release.RlsCode },
        "app.release.workflow.modal.response.callback.read");
};

/**
* 
 * @param {*} data
 */
app.release.workflow.modal.response.callback.read = function (data) {
    if (data && Array.isArray(data) && data.length) {
        data = data[0];
        // Store for later use
        app.release.workflow.modal.response.RqsCode = data.RqsCode;
        switch (data.RqsCode) {
            case C_APP_TS_REQUEST_PUBLISH:
                $("#request-workflow-modal-response-publish [name=rqs-value]").html(app.label.datamodel.request[data.RqsValue]);

                $("#request-workflow-modal-response-publish [name=wrq-exceptional-flag]").html(app.library.html.boolean(data.WrqExceptionalFlag, true, true));
                $("#request-workflow-modal-response-publish [name=wrq-datetime]").html(data.WrqDatetime ? moment(data.WrqDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-response-publish [name=wrq-reservation-flag]").html(app.library.html.boolean(data.WrqReservationFlag, true, true));
                $("#request-workflow-modal-response-publish [name=wrq-experimental-flag]").html(app.library.html.boolean(data.WrqExperimentalFlag, true, true));
                $("#request-workflow-modal-response-publish [name=wrq-archive-flag]").html(app.library.html.boolean(data.WrqArchiveFlag, true, true));

                $("#request-workflow-modal-response-publish [name=rqs-create-username]").html(app.library.html.link.user(data.RqsCcnCreateUsername));
                $("#request-workflow-modal-response-publish [name=rqs-dtg-create-datetime]").html(data.RqsDtgCreateDatetime ? moment(data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-response-publish [name=rqs-cmm-value]").html(app.library.html.parseBbCode(data.RqsCmmValue));
                break;
            case C_APP_TS_REQUEST_PROPERTY:
                $("#request-workflow-modal-response-flag [name=rqs-value]").html(app.label.datamodel.request[data.RqsValue]);

                $("#request-workflow-modal-response-flag [name=wrq-reservation-flag]").html(app.library.html.boolean(data.WrqReservationFlag, true, true));
                $("#request-workflow-modal-response-flag [name=wrq-experimental-flag]").html(app.library.html.boolean(data.WrqExperimentalFlag, true, true));
                $("#request-workflow-modal-response-flag [name=wrq-archive-flag]").html(app.library.html.boolean(data.WrqArchiveFlag, true, true));

                $("#request-workflow-modal-response-flag [name=rqs-create-username]").html(app.library.html.link.user(data.RqsCcnCreateUsername));
                $("#request-workflow-modal-response-flag [name=rqs-dtg-create-datetime]").html(data.RqsDtgCreateDatetime ? moment(data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-response-flag [name=rqs-cmm-value]").html(app.library.html.parseBbCode(data.RqsCmmValue));
                break;
            case C_APP_TS_REQUEST_DELETE:
                $("#request-workflow-modal-response-delete [name=rqs-value]").html(app.label.datamodel.request[data.RqsValue]);

                $("#request-workflow-modal-response-delete [name=rqs-create-username]").html(app.library.html.link.user(data.RqsCcnCreateUsername));
                $("#request-workflow-modal-response-delete [name=rqs-dtg-create-datetime]").html(data.RqsDtgCreateDatetime ? moment(data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-response-delete [name=rqs-cmm-value]").html(app.library.html.parseBbCode(data.RqsCmmValue));
                break;
            case C_APP_TS_REQUEST_ROLLBACK:
                $("#request-workflow-modal-response-rollback [name=rqs-value]").html(app.label.datamodel.request[data.RqsValue]);

                $("#request-workflow-modal-response-rollback [name=rqs-create-username]").html(app.library.html.link.user(data.RqsCcnCreateUsername));
                $("#request-workflow-modal-response-rollback [name=rqs-dtg-create-datetime]").html(data.RqsDtgCreateDatetime ? moment(data.RqsDtgCreateDatetime).format(app.config.mask.datetime.display) : "");
                $("#request-workflow-modal-response-rollback [name=rqs-cmm-value]").html(app.library.html.parseBbCode(data.RqsCmmValue));
                break;
        }
    }
    // Handle no data
    else {
        api.modal.information(app.label.static["api-ajax-nodata"]);
        app.release.workflow.request.render();
    }
};

/**
 * 
 */
app.release.workflow.modal.response.create = function () {
    switch (app.release.workflow.modal.response.RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            if (app.release.workflow.modal.response.fastrackSignoff) {
                $("#request-workflow-modal-response-publish [name=auto-signoff-warning]").show();
            }

            //check navigation if auto signoff
            if ((!app.release.SbjCode || !app.release.PrcCode) && app.release.workflow.modal.response.fastrackSignoff) {
                $("#request-workflow-modal-response-publish [name=navigation-warning]").show();
                $("#request-workflow-modal-response-publish [name=button-approve]").prop('disabled', true);
            }

            $("#request-workflow-modal-response-publish").modal("show").on('hide.bs.modal', function (e) { //clean up
                $("#request-workflow-modal-response-publish [name=auto-signoff-warning]").hide();
                $("#request-workflow-modal-response-publish [name=navigation-warning]").hide();
                $("#request-workflow-modal-response-publish [name=button-approve]").prop('disabled', false);

            });
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            if (app.release.workflow.modal.response.fastrackSignoff) {
                $("#request-workflow-modal-response-flag [name=auto-signoff-warning]").show();
            }
            $("#request-workflow-modal-response-flag").modal("show").on('hide.bs.modal', function (e) { //clean up
                $("#request-workflow-modal-response-flag [name=auto-signoff-warning]").hide();
                $("#request-workflow-modal-response-flag [name=update-wip-properties-warning]").hide();
            });

            //if updating properties and there is a WIP, warn user that this will be updated in WIP also
            if (app.release.liveHasWorkInProgress) {
                $("#request-workflow-modal-response-flag [name=update-wip-properties-warning]").show();
            }
            break;
        case C_APP_TS_REQUEST_DELETE:
            if (app.release.workflow.modal.response.fastrackSignoff) {
                $("#request-workflow-modal-response-delete [name=auto-signoff-warning]").show();
            }
            $("#request-workflow-modal-response-delete").modal("show").on('hide.bs.modal', function (e) { //clean up
                $("#request-workflow-modal-response-delete [name=auto-signoff-warning]").hide();
            });
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            if (app.release.workflow.modal.response.fastrackSignoff) {
                $("#request-workflow-modal-response-rollback [name=auto-signoff-warning]").show();
            }
            $("#request-workflow-modal-response-rollback").modal("show").on('hide.bs.modal', function (e) { //clean up
                $("#request-workflow-modal-response-rollback [name=auto-signoff-warning]").hide();
            });
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
        app.config.url.api.jsonrpc.private,
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
 * @param {*} data
 */
app.release.workflow.modal.response.callback.create = function (data) {
    if (data == C_APP_API_SUCCESS) {
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