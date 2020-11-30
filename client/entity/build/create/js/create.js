/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check(app.config.build.create.moderator ? [C_APP_PRIVILEGE_MODERATOR, C_APP_PRIVILEGE_POWER_USER] : [C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["build"]], [app.label.static["create"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["build"] + " - " + app.label.static["create"]);

    api.content.load("#build-create-initiate", "entity/build/create/index.initiate.html");
    api.content.load("#build-create-dimensions", "entity/build/create/index.dimension.html");
    api.content.load("#build-create-map", "entity/build/map/index.html");
    api.content.load("#overlay", "entity/build/create/index.modal.html");
    var uploadThreshold = app.library.utility.formatNumber(Math.ceil(app.config.transfer.threshold.hard / 1024 / 1024)) + " MB";
    // Set the max file-size in the Upload box
    $("#build-create-upload-si").find("[name=upload-file-max-size]").html(uploadThreshold);
    $("#build-create-upload-classification").find("[name=upload-file-max-size]").html(uploadThreshold);
    $("#build-create-upload-periods").find("[name=upload-file-max-size]").html(uploadThreshold);
    $("#build-create-import").find("[name=upload-file-max-size]").html(uploadThreshold);

    // Initiate Drag n Drop plugin
    api.plugin.dragndrop.initiate(document, window);
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});