/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER, C_APP_PRIVILEGE_MODERATOR]);
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["reports"]], [app.label.static["table-audit"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["reports"] + " - " + app.label.static["table-audit"]);
    app.navigation.setState("#nav-link-report-table-audit");

    app.tableaudit.setDatePicker();
    app.tableaudit.ajax.readReason();

    //app.tableaudit.validation.select();

    // Load Modal
    api.content.load("#modal-entity", "entity/report/tableaudit/index.modal.html");

    //cancel click
    $("#select-card").find("[name=button-cancel]").once("click", function () {
        $('html, body').animate({ scrollTop: $('#select-card').offset().top }, 1000);
        api.content.load("#body", "entity/report/tableaudit/")
    });

    $("#select-card").find("[name=button-submit]").once("click", app.tableaudit.ajax.readTableAudit);
    $("#table-audit-results").find("[name=csv]").once("click", app.tableaudit.callback.downloadResults);

    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});