/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.build.create.initiate.ajax.readLanguage();
    app.build.create.initiate.ajax.readFrequency();
    app.build.create.initiate.ajax.readCopyright();

    $("#build-create-initiate-setup").find("[name=button-matrix-lookup]").on("click", app.build.create.initiate.ajax.matrixLookup);

    if (app.config.dataset.officialStatistics) {
        $("#build-create-initiate-setup [name=official-flag]").prop('checked', true);
    }
    else {
        $("#build-create-initiate-setup [name=official-flag]").prop('checked', false);
    }

    $("#build-create-initiate-setup [name=official-flag]").bootstrapToggle({
        onlabel: app.label.static["true"],
        offlabel: app.label.static["false"],
        onstyle: "success text-light",
        offstyle: "warning text-dark",
        height: 38,
        style: "text-light",
        width: C_APP_TOGGLE_LENGTH
    });

    //Confirm reset and reset details
    $("#build-create-initiate-setup").find("[name=button-clear]").once("click", function () {
        api.modal.confirm(
            app.label.static["reset-page"],
            app.build.create.initiate.clear
        );
    });

    //initiate validation for setup
    app.build.create.initiate.validation.setup();

    $('#build-create-modal-frequency').on('show.bs.modal', function () {
        app.build.create.import.validate.frequencyModal();
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});