/*******************************************************************************
Custom JS application specific group.js
*******************************************************************************/
// HTML loaded
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["groups"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["groups"]);

    // GoTo
    var GroupCode = api.content.getParam("GrpCode");

    // Load Modal - must be after GoTo
    api.content.load("#overlay", "entity/manage/group/index.modal.html");

    // Get data from API for Group DataTable
    app.group.ajax.read();
    if (GroupCode) {
        app.group.ajax.readGroup(GroupCode);
    } else {
        $("#accordion-group").hide();
    }
    //initiate toggle buttons
    $('#group-modal-add-member [name=group-input-add-member-approve-flag]').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "success",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH //Depend on language translation.
    });
    //initiate toggle buttons
    $('#group-modal-update-group-member [name=gcc-approve-flag]').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "success",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH //Depend on language translation.
    });
    // Bootstrap tooltip
    $('[data-toggle="tooltip"]').tooltip();

    //Adding placeholders from config file for phone number
    $("#group-modal-create-group").find("[name = grp-contact-phone]").attr("placeholder", app.config.regex.phone.placeholder);
    $("#group-form-update-group").find("[name = grp-contact-phone]").attr("placeholder", app.config.regex.phone.placeholder);
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
}); 