/*******************************************************************************
Header
*******************************************************************************/

$(document).ready(function () {
    // Set Title
    $("#header [name=title]").html(app.config.title);
    // Set Logo
    $("#header [name=logo]").html(app.config.title).attr('alt', app.config.organisation).attr('src', app.config.url.logo);
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});