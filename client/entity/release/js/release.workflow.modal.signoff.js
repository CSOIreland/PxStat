/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    // Bind events for Publish
    $("#request-workflow-modal-signoff-publish").find("[name=button-reject]").once("click", function () {
        app.release.workflow.modal.signoff.SgnCode = C_APP_TS_SIGNOFF_REJECTED;
        $("#request-workflow-modal-signoff-publish form").submit();
    });
    $("#request-workflow-modal-signoff-publish").find("[name=button-approve]").once("click", function () {
        app.release.workflow.modal.signoff.SgnCode = C_APP_TS_SIGNOFF_APPROVED;
        $("#request-workflow-modal-signoff-publish form").submit();
    });

    // Bind events for Flag
    $("#request-workflow-modal-signoff-flag").find("[name=button-reject]").once("click", function () {
        app.release.workflow.modal.signoff.SgnCode = C_APP_TS_SIGNOFF_REJECTED;
        $("#request-workflow-modal-signoff-flag form").submit();
    });
    $("#request-workflow-modal-signoff-flag").find("[name=button-approve]").once("click", function () {
        app.release.workflow.modal.signoff.SgnCode = C_APP_TS_SIGNOFF_APPROVED;
        $("#request-workflow-modal-signoff-flag form").submit();
    });

    // Bind events for Delete
    $("#request-workflow-modal-signoff-delete").find("[name=button-reject]").once("click", function () {
        app.release.workflow.modal.signoff.SgnCode = C_APP_TS_SIGNOFF_REJECTED;
        $("#request-workflow-modal-signoff-delete form").submit();
    });
    $("#request-workflow-modal-signoff-delete").find("[name=button-approve]").once("click", function () {
        app.release.workflow.modal.signoff.SgnCode = C_APP_TS_SIGNOFF_APPROVED;
        $("#request-workflow-modal-signoff-delete form").submit();
    });

    // Bind events for Rollback
    $("#request-workflow-modal-signoff-rollback").find("[name=button-reject]").once("click", function () {
        app.release.workflow.modal.signoff.SgnCode = C_APP_TS_SIGNOFF_REJECTED;
        $("#request-workflow-modal-signoff-rollback form").submit();
    });
    $("#request-workflow-modal-signoff-rollback").find("[name=button-approve]").once("click", function () {
        app.release.workflow.modal.signoff.SgnCode = C_APP_TS_SIGNOFF_APPROVED;
        $("#request-workflow-modal-signoff-rollback form").submit();
    });
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});