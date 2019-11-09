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
    app.data.setDataPicker();
    $("#data-accordion-collection-api").show();
    api.content.load("#data-search-row", "entity/data/index.search.html");
    api.content.load("#data-metadata-row", "entity/data/index.metadata.html");
    api.content.load("#data-dataset-row", "entity/data/index.dataset.html");
    api.content.load("#data-dataview-row", "entity/data/index.dataview.html");
    api.content.load("#data-sharethis-row", "entity/data/index.sharethis.html");

    //initiate all copy to clipboard 
    new ClipboardJS("#data-accordion-api [name=copy-api-info], #data-accordion-api [name=copy-api-object], #data-collection-api [name=copy-api-info], #data-collection-api [name=copy-api-object], #data-sharethis [name=copy-link-info]");
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

    $("#data-accordion-api").on('show.bs.collapse', function () {
        $("#data-accordion-api").find("[name=accordion-icon]").removeClass().addClass("fas fa-minus-circle");
    });

    $("#data-accordion-api").on('hide.bs.collapse', function () {
        $("#data-accordion-api").find("[name=accordion-icon]").removeClass().addClass("fas fa-plus-circle");
    });

    $('#data-accordion-api').on('shown.bs.collapse', function () {
        $('html, body').animate({
            scrollTop: $("#data-accordion-api").offset().top
        }, 1000);
    });


    //populate colection api  
    app.data.callback.drawCollectionApiDetails();

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
    // Implement GoTo
    if (app.data.goTo.Search) {
        app.data.metadata.ajax.readSearch(app.data.goTo.Search);
    } else if (app.data.goTo.PrcCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.System.Navigation.Navigation_API.Search",
            { PrcCode: app.data.goTo.PrcCode, "LngIsoCode": app.label.language.iso.code },
            "app.data.metadata.callback.readResults",
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
            "app.data.metadata.callback.readResults",
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
            "app.data.metadata.callback.readResults",
            { MtrCode: app.data.goTo.MtrCode },
            null,
            null,
            { async: false }
        );
    }
});