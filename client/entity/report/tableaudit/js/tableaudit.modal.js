/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    //empty modal after close next modal time its ready 
    $('#table-audit-info-modal').on('hide.bs.modal', function (e) {
        $(this).find("[name=table-audit-footnote-info]").empty();
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});