/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    //get formats for create dropdown
    app.build.create.dimension.ajax.readFormat();

    //On click add the statistic
    $("#build-create-statistic").find("[name=manual-submit-statistics]").click(function () {
        var LngIsoCode = $(this).attr("lng-iso-code");
        app.build.create.dimension.submitManualStatistic(LngIsoCode);
    });
    //On click upload statistics
    $("#build-create-statistic").find("[name=upload-submit-statistics]").click(function (e) {
        e.preventDefault();
        var LngIsoCode = $(this).attr("lng-iso-code");
        app.build.create.dimension.submitUploadStatistic(LngIsoCode);
    });
    //Add periods manually
    $("#build-create-new-periods").find("[name=manual-submit-periods]").once("click", app.build.create.dimension.addPeriodsManual);
    //Add periods upload
    $("#build-create-new-periods").find("[name=upload-submit-periods]").once("click", app.build.create.dimension.addPeriodsUpload);

    //Search for a classification
    app.build.create.dimension.searchClassifications();


    //clean up modals after closing
    $('#build-create-search-classiication').on('hidden.bs.modal', function (e) {
        $("#build-create-search-classiication").find("[name=search-classifications-list-table-container]").hide();
        $("#build-create-search-classiication").find("[name=read-classification-table-container]").hide();
        $("#build-create-search-classiication").find("[name=classifications-search-input]").val("");
        $("#build-create-manual-classification").find("[name=manual-classification-errors]").empty();
    });
    $('#build-create-statistic').on('hidden.bs.modal', function (e) {
        $(this).find("[name=build-create-upload-statistic-file]").val("");
        $(this).find("[name=upload-file-name]").empty().hide();
        $(this).find("[name=upload-file-tip]").show();
        $(this).find("[name=upload-submit-statistics]").prop("disabled", true);
        $(this).find("[name=upload-si-errors]").empty();
        $('#build-create-upload-si').find("[name=upload-si-errors-card]").hide();
        $('#build-create-manual-si').find("[name=manual-si-errors-card]").hide();
    });
    $('#build-create-classification').on('hidden.bs.modal', function (e) {
        $(this).find("[name=build-create-upload-classification-file]").val("");
        $(this).find("[name=upload-file-name]").empty().hide();
        $(this).find("[name=upload-file-tip]").show();
        $(this).find("[name=upload-submit-classifications]").prop("disabled", true);
        $('#build-create-manual-classification').find("[name=manual-classification-errors-card]").hide();
        $('#build-create-upload-classification').find("[name=upload-classification-errors-card]").hide();
    });
    $('#build-create-new-periods').on('hidden.bs.modal', function (e) {
        $(this).find("[name=build-create-upload-periods-file]").val("");
        $(this).find("[name=upload-file-name]").empty().hide();
        $(this).find("[name=upload-file-tip]").show();
        $(this).find("[name=upload-submit-periods]").prop("disabled", true);
        $(this).find("[name=manual-periods-errors-card]").hide();
        $(this).find("[name=upload-periods-errors-card]").hide();
    });

    //set up modals correctly when opened
    $('#build-create-classification').on('show.bs.modal', function (e) {
        if (app.config.entity.build.geoJsonLookup.enabled) {
            $(this).find("[name=geojson-lookup]").each(function () {
                $(this).removeClass("d-none").attr("href", app.config.entity.build.geoJsonLookup.href);
            })
        }
        $("#build-create-manual-classification").addClass("show active");
        $("#build-create-upload-classification").removeClass("show active");
        $(this).find("[name=manual-classification-tab]").addClass("active show").attr("aria-selected", "true");
        $(this).find("[name=upload-classification-tab]").removeClass("active show").attr("aria-selected", "false");
    });

    $('#build-create-statistic').on('show.bs.modal', function (e) {
        $("#build-create-manual-si").addClass("show active");
        $("#build-create-upload-si").removeClass("show active");
        $(this).find("[name=manual-si-tab]").addClass("active show").attr("aria-selected", "true");
        $(this).find("[name=upload-si-tab]").removeClass("active show").attr("aria-selected", "false");
    });

    $('#build-create-new-periods').on('show.bs.modal', function (e) {
        $("#build-create-manual-periods").addClass("show active");
        $("#build-create-upload-periods").removeClass("show active");
        $(this).find("[name=manual-periods-tab]").addClass("active show").attr("aria-selected", "true");
        $(this).find("[name=upload-periods-tab]").removeClass("active show").attr("aria-selected", "false");
    });

    $('#build-create-search-classiication').on('shown.bs.modal', function (e) {
        $(this).find("[name=classifications-search-input]").focus();
    });

    //Check if user wants to reset dimensions and clear tabs.
    $("#build-create-matrix-dimensions").find("[name=reset-dimensions]").once("click", function () {
        api.modal.confirm(
            app.label.static["create-reset-dimension"],
            app.build.create.dimension.clearTabs
        );
    });


    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();

});