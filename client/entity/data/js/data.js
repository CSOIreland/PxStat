/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.navigation.layout.set(true);
    app.navigation.breadcrumb.set([]);

    // Read GoTo
    app.data.goTo.Search = api.content.getParam("Search");
    app.data.goTo.PrcCode = api.content.getParam("PrcCode");
    app.data.goTo.CprCode = api.content.getParam("CprCode");
    app.data.goTo.MtrCode = api.content.getParam("MtrCode");

    // Load Modal - must be after GoTo
    api.content.load("#overlay", "entity/data/index.modal.html");

    api.content.load("#data-search-row-desktop", "entity/data/index.search.html");
    //api.content.load("#data-search-result-row", "entity/data/index.search.result.html");
    api.content.load("#data-dataset-row", "entity/data/index.dataset.html");
    api.content.load("#data-share-row", "entity/data/index.share.html");
    // Init DatePicker
    app.data.setDatePicker();

    //set up search inputs
    $("#data-search-row-responsive, #data-search-row-desktop [name=search-input-group-holder]").html(
        $("#data-search-templates").find("[name=search-input-group]").clone().get(0).outerHTML
    );

    $("#data-search-row-desktop [name=search-input], #data-search-row-responsive [name=search-input]").attr('placeholder', app.label.static["search-for-data"]);

    //sync search boxes
    $("#data-search-row-desktop [name=search-input]").on("input", function () {
        $("#data-search-row-responsive [name=search-input]").val(this.value);
    });

    $("#data-search-row-responsive [name=search-input]").on("input", function () {
        $("#data-search-row-desktop [name=search-input]").val(this.value);
    });

    //run bootstrap toggle to show/hide search inputs
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

    //initiate all copy to clipboard 
    new ClipboardJS("#data-collection-api [name=copy-api-info], #data-collection-api [name=copy-api-object], #data-share [name=copy-link-info]");
    $("#data-accordion-collection-api").on('show.bs.collapse', function () {
        $("#data-accordion-collection-api").find("[name=accordion-icon]").removeClass().addClass("fas fa-minus-circle");
    });

    $("#data-accordion-collection-api").on('hide.bs.collapse', function () {
        $("#data-accordion-collection-api").find("[name=accordion-icon]").removeClass().addClass("fas fa-plus-circle");
    });

    $('#data-accordion-collection-api').on('shown.bs.collapse', function () {
        $('html, body').animate({
            scrollTop: $("#data-accordion-collection-api").offset().top
        }, 1000);
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
    // Implement GoTo
    if (app.data.goTo.Search) {
        app.data.searchResult.ajax.readSearch(app.data.goTo.Search);
    } else if (app.data.goTo.PrcCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.System.Navigation.Navigation_API.Search",
            { PrcCode: app.data.goTo.PrcCode, "LngIsoCode": app.label.language.iso.code },
            "app.data.searchResult.callback.readResults",
            { PrcCode: app.data.goTo.PrcCode },
            null,
            null,
            { async: false }
        );
    } else if (app.data.goTo.CprCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.System.Navigation.Navigation_API.Search",
            { CprCode: app.data.goTo.CprCode, "LngIsoCode": app.label.language.iso.code },
            "app.data.searchResult.callback.readResults",
            { CprCode: app.data.goTo.CprCode },
            null,
            null,
            { async: false }
        );
    } else if (app.data.goTo.MtrCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.System.Navigation.Navigation_API.Search",
            { MtrCode: app.data.goTo.MtrCode, "LngIsoCode": app.label.language.iso.code },
            "app.data.searchResult.callback.readResults",
            { MtrCode: app.data.goTo.MtrCode },
            null,
            null,
            { async: false }
        );
    }
});