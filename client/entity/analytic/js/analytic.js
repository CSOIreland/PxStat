/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_MODERATOR]);

    app.navigation.layout.set(false);
    app.navigation.breadcrumb.set([app.label.static["analytics"]]);

    app.analytic.setDatePicker();
    app.analytic.ajax.readSubject();
    app.analytic.ajax.readAnalytics();
    app.analytic.validation.select();
    //cancel click
    $("#select-card").find("[name=button-cancel]").once("click", function (e) {
        e.preventDefault();
        $("#select-card").find(".error").empty();
        $("#analytic-results").hide();
        //clear subject dropdown
        $("#select-card").find("[name=select-subject]").val(null).trigger("change");
        $("#select-card").find("[name=select-product]").empty();
        // Disable product 
        $("#select-card").find("[name=select-product]").prop('disabled', true);
        // Clear ip address
        $("#select-card").find("[name=nlt-masked-ip]").val("");
        // Reset date picker
        app.analytic.dateFrom = moment().subtract(app.config.entity.analytic.dateRangePicker, 'days');
        app.analytic.dateTo = moment();
        app.analytic.setDatePicker();
    });

    // Translate placheholder
    $("#select-card").find("[name='nlt-masked-ip']").attr('placeholder', app.label.static["sample-ip-address"]);

    // Load Modal 
    api.content.load("#analytic-modal", "entity/analytic/index.modal.html");
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});