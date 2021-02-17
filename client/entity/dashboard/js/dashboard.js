/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["dashboard"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["dashboard"]);

    // Load Analytics Modal 
    api.content.load("#modal-entity", "entity/analytic/index.modal.html");

    //Load requests Data Table data
    app.dashboard.workInProgress.ajax.read();
    app.dashboard.awaitingResponse.ajax.read();
    app.dashboard.awaitingSignoff.ajax.read();
    app.dashboard.pendinglive.ajax.read();
    app.dashboard.liveReleases.ajax.read();

    //Changing plus to minus
    $("#dashboard-accordion").on('show.bs.collapse', function (e) {
        $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-minus-circle");
    });
    //Changing minus to plus
    $("#dashboard-accordion").on('hide.bs.collapse', function (e) {
        $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-plus-circle");
    });

    // Check access to open relevant Accordion
    app.dashboard.ajax.ReadCurrent();
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    //Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});