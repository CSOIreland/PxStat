$(document).ready(function () {
    // Entity with restricted access
    app.navigation.access.check([C_APP_PRIVILEGE_MODERATOR, C_APP_PRIVILEGE_POWER_USER]);
    app.navigation.setLayout(false);
    app.navigation.setBreadcrumb([[app.label.static["build"]], [app.label.static["maps"]]]);
    app.navigation.setMetaDescription();
    app.navigation.setTitle(app.label.static["build"] + " - " + app.label.static["maps"]);
    app.navigation.setState("#nav-link-map");


    // Load Modal - must be after GoTo
    api.content.load("#modal-entity", "entity/build/geomap/index.modal.html");

    //remove add button if not administrator
    if (app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
        $("#map-read-container").find("[name=add-map-footer]").remove();
    }
    else {
        $("#map-read-container").find("[name=add-map-footer]").show()
    }

    $("#modal-entity").empty();
    api.content.load("#modal-entity", "entity/build/geomap/index.modal.html", null, true);
    api.content.load("#modal-entity", "entity/build/geomap/index.preview.modal.html", null, true);

    // Load the side panel
    api.content.load("#panel", "entity/build/geomap/index.panel.html");

    //display maps table
    app.geomap.ajax.readLayersSelect();

    $("#map-modal-add").find("[name=upload-file-max-size]").html(app.library.utility.formatNumber(Math.ceil(app.config.transfer.threshold.hard / 1024 / 1024)) + " MB");
    $("#map-modal-create-subset").find("[name=upload-file-max-size]").html(app.library.utility.formatNumber(Math.ceil(app.config.transfer.threshold.hard / 1024 / 1024)) + " MB");
    // Initiate Drag n Drop plugin
    api.plugin.dragndrop.initiate(document, window);

    $("#map-read-container").find("[name=button-add]").once("click", function () {
        app.geomap.validation.addMap();
        app.geomap.ajax.readLanguage();
        app.geomap.ajax.readLayers(
            {
                "idn": null,
                "callback": "app.geomap.callback.readLayersAdd"
            }
        );



        $("#map-modal-add").modal("show");

        //need to place this here as the range slider is in the <form> and the trigger reset in the form validation affects the change event
        //must come after app.geomap.validation.addMap()
        $("#map-modal-add").find("[name=simplify-range]").once("change", function () {
            app.geomap.viewAddMap($(this).val());
        });
    });


    $("#map-modal-add").find("[name=import]").once("click", app.geomap.setDetails);
    $("#map-modal-create-subset").find("[name=upload-submit-subset]").once("click", app.geomap.modal.createSubset);
    $("#map-modal-add").find("[name=set-properties]").once("click", app.geomap.setProperties);
    $("#map-modal-add").find("[name=create-subset]").once("click", function () {
        $("#map-modal-add").find("button[type=submit]").prop('disabled', true);
        $("#map-modal-add").find("[name=view-map-card]").hide();
        $("#map-modal-create-subset").modal("show");
    });

    $("#map-modal-add").find("[name=remove-subset]").once("click", function () {
        $("#map-modal-add").find("[name=view-map-card]").hide();
        api.modal.confirm(app.label.static["confirm-remove-subset"], app.geomap.modal.removeSubset);
    });

    $("#map-modal-add").find("[name=upload-file-reset]").once("click", app.geomap.modal.addMapReset);
    $("#map-modal-create-subset").find("[name=upload-reset-subset]").once("click", app.geomap.modal.createSubsetReset);
    $("#map-modal-add").find("[name=view-map]").once("click", function () {
        //map div has to be shown before any map is rendered
        $('#map-modal-add-preview-map-content').show();
        $("#map-modal-add-preview-properties-tab").removeClass("active");
        $("#map-modal-add-preview-map-tab").addClass("active");
        $("#map-modal-add-preview-properties-content").removeClass("active show");
        $("#map-modal-add-preview-map-content").addClass("active show");

        app.geomap.viewAddMap();
        app.geomap.renderMapProperties();
    });

    $("#map-modal-add").find("[name=simplify-range]").once("input", function () {
        $("#map-modal-add").find("[name=simplify-range-value]").text($(this).val() * 100000000)
    });

    $("#map-modal-add").on('hide.bs.modal', app.geomap.modal.addMapReset);

    $("#map-modal-create-subset").on('hide.bs.modal', app.geomap.modal.createSubsetReset);

    //Initialize TinyMce
    app.plugin.tinyMce.initiate(true);

    $('[data-bs-toggle="tooltip"]').tooltip()
    //run bootstrap toggle to show/hide toggle button
    app.library.bootstrap.getBreakPoint();
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});