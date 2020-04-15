/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    // Bind events
    $("#request-workflow-modal-request-publish").find("[name=wrq-exceptional-flag]").once("change", app.release.workflow.modal.request.checkDatetime);
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});
