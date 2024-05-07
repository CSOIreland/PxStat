/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    // Entity with restricted access
    //app.navigation.access.check();
    app.navigation.setLayout(false);

    app.navigation.setBreadcrumb([[app.label.static["keywords"]], [app.label.static["releases"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["keywords"] + " - " + app.label.static["releases"]);
    app.navigation.setState("#nav-link-keyword-release");



    // Load Modal
    api.content.load("#modal-entity", "entity/keyword/release/index.modal.html");

    // Load HTML for Data Set Modal 
    api.content.load("#modal-entity", "entity/data/index.modal.html", null, true);
    api.content.load("#data-dataset-row", "entity/data/index.dataset.html");

    // Load the side panel
    api.content.load("#panel", "entity/keyword/search/index.html");

    //Update DropDown Subject
    app.keyword.release.ajax.matrixReadList();
    // Toggle accordion 
    $('[data-bs-toggle="tooltip"]').tooltip();

    $("#keyword-release-modal-create [name=acronym-toggle], #keyword-release-modal-update [name=acronym-toggle]").bootstrapToggle("destroy").bootstrapToggle({
        onlabel: app.label.static["true"],
        offlabel: app.label.static["false"],
        onstyle: "success text-light",
        offstyle: "warning text-dark",
        height: 38,
        style: "text-light",
        width: C_APP_TOGGLE_LENGTH
    });

    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});
