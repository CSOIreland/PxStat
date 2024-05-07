/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//Load dynamically the ISOGRAM before document ready to avoid race conditions
if (typeof pxWidget === "undefined" || C_APP_URL_PXWIDGET_ISOGRAM.search(pxWidget.root) == -1) {
    jQuery.ajax({
        "url": C_APP_URL_PXWIDGET_ISOGRAM,
        "dataType": "script",
        "async": false,
        "error": function (jqXHR, textStatus, errorThrown) {
            api.modal.exception(app.label.static["api-ajax-exception"]);
        }
    });
};

$(document).ready(function () {
    app.navigation.setLayout(true);
    app.navigation.setBreadcrumb([]);
    app.navigation.setTitle(app.label.static["data"]);
    app.navigation.setState("#nav-link-data");
    app.navigation.setMetaDescription(app.library.html.parseDynamicLabel("meta-description-home", [app.config.organisation]))

    // Read GoTo
    app.data.goTo.Search = history.state.Search || api.content.getParam("Search");
    app.data.goTo.PrcCode = history.state.PrcCode || api.content.getParam("PrcCode");
    app.data.goTo.CprCode = history.state.CprCode || api.content.getParam("CprCode");
    app.data.goTo.MtrCode = history.state.MtrCode || api.content.getParam("MtrCode");
    app.data.goTo[C_APP_GOTO_PARAMS] = api.content.getParam(C_APP_GOTO_PARAMS);

    // Load Modal - must be after GoTo
    api.content.load("#modal-entity", "entity/data/index.modal.html");
    api.content.load("#data-share-row", "entity/data/index.share.html");
    api.content.load("#data-search-row-desktop", "entity/data/index.search.html");
    api.content.load("#data-dataset-row", "entity/data/index.dataset.html");

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
    app.library.bootstrap.getBreakPoint();

    //initiate all copy to clipboard 
    new ClipboardJS("#data-collection-api [name=copy-api-info], #data-collection-api [name=copy-api-object]");

    $('#data-accordion-collection-api').on('shown.bs.collapse', function () {
        $('html, body').animate({
            scrollTop: $("#data-accordion-collection-api").offset().top
        }, 1000);
    });

    // Implement GoTo
    if (app.data.goTo.Search) {
        app.data.searchResult.ajax.readSearch(app.data.goTo.Search);
    } else if (app.data.goTo.PrcCode) {
        //update state
        app.navigation.replaceState("#nav-link-data", {
            "PrcCode": app.data.goTo.PrcCode
        });
        // Run Sharethis.
        app.data.isSearch = false;
        app.data.isCopyrightGoTo = false;
        app.data.PrdCode = app.data.goTo.PrcCode;
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.public,
            "PxStat.System.Navigation.Navigation_API.Search",
            { PrcCode: app.data.goTo.PrcCode, "LngIsoCode": app.label.language.iso.code },
            "app.data.searchResult.callback.readResults",
            { PrcCode: app.data.goTo.PrcCode },
            null,
            null,
            { async: false }
        );
    } else if (app.data.goTo.CprCode) {
        //update state
        app.navigation.replaceState("#nav-link-data", {
            "CprCode": app.data.goTo.CprCode
        });
        app.data.isSearch = false;
        app.data.isCopyrightGoTo = true;
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.public,
            "PxStat.System.Navigation.Navigation_API.Search",
            { CprCode: app.data.goTo.CprCode, "LngIsoCode": app.label.language.iso.code },
            "app.data.searchResult.callback.readResults",
            { CprCode: app.data.goTo.CprCode },
            null,
            null,
            { async: false }
        );
    } else if (app.data.goTo.MtrCode) {
        //update state
        app.navigation.replaceState("#nav-link-data", {
            "MtrCode": app.data.goTo.MtrCode
        });
        app.data.isSearch = false;
        app.data.isCopyrightGoTo = false;
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.public,
            "PxStat.System.Navigation.Navigation_API.Search",
            { MtrCode: app.data.goTo.MtrCode, "LngIsoCode": app.label.language.iso.code },
            "app.data.searchResult.callback.readResults",
            { MtrCode: app.data.goTo.MtrCode },
            null,
            null,
            { async: false }
        );
    }

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});