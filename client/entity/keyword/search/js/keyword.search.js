/*******************************************************************************
Custom JS application specific
*******************************************************************************/
// On page load
$(document).ready(function () {

    //Key up at search-input
    $("#keyword-search-synonym-request").find("[name=sym-value]").once("keyup", function (e) {
        e.preventDefault();
        if (e.keyCode == 13) {
            app.keyword.search.ajax.searchSynonym();
        }
    });

    //Get the Synonyns for item requested
    $("#keyword-search-synonym-request").find("[name=button-synonym-search]").once('click', function (e) {
        e.preventDefault();
        app.keyword.search.ajax.searchSynonym();
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();

});