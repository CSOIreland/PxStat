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
    //Load requests Data Table data
    app.release.panel.pendingLive.ajax.read();
    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});

