/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Load the Chart module
    var chartModule = document.createElement('script');
    chartModule.src = app.config.plugin.highcharts.enabled ? C_APP_ANALYTIC_MODULE_HIGHCHARTS : C_APP_ANALYTIC_MODULE_DEFAULT;
    $("#body").append(chartModule);

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});