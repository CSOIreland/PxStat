/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    //insert navigation in navigation div
    var navigation = $("#data-search-templates").find("[name=navigation]").clone();;
    navigation.find("[data-toggle=collapse]").attr("data-target", "#data-browse-collapse");
    navigation.find(".collapse").attr("id", "data-browse-collapse");

    $('#data-search-result-pagination-toggle').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "success",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH
    });

    $("#data-navigation").html(navigation.get(0).outerHTML);
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

    //Pass User SELECTED language. //If Matrix in many languages return only Matrix in User SELECTED language.
    app.data.search.ajax.readNav(app.label.language.iso.code);
    //Key up at search-input
    $("#data-search-row-desktop [name=search-input], #data-search-row-responsive [name=search-input]").once("keyup", function (e) {
        e.preventDefault();
        if (e.keyCode == 13) {
            if ($(this).val().trim().length) {
                app.data.searchResult.ajax.readSearch($(this).val().trim());
            }

        }
    });
    //Click at Search button and get value of search-input
    $("#data-search-row-desktop [name=data-search-input-button], #data-search-row-responsive [name=data-search-input-button]").once("click", function (e) {
        e.preventDefault();
        //both search boxes are always synced so can get serach value from desktop input
        var search = $("#data-search-row-desktop [name=search-input]").val().trim();
        if (search.length) {
            app.data.searchResult.ajax.readSearch(search);
        }
    });
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();

});