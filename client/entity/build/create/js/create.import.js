/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Bind Parse button
    $("#build-create-import").find("[name=parse-source-file]").once("click", app.build.create.import.validate.ajax.read);
    // Bind Reset button
    $("#build-create-import").find("[name=import-file-reset]").once("click", app.build.create.import.reset);
    // Bind Show event
    $("#build-create-initiate-setup").find("[name=import-show-modal]").once("click", function () {
        $('#build-create-import').modal("show");
    });
    // Bind Close event
    $('#build-create-import').on('hide.bs.modal', app.build.create.import.reset);


});




