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
    app.navigation.setState("#nav-link-group");

    // GoTo
    var GroupCode = api.content.getParam("GrpCode");

    // Load Modal - must be after GoTo
    api.content.load("#modal-entity", "entity/manage/group/index.modal.html");

    // Get data from API for Group DataTable
    app.group.ajax.read();
    if (GroupCode) {
        app.group.ajax.readGroup(GroupCode);
    } else {
        $("#group-edit").hide();
    }
    //initiate toggle buttons
    $('#group-modal-add-member [name=group-input-add-member-approve-flag]').bootstrapToggle("destroy").bootstrapToggle({
        onlabel: app.label.static["true"],
        offlabel: app.label.static["false"],
        onstyle: "success text-light",
        offstyle: "warning text-dark",
        height: 38,
        style: "text-light",
        width: C_APP_TOGGLE_LENGTH //Depend on language translation.
    });
    //initiate toggle buttons
    $('#group-modal-update-group-member [name=gcc-approve-flag]').bootstrapToggle("destroy").bootstrapToggle({
        onlabel: app.label.static["true"],
        offlabel: app.label.static["false"],
        onstyle: "success text-light",
        offstyle: "warning text-dark",
        height: 38,
        style: "text-light",
        width: C_APP_TOGGLE_LENGTH //Depend on language translation.
    });
    // Bootstrap tooltip
    $('[data-bs-toggle="tooltip"]').tooltip();

    //Adding placeholders from config file for phone number
    $("#group-modal-create-group").find("[name = grp-contact-phone]").attr("placeholder", app.config.regex.phone.placeholder);
    $("#group-form-update-group").find("[name = grp-contact-phone]").attr("placeholder", app.config.regex.phone.placeholder);
    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
}); 