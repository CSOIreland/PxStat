/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    //empty modal after close next modal time its ready 
    $('#matrix-chart-modal').on('hide.bs.modal', function (e) {
        $(this).find("[name=dates-line-chart]").empty();
        $(this).find("[name=referrer-column-chart]").empty();
        $(this).find("[name=user-language-column-chart]").empty();
        $(this).find("[name=browser-pie-chart]").empty();
        $(this).find("[name=operating-system-pie-chart]").empty();
        $(this).find("[name=language-pie-chart]").empty();
        $(this).find("[name=format-pie-chart]").empty();
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});