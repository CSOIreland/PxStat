/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["dashboard"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["dashboard"]);
    app.navigation.setState("#nav-link-dashboard");


    // Load Analytics Modal 
    api.content.load("#modal-entity", "entity/analytic/index.modal.html");

    //Load requests Data Table data
    app.dashboard.workInProgress.ajax.read();
    app.dashboard.awaitingResponse.ajax.read();
    app.dashboard.awaitingSignoff.ajax.read();
    app.dashboard.pendinglive.ajax.read();
    app.dashboard.liveReleases.ajax.read();

    // Check access to open relevant Accordion
    app.dashboard.ajax.ReadCurrent();
    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    //Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});