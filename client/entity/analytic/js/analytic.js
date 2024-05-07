/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["analytics"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["analytics"]);
    app.navigation.setState("#nav-link-analytic");


    // Load Modal 
    api.content.load("#modal-entity", "entity/analytic/index.modal.html");

    app.analytic.setDatePicker();
    app.analytic.ajax.readSubject();
    app.analytic.validation.select();
    //cancel click
    $("#analytic-select-card").find("[name=button-cancel]").once("click", function (e) {
        e.preventDefault();
        $("#analytic-select-card").find(".error").empty();
        $("#analytic-results").hide();
        //clear subject dropdown
        $("#analytic-select-card").find("[name=select-subject]").val(null).trigger("change");
        $("#analytic-select-card").find("[name=select-product]").empty();
        // Disable product 
        $("#analytic-select-card").find("[name=select-product]").prop('disabled', true);
        // Clear ip address
        $("#analytic-select-card").find("[name=nlt-masked-ip]").val("");
        // Reset date picker
        app.analytic.dateFrom = moment().subtract(app.config.entity.analytic.dateRangePicker, 'days');
        app.analytic.dateTo = moment().subtract(1, 'days');
        app.analytic.setDatePicker();
    });

    $("#analytic-data").find("[name=csv]").once("click", app.analytic.callback.downloadResults);

    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});