/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    // Read Reason Codes
    app.release.reason.readCodeList();
    // Add Reason
    $("#release-reason").find("[name=add-reason]").once("click", app.release.reason.create);
    // Change Reason Code
    $("#release-reason-modal-create [name=rsn-code]").bind("change", app.release.reason.changeCode);

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();

});

