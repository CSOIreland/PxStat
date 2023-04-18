/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    $("#release-source").find("[name=compare-release]").once("click", function () {
        //first check that the language is available in the previous release
        if ($.inArray($("#release-source [name=lng-iso-code] option:selected").val(), app.release.LngIsoCodePrevious) != -1) {
            if (app.release.fileContent.length > app.config.entity.release.comparison.threshold.soft) {
                api.modal.confirm(app.label.static["confirm-comparison"], app.release.comparison.render);
            }
            else {
                // Run comparison
                app.release.comparison.render();
            }
        }
        else {
            api.modal.information(app.library.html.parseDynamicLabel("comparison-language-not-available", [$("#release-source [name=lng-iso-code] option:selected").text()]));
        }

    });

    $("#release-comparison-modal").on('show.bs.modal', function (e) {
        $("#comparison-amendment-toggle").bootstrapToggle("destroy").bootstrapToggle({
            on: app.label.static["false"],
            off: app.label.static["true"],
            onstyle: "primary",
            offstyle: "neutral",
            width: C_APP_TOGGLE_LENGTH
        });

        $("#comparison-amendment-toggle").bootstrapToggle('on');

        $("#comparison-addition-toggle").bootstrapToggle("destroy").bootstrapToggle({
            on: app.label.static["false"],
            off: app.label.static["true"],
            onstyle: "primary",
            offstyle: "neutral",
            width: C_APP_TOGGLE_LENGTH
        });

        $("#comparison-addition-toggle").bootstrapToggle('on');

        $("#comparison-deletion-toggle").bootstrapToggle("destroy").bootstrapToggle({
            on: app.label.static["false"],
            off: app.label.static["true"],
            onstyle: "primary",
            offstyle: "neutral",
            width: C_APP_TOGGLE_LENGTH
        });

        $("#comparison-deletion-toggle").bootstrapToggle('on');

    })

    $("#release-comparison-modal").on('hide.bs.modal', function (e) {
        app.release.comparison.previousReleaseData = null;
        app.release.comparison.previousMatrixData = null;
        app.release.comparison.currentReleaseData = null;
        app.release.comparison.currentMatrixData = null;
        //remove all difference styling on closing of modal
        $(this).find("." + app.config.entity.release.comparison.differenceClass).removeClass(app.config.entity.release.comparison.differenceClass);
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});