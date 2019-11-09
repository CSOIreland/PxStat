/*******************************************************************************
Custom JS application specific
*******************************************************************************/
/**
 * On page load
 */
$(document).ready(function () {

    //Load requests Data Table data
    app.release.panel.workInProgress.ajax.read();
    //Load requests Data Table data
    app.release.panel.awaitingResponse.ajax.read();
    //Load requests Data Table data
    app.release.panel.awaitingSignoff.ajax.read();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});

