/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    //empty modal after close next modal time its ready 
    $('#matrix-chart-modal').on('hide.bs.modal', function (e) {
        $(this).find("[name=summary-card]").find("[name=analytic-sum-bots]").empty();
        $(this).find("[name=summary-card]").find("[name=analytic-sum-m2m]").empty();
        $(this).find("[name=summary-card]").find("[name=analytic-sum-widgets]").empty();
        $(this).find("[name=summary-card]").find("[name=analytic-sum-users]").empty();
        $(this).find("[name=summary-card]").find("[name=analytic-sum-totals]").empty();
        $(this).find("[name=dates-line-chart]").empty();
        $(this).find("[name=referrer-column-chart]").empty();
        $(this).find("[name=user-language-column-chart]").empty();
        $(this).find("[name=browser-pie-chart]").empty();
        $(this).find("[name=operating-system-pie-chart]").empty();
        $(this).find("[name=language-pie-chart]").empty();
        $(this).find("[name=format-pie-chart]").empty();
    });

    $('#matrix-chart-modal').find('[name="submit"]').on('click', function () {
        app.analytic.drawCallback.drawModalResults();
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});