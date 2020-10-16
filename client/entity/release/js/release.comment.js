/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Bind events
    $("#release-comment").find("[name=create]").once("click", app.release.comment.create);
    $("#release-comment").find("[name=delete]").once("click", app.release.comment.delete);
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});

