/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Bind cancel publish
    $("#release-workflow-request").find("[name=cancel-publish]").once("click", function () {
        app.release.workflow.modal.request.validation.cancelPendingLive();
    });
    // Bind Awaiting Request
    $("#release-workflow-request").find("[name=button-add]").once("click", app.release.workflow.modal.request.ajax.ReadCurrent);
    $("#release-workflow-request").find("[name=rqs-code]").once("change", function () {
        if ($(this).val()) {
            $("#release-workflow-request [name=button-add]").prop("disabled", false);
        } else {
            $("#release-workflow-request [name=button-add]").prop("disabled", true);
        }
    }).trigger("change");

    // Bind Add Response
    $("#release-workflow-response").find("[name=button-add]").once("click", app.release.workflow.modal.response.ajax.ReadCurrent);

    // Bind Add Signoff
    $("#release-workflow-signoff").find("[name=button-add]").once("click", app.release.workflow.modal.signoff.create);
    // Bootstrap tooltip
    $('[data-bs-toggle="tooltip"]').tooltip();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});


