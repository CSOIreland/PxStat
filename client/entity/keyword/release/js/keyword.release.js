/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    // Entity with restricted access
    //app.navigation.access.check();
    app.navigation.layout.set(false);
    app.navigation.breadcrumb.set([app.label.static["keywords"], app.label.static["releases"]]);
    // Load HTML for Data Set Modal 
    api.content.load("#dataViewModal #data-dataset-row", "entity/data/index.dataset.html");
    api.content.load("#dataViewModal #data-dataview-row", "entity/data/index.dataview.html");
    // Load the side panel
    api.content.load("#panel", "entity/keyword/search/index.html");

    //Update DropDown Subject
    app.keyword.release.ajax.matrixReadList();
    // Toggle accordion 
    $('[data-toggle="tooltip"]').tooltip();

    $("#keyword-release-modal-create [name=acronym-toggle], #keyword-release-modal-update [name=acronym-toggle]").bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "primary",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH
    });

    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});
