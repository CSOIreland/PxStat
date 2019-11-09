/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Bind Parse button
    $("#build-create-import [name=parse-source-file]").once("click", app.build.create.import.validate.ajax.read);
    // Bind Reset button
    $("#build-create-import [name=import-file-reset]").once("click", app.build.create.import.reset);
    // Bind Close event
    $('#build-create-import').on('hide.bs.modal', app.build.create.import.reset);
});




