/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_MODERATOR, C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.layout.set(false);
    app.navigation.breadcrumb.set([app.label.static["build"], app.label.static["create"]]);
    app.navigation.breadcrumb.set([app.label.static.build, app.label.static["create"]]);

    api.content.load("#build-create-initiate", "entity/build/create/index.initiate.html");
    api.content.load("#build-create-dimensions", "entity/build/create/index.dimension.html");
    api.content.load("#build-create-map", "entity/build/map/index.html");

    // Set the max file-size in the Upload box
    $("[name=upload-file-max-size]").html(app.library.utility.formatNumber(Math.ceil(app.config.upload.threshold.hard / 1024 / 1024)) + " MB").show();
    // Initiate Drag n Drop plugin
    api.plugin.dragndrop.initiate(document, window);
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});