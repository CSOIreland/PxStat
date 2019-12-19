/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    $("#data-search-input").find("[name=search-input]").focus();
    //insert navigation in navigation div
    $("#data-navigation").html(
        $("#data-search-templates").find("[name=navigation]").clone().get(0).outerHTML
    );
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

    //Pass User SELECTED language. //If Matrix in many languages return only Matrix in User SELECTED language.
    app.data.search.ajax.readNav(app.label.language.iso.code);
    //Key up at search-input
    $("#data-search-input").find("[name=search-input]").once("keyup", function (e) {
        e.preventDefault();
        if (e.keyCode == 13) {
            app.data.metadata.ajax.readSearch();
        }
    });
    //Click at Search button and get value of search-input
    $("#data-search-input").find("[name=data-search-input-button]").once("click", function (e) {
        e.preventDefault();
        app.data.metadata.ajax.readSearch();
    });
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
    $("#data-search-input").find("[name=search-input]").attr('placeholder', app.label.static["search-for-data"]);
});