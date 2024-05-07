/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Bootstrap tooltip
    $('[data-bs-toggle="tooltip"]').tooltip();

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});