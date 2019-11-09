/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.alert.notice.ajax.read();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});