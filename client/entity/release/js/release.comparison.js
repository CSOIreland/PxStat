/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

    $("#release-source").find("[name=compare-release]").once("click", function () {

        if (app.release.fileContent.length > app.config.entity.release.comparison.threshold.soft) {
            api.modal.confirm(app.label.static["confirm-comparison"], app.release.comparison.render);
        }
        else {
            // Run comparison
            app.release.comparison.render();
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













    //empty dynamic data from modal when you close it
    /* $('#release-comparison-modal').on('hide.bs.modal', function (e) {
        $("[empty=true]").each(function () {
            $(this).empty();
        });

        if ($.fn.DataTable.isDataTable($("#comparison-datatable-amendment").find("table"))) {
            $("#comparison-datatable-amendment").find("table").DataTable().destroy();
        }

        if ($.fn.DataTable.isDataTable($("#comparison-datatable-addition").find("table"))) {
            $("#comparison-datatable-comparison-datatable-addition").find("table").DataTable().destroy();
        }

        if ($.fn.DataTable.isDataTable($("#comparison-datatable-deletion").find("table"))) {
            $("#comparison-datatable-deletion").find("table").DataTable().destroy();
        }
    }) */
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});