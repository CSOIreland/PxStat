/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    // Bind events
    $("#request-workflow-modal-response-publish").find("[name=button-reject]").once("click", function () {
        app.release.workflow.modal.response.RspCode = C_APP_TS_RESPONSE_REJECTED;
        $("#request-workflow-modal-response-publish form").submit();
    });
    $("#request-workflow-modal-response-publish").find("[name=button-approve]").once("click", function () {
        app.release.workflow.modal.response.RspCode = C_APP_TS_RESPONSE_APPROVED;
        $("#request-workflow-modal-response-publish form").submit();
    });

    // Bind events for Flag
    $("#request-workflow-modal-response-flag").find("[name=button-reject]").once("click", function () {
        app.release.workflow.modal.response.RspCode = C_APP_TS_RESPONSE_REJECTED;
        $("#request-workflow-modal-response-flag form").submit();
    });
    $("#request-workflow-modal-response-flag").find("[name=button-approve]").once("click", function () {
        app.release.workflow.modal.response.RspCode = C_APP_TS_RESPONSE_APPROVED;
        $("#request-workflow-modal-response-flag form").submit();
    });

    // Bind events for Delete
    $("#request-workflow-modal-response-delete").find("[name=button-reject]").once("click", function () {
        app.release.workflow.modal.response.RspCode = C_APP_TS_RESPONSE_REJECTED;
        $("#request-workflow-modal-response-delete form").submit();
    });
    $("#request-workflow-modal-response-delete").find("[name=button-approve]").once("click", function () {
        app.release.workflow.modal.response.RspCode = C_APP_TS_RESPONSE_APPROVED;
        $("#request-workflow-modal-response-delete form").submit();
    });

    // Bind events for Rollback
    $("#request-workflow-modal-response-rollback").find("[name=button-reject]").once("click", function () {
        app.release.workflow.modal.response.RspCode = C_APP_TS_RESPONSE_REJECTED;
        $("#request-workflow-modal-response-rollback form").submit();
    });
    $("#request-workflow-modal-response-rollback").find("[name=button-approve]").once("click", function () {
        app.release.workflow.modal.response.RspCode = C_APP_TS_RESPONSE_APPROVED;
        $("#request-workflow-modal-response-rollback form").submit();
    });
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});