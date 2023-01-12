/*******************************************************************************
Custom JS application specific
*******************************************************************************/

/**
 * On page load
 */
$(document).ready(function () {
    $("#release-navigation-association-read-modal").find("[name=add-association]").once("click", function () {
        app.release.navigation.associationAdd();
    });

    $('#release-navigation-association-add-modal').on('hide.bs.modal', function (event) {
        $("#release-navigation-association-add-modal").find("[name=sbj-code]").empty();
        $("#release-navigation-association-add-modal").find("[name=prc-code]").empty();
    })
});